using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Contabilidad.Models;

namespace Contabilidad.UI
{
    /// <summary>
    /// Grilla generica de dos columnas (Concepto / Monto) para mostrar reportes
    /// financieros en cascada: Estado de Resultados y Balance General. El estilo de
    /// cada fila (seccion, detalle, subtotal, total, nota) depende de FilaReporte.Tipo.
    /// </summary>
    public class ReporteFinancieroDataGridView : DataGridView
    {
        public ReporteFinancieroDataGridView()
        {
            if (GridStyleHelper.EnDisenio) return;

            GridStyleHelper.EstilizarBase(this);
            GridStyleHelper.DesactivarResaltadoSeleccion(this);
            ConfigurarColumnas();
        }

        private void ConfigurarColumnas()
        {
            Columns.Clear();
            Columns.Add("colConcepto", "Concepto");
            Columns.Add("colMonto", "Monto");

            Columns["colConcepto"].FillWeight = 70;
            Columns["colMonto"].FillWeight = 30;
            Columns["colMonto"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            GridStyleHelper.DesactivarOrdenamiento(this);
        }

        public void CargarFilas(List<FilaReporte> filas)
        {
            Rows.Clear();

            foreach (var fila in filas)
            {
                string textoMonto = fila.Monto.HasValue
                    ? fila.Monto.Value.ToString("N2", CultureInfo.InvariantCulture)
                    : string.Empty;

                int idx = Rows.Add(fila.Concepto, textoMonto);
                var row = Rows[idx];

                switch (fila.Tipo)
                {
                    case TipoFilaReporte.Seccion:
                        row.DefaultCellStyle.BackColor = GridStyleHelper.ColorEncabezado;
                        row.DefaultCellStyle.ForeColor = Color.White;
                        row.DefaultCellStyle.Font = new Font(Font, FontStyle.Bold);
                        break;

                    case TipoFilaReporte.Subtotal:
                        row.DefaultCellStyle.BackColor = GridStyleHelper.ColorFilaGlosa;
                        row.DefaultCellStyle.Font = new Font(Font, FontStyle.Bold);
                        break;

                    case TipoFilaReporte.Total:
                        row.DefaultCellStyle.BackColor = GridStyleHelper.ColorColumnaCuenta;
                        row.DefaultCellStyle.Font = new Font(Font, FontStyle.Bold);
                        break;

                    case TipoFilaReporte.Nota:
                        row.DefaultCellStyle.Font = new Font(Font, FontStyle.Italic);
                        break;

                    case TipoFilaReporte.Advertencia:
                        row.DefaultCellStyle.BackColor = GridStyleHelper.ColorAdvertenciaFondo;
                        row.DefaultCellStyle.ForeColor = GridStyleHelper.ColorAdvertenciaTexto;
                        row.DefaultCellStyle.Font = new Font(Font, FontStyle.Bold);
                        break;
                }
            }

            ClearSelection();
            CurrentCell = null;
        }
    }
}
