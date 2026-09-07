using Contabilidad.Models;

namespace Contabilidad.UI
{
    internal enum TipoFilaLibroContable
    {
        Detalle,
        Glosa,
        Separador
    }

    /// <summary>
    /// Informacion asociada (Tag) a cada fila de una grilla de Libro Diario o Libro de
    /// Ajustes, usada para saber a que asiento/ajuste pertenece y como debe pintarse.
    /// </summary>
    internal class FilaLibroContable
    {
        public TipoFilaLibroContable Tipo { get; set; }
        public IAsientoLibro Asiento { get; set; }
        public bool EsPrimeraFilaDelAsiento { get; set; }
    }
}
