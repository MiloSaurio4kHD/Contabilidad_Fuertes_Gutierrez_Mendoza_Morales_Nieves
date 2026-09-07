using System;
using System.Collections.Generic;
using System.Linq;

namespace Contabilidad.Models
{
    public class AsientoContable : IAsientoLibro
    {
        public int Numero { get; set; }
        public DateTime Fecha { get; set; }
        public string Glosa { get; set; }
        public List<DetalleAsiento> Detalles { get; set; }

        public string NumeroDisplay
        {
            get { return Numero.ToString(); }
        }

        public AsientoContable()
        {
            Detalles = new List<DetalleAsiento>();
        }

        public decimal TotalDebe
        {
            get { return Detalles.Sum(d => d.Debe); }
        }

        public decimal TotalHaber
        {
            get { return Detalles.Sum(d => d.Haber); }
        }

        public bool EstaBalanceado
        {
            get { return TotalDebe == TotalHaber && TotalDebe > 0; }
        }
    }
}
