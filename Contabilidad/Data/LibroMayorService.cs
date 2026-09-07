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

        public List<CuentaMayor> ObtenerLibroMayor()
        {
            var porCodigo = new Dictionary<string, CuentaMayor>(StringComparer.OrdinalIgnoreCase);

            void Procesar(IEnumerable<IAsientoLibro> asientos)
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

            Procesar(_libroDiarioRepository.ObtenerTodos().Cast<IAsientoLibro>());
            Procesar(_ajusteRepository.ObtenerTodos().Cast<IAsientoLibro>());

            return porCodigo.Values.OrderBy(c => c.Codigo).ToList();
        }
    }
}
