using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using Contabilidad.Models;

namespace Contabilidad.Data
{
    public class CuentaRepository
    {
        private static readonly string CarpetaDatos = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        private static readonly string RutaCatalogoSemilla = Path.Combine(CarpetaDatos, "cuentas_catalogo.json");
        private static readonly string RutaCuentas = Path.Combine(CarpetaDatos, "cuentas.json");

        /// <summary>
        /// Codigos del catalogo semilla que el usuario ya edito (les cambio el codigo).
        /// Se guardan aqui para que AsegurarArchivoInicial() no los vuelva a insertar como
        /// si fueran cuentas nuevas: sin esto, editar el codigo de una cuenta del catalogo
        /// original liberaba su codigo viejo, y en la siguiente lectura la reconciliacion
        /// con el catalogo semilla la volvia a crear (quedaba "duplicada").
        /// </summary>
        private static readonly string RutaCodigosRenombrados = Path.Combine(CarpetaDatos, "cuentas_catalogo_renombrados.json");

        /// <summary>
        /// Devuelve el JSON de cuentas tal cual esta guardado, para poder empaquetarlo
        /// dentro de un archivo de estado sin tener que deserializar y volver a serializar.
        /// </summary>
        public string ObtenerJsonCrudo()
        {
            AsegurarArchivoInicial();
            return File.ReadAllText(RutaCuentas);
        }

        /// <summary>
        /// Reemplaza el archivo de cuentas con el JSON recibido tal cual (por ejemplo,
        /// al restaurar un estado guardado antes). No valida duplicados: se asume que el
        /// JSON ya es valido porque salio de esta misma app.
        /// </summary>
        public void GuardarJsonCrudo(string json)
        {
            Directory.CreateDirectory(CarpetaDatos);
            File.WriteAllText(RutaCuentas, json);
        }

        public List<Cuenta> ObtenerTodas()
        {
            AsegurarArchivoInicial();
            var json = File.ReadAllText(RutaCuentas);
            var arreglo = JsonHelper.ParseArray(json);
            var cuentas = new List<Cuenta>();
            foreach (Dictionary<string, object> item in arreglo)
            {
                cuentas.Add(MapearDesdeDiccionario(item));
            }
            return cuentas.OrderBy(c => c.Codigo).ToList();
        }

