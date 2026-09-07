using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Contabilidad.Models;

namespace Contabilidad.Data
{
    /// <summary>
    /// Arma una hoja de Excel por cada pestaña de la app (Libro Diario, Libro Ajustes,
    /// Libro Mayor, Balance de Comprobacion, Estado de Resultados, Balance General y
    /// Cuentas), leyendo de los mismos repositorios/servicios que ya usa frmPrincipal.
    ///
    /// Los subtotales/totales se escriben como formulas reales de Excel (no como el
    /// numero ya calculado): dentro de cada hoja se suman sus propias filas de detalle,
    /// y donde tiene sentido una hoja referencia directamente una celda de otra (Balance
    /// de Comprobacion toma sus sumas del Libro Mayor; Balance General toma la utilidad
    /// y los impuestos del Estado de Resultados). Solo las lineas de detalle individuales
    /// (una venta, un gasto puntual) quedan como el numero ya calculado por la app: son el
    /// dato de origen, no algo a recalcular.
    /// </summary>
    public class ExportacionExcelService
    {
        private readonly CuentaRepository _cuentaRepository = new CuentaRepository();
        private readonly LibroDiarioRepository _libroDiarioRepository = new LibroDiarioRepository();
        private readonly AjusteRepository _ajusteRepository = new AjusteRepository();
        private readonly LibroMayorService _libroMayorService = new LibroMayorService();
        private readonly EstadoResultadoService _estadoResultadoService = new EstadoResultadoService();
        private readonly BalanceGeneralService _balanceGeneralService = new BalanceGeneralService();

        public void Exportar(string ruta)
        {
            var hojas = new List<HojaExcel>();
            var mapaMovimientos = new MapaMovimientos();

            hojas.Add(ConstruirHojaLibroContable("Libro Diario", _libroDiarioRepository.ObtenerTodos().Cast<IAsientoLibro>().ToList(), mapaMovimientos));
            hojas.Add(ConstruirHojaLibroContable("Libro Ajustes", _ajusteRepository.ObtenerTodos().Cast<IAsientoLibro>().ToList(), mapaMovimientos));

            var cuentasMayor = _libroMayorService.ObtenerLibroMayor();
            Dictionary<string, int> filaSumaPorCuenta;
            hojas.Add(ConstruirHojaLibroMayor(cuentasMayor, mapaMovimientos, out filaSumaPorCuenta));

            hojas.Add(ConstruirHojaBalanceComprobacion(cuentasMayor, "Libro Mayor", filaSumaPorCuenta));

            var er = _estadoResultadoService.Generar();
            Dictionary<string, int> filasEstadoResultado;
            hojas.Add(ConstruirHojaEstadoResultado(er, "Libro Mayor", filaSumaPorCuenta, out filasEstadoResultado));

            var bg = _balanceGeneralService.Generar();
            hojas.Add(ConstruirHojaBalanceGeneral(bg, "Estado Resultado", filasEstadoResultado, "Libro Mayor", filaSumaPorCuenta));

            hojas.Add(ConstruirHojaCuentas(_cuentaRepository.ObtenerTodas()));

            ExcelWriter.Exportar(ruta, hojas);
        }

        // ---------------------------------------------------------------
        // Libro Diario / Libro Ajustes
        // ---------------------------------------------------------------

