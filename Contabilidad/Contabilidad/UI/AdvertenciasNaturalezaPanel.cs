using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Contabilidad.UI
{
    /// <summary>
    /// Panel que se coloca debajo del Balance de Comprobacion (ajustado y sin ajustar) o
    /// del Balance General para listar, en texto completo, las advertencias de cuentas con
    /// saldo contrario a su naturaleza contable (ver AdvertenciaSaldoHelper). Permanece
    /// oculto cuando no hay advertencias que mostrar, asi que no ocupa espacio cuando todo
    /// esta normal.
    /// </summary>
    public class AdvertenciasNaturalezaPanel : Panel
    {
        private readonly TextBox _texto;

        public AdvertenciasNaturalezaPanel()
        {
            Dock = DockStyle.Bottom;
            Height = 90;
            BackColor = GridStyleHelper.ColorAdvertenciaFondo;
            Padding = new Padding(8);
            Visible = false;

            _texto = new TextBox();
            _texto.Multiline = true;
            _texto.ReadOnly = true;
            _texto.BorderStyle = BorderStyle.None;
            _texto.ScrollBars = ScrollBars.Vertical;
            _texto.Dock = DockStyle.Fill;
            _texto.BackColor = GridStyleHelper.ColorAdvertenciaFondo;
            _texto.ForeColor = GridStyleHelper.ColorAdvertenciaTexto;
            _texto.Font = new Font(_texto.Font, FontStyle.Bold);
            _texto.TabStop = false;
            Controls.Add(_texto);
        }

        public void Mostrar(IList<string> advertencias)
        {
            if (advertencias == null || advertencias.Count == 0)
            {
                Visible = false;
                _texto.Text = string.Empty;
                return;
            }

            _texto.Text = string.Join(System.Environment.NewLine + System.Environment.NewLine, advertencias);
            Visible = true;
        }
    }
}
