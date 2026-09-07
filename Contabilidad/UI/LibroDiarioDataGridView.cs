namespace Contabilidad.UI
{
    /// <summary>
    /// Grilla del Libro Diario (columna de numero "Asiento N°").
    /// La logica de armado y pintado esta en LibroContableDataGridViewBase.
    /// </summary>
    public class LibroDiarioDataGridView : LibroContableDataGridViewBase
    {
        public LibroDiarioDataGridView() : base("Asiento N°")
        {
        }
    }
}
