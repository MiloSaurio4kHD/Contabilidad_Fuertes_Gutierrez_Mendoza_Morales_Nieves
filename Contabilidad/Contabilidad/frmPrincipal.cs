using Contabilidad.Data;
using Contabilidad.Models;
using Contabilidad.UI;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace Contabilidad
{
    public partial class frmPrincipal : Form
    {
        private readonly CuentaRepository _cuentaRepository = new CuentaRepository();
        private readonly LibroDiarioRepository _libroDiarioRepository = new LibroDiarioRepository();
        private readonly AjusteRepository _ajusteRepository = new AjusteRepository();
        private readonly LibroMayorService _libroMayorService = new LibroMayorService();
        private readonly EstadoResultadoService _estadoResultadoService = new EstadoResultadoService();
        private readonly BalanceGeneralService _balanceGeneralService = new BalanceGeneralService();
        private System.Collections.Generic.List<CuentaMayor> _ultimoLibroMayor = new System.Collections.Generic.List<CuentaMayor>();
        private readonly ResumenLibroDataGridView resumenLibroDiario = new ResumenLibroDataGridView();
        private readonly ResumenLibroDataGridView resumenLibroAjustes = new ResumenLibroDataGridView();

        public frmPrincipal()
        {
            InitializeComponent();
            pnlLibroMayor.Resize += (sender, e) => RenderizarLibroMayor();
            ConfigurarIconos();
            ConfigurarResumenes();
        }

        /// <summary>
        /// Agrega el cuadro de Resumen (Total Debe/Haber, Diferencia, N° de asientos y
        /// Estado) debajo de los campos de Saldo debe/haber, tanto en Libro Diario como
        /// en Libro Ajustes. Se agrega por codigo (no en el Designer): arrastrarlo como
        /// control del Designer causaba que el formulario se dañara al abrirlo ahi, asi
        /// que su posicion se fija aqui. Si hace falta moverlo, se ajustan estas dos lineas.
        /// </summary>
        private void ConfigurarResumenes()
        {
            if (GridStyleHelper.EnDisenio) return;

            resumenLibroDiario.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            resumenLibroDiario.Location = new System.Drawing.Point(txtSaldoHaber.Left, txtSaldoHaber.Bottom + 25);
            tbpLibroDiario.Controls.Add(resumenLibroDiario);

            resumenLibroAjustes.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            resumenLibroAjustes.Location = new System.Drawing.Point(txtSaldoHaberAjuste.Left, txtSaldoHaberAjuste.Bottom + 25);
            tbpLibroAjustado.Controls.Add(resumenLibroAjustes);
        }

        /// <summary>
        /// Asigna los iconos de la carpeta Icons a las pestañas y a los botones de
        /// accion (Agregar/Editar/Eliminar). Si algun archivo no existe (por ejemplo
        /// en tiempo de diseño) simplemente no se asigna icono, sin romper la app.
        /// </summary>
        private void ConfigurarIconos()
        {
            if (GridStyleHelper.EnDisenio) return;

            var iconosPestanas = new ImageList { ImageSize = new System.Drawing.Size(16, 16), ColorDepth = ColorDepth.Depth32Bit };
            IconHelper.RegistrarEnImageList(iconosPestanas, "LibroDiario", "LibroDiario.ico");
            IconHelper.RegistrarEnImageList(iconosPestanas, "LibroAjustes", "LibroAjustes.ico");
            IconHelper.RegistrarEnImageList(iconosPestanas, "LibroMayor", "libroMayor.ico");
            IconHelper.RegistrarEnImageList(iconosPestanas, "BalanceComprobacion", "BalanceComprobacion.ico");
            IconHelper.RegistrarEnImageList(iconosPestanas, "EstadoResultado", "Balance.ico");
            IconHelper.RegistrarEnImageList(iconosPestanas, "BalanceGeneral", "BalanceGeneral.ico");
            IconHelper.RegistrarEnImageList(iconosPestanas, "Configuracion", "Settings.ico");

            tabControl1.ImageList = iconosPestanas;
            tbpLibroDiario.ImageKey = "LibroDiario";
            tbpLibroAjustado.ImageKey = "LibroAjustes";
            tbpLibroMayor.ImageKey = "LibroMayor";
            tbpBalanceComprobacion.ImageKey = "BalanceComprobacion";
            tbpEstadoResultado.ImageKey = "EstadoResultado";
            tbpBalanceGeneral.ImageKey = "BalanceGeneral";
            tblConfiguración.ImageKey = "Configuracion";

            IconHelper.AplicarIconoBoton(btnAgregarAsiento, "Add.ico");
            IconHelper.AplicarIconoBoton(btnEditarAsiento, "Edit.ico");
            IconHelper.AplicarIconoBoton(btnEliminarAsiento, "Remove.ico");
            IconHelper.AplicarIconoBoton(btnAgregarAjuste, "Add.ico");
            IconHelper.AplicarIconoBoton(btnEditarAjuste, "Edit.ico");
            IconHelper.AplicarIconoBoton(btnEliminarAjuste, "Remove.ico");
            IconHelper.AplicarIconoBoton(btnCrearCuenta, "Add.ico");
            IconHelper.AplicarIconoBoton(btnEditarCuenta, "Edit.ico");
            IconHelper.AplicarIconoBoton(btnEliminarCuenta, "Remove.ico");
        }

        private void frmPrincipal_Load(object sender, EventArgs e)
        {
            CargarCuentas();
            CargarLibroDiario();
            CargarLibroAjustes();
            ActualizarReportesDerivados();
        }

        private void CargarCuentas()
        {
            var cuentas = _cuentaRepository.ObtenerTodas();
            dgvCuentas.CargarCuentas(cuentas);
        }

        private void CargarLibroDiario()
        {
            var asientos = _libroDiarioRepository.ObtenerTodos();
            dgvLibroDiario.CargarAsientos(asientos.Cast<IAsientoLibro>().ToList());

            decimal totalDebe = asientos.SelectMany(a => a.Detalles).Sum(d => d.Debe);
            decimal totalHaber = asientos.SelectMany(a => a.Detalles).Sum(d => d.Haber);
            txtSaldoDebito.Text = totalDebe.ToString("N2", CultureInfo.InvariantCulture);
            txtSaldoHaber.Text = totalHaber.ToString("N2", CultureInfo.InvariantCulture);
            resumenLibroDiario.Cargar(totalDebe, totalHaber, asientos.Count);
        }

        private void CargarLibroAjustes()
        {
            var ajustes = _ajusteRepository.ObtenerTodos();
            dgvLibroAjustes.CargarAsientos(ajustes.Cast<IAsientoLibro>().ToList());

            decimal totalDebe = ajustes.SelectMany(a => a.Detalles).Sum(d => d.Debe);
            decimal totalHaber = ajustes.SelectMany(a => a.Detalles).Sum(d => d.Haber);
            txtSaldoDebitoAjuste.Text = totalDebe.ToString("N2", CultureInfo.InvariantCulture);
            txtSaldoHaberAjuste.Text = totalHaber.ToString("N2", CultureInfo.InvariantCulture);
            resumenLibroAjustes.Cargar(totalDebe, totalHaber, ajustes.Count);
        }

        /// <summary>
        /// Recalcula todo lo que se deriva del Libro Diario y del Libro de Ajustes:
        /// Libro Mayor, Balance de Comprobacion, Estado de Resultados y Balance General.
        /// Se llama automaticamente cada vez que se agrega, edita o elimina un asiento
        /// o un ajuste, para que estos reportes esten siempre al dia.
        /// </summary>
        private void ActualizarReportesDerivados()
        {
            _ultimoLibroMayor = _libroMayorService.ObtenerLibroMayor();
            RenderizarLibroMayor();
            dgvBalanceComprobacion.CargarCuentas(_ultimoLibroMayor);

            CargarEstadoResultado();
            CargarBalanceGeneral();
        }

        private void CargarEstadoResultado()
        {
            var er = _estadoResultadoService.Generar();
            var filas = new System.Collections.Generic.List<FilaReporte>();

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
            filas.Add(new FilaReporte("Utilidad antes de participación", er.UtilidadAntesParticipacion, TipoFilaReporte.Detalle));
            filas.Add(new FilaReporte("(-) 15% Participación Trabajadores", -er.Participacion15, TipoFilaReporte.Detalle));
            filas.Add(new FilaReporte("Utilidad antes de Impuesto a la Renta", er.UtilidadAntesImpuesto, TipoFilaReporte.Detalle));
            filas.Add(new FilaReporte("(-) 25% Impuesto a la Renta", -er.ImpuestoRenta25, TipoFilaReporte.Detalle));
            filas.Add(new FilaReporte("Utilidad del Ejercicio", er.UtilidadEjercicio, TipoFilaReporte.Total));

            dgvEstadoResultado.CargarFilas(filas);
        }

        private void CargarBalanceGeneral()
        {
            var bg = _balanceGeneralService.Generar();
            var filas = new System.Collections.Generic.List<FilaReporte>();

            filas.Add(new FilaReporte("ACTIVOS", null, TipoFilaReporte.Seccion));
            filas.Add(new FilaReporte("Activos Corrientes", null, TipoFilaReporte.Detalle));
            foreach (var linea in bg.ActivosCorrientes)
            {
                filas.Add(new FilaReporte("    " + linea.Nombre, linea.Monto, TipoFilaReporte.Detalle));
            }
            filas.Add(new FilaReporte("Total Activos Corrientes", bg.TotalActivoCorriente, TipoFilaReporte.Subtotal));

            filas.Add(new FilaReporte("Activos No Corrientes", null, TipoFilaReporte.Detalle));
            foreach (var linea in bg.ActivosNoCorrientes)
            {
                filas.Add(new FilaReporte("    " + linea.Nombre, linea.Monto, TipoFilaReporte.Detalle));
            }
            filas.Add(new FilaReporte("Total Activos No Corrientes", bg.TotalActivoNoCorriente, TipoFilaReporte.Subtotal));
            filas.Add(new FilaReporte("TOTAL ACTIVOS", bg.TotalActivo, TipoFilaReporte.Total));

            filas.Add(new FilaReporte("PASIVOS", null, TipoFilaReporte.Seccion));
            filas.Add(new FilaReporte("Pasivos Corrientes", null, TipoFilaReporte.Detalle));
            foreach (var linea in bg.PasivosCorrientes)
            {
                filas.Add(new FilaReporte("    " + linea.Nombre, linea.Monto, TipoFilaReporte.Detalle));
            }
            filas.Add(new FilaReporte("Total Pasivos Corrientes", bg.TotalPasivoCorriente, TipoFilaReporte.Subtotal));

            filas.Add(new FilaReporte("Pasivos No Corrientes", null, TipoFilaReporte.Detalle));
            foreach (var linea in bg.PasivosNoCorrientes)
            {
                filas.Add(new FilaReporte("    " + linea.Nombre, linea.Monto, TipoFilaReporte.Detalle));
            }
            filas.Add(new FilaReporte("Total Pasivos No Corrientes", bg.TotalPasivoNoCorriente, TipoFilaReporte.Subtotal));
            filas.Add(new FilaReporte("TOTAL PASIVOS", bg.TotalPasivo, TipoFilaReporte.Total));

            filas.Add(new FilaReporte("PATRIMONIO", null, TipoFilaReporte.Seccion));
            foreach (var linea in bg.Patrimonio)
            {
                filas.Add(new FilaReporte(linea.Nombre, linea.Monto, TipoFilaReporte.Detalle));
            }
            filas.Add(new FilaReporte("TOTAL PATRIMONIO", bg.TotalPatrimonio, TipoFilaReporte.Subtotal));

            filas.Add(new FilaReporte("TOTAL PASIVOS + PATRIMONIO", bg.TotalPasivoMasPatrimonio, TipoFilaReporte.Total));

            filas.Add(new FilaReporte("ECUACIÓN CONTABLE: Activo = Pasivo + Patrimonio",
                null, TipoFilaReporte.Seccion));
            filas.Add(new FilaReporte(
                string.Format("{0:N2} {1} {2:N2}", bg.TotalActivo, bg.Cuadra ? "=" : "≠", bg.TotalPasivoMasPatrimonio),
                null, bg.Cuadra ? TipoFilaReporte.Subtotal : TipoFilaReporte.Nota));

            dgvBalanceGeneral.CargarFilas(filas);
        }

        /// <summary>
        /// Dibuja las cuentas T en dos columnas (como en la hoja de Excel de referencia)
        /// dentro de pnlLibroMayor. Se vuelve a llamar cada vez que el panel cambia de
        /// tamaño (ver el Resize suscrito en el constructor) para que el ancho de cada
        /// columna se recalcule y el libro sea responsivo al redimensionar la ventana.
        /// </summary>
        private void RenderizarLibroMayor()
        {
            pnlLibroMayor.SuspendLayout();
            pnlLibroMayor.Controls.Clear();

            const int margen = 10;
            const int espacioEntreColumnas = 10;
            int anchoTotal = Math.Max(200, pnlLibroMayor.ClientSize.Width - (margen * 2));
            int anchoColumna = Math.Max(200, (anchoTotal - espacioEntreColumnas) / 2);
            int xColumnaIzquierda = margen;
            int xColumnaDerecha = margen + anchoColumna + espacioEntreColumnas;

            int yIzquierda = margen;
            int yDerecha = margen;

            for (int i = 0; i < _ultimoLibroMayor.Count; i++)
            {
                bool esColumnaIzquierda = i % 2 == 0;
                int x = esColumnaIzquierda ? xColumnaIzquierda : xColumnaDerecha;
                int y = esColumnaIzquierda ? yIzquierda : yDerecha;

                var panelCuenta = new TCuentaPanel();
                panelCuenta.Cargar(_ultimoLibroMayor[i], anchoColumna);
                panelCuenta.Location = new System.Drawing.Point(x, y);
                pnlLibroMayor.Controls.Add(panelCuenta);

                if (esColumnaIzquierda)
                {
                    yIzquierda += panelCuenta.Height + margen;
                }
                else
                {
                    yDerecha += panelCuenta.Height + margen;
                }
            }

            pnlLibroMayor.ResumeLayout(true);
        }

        private void tbpLibroDiario_Click(object sender, EventArgs e)
        {
        }

        private void dgvLibroDiario_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void btnAgregarAsiento_Click(object sender, EventArgs e)
        {
            int siguienteNumero = _libroDiarioRepository.ObtenerSiguienteNumero();
            using (var frm = new frmAgregarAsientoLibroDiario(ModoFormulario.Crear, siguienteNumero))
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    _libroDiarioRepository.Agregar(frm.Asiento);
                    CargarLibroDiario();
                    ActualizarReportesDerivados();
                }
            }
        }

        private void btnEditarAsiento_Click(object sender, EventArgs e)
        {
            var asiento = dgvLibroDiario.ObtenerAsientoSeleccionado() as AsientoContable;
            if (asiento == null)
            {
                MessageBox.Show(this, "Seleccione un asiento del Libro Diario para editar.", "Editar asiento",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var frm = new frmAgregarAsientoLibroDiario(ModoFormulario.Editar, asiento.Numero, asiento))
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    _libroDiarioRepository.Actualizar(asiento.Numero, frm.Asiento);
                    CargarLibroDiario();
                    ActualizarReportesDerivados();
                }
            }
        }

        private void btnEliminarAsiento_Click(object sender, EventArgs e)
        {
            var asiento = dgvLibroDiario.ObtenerAsientoSeleccionado() as AsientoContable;
            if (asiento == null)
            {
                MessageBox.Show(this, "Seleccione un asiento del Libro Diario para eliminar.", "Eliminar asiento",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var confirmacion = MessageBox.Show(this,
                string.Format("¿Desea eliminar el asiento N° {0}?", asiento.Numero),
                "Eliminar asiento", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmacion == DialogResult.Yes)
            {
                _libroDiarioRepository.Eliminar(asiento.Numero);
                CargarLibroDiario();
                ActualizarReportesDerivados();
            }
        }

        private void txtSaldoDebito_TextChanged(object sender, EventArgs e)
        {
        }

        private void txtSaldoHaber_TextChanged(object sender, EventArgs e)
        {
        }

        private void btnAgregarAjuste_Click(object sender, EventArgs e)
        {
            int siguienteNumero = _ajusteRepository.ObtenerSiguienteNumero();
            using (var frm = new frmAgregarAjusteLibroDiario(ModoFormulario.Crear, siguienteNumero))
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    _ajusteRepository.Agregar(frm.Ajuste);
                    CargarLibroAjustes();
                    ActualizarReportesDerivados();
                }
            }
        }

        private void btnEditarAjuste_Click(object sender, EventArgs e)
        {
            var ajuste = dgvLibroAjustes.ObtenerAsientoSeleccionado() as Ajuste;
            if (ajuste == null)
            {
                MessageBox.Show(this, "Seleccione un ajuste del Libro de Ajustes para editar.", "Editar ajuste",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var frm = new frmAgregarAjusteLibroDiario(ModoFormulario.Editar, ajuste.Numero, ajuste))
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    _ajusteRepository.Actualizar(ajuste.Numero, frm.Ajuste);
                    CargarLibroAjustes();
                    ActualizarReportesDerivados();
                }
            }
        }

        private void btnEliminarAjuste_Click(object sender, EventArgs e)
        {
            var ajuste = dgvLibroAjustes.ObtenerAsientoSeleccionado() as Ajuste;
            if (ajuste == null)
            {
                MessageBox.Show(this, "Seleccione un ajuste del Libro de Ajustes para eliminar.", "Eliminar ajuste",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var confirmacion = MessageBox.Show(this,
                string.Format("¿Desea eliminar el ajuste {0}?", ajuste.NumeroDisplay),
                "Eliminar ajuste", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmacion == DialogResult.Yes)
            {
                _ajusteRepository.Eliminar(ajuste.Numero);
                CargarLibroAjustes();
                ActualizarReportesDerivados();
            }
        }

        private void btnCrearCuenta_Click(object sender, EventArgs e)
        {
            using (var frm = new frmAgregarCuenta(ModoFormulario.Crear))
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        _cuentaRepository.Agregar(frm.Cuenta);
                        CargarCuentas();
                    }
                    catch (InvalidOperationException ex)
                    {
                        MessageBox.Show(this, ex.Message, "Crear cuenta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private void btnEditarCuenta_Click(object sender, EventArgs e)
        {
            var cuenta = dgvCuentas.ObtenerCuentaSeleccionada();
            if (cuenta == null)
            {
                MessageBox.Show(this, "Seleccione una cuenta para editar.", "Editar cuenta",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var frm = new frmAgregarCuenta(ModoFormulario.Editar, cuenta))
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        _cuentaRepository.Actualizar(cuenta.Codigo, frm.Cuenta);
                        CargarCuentas();
                    }
                    catch (InvalidOperationException ex)
                    {
                        MessageBox.Show(this, ex.Message, "Editar cuenta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private void btnEliminarCuenta_Click(object sender, EventArgs e)
        {
            var cuenta = dgvCuentas.ObtenerCuentaSeleccionada();
            if (cuenta == null)
            {
                MessageBox.Show(this, "Seleccione una cuenta para eliminar.", "Eliminar cuenta",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var confirmacion = MessageBox.Show(this,
                string.Format("¿Desea eliminar la cuenta '{0} - {1}'?", cuenta.Codigo, cuenta.Nombre),
                "Eliminar cuenta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmacion == DialogResult.Yes)
            {
                _cuentaRepository.Eliminar(cuenta.Codigo);
                CargarCuentas();
            }
        }

        private void tbpLibroAjustado_Click(object sender, EventArgs e)
        {

        }
    }
}
