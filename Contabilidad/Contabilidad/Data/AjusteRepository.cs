using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Contabilidad.Models;

namespace Contabilidad.Data
{
    public class AjusteRepository
    {
        private const string FormatoFecha = "yyyy-MM-dd";
        private static readonly string CarpetaDatos = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        private static readonly string RutaAjustes = Path.Combine(CarpetaDatos, "ajustes.json");

        public List<Ajuste> ObtenerTodos()
        {
            AsegurarArchivoInicial();
            var json = File.ReadAllText(RutaAjustes);
            var arreglo = JsonHelper.ParseArray(json);
            var ajustes = new List<Ajuste>();
            foreach (Dictionary<string, object> item in arreglo)
            {
                ajustes.Add(MapearDesdeDiccionario(item));
            }
            return ajustes.OrderBy(a => a.Numero).ToList();
        }

        /// <summary>
        /// Devuelve el JSON de ajustes tal cual esta guardado, para empaquetarlo dentro
        /// de un archivo de estado sin deserializar y volver a serializar.
        /// </summary>
        public string ObtenerJsonCrudo()
        {
            AsegurarArchivoInicial();
            return File.ReadAllText(RutaAjustes);
        }

        /// <summary>
        /// Reemplaza el archivo de ajustes con el JSON recibido tal cual (restaurar un
        /// estado guardado antes, o vaciarlo con "[]" al eliminar el estado).
        /// </summary>
        public void GuardarJsonCrudo(string json)
        {
            Directory.CreateDirectory(CarpetaDatos);
            File.WriteAllText(RutaAjustes, json);
        }

        /// <summary>
        /// Elimina todos los ajustes del Libro de Ajustes (no toca el catalogo de cuentas).
        /// </summary>
        public void EliminarTodos()
        {
            Guardar(new List<Ajuste>());
        }

        public int ObtenerSiguienteNumero()
        {
            var ajustes = ObtenerTodos();
            return ajustes.Count == 0 ? 1 : ajustes.Max(a => a.Numero) + 1;
        }

        public void Agregar(Ajuste ajuste)
        {
            var ajustes = ObtenerTodos();
            ajustes.Add(ajuste);
            Guardar(ajustes);
        }

        public void Actualizar(int numero, Ajuste ajuste)
        {
            var ajustes = ObtenerTodos();
            var existente = ajustes.FirstOrDefault(a => a.Numero == numero);
            if (existente == null)
                throw new InvalidOperationException("El ajuste que intenta editar ya no existe.");
            ajustes.Remove(existente);
            ajustes.Add(ajuste);
            Guardar(ajustes);
        }

        public void Eliminar(int numero)
        {
            var ajustes = ObtenerTodos();
            var existente = ajustes.FirstOrDefault(a => a.Numero == numero);
            if (existente == null) return;
            ajustes.Remove(existente);
            Guardar(ajustes);
        }

        /// <summary>
        /// True si algun ajuste guardado tiene un detalle que use esa cuenta (para
        /// bloquear el borrado de cuentas en uso desde el catalogo).
        /// </summary>
        public bool TieneMovimientos(string codigoCuenta)
        {
            return ObtenerTodos().Any(a => a.Detalles.Any(d =>
                string.Equals(d.CuentaCodigo, codigoCuenta, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// Actualiza el codigo/nombre de una cuenta en todos los detalles de los ajustes
        /// ya guardados que la referencien. Se usa cuando se edita una cuenta en el
        /// catalogo, para que el Libro de Ajustes (y todo lo que se deriva de el) no
        /// quede con el nombre o codigo viejo. Devuelve true si modifico algun ajuste.
        /// </summary>
        public bool ActualizarCuentaEnDetalles(string codigoOriginal, string codigoNuevo, string nombreNuevo)
        {
            var ajustes = ObtenerTodos();
            bool huboCambios = false;
            foreach (var ajuste in ajustes)
            {
                foreach (var detalle in ajuste.Detalles)
                {
                    if (string.Equals(detalle.CuentaCodigo, codigoOriginal, StringComparison.OrdinalIgnoreCase))
                    {
                        detalle.CuentaCodigo = codigoNuevo;
                        detalle.CuentaNombre = nombreNuevo;
                        huboCambios = true;
                    }
                }
            }
            if (huboCambios) Guardar(ajustes);
            return huboCambios;
        }

        private void Guardar(List<Ajuste> ajustes)
        {
            var lista = ajustes.Select(MapearADiccionario).ToList();
            var json = JsonHelper.ToJson(lista);
            Directory.CreateDirectory(CarpetaDatos);
            File.WriteAllText(RutaAjustes, json);
        }

        private void AsegurarArchivoInicial()
        {
            Directory.CreateDirectory(CarpetaDatos);
            if (!File.Exists(RutaAjustes))
            {
                File.WriteAllText(RutaAjustes, "[]");
            }
        }

        private static Ajuste MapearDesdeDiccionario(Dictionary<string, object> item)
        {
            var ajuste = new Ajuste
            {
                Numero = JsonHelper.ObtenerEntero(item, "numero"),
                Fecha = DateTime.ParseExact(JsonHelper.ObtenerTexto(item, "fecha"), FormatoFecha, CultureInfo.InvariantCulture),
                Glosa = JsonHelper.ObtenerTexto(item, "glosa")
            };

            object detallesObj;
            if (item.TryGetValue("detalles", out detallesObj) && detallesObj is object[])
            {
                foreach (Dictionary<string, object> detalleDict in (object[])detallesObj)
                {
                    ajuste.Detalles.Add(new DetalleAsiento
                    {
                        CuentaCodigo = JsonHelper.ObtenerTexto(detalleDict, "cuentaCodigo"),
                        CuentaNombre = JsonHelper.ObtenerTexto(detalleDict, "cuentaNombre"),
                        Debe = JsonHelper.ObtenerDecimal(detalleDict, "debe"),
                        Haber = JsonHelper.ObtenerDecimal(detalleDict, "haber")
                    });
                }
            }

            return ajuste;
        }

        private static Dictionary<string, object> MapearADiccionario(Ajuste ajuste)
        {
            return new Dictionary<string, object>
            {
                { "numero", ajuste.Numero },
                { "fecha", ajuste.Fecha.ToString(FormatoFecha, CultureInfo.InvariantCulture) },
                { "glosa", ajuste.Glosa },
                { "detalles", ajuste.Detalles.Select(d => new Dictionary<string, object>
                    {
                        { "cuentaCodigo", d.CuentaCodigo },
                        { "cuentaNombre", d.CuentaNombre },
                        { "debe", d.Debe },
                        { "haber", d.Haber }
                    }).ToList()
                }
            };
        }
    }
}
