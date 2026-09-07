using System.Drawing;
using System.Windows.Forms;
using Contabilidad.Models;

namespace Contabilidad.UI
{
    /// <summary>
    /// Encabezado con el nombre de la cuenta + su cuenta T (TCuentaDataGridView).
    /// Se usa para apilar varias cuentas dentro del panel del Libro Mayor.
    /// </summary>
    public class TCuentaPanel : Panel
    {
        private readonly Label _lblCuenta;
        private readonly TCuentaDataGridView _grid;

        public TCuentaPanel()
        {
            BorderStyle = BorderStyle.FixedSingle;

            _lblCuenta = new Label
            {
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = GridStyleHelper.ColorEncabezado,
                ForeColor = Color.White,
                Font = new Font(Font, FontStyle.Bold)
            };

            _grid = new TCuentaDataGridView();

            Controls.Add(_grid);
            Controls.Add(_lblCuenta);
        }

        public void Cargar(CuentaMayor cuenta, int ancho)
        {
            const int alturaEncabezado = 26;

            _lblCuenta.Text = cuenta.CodigoNombre;
            _lblCuenta.SetBounds(0, 0, ancho, alturaEncabezado);

            _grid.Width = ancho;
            _grid.CargarCuenta(cuenta);
            _grid.SetBounds(0, alturaEncabezado, ancho, _grid.Height);

            Width = ancho;
            Height = alturaEncabezado + _grid.Height + 2;
        }
    }
}
