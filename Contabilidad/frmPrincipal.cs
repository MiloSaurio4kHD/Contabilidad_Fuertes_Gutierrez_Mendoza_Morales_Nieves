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
        private readonly EstadoService _estadoService = new EstadoService();
        private readonly ExportacionExcelService _exportacionExcelService = new ExportacionExcelService();
        private System.Collections.Generic.List<CuentaMayor> _ultimoLibroMayor = new System.Collections.Generic.List<CuentaMayor>();
        private readonly ResumenLibroDataGridView resumenLibroDiario = new ResumenLibroDataGridView();
        private readonly ResumenLibroDataGridView resumenLibroAjustes = new ResumenLibroDataGridView();

        public frmPrincipal()
        {
            InitializeComponent();
            pnlLibroMayor.Resize += (sender, e) => RenderizarLibroMayor();
            ConfigurarIconos();
            ConfigurarResumenes();
            ConfigurarBotonesEstado();
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
        /// Agrega, en la pestaña Configuración, los botones de Guardar/Cargar/Eliminar
        /// estado y Exportar a Excel, debajo de los de Crear/Editar/Eliminar cuenta.
        /// Se agregan por codigo (no en el Designer) por la misma razon que el Resumen.
        /// </summary>
        private void ConfigurarBotonesEstado()
        {
            if (GridStyleHelper.EnDisenio) return;

            var lblSeparador = new Label
            {
                Text = "Respaldo y exportación",
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new System.Drawing.Point(btnEliminarCuenta.Left, btnEliminarCuenta.Bottom + 20)
            };

            var btnGuardarEstado = new Button
            {
                Text = "Guardar estado",
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new System.Drawing.Point(btnEliminarCuenta.Left, lblSeparador.Bottom + 8),
                Size = btnEliminarCuenta.Size
            };
            btnGuardarEstado.Click += btnGuardarEstado_Click;
            IconHelper.AplicarIconoBoton(btnGuardarEstado, "Save.ico");

            var btnCargarEstado = new Button
            {
                Text = "Cargar estado",
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new System.Drawing.Point(btnEliminarCuenta.Left, btnGuardarEstado.Bottom + 8),
                Size = btnEliminarCuenta.Size
            };
            btnCargarEstado.Click += btnCargarEstado_Click;
            IconHelper.AplicarIconoBoton(btnCargarEstado, "Upload.ico");

            var btnEliminarEstado = new Button
            {
                Text = "Eliminar estado",
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new System.Drawing.Point(btnEliminarCuenta.Left, btnCargarEstado.Bottom + 8),
                Size = btnEliminarCuenta.Size
            };
            btnEliminarEstado.Click += btnEliminarEstado_Click;
            IconHelper.AplicarIconoBoton(btnEliminarEstado, "Remove.ico");

            var btnExportarExcel = new Button
            {
                Text = "Exportar a Excel",
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new System.Drawing.Point(btnEliminarCuenta.Left, btnEliminarEstado.Bottom + 8),
                Size = btnEliminarCuenta.Size
            };
            btnExportarExcel.Click += btnExportarExcel_Click;
            IconHelper.AplicarIconoBoton(btnExportarExcel, "Excel.ico");

            tblConfiguración.Controls.Add(lblSeparador);
            tblConfiguración.Controls.Add(btnGuardarEstado);
            tblConfiguración.Controls.Add(btnCargarEstado);
            tblConfiguración.Controls.Add(btnEliminarEstado);
            tblConfiguración.Controls.Add(btnExportarExcel);
        }

        private void btnGuardarEstado_Click(object sender, EventArgs e)
        {
            using (var dlg = new SaveFileDialog())
            {
                dlg.Filter = "Archivo de estado (*.json)|*.json";
                dlg.FileName = "estado_contabilidad.json";
                dlg.InitialDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");

                if (dlg.ShowDialog(this) != DialogResult.OK) return;

                try
                {
                    _estadoService.Guardar(dlg.FileName);
                    MessageBox.Show(this, "Estado guardado correctamente en:\n" + dlg.FileName,
                        "Guardar estado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "No se pudo guardar el estado: " + ex.Message,
                        "Guardar estado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnCargarEstado_Click(object sender, EventArgs e)
        {
            var confirmacion = MessageBox.Show(this,
                "Esto va a reemplazar TODOS los datos actuales (cuentas, asientos y ajustes) con los del archivo que selecciones. ¿Desea continuar?",
                "Cargar estado", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirmacion != DialogResult.Yes) return;

            using (var dlg = new OpenFileDialog())
            {
                dlg.Filter = "Archivo de estado (*.json)|*.json";
                dlg.InitialDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");

                if (dlg.ShowDialog(this) != DialogResult.OK) return;

                try
                {
                    _estadoService.Cargar(dlg.FileName);
                    CargarCuentas();
                    CargarLibroDiario();
                    CargarLibroAjustes();
                    ActualizarReportesDerivados();
                    MessageBox.Show(this, "Estado cargado correctamente.",
                        "Cargar estado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "No se pudo cargar el estado: " + ex.Message,
                        "Cargar estado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnEliminarEstado_Click(object sender, EventArgs e)
        {
            var confirmacion = MessageBox.Show(this,
                "¿Desea eliminar TODOS los asientos y ajustes? Las cuentas no se verán afectadas. Esta acción no se puede deshacer.",
                "Eliminar estado", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirmacion != DialogResult.Yes) return;

            _estadoService.EliminarAsientosYAjustes();
            CargarLibroDiario();
            CargarLibroAjustes();
            ActualizarReportesDerivados();
            MessageBox.Show(this, "Se eliminaron todos los asientos y ajustes. Las cuentas se conservaron.",
                "Eliminar estado", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnExportarExcel_Click(object sender, EventArgs e)
        {
            using (var dlg = new SaveFileDialog())
            {
                dlg.Filter = "Libro de Excel (*.xlsx)|*.xlsx";
                dlg.FileName = "contabilidad_" + DateTime.Now.ToString("ddMMyy_HHmmss") + ".xlsx";

                if (dlg.ShowDialog(this) != DialogResult.OK) return;

                try
                {
                    _exportacionExcelService.Exportar(dlg.FileName);
                    MessageBox.Show(this, "Archivo exportado correctamente en:\n" + dlg.FileName,
                        "Exportar a Excel", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "No se pudo exportar: " + ex.Message,
                        "Exportar a Excel", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
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
            dgvEstadoResultado.CargarFilas(FilaReporteBuilder.ConstruirFilasEstadoResultado(er));
        }

        private void CargarBalanceGeneral()
        {
            var bg = _balanceGeneralService.Generar();
            dgvBalanceGeneral.CargarFilas(FilaReporteBuilder.ConstruirFilasBalanceGeneral(bg));
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
