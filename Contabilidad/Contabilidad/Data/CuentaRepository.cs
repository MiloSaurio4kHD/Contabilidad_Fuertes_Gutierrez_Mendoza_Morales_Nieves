using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Contabilidad.Models;

namespace Contabilidad.Data
{
    public class CuentaRepository
    {
        private static readonly string CarpetaDatos = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        private static readonly string RutaCatalogoSemilla = Path.Combine(CarpetaDatos, "cuentas_catalogo.json");
        private static readonly string RutaCuentas = Path.Combine(CarpetaDatos, "cuentas.json");

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

            if (!string.Equals(codigoOriginal, cuenta.Codigo, StringComparison.OrdinalIgnoreCase) &&
                cuentas.Any(c => string.Equals(c.Codigo, cuenta.Codigo, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException(string.Format("Ya existe una cuenta con el codigo '{0}'.", cuenta.Codigo));

            cuentas.Remove(existente);
            cuentas.Add(cuenta);
            Guardar(cuentas);
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

            var semilla = JsonHelper.ParseArray(File.ReadAllText(RutaCatalogoSemilla));
            bool faltaAlguna = false;
            foreach (Dictionary<string, object> item in semilla)
            {
                var codigo = JsonHelper.ObtenerTexto(item, "codigo");
                if (codigosExistentes.Contains(codigo)) continue;
                cuentas.Add(item);
                faltaAlguna = true;
            }

            if (faltaAlguna)
            {
                File.WriteAllText(RutaCuentas, JsonHelper.ToJson(cuentas));
            }
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
