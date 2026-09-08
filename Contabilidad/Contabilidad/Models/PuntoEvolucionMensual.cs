namespace Contabilidad.Models
{
    /// <summary>
    /// Un punto de la tendencia mensual de Ingresos vs Gastos, usado en el grafico
    /// de "Evolucion Mensual" del Dashboard.
    /// </summary>
    public class PuntoEvolucionMensual
    {
        public string Etiqueta { get; set; }
        public decimal Ingresos { get; set; }
        public decimal Gastos { get; set; }
    }
}
