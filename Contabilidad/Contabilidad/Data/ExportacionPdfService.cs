using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Contabilidad.Models;
using Contabilidad.UI;

namespace Contabilidad.Data
{
    /// <summary>
    /// Arma un PDF con todo el sistema (Catálogo de Cuentas, Libro Diario, Libro de
    /// Ajustes, Libro Mayor, Balance de Comprobación, Estado de Resultados y Balance
    /// General), leyendo de los mismos repositorios/servicios que ya usa
    /// ExportacionExcelService. A diferencia del Excel (que tiene fórmulas vivas), el PDF
    /// es un documento estático: los dos reportes en cascada reutilizan directamente
    /// FilaReporteBuilder, la misma clase que ya arma esas filas para la grilla en
    /// pantalla, para no duplicar esa lógica.
    /// </summary>
    public class ExportacionPdfService
    {
        private readonly CuentaRepository _cuentaRepository = new CuentaRepository();
        private readonly LibroDiarioRepository _libroDiarioRepository = new LibroDiarioRepository();
        private readonly AjusteRepository _ajusteRepository = new AjusteRepository();
        private readonly LibroMayorService _libroMayorService = new LibroMayorService();
        private readonly EstadoResultadoService _estadoResultadoService = new EstadoResultadoService();
        private readonly BalanceGeneralService _balanceGeneralService = new BalanceGeneralService();

        public void Exportar(string ruta)
        {
            var secciones = new List<SeccionPdf>();

            secciones.Add(ConstruirSeccionCuentas(_cuentaRepository.ObtenerTodas()));
            secciones.Add(ConstruirSeccionLibro("Libro Diario", _libroDiarioRepository.ObtenerTodos().Cast<IAsientoLibro>().ToList()));
            secciones.Add(ConstruirSeccionLibro("Libro de Ajustes", _ajusteRepository.ObtenerTodos().Cast<IAsientoLibro>().ToList()));

            var cuentasMayor = _libroMayorService.ObtenerLibroMayor();
            secciones.Add(ConstruirSeccionLibroMayor(cuentasMayor));
            secciones.Add(ConstruirSeccionBalanceComprobacion(cuentasMayor));

            var er = _estadoResultadoService.Generar();
            secciones.Add(ConstruirSeccionDesdeFilas("Estado de Resultados", FilaReporteBuilder.ConstruirFilasEstadoResultado(er)));

            var bg = _balanceGeneralService.Generar();
            secciones.Add(ConstruirSeccionDesdeFilas("Balance General", FilaReporteBuilder.ConstruirFilasBalanceGeneral(bg)));

            PdfWriter.Exportar(ruta, secciones);
        }

        private static SeccionPdf ConstruirSeccionCuentas(List<Cuenta> cuentas)
        {
            var seccion = new SeccionPdf("Catálogo de Cuentas",
                new[] { "Código", "Nombre", "Tipo", "Naturaleza" },
                new double[] { 15, 45, 25, 15 });

            foreach (var cuenta in cuentas)
            {
                seccion.Filas.Add(new FilaPdf(EstiloFilaPdf.Detalle,
                    cuenta.Codigo, cuenta.Nombre, cuenta.Tipo.ToTextoAmigable(), cuenta.Naturaleza.ToString()));
            }

            if (seccion.Filas.Count == 0)
            {
                seccion.Filas.Add(new FilaPdf(EstiloFilaPdf.Nota, "(Sin cuentas)"));
            }

            return seccion;
        }

        private static SeccionPdf ConstruirSeccionLibro(string titulo, List<IAsientoLibro> asientos)
        {
            var seccion = new SeccionPdf(titulo,
                new[] { "N°", "Fecha", "Cuenta", "Debe", "Haber" },
                new double[] { 10, 14, 46, 15, 15 });

            foreach (var asiento in asientos)
            {
                bool esPrimera = true;
                foreach (var detalle in asiento.Detalles)
                {
                    seccion.Filas.Add(new FilaPdf(EstiloFilaPdf.Detalle,
                        esPrimera ? asiento.NumeroDisplay : string.Empty,
                        esPrimera ? asiento.Fecha.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : string.Empty,
                        detalle.CuentaNombre,
                        detalle.Debe > 0 ? detalle.Debe.ToString("N2", CultureInfo.InvariantCulture) : string.Empty,
                        detalle.Haber > 0 ? detalle.Haber.ToString("N2", CultureInfo.InvariantCulture) : string.Empty));
                    esPrimera = false;
                }
                seccion.Filas.Add(new FilaPdf(EstiloFilaPdf.Nota, "   " + asiento.Glosa));
            }

            if (seccion.Filas.Count == 0)
            {
                seccion.Filas.Add(new FilaPdf(EstiloFilaPdf.Nota, "(Sin registros)"));
            }

            return seccion;
        }

