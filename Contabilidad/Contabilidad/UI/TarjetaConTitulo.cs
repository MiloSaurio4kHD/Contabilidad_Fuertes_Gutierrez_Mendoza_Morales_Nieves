using System.Drawing;
using System.Windows.Forms;

namespace Contabilidad.UI
{
    /// <summary>
    /// Card reutilizable para el Dashboard: una franja superior verde oscuro con el
    /// titulo en blanco, y un area de contenido debajo (ContentPanel) donde cada
    /// tarjeta mete su grafico o control. Mismo estilo verde/blanco del resto de la app.
    /// </summary>
    public class TarjetaConTitulo : Panel
    {
        public Panel ContentPanel { get; private set; }

        public TarjetaConTitulo(string titulo)
        {
            if (GridStyleHelper.EnDisenio) return;

            BorderStyle = BorderStyle.FixedSingle;
            BackColor = Color.White;

            var franjaTitulo = new Label
            {
                Text = titulo,
                Dock = DockStyle.Top,
                Height = 28,
                BackColor = GridStyleHelper.ColorEncabezado,
                ForeColor = Color.White,
                Font = new Font(Font, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(8, 0, 0, 0)
            };

            ContentPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };

            Controls.Add(ContentPanel);
            Controls.Add(franjaTitulo);
        }
    }
}
