using System;

namespace Contabilidad.Models
{
    public class Cuenta
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public TipoCuenta Tipo { get; set; }
        public string Elemento { get; set; }
        public string Grupo { get; set; }
        public string CuentaPadre { get; set; }
        public int Nivel { get; set; }
        public NaturalezaCuenta Naturaleza { get; set; }

        public string CodigoNombre
        {
            get { return string.Format("{0} - {1}", Codigo, Nombre); }
        }

        public override string ToString()
        {
            return CodigoNombre;
        }

        public override bool Equals(object obj)
        {
            var otra = obj as Cuenta;
            if (otra == null) return false;
            return string.Equals(Codigo, otra.Codigo, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return Codigo == null ? 0 : Codigo.ToUpperInvariant().GetHashCode();
        }
    }
}
