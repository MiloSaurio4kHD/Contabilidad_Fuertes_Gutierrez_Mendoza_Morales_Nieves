using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Contabilidad.Models;

namespace Contabilidad.UI
{
    /// <summary>
    /// Grilla del Balance de Comprobacion (se usa tanto para el ajustado como para el sin
    /// ajustar): cuenta, sumas de debe/haber y saldos deudor/acreedor, con una fila de
    /// totales al final.
    /// </summary>
    public class BalanceComprobacionDataGridView : DataGridView
    {
        private readonly List<string> _advertencias = new List<string>();

        /// <summary>
        /// Mensajes de advertencia (ver AdvertenciaSaldoHelper) de las cuentas cuyo saldo,
        /// en la ultima llamada a CargarCuentas, resulto contrario a su naturaleza contable
        /// esperada. Se recalcula por completo en cada llamada.
        /// </summary>
        public IList<string> Advertencias
        {
            get { return _advertencias; }
        }

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

        /// <param name="cuentas">Cuentas del Libro Mayor a mostrar.</param>
        /// <param name="catalogo">
        /// Plan de cuentas por codigo, para poder comparar la naturaleza configurada de cada
        /// cuenta contra su saldo calculado y advertir si son contrarias. Si es null, no se
        /// hace ninguna deteccion (la grilla se comporta como antes).
        /// </param>
        public void CargarCuentas(List<CuentaMayor> cuentas, Dictionary<string, Cuenta> catalogo)
        {
            Rows.Clear();
            _advertencias.Clear();

            foreach (var cuenta in cuentas)
            {
                int idx = Rows.Add(
                    cuenta.CodigoNombre,
                    cuenta.TotalDebe.ToString("N2", CultureInfo.InvariantCulture),
                    cuenta.TotalHaber.ToString("N2", CultureInfo.InvariantCulture),
                    cuenta.SaldoDeudor > 0 ? cuenta.SaldoDeudor.ToString("N2", CultureInfo.InvariantCulture) : string.Empty,
                    cuenta.SaldoAcreedor > 0 ? cuenta.SaldoAcreedor.ToString("N2", CultureInfo.InvariantCulture) : string.Empty);

                Cuenta cuentaCatalogo;
                if (catalogo == null || !catalogo.TryGetValue(cuenta.Codigo, out cuentaCatalogo)) continue;

                string advertencia = AdvertenciaSaldoHelper.Detectar(cuentaCatalogo, cuenta.EsSaldoDeudor,
                    cuenta.EsSaldoDeudor ? cuenta.SaldoDeudor : cuenta.SaldoAcreedor);
                if (advertencia == null) continue;

                _advertencias.Add(advertencia);
                Rows[idx].DefaultCellStyle.BackColor = GridStyleHelper.ColorAdvertenciaFondo;
                Rows[idx].DefaultCellStyle.ForeColor = GridStyleHelper.ColorAdvertenciaTexto;
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