        private static HojaExcel ConstruirHojaLibroContable(string nombreHoja, List<IAsientoLibro> asientos, MapaMovimientos mapaMovimientos)
        {
            var hoja = new HojaExcel(nombreHoja) { ColumnasEncabezado = 5 };
            hoja.AnchoColumnas.AddRange(new[] { 10.0, 12.0, 40.0, 14.0, 14.0 });

            hoja.Filas.Add(FilaTitulo(nombreHoja));
            hoja.Filas.Add(Fila(EstiloCelda.Encabezado, "N°", "Fecha", "Cuenta", "Debe", "Haber"));

            int filaInicioDatos = hoja.Filas.Count + 1;

            foreach (var asiento in asientos)
            {
                for (int i = 0; i < asiento.Detalles.Count; i++)
                {
                    var detalle = asiento.Detalles[i];
                    bool esPrimera = i == 0;
                    int filaActual = hoja.Filas.Count + 1;

                    CeldaExcel celdaDebe;
                    if (detalle.Debe > 0)
                    {
                        celdaDebe = CeldaExcel.DeNumero(detalle.Debe);
                        mapaMovimientos.RegistrarDebe(detalle.CuentaCodigo, ExcelWriter.EnHoja(nombreHoja, ExcelWriter.Direccion(filaActual, 3)));
                    }
                    else
                    {
                        celdaDebe = CeldaExcel.DeTexto(string.Empty);
                    }

                    CeldaExcel celdaHaber;
                    if (detalle.Haber > 0)
                    {
                        celdaHaber = CeldaExcel.DeNumero(detalle.Haber);
                        mapaMovimientos.RegistrarHaber(detalle.CuentaCodigo, ExcelWriter.EnHoja(nombreHoja, ExcelWriter.Direccion(filaActual, 4)));
                    }
                    else
                    {
                        celdaHaber = CeldaExcel.DeTexto(string.Empty);
                    }

                    hoja.Filas.Add(new List<CeldaExcel>
                    {
                        CeldaExcel.DeTexto(esPrimera ? asiento.NumeroDisplay : string.Empty),
                        CeldaExcel.DeTexto(esPrimera ? asiento.Fecha.ToString("d/M/yyyy", CultureInfo.InvariantCulture) : string.Empty),
                        CeldaExcel.DeTexto(detalle.CuentaNombre),
                        celdaDebe,
                        celdaHaber
                    });
                }

                hoja.Filas.Add(Fila(EstiloCelda.Normal, string.Empty, string.Empty, "G: " + asiento.Glosa, string.Empty, string.Empty));
            }

            int filaFinDatos = hoja.Filas.Count;
            bool hayAsientos = filaFinDatos >= filaInicioDatos;

            decimal totalDebe = asientos.SelectMany(a => a.Detalles).Sum(d => d.Debe);
            decimal totalHaber = asientos.SelectMany(a => a.Detalles).Sum(d => d.Haber);

            hoja.Filas.Add(Fila(EstiloCelda.Encabezado, string.Empty, string.Empty, "RESUMEN", string.Empty, string.Empty));

            hoja.Filas.Add(FilaResumen("Total Debe", FormulaSumaOCero(filaInicioDatos, filaFinDatos, 3, totalDebe, EstiloCelda.Total)));
            int filaTotalDebe = hoja.Filas.Count;

            hoja.Filas.Add(FilaResumen("Total Haber", FormulaSumaOCero(filaInicioDatos, filaFinDatos, 4, totalHaber, EstiloCelda.Total)));
            int filaTotalHaber = hoja.Filas.Count;

            string dirTotalDebe = ExcelWriter.Direccion(filaTotalDebe, 3);
            string dirTotalHaber = ExcelWriter.Direccion(filaTotalHaber, 3);
            decimal diferencia = totalDebe - totalHaber;
            hoja.Filas.Add(FilaResumen("Diferencia",
                CeldaExcel.DeFormulaNumero(dirTotalDebe + "-" + dirTotalHaber, diferencia)));
            int filaDiferencia = hoja.Filas.Count;
            string dirDiferencia = ExcelWriter.Direccion(filaDiferencia, 3);

            CeldaExcel celdaNAsientos = hayAsientos
                ? CeldaExcel.DeFormulaNumero("SUMPRODUCT(--(" + ExcelWriter.Rango(filaInicioDatos, filaFinDatos, 0) + "<>\"\"))", asientos.Count)
                : CeldaExcel.DeNumero(0);
            hoja.Filas.Add(FilaResumen("N° asientos", celdaNAsientos));

            bool cuadrado = diferencia == 0;
            hoja.Filas.Add(FilaResumen("Estado:",
                CeldaExcel.DeFormulaTexto(
                    string.Format("IF({0}=0,\"CUADRADO ✓\",\"DESCUADRADO ✗\")", dirDiferencia),
                    cuadrado ? "CUADRADO ✓" : "DESCUADRADO ✗")));

            return hoja;
        }

        private static List<CeldaExcel> FilaResumen(string etiqueta, CeldaExcel valor)
        {
            return new List<CeldaExcel>
            {
                CeldaExcel.DeTexto(string.Empty),
                CeldaExcel.DeTexto(string.Empty),
                CeldaExcel.DeTexto(etiqueta, EstiloCelda.Total),
                valor,
                CeldaExcel.DeTexto(string.Empty)
            };
        }

        // ---------------------------------------------------------------
        // Libro Mayor
        // ---------------------------------------------------------------

