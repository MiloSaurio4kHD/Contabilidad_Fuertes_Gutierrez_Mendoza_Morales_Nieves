using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Threading;

namespace Contabilidad.Data
{
    /// <summary>
    /// Como debe pintarse una fila dentro de una seccion del PDF: titulo de seccion en
    /// verde (ej. "INGRESOS"), una fila de detalle normal, un subtotal, un total, o una
    /// nota (texto libre en una sola columna). Mismo criterio visual que TipoFilaReporte
    /// y EstiloCelda ya usados en pantalla y en el exportador de Excel.
    /// </summary>
    public enum EstiloFilaPdf
    {
        Seccion,
        Detalle,
        Subtotal,
        Total,
        Nota
    }

    /// <summary>Una fila de datos dentro de una SeccionPdf, ya lista para dibujar.</summary>
    public class FilaPdf
    {
        public string[] Columnas { get; private set; }
        public EstiloFilaPdf Estilo { get; private set; }

        public FilaPdf(EstiloFilaPdf estilo, params string[] columnas)
        {
            Estilo = estilo;
            Columnas = columnas;
        }
    }

    /// <summary>
    /// Una seccion del reporte (equivalente a una hoja de Excel): un titulo, los
    /// encabezados de columna (se repiten arriba de cada pagina mientras dure la
    /// seccion) y sus filas de datos. Cada seccion arranca siempre en pagina nueva.
    /// </summary>
    public class SeccionPdf
    {
        public string Titulo { get; private set; }
        public string[] Encabezados { get; private set; }
        public double[] AnchoColumnas { get; private set; }
        public List<FilaPdf> Filas { get; private set; }

        public SeccionPdf(string titulo, string[] encabezados, double[] anchoColumnas)
        {
            Titulo = titulo;
            Encabezados = encabezados;
            AnchoColumnas = anchoColumnas;
            Filas = new List<FilaPdf>();
        }
    }

    /// <summary>
    /// Genera un PDF a partir de una lista de secciones, usando PrintDocument contra la
    /// impresora virtual "Microsoft Print to PDF" que trae Windows 10/11 de fabrica. No
    /// se arma el formato PDF a mano ni se depende de ninguna libreria de terceros: se
    /// dibuja con las mismas primitivas de GDI+ (Graphics.DrawString/FillRectangle) que
    /// ya usa el resto de la app, y Windows se encarga de producir el archivo real.
    /// </summary>
    public static class PdfWriter
    {
        private const string NombreImpresora = "Microsoft Print to PDF";
        private static readonly Color ColorEncabezado = Color.FromArgb(0x37, 0x56, 0x23);
        private static readonly Color ColorFilaGlosa = Color.FromArgb(0xF2, 0xF2, 0xF2);
        private static readonly Color ColorColumnaCuenta = Color.FromArgb(0xDC, 0xE6, 0xF1);

        public static void Exportar(string ruta, List<SeccionPdf> secciones)
        {
            using (var doc = new PrintDocument())
            {
                doc.PrinterSettings.PrinterName = NombreImpresora;
                if (!doc.PrinterSettings.IsValid)
                {
                    throw new InvalidOperationException(
                        "No se encontró la impresora \"Microsoft Print to PDF\". Actívala desde " +
                        "Configuración > Aplicaciones > Características opcionales (o Panel de " +
                        "Control > Dispositivos e impresoras) e intenta de nuevo.");
                }

                doc.PrinterSettings.PrintToFile = true;
                doc.PrinterSettings.PrintFileName = ruta;
                doc.DefaultPageSettings.PaperSize = new PaperSize("A4", 827, 1169);
                doc.DefaultPageSettings.Margins = new Margins(50, 50, 60, 60);
                doc.PrintController = new StandardPrintController();

                var sesion = new SesionImpresion(secciones);
                doc.PrintPage += sesion.PrintPage;
                doc.Print();
            }

            EsperarArchivoListo(ruta);
        }