        private static SeccionPdf ConstruirSeccionLibroMayor(List<CuentaMayor> cuentasMayor)
        {
            var seccion = new SeccionPdf("Libro Mayor",
                new[] { "N°", "Debe", "N°", "Haber" },
                new double[] { 12, 28, 12, 28 });

            foreach (var cuenta in cuentasMayor)
            {
                seccion.Filas.Add(new FilaPdf(EstiloFilaPdf.Subtotal, cuenta.CodigoNombre));

                int filas = Math.Max(cuenta.MovimientosDebe.Count, cuenta.MovimientosHaber.Count);
                for (int i = 0; i < filas; i++)
                {
                    string nDebe = i < cuenta.MovimientosDebe.Count ? cuenta.MovimientosDebe[i].NumeroDisplay : string.Empty;
                    string montoDebe = i < cuenta.MovimientosDebe.Count ? cuenta.MovimientosDebe[i].Monto.ToString("N2", CultureInfo.InvariantCulture) : string.Empty;
                    string nHaber = i < cuenta.MovimientosHaber.Count ? cuenta.MovimientosHaber[i].NumeroDisplay : string.Empty;
                    string montoHaber = i < cuenta.MovimientosHaber.Count ? cuenta.MovimientosHaber[i].Monto.ToString("N2", CultureInfo.InvariantCulture) : string.Empty;
                    seccion.Filas.Add(new FilaPdf(EstiloFilaPdf.Detalle, nDebe, montoDebe, nHaber, montoHaber));
                }

                seccion.Filas.Add(new FilaPdf(EstiloFilaPdf.Total,
                    string.Format("Saldo {0}: {1}",
                        cuenta.EsSaldoDeudor ? "Deudor" : "Acreedor",
                        Math.Abs(cuenta.Saldo).ToString("N2", CultureInfo.InvariantCulture))));
            }

            if (seccion.Filas.Count == 0)
            {
                seccion.Filas.Add(new FilaPdf(EstiloFilaPdf.Nota, "(Sin cuentas con movimiento)"));
            }

            return seccion;
        }

        private static SeccionPdf ConstruirSeccionBalanceComprobacion(List<CuentaMayor> cuentasMayor)
        {
            var seccion = new SeccionPdf("Balance de Comprobación",
                new[] { "Cuenta", "Suma Debe", "Suma Haber", "Saldo Deudor", "Saldo Acreedor" },
                new double[] { 40, 15, 15, 15, 15 });

            decimal totalDebe = 0, totalHaber = 0, totalSaldoDeudor = 0, totalSaldoAcreedor = 0;

            foreach (var cuenta in cuentasMayor)
            {
                seccion.Filas.Add(new FilaPdf(EstiloFilaPdf.Detalle,
                    cuenta.CodigoNombre,
                    cuenta.TotalDebe.ToString("N2", CultureInfo.InvariantCulture),
                    cuenta.TotalHaber.ToString("N2", CultureInfo.InvariantCulture),
                    cuenta.SaldoDeudor > 0 ? cuenta.SaldoDeudor.ToString("N2", CultureInfo.InvariantCulture) : string.Empty,
                    cuenta.SaldoAcreedor > 0 ? cuenta.SaldoAcreedor.ToString("N2", CultureInfo.InvariantCulture) : string.Empty));

                totalDebe += cuenta.TotalDebe;
                totalHaber += cuenta.TotalHaber;
                totalSaldoDeudor += cuenta.SaldoDeudor;
                totalSaldoAcreedor += cuenta.SaldoAcreedor;
            }

            seccion.Filas.Add(new FilaPdf(EstiloFilaPdf.Total,
                "TOTAL",
                totalDebe.ToString("N2", CultureInfo.InvariantCulture),
                totalHaber.ToString("N2", CultureInfo.InvariantCulture),
                totalSaldoDeudor.ToString("N2", CultureInfo.InvariantCulture),
                totalSaldoAcreedor.ToString("N2", CultureInfo.InvariantCulture)));

            return seccion;
        }

        private static SeccionPdf ConstruirSeccionDesdeFilas(string titulo, List<FilaReporte> filas)
        {
            var seccion = new SeccionPdf(titulo, new[] { "Concepto", "Monto" }, new double[] { 70, 30 });

            foreach (var fila in filas)
            {
                string monto = fila.Monto.HasValue ? fila.Monto.Value.ToString("N2", CultureInfo.InvariantCulture) : string.Empty;

                EstiloFilaPdf estilo;
                switch (fila.Tipo)
                {
                    case TipoFilaReporte.Seccion: estilo = EstiloFilaPdf.Seccion; break;
                    case TipoFilaReporte.Subtotal: estilo = EstiloFilaPdf.Subtotal; break;
                    case TipoFilaReporte.Total: estilo = EstiloFilaPdf.Total; break;
                    case TipoFilaReporte.Nota: estilo = EstiloFilaPdf.Nota; break;
                    default: estilo = EstiloFilaPdf.Detalle; break;
                }

                seccion.Filas.Add(new FilaPdf(estilo, fila.Concepto, monto));
            }

            return seccion;
        }
    }
}
