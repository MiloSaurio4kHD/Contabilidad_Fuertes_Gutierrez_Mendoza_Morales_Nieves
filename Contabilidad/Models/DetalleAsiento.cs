namespace Contabilidad.Models
{
    public class DetalleAsiento
    {
        public string CuentaCodigo { get; set; }
        public string CuentaNombre { get; set; }
        public decimal Debe { get; set; }
        public decimal Haber { get; set; }

        public string CuentaCodigoNombre
        {
            get { return string.Format("{0} - {1}", CuentaCodigo, CuentaNombre); }
        }
    }
}