        private static HojaExcel ConstruirHojaLibroMayor(List<CuentaMayor> cuentas, MapaMovimientos mapaMovimientos, out Dictionary<string, int> filaSumaPorCuenta)
        {
            var hoja = new HojaExcel("Libro Mayor") { ColumnasEncabezado = 4 };
            hoja.AnchoColumnas.AddRange(new[] { 10.0, 14.0, 10.0, 14.0 });
            hoja.Filas.Add(FilaTitulo("Libro Mayor"));

            filaSumaPorCuenta = new Dictionary<string, int>();

            foreach (var cuenta in cuentas)
            {
                hoja.Filas.Add(Fila(EstiloCelda.Encabezado, cuenta.CodigoNombre, string.Empty, string.Empty, string.Empty));
                hoja.Filas.Add(Fila(EstiloCelda.Total, "N°", "Debe", "N°", "Haber"));

                int filaInicioMov = hoja.Filas.Count + 1;
                int filasMovimiento = System.Math.Max(cuenta.MovimientosDebe.Count, cuenta.MovimientosHaber.Count);
                for (int i = 0; i < filasMovimiento; i++)
                {
                    var debe = i < cuenta.MovimientosDebe.Count ? cuenta.MovimientosDebe[i] : null;
                    var haber = i < cuenta.MovimientosHaber.Count ? cuenta.MovimientosHaber[i] : null;

                    CeldaExcel celdaDebe = CeldaExcel.DeTexto(string.Empty);
                    if (debe != null)
                    {
                        string refOrigen = mapaMovimientos.SiguienteDebe(cuenta.Codigo);
                        celdaDebe = refOrigen != null ? CeldaExcel.DeFormulaNumero(refOrigen, debe.Monto) : CeldaExcel.DeNumero(debe.Monto);
                    }

                    CeldaExcel celdaHaber = CeldaExcel.DeTexto(string.Empty);
                    if (haber != null)
                    {
                        string refOrigen = mapaMovimientos.SiguienteHaber(cuenta.Codigo);
                        celdaHaber = refOrigen != null ? CeldaExcel.DeFormulaNumero(refOrigen, haber.Monto) : CeldaExcel.DeNumero(haber.Monto);
                    }

                    hoja.Filas.Add(new List<CeldaExcel>
                    {
                        CeldaExcel.DeTexto(debe != null ? debe.NumeroDisplay : string.Empty),
                        celdaDebe,
                        CeldaExcel.DeTexto(haber != null ? haber.NumeroDisplay : string.Empty),
                        celdaHaber
                    });
                }
                int filaFinMov = hoja.Filas.Count;

                var celdaSumaDebe = FormulaSumaOCero(filaInicioMov, filaFinMov, 1, cuenta.TotalDebe, EstiloCelda.Total);
                var celdaSumaHaber = FormulaSumaOCero(filaInicioMov, filaFinMov, 3, cuenta.TotalHaber, EstiloCelda.Total);

                hoja.Filas.Add(new List<CeldaExcel>
                {
                    CeldaExcel.DeTexto("Σ", EstiloCelda.Total),
                    celdaSumaDebe,
                    CeldaExcel.DeTexto("Σ", EstiloCelda.Total),
                    celdaSumaHaber
                });
                int filaSuma = hoja.Filas.Count;
                filaSumaPorCuenta[cuenta.Codigo] = filaSuma;

                string dirSumaDebe = ExcelWriter.Direccion(filaSuma, 1);
                string dirSumaHaber = ExcelWriter.Direccion(filaSuma, 3);
                string formulaSaldo = string.Format(
                    "\"Saldo \"&IF({0}>={1},\"Deudor: \",\"Acreedor: \")&TEXT(ABS({0}-{1}),\"#,##0.00\")",
                    dirSumaDebe, dirSumaHaber);
                string etiquetaSaldo = cuenta.EsSaldoDeudor ? "Saldo Deudor: " : "Saldo Acreedor: ";
                decimal montoSaldoCache = cuenta.EsSaldoDeudor ? cuenta.Saldo : -cuenta.Saldo;
                string textoSaldoCache = etiquetaSaldo + montoSaldoCache.ToString("N2", CultureInfo.InvariantCulture);

                hoja.Filas.Add(new List<CeldaExcel>
                {
                    CeldaExcel.DeFormulaTexto(formulaSaldo, textoSaldoCache, EstiloCelda.Encabezado),
                    CeldaExcel.DeTexto(string.Empty, EstiloCelda.Encabezado),
                    CeldaExcel.DeTexto(string.Empty, EstiloCelda.Encabezado),
                    CeldaExcel.DeTexto(string.Empty, EstiloCelda.Encabezado)
                });

                hoja.Filas.Add(Fila(EstiloCelda.Normal, string.Empty, string.Empty, string.Empty, string.Empty));
            }

            return hoja;
        }

        // ---------------------------------------------------------------
        // Balance de Comprobación
        // ---------------------------------------------------------------

