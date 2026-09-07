using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Contabilidad.Models;

namespace Contabilidad.UI
{
    /// <summary>
    /// Representa la "cuenta T" de una sola cuenta contable dentro del Libro Mayor:
    /// columnas N°/Debe a la izquierda, N°/Haber a la derecha, una fila de totales
    /// y una fila final combinada con el saldo (deudor o acreedor).
    /// </summary>
    public class TCuentaDataGridView : DataGridView
    {
        private const int ColNumeroDebe = 0;
        private const int ColDebe = 1;
        private const int ColNumeroHaber = 2;
        private const int ColHaber = 3;

        public TCuentaDataGridView()
        {
            if (GridStyleHelper.EnDisenio) return;

            GridStyleHelper.EstilizarBase(this);
            GridStyleHelper.DesactivarResaltadoSeleccion(this);
            ConfigurarColumnas();
        }

        private void ConfigurarColumnas()
        {
            Columns.Clear();
            Columns.Add("colNumeroDebe", "N°");
            Columns.Add("colDebe", "Debe");
            Columns.Add("colNumeroHaber", "N°");
            Columns.Add("colHaber", "Haber");

            Columns[ColNumeroDebe].FillWeight = 20;
            Columns[ColDebe].FillWeight = 30;
            Columns[ColNumeroHaber].FillWeight = 20;
            Columns[ColHaber].FillWeight = 30;

            Columns[ColNumeroDebe].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Columns[ColNumeroHaber].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Columns[ColDebe].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            Columns[ColHaber].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            ScrollBars = ScrollBars.None;
            ColumnHeadersHeight = 24;
            RowTemplate.Height = 22;

            GridStyleHelper.DesactivarOrdenamiento(this);
        }

        /// <summary>
        /// Carga los movimientos de la cuenta y ajusta la altura del control
        /// al contenido (para poder apilar varias cuentas T sin recortarlas).
        /// </summary>
        public void CargarCuenta(CuentaMayor cuenta)
        {
            Rows.Clear();

            int filas = Math.Max(cuenta.MovimientosDebe.Count, cuenta.MovimientosHaber.Count);
            for (int i = 0; i < filas; i++)
            {
                var debe = i < cuenta.MovimientosDebe.Count ? cuenta.MovimientosDebe[i] : null;
                var haber = i < cuenta.MovimientosHaber.Count ? cuenta.MovimientosHaber[i] : null;

                Rows.Add(
                    debe != null ? debe.NumeroDisplay : string.Empty,
                    debe != null ? debe.Monto.ToString("N2", CultureInfo.InvariantCulture) : string.Empty,
                    haber != null ? haber.NumeroDisplay : string.Empty,
                    haber != null ? haber.Monto.ToString("N2", CultureInfo.InvariantCulture) : string.Empty);
            }

            int idxTotales = Rows.Add("Σ", cuenta.TotalDebe.ToString("N2", CultureInfo.InvariantCulture),
                "Σ", cuenta.TotalHaber.ToString("N2", CultureInfo.InvariantCulture));
            Rows[idxTotales].DefaultCellStyle.Font = new Font(Font, FontStyle.Bold);
            Rows[idxTotales].DefaultCellStyle.BackColor = GridStyleHelper.ColorFilaGlosa;

            int idxSaldo = Rows.Add(string.Empty, string.Empty, string.Empty, string.Empty);
            Rows[idxSaldo].Tag = cuenta;
            Rows[idxSaldo].Height = 26;

            Height = ColumnHeadersHeight + Rows.Cast<DataGridViewRow>().Sum(r => r.Height) + 2;
            ClearSelection();
            CurrentCell = null;
        }

        protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0 || e.RowIndex >= Rows.Count)
            {
                base.OnCellPainting(e);
                return;
            }

            var cuenta = Rows[e.RowIndex].Tag as CuentaMayor;
            if (cuenta == null)
            {
                base.OnCellPainting(e);
                return;
            }

            // Fila de saldo: una sola celda combinada con "Saldo Deudor/Acreedor: monto".
            if (e.ColumnIndex < Columns.Count - 1)
            {
                e.Graphics.FillRectangle(new SolidBrush(GridStyleHelper.ColorEncabezado), e.CellBounds);
                e.Handled = true;
                return;
            }

            var rectPrimeraColumna = GetCellDisplayRectangle(ColNumeroDebe, e.RowIndex, true);
            var rectCompleto = Rectangle.FromLTRB(rectPrimeraColumna.Left, e.CellBounds.Top, e.CellBounds.Right, e.CellBounds.Bottom);

            e.Graphics.FillRectangle(new SolidBrush(GridStyleHelper.ColorEncabezado), rectCompleto);

            string etiqueta = cuenta.EsSaldoDeudor ? "Saldo Deudor" : "Saldo Acreedor";
            decimal monto = cuenta.EsSaldoDeudor ? cuenta.Saldo : -cuenta.Saldo;
            string texto = string.Format("{0}: {1}", etiqueta, monto.ToString("N2", CultureInfo.InvariantCulture));

            using (var fuente = new Font(Font, FontStyle.Bold))
            {
                TextRenderer.DrawText(e.Graphics, texto, fuente, rectCompleto, Color.White,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }

            e.Handled = true;
        }
    }
}
