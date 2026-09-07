using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Contabilidad.Models;

namespace Contabilidad.UI
{
    /// <summary>
    /// Grilla del Balance de Comprobacion: cuenta, sumas de debe/haber y saldos
    /// deudor/acreedor, con una fila de totales al final.
    /// </summary>
    public class BalanceComprobacionDataGridView : DataGridView
    {
        public BalanceComprobacionDataGridView()
        {
            if (GridStyleHelper.EnDisenio) return;

            GridStyleHelper.EstilizarBase(this);
            GridStyleHelper.DesactivarResaltadoSeleccion(this);
            ConfigurarColumnas();
        }

        private void ConfigurarColumnas()
        {
            Columns.Clear();
            Columns.Add("colCuenta", "Cuenta");
            Columns.Add("colSumaDebe", "Suma Debe");
            Columns.Add("colSumaHaber", "Suma Haber");
            Columns.Add("colSaldoDeudor", "Saldo Deudor");
            Columns.Add("colSaldoAcreedor", "Saldo Acreedor");

            Columns["colCuenta"].FillWeight = 35;
            Columns["colSumaDebe"].FillWeight = 16;
            Columns["colSumaHaber"].FillWeight = 16;
            Columns["colSaldoDeudor"].FillWeight = 16;
            Columns["colSaldoAcreedor"].FillWeight = 16;

            foreach (DataGridViewColumn columna in Columns)
            {
                if (columna.Name != "colCuenta")
                {
                    columna.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
            }

            GridStyleHelper.DesactivarOrdenamiento(this);
        }

        public void CargarCuentas(System.Collections.Generic.List<CuentaMayor> cuentas)
        {
            Rows.Clear();

            foreach (var cuenta in cuentas)
            {
                Rows.Add(
                    cuenta.CodigoNombre,
                    cuenta.TotalDebe.ToString("N2", CultureInfo.InvariantCulture),
                    cuenta.TotalHaber.ToString("N2", CultureInfo.InvariantCulture),
                    cuenta.SaldoDeudor > 0 ? cuenta.SaldoDeudor.ToString("N2", CultureInfo.InvariantCulture) : string.Empty,
                    cuenta.SaldoAcreedor > 0 ? cuenta.SaldoAcreedor.ToString("N2", CultureInfo.InvariantCulture) : string.Empty);
            }

            int idxTotal = Rows.Add(
                "TOTAL",
                cuentas.Sum(c => c.TotalDebe).ToString("N2", CultureInfo.InvariantCulture),
                cuentas.Sum(c => c.TotalHaber).ToString("N2", CultureInfo.InvariantCulture),
                cuentas.Sum(c => c.SaldoDeudor).ToString("N2", CultureInfo.InvariantCulture),
                cuentas.Sum(c => c.SaldoAcreedor).ToString("N2", CultureInfo.InvariantCulture));
            Rows[idxTotal].DefaultCellStyle.Font = new Font(Font, FontStyle.Bold);
            Rows[idxTotal].DefaultCellStyle.BackColor = GridStyleHelper.ColorFilaGlosa;

            ClearSelection();
            CurrentCell = null;
        }
    }
}