        private static HojaExcel ConstruirHojaBalanceComprobacion(List<CuentaMayor> cuentas, string nombreHojaLibroMayor, Dictionary<string, int> filaSumaPorCuenta)
        {
            var hoja = new HojaExcel("Balance Comprobación") { ColumnasEncabezado = 5 };
            hoja.AnchoColumnas.AddRange(new[] { 40.0, 14.0, 14.0, 14.0, 14.0 });
            hoja.Filas.Add(FilaTitulo("Balance Comprobación"));
            hoja.Filas.Add(Fila(EstiloCelda.Encabezado, "Cuenta", "Suma Debe", "Suma Haber", "Saldo Deudor", "Saldo Acreedor"));

            int filaInicioDatos = hoja.Filas.Count + 1;

            foreach (var cuenta in cuentas)
            {
                int filaLibroMayor;
                CeldaExcel celdaSumaDebe, celdaSumaHaber;
                if (filaSumaPorCuenta.TryGetValue(cuenta.Codigo, out filaLibroMayor))
                {
                    celdaSumaDebe = CeldaExcel.DeFormulaNumero(
                        ExcelWriter.EnHoja(nombreHojaLibroMayor, ExcelWriter.Direccion(filaLibroMayor, 1)), cuenta.TotalDebe);
                    celdaSumaHaber = CeldaExcel.DeFormulaNumero(
                        ExcelWriter.EnHoja(nombreHojaLibroMayor, ExcelWriter.Direccion(filaLibroMayor, 3)), cuenta.TotalHaber);
                }
                else
                {
                    celdaSumaDebe = CeldaExcel.DeNumero(cuenta.TotalDebe);
                    celdaSumaHaber = CeldaExcel.DeNumero(cuenta.TotalHaber);
                }

                int filaActual = hoja.Filas.Count + 1;
                string dirSumaDebeAqui = ExcelWriter.Direccion(filaActual, 1);
                string dirSumaHaberAqui = ExcelWriter.Direccion(filaActual, 2);

                hoja.Filas.Add(new List<CeldaExcel>
                {
                    CeldaExcel.DeTexto(cuenta.CodigoNombre),
                    celdaSumaDebe,
                    celdaSumaHaber,
                    CeldaExcel.DeFormulaNumero(string.Format("IF({0}>={1},{0}-{1},0)", dirSumaDebeAqui, dirSumaHaberAqui), cuenta.SaldoDeudor),
                    CeldaExcel.DeFormulaNumero(string.Format("IF({1}>{0},{1}-{0},0)", dirSumaDebeAqui, dirSumaHaberAqui), cuenta.SaldoAcreedor)
                });
            }

            int filaFinDatos = hoja.Filas.Count;

            hoja.Filas.Add(new List<CeldaExcel>
            {
                CeldaExcel.DeTexto("TOTAL", EstiloCelda.Total),
                FormulaSumaOCero(filaInicioDatos, filaFinDatos, 1, cuentas.Sum(c => c.TotalDebe), EstiloCelda.Total),
                FormulaSumaOCero(filaInicioDatos, filaFinDatos, 2, cuentas.Sum(c => c.TotalHaber), EstiloCelda.Total),
                FormulaSumaOCero(filaInicioDatos, filaFinDatos, 3, cuentas.Sum(c => c.SaldoDeudor), EstiloCelda.Total),
                FormulaSumaOCero(filaInicioDatos, filaFinDatos, 4, cuentas.Sum(c => c.SaldoAcreedor), EstiloCelda.Total)
            });

            return hoja;
        }

        // ---------------------------------------------------------------
        // Estado de Resultados
        // ---------------------------------------------------------------

