using System.Drawing;
using System.Windows.Forms;

namespace Contabilidad.UI
{
    /// <summary>
    /// Columna de grid que se ve como un boton chico con icono + texto (ej. "Editar"/"Quitar"
    /// en el detalle de un asiento/ajuste). El texto y el icono son iguales para toda la
    /// columna, asi que se guardan aqui en vez de por celda.
    /// </summary>
    public class BotonConIconoColumn : DataGridViewTextBoxColumn
    {
        public string Texto { get; set; }
        public Bitmap Icono { get; set; }

        public BotonConIconoColumn()
        {
            CellTemplate = new BotonConIconoCell();
        }
    }

    /// <summary>
    /// Dibuja el "boton" (fondo + borde + icono + texto) sobreescribiendo Paint. No se usa el
    /// evento CellPainting con e.Handled=true porque, en un DataGridView con varias columnas en
    /// modo Fill, eso hace que WinForms deje de reconocer el clic en esa celda (verificado a
    /// mano: con e.Handled=true el CellMouseDown/CellMouseUp siguen disparando pero CellClick
    /// no). Sobreescribir Paint en una celda propia no tiene ese problema.
    /// </summary>
    public class BotonConIconoCell : DataGridViewTextBoxCell
    {
        protected override void Paint(Graphics g, Rectangle clipBounds, Rectangle cellBounds, int rowIndex,
            DataGridViewElementStates cellState, object value, object formattedValue, string errorText,
            DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle,
            DataGridViewPaintParts paintParts)
        {
            var columna = OwningColumn as BotonConIconoColumn;
            string texto = columna != null ? columna.Texto : string.Empty;
            Bitmap icono = columna != null ? columna.Icono : null;

            using (var fondoCelda = new SolidBrush(cellStyle.BackColor))
            {
                g.FillRectangle(fondoCelda, cellBounds);
            }

            var rectoBoton = Rectangle.Inflate(cellBounds, -2, -3);
            using (var fondoBoton = new SolidBrush(Color.FromArgb(0xF0, 0xF0, 0xF0)))
            {
                g.FillRectangle(fondoBoton, rectoBoton);
            }
            g.DrawRectangle(Pens.Silver, rectoBoton);

            int x = rectoBoton.Left + 6;
            if (icono != null)
            {
                int y = rectoBoton.Top + (rectoBoton.Height - icono.Height) / 2;
                g.DrawImage(icono, x, y, icono.Width, icono.Height);
                x += icono.Width + 4;
            }

            var rectoTexto = new Rectangle(x, rectoBoton.Top, rectoBoton.Right - x - 2, rectoBoton.Height);
            TextRenderer.DrawText(g, texto, cellStyle.Font, rectoTexto, cellStyle.ForeColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
        }
    }
}
