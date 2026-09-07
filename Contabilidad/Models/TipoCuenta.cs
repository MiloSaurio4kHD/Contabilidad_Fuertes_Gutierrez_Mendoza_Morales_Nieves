namespace Contabilidad.Models
{
    public enum TipoCuenta
    {
        ActivoCorriente,
        ActivoNoCorriente,
        PasivoCorriente,
        PasivoNoCorriente,
        Patrimonio,
        Ingresos,
        Gastos
    }

    public enum NaturalezaCuenta
    {
        Deudora,
        Acreedora
    }

    public static class TipoCuentaExtensions
    {
        public static string ToTextoAmigable(this TipoCuenta tipo)
        {
            switch (tipo)
            {
                case TipoCuenta.ActivoCorriente: return "Activo Corriente";
                case TipoCuenta.ActivoNoCorriente: return "Activo No Corriente";
                case TipoCuenta.PasivoCorriente: return "Pasivo Corriente";
                case TipoCuenta.PasivoNoCorriente: return "Pasivo No Corriente";
                case TipoCuenta.Patrimonio: return "Patrimonio";
                case TipoCuenta.Ingresos: return "Ingresos";
                case TipoCuenta.Gastos: return "Gastos";
                default: return tipo.ToString();
            }
        }

        public static NaturalezaCuenta ObtenerNaturaleza(this TipoCuenta tipo)
        {
            switch (tipo)
            {
                case TipoCuenta.ActivoCorriente:
                case TipoCuenta.ActivoNoCorriente:
                case TipoCuenta.Gastos:
                    return NaturalezaCuenta.Deudora;
                default:
                    return NaturalezaCuenta.Acreedora;
            }
        }
    }
}
