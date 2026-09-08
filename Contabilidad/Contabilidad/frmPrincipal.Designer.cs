namespace Contabilidad
{
    partial class frmPrincipal
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPrincipal));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tbpDashBoard = new System.Windows.Forms.TabPage();
            this.tbpLibroDiario = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSaldoDebito = new System.Windows.Forms.TextBox();
            this.txtSaldoHaber = new System.Windows.Forms.TextBox();
            this.btnEliminarAsiento = new System.Windows.Forms.Button();
            this.btnEditarAsiento = new System.Windows.Forms.Button();
            this.btnAgregarAsiento = new System.Windows.Forms.Button();
            this.dgvLibroDiario = new Contabilidad.UI.LibroDiarioDataGridView();
            this.tbpLibroAjustado = new System.Windows.Forms.TabPage();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtSaldoDebitoAjuste = new System.Windows.Forms.TextBox();
            this.txtSaldoHaberAjuste = new System.Windows.Forms.TextBox();
            this.btnEliminarAjuste = new System.Windows.Forms.Button();
            this.btnEditarAjuste = new System.Windows.Forms.Button();
            this.btnAgregarAjuste = new System.Windows.Forms.Button();
            this.dgvLibroAjustes = new Contabilidad.UI.LibroAjustesDataGridView();
            this.tbpLibroMayorSinAjustar = new System.Windows.Forms.TabPage();
            this.tbpLibroMayor = new System.Windows.Forms.TabPage();
            this.pnlLibroMayor = new System.Windows.Forms.Panel();
            this.tbpBalanceComprobacionSinAjustar = new System.Windows.Forms.TabPage();
            this.tbpBalanceComprobacion = new System.Windows.Forms.TabPage();
            this.dgvBalanceComprobacion = new Contabilidad.UI.BalanceComprobacionDataGridView();
            this.tbpEstadoResultado = new System.Windows.Forms.TabPage();
            this.dgvEstadoResultado = new Contabilidad.UI.ReporteFinancieroDataGridView();
            this.tbpBalanceGeneral = new System.Windows.Forms.TabPage();
            this.dgvBalanceGeneral = new Contabilidad.UI.ReporteFinancieroDataGridView();
            this.tblConfiguración = new System.Windows.Forms.TabPage();
            this.btnEliminarCuenta = new System.Windows.Forms.Button();
            this.btnEditarCuenta = new System.Windows.Forms.Button();
            this.btnCrearCuenta = new System.Windows.Forms.Button();
            this.dgvCuentas = new Contabilidad.UI.CuentasDataGridView();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.folderBrowserDialog2 = new System.Windows.Forms.FolderBrowserDialog();
            this.tabControl1.SuspendLayout();
            this.tbpLibroDiario.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLibroDiario)).BeginInit();
            this.tbpLibroAjustado.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLibroAjustes)).BeginInit();
            this.tbpLibroMayor.SuspendLayout();
            this.tbpBalanceComprobacion.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBalanceComprobacion)).BeginInit();
            this.tbpEstadoResultado.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEstadoResultado)).BeginInit();
            this.tbpBalanceGeneral.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBalanceGeneral)).BeginInit();
            this.tblConfiguración.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCuentas)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tbpDashBoard);
            this.tabControl1.Controls.Add(this.tbpLibroDiario);
            this.tabControl1.Controls.Add(this.tbpLibroAjustado);
            this.tabControl1.Controls.Add(this.tbpLibroMayorSinAjustar);
            this.tabControl1.Controls.Add(this.tbpLibroMayor);
            this.tabControl1.Controls.Add(this.tbpBalanceComprobacionSinAjustar);
            this.tabControl1.Controls.Add(this.tbpBalanceComprobacion);
            this.tabControl1.Controls.Add(this.tbpEstadoResultado);
            this.tabControl1.Controls.Add(this.tbpBalanceGeneral);
            this.tabControl1.Controls.Add(this.tblConfiguración);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1924, 814);
            this.tabControl1.TabIndex = 0;
            // 
            // tbpDashBoard
            // 
            this.tbpDashBoard.Location = new System.Drawing.Point(4, 25);
            this.tbpDashBoard.Margin = new System.Windows.Forms.Padding(4);
            this.tbpDashBoard.Name = "tbpDashBoard";
            this.tbpDashBoard.Size = new System.Drawing.Size(1916, 785);
            this.tbpDashBoard.TabIndex = 9;
            this.tbpDashBoard.Text = "DashBoard";
            this.tbpDashBoard.UseVisualStyleBackColor = true;
            // 
            // tbpLibroDiario
            // 
            this.tbpLibroDiario.Controls.Add(this.label2);
            this.tbpLibroDiario.Controls.Add(this.label1);
            this.tbpLibroDiario.Controls.Add(this.txtSaldoDebito);
            this.tbpLibroDiario.Controls.Add(this.txtSaldoHaber);
            this.tbpLibroDiario.Controls.Add(this.btnEliminarAsiento);
            this.tbpLibroDiario.Controls.Add(this.btnEditarAsiento);
            this.tbpLibroDiario.Controls.Add(this.btnAgregarAsiento);
            this.tbpLibroDiario.Controls.Add(this.dgvLibroDiario);
            this.tbpLibroDiario.Location = new System.Drawing.Point(4, 25);
            this.tbpLibroDiario.Margin = new System.Windows.Forms.Padding(4);
            this.tbpLibroDiario.Name = "tbpLibroDiario";
            this.tbpLibroDiario.Padding = new System.Windows.Forms.Padding(4);
            this.tbpLibroDiario.Size = new System.Drawing.Size(1916, 785);
            this.tbpLibroDiario.TabIndex = 0;
            this.tbpLibroDiario.Text = "Libro diario";
            this.tbpLibroDiario.UseVisualStyleBackColor = true;
            this.tbpLibroDiario.Click += new System.EventHandler(this.tbpLibroDiario_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1535, 202);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 16);
            this.label2.TabIndex = 7;
            this.label2.Text = "Saldo debe";
            this.label2.Visible = false;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1532, 204);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 16);
            this.label1.TabIndex = 6;
            this.label1.Text = "Saldo haber";
            this.label1.Visible = false;
            // 
            // txtSaldoDebito
            // 
            this.txtSaldoDebito.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSaldoDebito.Location = new System.Drawing.Point(1539, 200);
            this.txtSaldoDebito.Margin = new System.Windows.Forms.Padding(4);
            this.txtSaldoDebito.Name = "txtSaldoDebito";
            this.txtSaldoDebito.ReadOnly = true;
            this.txtSaldoDebito.Size = new System.Drawing.Size(136, 22);
            this.txtSaldoDebito.TabIndex = 5;
            this.txtSaldoDebito.TabStop = false;
            this.txtSaldoDebito.Visible = false;
            this.txtSaldoDebito.TextChanged += new System.EventHandler(this.txtSaldoDebito_TextChanged);
            // 
            // txtSaldoHaber
            // 
            this.txtSaldoHaber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSaldoHaber.Location = new System.Drawing.Point(1539, 200);
            this.txtSaldoHaber.Margin = new System.Windows.Forms.Padding(4);
            this.txtSaldoHaber.Name = "txtSaldoHaber";
            this.txtSaldoHaber.ReadOnly = true;
            this.txtSaldoHaber.Size = new System.Drawing.Size(136, 22);
            this.txtSaldoHaber.TabIndex = 4;
            this.txtSaldoHaber.TabStop = false;
            this.txtSaldoHaber.Visible = false;
            this.txtSaldoHaber.TextChanged += new System.EventHandler(this.txtSaldoHaber_TextChanged);
            // 
            // btnEliminarAsiento
            // 
            this.btnEliminarAsiento.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEliminarAsiento.Location = new System.Drawing.Point(1583, 149);
            this.btnEliminarAsiento.Margin = new System.Windows.Forms.Padding(4);
            this.btnEliminarAsiento.Name = "btnEliminarAsiento";
            this.btnEliminarAsiento.Size = new System.Drawing.Size(236, 42);
            this.btnEliminarAsiento.TabIndex = 3;
            this.btnEliminarAsiento.Text = "Eliminar asiento";
            this.btnEliminarAsiento.UseVisualStyleBackColor = true;
            this.btnEliminarAsiento.Click += new System.EventHandler(this.btnEliminarAsiento_Click);
            // 
            // btnEditarAsiento
            // 
            this.btnEditarAsiento.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEditarAsiento.Location = new System.Drawing.Point(1583, 100);
            this.btnEditarAsiento.Margin = new System.Windows.Forms.Padding(4);
            this.btnEditarAsiento.Name = "btnEditarAsiento";
            this.btnEditarAsiento.Size = new System.Drawing.Size(236, 42);
            this.btnEditarAsiento.TabIndex = 2;
            this.btnEditarAsiento.Text = "Editar asiento";
            this.btnEditarAsiento.UseVisualStyleBackColor = true;
            this.btnEditarAsiento.Click += new System.EventHandler(this.btnEditarAsiento_Click);
            // 
            // btnAgregarAsiento
            // 
            this.btnAgregarAsiento.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAgregarAsiento.Location = new System.Drawing.Point(1583, 51);
            this.btnAgregarAsiento.Margin = new System.Windows.Forms.Padding(4);
            this.btnAgregarAsiento.Name = "btnAgregarAsiento";
            this.btnAgregarAsiento.Size = new System.Drawing.Size(236, 42);
            this.btnAgregarAsiento.TabIndex = 1;
            this.btnAgregarAsiento.Text = "Crear nuevo asiento";
            this.btnAgregarAsiento.UseVisualStyleBackColor = true;
            this.btnAgregarAsiento.Click += new System.EventHandler(this.btnAgregarAsiento_Click);
            // 
            // dgvLibroDiario
            // 
            this.dgvLibroDiario.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvLibroDiario.ColumnHeadersHeight = 29;
            this.dgvLibroDiario.Location = new System.Drawing.Point(11, 7);
            this.dgvLibroDiario.Margin = new System.Windows.Forms.Padding(4);
            this.dgvLibroDiario.Name = "dgvLibroDiario";
            this.dgvLibroDiario.RowHeadersWidth = 51;
            this.dgvLibroDiario.Size = new System.Drawing.Size(1463, 711);
            this.dgvLibroDiario.TabIndex = 0;
            this.dgvLibroDiario.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvLibroDiario_CellContentClick);
            // 
            // tbpLibroAjustado
            // 
            this.tbpLibroAjustado.Controls.Add(this.label6);
            this.tbpLibroAjustado.Controls.Add(this.label5);
            this.tbpLibroAjustado.Controls.Add(this.txtSaldoDebitoAjuste);
            this.tbpLibroAjustado.Controls.Add(this.txtSaldoHaberAjuste);
            this.tbpLibroAjustado.Controls.Add(this.btnEliminarAjuste);
            this.tbpLibroAjustado.Controls.Add(this.btnEditarAjuste);
            this.tbpLibroAjustado.Controls.Add(this.btnAgregarAjuste);
            this.tbpLibroAjustado.Controls.Add(this.dgvLibroAjustes);
            this.tbpLibroAjustado.Location = new System.Drawing.Point(4, 25);
            this.tbpLibroAjustado.Margin = new System.Windows.Forms.Padding(4);
            this.tbpLibroAjustado.Name = "tbpLibroAjustado";
            this.tbpLibroAjustado.Padding = new System.Windows.Forms.Padding(4);
            this.tbpLibroAjustado.Size = new System.Drawing.Size(1916, 785);
            this.tbpLibroAjustado.TabIndex = 1;
            this.tbpLibroAjustado.Text = "Libro ajustes";
            this.tbpLibroAjustado.UseVisualStyleBackColor = true;
            this.tbpLibroAjustado.Click += new System.EventHandler(this.tbpLibroAjustado_Click);
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(1539, 201);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(78, 16);
            this.label6.TabIndex = 15;
            this.label6.Text = "Saldo debe";
            this.label6.Visible = false;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(1539, 201);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(81, 16);
            this.label5.TabIndex = 14;
            this.label5.Text = "Saldo haber";
            this.label5.Visible = false;
            // 
            // txtSaldoDebitoAjuste
            // 
            this.txtSaldoDebitoAjuste.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSaldoDebitoAjuste.Location = new System.Drawing.Point(1539, 201);
            this.txtSaldoDebitoAjuste.Margin = new System.Windows.Forms.Padding(4);
            this.txtSaldoDebitoAjuste.Name = "txtSaldoDebitoAjuste";
            this.txtSaldoDebitoAjuste.ReadOnly = true;
            this.txtSaldoDebitoAjuste.Size = new System.Drawing.Size(132, 22);
            this.txtSaldoDebitoAjuste.TabIndex = 13;
            this.txtSaldoDebitoAjuste.TabStop = false;
            this.txtSaldoDebitoAjuste.Visible = false;
            // 
            // txtSaldoHaberAjuste
            // 
            this.txtSaldoHaberAjuste.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSaldoHaberAjuste.Location = new System.Drawing.Point(1539, 201);
            this.txtSaldoHaberAjuste.Margin = new System.Windows.Forms.Padding(4);
            this.txtSaldoHaberAjuste.Name = "txtSaldoHaberAjuste";
            this.txtSaldoHaberAjuste.ReadOnly = true;
            this.txtSaldoHaberAjuste.Size = new System.Drawing.Size(132, 22);
            this.txtSaldoHaberAjuste.TabIndex = 12;
            this.txtSaldoHaberAjuste.TabStop = false;
            this.txtSaldoHaberAjuste.Visible = false;
            // 
            // btnEliminarAjuste
            // 
            this.btnEliminarAjuste.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEliminarAjuste.Location = new System.Drawing.Point(1583, 150);
            this.btnEliminarAjuste.Margin = new System.Windows.Forms.Padding(4);
            this.btnEliminarAjuste.Name = "btnEliminarAjuste";
            this.btnEliminarAjuste.Size = new System.Drawing.Size(232, 42);
            this.btnEliminarAjuste.TabIndex = 11;
            this.btnEliminarAjuste.Text = "Eliminar ajuste";
            this.btnEliminarAjuste.UseVisualStyleBackColor = true;
            this.btnEliminarAjuste.Click += new System.EventHandler(this.btnEliminarAjuste_Click);
            // 
            // btnEditarAjuste
            // 
            this.btnEditarAjuste.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEditarAjuste.Location = new System.Drawing.Point(1583, 101);
            this.btnEditarAjuste.Margin = new System.Windows.Forms.Padding(4);
            this.btnEditarAjuste.Name = "btnEditarAjuste";
            this.btnEditarAjuste.Size = new System.Drawing.Size(232, 42);
            this.btnEditarAjuste.TabIndex = 10;
            this.btnEditarAjuste.Text = "Editar ajuste";
            this.btnEditarAjuste.UseVisualStyleBackColor = true;
            this.btnEditarAjuste.Click += new System.EventHandler(this.btnEditarAjuste_Click);
            // 
            // btnAgregarAjuste
            // 
            this.btnAgregarAjuste.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAgregarAjuste.Location = new System.Drawing.Point(1583, 51);
            this.btnAgregarAjuste.Margin = new System.Windows.Forms.Padding(4);
            this.btnAgregarAjuste.Name = "btnAgregarAjuste";
            this.btnAgregarAjuste.Size = new System.Drawing.Size(232, 42);
            this.btnAgregarAjuste.TabIndex = 9;
            this.btnAgregarAjuste.Text = "Crear nuevo ajuste";
            this.btnAgregarAjuste.UseVisualStyleBackColor = true;
            this.btnAgregarAjuste.Click += new System.EventHandler(this.btnAgregarAjuste_Click);
            // 
            // dgvLibroAjustes
            // 
            this.dgvLibroAjustes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvLibroAjustes.ColumnHeadersHeight = 29;
            this.dgvLibroAjustes.Location = new System.Drawing.Point(11, 7);
            this.dgvLibroAjustes.Margin = new System.Windows.Forms.Padding(4);
            this.dgvLibroAjustes.Name = "dgvLibroAjustes";
            this.dgvLibroAjustes.RowHeadersWidth = 51;
            this.dgvLibroAjustes.Size = new System.Drawing.Size(1463, 711);
            this.dgvLibroAjustes.TabIndex = 8;
            // 
            // tbpLibroMayorSinAjustar
            // 
            this.tbpLibroMayorSinAjustar.Location = new System.Drawing.Point(4, 25);
            this.tbpLibroMayorSinAjustar.Margin = new System.Windows.Forms.Padding(4);
            this.tbpLibroMayorSinAjustar.Name = "tbpLibroMayorSinAjustar";
            this.tbpLibroMayorSinAjustar.Size = new System.Drawing.Size(1916, 785);
            this.tbpLibroMayorSinAjustar.TabIndex = 7;
            this.tbpLibroMayorSinAjustar.Text = "Libro mayor";
            this.tbpLibroMayorSinAjustar.UseVisualStyleBackColor = true;
            // 
            // tbpLibroMayor
            // 
            this.tbpLibroMayor.Controls.Add(this.pnlLibroMayor);
            this.tbpLibroMayor.Location = new System.Drawing.Point(4, 25);
            this.tbpLibroMayor.Margin = new System.Windows.Forms.Padding(4);
            this.tbpLibroMayor.Name = "tbpLibroMayor";
            this.tbpLibroMayor.Size = new System.Drawing.Size(1916, 785);
            this.tbpLibroMayor.TabIndex = 2;
            this.tbpLibroMayor.Text = "Libro mayor ajustado";
            this.tbpLibroMayor.UseVisualStyleBackColor = true;
            // 
            // pnlLibroMayor
            // 
            this.pnlLibroMayor.AutoScroll = true;
            this.pnlLibroMayor.BackColor = System.Drawing.Color.White;
            this.pnlLibroMayor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlLibroMayor.Location = new System.Drawing.Point(0, 0);
            this.pnlLibroMayor.Margin = new System.Windows.Forms.Padding(4);
            this.pnlLibroMayor.Name = "pnlLibroMayor";
            this.pnlLibroMayor.Size = new System.Drawing.Size(1916, 785);
            this.pnlLibroMayor.TabIndex = 0;
            // 
            // tbpBalanceComprobacionSinAjustar
            // 
            this.tbpBalanceComprobacionSinAjustar.Location = new System.Drawing.Point(4, 25);
            this.tbpBalanceComprobacionSinAjustar.Margin = new System.Windows.Forms.Padding(4);
            this.tbpBalanceComprobacionSinAjustar.Name = "tbpBalanceComprobacionSinAjustar";
            this.tbpBalanceComprobacionSinAjustar.Size = new System.Drawing.Size(1916, 785);
            this.tbpBalanceComprobacionSinAjustar.TabIndex = 8;
            this.tbpBalanceComprobacionSinAjustar.Text = "Balance comprobación";
            this.tbpBalanceComprobacionSinAjustar.UseVisualStyleBackColor = true;
            // 
            // tbpBalanceComprobacion
            // 
            this.tbpBalanceComprobacion.Controls.Add(this.dgvBalanceComprobacion);
            this.tbpBalanceComprobacion.Location = new System.Drawing.Point(4, 25);
            this.tbpBalanceComprobacion.Margin = new System.Windows.Forms.Padding(4);
            this.tbpBalanceComprobacion.Name = "tbpBalanceComprobacion";
            this.tbpBalanceComprobacion.Size = new System.Drawing.Size(1916, 785);
            this.tbpBalanceComprobacion.TabIndex = 3;
            this.tbpBalanceComprobacion.Text = "Balance comprobación ajustado";
            this.tbpBalanceComprobacion.UseVisualStyleBackColor = true;
            // 
            // dgvBalanceComprobacion
            // 
            this.dgvBalanceComprobacion.ColumnHeadersHeight = 29;
            this.dgvBalanceComprobacion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvBalanceComprobacion.Location = new System.Drawing.Point(0, 0);
            this.dgvBalanceComprobacion.Margin = new System.Windows.Forms.Padding(4);
            this.dgvBalanceComprobacion.Name = "dgvBalanceComprobacion";
            this.dgvBalanceComprobacion.RowHeadersWidth = 51;
            this.dgvBalanceComprobacion.Size = new System.Drawing.Size(1916, 785);
            this.dgvBalanceComprobacion.TabIndex = 0;
            // 
            // tbpEstadoResultado
            // 
            this.tbpEstadoResultado.Controls.Add(this.dgvEstadoResultado);
            this.tbpEstadoResultado.Location = new System.Drawing.Point(4, 25);
            this.tbpEstadoResultado.Margin = new System.Windows.Forms.Padding(4);
            this.tbpEstadoResultado.Name = "tbpEstadoResultado";
            this.tbpEstadoResultado.Size = new System.Drawing.Size(1916, 785);
            this.tbpEstadoResultado.TabIndex = 4;
            this.tbpEstadoResultado.Text = "Estado resultado";
            this.tbpEstadoResultado.UseVisualStyleBackColor = true;
            // 
            // dgvEstadoResultado
            // 
            this.dgvEstadoResultado.ColumnHeadersHeight = 29;
            this.dgvEstadoResultado.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvEstadoResultado.Location = new System.Drawing.Point(0, 0);
            this.dgvEstadoResultado.Margin = new System.Windows.Forms.Padding(4);
            this.dgvEstadoResultado.Name = "dgvEstadoResultado";
            this.dgvEstadoResultado.RowHeadersWidth = 51;
            this.dgvEstadoResultado.Size = new System.Drawing.Size(1916, 785);
            this.dgvEstadoResultado.TabIndex = 0;
            // 
            // tbpBalanceGeneral
            // 
            this.tbpBalanceGeneral.Controls.Add(this.dgvBalanceGeneral);
            this.tbpBalanceGeneral.Location = new System.Drawing.Point(4, 25);
            this.tbpBalanceGeneral.Margin = new System.Windows.Forms.Padding(4);
            this.tbpBalanceGeneral.Name = "tbpBalanceGeneral";
            this.tbpBalanceGeneral.Size = new System.Drawing.Size(1916, 785);
            this.tbpBalanceGeneral.TabIndex = 5;
            this.tbpBalanceGeneral.Text = "Balance general";
            this.tbpBalanceGeneral.UseVisualStyleBackColor = true;
            // 
            // dgvBalanceGeneral
            // 
            this.dgvBalanceGeneral.ColumnHeadersHeight = 29;
            this.dgvBalanceGeneral.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvBalanceGeneral.Location = new System.Drawing.Point(0, 0);
            this.dgvBalanceGeneral.Margin = new System.Windows.Forms.Padding(4);
            this.dgvBalanceGeneral.Name = "dgvBalanceGeneral";
            this.dgvBalanceGeneral.RowHeadersWidth = 51;
            this.dgvBalanceGeneral.Size = new System.Drawing.Size(1916, 785);
            this.dgvBalanceGeneral.TabIndex = 0;
            // 
            // tblConfiguración
            // 
            this.tblConfiguración.Controls.Add(this.btnEliminarCuenta);
            this.tblConfiguración.Controls.Add(this.btnEditarCuenta);
            this.tblConfiguración.Controls.Add(this.btnCrearCuenta);
            this.tblConfiguración.Controls.Add(this.dgvCuentas);
            this.tblConfiguración.Location = new System.Drawing.Point(4, 25);
            this.tblConfiguración.Margin = new System.Windows.Forms.Padding(4);
            this.tblConfiguración.Name = "tblConfiguración";
            this.tblConfiguración.Size = new System.Drawing.Size(1916, 785);
            this.tblConfiguración.TabIndex = 6;
            this.tblConfiguración.Text = "Configuración";
            this.tblConfiguración.UseVisualStyleBackColor = true;
            // 
            // btnEliminarCuenta
            // 
            this.btnEliminarCuenta.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEliminarCuenta.Location = new System.Drawing.Point(1583, 150);
            this.btnEliminarCuenta.Margin = new System.Windows.Forms.Padding(4);
            this.btnEliminarCuenta.Name = "btnEliminarCuenta";
            this.btnEliminarCuenta.Size = new System.Drawing.Size(232, 42);
            this.btnEliminarCuenta.TabIndex = 7;
            this.btnEliminarCuenta.Text = "Eliminar cuenta";
            this.btnEliminarCuenta.UseVisualStyleBackColor = true;
            this.btnEliminarCuenta.Click += new System.EventHandler(this.btnEliminarCuenta_Click);
            // 
            // btnEditarCuenta
            // 
            this.btnEditarCuenta.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEditarCuenta.Location = new System.Drawing.Point(1583, 101);
            this.btnEditarCuenta.Margin = new System.Windows.Forms.Padding(4);
            this.btnEditarCuenta.Name = "btnEditarCuenta";
            this.btnEditarCuenta.Size = new System.Drawing.Size(232, 42);
            this.btnEditarCuenta.TabIndex = 6;
            this.btnEditarCuenta.Text = "Editar cuenta";
            this.btnEditarCuenta.UseVisualStyleBackColor = true;
            this.btnEditarCuenta.Click += new System.EventHandler(this.btnEditarCuenta_Click);
            // 
            // btnCrearCuenta
            // 
            this.btnCrearCuenta.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCrearCuenta.Location = new System.Drawing.Point(1583, 51);
            this.btnCrearCuenta.Margin = new System.Windows.Forms.Padding(4);
            this.btnCrearCuenta.Name = "btnCrearCuenta";
            this.btnCrearCuenta.Size = new System.Drawing.Size(232, 42);
            this.btnCrearCuenta.TabIndex = 5;
            this.btnCrearCuenta.Text = "Crear nueva cuenta";
            this.btnCrearCuenta.UseVisualStyleBackColor = true;
            this.btnCrearCuenta.Click += new System.EventHandler(this.btnCrearCuenta_Click);
            // 
            // dgvCuentas
            // 
            this.dgvCuentas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvCuentas.ColumnHeadersHeight = 29;
            this.dgvCuentas.Location = new System.Drawing.Point(11, 7);
            this.dgvCuentas.Margin = new System.Windows.Forms.Padding(4);
            this.dgvCuentas.Name = "dgvCuentas";
            this.dgvCuentas.RowHeadersWidth = 51;
            this.dgvCuentas.Size = new System.Drawing.Size(1463, 711);
            this.dgvCuentas.TabIndex = 4;
            // 
            // frmPrincipal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1924, 814);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(1918, 851);
            this.Name = "frmPrincipal";
            this.Text = "Contabilidad";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmPrincipal_Load);
            this.tabControl1.ResumeLayout(false);
            this.tbpLibroDiario.ResumeLayout(false);
            this.tbpLibroDiario.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLibroDiario)).EndInit();
            this.tbpLibroAjustado.ResumeLayout(false);
            this.tbpLibroAjustado.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLibroAjustes)).EndInit();
            this.tbpLibroMayor.ResumeLayout(false);
            this.tbpBalanceComprobacion.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBalanceComprobacion)).EndInit();
            this.tbpEstadoResultado.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvEstadoResultado)).EndInit();
            this.tbpBalanceGeneral.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBalanceGeneral)).EndInit();
            this.tblConfiguración.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCuentas)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tbpLibroDiario;
        private System.Windows.Forms.TabPage tbpLibroAjustado;
        private System.Windows.Forms.TabPage tbpLibroMayor;
        private System.Windows.Forms.TabPage tbpBalanceComprobacion;
        private System.Windows.Forms.TabPage tbpEstadoResultado;
        private System.Windows.Forms.TabPage tbpBalanceGeneral;
        private System.Windows.Forms.Button btnEliminarAsiento;
        private System.Windows.Forms.Button btnEditarAsiento;
        private System.Windows.Forms.Button btnAgregarAsiento;
        private Contabilidad.UI.LibroDiarioDataGridView dgvLibroDiario;
        private System.Windows.Forms.TabPage tblConfiguración;
        private System.Windows.Forms.TextBox txtSaldoHaber;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSaldoDebito;
        private System.Windows.Forms.Button btnEliminarCuenta;
        private System.Windows.Forms.Button btnEditarCuenta;
        private System.Windows.Forms.Button btnCrearCuenta;
        private Contabilidad.UI.CuentasDataGridView dgvCuentas;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtSaldoDebitoAjuste;
        private System.Windows.Forms.TextBox txtSaldoHaberAjuste;
        private System.Windows.Forms.Button btnEliminarAjuste;
        private System.Windows.Forms.Button btnEditarAjuste;
        private System.Windows.Forms.Button btnAgregarAjuste;
        private Contabilidad.UI.LibroAjustesDataGridView dgvLibroAjustes;
        private System.Windows.Forms.Panel pnlLibroMayor;
        private Contabilidad.UI.BalanceComprobacionDataGridView dgvBalanceComprobacion;
        private Contabilidad.UI.ReporteFinancieroDataGridView dgvEstadoResultado;
        private Contabilidad.UI.ReporteFinancieroDataGridView dgvBalanceGeneral;
        private System.Windows.Forms.TabPage tbpDashBoard;
        private System.Windows.Forms.TabPage tbpLibroMayorSinAjustar;
        private System.Windows.Forms.TabPage tbpBalanceComprobacionSinAjustar;
    }
}
