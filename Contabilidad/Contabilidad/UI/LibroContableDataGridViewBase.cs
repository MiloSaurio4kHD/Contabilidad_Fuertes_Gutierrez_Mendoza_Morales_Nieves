using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Contabilidad.Models;

namespace Contabilidad.UI
{
    /// <summary>
    /// Base comun para las grillas de Libro Diario y Libro de Ajustes: ambas agrupan
    /// las lineas de detalle de cada asiento/ajuste, muestran una fila de glosa combinada
    /// (Numero + Fecha + Cuenta, sin tocar Debe/Haber) y separan cada grupo con una fila en blanco.
    /// </summary>
    public abstract class LibroContableDataGridViewBase : DataGridView
    {
        protected const int ColNumero = 0;
        protected const int ColFecha = 1;
        protected const int ColCuenta = 2;
        protected const int ColDebe = 3;
        protected const int ColHaber = 4;

        protected LibroContableDataGridViewBase(string encabezadoNumero)
        {
            if (GridStyleHelper.EnDisenio) return;

            GridStyleHelper.EstilizarBase(this);
            ConfigurarColumnas(encabezadoNumero);
        }

        private void ConfigurarColumnas(string encabezadoNumero)
        {
            Columns.Clear();
            Columns.Add("colNumero", encabezadoNumero);
            Columns.Add("colFecha", "Fecha");
            Columns.Add("colCuenta", "Cuenta");
            Columns.Add("colDebe", "Debe");
            Columns.Add("colHaber", "Haber");

            Columns[ColNumero].FillWeight = 12;
            Columns[ColFecha].FillWeight = 15;
            Columns[ColCuenta].FillWeight = 43;
            Columns[ColDebe].FillWeight = 15;
            Columns[ColHaber].FillWeight = 15;

            Columns[ColNumero].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Columns[ColFecha].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Columns[ColDebe].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            Columns[ColHaber].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            Columns[ColCuenta].DefaultCellStyle.BackColor = GridStyleHelper.ColorColumnaCuenta;

            GridStyleHelper.DesactivarOrdenamiento(this);
        }

        public void CargarAsientos(List<IAsientoLibro> asientos)
        {
            Rows.Clear();

            for (int a = 0; a < asientos.Count; a++)
            {
                var asiento = asientos[a];

                for (int i = 0; i < asiento.Detalles.Count; i++)
                {
                    var detalle = asiento.Detalles[i];
                    bool esPrimera = i == 0;

                    int idx = Rows.Add(
                        esPrimera ? asiento.NumeroDisplay : string.Empty,
                        esPrimera ? asiento.Fecha.ToString("d/M/yyyy", CultureInfo.InvariantCulture) : string.Empty,
                        detalle.CuentaNombre,
                        detalle.Debe > 0 ? detalle.Debe.ToString("N2", CultureInfo.InvariantCulture) : string.Empty,
                        detalle.Haber > 0 ? detalle.Haber.ToString("N2", CultureInfo.InvariantCulture) : string.Empty);

                    Rows[idx].Tag = new FilaLibroContable
                    {
                        Tipo = TipoFilaLibroContable.Detalle,
                        Asiento = asiento,
                        EsPrimeraFilaDelAsiento = esPrimera
                    };
                }

                int idxGlosa = Rows.Add(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
                Rows[idxGlosa].Tag = new FilaLibroContable { Tipo = TipoFilaLibroContable.Glosa, Asiento = asiento };
                Rows[idxGlosa].Height = 48;

                if (a < asientos.Count - 1)
                {
                    int idxSep = Rows.Add(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
                    Rows[idxSep].Tag = new FilaLibroContable { Tipo = TipoFilaLibroContable.Separador, Asiento = null };
                    Rows[idxSep].Height = 10;
                    Rows[idxSep].DefaultCellStyle.BackColor = Color.White;
                    Rows[idxSep].DefaultCellStyle.SelectionBackColor = Color.White;
                }
            }
        }

        public IAsientoLibro ObtenerAsientoSeleccionado()
        {
            if (CurrentRow == null) return null;
            var fila = CurrentRow.Tag as FilaLibroContable;
            return fila == null ? null : fila.Asiento;
        }

        protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0 || e.RowIndex >= Rows.Count)
            {
                base.OnCellPainting(e);
                return;
            }

            var fila = Rows[e.RowIndex].Tag as FilaLibroContable;
            if (fila == null)
            {
                base.OnCellPainting(e);
                return;
            }

            if (fila.Tipo == TipoFilaLibroContable.Separador)
            {
                e.Graphics.FillRectangle(Brushes.White, e.CellBounds);
                e.Handled = true;
                return;
            }

            if (fila.Tipo == TipoFilaLibroContable.Glosa)
            {
                if (e.ColumnIndex < ColCuenta)
                {
                    // Numero y Fecha: solo relleno, la glosa se dibuja al llegar a la columna Cuenta.
                    e.Graphics.FillRectangle(new SolidBrush(GridStyleHelper.ColorFilaGlosa), e.CellBounds);
                    e.Handled = true;
                    return;
                }

                if (e.ColumnIndex > ColCuenta)
                {
                    // Debe y Haber deben quedar en blanco: nada de la glosa debe tocarlos.
                    e.Graphics.FillRectangle(Brushes.White, e.CellBounds);
                    e.Handled = true;
                    return;
                }

                // Columna Cuenta: aqui se dibuja el "cuadro" combinado que cubre
                // Numero + Fecha + Cuenta, sin invadir Debe ni Haber.
                var rectPrimeraColumna = GetCellDisplayRectangle(ColNumero, e.RowIndex, true);
                var rectCompleto = Rectangle.FromLTRB(rectPrimeraColumna.Left, e.CellBounds.Top, e.CellBounds.Right, e.CellBounds.Bottom);

                e.Graphics.FillRectangle(new SolidBrush(GridStyleHelper.ColorFilaGlosa), rectCompleto);
                using (var lapiz = new Pen(GridStyleHelper.ColorBordes))
                {
                    e.Graphics.DrawRectangle(lapiz, rectCompleto.X, rectCompleto.Y, rectCompleto.Width - 1, rectCompleto.Height - 1);
                }

                string texto = "G: " + fila.Asiento.Glosa;
                var relleno = new Rectangle(rectCompleto.X + 8, rectCompleto.Y + 2, rectCompleto.Width - 16, rectCompleto.Height - 4);
                using (var fuente = new Font(Font, FontStyle.Italic))
                {
                    TextRenderer.DrawText(e.Graphics, texto, fuente, relleno, Color.Black,
                        TextFormatFlags.WordBreak | TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                }

                e.Handled = true;
                return;
            }

            base.OnCellPainting(e);
        }
    }
}
