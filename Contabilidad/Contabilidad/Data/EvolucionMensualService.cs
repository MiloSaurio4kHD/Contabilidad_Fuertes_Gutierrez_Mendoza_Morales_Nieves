using System;
using System.Collections.Generic;
using System.Linq;
using Contabilidad.Models;

namespace Contabilidad.Data
{
    /// <summary>
    /// Calcula la tendencia mensual de Ingresos vs Gastos a partir del Libro Diario
    /// (no incluye ajustes: estos suelen registrarse todos en la misma fecha de
    /// cierre y distorsionarian la tendencia operativa mes a mes).
    /// </summary>
    public class EvolucionMensualService
    {
        private static readonly string[] MesesAbreviados =
        {
            "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic"
        };

        private readonly LibroDiarioRepository _libroDiarioRepository = new LibroDiarioRepository();
        private readonly CuentaRepository _cuentaRepository = new CuentaRepository();

        public List<PuntoEvolucionMensual> ObtenerUltimosMeses(int cantidadMeses = 6)
        {
            var catalogo = _cuentaRepository.ObtenerTodas()
                .GroupBy(c => c.Codigo, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            var porMes = new Dictionary<DateTime, PuntoEvolucionMensual>();

            foreach (var asiento in _libroDiarioRepository.ObtenerTodos())
            {
                var clave = new DateTime(asiento.Fecha.Year, asiento.Fecha.Month, 1);
                PuntoEvolucionMensual punto;
                if (!porMes.TryGetValue(clave, out punto))
                {
                    punto = new PuntoEvolucionMensual { Etiqueta = MesesAbreviados[clave.Month - 1] + " " + clave.Year };
                    porMes[clave] = punto;
                }

                foreach (var detalle in asiento.Detalles)
                {
                    Cuenta cuenta;
                    if (!catalogo.TryGetValue(detalle.CuentaCodigo, out cuenta)) continue;

                    if (cuenta.Tipo == TipoCuenta.Ingresos)
                    {
                        punto.Ingresos += detalle.Haber - detalle.Debe;
                    }
                    else if (cuenta.Tipo == TipoCuenta.Gastos)
                    {
                        punto.Gastos += detalle.Debe - detalle.Haber;
                    }
                }
            }

            var ordenados = porMes.OrderBy(kv => kv.Key).Select(kv => kv.Value).ToList();
            int desde = Math.Max(0, ordenados.Count - cantidadMeses);
            return ordenados.Skip(desde).ToList();
        }
    }
}
