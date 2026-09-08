using System;
using System.IO;

namespace Contabilidad.Data
{
    /// <summary>
    /// Rutas de datos de la aplicacion. Los archivos que la app necesita ESCRIBIR
    /// (cuentas.json, asientos.json, ajustes.json, etc.) se guardan en AppData del
    /// usuario, no junto al ejecutable: cuando la app se instala con el instalador
    /// en "Program Files", esa carpeta es de solo lectura para usuarios normales y
    /// escribir ahi lanza "Acceso denegado".
    /// </summary>
    public static class RutasApp
    {
        public static readonly string CarpetaDatos = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Contabilidad");

        /// <summary>
        /// Carpeta "Data" junto al ejecutable, donde viaja el catalogo semilla
        /// (cuentas_catalogo.json) que se instala con la app. Es de solo lectura.
        /// </summary>
        public static readonly string CarpetaDatosSemilla = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "Data");
    }
}
