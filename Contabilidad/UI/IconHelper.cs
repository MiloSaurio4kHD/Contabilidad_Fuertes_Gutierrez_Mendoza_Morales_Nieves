using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Contabilidad.UI
{
    /// <summary>
    /// Carga los iconos de la carpeta Icons (junto al ejecutable) para usarlos en
    /// botones y en las pestañas del TabControl. Si el archivo no existe (por ejemplo
    /// en tiempo de diseño, donde la carpeta base no es bin\Debug) devuelve null en vez
    /// de lanzar una excepción, para no romper la app ni el diseñador.
    /// </summary>
    public static class IconHelper
    {
        private static readonly string CarpetaIconos = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Icons");
        private static readonly Dictionary<string, Bitmap> Cache = new Dictionary<string, Bitmap>(StringComparer.OrdinalIgnoreCase);

        public static Bitmap ObtenerBitmap(string nombreArchivo, int tamano = 16)
        {
            string clave = nombreArchivo + "_" + tamano;
            Bitmap bitmap;
            if (Cache.TryGetValue(clave, out bitmap)) return bitmap;

            string ruta = Path.Combine(CarpetaIconos, nombreArchivo);
            if (!File.Exists(ruta)) return null;

            using (var icono = new Icon(ruta, new Size(tamano, tamano)))
            {
                bitmap = icono.ToBitmap();
            }
            Cache[clave] = bitmap;
            return bitmap;
        }

        /// <summary>
        /// El ImageList no clona el icono al agregarlo: guarda la referencia y crea el
        /// handle recien cuando se usa (por ejemplo al asignar TabControl.ImageList).
        /// Por eso el Icon no se debe desechar aqui (a diferencia de ObtenerBitmap, que
        /// si convierte a Bitmap de inmediato).
        /// </summary>
        public static void RegistrarEnImageList(ImageList imageList, string clave, string nombreArchivo)
        {
            string ruta = Path.Combine(CarpetaIconos, nombreArchivo);
            if (!File.Exists(ruta)) return;

            var icono = new Icon(ruta, imageList.ImageSize);
            imageList.Images.Add(clave, icono);
        }

        /// <summary>
        /// Aplica un icono a la izquierda del texto de un boton, con la separacion
        /// necesaria para que no queden pegados.
        /// </summary>
        public static void AplicarIconoBoton(Button boton, string nombreArchivo)
        {
            var bitmap = ObtenerBitmap(nombreArchivo);
            if (bitmap == null) return;

            boton.Image = bitmap;
            boton.ImageAlign = ContentAlignment.MiddleLeft;
            boton.TextAlign = ContentAlignment.MiddleCenter;
            boton.TextImageRelation = TextImageRelation.ImageBeforeText;
            boton.Padding = new Padding(10, 0, 0, 0);
        }
    }
}
