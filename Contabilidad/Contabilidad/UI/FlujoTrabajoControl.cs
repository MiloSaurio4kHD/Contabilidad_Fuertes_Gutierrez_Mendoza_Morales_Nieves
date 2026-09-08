using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Contabilidad.UI
{
    /// <summary>
    /// Una etapa del ciclo contable mostrada en el FlujoTrabajoControl: un icono,
    /// una etiqueta debajo, y un contador opcional (por ejemplo cuantos asientos hay)
    /// que se muestra como una segunda linea de texto gris debajo de la etiqueta
    /// cuando es mayor a 0 (a proposito no es una insignia roja, para que no parezca
    /// una notificacion pendiente).
    /// </summary>
    public class EtapaFlujo
    {
        public string Etiqueta { get; set; }
        public string Icono { get; set; }
        public int Contador { get; set; }
    }

    /// <summary>
    /// Dibuja el ciclo contable (Diario -> Ajustes -> Mayor -> Balances -> Reportes)
    /// como una linea de tiempo horizontal: circulos conectados con el icono de cada
    /// modulo, una insignia con el conteo cuando aplica, y la etiqueta debajo. Cada
    /// circulo es clickeable (EtapaClick) para saltar directo a esa pestaña.
    /// </summary>
    public class FlujoTrabajoControl : Panel
    {
        private const int RadioCirculo = 26;

        private EtapaFlujo[] _etapas = new EtapaFlujo[0];
        private readonly List<Rectangle> _rectangulosEtapas = new List<Rectangle>();

        public event Action<int> EtapaClick;

        public FlujoTrabajoControl()
        {
            if (GridStyleHelper.EnDisenio) return;

            BackColor = Color.White;
            DoubleBuffered = true;
            Click += FlujoTrabajoControl_Click;
        }

        public void ConfigurarEtapas(EtapaFlujo[] etapas)
        {
            if (GridStyleHelper.EnDisenio) return;

            _etapas = etapas ?? new EtapaFlujo[0];
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            _rectangulosEtapas.Clear();
            if (_etapas.Length == 0) return;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            int margen = 40;
            int anchoUtil = Math.Max(1, Width - margen * 2);
            int paso = _etapas.Length > 1 ? anchoUtil / (_etapas.Length - 1) : 0;
            int y = Height / 2 - 10;

            using (var lapizLinea = new Pen(Color.FromArgb(0xB0, 0xB0, 0xB0), 2f))
            using (var pincelCirculo = new SolidBrush(GridStyleHelper.ColorEncabezado))
            using (var pincelTexto = new SolidBrush(Color.FromArgb(0x33, 0x33, 0x33)))
            using (var pincelContador = new SolidBrush(Color.FromArgb(0x80, 0x80, 0x80)))
            using (var fuenteEtiqueta = new Font(Font.FontFamily, 8.5f))
            using (var fuenteContador = new Font(Font.FontFamily, 7.5f))
            {
                // Linea conectora, de centro a centro del primer al ultimo circulo.
                if (_etapas.Length > 1)
                {
                    e.Graphics.DrawLine(lapizLinea, margen, y, margen + paso * (_etapas.Length - 1), y);
                }

                for (int i = 0; i < _etapas.Length; i++)
                {
                    int x = margen + paso * i;
                    var rectCirculo = new Rectangle(x - RadioCirculo, y - RadioCirculo, RadioCirculo * 2, RadioCirculo * 2);
                    _rectangulosEtapas.Add(rectCirculo);

                    e.Graphics.FillEllipse(pincelCirculo, rectCirculo);

                    var icono = IconHelper.ObtenerBitmap(_etapas[i].Icono, 24);
                    if (icono != null)
                    {
                        e.Graphics.DrawImage(icono, x - 12, y - 12, 24, 24);
                    }

                    var tamanoEtiqueta = e.Graphics.MeasureString(_etapas[i].Etiqueta, fuenteEtiqueta);
                    float yEtiqueta = y + RadioCirculo + 8;
                    e.Graphics.DrawString(_etapas[i].Etiqueta, fuenteEtiqueta, pincelTexto,
                        x - tamanoEtiqueta.Width / 2, yEtiqueta);

                    if (_etapas[i].Contador > 0)
                    {
                        string textoContador = _etapas[i].Contador + (_etapas[i].Contador == 1 ? " registro" : " registros");
                        var tamanoContador = e.Graphics.MeasureString(textoContador, fuenteContador);
                        e.Graphics.DrawString(textoContador, fuenteContador, pincelContador,
                            x - tamanoContador.Width / 2, yEtiqueta + tamanoEtiqueta.Height);
                    }
                }
            }
        }

        private void FlujoTrabajoControl_Click(object sender, EventArgs e)
        {
            var posicion = PointToClient(Cursor.Position);
            for (int i = 0; i < _rectangulosEtapas.Count; i++)
            {
                if (_rectangulosEtapas[i].Contains(posicion))
                {
                    if (EtapaClick != null) EtapaClick(i);
                    return;
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }
    }
}