        public void Agregar(Cuenta cuenta)
        {
            var cuentas = ObtenerTodas();
            if (cuentas.Any(c => string.Equals(c.Codigo, cuenta.Codigo, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException(string.Format("Ya existe una cuenta con el codigo '{0}'.", cuenta.Codigo));
            cuentas.Add(cuenta);
            Guardar(cuentas);
        }

        public void Actualizar(string codigoOriginal, Cuenta cuenta)
        {
            var cuentas = ObtenerTodas();
            var existente = cuentas.FirstOrDefault(c => string.Equals(c.Codigo, codigoOriginal, StringComparison.OrdinalIgnoreCase));
            if (existente == null)
                throw new InvalidOperationException("La cuenta que intenta editar ya no existe.");

            bool cambioCodigo = !string.Equals(codigoOriginal, cuenta.Codigo, StringComparison.OrdinalIgnoreCase);
            if (cambioCodigo && cuentas.Any(c => string.Equals(c.Codigo, cuenta.Codigo, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException(string.Format("Ya existe una cuenta con el codigo '{0}'.", cuenta.Codigo));

            cuentas.Remove(existente);
            cuentas.Add(cuenta);
            Guardar(cuentas);

            // Si se le cambio el codigo a una cuenta del catalogo original, hay que
            // recordar que ese codigo viejo ya fue "usado" para renombrar, y no dejar
            // que la reconciliacion con el catalogo semilla lo vuelva a crear.
            if (cambioCodigo && EsDelCatalogoOriginal(codigoOriginal))
            {
                MarcarCodigoOriginalComoRenombrado(codigoOriginal);
            }
        }

        /// <summary>
        /// True si el codigo pertenece al catalogo semilla original (Data\cuentas_catalogo.json),
        /// para no dejar eliminar esas cuentas desde la app.
        /// </summary>
        public bool EsDelCatalogoOriginal(string codigo)
        {
            if (!File.Exists(RutaCatalogoSemilla)) return false;

            var semilla = JsonHelper.ParseArray(File.ReadAllText(RutaCatalogoSemilla));
            foreach (Dictionary<string, object> item in semilla)
            {
                if (string.Equals(JsonHelper.ObtenerTexto(item, "codigo"), codigo, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public void Eliminar(string codigo)
        {
            var cuentas = ObtenerTodas();
            var existente = cuentas.FirstOrDefault(c => string.Equals(c.Codigo, codigo, StringComparison.OrdinalIgnoreCase));
            if (existente == null) return;
            cuentas.Remove(existente);
            Guardar(cuentas);
        }

        private void Guardar(List<Cuenta> cuentas)
        {
            var lista = cuentas.Select(MapearADiccionario).ToList();
            var json = JsonHelper.ToJson(lista);
            Directory.CreateDirectory(CarpetaDatos);
            File.WriteAllText(RutaCuentas, json);
        }

        /// <summary>
        /// Garantiza que el archivo de trabajo exista y que contenga, como minimo,
        /// todas las cuentas del catalogo semilla (Data\cuentas_catalogo.json).
        /// Las cuentas que el usuario ya haya creado o editado no se tocan.
        /// </summary>
        private void AsegurarArchivoInicial()
        {
            Directory.CreateDirectory(CarpetaDatos);

            if (!File.Exists(RutaCuentas))
            {
                File.WriteAllText(RutaCuentas, "[]");
            }

            if (!File.Exists(RutaCatalogoSemilla)) return;

            var existentes = JsonHelper.ParseArray(File.ReadAllText(RutaCuentas));
            var codigosExistentes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var cuentas = new List<Dictionary<string, object>>();
            foreach (Dictionary<string, object> item in existentes)
            {
                cuentas.Add(item);
                codigosExistentes.Add(JsonHelper.ObtenerTexto(item, "codigo"));
            }

            var codigosRenombrados = ObtenerCodigosRenombrados();
            var semilla = JsonHelper.ParseArray(File.ReadAllText(RutaCatalogoSemilla));
            bool faltaAlguna = false;
            foreach (Dictionary<string, object> item in semilla)
            {
                var codigo = JsonHelper.ObtenerTexto(item, "codigo");
                if (codigosExistentes.Contains(codigo)) continue;
                if (codigosRenombrados.Contains(codigo)) continue;
                cuentas.Add(item);
                faltaAlguna = true;
            }

            if (faltaAlguna)
            {
                File.WriteAllText(RutaCuentas, JsonHelper.ToJson(cuentas));
            }
        }

        private static HashSet<string> ObtenerCodigosRenombrados()
        {
            var resultado = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (!File.Exists(RutaCodigosRenombrados)) return resultado;

            var texto = File.ReadAllText(RutaCodigosRenombrados);
            if (string.IsNullOrWhiteSpace(texto)) return resultado;

            var serializer = new JavaScriptSerializer();
            var arreglo = serializer.Deserialize<string[]>(texto);
            foreach (var codigo in arreglo) resultado.Add(codigo);
            return resultado;
        }

        private static void MarcarCodigoOriginalComoRenombrado(string codigo)
        {
            var codigos = ObtenerCodigosRenombrados();
            if (!codigos.Add(codigo)) return;

            Directory.CreateDirectory(CarpetaDatos);
            var serializer = new JavaScriptSerializer();
            File.WriteAllText(RutaCodigosRenombrados, serializer.Serialize(codigos.ToList()));
        }

        private static Cuenta MapearDesdeDiccionario(Dictionary<string, object> item)
        {
            return new Cuenta
            {
                Codigo = JsonHelper.ObtenerTexto(item, "codigo"),
                Nombre = JsonHelper.ObtenerTexto(item, "nombre"),
                Tipo = (TipoCuenta)Enum.Parse(typeof(TipoCuenta), JsonHelper.ObtenerTexto(item, "tipo")),
                Elemento = JsonHelper.ObtenerTexto(item, "elemento"),
                Grupo = JsonHelper.ObtenerTexto(item, "grupo"),
                CuentaPadre = JsonHelper.ObtenerTexto(item, "cuentaPadre"),
                Nivel = JsonHelper.ObtenerEntero(item, "nivel", 3),
                Naturaleza = (NaturalezaCuenta)Enum.Parse(typeof(NaturalezaCuenta), JsonHelper.ObtenerTexto(item, "naturaleza"))
            };
        }

        private static Dictionary<string, object> MapearADiccionario(Cuenta cuenta)
        {
            return new Dictionary<string, object>
            {
                { "codigo", cuenta.Codigo },
                { "nombre", cuenta.Nombre },
                { "tipo", cuenta.Tipo.ToString() },
                { "elemento", cuenta.Elemento },
                { "grupo", cuenta.Grupo },
                { "cuentaPadre", cuenta.CuentaPadre },
                { "nivel", cuenta.Nivel },
                { "naturaleza", cuenta.Naturaleza.ToString() }
            };
        }
    }
}
