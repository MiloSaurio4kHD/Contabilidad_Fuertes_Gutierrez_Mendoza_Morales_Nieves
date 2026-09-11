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

        /// <summary>
        /// Codigo de la cuenta de origen en el Libro Mayor (null para lineas calculadas,
        /// como la participacion de trabajadores o la utilidad del ejercicio, que no
        /// vienen de una sola cuenta). Sirve para que la exportacion a Excel pueda
        /// escribir una formula que apunte al Libro Mayor en vez de un numero suelto.
        /// </summary>
        public string Codigo { get; set; }

        /// <summary>
        /// Texto de advertencia (ver AdvertenciaSaldoHelper) si el saldo de esta cuenta es
        /// contrario a su naturaleza contable esperada; null si el saldo es normal.
        /// </summary>
        public string Advertencia { get; set; }
    }
}