        private static HojaExcel ConstruirHojaEstadoResultado(EstadoResultado er, string nombreHojaLibroMayor, Dictionary<string, int> filaSumaPorCuenta, out Dictionary<string, int> filasClave)
        {
            var hoja = new HojaExcel("Estado Resultado") { ColumnasEncabezado = 2 };
            hoja.AnchoColumnas.AddRange(new[] { 50.0, 16.0 });
            hoja.Filas.Add(FilaTitulo("Estado Resultado"));
            filasClave = new Dictionary<string, int>();

            hoja.Filas.Add(Fila2("INGRESOS", string.Empty, EstiloCelda.Encabezado));
            int inicioIngresos = hoja.Filas.Count + 1;
            foreach (var linea in er.Ingresos)
            {
                var celda = CeldaDesdeLibroMayor(linea, nombreHojaLibroMayor, filaSumaPorCuenta, haberMenosDebe: true);
                hoja.Filas.Add(new List<CeldaExcel> { CeldaExcel.DeTexto(linea.Nombre), celda });
            }
            int finIngresos = hoja.Filas.Count;

            hoja.Filas.Add(new List<CeldaExcel>
            {
                CeldaExcel.DeTexto("Total Ingresos", EstiloCelda.Total),
                FormulaSumaOCero(inicioIngresos, finIngresos, 1, er.TotalIngresos, EstiloCelda.Total)
            });
            int filaTotalIngresos = hoja.Filas.Count;

            hoja.Filas.Add(Fila2("GASTOS OPERATIVOS Y NO OPERATIVOS (-)", string.Empty, EstiloCelda.Encabezado));
            int inicioGastos = hoja.Filas.Count + 1;
            foreach (var linea in er.Gastos)
            {
                var celda = CeldaDesdeLibroMayor(linea, nombreHojaLibroMayor, filaSumaPorCuenta, haberMenosDebe: false);
                hoja.Filas.Add(new List<CeldaExcel> { CeldaExcel.DeTexto(linea.Nombre), celda });
            }
            int finGastos = hoja.Filas.Count;

            hoja.Filas.Add(new List<CeldaExcel>
            {
                CeldaExcel.DeTexto("Total Gastos", EstiloCelda.Total),
                FormulaSumaOCero(inicioGastos, finGastos, 1, er.TotalGastos, EstiloCelda.Total)
            });
            int filaTotalGastos = hoja.Filas.Count;

            hoja.Filas.Add(Fila2("CÁLCULO DE UTILIDAD", string.Empty, EstiloCelda.Encabezado));

            string dirTotalIngresos = ExcelWriter.Direccion(filaTotalIngresos, 1);
            string dirTotalGastos = ExcelWriter.Direccion(filaTotalGastos, 1);
            hoja.Filas.Add(new List<CeldaExcel>
            {
                CeldaExcel.DeTexto("Utilidad antes de participación"),
                CeldaExcel.DeFormulaNumero(dirTotalIngresos + "-" + dirTotalGastos, er.UtilidadAntesParticipacion)
            });
            string dirUtilidadAntesParticipacion = ExcelWriter.Direccion(hoja.Filas.Count, 1);

            hoja.Filas.Add(new List<CeldaExcel>
            {
                CeldaExcel.DeTexto("(-) 15% Participación Trabajadores"),
                CeldaExcel.DeFormulaNumero(
                    string.Format("-IF({0}>0,ROUND({0}*0.15,2),0)", dirUtilidadAntesParticipacion),
                    -er.Participacion15)
            });
            int filaParticipacion15 = hoja.Filas.Count;
            string dirParticipacion15 = ExcelWriter.Direccion(filaParticipacion15, 1);

            hoja.Filas.Add(new List<CeldaExcel>
            {
                CeldaExcel.DeTexto("Utilidad antes de Impuesto a la Renta"),
                CeldaExcel.DeFormulaNumero(dirUtilidadAntesParticipacion + "+" + dirParticipacion15, er.UtilidadAntesImpuesto)
            });
            string dirUtilidadAntesImpuesto = ExcelWriter.Direccion(hoja.Filas.Count, 1);

            hoja.Filas.Add(new List<CeldaExcel>
            {
                CeldaExcel.DeTexto("(-) 25% Impuesto a la Renta"),
                CeldaExcel.DeFormulaNumero(
                    string.Format("-IF({0}>0,ROUND({0}*0.25,2),0)", dirUtilidadAntesImpuesto),
                    -er.ImpuestoRenta25)
            });
            int filaImpuesto25 = hoja.Filas.Count;
            string dirImpuesto25 = ExcelWriter.Direccion(filaImpuesto25, 1);

            hoja.Filas.Add(new List<CeldaExcel>
            {
                CeldaExcel.DeTexto("Utilidad del Ejercicio", EstiloCelda.Total),
                CeldaExcel.DeFormulaNumero(dirUtilidadAntesImpuesto + "+" + dirImpuesto25, er.UtilidadEjercicio, EstiloCelda.Total)
            });
            int filaUtilidadEjercicio = hoja.Filas.Count;

            filasClave["Participacion15"] = filaParticipacion15;
            filasClave["ImpuestoRenta25"] = filaImpuesto25;
            filasClave["UtilidadEjercicio"] = filaUtilidadEjercicio;

            return hoja;
        }

        // ---------------------------------------------------------------
        // Balance General
        // ---------------------------------------------------------------

