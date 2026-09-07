using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Contabilidad.Models;

namespace Contabilidad.Data
{
    public class LibroDiarioRepository
    {
        private const string FormatoFecha = "yyyy-MM-dd";
        private static readonly string CarpetaDatos = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        private static readonly string RutaAsientos = Path.Combine(CarpetaDatos, "asientos.json");

        public List<AsientoContable> ObtenerTodos()
        {
            AsegurarArchivoInicial();
            var json = File.ReadAllText(RutaAsientos);
            var arreglo = JsonHelper.ParseArray(json);
            var asientos = new List<AsientoContable>();
            foreach (Dictionary<string, object> item in arreglo)
            {
                asientos.Add(MapearDesdeDiccionario(item));
            }
            return asientos.OrderBy(a => a.Numero).ToList();
        }

        public int ObtenerSiguienteNumero()
        {
            var asientos = ObtenerTodos();
            return asientos.Count == 0 ? 1 : asientos.Max(a => a.Numero) + 1;
        }

        public void Agregar(AsientoContable asiento)
        {
            var asientos = ObtenerTodos();
            asientos.Add(asiento);
            Guardar(asientos);
        }

        public void Actualizar(int numero, AsientoContable asiento)
        {
            var asientos = ObtenerTodos();
            var existente = asientos.FirstOrDefault(a => a.Numero == numero);
            if (existente == null)
                throw new InvalidOperationException("El asiento que intenta editar ya no existe.");
            asientos.Remove(existente);
            asientos.Add(asiento);
            Guardar(asientos);
        }

        public void Eliminar(int numero)
        {
            var asientos = ObtenerTodos();
            var existente = asientos.FirstOrDefault(a => a.Numero == numero);
            if (existente == null) return;
            asientos.Remove(existente);
            Guardar(asientos);
        }

        private void Guardar(List<AsientoContable> asientos)
        {
            var lista = asientos.Select(MapearADiccionario).ToList();
            var json = JsonHelper.ToJson(lista);
            Directory.CreateDirectory(CarpetaDatos);
            File.WriteAllText(RutaAsientos, json);
        }

        private void AsegurarArchivoInicial()
        {
            Directory.CreateDirectory(CarpetaDatos);
            if (!File.Exists(RutaAsientos))
            {
                File.WriteAllText(RutaAsientos, "[]");
            }
        }

        private static AsientoContable MapearDesdeDiccionario(Dictionary<string, object> item)
        {
            var asiento = new AsientoContable
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
                    asiento.Detalles.Add(new DetalleAsiento
                    {
                        CuentaCodigo = JsonHelper.ObtenerTexto(detalleDict, "cuentaCodigo"),
                        CuentaNombre = JsonHelper.ObtenerTexto(detalleDict, "cuentaNombre"),
                        Debe = JsonHelper.ObtenerDecimal(detalleDict, "debe"),
                        Haber = JsonHelper.ObtenerDecimal(detalleDict, "haber")
                    });
                }
            }

            return asiento;
        }

        private static Dictionary<string, object> MapearADiccionario(AsientoContable asiento)
        {
            return new Dictionary<string, object>
            {
                { "numero", asiento.Numero },
                { "fecha", asiento.Fecha.ToString(FormatoFecha, CultureInfo.InvariantCulture) },
                { "glosa", asiento.Glosa },
                { "detalles", asiento.Detalles.Select(d => new Dictionary<string, object>
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
