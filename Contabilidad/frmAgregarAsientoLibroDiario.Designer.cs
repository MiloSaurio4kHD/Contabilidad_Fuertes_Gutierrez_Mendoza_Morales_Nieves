namespace Contabilidad
{
    partial class frmAgregarAsientoLibroDiario
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dgvAsientoIndividual = new System.Windows.Forms.DataGridView();
            this.txtGlosa = new System.Windows.Forms.TextBox();
            this.btnAgregarAsiento = new System.Windows.Forms.Button();
            this.gpDebeHaber = new System.Windows.Forms.GroupBox();
            this.rbHaber = new System.Windows.Forms.RadioButton();
            this.rbDebe = new System.Windows.Forms.RadioButton();
            this.nudCantidad = new System.Windows.Forms.NumericUpDown();
            this.btnAgregarCuenta = new System.Windows.Forms.Button();
            this.cmbCuenta = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSaldoDebito = new System.Windows.Forms.TextBox();
            this.txtSaldoHaber = new System.Windows.Forms.TextBox();
            this.dtpFechaAsiento = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAsientoIndividual)).BeginInit();
            this.gpDebeHaber.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCantidad)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvAsientoIndividual
            // 
            this.dgvAsientoIndividual.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.dgvAsientoIndividual.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAsientoIndividual.Location = new System.Drawing.Point(13, 186);
            this.dgvAsientoIndividual.Margin = new System.Windows.Forms.Padding(4);
            this.dgvAsientoIndividual.Name = "dgvAsientoIndividual";
            this.dgvAsientoIndividual.Size = new System.Drawing.Size(1028, 191);
            this.dgvAsientoIndividual.TabIndex = 6;
            this.dgvAsientoIndividual.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvAsientoIndividual_CellContentClick);
            // 
            // txtGlosa
            // 
            this.txtGlosa.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtGlosa.Location = new System.Drawing.Point(13, 98);
            this.txtGlosa.Margin = new System.Windows.Forms.Padding(4);
            this.txtGlosa.Multiline = true;
            this.txtGlosa.Name = "txtGlosa";
            this.txtGlosa.Size = new System.Drawing.Size(1028, 71);
            this.txtGlosa.TabIndex = 5;
            this.txtGlosa.TextChanged += new System.EventHandler(this.txtGlosa_TextChanged);
            // 
            // btnAgregarAsiento
            // 
            this.btnAgregarAsiento.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnAgregarAsiento.Location = new System.Drawing.Point(825, 385);
            this.btnAgregarAsiento.Margin = new System.Windows.Forms.Padding(4);
            this.btnAgregarAsiento.Name = "btnAgregarAsiento";
            this.btnAgregarAsiento.Size = new System.Drawing.Size(216, 51);
            this.btnAgregarAsiento.TabIndex = 9;
            this.btnAgregarAsiento.Text = "Guardar asiento";
            this.btnAgregarAsiento.UseVisualStyleBackColor = true;
            this.btnAgregarAsiento.Click += new System.EventHandler(this.btnAgregarAsiento_Click);
            // 
            // gpDebeHaber
            // 
            this.gpDebeHaber.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.gpDebeHaber.Controls.Add(this.rbHaber);
            this.gpDebeHaber.Controls.Add(this.rbDebe);
            this.gpDebeHaber.Location = new System.Drawing.Point(430, 28);
            this.gpDebeHaber.Margin = new System.Windows.Forms.Padding(4);
            this.gpDebeHaber.Name = "gpDebeHaber";
            this.gpDebeHaber.Padding = new System.Windows.Forms.Padding(4);
            this.gpDebeHaber.Size = new System.Drawing.Size(261, 56);
            this.gpDebeHaber.TabIndex = 1;
            this.gpDebeHaber.TabStop = false;
            this.gpDebeHaber.Text = "Tipo (Debe/Haber)";
            // 
            // rbHaber
            // 
            this.rbHaber.AutoSize = true;
            this.rbHaber.Location = new System.Drawing.Point(129, 23);
            this.rbHaber.Margin = new System.Windows.Forms.Padding(4);
            this.rbHaber.Name = "rbHaber";
            this.rbHaber.Size = new System.Drawing.Size(63, 20);
            this.rbHaber.TabIndex = 1;
            this.rbHaber.TabStop = true;
            this.rbHaber.Text = "Haber";
            this.rbHaber.UseVisualStyleBackColor = true;
            this.rbHaber.CheckedChanged += new System.EventHandler(this.rbHaber_CheckedChanged);
            // 
            // rbDebe
            // 
            this.rbDebe.AutoSize = true;
            this.rbDebe.Location = new System.Drawing.Point(8, 23);
            this.rbDebe.Margin = new System.Windows.Forms.Padding(4);
            this.rbDebe.Name = "rbDebe";
            this.rbDebe.Size = new System.Drawing.Size(59, 20);
            this.rbDebe.TabIndex = 0;
            this.rbDebe.TabStop = true;
            this.rbDebe.Text = "Debe";
            this.rbDebe.UseVisualStyleBackColor = true;
            this.rbDebe.CheckedChanged += new System.EventHandler(this.rbDebe_CheckedChanged);
            // 
            // nudCantidad
            // 
            this.nudCantidad.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.nudCantidad.Location = new System.Drawing.Point(699, 48);
            this.nudCantidad.Margin = new System.Windows.Forms.Padding(4);
            this.nudCantidad.Name = "nudCantidad";
            this.nudCantidad.Size = new System.Drawing.Size(146, 22);
            this.nudCantidad.TabIndex = 3;
            this.nudCantidad.ValueChanged += new System.EventHandler(this.nudCantidad_ValueChanged);
            // 
            // btnAgregarCuenta
            // 
            this.btnAgregarCuenta.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnAgregarCuenta.Location = new System.Drawing.Point(865, 44);
            this.btnAgregarCuenta.Margin = new System.Windows.Forms.Padding(4);
            this.btnAgregarCuenta.Name = "btnAgregarCuenta";
            this.btnAgregarCuenta.Size = new System.Drawing.Size(176, 28);
            this.btnAgregarCuenta.TabIndex = 4;
            this.btnAgregarCuenta.Text = "Agregar cuenta";
            this.btnAgregarCuenta.UseVisualStyleBackColor = true;
            this.btnAgregarCuenta.Click += new System.EventHandler(this.btnAgregarCuenta_Click);
            // 
            // cmbCuenta
            // 
            this.cmbCuenta.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cmbCuenta.FormattingEnabled = true;
            this.cmbCuenta.Location = new System.Drawing.Point(17, 47);
            this.cmbCuenta.Margin = new System.Windows.Forms.Padding(4);
            this.cmbCuenta.Name = "cmbCuenta";
            this.cmbCuenta.Size = new System.Drawing.Size(385, 24);
            this.cmbCuenta.TabIndex = 0;
            this.cmbCuenta.SelectedIndexChanged += new System.EventHandler(this.cmbCuenta_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 406);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 16);
            this.label2.TabIndex = 23;
            this.label2.Text = "Saldo debe:";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(243, 406);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 16);
            this.label1.TabIndex = 22;
            this.label1.Text = "Saldo haber:";
            // 
            // txtSaldoDebito
            // 
            this.txtSaldoDebito.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtSaldoDebito.Location = new System.Drawing.Point(103, 403);
            this.txtSaldoDebito.Margin = new System.Windows.Forms.Padding(4);
            this.txtSaldoDebito.Name = "txtSaldoDebito";
            this.txtSaldoDebito.ReadOnly = true;
            this.txtSaldoDebito.Size = new System.Drawing.Size(132, 22);
            this.txtSaldoDebito.TabIndex = 7;
            this.txtSaldoDebito.TabStop = false;
            this.txtSaldoDebito.TextChanged += new System.EventHandler(this.txtSaldoDebito_TextChanged);
            // 
            // txtSaldoHaber
            // 
            this.txtSaldoHaber.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtSaldoHaber.Location = new System.Drawing.Point(332, 403);
            this.txtSaldoHaber.Margin = new System.Windows.Forms.Padding(4);
            this.txtSaldoHaber.Name = "txtSaldoHaber";
            this.txtSaldoHaber.ReadOnly = true;
            this.txtSaldoHaber.Size = new System.Drawing.Size(132, 22);
            this.txtSaldoHaber.TabIndex = 8;
            this.txtSaldoHaber.TabStop = false;
            this.txtSaldoHaber.TextChanged += new System.EventHandler(this.txtSaldoHaber_TextChanged);
            // 
            // dtpFechaAsiento
            // 
            this.dtpFechaAsiento.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.dtpFechaAsiento.Location = new System.Drawing.Point(559, 403);
            this.dtpFechaAsiento.Margin = new System.Windows.Forms.Padding(4);
            this.dtpFechaAsiento.Name = "dtpFechaAsiento";
            this.dtpFechaAsiento.Size = new System.Drawing.Size(258, 22);
            this.dtpFechaAsiento.TabIndex = 2;
            this.dtpFechaAsiento.ValueChanged += new System.EventHandler(this.dtpFechaAsiento_ValueChanged);
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 28);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 16);
            this.label3.TabIndex = 25;
            this.label3.Text = "Cuenta";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(698, 28);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 16);
            this.label4.TabIndex = 26;
            this.label4.Text = "Monto";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(503, 406);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(48, 16);
            this.label5.TabIndex = 27;
            this.label5.Text = "Fecha:";
            // 
            // frmAgregarAsientoLibroDiario
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1054, 449);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.nudCantidad);
            this.Controls.Add(this.dtpFechaAsiento);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtSaldoDebito);
            this.Controls.Add(this.txtSaldoHaber);
            this.Controls.Add(this.cmbCuenta);
            this.Controls.Add(this.gpDebeHaber);
            this.Controls.Add(this.btnAgregarCuenta);
            this.Controls.Add(this.btnAgregarAsiento);
            this.Controls.Add(this.txtGlosa);
            this.Controls.Add(this.dgvAsientoIndividual);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1070, 488);
            this.MinimumSize = new System.Drawing.Size(1070, 488);
            this.Name = "frmAgregarAsientoLibroDiario";
            this.Text = "frmAgregarAsientoLibroDiario";
            this.Load += new System.EventHandler(this.frmAgregarAsientoLibroDiario_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAsientoIndividual)).EndInit();
            this.gpDebeHaber.ResumeLayout(false);
            this.gpDebeHaber.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCantidad)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataGridView dgvAsientoIndividual;
        private System.Windows.Forms.TextBox txtGlosa;
        private System.Windows.Forms.Button btnAgregarAsiento;
        private System.Windows.Forms.GroupBox gpDebeHaber;
        private System.Windows.Forms.NumericUpDown nudCantidad;
        private System.Windows.Forms.RadioButton rbHaber;
        private System.Windows.Forms.RadioButton rbDebe;
        private System.Windows.Forms.Button btnAgregarCuenta;
        private System.Windows.Forms.ComboBox cmbCuenta;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSaldoDebito;
        private System.Windows.Forms.TextBox txtSaldoHaber;
        private System.Windows.Forms.DateTimePicker dtpFechaAsiento;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
    }
}