        private static HojaExcel ConstruirHojaBalanceGeneral(BalanceGeneral bg, string nombreHojaEstadoResultado, Dictionary<string, int> filasEstadoResultado, string nombreHojaLibroMayor, Dictionary<string, int> filaSumaPorCuenta)
        {
            var hoja = new HojaExcel("Balance General") { ColumnasEncabezado = 2 };
            hoja.AnchoColumnas.AddRange(new[] { 50.0, 16.0 });
            hoja.Filas.Add(FilaTitulo("Balance General"));

            hoja.Filas.Add(Fila2("ACTIVOS", string.Empty, EstiloCelda.Encabezado));
            hoja.Filas.Add(Fila2("Activos Corrientes", string.Empty, EstiloCelda.Normal));
            int inicioAC = hoja.Filas.Count + 1;
            foreach (var linea in bg.ActivosCorrientes)
            {
                var celda = CeldaDesdeLibroMayor(linea, nombreHojaLibroMayor, filaSumaPorCuenta, haberMenosDebe: false);
                hoja.Filas.Add(new List<CeldaExcel> { CeldaExcel.DeTexto("    " + linea.Nombre), celda });
            }
            int finAC = hoja.Filas.Count;
            hoja.Filas.Add(new List<CeldaExcel>
            {
                CeldaExcel.DeTexto("Total Activos Corrientes", EstiloCelda.Total),
                FormulaSumaOCero(inicioAC, finAC, 1, bg.TotalActivoCorriente, EstiloCelda.Total)
            });
            int filaTotalAC = hoja.Filas.Count;

            hoja.Filas.Add(Fila2("Activos No Corrientes", string.Empty, EstiloCelda.Normal));
            int inicioANC = hoja.Filas.Count + 1;
            foreach (var linea in bg.ActivosNoCorrientes)
            {
                var celda = CeldaDesdeLibroMayor(linea, nombreHojaLibroMayor, filaSumaPorCuenta, haberMenosDebe: false);
                hoja.Filas.Add(new List<CeldaExcel> { CeldaExcel.DeTexto("    " + linea.Nombre), celda });
            }
            int finANC = hoja.Filas.Count;
            hoja.Filas.Add(new List<CeldaExcel>
            {
                CeldaExcel.DeTexto("Total Activos No Corrientes", EstiloCelda.Total),
                FormulaSumaOCero(inicioANC, finANC, 1, bg.TotalActivoNoCorriente, EstiloCelda.Total)
            });
            int filaTotalANC = hoja.Filas.Count;

            string dirTotalAC = ExcelWriter.Direccion(filaTotalAC, 1);
            string dirTotalANC = ExcelWriter.Direccion(filaTotalANC, 1);
            hoja.Filas.Add(new List<CeldaExcel>
            {
                CeldaExcel.DeTexto("TOTAL ACTIVOS", EstiloCelda.Total),
                CeldaExcel.DeFormulaNumero(dirTotalAC + "+" + dirTotalANC, bg.TotalActivo, EstiloCelda.Total)
            });
            int filaTotalActivos = hoja.Filas.Count;

            hoja.Filas.Add(Fila2("PASIVOS", string.Empty, EstiloCelda.Encabezado));
            hoja.Filas.Add(Fila2("Pasivos Corrientes", string.Empty, EstiloCelda.Normal));
            int inicioPC = hoja.Filas.Count + 1;
            foreach (var linea in bg.PasivosCorrientes)
            {
                var celda = CeldaPasivoOPatrimonioCalculado(linea, nombreHojaEstadoResultado, filasEstadoResultado, nombreHojaLibroMayor, filaSumaPorCuenta);
                hoja.Filas.Add(new List<CeldaExcel> { CeldaExcel.DeTexto("    " + linea.Nombre), celda });
            }
            int finPC = hoja.Filas.Count;
            hoja.Filas.Add(new List<CeldaExcel>
            {
                CeldaExcel.DeTexto("Total Pasivos Corrientes", EstiloCelda.Total),
                FormulaSumaOCero(inicioPC, finPC, 1, bg.TotalPasivoCorriente, EstiloCelda.Total)
            });
            int filaTotalPC = hoja.Filas.Count;

            hoja.Filas.Add(Fila2("Pasivos No Corrientes", string.Empty, EstiloCelda.Normal));
            int inicioPNC = hoja.Filas.Count + 1;
            foreach (var linea in bg.PasivosNoCorrientes)
            {
                var celda = CeldaDesdeLibroMayor(linea, nombreHojaLibroMayor, filaSumaPorCuenta, haberMenosDebe: true);
                hoja.Filas.Add(new List<CeldaExcel> { CeldaExcel.DeTexto("    " + linea.Nombre), celda });
            }
            int finPNC = hoja.Filas.Count;
            hoja.Filas.Add(new List<CeldaExcel>
            {
                CeldaExcel.DeTexto("Total Pasivos No Corrientes", EstiloCelda.Total),
                FormulaSumaOCero(inicioPNC, finPNC, 1, bg.TotalPasivoNoCorriente, EstiloCelda.Total)
            });
            int filaTotalPNC = hoja.Filas.Count;

            string dirTotalPC = ExcelWriter.Direccion(filaTotalPC, 1);
            string dirTotalPNC = ExcelWriter.Direccion(filaTotalPNC, 1);
            hoja.Filas.Add(new List<CeldaExcel>
            {
                CeldaExcel.DeTexto("TOTAL PASIVOS", EstiloCelda.Total),
                CeldaExcel.DeFormulaNumero(dirTotalPC + "+" + dirTotalPNC, bg.TotalPasivo, EstiloCelda.Total)
            });
            int filaTotalPasivos = hoja.Filas.Count;

            hoja.Filas.Add(Fila2("PATRIMONIO", string.Empty, EstiloCelda.Encabezado));
            int inicioPat = hoja.Filas.Count + 1;
            foreach (var linea in bg.Patrimonio)
            {
                var celda = CeldaPasivoOPatrimonioCalculado(linea, nombreHojaEstadoResultado, filasEstadoResultado, nombreHojaLibroMayor, filaSumaPorCuenta);
                hoja.Filas.Add(new List<CeldaExcel> { CeldaExcel.DeTexto(linea.Nombre), celda });
            }
            int finPat = hoja.Filas.Count;
            hoja.Filas.Add(new List<CeldaExcel>
            {
                CeldaExcel.DeTexto("TOTAL PATRIMONIO", EstiloCelda.Total),
                FormulaSumaOCero(inicioPat, finPat, 1, bg.TotalPatrimonio, EstiloCelda.Total)
            });
            int filaTotalPatrimonio = hoja.Filas.Count;

            string dirTotalPasivos = ExcelWriter.Direccion(filaTotalPasivos, 1);
            string dirTotalPatrimonio = ExcelWriter.Direccion(filaTotalPatrimonio, 1);
            hoja.Filas.Add(new List<CeldaExcel>
            {
                CeldaExcel.DeTexto("TOTAL PASIVOS + PATRIMONIO", EstiloCelda.Total),
                CeldaExcel.DeFormulaNumero(dirTotalPasivos + "+" + dirTotalPatrimonio, bg.TotalPasivoMasPatrimonio, EstiloCelda.Total)
            });
            int filaTotalPP = hoja.Filas.Count;

            hoja.Filas.Add(Fila2("ECUACIÓN CONTABLE: Activo = Pasivo + Patrimonio", string.Empty, EstiloCelda.Encabezado));

            string dirTotalActivos = ExcelWriter.Direccion(filaTotalActivos, 1);
            string dirTotalPP = ExcelWriter.Direccion(filaTotalPP, 1);
            string formulaEcuacion = string.Format(
                "TEXT({0},\"#,##0.00\")&\" \"&IF({0}={1},\"=\",\"≠\")&\" \"&TEXT({1},\"#,##0.00\")",
                dirTotalActivos, dirTotalPP);
            string textoEcuacionCache = string.Format("{0:N2} {1} {2:N2}", bg.TotalActivo, bg.Cuadra ? "=" : "≠", bg.TotalPasivoMasPatrimonio);
            hoja.Filas.Add(new List<CeldaExcel>
            {
                CeldaExcel.DeFormulaTexto(formulaEcuacion, textoEcuacionCache, bg.Cuadra ? EstiloCelda.Total : EstiloCelda.Normal),
                CeldaExcel.DeTexto(string.Empty)
            });

            return hoja;
        }

