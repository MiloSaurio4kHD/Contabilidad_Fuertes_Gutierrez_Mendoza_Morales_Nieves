namespace Contabilidad.UI
{
    /// <summary>
    /// Grilla del Libro de Ajustes (columna de numero "Ajuste N°", ej. A1, A2...).
    /// La logica de armado y pintado esta en LibroContableDataGridViewBase.
    /// </summary>
    public class LibroAjustesDataGridView : LibroContableDataGridViewBase
    {
        public LibroAjustesDataGridView() : base("Ajuste N°")
        {
        }
    }
}
