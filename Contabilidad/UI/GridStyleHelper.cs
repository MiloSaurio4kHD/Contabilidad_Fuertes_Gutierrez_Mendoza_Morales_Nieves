using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Contabilidad.UI
{
    /// <summary>
    /// Estilos comunes reutilizables para los DataGridView de los distintos libros contables.
    /// </summary>
    public static class GridStyleHelper
    {
        /// <summary>
        /// True cuando el control se esta construyendo dentro del diseñador de Visual Studio.
        /// Los grids que arman sus propias columnas en el constructor deben omitir ese armado
        /// en diseño: si no, el diseñador "adopta" esas columnas como si fueran suyas y las
        /// vuelve a escribir en el .Designer.cs, duplicandolas la proxima vez que se ejecuta
        /// (Control.DesignMode no sirve aqui porque no esta disponible dentro del constructor).
        /// </summary>
        public static bool EnDisenio
        {
            get { return LicenseManager.UsageMode == LicenseUsageMode.Designtime; }
        }

        public static readonly Color ColorEncabezado = Color.FromArgb(0x37, 0x56, 0x23);
        public static readonly Color ColorTextoEncabezado = Color.White;
        public static readonly Color ColorColumnaCuenta = Color.FromArgb(0xDC, 0xE6, 0xF1);
        public static readonly Color ColorFilaGlosa = Color.FromArgb(0xF2, 0xF2, 0xF2);
        public static readonly Color ColorBordes = Color.FromArgb(0xB0, 0xB0, 0xB0);

        public static void EstilizarBase(DataGridView dgv)
        {
            dgv.BorderStyle = BorderStyle.Fixed3D;
            dgv.BackgroundColor = Color.White;
            dgv.GridColor = ColorBordes;
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.EnableHeadersVisualStyles = false;

            dgv.ColumnHeadersDefaultCellStyle.BackColor = ColorEncabezado;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = ColorTextoEncabezado;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font(dgv.Font, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgv.ColumnHeadersHeight = 32;

            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0xCC, 0xE4, 0xF7);
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgv.DefaultCellStyle.Padding = new Padding(4, 2, 4, 2);
        }

        /// <summary>
        /// Para grillas puramente informativas (Libro Mayor, Balance de Comprobacion):
        /// no hay ninguna accion atada a "la fila seleccionada", asi que se anula el
        /// resaltado de seleccion para que no se vea una fila/columna pintada sin motivo
        /// (el DataGridView selecciona la primera celda apenas se crea su handle).
        /// </summary>
        public static void DesactivarResaltadoSeleccion(DataGridView dgv)
        {
            dgv.DefaultCellStyle.SelectionBackColor = Color.White;
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = ColorEncabezado;
            dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = ColorTextoEncabezado;
        }

        /// <summary>
        /// Ninguna de estas grillas soporta reordenarse por columna: dependen del orden
        /// original de las filas (asientos agrupados con su glosa, cuentas T con la fila
        /// de saldo al final, el Balance con el TOTAL al final). Si el usuario hace clic
        /// en un encabezado y ordena, todo ese armado se desarma. Se llama despues de
        /// agregar las columnas, porque antes de eso no hay nada que desactivar.
        /// </summary>
        public static void DesactivarOrdenamiento(DataGridView dgv)
        {
            foreach (DataGridViewColumn columna in dgv.Columns)
            {
                columna.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }
    }
}