        /// <summary>
        /// Las lineas de "15% Participación..."/"25% Impuesto..." (en Pasivos Corrientes) y
        /// "Utilidad/Pérdida del Ejercicio" (en Patrimonio) las agrega BalanceGeneralService
        /// tomando el valor del Estado de Resultados: aqui se reemplazan por una referencia
        /// a esa hoja en vez de dejarlas como numero suelto, para que quede conectado.
        /// </summary>
        private static CeldaExcel CeldaPasivoOPatrimonioCalculado(LineaCuentaMonto linea, string nombreHojaEstadoResultado, Dictionary<string, int> filasEstadoResultado,
            string nombreHojaLibroMayor, Dictionary<string, int> filaSumaPorCuenta)
        {
            int filaEstadoResultado;

            if (linea.Nombre.StartsWith("15% Participación") && filasEstadoResultado.TryGetValue("Participacion15", out filaEstadoResultado))
            {
                string refCelda = ExcelWriter.EnHoja(nombreHojaEstadoResultado, ExcelWriter.Direccion(filaEstadoResultado, 1));
                return CeldaExcel.DeFormulaNumero("-" + refCelda, linea.Monto);
            }

            if (linea.Nombre.StartsWith("25% Impuesto") && filasEstadoResultado.TryGetValue("ImpuestoRenta25", out filaEstadoResultado))
            {
                string refCelda = ExcelWriter.EnHoja(nombreHojaEstadoResultado, ExcelWriter.Direccion(filaEstadoResultado, 1));
                return CeldaExcel.DeFormulaNumero("-" + refCelda, linea.Monto);
            }

            if ((linea.Nombre == "Utilidad del Ejercicio" || linea.Nombre == "Pérdida del Ejercicio")
                && filasEstadoResultado.TryGetValue("UtilidadEjercicio", out filaEstadoResultado))
            {
                string refCelda = ExcelWriter.EnHoja(nombreHojaEstadoResultado, ExcelWriter.Direccion(filaEstadoResultado, 1));
                return CeldaExcel.DeFormulaNumero(refCelda, linea.Monto);
            }

            // No es una linea calculada: es una cuenta real (Proveedores, Capital Social, etc.),
            // asi que se conecta directo al Libro Mayor igual que las demas.
            return CeldaDesdeLibroMayor(linea, nombreHojaLibroMayor, filaSumaPorCuenta, haberMenosDebe: true);
        }

        /// <summary>
        /// Convierte una linea de Estado de Resultados/Balance General en una formula que
        /// apunta a la fila Σ de esa cuenta en el Libro Mayor (Haber-Debe o Debe-Haber segun
        /// corresponda), para poder rastrear de donde sale cada numero. Si la linea no tiene
        /// codigo de cuenta o esa cuenta no aparece en el Libro Mayor, se deja el numero fijo.
        /// </summary>
        private static CeldaExcel CeldaDesdeLibroMayor(LineaCuentaMonto linea, string nombreHojaLibroMayor, Dictionary<string, int> filaSumaPorCuenta, bool haberMenosDebe)
        {
            int filaLibroMayor;
            if (string.IsNullOrEmpty(linea.Codigo) || !filaSumaPorCuenta.TryGetValue(linea.Codigo, out filaLibroMayor))
            {
                return CeldaExcel.DeNumero(linea.Monto);
            }

            string dirDebe = ExcelWriter.EnHoja(nombreHojaLibroMayor, ExcelWriter.Direccion(filaLibroMayor, 1));
            string dirHaber = ExcelWriter.EnHoja(nombreHojaLibroMayor, ExcelWriter.Direccion(filaLibroMayor, 3));
            string formula = haberMenosDebe ? (dirHaber + "-" + dirDebe) : (dirDebe + "-" + dirHaber);
            return CeldaExcel.DeFormulaNumero(formula, linea.Monto);
        }

