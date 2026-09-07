namespace Contabilidad.Models
{
    /// <summary>
    /// Una linea generica "nombre + monto" usada dentro del Estado de Resultados
    /// y el Balance General (una cuenta con su valor, o una linea calculada como
    /// la participacion de trabajadores o la utilidad del ejercicio).
    /// </summary>
    public class LineaCuentaMonto
    {
        public string Nombre { get; set; }
        public decimal Monto { get; set; }
    }
}
