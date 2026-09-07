namespace Contabilidad.Models
{
    /// <summary>
    /// Como debe pintarse una fila dentro de un reporte financiero (Estado de
    /// Resultados o Balance General): titulo de seccion, una cuenta de detalle,
    /// un subtotal de seccion, el total final, o una nota (ej. la ecuacion contable).
    /// </summary>
    public enum TipoFilaReporte
    {
        Seccion,
        Detalle,
        Subtotal,
        Total,
        Nota
    }

    /// <summary>
    /// Una fila del Estado de Resultados o del Balance General, ya lista para mostrar.
    /// </summary>
    public class FilaReporte
    {
        public string Concepto { get; set; }
        public decimal? Monto { get; set; }
        public TipoFilaReporte Tipo { get; set; }

        public FilaReporte(string concepto, decimal? monto, TipoFilaReporte tipo)
        {
            Concepto = concepto;
            Monto = monto;
            Tipo = tipo;
        }
    }
}