        // ---------------------------------------------------------------
        // Cuentas
        // ---------------------------------------------------------------

        private static HojaExcel ConstruirHojaCuentas(List<Cuenta> cuentas)
        {
            var hoja = new HojaExcel("Cuentas") { ColumnasEncabezado = 4 };
            hoja.AnchoColumnas.AddRange(new[] { 12.0, 40.0, 22.0, 14.0 });
            hoja.Filas.Add(FilaTitulo("Cuentas"));
            hoja.Filas.Add(Fila(EstiloCelda.Encabezado, "Código", "Nombre", "Tipo", "Naturaleza"));

            foreach (var cuenta in cuentas.OrderBy(c => c.Codigo))
            {
                hoja.Filas.Add(Fila(EstiloCelda.Normal, cuenta.Codigo, cuenta.Nombre, cuenta.Tipo.ToTextoAmigable(), cuenta.Naturaleza.ToString()));
            }

            return hoja;
        }

        // ---------------------------------------------------------------
        // Helpers comunes
        // ---------------------------------------------------------------

        private static List<CeldaExcel> FilaTitulo(string texto)
        {
            return new List<CeldaExcel> { CeldaExcel.DeTexto(texto, EstiloCelda.Titulo) };
        }

        private static List<CeldaExcel> Fila(EstiloCelda estilo, params string[] textos)
        {
            return textos.Select(t => CeldaExcel.DeTexto(t, estilo)).ToList();
        }

        private static List<CeldaExcel> Fila2(string concepto, string valorTexto, EstiloCelda estilo)
        {
            return new List<CeldaExcel> { CeldaExcel.DeTexto(concepto, estilo), CeldaExcel.DeTexto(valorTexto, estilo) };
        }

        /// <summary>SUMA(rango) si hay al menos una fila en el rango; si no, 0 fijo (evita un rango invertido cuando una seccion no tiene lineas).</summary>
        private static CeldaExcel FormulaSumaOCero(int filaInicio, int filaFin, int columna0, decimal valorCache, EstiloCelda estilo = EstiloCelda.Normal)
        {
            if (filaFin < filaInicio) return CeldaExcel.DeNumero(0, estilo);
            return CeldaExcel.DeFormulaNumero("SUM(" + ExcelWriter.Rango(filaInicio, filaFin, columna0) + ")", valorCache, estilo);
        }

        /// <summary>
        /// Registra, en el orden en que se van escribiendo, la celda exacta (con hoja incluida,
        /// ej. "'Libro Diario'!D7") de cada movimiento de Debe/Haber por cuenta, mientras se arma
        /// Libro Diario y Libro Ajustes. Libro Mayor despues va sacando esas direcciones en el
        /// mismo orden en que LibroMayorService arma sus propias listas de movimientos (mismos
        /// asientos, mismo orden), para que cada celda de Libro Mayor sea una formula que apunta
        /// directo al Libro Diario/Ajustes en vez de un numero repetido.
        /// </summary>
        private class MapaMovimientos
        {
            private readonly Dictionary<string, Queue<string>> _debe = new Dictionary<string, Queue<string>>();
            private readonly Dictionary<string, Queue<string>> _haber = new Dictionary<string, Queue<string>>();

            public void RegistrarDebe(string codigoCuenta, string referencia)
            {
                Agregar(_debe, codigoCuenta, referencia);
            }

            public void RegistrarHaber(string codigoCuenta, string referencia)
            {
                Agregar(_haber, codigoCuenta, referencia);
            }

            public string SiguienteDebe(string codigoCuenta)
            {
                return Siguiente(_debe, codigoCuenta);
            }

            public string SiguienteHaber(string codigoCuenta)
            {
                return Siguiente(_haber, codigoCuenta);
            }

            private static void Agregar(Dictionary<string, Queue<string>> mapa, string codigo, string referencia)
            {
                Queue<string> cola;
                if (!mapa.TryGetValue(codigo, out cola))
                {
                    cola = new Queue<string>();
                    mapa[codigo] = cola;
                }
                cola.Enqueue(referencia);
            }

            private static string Siguiente(Dictionary<string, Queue<string>> mapa, string codigo)
            {
                Queue<string> cola;
                if (mapa.TryGetValue(codigo, out cola) && cola.Count > 0) return cola.Dequeue();
                return null;
            }
        }
    }
}
