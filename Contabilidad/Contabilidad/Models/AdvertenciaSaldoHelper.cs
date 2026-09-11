using System;
using System.Globalization;

namespace Contabilidad.Models
{
    /// <summary>
    /// Detecta cuentas cuyo saldo calculado (deudor o acreedor, segun Debe-Haber en el
    /// Libro Mayor) es contrario a la naturaleza contable configurada para esa cuenta en
    /// el plan de cuentas. No modifica el saldo ni los movimientos: solo arma el texto de
    /// advertencia para que el usuario revise los asientos. Se usa tanto en el Balance de
    /// Comprobacion (ajustado y sin ajustar) como en el Balance General para que todos
    /// reporten la misma inconsistencia con el mismo mensaje.
    /// </summary>
    public static class AdvertenciaSaldoHelper
    {
        public static string Detectar(Cuenta cuenta, bool esSaldoDeudor, decimal montoSaldo)
        {
            if (cuenta == null || montoSaldo == 0m) return null;

            // Las cuentas de depreciacion/amortizacion acumulada estan catalogadas como
            // Activo con naturaleza Deudora, pero por diseño reducen el activo acumulando
            // saldo acreedor: eso es su comportamiento normal, no un error de captura, asi
            // que se excluyen de esta advertencia (a pedido del usuario).
            if (EsCuentaContraActivoAcumulada(cuenta)) return null;

            bool coincideConNaturaleza = (cuenta.Naturaleza == NaturalezaCuenta.Deudora) == esSaldoDeudor;
            if (coincideConNaturaleza) return null;

            string sujeto, verbo;
            switch (cuenta.Tipo)
            {
                case TipoCuenta.ActivoCorriente:
                case TipoCuenta.ActivoNoCorriente:
                    sujeto = "Los activos"; verbo = "tienen";
                    break;
                case TipoCuenta.PasivoCorriente:
                case TipoCuenta.PasivoNoCorriente:
                    sujeto = "Los pasivos"; verbo = "tienen";
                    break;
                case TipoCuenta.Patrimonio:
                    sujeto = "El patrimonio"; verbo = "tiene";
                    break;
                case TipoCuenta.Ingresos:
                    sujeto = "Los ingresos"; verbo = "tienen";
                    break;
                default:
                    sujeto = "Los gastos"; verbo = "tienen";
                    break;
            }

            string tipoSaldoActual = esSaldoDeudor ? "deudor" : "acreedor";
            string naturalezaEsperada = cuenta.Naturaleza == NaturalezaCuenta.Deudora ? "deudora" : "acreedora";

            return string.Format(CultureInfo.InvariantCulture,
                "⚠ La cuenta {0} presenta un saldo {1} de {2:N2}. {3} normalmente {4} naturaleza {5}. Revise los movimientos registrados.",
                cuenta.Nombre, tipoSaldoActual, montoSaldo, sujeto, verbo, naturalezaEsperada);
        }

        /// <summary>
        /// True para "Depreciacion Acumulada", "Amortizacion Acumulada" y sus subcuentas
        /// (ej. "Deprec. Acum. Vehiculo"): cuentas de Activo cuyo saldo acreedor es su
        /// comportamiento esperado, no un error. Se detecta por nombre (contiene "acum" y
        /// "deprec" o "amortiz") en vez de por codigo, para cubrir tambien las subcuentas
        /// sin tener que mantener una lista de codigos.
        /// </summary>
        private static bool EsCuentaContraActivoAcumulada(Cuenta cuenta)
        {
            string nombre = cuenta.Nombre ?? string.Empty;
            bool esAcumulada = nombre.IndexOf("acum", StringComparison.OrdinalIgnoreCase) >= 0;
            bool esDeprecOAmortiz = nombre.IndexOf("deprec", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                     nombre.IndexOf("amortiz", StringComparison.OrdinalIgnoreCase) >= 0;
            return esAcumulada && esDeprecOAmortiz;
        }
    }
}
