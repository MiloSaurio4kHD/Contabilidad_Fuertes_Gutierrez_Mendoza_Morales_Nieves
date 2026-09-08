using Contabilidad.Data;
using Contabilidad.Models;
using Contabilidad.UI;
using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

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
        private System.Collections.Generic.List<CuentaMayor> _ultimoLibroMayorSinAjustar = new System.Collections.Generic.List<CuentaMayor>();
        private EstadoResultado _ultimoEstadoResultado = new EstadoResultado();
        private BalanceGeneral _ultimoBalanceGeneral = new BalanceGeneral();
        private readonly ResumenLibroDataGridView resumenLibroDiario = new ResumenLibroDataGridView();
        private readonly ResumenLibroDataGridView resumenLibroAjustes = new ResumenLibroDataGridView();
        private readonly Panel pnlLibroMayorSinAjustar = new Panel();
        private readonly BalanceComprobacionDataGridView dgvBalanceComprobacionSinAjustar = new BalanceComprobacionDataGridView();
        private readonly EvolucionMensualService _evolucionMensualService = new EvolucionMensualService();
        private readonly Panel _pnlDashboard = new Panel();
        private readonly Chart _chartBalance = new Chart();
        private readonly Chart _chartResultados = new Chart();
        private readonly Chart _chartEvolucion = new Chart();
        private readonly FlujoTrabajoControl _flujoTrabajo = new FlujoTrabajoControl();
        private readonly Panel _pnlIndicadorEcuacion = new Panel();
        private readonly Label _lblEcuacionEstado = new Label();
        private readonly Label _lblEcuacionDetalle = new Label();
        private readonly Label _lblCatalogoCuentas = new Label();
        private readonly Label _lblUtilidadEjercicio = new Label();
        private readonly Label _lblCascadaTributaria = new Label();
        private readonly Label _lblBuscarCuenta = new Label();
        private readonly TextBox _txtBuscarCuenta = new TextBox();
        private System.Collections.Generic.List<Cuenta> _todasLasCuentasCache = new System.Collections.Generic.List<Cuenta>();

        public frmPrincipal()
        {
            InitializeComponent();

            // Por debajo de este tamaño el Dashboard (7 tarjetas, la mas ancha con 3
            // columnas de 380px) y otros modulos empiezan a verse recortados o muy
            // apretados. Confirmado por el usuario como el tamaño minimo utilizable.
            MinimumSize = new System.Drawing.Size(1400, 700);

            pnlLibroMayor.Resize += (sender, e) => RenderizarLibroMayorEn(pnlLibroMayor, _ultimoLibroMayor);
            ConfigurarIconos();
            ConfigurarResumenes();
            ConfigurarBotonesEstado();
            ConfigurarBuscadorCuentas();
            ConfigurarLibrosSinAjustar();
            ConfigurarDashboard();

            // WindowState=Maximized recien se aplica visualmente cuando la ventana se
            // muestra (Load todavia ve el tamaño de diseño), asi que el centrado del
            // Dashboard se recalcula una vez mas en Shown, ya con el tamaño definitivo.
            Shown += (sender, e) => PosicionarDashboard();
        }

        /// <summary>
        /// Agrega un cuadro de busqueda arriba del catalogo de cuentas (Configuracion),
        /// para no tener que scrollear entre todas las cuentas para encontrar una. Filtra
        /// por codigo o nombre a medida que se escribe. Se agrega por codigo, recorriendo
        /// dgvCuentas hacia abajo, para no tocar el Designer.
        /// </summary>
        private void ConfigurarBuscadorCuentas()
        {
            if (GridStyleHelper.EnDisenio) return;

            var ubicacionOriginal = dgvCuentas.Location;
            var tamanoOriginal = dgvCuentas.Size;

            _lblBuscarCuenta.Text = "Buscar cuenta (código o nombre):";
            _lblBuscarCuenta.AutoSize = true;
            _lblBuscarCuenta.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            _lblBuscarCuenta.ForeColor = GridStyleHelper.ColorEncabezado;
            _lblBuscarCuenta.Location = ubicacionOriginal;
            tblConfiguración.Controls.Add(_lblBuscarCuenta);

            _txtBuscarCuenta.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _txtBuscarCuenta.Location = new System.Drawing.Point(ubicacionOriginal.X, _lblBuscarCuenta.Bottom + 2);
            _txtBuscarCuenta.Width = tamanoOriginal.Width;
            _txtBuscarCuenta.TextChanged += (sender, e) => FiltrarCuentas();
            tblConfiguración.Controls.Add(_txtBuscarCuenta);

            int alturaOcupada = (_txtBuscarCuenta.Bottom - ubicacionOriginal.Y) + 8;
            dgvCuentas.Location = new System.Drawing.Point(ubicacionOriginal.X, ubicacionOriginal.Y + alturaOcupada);
            dgvCuentas.Size = new System.Drawing.Size(tamanoOriginal.Width, tamanoOriginal.Height - alturaOcupada);
        }

        /// <summary>
        /// Vuelve a llenar dgvCuentas a partir de _todasLasCuentasCache, aplicando el
        /// filtro de _txtBuscarCuenta si el usuario esta buscando algo.
        /// </summary>
        private void FiltrarCuentas()
        {
            string filtro = _txtBuscarCuenta.Text.Trim();
            var coincidencias = string.IsNullOrEmpty(filtro)
                ? _todasLasCuentasCache
                : _todasLasCuentasCache.Where(c =>
                    c.Codigo.IndexOf(filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    c.Nombre.IndexOf(filtro, StringComparison.OrdinalIgnoreCase) >= 0).ToList();

            dgvCuentas.CargarCuentas(coincidencias);
        }

        /// <summary>
        /// Agrega, por codigo, el panel del Libro Mayor sin ajustar y el grid del Balance
        /// de Comprobacion sin ajustar en sus pestañas (ambas ya creadas vacias en el
        /// Diseñador). Mismo motivo que el resto de controles agregados por codigo: no
        /// tocar el Designer para no arriesgar que se dañe.
        /// </summary>
        private void ConfigurarLibrosSinAjustar()
        {
            if (GridStyleHelper.EnDisenio) return;

            pnlLibroMayorSinAjustar.Dock = DockStyle.Fill;
            pnlLibroMayorSinAjustar.AutoScroll = true;
            pnlLibroMayorSinAjustar.BackColor = System.Drawing.Color.White;
            pnlLibroMayorSinAjustar.Resize += (sender, e) => RenderizarLibroMayorEn(pnlLibroMayorSinAjustar, _ultimoLibroMayorSinAjustar);
            tbpLibroMayorSinAjustar.Controls.Add(pnlLibroMayorSinAjustar);

            dgvBalanceComprobacionSinAjustar.Dock = DockStyle.Fill;
            tbpBalanceComprobacionSinAjustar.Controls.Add(dgvBalanceComprobacionSinAjustar);
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

        /// <summary>
        /// Arma el Dashboard: una cuadricula de tarjetas con el estado esencial (totales,
        /// utilidad del ejercicio, cuantas cuentas/asientos/ajustes hay, si cuadra la
        /// ecuacion contable) y, debajo, botones de acceso rapido a cada pestaña. Todo por
        /// codigo dentro de la pestaña vacia que ya se creo en el Diseñador.
        /// </summary>
        private static readonly Color ColorVerdeOscuro = Color.FromArgb(0x1B, 0x7A, 0x1B);
        private static readonly Color ColorRojo = Color.FromArgb(0xB0, 0x20, 0x20);
        private static readonly Color ColorVerdeClaro = Color.FromArgb(0x8B, 0xC3, 0x4A);
        private static readonly Color ColorVerdeGrisaceo = Color.FromArgb(0x6A, 0x8F, 0x5A);

        private readonly TarjetaConTitulo _tarjBalance = new TarjetaConTitulo("Visión General del Balance");
        private readonly TarjetaConTitulo _tarjResultados = new TarjetaConTitulo("Rendimiento Operativo");
        private readonly TarjetaConTitulo _tarjEcuacion = new TarjetaConTitulo("Verificación de la Ecuación");
        private readonly TarjetaConTitulo _tarjCatalogo = new TarjetaConTitulo("Catálogo de Cuentas");
        private readonly TarjetaConTitulo _tarjFlujo = new TarjetaConTitulo("Flujo de Trabajo Contable");
        private readonly TarjetaConTitulo _tarjEvolucion = new TarjetaConTitulo("Evolución Mensual de Ingresos/Gastos");
        private readonly TarjetaConTitulo _tarjHerramientas = new TarjetaConTitulo("Herramientas de Exportación y Respaldo");

        /// <summary>
        /// Arma las 7 tarjetas del Dashboard (2 filas: graficos/indicadores arriba,
        /// flujo de trabajo/tendencia/herramientas abajo). Cada tarjeta usa
        /// TarjetaConTitulo como contenedor comun; el contenido especifico de cada una
        /// (grafico, control de flujo, botones) se arma una sola vez aqui y despues solo
        /// se actualiza con datos nuevos en ActualizarDashboard().
        /// </summary>
        private void ConfigurarDashboard()
        {
            if (GridStyleHelper.EnDisenio) return;

            _pnlDashboard.Dock = DockStyle.Fill;
            _pnlDashboard.AutoScroll = true;
            _pnlDashboard.BackColor = Color.White;
            tbpDashBoard.Controls.Add(_pnlDashboard);

            ConfigurarAparienciaChart(_chartBalance, mostrarLeyenda: false);
            _chartBalance.ChartAreas[0].AxisY.LabelStyle.Format = "N0";
            var seriePorDefectoBalance = _chartBalance.Series[0];
            seriePorDefectoBalance.ChartType = SeriesChartType.Column;
            seriePorDefectoBalance.IsValueShownAsLabel = true;
            seriePorDefectoBalance.LabelFormat = "N2";
            seriePorDefectoBalance.Points.AddXY("Activos", 0);
            seriePorDefectoBalance.Points.AddXY("Pasivos", 0);
            seriePorDefectoBalance.Points.AddXY("Patrimonio", 0);
            seriePorDefectoBalance.Points[0].Color = GridStyleHelper.ColorEncabezado;
            seriePorDefectoBalance.Points[1].Color = ColorVerdeGrisaceo;
            seriePorDefectoBalance.Points[2].Color = ColorVerdeClaro;
            _chartBalance.Dock = DockStyle.Fill;
            _tarjBalance.ContentPanel.Controls.Add(_chartBalance);

            ConfigurarAparienciaChart(_chartResultados, mostrarLeyenda: false);
            var serieResultados = _chartResultados.Series[0];
            serieResultados.ChartType = SeriesChartType.Column;
            serieResultados.IsValueShownAsLabel = true;
            serieResultados.LabelFormat = "N2";
            serieResultados.Points.AddXY("Ingresos", 0);
            serieResultados.Points.AddXY("Gastos", 0);
            serieResultados.Points[0].Color = GridStyleHelper.ColorEncabezado;
            serieResultados.Points[1].Color = ColorRojo;
            _chartResultados.Dock = DockStyle.Top;
            _chartResultados.Height = 120;

            // IMPORTANTE: cuando un panel mezcla un hijo Dock=Fill con hijos
            // Dock=Top/Bottom, WinForms solo calcula bien el alto del Fill si este se
            // agrega a Controls ANTES que los Top/Bottom (si se agrega despues, el Fill
            // termina ocupando el panel entero y tapando a los otros). Por eso aqui
            // pnlInferiorResultados (Fill) se agrega antes que _chartResultados (Top), y
            // mas abajo _lblCascadaTributaria (Fill) se agrega antes que
            // _lblUtilidadEjercicio (Bottom).
            var pnlInferiorResultados = new Panel { Dock = DockStyle.Fill };

            _lblUtilidadEjercicio.Dock = DockStyle.Bottom;
            _lblUtilidadEjercicio.AutoSize = false;
            _lblUtilidadEjercicio.Height = 34;
            _lblUtilidadEjercicio.TextAlign = ContentAlignment.MiddleCenter;
            _lblUtilidadEjercicio.Font = new Font(Font, FontStyle.Bold);
            _lblUtilidadEjercicio.ForeColor = Color.White;

            _lblCascadaTributaria.Dock = DockStyle.Fill;
            _lblCascadaTributaria.AutoSize = false;
            _lblCascadaTributaria.TextAlign = ContentAlignment.MiddleCenter;
            _lblCascadaTributaria.Font = new Font(Font.FontFamily, 7.5f);
            _lblCascadaTributaria.ForeColor = Color.FromArgb(0x55, 0x55, 0x55);

            pnlInferiorResultados.Controls.Add(_lblCascadaTributaria);
            pnlInferiorResultados.Controls.Add(_lblUtilidadEjercicio);

            _tarjResultados.ContentPanel.Controls.Add(pnlInferiorResultados);
            _tarjResultados.ContentPanel.Controls.Add(_chartResultados);

            // _pnlIndicadorEcuacion y _lblEcuacionEstado van los dos "arriba" (Dock=Top):
            // se meten en un sub-panel propio para no depender del orden relativo entre
            // dos controles con el mismo Dock (el circulo siempre queda arriba del texto).
            var pnlEncabezadoEcuacion = new Panel { Dock = DockStyle.Top, Height = 116 };
            _pnlIndicadorEcuacion.Dock = DockStyle.Top;
            _pnlIndicadorEcuacion.Height = 90;
            _pnlIndicadorEcuacion.Paint += PnlIndicadorEcuacion_Paint;

            _lblEcuacionEstado.Dock = DockStyle.Fill;
            _lblEcuacionEstado.AutoSize = false;
            _lblEcuacionEstado.TextAlign = ContentAlignment.MiddleCenter;
            _lblEcuacionEstado.Font = new Font(Font, FontStyle.Bold);

            pnlEncabezadoEcuacion.Controls.Add(_lblEcuacionEstado);
            pnlEncabezadoEcuacion.Controls.Add(_pnlIndicadorEcuacion);

            _lblEcuacionDetalle.Dock = DockStyle.Fill;
            _lblEcuacionDetalle.AutoSize = false;
            _lblEcuacionDetalle.TextAlign = ContentAlignment.TopCenter;
            _lblEcuacionDetalle.Text = "Ecuación Contable:\nActivo = Pasivo + Patrimonio";
            _lblEcuacionDetalle.ForeColor = Color.FromArgb(0x55, 0x55, 0x55);
            _lblEcuacionDetalle.Font = new Font(Font.FontFamily, 8.5f);

            // El Fill (_lblEcuacionDetalle) se agrega antes que el Top
            // (pnlEncabezadoEcuacion): ver la nota sobre orden de Dock en la tarjeta de
            // Rendimiento Operativo, unas lineas arriba.
            _tarjEcuacion.ContentPanel.Controls.Add(_lblEcuacionDetalle);
            _tarjEcuacion.ContentPanel.Controls.Add(pnlEncabezadoEcuacion);

            _lblCatalogoCuentas.Dock = DockStyle.Fill;
            _lblCatalogoCuentas.AutoSize = false;
            _lblCatalogoCuentas.TextAlign = ContentAlignment.MiddleCenter;
            _lblCatalogoCuentas.Font = new Font(Font.FontFamily, 32f, FontStyle.Bold);
            _lblCatalogoCuentas.ForeColor = GridStyleHelper.ColorEncabezado;
            _tarjCatalogo.ContentPanel.Controls.Add(_lblCatalogoCuentas);

            _flujoTrabajo.Dock = DockStyle.Fill;
            _flujoTrabajo.EtapaClick += EtapaFlujo_Click;
            _tarjFlujo.ContentPanel.Controls.Add(_flujoTrabajo);

            ConfigurarAparienciaChart(_chartEvolucion, mostrarLeyenda: true);
            var serieIngresosMes = new Series("Ingresos") { ChartType = SeriesChartType.SplineArea, Color = Color.FromArgb(120, GridStyleHelper.ColorEncabezado), BorderColor = GridStyleHelper.ColorEncabezado, BorderWidth = 2 };
            var serieGastosMes = new Series("Gastos") { ChartType = SeriesChartType.SplineArea, Color = Color.FromArgb(100, ColorRojo), BorderColor = ColorRojo, BorderWidth = 2 };
            _chartEvolucion.Series.Clear();
            _chartEvolucion.Series.Add(serieIngresosMes);
            _chartEvolucion.Series.Add(serieGastosMes);
            _chartEvolucion.Dock = DockStyle.Fill;
            _tarjEvolucion.ContentPanel.Controls.Add(_chartEvolucion);

            ConfigurarHerramientas();

            foreach (var tarjeta in new[] { _tarjBalance, _tarjResultados, _tarjEcuacion, _tarjCatalogo, _tarjFlujo, _tarjEvolucion, _tarjHerramientas })
            {
                _pnlDashboard.Controls.Add(tarjeta);
            }

            _pnlDashboard.Resize += (sender, e) => PosicionarDashboard();
            PosicionarDashboard();
        }

        /// <summary>
        /// Aplica el estilo comun (sin bordes de plotting, fondo blanco, colores del
        /// tema) a un Chart nuevo, dejandolo con una unica serie generica lista para
        /// que el que llama la configure segun necesite.
        /// </summary>
        private void ConfigurarAparienciaChart(Chart chart, bool mostrarLeyenda)
        {
            var area = new ChartArea("Principal");
            area.BackColor = Color.White;
            area.AxisX.LineColor = Color.FromArgb(0xD0, 0xD0, 0xD0);
            area.AxisY.LineColor = Color.FromArgb(0xD0, 0xD0, 0xD0);
            area.AxisX.MajorGrid.LineColor = Color.FromArgb(0xEC, 0xEC, 0xEC);
            area.AxisY.MajorGrid.LineColor = Color.FromArgb(0xEC, 0xEC, 0xEC);
            area.AxisY.LabelStyle.Font = new Font(Font.FontFamily, 7.5f);
            area.AxisX.LabelStyle.Font = new Font(Font.FontFamily, 7.5f);

            chart.ChartAreas.Clear();
            chart.ChartAreas.Add(area);
            chart.BackColor = Color.White;
            chart.Series.Clear();
            chart.Series.Add(new Series("Datos"));

            if (mostrarLeyenda)
            {
                var leyenda = new Legend("LeyendaPrincipal") { Docking = Docking.Bottom, Font = new Font(Font.FontFamily, 8f) };
                chart.Legends.Clear();
                chart.Legends.Add(leyenda);
            }
            else
            {
                chart.Legends.Clear();
            }
        }

        /// <summary>
        /// Botones de respaldo/exportacion agrupados en una sola tarjeta del Dashboard.
        /// Reutilizan los mismos manejadores de la pestaña Configuracion (no duplica logica).
        /// </summary>
        private void ConfigurarHerramientas()
        {
            var panelBotones = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 44, FlowDirection = FlowDirection.LeftToRight, WrapContents = false };

            var btnGuardar = new Button { Text = "Guardar estado", AutoSize = true, Height = 34 };
            btnGuardar.Click += btnGuardarEstado_Click;
            IconHelper.AplicarIconoBoton(btnGuardar, "Save.ico");

            var btnCargar = new Button { Text = "Cargar estado", AutoSize = true, Height = 34 };
            btnCargar.Click += btnCargarEstado_Click;
            IconHelper.AplicarIconoBoton(btnCargar, "Upload.ico");

            var btnEliminar = new Button { Text = "Eliminar estado", AutoSize = true, Height = 34 };
            btnEliminar.Click += btnEliminarEstado_Click;
            IconHelper.AplicarIconoBoton(btnEliminar, "Remove.ico");

            panelBotones.Controls.Add(btnGuardar);
            panelBotones.Controls.Add(btnCargar);
            panelBotones.Controls.Add(btnEliminar);

            var btnExportarDestacado = new Button
            {
                Text = "Exportar a Excel (con Fórmulas Reales)",
                Dock = DockStyle.Bottom,
                Height = 44,
                BackColor = GridStyleHelper.ColorEncabezado,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font(Font, FontStyle.Bold)
            };
            btnExportarDestacado.FlatAppearance.BorderColor = GridStyleHelper.ColorEncabezado;
            btnExportarDestacado.Click += btnExportarExcel_Click;
            IconHelper.AplicarIconoBoton(btnExportarDestacado, "Excel.ico");

            _tarjHerramientas.ContentPanel.Controls.Add(btnExportarDestacado);
            _tarjHerramientas.ContentPanel.Controls.Add(panelBotones);
        }

        private void PnlIndicadorEcuacion_Paint(object sender, PaintEventArgs e)
        {
            bool cuadra = _ultimoBalanceGeneral.Cuadra;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            int diametro = 64;
            int x = (_pnlIndicadorEcuacion.Width - diametro) / 2;
            int y = 10;
            var color = cuadra ? ColorVerdeOscuro : ColorRojo;

            using (var pincel = new SolidBrush(color))
            using (var fuenteSimbolo = new Font(Font.FontFamily, 26f, FontStyle.Bold))
            {
                e.Graphics.FillEllipse(pincel, x, y, diametro, diametro);
                string simbolo = cuadra ? "✓" : "✗";
                var tamano = e.Graphics.MeasureString(simbolo, fuenteSimbolo);
                e.Graphics.DrawString(simbolo, fuenteSimbolo, Brushes.White,
                    x + (diametro - tamano.Width) / 2, y + (diametro - tamano.Height) / 2);
            }
        }

        private void EtapaFlujo_Click(int indice)
        {
            var destinos = new[] { tbpLibroDiario, tbpLibroAjustado, tbpLibroMayor, tbpBalanceComprobacion, tbpEstadoResultado };
            if (indice >= 0 && indice < destinos.Length)
            {
                tabControl1.SelectedTab = destinos[indice];
            }
        }

        /// <summary>
        /// Centra horizontalmente, fila por fila, las tarjetas del Dashboard dentro de
        /// pnlDashboard. Se recalcula cada vez que la ventana cambia de tamaño (igual
        /// que el resto de paneles responsivos de la app).
        /// </summary>
        private void PosicionarDashboard()
        {
            const int espacio = 20;
            const int margenSuperior = 20;

            _tarjBalance.Size = new Size(330, 260);
            _tarjResultados.Size = new Size(330, 260);
            _tarjEcuacion.Size = new Size(220, 260);
            _tarjCatalogo.Size = new Size(220, 260);

            _tarjFlujo.Size = new Size(500, 200);
            _tarjEvolucion.Size = new Size(380, 200);
            _tarjHerramientas.Size = new Size(380, 200);

            var fila1 = new[] { _tarjBalance, _tarjResultados, _tarjEcuacion, _tarjCatalogo };
            var fila2 = new[] { _tarjFlujo, _tarjEvolucion, _tarjHerramientas };

            int anchoFila1 = fila1.Sum(t => t.Width) + espacio * (fila1.Length - 1);
            int xFila1 = Math.Max(espacio, (_pnlDashboard.ClientSize.Width - anchoFila1) / 2);
            int y1 = margenSuperior;
            foreach (var tarjeta in fila1)
            {
                tarjeta.Location = new Point(xFila1, y1);
                xFila1 += tarjeta.Width + espacio;
            }

            int anchoFila2 = fila2.Sum(t => t.Width) + espacio * (fila2.Length - 1);
            int xFila2 = Math.Max(espacio, (_pnlDashboard.ClientSize.Width - anchoFila2) / 2);
            int y2 = y1 + fila1.Max(t => t.Height) + espacio;
            foreach (var tarjeta in fila2)
            {
                tarjeta.Location = new Point(xFila2, y2);
                xFila2 += tarjeta.Width + espacio;
            }
        }

        /// <summary>
        /// Recalcula el contenido de las tarjetas del Dashboard (graficos, indicador de
        /// ecuacion, flujo de trabajo). Se llama cada vez que cambian las cuentas o los
        /// reportes derivados, para que siempre este al dia.
        /// </summary>
        private void ActualizarDashboard()
        {
            if (GridStyleHelper.EnDisenio) return;

            var bg = _ultimoBalanceGeneral;
            var er = _ultimoEstadoResultado;

            _chartBalance.Series[0].Points[0].SetValueY(bg.TotalActivo);
            _chartBalance.Series[0].Points[1].SetValueY(bg.TotalPasivo);
            _chartBalance.Series[0].Points[2].SetValueY(bg.TotalPatrimonio);

            _chartResultados.Series[0].Points[0].SetValueY(er.TotalIngresos);
            _chartResultados.Series[0].Points[1].SetValueY(er.TotalGastos);

            bool utilidadPositiva = er.UtilidadEjercicio >= 0;
            _lblUtilidadEjercicio.BackColor = utilidadPositiva ? ColorVerdeOscuro : ColorRojo;
            _lblUtilidadEjercicio.Text = string.Format("{0}: {1}",
                utilidadPositiva ? "Utilidad del Ejercicio" : "Pérdida del Ejercicio",
                er.UtilidadEjercicio.ToString("N2", CultureInfo.InvariantCulture));

            _lblCascadaTributaria.Text = string.Format(
                "Utilidad antes de Participación: {0}\n(-) 15% Participación Trabajadores: {1}\nUtilidad antes de Impuesto: {2}\n(-) 25% Impuesto a la Renta: {3}",
                er.UtilidadAntesParticipacion.ToString("N2", CultureInfo.InvariantCulture),
                er.Participacion15.ToString("N2", CultureInfo.InvariantCulture),
                er.UtilidadAntesImpuesto.ToString("N2", CultureInfo.InvariantCulture),
                er.ImpuestoRenta25.ToString("N2", CultureInfo.InvariantCulture));

            _lblEcuacionEstado.Text = bg.Cuadra ? "SISTEMA CUADRADO" : "SISTEMA DESCUADRADO";
            _lblEcuacionEstado.ForeColor = bg.Cuadra ? ColorVerdeOscuro : ColorRojo;
            _pnlIndicadorEcuacion.Invalidate();

            _lblCatalogoCuentas.Text = _cuentaRepository.ObtenerTodas().Count.ToString(CultureInfo.InvariantCulture);

            _flujoTrabajo.ConfigurarEtapas(new[]
            {
                new EtapaFlujo { Etiqueta = "Diario", Icono = "LibroDiario.ico", Contador = _libroDiarioRepository.ObtenerTodos().Count },
                new EtapaFlujo { Etiqueta = "Ajustes", Icono = "LibroAjustes.ico", Contador = _ajusteRepository.ObtenerTodos().Count },
                new EtapaFlujo { Etiqueta = "Mayor", Icono = "libroMayor.ico", Contador = 0 },
                new EtapaFlujo { Etiqueta = "Balances", Icono = "Balance.ico", Contador = 0 },
                new EtapaFlujo { Etiqueta = "Resultados", Icono = "EstadoResultado.ico", Contador = 0 }
            });

            var puntosMensuales = _evolucionMensualService.ObtenerUltimosMeses();
            _chartEvolucion.Series["Ingresos"].Points.Clear();
            _chartEvolucion.Series["Gastos"].Points.Clear();
            foreach (var punto in puntosMensuales)
            {
                _chartEvolucion.Series["Ingresos"].Points.AddXY(punto.Etiqueta, punto.Ingresos);
                _chartEvolucion.Series["Gastos"].Points.AddXY(punto.Etiqueta, punto.Gastos);
            }
        }

        private void btnGuardarEstado_Click(object sender, EventArgs e)
        {
            using (var dlg = new SaveFileDialog())
            {
                dlg.Filter = "Archivo de estado (*.json)|*.json";
                dlg.FileName = "estado_contabilidad.json";
                dlg.InitialDirectory = RutasApp.CarpetaDatos;

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
                dlg.InitialDirectory = RutasApp.CarpetaDatos;

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
            IconHelper.RegistrarEnImageList(iconosPestanas, "Dashboard", "Dashboard.ico");
            IconHelper.RegistrarEnImageList(iconosPestanas, "LibroDiario", "LibroDiario.ico");
            IconHelper.RegistrarEnImageList(iconosPestanas, "LibroAjustes", "LibroAjustes.ico");
            IconHelper.RegistrarEnImageList(iconosPestanas, "LibroMayorSinAjustar", "LibroMayorSinAjustes.ico");
            IconHelper.RegistrarEnImageList(iconosPestanas, "LibroMayor", "libroMayor.ico");
            IconHelper.RegistrarEnImageList(iconosPestanas, "BalanceComprobacionSinAjustar", "BalanceComprobacionSinAjustar.ico");
            // Balance.ico va en Balance Comprobación (ajustado) y EstadoResultado.ico en
            // Estado Resultado: quedaban al reves antes, se corrigio a pedido del usuario.
            IconHelper.RegistrarEnImageList(iconosPestanas, "BalanceComprobacion", "Balance.ico");
            IconHelper.RegistrarEnImageList(iconosPestanas, "EstadoResultado", "EstadoResultado.ico");
            IconHelper.RegistrarEnImageList(iconosPestanas, "BalanceGeneral", "BalanceGeneral.ico");
            IconHelper.RegistrarEnImageList(iconosPestanas, "Configuracion", "Settings.ico");

            tabControl1.ImageList = iconosPestanas;
            tbpDashBoard.ImageKey = "Dashboard";
            tbpLibroDiario.ImageKey = "LibroDiario";
            tbpLibroAjustado.ImageKey = "LibroAjustes";
            tbpLibroMayorSinAjustar.ImageKey = "LibroMayorSinAjustar";
            tbpLibroMayor.ImageKey = "LibroMayor";
            tbpBalanceComprobacionSinAjustar.ImageKey = "BalanceComprobacionSinAjustar";
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

            // En el constructor, ClientSize todavia refleja el tamaño de diseño (la
            // ventana recien se maximiza al mostrarse), asi que el centrado del Dashboard
            // calculado ahi queda corto. Se recalcula aqui, ya con el tamaño real de la
            // ventana maximizada.
            PosicionarDashboard();
        }

        private void CargarCuentas()
        {
            _todasLasCuentasCache = _cuentaRepository.ObtenerTodas();
            FiltrarCuentas();
            ActualizarDashboard();
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
            RenderizarLibroMayorEn(pnlLibroMayor, _ultimoLibroMayor);
            dgvBalanceComprobacion.CargarCuentas(_ultimoLibroMayor);

            _ultimoLibroMayorSinAjustar = _libroMayorService.ObtenerLibroMayorSinAjustar();
            RenderizarLibroMayorEn(pnlLibroMayorSinAjustar, _ultimoLibroMayorSinAjustar);
            dgvBalanceComprobacionSinAjustar.CargarCuentas(_ultimoLibroMayorSinAjustar);

            CargarEstadoResultado();
            CargarBalanceGeneral();
            ActualizarDashboard();
        }

        private void CargarEstadoResultado()
        {
            _ultimoEstadoResultado = _estadoResultadoService.Generar();
            dgvEstadoResultado.CargarFilas(FilaReporteBuilder.ConstruirFilasEstadoResultado(_ultimoEstadoResultado));
        }

        private void CargarBalanceGeneral()
        {
            _ultimoBalanceGeneral = _balanceGeneralService.Generar();
            dgvBalanceGeneral.CargarFilas(FilaReporteBuilder.ConstruirFilasBalanceGeneral(_ultimoBalanceGeneral));
        }

        /// <summary>
        /// Dibuja las cuentas T en dos columnas (como en la hoja de Excel de referencia)
        /// dentro del panel indicado. Se vuelve a llamar cada vez que el panel cambia de
        /// tamaño (ver el Resize suscrito en el constructor) para que el ancho de cada
        /// columna se recalcule y el libro sea responsivo al redimensionar la ventana.
        /// </summary>
        private void RenderizarLibroMayorEn(Panel panel, System.Collections.Generic.List<CuentaMayor> cuentas)
        {
            panel.SuspendLayout();
            panel.Controls.Clear();

            const int margen = 10;
            const int espacioEntreColumnas = 10;
            int anchoTotal = Math.Max(200, panel.ClientSize.Width - (margen * 2));
            int anchoColumna = Math.Max(200, (anchoTotal - espacioEntreColumnas) / 2);
            int xColumnaIzquierda = margen;
            int xColumnaDerecha = margen + anchoColumna + espacioEntreColumnas;

            int yIzquierda = margen;
            int yDerecha = margen;

            for (int i = 0; i < cuentas.Count; i++)
            {
                bool esColumnaIzquierda = i % 2 == 0;
                int x = esColumnaIzquierda ? xColumnaIzquierda : xColumnaDerecha;
                int y = esColumnaIzquierda ? yIzquierda : yDerecha;

                var panelCuenta = new TCuentaPanel();
                panelCuenta.Cargar(cuentas[i], anchoColumna);
                panelCuenta.Location = new System.Drawing.Point(x, y);
                panel.Controls.Add(panelCuenta);

                if (esColumnaIzquierda)
                {
                    yIzquierda += panelCuenta.Height + margen;
                }
                else
                {
                    yDerecha += panelCuenta.Height + margen;
                }
            }

            panel.ResumeLayout(true);
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
                        CascadaActualizarCuentaEnLibros(cuenta.Codigo, frm.Cuenta);
                        CargarCuentas();
                    }
                    catch (InvalidOperationException ex)
                    {
                        MessageBox.Show(this, ex.Message, "Editar cuenta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        /// <summary>
        /// Cuando se edita una cuenta (cambia su codigo y/o nombre), propaga el cambio a
        /// todos los asientos del Libro Diario y ajustes del Libro de Ajustes que ya la
        /// referencian, y refresca los reportes derivados para que no queden con datos
        /// viejos (antes solo se actualizaba el catalogo, pero no lo ya guardado).
        /// </summary>
        private void CascadaActualizarCuentaEnLibros(string codigoOriginal, Cuenta cuentaNueva)
        {
            bool cambioEnDiario = _libroDiarioRepository.ActualizarCuentaEnDetalles(codigoOriginal, cuentaNueva.Codigo, cuentaNueva.Nombre);
            bool cambioEnAjustes = _ajusteRepository.ActualizarCuentaEnDetalles(codigoOriginal, cuentaNueva.Codigo, cuentaNueva.Nombre);

            if (cambioEnDiario) CargarLibroDiario();
            if (cambioEnAjustes) CargarLibroAjustes();
            if (cambioEnDiario || cambioEnAjustes) ActualizarReportesDerivados();
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

            if (_cuentaRepository.EsDelCatalogoOriginal(cuenta.Codigo))
            {
                MessageBox.Show(this,
                    string.Format("No se puede eliminar la cuenta '{0} - {1}' porque pertenece al catálogo original del sistema.", cuenta.Codigo, cuenta.Nombre),
                    "Eliminar cuenta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool enUsoDiario = _libroDiarioRepository.TieneMovimientos(cuenta.Codigo);
            bool enUsoAjustes = _ajusteRepository.TieneMovimientos(cuenta.Codigo);
            if (enUsoDiario || enUsoAjustes)
            {
                string donde = enUsoDiario && enUsoAjustes
                    ? "en el Libro Diario y en el Libro de Ajustes"
                    : enUsoDiario ? "en el Libro Diario" : "en el Libro de Ajustes";
                MessageBox.Show(this,
                    string.Format("No se puede eliminar la cuenta '{0} - {1}' porque ya está en uso {2}.", cuenta.Codigo, cuenta.Nombre, donde),
                    "Eliminar cuenta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
