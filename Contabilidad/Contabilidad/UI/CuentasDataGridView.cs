using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Contabilidad.Models;

namespace Contabilidad.UI
{
    /// <summary>
    /// DataGridView especializado para el catalogo de cuentas (pestaña Configuracion).
    /// </summary>
    public class CuentasDataGridView : DataGridView
    {
        public CuentasDataGridView()
        {
            if (GridStyleHelper.EnDisenio) return;

            GridStyleHelper.EstilizarBase(this);
            ConfigurarColumnas();
        }

        private void ConfigurarColumnas()
        {
            Columns.Clear();
            Columns.Add("colCodigo", "Código");
            Columns.Add("colNombre", "Nombre");
            Columns.Add("colTipo", "Tipo");
            Columns.Add("colNaturaleza", "Naturaleza");

            Columns["colCodigo"].FillWeight = 15;
            Columns["colNombre"].FillWeight = 45;
            Columns["colTipo"].FillWeight = 25;
            Columns["colNaturaleza"].FillWeight = 15;

            Columns["colCodigo"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Columns["colNaturaleza"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            GridStyleHelper.DesactivarOrdenamiento(this);
        }

        public void CargarCuentas(List<Cuenta> cuentas)
        {
            Rows.Clear();
            foreach (var cuenta in cuentas.OrderBy(c => c.Codigo))
            {
                int idx = Rows.Add(
                    cuenta.Codigo,
                    cuenta.Nombre,
                    cuenta.Tipo.ToTextoAmigable(),
                    cuenta.Naturaleza.ToString());
                Rows[idx].Tag = cuenta;
            }
        }

        public Cuenta ObtenerCuentaSeleccionada()
        {
            if (CurrentRow == null) return null;
            return CurrentRow.Tag as Cuenta;
        }
    }
}
