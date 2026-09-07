using System;
using System.Collections.Generic;
using System.Linq;
using Contabilidad.Models;

namespace Contabilidad.Data
{
    /// <summary>
    /// Calcula el Estado de Resultados a partir del Libro Mayor: suma los ingresos
    /// y gastos con movimiento, y calcula la cascada de utilidad (15% participacion
    /// de trabajadores, 25% impuesto a la renta) hasta la utilidad del ejercicio.
    /// No persiste nada: siempre se recalcula.
    /// </summary>
    public class EstadoResultadoService
    {
        private readonly LibroMayorService _libroMayorService = new LibroMayorService();
        private readonly CuentaRepository _cuentaRepository = new CuentaRepository();

        public EstadoResultado Generar()
        {
            var cuentasMayor = _libroMayorService.ObtenerLibroMayor();
            var catalogo = _cuentaRepository.ObtenerTodas()
                .GroupBy(c => c.Codigo, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            var resultado = new EstadoResultado();

            foreach (var cuentaMayor in cuentasMayor.OrderBy(c => c.Codigo))
            {
                Cuenta cuenta;
                if (!catalogo.TryGetValue(cuentaMayor.Codigo, out cuenta)) continue;

                if (cuenta.Tipo == TipoCuenta.Ingresos)
                {
                    decimal monto = cuentaMayor.TotalHaber - cuentaMayor.TotalDebe;
                    if (monto == 0) continue;
                    resultado.Ingresos.Add(new LineaCuentaMonto { Nombre = cuenta.Nombre, Monto = monto });
                }
                else if (cuenta.Tipo == TipoCuenta.Gastos)
                {
                    decimal monto = cuentaMayor.TotalDebe - cuentaMayor.TotalHaber;
                    if (monto == 0) continue;
                    resultado.Gastos.Add(new LineaCuentaMonto { Nombre = cuenta.Nombre, Monto = monto });
                }
            }

            resultado.TotalIngresos = resultado.Ingresos.Sum(l => l.Monto);
            resultado.TotalGastos = resultado.Gastos.Sum(l => l.Monto);
            resultado.UtilidadAntesParticipacion = resultado.TotalIngresos - resultado.TotalGastos;

            if (resultado.UtilidadAntesParticipacion > 0)
            {
                resultado.Participacion15 = Math.Round(resultado.UtilidadAntesParticipacion * 0.15m, 2, MidpointRounding.AwayFromZero);
                resultado.UtilidadAntesImpuesto = resultado.UtilidadAntesParticipacion - resultado.Participacion15;
                resultado.ImpuestoRenta25 = Math.Round(resultado.UtilidadAntesImpuesto * 0.25m, 2, MidpointRounding.AwayFromZero);
                resultado.UtilidadEjercicio = resultado.UtilidadAntesImpuesto - resultado.ImpuestoRenta25;
            }
            else
            {
                resultado.Participacion15 = 0;
                resultado.UtilidadAntesImpuesto = resultado.UtilidadAntesParticipacion;
                resultado.ImpuestoRenta25 = 0;
                resultado.UtilidadEjercicio = resultado.UtilidadAntesParticipacion;
            }

            return resultado;
        }
    }
}
