using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace Contabilidad.Data
{
    /// <summary>
    /// Estilo visual de una celda al exportar a Excel: los mismos que ya se usan en las
    /// grillas de la app (ver GridStyleHelper / ReporteFinancieroDataGridView), mas Titulo
    /// para el nombre de la hoja en grande arriba de todo.
    /// </summary>
    public enum EstiloCelda
    {
        Normal,
        Encabezado,
        Total,
        Titulo
    }

    internal enum TipoValorCelda
    {
        Texto,
        Numero,
        FormulaNumero,
        FormulaTexto
    }

    public class CeldaExcel
    {
        public string Texto { get; private set; }
        public decimal Valor { get; private set; }
        public string Formula { get; private set; }
        public EstiloCelda Estilo { get; private set; }
        internal TipoValorCelda TipoValor { get; private set; }

        private CeldaExcel() { }

        public static CeldaExcel DeTexto(string texto, EstiloCelda estilo = EstiloCelda.Normal)
        {
            return new CeldaExcel { Texto = texto ?? string.Empty, Estilo = estilo, TipoValor = TipoValorCelda.Texto };
        }

        public static CeldaExcel DeNumero(decimal valor, EstiloCelda estilo = EstiloCelda.Normal)
        {
            return new CeldaExcel { Valor = valor, Estilo = estilo, TipoValor = TipoValorCelda.Numero };
        }

        /// <summary>Formula cuyo resultado es un numero/monto (ej. SUM(D2:D10)).</summary>
        public static CeldaExcel DeFormulaNumero(string formula, decimal valorCacheado, EstiloCelda estilo = EstiloCelda.Normal)
        {
            return new CeldaExcel { Formula = formula, Valor = valorCacheado, Estilo = estilo, TipoValor = TipoValorCelda.FormulaNumero };
        }

        /// <summary>Formula cuyo resultado es texto (ej. IF(D4=0,"CUADRADO","DESCUADRADO")).</summary>
        public static CeldaExcel DeFormulaTexto(string formula, string textoCacheado, EstiloCelda estilo = EstiloCelda.Normal)
        {
            return new CeldaExcel { Formula = formula, Texto = textoCacheado ?? string.Empty, Estilo = estilo, TipoValor = TipoValorCelda.FormulaTexto };
        }
    }

    public class HojaExcel
    {
        public string Nombre { get; set; }
        public List<List<CeldaExcel>> Filas { get; set; }

        /// <summary>Cuantas columnas debe abarcar la fila de titulo (fusionada) en la fila 1.</summary>
        public int ColumnasEncabezado { get; set; }

        /// <summary>Ancho de cada columna, en caracteres. Si falta alguna, se usa un ancho por defecto.</summary>
        public List<double> AnchoColumnas { get; set; }

        public HojaExcel(string nombre)
        {
            Nombre = nombre;
            Filas = new List<List<CeldaExcel>>();
            AnchoColumnas = new List<double>();
        }
    }

    /// <summary>
    /// Arma un archivo .xlsx valido a mano (un .xlsx es un .zip con archivos XML adentro),
    /// sin depender de ninguna libreria externa. Los totales/subtotales se escriben como
    /// formulas reales de Excel (con su valor ya calculado como cache, por si algo no
    /// recalcula al abrir el archivo).
    ///
    /// IMPORTANTE: las formulas se guardan siempre con nombres de funcion en ingles
    /// (SUM, IF, ROUND, COUNTA, TEXT, ABS) y "." como separador decimal: asi es como el
    /// formato .xlsx graba las formulas SIEMPRE, sin importar el idioma de Excel de quien
    /// lo abre despues. Excel las traduce solo (a SUMA, SI, etc.) al mostrarlas.
    /// </summary>
    public static class ExcelWriter
    {
        private const double AnchoColumnaPorDefecto = 14;

        public static void Exportar(string ruta, List<HojaExcel> hojas)
        {
            using (var stream = new FileStream(ruta, FileMode.Create, FileAccess.Write))
            using (var archivo = new ZipArchive(stream, ZipArchiveMode.Create))
            {
                EscribirEntrada(archivo, "[Content_Types].xml", ConstruirContentTypes(hojas.Count));
                EscribirEntrada(archivo, "_rels/.rels", ConstruirRelsRaiz());
                EscribirEntrada(archivo, "xl/workbook.xml", ConstruirWorkbook(hojas));
                EscribirEntrada(archivo, "xl/_rels/workbook.xml.rels", ConstruirWorkbookRels(hojas.Count));
                EscribirEntrada(archivo, "xl/styles.xml", ConstruirStyles());

                for (int i = 0; i < hojas.Count; i++)
                {
                    string nombreEntrada = string.Format("xl/worksheets/sheet{0}.xml", i + 1);
                    EscribirEntrada(archivo, nombreEntrada, ConstruirHoja(hojas[i]));
                }
            }
        }

        /// <summary>Direccion de celda tipo "D5" a partir de fila (1-based) y columna (0-based).</summary>
        public static string Direccion(int fila, int columna0)
        {
            return ColumnaExcel(columna0) + fila.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>Rango tipo "D2:D10" para una misma columna, entre dos filas (1-based).</summary>
        public static string Rango(int filaInicio, int filaFin, int columna0)
        {
            return Direccion(filaInicio, columna0) + ":" + Direccion(filaFin, columna0);
        }

        /// <summary>Antepone el nombre de hoja (entre comillas simples) a una direccion o rango, para referencias entre hojas.</summary>
        public static string EnHoja(string nombreHoja, string direccionORango)
        {
            return "'" + nombreHoja + "'!" + direccionORango;
        }

        private static void EscribirEntrada(ZipArchive archivo, string nombre, string contenido)
        {
            var entrada = archivo.CreateEntry(nombre, CompressionLevel.Fastest);
            using (var escritor = new StreamWriter(entrada.Open(), new UTF8Encoding(false)))
            {
                escritor.Write(contenido);
            }
        }

        private static string ConstruirContentTypes(int cantidadHojas)
        {
            var sb = new StringBuilder();
            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
            sb.Append("<Types xmlns=\"http://schemas.openxmlformats.org/package/2006/content-types\">");
            sb.Append("<Default Extension=\"rels\" ContentType=\"application/vnd.openxmlformats-package.relationships+xml\"/>");
            sb.Append("<Default Extension=\"xml\" ContentType=\"application/xml\"/>");
            sb.Append("<Override PartName=\"/xl/workbook.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml\"/>");
            sb.Append("<Override PartName=\"/xl/styles.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml\"/>");
            for (int i = 1; i <= cantidadHojas; i++)
            {
                sb.AppendFormat("<Override PartName=\"/xl/worksheets/sheet{0}.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml\"/>", i);
            }
            sb.Append("</Types>");
            return sb.ToString();
        }

        private static string ConstruirRelsRaiz()
        {
            return "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>"
                 + "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">"
                 + "<Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument\" Target=\"xl/workbook.xml\"/>"
                 + "</Relationships>";
        }

        private static string ConstruirWorkbook(List<HojaExcel> hojas)
        {
            var sb = new StringBuilder();
            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
            sb.Append("<workbook xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\">");
            sb.Append("<sheets>");
            for (int i = 0; i < hojas.Count; i++)
            {
                sb.AppendFormat("<sheet name=\"{0}\" sheetId=\"{1}\" r:id=\"rId{1}\"/>", EscaparXml(NormalizarNombreHoja(hojas[i].Nombre)), i + 1);
            }
            sb.Append("</sheets>");
            sb.Append("<calcPr fullCalcOnLoad=\"1\"/>");
            sb.Append("</workbook>");
            return sb.ToString();
        }

        private static string ConstruirWorkbookRels(int cantidadHojas)
        {
            var sb = new StringBuilder();
            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
            sb.Append("<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">");
            for (int i = 1; i <= cantidadHojas; i++)
            {
                sb.AppendFormat("<Relationship Id=\"rId{0}\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet\" Target=\"worksheets/sheet{0}.xml\"/>", i);
            }
            sb.AppendFormat("<Relationship Id=\"rId{0}\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles\" Target=\"styles.xml\"/>", cantidadHojas + 1);
            sb.Append("</Relationships>");
            return sb.ToString();
        }

        /// <summary>
        /// Estilos: 0=Normal texto, 1=Normal moneda, 2=Encabezado texto, 3=Encabezado moneda,
        /// 4=Total texto, 5=Total moneda, 6=Titulo. Todos con borde delgado salvo Titulo.
        /// numFmtId=4 es el formato integrado de Excel "#,##0.00".
        /// </summary>
        private static string ConstruirStyles()
        {
            return "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>"
                 + "<styleSheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\">"
                 + "<fonts count=\"4\">"
                 + "<font><name val=\"Calibri\"/><sz val=\"10\"/></font>"
                 + "<font><name val=\"Calibri\"/><b/><color rgb=\"FFFFFFFF\"/><sz val=\"10\"/></font>"
                 + "<font><name val=\"Calibri\"/><b/><sz val=\"10\"/></font>"
                 + "<font><name val=\"Calibri\"/><b/><sz val=\"14\"/></font>"
                 + "</fonts>"
                 // Los indices 0 y 1 estan reservados por Excel (siempre "ninguno" y "gray125",
                 // sin importar lo que se declare ahi); los rellenos propios empiezan en el 2.
                 + "<fills count=\"4\">"
                 + "<fill><patternFill patternType=\"none\"/></fill>"
                 + "<fill><patternFill patternType=\"gray125\"/></fill>"
                 + "<fill><patternFill patternType=\"solid\"><fgColor rgb=\"FF375623\"/><bgColor rgb=\"FF375623\"/></patternFill></fill>"
                 + "<fill><patternFill patternType=\"solid\"><fgColor rgb=\"FFF2F2F2\"/><bgColor rgb=\"FFF2F2F2\"/></patternFill></fill>"
                 + "</fills>"
                 + "<borders count=\"2\">"
                 + "<border><left/><right/><top/><bottom/><diagonal/></border>"
                 + "<border>"
                 + "<left style=\"thin\"><color indexed=\"64\"/></left>"
                 + "<right style=\"thin\"><color indexed=\"64\"/></right>"
                 + "<top style=\"thin\"><color indexed=\"64\"/></top>"
                 + "<bottom style=\"thin\"><color indexed=\"64\"/></bottom>"
                 + "<diagonal/>"
                 + "</border>"
                 + "</borders>"
                 + "<cellStyleXfs count=\"1\"><xf numFmtId=\"0\" fontId=\"0\" fillId=\"0\" borderId=\"0\"/></cellStyleXfs>"
                 + "<cellXfs count=\"7\">"
                 + "<xf numFmtId=\"0\" fontId=\"0\" fillId=\"0\" borderId=\"1\" xfId=\"0\" applyBorder=\"1\"><alignment vertical=\"center\"/></xf>"
                 + "<xf numFmtId=\"4\" fontId=\"0\" fillId=\"0\" borderId=\"1\" xfId=\"0\" applyBorder=\"1\" applyNumberFormat=\"1\"><alignment horizontal=\"right\" vertical=\"center\"/></xf>"
                 + "<xf numFmtId=\"0\" fontId=\"1\" fillId=\"2\" borderId=\"1\" xfId=\"0\" applyFont=\"1\" applyFill=\"1\" applyBorder=\"1\"><alignment horizontal=\"center\" vertical=\"center\"/></xf>"
                 + "<xf numFmtId=\"4\" fontId=\"1\" fillId=\"2\" borderId=\"1\" xfId=\"0\" applyFont=\"1\" applyFill=\"1\" applyBorder=\"1\" applyNumberFormat=\"1\"><alignment horizontal=\"right\" vertical=\"center\"/></xf>"
                 + "<xf numFmtId=\"0\" fontId=\"2\" fillId=\"3\" borderId=\"1\" xfId=\"0\" applyFont=\"1\" applyFill=\"1\" applyBorder=\"1\"><alignment vertical=\"center\"/></xf>"
                 + "<xf numFmtId=\"4\" fontId=\"2\" fillId=\"3\" borderId=\"1\" xfId=\"0\" applyFont=\"1\" applyFill=\"1\" applyBorder=\"1\" applyNumberFormat=\"1\"><alignment horizontal=\"right\" vertical=\"center\"/></xf>"
                 + "<xf numFmtId=\"0\" fontId=\"3\" fillId=\"0\" borderId=\"0\" xfId=\"0\" applyFont=\"1\"><alignment horizontal=\"center\" vertical=\"center\"/></xf>"
                 + "</cellXfs>"
                 + "<cellStyles count=\"1\"><cellStyle name=\"Normal\" xfId=\"0\" builtinId=\"0\"/></cellStyles>"
                 + "</styleSheet>";
        }

        private static int IndiceEstilo(EstiloCelda estilo, bool moneda)
        {
            switch (estilo)
            {
                case EstiloCelda.Encabezado: return moneda ? 3 : 2;
                case EstiloCelda.Total: return moneda ? 5 : 4;
                case EstiloCelda.Titulo: return 6;
                default: return moneda ? 1 : 0;
            }
        }

        private static string ConstruirHoja(HojaExcel hoja)
        {
            var sb = new StringBuilder();
            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
            sb.Append("<worksheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\">");

            int totalColumnas = hoja.Filas.Count == 0 ? 0 : hoja.Filas.Max(f => f.Count);
            if (totalColumnas > 0)
            {
                sb.Append("<cols>");
                for (int c = 0; c < totalColumnas; c++)
                {
                    double ancho = c < hoja.AnchoColumnas.Count ? hoja.AnchoColumnas[c] : AnchoColumnaPorDefecto;
                    sb.AppendFormat(CultureInfo.InvariantCulture,
                        "<col min=\"{0}\" max=\"{0}\" width=\"{1}\" customWidth=\"1\"/>", c + 1, ancho);
                }
                sb.Append("</cols>");
            }

            sb.Append("<sheetData>");

            for (int f = 0; f < hoja.Filas.Count; f++)
            {
                int numeroFila = f + 1;
                sb.AppendFormat("<row r=\"{0}\">", numeroFila);
                var fila = hoja.Filas[f];
                for (int c = 0; c < fila.Count; c++)
                {
                    EscribirCelda(sb, fila[c], numeroFila, c);
                }
                sb.Append("</row>");
            }

            sb.Append("</sheetData>");

            if (hoja.ColumnasEncabezado > 1)
            {
                sb.Append("<mergeCells count=\"1\">");
                sb.AppendFormat("<mergeCell ref=\"A1:{0}\"/>", Direccion(1, hoja.ColumnasEncabezado - 1));
                sb.Append("</mergeCells>");
            }

            sb.Append("</worksheet>");
            return sb.ToString();
        }

        private static void EscribirCelda(StringBuilder sb, CeldaExcel celda, int fila, int columna0)
        {
            string referencia = Direccion(fila, columna0);

            switch (celda.TipoValor)
            {
                case TipoValorCelda.Texto:
                    sb.AppendFormat("<c r=\"{0}\" t=\"inlineStr\" s=\"{1}\"><is><t xml:space=\"preserve\">{2}</t></is></c>",
                        referencia, IndiceEstilo(celda.Estilo, false), EscaparXml(celda.Texto));
                    break;

                case TipoValorCelda.Numero:
                    sb.AppendFormat(CultureInfo.InvariantCulture, "<c r=\"{0}\" s=\"{1}\"><v>{2}</v></c>",
                        referencia, IndiceEstilo(celda.Estilo, true), celda.Valor);
                    break;

                case TipoValorCelda.FormulaNumero:
                    sb.AppendFormat(CultureInfo.InvariantCulture, "<c r=\"{0}\" s=\"{1}\"><f>{2}</f><v>{3}</v></c>",
                        referencia, IndiceEstilo(celda.Estilo, true), EscaparXml(celda.Formula), celda.Valor);
                    break;

                case TipoValorCelda.FormulaTexto:
                    sb.AppendFormat("<c r=\"{0}\" t=\"str\" s=\"{1}\"><f>{2}</f><v>{3}</v></c>",
                        referencia, IndiceEstilo(celda.Estilo, false), EscaparXml(celda.Formula), EscaparXml(celda.Texto));
                    break;
            }
        }

        /// <summary>Convierte un indice de columna (0 = A, 1 = B, ..., 26 = AA) a letra de Excel.</summary>
        private static string ColumnaExcel(int indice)
        {
            string resultado = string.Empty;
            int n = indice;
            do
            {
                resultado = (char)('A' + (n % 26)) + resultado;
                n = (n / 26) - 1;
            } while (n >= 0);
            return resultado;
        }

        private static string NormalizarNombreHoja(string nombre)
        {
            string limpio = nombre;
            foreach (char invalido in new[] { ':', '\\', '/', '?', '*', '[', ']' })
            {
                limpio = limpio.Replace(invalido, ' ');
            }
            return limpio.Length > 31 ? limpio.Substring(0, 31) : limpio;
        }

        private static string EscaparXml(string texto)
        {
            return texto
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&apos;");
        }
    }
}
