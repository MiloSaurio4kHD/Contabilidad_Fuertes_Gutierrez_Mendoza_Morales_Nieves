using System.Collections.Generic;
using Contabilidad.Models;

namespace Contabilidad.UI
{
    /// <summary>
    /// Arma las filas (Concepto/Monto) del Estado de Resultados y del Balance General a
    /// partir de los totales ya calculados. Se extrajo de frmPrincipal para que tanto la
    /// grilla en pantalla como la exportacion a Excel muestren exactamente los mismos
    /// renglones, sin duplicar esta logica en dos lugares.
    /// </summary>
    public static class FilaReporteBuilder
    {
        public static List<FilaReporte> ConstruirFilasEstadoResultado(EstadoResultado er)
        {
            var filas = new List<FilaReporte>();

            filas.Add(new FilaReporte("INGRESOS", null, TipoFilaReporte.Seccion));
            foreach (var linea in er.Ingresos)
            {
                filas.Add(new FilaReporte(linea.Nombre, linea.Monto, TipoFilaReporte.Detalle));
            }
            filas.Add(new FilaReporte("Total Ingresos", er.TotalIngresos, TipoFilaReporte.Subtotal));

            filas.Add(new FilaReporte("GASTOS OPERATIVOS Y NO OPERATIVOS (-)", null, TipoFilaReporte.Seccion));
            foreach (var linea in er.Gastos)
            {
                filas.Add(new FilaReporte(linea.Nombre, linea.Monto, TipoFilaReporte.Detalle));
            }
            filas.Add(new FilaReporte("Total Gastos", er.TotalGastos, TipoFilaReporte.Subtotal));

            filas.Add(new FilaReporte("CÁLCULO DE UTILIDAD", null, TipoFilaReporte.Seccion));
            filas.Add(new FilaReporte(
                string.Format("Total Ingresos ({0:N2}) - Total Gastos ({1:N2})", er.TotalIngresos, er.TotalGastos),
                null, TipoFilaReporte.Nota));
            filas.Add(new FilaReporte("Utilidad antes de participación", er.UtilidadAntesParticipacion, TipoFilaReporte.Detalle));
            filas.Add(new FilaReporte("(-) 15% Participación Trabajadores", -er.Participacion15, TipoFilaReporte.Detalle));
            filas.Add(new FilaReporte("Utilidad antes de Impuesto a la Renta", er.UtilidadAntesImpuesto, TipoFilaReporte.Detalle));
            filas.Add(new FilaReporte("(-) 25% Impuesto a la Renta", -er.ImpuestoRenta25, TipoFilaReporte.Detalle));
            filas.Add(new FilaReporte("Utilidad del Ejercicio", er.UtilidadEjercicio, TipoFilaReporte.Total));

            return filas;
        }

        public static List<FilaReporte> ConstruirFilasBalanceGeneral(BalanceGeneral bg)
        {
            var filas = new List<FilaReporte>();

            filas.Add(new FilaReporte("ACTIVOS", null, TipoFilaReporte.Seccion));
            filas.Add(new FilaReporte("Activos Corrientes", null, TipoFilaReporte.Detalle));
            foreach (var linea in bg.ActivosCorrientes)
            {
                filas.Add(ConstruirFilaDetalleCuenta(linea, "    "));
            }
            filas.Add(new FilaReporte("Total Activos Corrientes", bg.TotalActivoCorriente, TipoFilaReporte.Subtotal));

            filas.Add(new FilaReporte("Activos No Corrientes", null, TipoFilaReporte.Detalle));
            foreach (var linea in bg.ActivosNoCorrientes)
            {
                filas.Add(ConstruirFilaDetalleCuenta(linea, "    "));
            }
            filas.Add(new FilaReporte("Total Activos No Corrientes", bg.TotalActivoNoCorriente, TipoFilaReporte.Subtotal));
            filas.Add(new FilaReporte("TOTAL ACTIVOS", bg.TotalActivo, TipoFilaReporte.Total));

            filas.Add(new FilaReporte("PASIVOS", null, TipoFilaReporte.Seccion));
            filas.Add(new FilaReporte("Pasivos Corrientes", null, TipoFilaReporte.Detalle));
            foreach (var linea in bg.PasivosCorrientes)
            {
                filas.Add(ConstruirFilaDetalleCuenta(linea, "    "));
            }
            filas.Add(new FilaReporte("Total Pasivos Corrientes", bg.TotalPasivoCorriente, TipoFilaReporte.Subtotal));

            filas.Add(new FilaReporte("Pasivos No Corrientes", null, TipoFilaReporte.Detalle));
            foreach (var linea in bg.PasivosNoCorrientes)
            {
                filas.Add(ConstruirFilaDetalleCuenta(linea, "    "));
            }
            filas.Add(new FilaReporte("Total Pasivos No Corrientes", bg.TotalPasivoNoCorriente, TipoFilaReporte.Subtotal));
            filas.Add(new FilaReporte("TOTAL PASIVOS", bg.TotalPasivo, TipoFilaReporte.Total));

            filas.Add(new FilaReporte("PATRIMONIO", null, TipoFilaReporte.Seccion));
            foreach (var linea in bg.Patrimonio)
            {
                filas.Add(ConstruirFilaDetalleCuenta(linea, string.Empty));
            }
            filas.Add(new FilaReporte("TOTAL PATRIMONIO", bg.TotalPatrimonio, TipoFilaReporte.Subtotal));

            filas.Add(new FilaReporte("TOTAL PASIVOS + PATRIMONIO", bg.TotalPasivoMasPatrimonio, TipoFilaReporte.Total));

            filas.Add(new FilaReporte("ECUACIÓN CONTABLE: Activo = Pasivo + Patrimonio",
                null, TipoFilaReporte.Seccion));
            filas.Add(new FilaReporte(
                string.Format("{0:N2} {1} {2:N2}", bg.TotalActivo, bg.Cuadra ? "=" : "≠", bg.TotalPasivoMasPatrimonio),
                null, bg.Cuadra ? TipoFilaReporte.Subtotal : TipoFilaReporte.Nota));

            return filas;
        }

        /// <summary>
        /// Antepone "(-) " al nombre de una cuenta cuyo monto es negativo (resta al total
        /// de su sección en vez de sumar), por ejemplo una depreciación acumulada dentro
        /// de Activos No Corrientes o una pérdida del ejercicio dentro de Patrimonio.
        /// </summary>
        private static string NombreConSigno(LineaCuentaMonto linea)
        {
            return linea.Monto < 0 ? "(-) " + linea.Nombre : linea.Nombre;
        }

        /// <summary>
        /// Arma la fila de detalle de una cuenta del Balance General. Si la cuenta tiene
        /// una advertencia de naturaleza (ver AdvertenciaSaldoHelper), se antepone el icono
        /// de alerta en vez de la sangria normal, para que resalte junto con el color de la
        /// fila que le aplica ReporteFinancieroDataGridView.
        /// </summary>
        private static FilaReporte ConstruirFilaDetalleCuenta(LineaCuentaMonto linea, string prefijoNormal)
        {
            bool tieneAdvertencia = linea.Advertencia != null;
            string concepto = (tieneAdvertencia ? "⚠ " : prefijoNormal) + NombreConSigno(linea);
            return new FilaReporte(concepto, linea.Monto, tieneAdvertencia ? TipoFilaReporte.Advertencia : TipoFilaReporte.Detalle);
        }
    }
}
