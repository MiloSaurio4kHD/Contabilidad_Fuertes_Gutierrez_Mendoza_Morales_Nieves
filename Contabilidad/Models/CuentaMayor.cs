using System.Collections.Generic;
using System.Linq;

namespace Contabilidad.Models
{
    /// <summary>
    /// Un movimiento individual dentro de un lado (Debe o Haber) de una cuenta del Libro Mayor.
    /// </summary>
    public class MovimientoMayor
    {
        public string NumeroDisplay { get; set; }
        public decimal Monto { get; set; }
    }

    /// <summary>
    /// Representa la "cuenta T" de una cuenta contable en el Libro Mayor: todos sus
    /// movimientos de Debe y de Haber (provenientes del Libro Diario y del Libro de Ajustes),
    /// con sus totales y su saldo.
    /// </summary>
    public class CuentaMayor
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public List<MovimientoMayor> MovimientosDebe { get; set; }
        public List<MovimientoMayor> MovimientosHaber { get; set; }

        public CuentaMayor()
        {
            MovimientosDebe = new List<MovimientoMayor>();
            MovimientosHaber = new List<MovimientoMayor>();
        }

        public string CodigoNombre
        {
            get { return string.Format("{0} - {1}", Codigo, Nombre); }
        }

        public decimal TotalDebe
        {
            get { return MovimientosDebe.Sum(m => m.Monto); }
        }

        public decimal TotalHaber
        {
            get { return MovimientosHaber.Sum(m => m.Monto); }
        }

        public decimal Saldo
        {
            get { return TotalDebe - TotalHaber; }
        }

        public bool EsSaldoDeudor
        {
            get { return Saldo >= 0; }
        }

        public decimal SaldoDeudor
        {
            get { return EsSaldoDeudor ? Saldo : 0m; }
        }

        public decimal SaldoAcreedor
        {
            get { return EsSaldoDeudor ? 0m : -Saldo; }
        }
    }
}
