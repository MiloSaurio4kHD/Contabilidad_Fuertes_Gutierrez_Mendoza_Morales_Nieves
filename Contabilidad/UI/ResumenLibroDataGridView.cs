using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace Contabilidad.UI
{
    /// <summary>
    /// Cuadro de resumen (Total Debe, Total Haber, Diferencia, N° de asientos y Estado
    /// Cuadrado/Descuadrado) que se muestra en Libro Diario y Libro Ajustes, con el mismo
    /// estilo verde/blanco que el resto de las grillas de la app. La fila "RESUMEN" no usa
    /// los encabezados de columna: se pinta como una fila normal con las dos celdas del
    /// mismo color, simulando una celda combinada (igual que las secciones de
    /// ReporteFinancieroDataGridView).
    /// </summary>
    public class ResumenLibroDataGridView : DataGridView
    {
        private int _altoContenido;

        public ResumenLibroDataGridView()
        {
            if (GridStyleHelper.EnDisenio) return;

            GridStyleHelper.EstilizarBase(this);
            GridStyleHelper.DesactivarResaltadoSeleccion(this);
            ConfigurarColumnas();
            ConfigurarFilas();
        }

        private void ConfigurarColumnas()
        {
            Columns.Clear();
            Columns.Add("colConcepto", "Concepto");
            Columns.Add("colValor", "Valor");

            Columns["colConcepto"].FillWeight = 40;
            Columns["colValor"].FillWeight = 60;
            Columns["colValor"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            ColumnHeadersVisible = false;
            ScrollBars = ScrollBars.None;
            AllowUserToResizeColumns = false;
            RowTemplate.Height = 24;

            GridStyleHelper.DesactivarOrdenamiento(this);
        }

        private void ConfigurarFilas()
        {
            Rows.Clear();

            int idxTitulo = Rows.Add("RESUMEN", string.Empty);
            Rows[idxTitulo].DefaultCellStyle.BackColor = GridStyleHelper.ColorEncabezado;
            Rows[idxTitulo].DefaultCellStyle.ForeColor = Color.White;
            Rows[idxTitulo].DefaultCellStyle.Font = new Font(Font, FontStyle.Bold);
            Rows[idxTitulo].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Rows[idxTitulo].Height = 28;

            Rows.Add("Total Debe", string.Empty);
            Rows.Add("Total Haber", string.Empty);
            Rows.Add("Diferencia", string.Empty);
            Rows.Add("N° asientos", string.Empty);

            int idxEstado = Rows.Add("Estado:", string.Empty);
            Rows[idxEstado].Cells[1].Style.Font = new Font(Font.FontFamily, 8f, FontStyle.Bold);
            Rows[idxEstado].Cells[1].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            _altoContenido = Rows.Cast<DataGridViewRow>().Sum(r => r.Height) + 4;
            Height = _altoContenido;
        }

        /// <summary>
        /// Recalcula el resumen a partir de los totales del libro (Diario o Ajustes).
        /// La diferencia se muestra como "-" cuando cuadra, igual que un resumen de Excel.
        /// </summary>
        public void Cargar(decimal totalDebe, decimal totalHaber, int cantidadAsientos)
        {
            if (GridStyleHelper.EnDisenio) return;

            // Si en el diseñador lo redimensionaron sin querer al moverlo, se restaura el
            // alto aqui para que no se corte ninguna fila (el ancho si puede variar, las
            // columnas son de tipo Fill).
            Height = _altoContenido;

            decimal diferencia = totalDebe - totalHaber;
            bool cuadrado = diferencia == 0;

            Rows[1].Cells[1].Value = totalDebe.ToString("N2", CultureInfo.InvariantCulture);
            Rows[2].Cells[1].Value = totalHaber.ToString("N2", CultureInfo.InvariantCulture);
            Rows[3].Cells[1].Value = cuadrado ? "-" : diferencia.ToString("N2", CultureInfo.InvariantCulture);
            Rows[4].Cells[1].Value = cantidadAsientos.ToString(CultureInfo.InvariantCulture);

            Rows[5].Cells[1].Value = cuadrado ? "CUADRADO ✓" : "DESCUADRADO ✗";
            Rows[5].Cells[1].Style.ForeColor = cuadrado ? Color.FromArgb(0x1B, 0x7A, 0x1B) : Color.FromArgb(0xB0, 0x20, 0x20);

            ClearSelection();
            CurrentCell = null;
        }
    }
}
