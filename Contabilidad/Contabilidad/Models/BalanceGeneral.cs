using System;
using System.Collections.Generic;

namespace Contabilidad.Models
{
    /// <summary>
    /// Resultado del calculo del Balance General: activos, pasivos y patrimonio
    /// agrupados por su tipo, con sus totales y el chequeo de la ecuacion contable
    /// (Activo = Pasivo + Patrimonio).
    /// </summary>
    public class BalanceGeneral
    {
        public List<LineaCuentaMonto> ActivosCorrientes { get; set; }
        public decimal TotalActivoCorriente { get; set; }

        public List<LineaCuentaMonto> ActivosNoCorrientes { get; set; }
        public decimal TotalActivoNoCorriente { get; set; }

        public decimal TotalActivo { get; set; }

        public List<LineaCuentaMonto> PasivosCorrientes { get; set; }
        public decimal TotalPasivoCorriente { get; set; }

        public List<LineaCuentaMonto> PasivosNoCorrientes { get; set; }
        public decimal TotalPasivoNoCorriente { get; set; }

        public decimal TotalPasivo { get; set; }

        public List<LineaCuentaMonto> Patrimonio { get; set; }
        public decimal TotalPatrimonio { get; set; }

        public decimal TotalPasivoMasPatrimonio { get; set; }

        public bool Cuadra
        {
            get { return Math.Abs(TotalActivo - TotalPasivoMasPatrimonio) < 0.01m; }
        }

        /// <summary>
        /// Mensajes de advertencia (ver AdvertenciaSaldoHelper) de las cuentas cuyo saldo
        /// resulto contrario a su naturaleza contable esperada. No afecta los totales ni
        /// la ecuacion contable: es solo informativo para que el usuario revise los asientos.
        /// </summary>
        public List<string> Advertencias { get; set; }

        public BalanceGeneral()
        {
            ActivosCorrientes = new List<LineaCuentaMonto>();
            ActivosNoCorrientes = new List<LineaCuentaMonto>();
            PasivosCorrientes = new List<LineaCuentaMonto>();
            PasivosNoCorrientes = new List<LineaCuentaMonto>();
            Patrimonio = new List<LineaCuentaMonto>();
            Advertencias = new List<string>();
        }
    }
}
