using System;
using System.Collections.Generic;
using System.Linq;
using Contabilidad.Models;

namespace Contabilidad.Data
{
    /// <summary>
    /// Calcula el Libro Mayor (cuentas T) y sirve de base para el Balance de Comprobacion,
    /// combinando los movimientos del Libro Diario y del Libro de Ajustes.
    /// No persiste nada: siempre se recalcula a partir de los otros repositorios.
    /// </summary>
    public class LibroMayorService
    {
        private readonly LibroDiarioRepository _libroDiarioRepository = new LibroDiarioRepository();
        private readonly AjusteRepository _ajusteRepository = new AjusteRepository();

        /// <summary>Libro Mayor ajustado: combina Libro Diario y Libro de Ajustes.</summary>
        public List<CuentaMayor> ObtenerLibroMayor()
        {
            return Calcular(
                _libroDiarioRepository.ObtenerTodos().Cast<IAsientoLibro>(),
                _ajusteRepository.ObtenerTodos().Cast<IAsientoLibro>());
        }

        /// <summary>Libro Mayor sin ajustar: solo los movimientos del Libro Diario.</summary>
        public List<CuentaMayor> ObtenerLibroMayorSinAjustar()
        {
            return Calcular(_libroDiarioRepository.ObtenerTodos().Cast<IAsientoLibro>());
        }

        private static List<CuentaMayor> Calcular(params IEnumerable<IAsientoLibro>[] gruposDeAsientos)
        {
            var porCodigo = new Dictionary<string, CuentaMayor>(StringComparer.OrdinalIgnoreCase);

            foreach (var asientos in gruposDeAsientos)
            {
                foreach (var asiento in asientos)
                {
                    foreach (var detalle in asiento.Detalles)
                    {
                        CuentaMayor cuentaMayor;
                        if (!porCodigo.TryGetValue(detalle.CuentaCodigo, out cuentaMayor))
                        {
                            cuentaMayor = new CuentaMayor { Codigo = detalle.CuentaCodigo, Nombre = detalle.CuentaNombre };
                            porCodigo[detalle.CuentaCodigo] = cuentaMayor;
                        }

                        if (detalle.Debe > 0)
                        {
                            cuentaMayor.MovimientosDebe.Add(new MovimientoMayor { NumeroDisplay = asiento.NumeroDisplay, Monto = detalle.Debe });
                        }
                        if (detalle.Haber > 0)
                        {
                            cuentaMayor.MovimientosHaber.Add(new MovimientoMayor { NumeroDisplay = asiento.NumeroDisplay, Monto = detalle.Haber });
                        }
                    }
                }
            }

            return porCodigo.Values.OrderBy(c => c.Codigo).ToList();
        }
    }
}
