using System;
using System.Collections.Generic;
using System.Linq;

namespace Contabilidad.Models
{
    /// <summary>
    /// Asiento del Libro de Ajustes. Se numeran como A1, A2, A3... (ver NumeroDisplay)
    /// en vez de numeros simples como el Libro Diario.
    /// </summary>
    public class Ajuste : IAsientoLibro
    {
        public int Numero { get; set; }
        public DateTime Fecha { get; set; }
        public string Glosa { get; set; }
        public List<DetalleAsiento> Detalles { get; set; }

        public string NumeroDisplay
        {
            get { return "A" + Numero; }
        }

        public Ajuste()
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
