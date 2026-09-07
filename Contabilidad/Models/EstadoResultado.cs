using System.Collections.Generic;

namespace Contabilidad.Models
{
    /// <summary>
    /// Resultado del calculo del Estado de Resultados: ingresos y gastos con
    /// movimiento en el Libro Mayor, y la cascada de utilidad (participacion
    /// de trabajadores 15% e impuesto a la renta 25%) hasta la utilidad del ejercicio.
    /// </summary>
    public class EstadoResultado
    {
        public List<LineaCuentaMonto> Ingresos { get; set; }
        public decimal TotalIngresos { get; set; }

        public List<LineaCuentaMonto> Gastos { get; set; }
        public decimal TotalGastos { get; set; }

        public decimal UtilidadAntesParticipacion { get; set; }
        public decimal Participacion15 { get; set; }
        public decimal UtilidadAntesImpuesto { get; set; }
        public decimal ImpuestoRenta25 { get; set; }
        public decimal UtilidadEjercicio { get; set; }

        public EstadoResultado()
        {
            Ingresos = new List<LineaCuentaMonto>();
            Gastos = new List<LineaCuentaMonto>();
        }
    }
}