        /// <summary>
        /// "Microsoft Print to PDF" termina de escribir el archivo de forma asincrona por
        /// fuera de doc.Print() (via el spooler de impresion): el metodo Print() puede volver
        /// antes de que el archivo este completo. Si en ese momento algo mas (como el boton
        /// "Exportar a PDF", que abre el archivo apenas termina la exportacion) intenta abrirlo,
        /// el visor lo encuentra a medio escribir y muestra un error hasta que el usuario le da
        /// "Actualizar". Por eso aca se espera hasta poder abrir el archivo en modo exclusivo
        /// (senal de que el spooler ya lo solto) antes de devolver el control a quien llamo.
        /// </summary>
        private static void EsperarArchivoListo(string ruta)
        {
            const int intentosMax = 100;
            const int esperaMs = 100;

            for (int intento = 0; intento < intentosMax; intento++)
            {
                try
                {
                    using (var fs = new FileStream(ruta, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        if (fs.Length > 0) return;
                    }
                }
                catch (IOException)
                {
                    // El spooler todavia tiene el archivo abierto: se reintenta.
                }
                catch (UnauthorizedAccessException)
                {
                    // Mismo caso en algunos sistemas: se reintenta.
                }

                Thread.Sleep(esperaMs);
            }
        }

        /// <summary>
        /// Guarda en donde va la impresion entre un PrintPage y el siguiente (que seccion,
        /// que fila): PrintDocument llama a PrintPage una vez por cada pagina fisica, sin
        /// saber de antemano cuantas van a hacer falta.
        /// </summary>
        private class SesionImpresion
        {
            private readonly List<SeccionPdf> _secciones;
            private int _seccionIdx;
            private int _filaIdx;
            private bool _portadaDibujada;

            public SesionImpresion(List<SeccionPdf> secciones)
            {
                _secciones = secciones;
            }

            public void PrintPage(object sender, PrintPageEventArgs e)
            {
                var g = e.Graphics;
                var area = e.MarginBounds;

                if (!_portadaDibujada)
                {
                    DibujarPortada(g, area);
                    _portadaDibujada = true;
                    e.HasMorePages = _seccionIdx < _secciones.Count;
                    return;
                }

                float y = area.Top;

                while (_seccionIdx < _secciones.Count)
                {
                    var seccion = _secciones[_seccionIdx];

                    if (_filaIdx == 0)
                    {
                        y = DibujarTitulo(g, area, seccion, y);
                    }
                    y = DibujarEncabezado(g, area, seccion, y);

                    while (_filaIdx < seccion.Filas.Count)
                    {
                        const float alto = 18f;
                        if (y + alto > area.Bottom)
                        {
                            e.HasMorePages = true;
                            return;
                        }

                        DibujarFila(g, area, seccion, seccion.Filas[_filaIdx], y);
                        y += alto;
                        _filaIdx++;
                    }

                    _seccionIdx++;
                    _filaIdx = 0;
                    if (_seccionIdx < _secciones.Count)
                    {
                        e.HasMorePages = true;
                        return;
                    }
                }

                e.HasMorePages = false;
            }

            private static void DibujarPortada(Graphics g, Rectangle area)
            {
                using (var fuenteTitulo = new Font("Segoe UI", 22f, FontStyle.Bold))
                using (var fuenteSub = new Font("Segoe UI", 11f))
                using (var pincelTitulo = new SolidBrush(ColorEncabezado))
                {
                    string titulo = "Contabilidad - Reporte Completo";
                    var tamanoTitulo = g.MeasureString(titulo, fuenteTitulo);
                    g.DrawString(titulo, fuenteTitulo, pincelTitulo,
                        area.Left + (area.Width - tamanoTitulo.Width) / 2, area.Top + 250);

                    string fecha = "Generado el " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                    var tamanoFecha = g.MeasureString(fecha, fuenteSub);
                    g.DrawString(fecha, fuenteSub, Brushes.Gray,
                        area.Left + (area.Width - tamanoFecha.Width) / 2, area.Top + 250 + tamanoTitulo.Height + 10);
                }
            }

            private static float DibujarTitulo(Graphics g, Rectangle area, SeccionPdf seccion, float y)
            {
                using (var fuente = new Font("Segoe UI", 14f, FontStyle.Bold))
                {
                    g.DrawString(seccion.Titulo, fuente, Brushes.Black, area.Left, y);
                }
                return y + 28f;
            }

            private static float DibujarEncabezado(Graphics g, Rectangle area, SeccionPdf seccion, float y)
            {
                const float alto = 22f;
                using (var fondo = new SolidBrush(ColorEncabezado))
                using (var fuente = new Font("Segoe UI", 9f, FontStyle.Bold))
                {
                    g.FillRectangle(fondo, area.Left, y, area.Width, alto);
                    DibujarColumnas(g, area, seccion.AnchoColumnas, seccion.Encabezados, fuente, Brushes.White, y, alto);
                }
                return y + alto;
            }

            private static void DibujarFila(Graphics g, Rectangle area, SeccionPdf seccion, FilaPdf fila, float y)
            {
                const float alto = 18f;
                Brush fondo = null;
                Brush pincelTexto = Brushes.Black;
                FontStyle estiloFuente = FontStyle.Regular;

                switch (fila.Estilo)
                {
                    case EstiloFilaPdf.Seccion:
                        fondo = new SolidBrush(ColorEncabezado);
                        pincelTexto = Brushes.White;
                        estiloFuente = FontStyle.Bold;
                        break;
                    case EstiloFilaPdf.Subtotal:
                        fondo = new SolidBrush(ColorFilaGlosa);
                        estiloFuente = FontStyle.Bold;
                        break;
                    case EstiloFilaPdf.Total:
                        fondo = new SolidBrush(ColorColumnaCuenta);
                        estiloFuente = FontStyle.Bold;
                        break;
                    case EstiloFilaPdf.Nota:
                        estiloFuente = FontStyle.Italic;
                        break;
                }

                using (var fuente = new Font("Segoe UI", 8.5f, estiloFuente))
                {
                    if (fondo != null)
                    {
                        g.FillRectangle(fondo, area.Left, y, area.Width, alto);
                        fondo.Dispose();
                    }

                    // Una fila con una sola columna (nombre de cuenta en el Libro Mayor,
                    // la glosa de un asiento, el saldo final) ocupa todo el ancho de la
                    // tabla en vez de quedar encajonada en el ancho angosto de la primera
                    // columna de la seccion.
                    if (fila.Columnas.Length == 1)
                    {
                        var rectCompleto = new RectangleF(area.Left + 3, y + 2, area.Width - 6, alto - 2);
                        g.DrawString(fila.Columnas[0] ?? string.Empty, fuente, pincelTexto, rectCompleto);
                    }
                    else
                    {
                        DibujarColumnas(g, area, seccion.AnchoColumnas, fila.Columnas, fuente, pincelTexto, y, alto);
                    }
                }
            }

            private static void DibujarColumnas(Graphics g, Rectangle area, double[] anchos, string[] textos, Font fuente, Brush pincel, float y, float alto)
            {
                double sumaPesos = 0;
                foreach (var peso in anchos) sumaPesos += peso;

                float x = area.Left;
                for (int i = 0; i < anchos.Length; i++)
                {
                    float anchoCol = (float)(area.Width * (anchos[i] / sumaPesos));
                    string texto = (textos != null && i < textos.Length) ? textos[i] : string.Empty;
                    var rectTexto = new RectangleF(x + 3, y + 2, anchoCol - 6, alto - 2);
                    g.DrawString(texto ?? string.Empty, fuente, pincel, rectTexto);
                    x += anchoCol;
                }
            }
        }
    }
}
