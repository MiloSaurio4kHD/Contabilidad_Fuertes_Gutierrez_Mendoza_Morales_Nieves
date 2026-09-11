namespace Contabilidad
{
    partial class frmAgregarAjusteLibroDiario
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAgregarAjusteLibroDiario));
            this.dgvAjusteIndividual = new System.Windows.Forms.DataGridView();
            this.txtGlosa = new System.Windows.Forms.TextBox();
            this.btnAgregarAjuste = new System.Windows.Forms.Button();
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
            this.dtpFechaAjuste = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAjusteIndividual)).BeginInit();
            this.gpDebeHaber.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCantidad)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvAjusteIndividual
            // 
            this.dgvAjusteIndividual.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.dgvAjusteIndividual.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAjusteIndividual.Location = new System.Drawing.Point(10, 151);
            this.dgvAjusteIndividual.Name = "dgvAjusteIndividual";
            this.dgvAjusteIndividual.RowHeadersWidth = 51;
            this.dgvAjusteIndividual.Size = new System.Drawing.Size(771, 155);
            this.dgvAjusteIndividual.TabIndex = 6;
            this.dgvAjusteIndividual.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvAjusteIndividual_CellContentClick);
            // 
            // txtGlosa
            // 
            this.txtGlosa.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtGlosa.Location = new System.Drawing.Point(10, 80);
            this.txtGlosa.Multiline = true;
            this.txtGlosa.Name = "txtGlosa";
            this.txtGlosa.Size = new System.Drawing.Size(772, 58);
            this.txtGlosa.TabIndex = 5;
            // 
            // btnAgregarAjuste
            // 
            this.btnAgregarAjuste.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnAgregarAjuste.Location = new System.Drawing.Point(619, 313);
            this.btnAgregarAjuste.Name = "btnAgregarAjuste";
            this.btnAgregarAjuste.Size = new System.Drawing.Size(162, 41);
            this.btnAgregarAjuste.TabIndex = 9;
            this.btnAgregarAjuste.Text = "Agregar ajuste";
            this.btnAgregarAjuste.UseVisualStyleBackColor = true;
            this.btnAgregarAjuste.Click += new System.EventHandler(this.btnAgregarAjuste_Click);
            // 
            // gpDebeHaber
            // 
            this.gpDebeHaber.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.gpDebeHaber.Controls.Add(this.rbHaber);
            this.gpDebeHaber.Controls.Add(this.rbDebe);
            this.gpDebeHaber.Location = new System.Drawing.Point(322, 23);
            this.gpDebeHaber.Name = "gpDebeHaber";
            this.gpDebeHaber.Size = new System.Drawing.Size(196, 46);
            this.gpDebeHaber.TabIndex = 1;
            this.gpDebeHaber.TabStop = false;
            this.gpDebeHaber.Text = "Tipo (Debe/Haber)";
            // 
            // rbHaber
            // 
            this.rbHaber.AutoSize = true;
            this.rbHaber.Location = new System.Drawing.Point(97, 19);
            this.rbHaber.Name = "rbHaber";
            this.rbHaber.Size = new System.Drawing.Size(54, 17);
            this.rbHaber.TabIndex = 1;
            this.rbHaber.TabStop = true;
            this.rbHaber.Text = "Haber";
            this.rbHaber.UseVisualStyleBackColor = true;
            // 
            // rbDebe
            // 
            this.rbDebe.AutoSize = true;
            this.rbDebe.Location = new System.Drawing.Point(6, 19);
            this.rbDebe.Name = "rbDebe";
            this.rbDebe.Size = new System.Drawing.Size(51, 17);
            this.rbDebe.TabIndex = 0;
            this.rbDebe.TabStop = true;
            this.rbDebe.Text = "Debe";
            this.rbDebe.UseVisualStyleBackColor = true;
            // 
            // nudCantidad
            // 
            this.nudCantidad.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.nudCantidad.Location = new System.Drawing.Point(524, 39);
            this.nudCantidad.Name = "nudCantidad";
            this.nudCantidad.Size = new System.Drawing.Size(110, 20);
            this.nudCantidad.TabIndex = 3;
            // 
            // btnAgregarCuenta
            // 
            this.btnAgregarCuenta.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnAgregarCuenta.Location = new System.Drawing.Point(649, 36);
            this.btnAgregarCuenta.Name = "btnAgregarCuenta";
            this.btnAgregarCuenta.Size = new System.Drawing.Size(132, 23);
            this.btnAgregarCuenta.TabIndex = 4;
            this.btnAgregarCuenta.Text = "Agregar cuenta";
            this.btnAgregarCuenta.UseVisualStyleBackColor = true;
            this.btnAgregarCuenta.Click += new System.EventHandler(this.btnAgregarCuenta_Click);
            // 
            // cmbCuenta
            // 
            this.cmbCuenta.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cmbCuenta.FormattingEnabled = true;
            this.cmbCuenta.Location = new System.Drawing.Point(13, 38);
            this.cmbCuenta.Name = "cmbCuenta";
            this.cmbCuenta.Size = new System.Drawing.Size(290, 21);
            this.cmbCuenta.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 330);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 23;
            this.label2.Text = "Saldo debe:";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(182, 330);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 22;
            this.label1.Text = "Saldo haber:";
            // 
            // txtSaldoDebito
            // 
            this.txtSaldoDebito.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtSaldoDebito.Location = new System.Drawing.Point(77, 327);
            this.txtSaldoDebito.Name = "txtSaldoDebito";
            this.txtSaldoDebito.ReadOnly = true;
            this.txtSaldoDebito.Size = new System.Drawing.Size(100, 20);
            this.txtSaldoDebito.TabIndex = 7;
            this.txtSaldoDebito.TabStop = false;
            // 
            // txtSaldoHaber
            // 
            this.txtSaldoHaber.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtSaldoHaber.Location = new System.Drawing.Point(249, 327);
            this.txtSaldoHaber.Name = "txtSaldoHaber";
            this.txtSaldoHaber.ReadOnly = true;
            this.txtSaldoHaber.Size = new System.Drawing.Size(100, 20);
            this.txtSaldoHaber.TabIndex = 8;
            this.txtSaldoHaber.TabStop = false;
            // 
            // dtpFechaAjuste
            // 
            this.dtpFechaAjuste.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.dtpFechaAjuste.Location = new System.Drawing.Point(419, 327);
            this.dtpFechaAjuste.Name = "dtpFechaAjuste";
            this.dtpFechaAjuste.Size = new System.Drawing.Size(194, 20);
            this.dtpFechaAjuste.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 25;
            this.label3.Text = "Cuenta";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(524, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 13);
            this.label4.TabIndex = 26;
            this.label4.Text = "Monto";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(377, 330);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 13);
            this.label5.TabIndex = 27;
            this.label5.Text = "Fecha:";
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 64);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(34, 13);
            this.label6.TabIndex = 28;
            this.label6.Text = "Glosa";
            // 
            // frmAgregarAjusteLibroDiario
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(790, 365);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.nudCantidad);
            this.Controls.Add(this.dtpFechaAjuste);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtSaldoDebito);
            this.Controls.Add(this.txtSaldoHaber);
            this.Controls.Add(this.cmbCuenta);
            this.Controls.Add(this.gpDebeHaber);
            this.Controls.Add(this.btnAgregarCuenta);
            this.Controls.Add(this.btnAgregarAjuste);
            this.Controls.Add(this.txtGlosa);
            this.Controls.Add(this.dgvAjusteIndividual);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(806, 404);
            this.MinimumSize = new System.Drawing.Size(806, 404);
            this.Name = "frmAgregarAjusteLibroDiario";
            this.Text = "frmAgregarAjusteLibroDiario";
            ((System.ComponentModel.ISupportInitialize)(this.dgvAjusteIndividual)).EndInit();
            this.gpDebeHaber.ResumeLayout(false);
            this.gpDebeHaber.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCantidad)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataGridView dgvAjusteIndividual;
        private System.Windows.Forms.TextBox txtGlosa;
        private System.Windows.Forms.Button btnAgregarAjuste;
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
        private System.Windows.Forms.DateTimePicker dtpFechaAjuste;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
    }
}
