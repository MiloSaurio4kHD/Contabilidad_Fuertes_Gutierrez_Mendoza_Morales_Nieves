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
            ((System.ComponentModel.ISupportInitialize)(this.dgvAjusteIndividual)).BeginInit();
            this.gpDebeHaber.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCantidad)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvAjusteIndividual
            // 
            this.dgvAjusteIndividual.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAjusteIndividual.Location = new System.Drawing.Point(13, 186);
            this.dgvAjusteIndividual.Margin = new System.Windows.Forms.Padding(4);
            this.dgvAjusteIndividual.Name = "dgvAjusteIndividual";
            this.dgvAjusteIndividual.Size = new System.Drawing.Size(1028, 191);
            this.dgvAjusteIndividual.TabIndex = 6;
            this.dgvAjusteIndividual.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvAjusteIndividual_CellContentClick);
            // 
            // txtGlosa
            // 
            this.txtGlosa.Location = new System.Drawing.Point(13, 98);
            this.txtGlosa.Margin = new System.Windows.Forms.Padding(4);
            this.txtGlosa.Multiline = true;
            this.txtGlosa.Name = "txtGlosa";
            this.txtGlosa.Size = new System.Drawing.Size(1028, 71);
            this.txtGlosa.TabIndex = 5;
            // 
            // btnAgregarAjuste
            // 
            this.btnAgregarAjuste.Location = new System.Drawing.Point(825, 385);
            this.btnAgregarAjuste.Margin = new System.Windows.Forms.Padding(4);
            this.btnAgregarAjuste.Name = "btnAgregarAjuste";
            this.btnAgregarAjuste.Size = new System.Drawing.Size(216, 51);
            this.btnAgregarAjuste.TabIndex = 9;
            this.btnAgregarAjuste.Text = "Agregar ajuste";
            this.btnAgregarAjuste.UseVisualStyleBackColor = true;
            this.btnAgregarAjuste.Click += new System.EventHandler(this.btnAgregarAjuste_Click);
            // 
            // gpDebeHaber
            // 
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
            // 
            // nudCantidad
            // 
            this.nudCantidad.Location = new System.Drawing.Point(699, 48);
            this.nudCantidad.Margin = new System.Windows.Forms.Padding(4);
            this.nudCantidad.Name = "nudCantidad";
            this.nudCantidad.Size = new System.Drawing.Size(146, 22);
            this.nudCantidad.TabIndex = 3;
            // 
            // btnAgregarCuenta
            // 
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
            this.cmbCuenta.FormattingEnabled = true;
            this.cmbCuenta.Location = new System.Drawing.Point(17, 47);
            this.cmbCuenta.Margin = new System.Windows.Forms.Padding(4);
            this.cmbCuenta.Name = "cmbCuenta";
            this.cmbCuenta.Size = new System.Drawing.Size(385, 24);
            this.cmbCuenta.TabIndex = 0;
            // 
            // label2
            // 
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
            this.txtSaldoDebito.Location = new System.Drawing.Point(103, 403);
            this.txtSaldoDebito.Margin = new System.Windows.Forms.Padding(4);
            this.txtSaldoDebito.Name = "txtSaldoDebito";
            this.txtSaldoDebito.ReadOnly = true;
            this.txtSaldoDebito.Size = new System.Drawing.Size(132, 22);
            this.txtSaldoDebito.TabIndex = 7;
            this.txtSaldoDebito.TabStop = false;
            // 
            // txtSaldoHaber
            // 
            this.txtSaldoHaber.Location = new System.Drawing.Point(332, 403);
            this.txtSaldoHaber.Margin = new System.Windows.Forms.Padding(4);
            this.txtSaldoHaber.Name = "txtSaldoHaber";
            this.txtSaldoHaber.ReadOnly = true;
            this.txtSaldoHaber.Size = new System.Drawing.Size(132, 22);
            this.txtSaldoHaber.TabIndex = 8;
            this.txtSaldoHaber.TabStop = false;
            // 
            // dtpFechaAjuste
            // 
            this.dtpFechaAjuste.Location = new System.Drawing.Point(559, 403);
            this.dtpFechaAjuste.Margin = new System.Windows.Forms.Padding(4);
            this.dtpFechaAjuste.Name = "dtpFechaAjuste";
            this.dtpFechaAjuste.Size = new System.Drawing.Size(258, 22);
            this.dtpFechaAjuste.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 22);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 16);
            this.label3.TabIndex = 25;
            this.label3.Text = "Cuenta";
            // 
            // label4
            // 
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
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(506, 406);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(48, 16);
            this.label5.TabIndex = 27;
            this.label5.Text = "Fecha:";
            // 
            // frmAgregarAjusteLibroDiario
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1054, 449);
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
            this.Margin = new System.Windows.Forms.Padding(4);
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
    }
}
