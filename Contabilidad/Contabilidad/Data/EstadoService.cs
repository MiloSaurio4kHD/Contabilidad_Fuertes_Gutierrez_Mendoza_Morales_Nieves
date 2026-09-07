using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;

namespace Contabilidad.Data
{
    /// <summary>
    /// Guarda y restaura el "estado" completo de la app (cuentas, asientos y ajustes)
    /// en un unico archivo JSON, y permite borrar asientos/ajustes sin tocar el catalogo
    /// de cuentas. No revalida nada: los tres JSON ya son validos porque salen de los
    /// mismos repositorios que los generaron.
    /// </summary>
    public class EstadoService
    {
        private readonly CuentaRepository _cuentaRepository = new CuentaRepository();
        private readonly LibroDiarioRepository _libroDiarioRepository = new LibroDiarioRepository();
        private readonly AjusteRepository _ajusteRepository = new AjusteRepository();

        /// <summary>
        /// Empaqueta cuentas, asientos y ajustes (cada uno ya es un arreglo JSON valido)
        /// dentro de un solo objeto JSON, por concatenacion directa de texto.
        /// </summary>
        public void Guardar(string ruta)
        {
            string cuentasJson = _cuentaRepository.ObtenerJsonCrudo();
            string asientosJson = _libroDiarioRepository.ObtenerJsonCrudo();
            string ajustesJson = _ajusteRepository.ObtenerJsonCrudo();

            string combinado = "{\"cuentas\":" + cuentasJson + ",\"asientos\":" + asientosJson + ",\"ajustes\":" + ajustesJson + "}";
            File.WriteAllText(ruta, combinado);
        }

        /// <summary>
        /// Lee un archivo de estado guardado con Guardar() y reemplaza cuentas.json,
        /// asientos.json y ajustes.json con su contenido.
        /// </summary>
        public void Cargar(string ruta)
        {
            var serializer = new JavaScriptSerializer();
            string contenido = File.ReadAllText(ruta);

            var raiz = serializer.DeserializeObject(contenido) as Dictionary<string, object>;
            if (raiz == null || !raiz.ContainsKey("cuentas") || !raiz.ContainsKey("asientos") || !raiz.ContainsKey("ajustes"))
            {
                throw new InvalidOperationException("El archivo seleccionado no tiene el formato de un estado guardado por esta app.");
            }

            _cuentaRepository.GuardarJsonCrudo(serializer.Serialize(raiz["cuentas"]));
            _libroDiarioRepository.GuardarJsonCrudo(serializer.Serialize(raiz["asientos"]));
            _ajusteRepository.GuardarJsonCrudo(serializer.Serialize(raiz["ajustes"]));
        }

        /// <summary>
        /// Elimina todos los asientos y ajustes. El catalogo de cuentas no se toca.
        /// </summary>
        public void EliminarAsientosYAjustes()
        {
            _libroDiarioRepository.EliminarTodos();
            _ajusteRepository.EliminarTodos();
        }
    }
}
