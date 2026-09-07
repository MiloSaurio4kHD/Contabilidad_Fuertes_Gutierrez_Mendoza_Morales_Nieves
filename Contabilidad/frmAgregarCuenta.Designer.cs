namespace Contabilidad
{
    partial class frmAgregarCuenta
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
            this.lblCodigo = new System.Windows.Forms.Label();
            this.txtCodigo = new System.Windows.Forms.TextBox();
            this.lblNombre = new System.Windows.Forms.Label();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.gpTipo = new System.Windows.Forms.GroupBox();
            this.rbActivoCorriente = new System.Windows.Forms.RadioButton();
            this.rbActivoNoCorriente = new System.Windows.Forms.RadioButton();
            this.rbPasivoCorriente = new System.Windows.Forms.RadioButton();
            this.rbPasivoNoCorriente = new System.Windows.Forms.RadioButton();
            this.rbPatrimonio = new System.Windows.Forms.RadioButton();
            this.rbIngresos = new System.Windows.Forms.RadioButton();
            this.rbGastos = new System.Windows.Forms.RadioButton();
            this.btnGuardarCuenta = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.gpTipo.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblCodigo
            // 
            this.lblCodigo.AutoSize = true;
            this.lblCodigo.Location = new System.Drawing.Point(28, 25);
            this.lblCodigo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCodigo.Name = "lblCodigo";
            this.lblCodigo.Size = new System.Drawing.Size(54, 16);
            this.lblCodigo.TabIndex = 0;
            this.lblCodigo.Text = "Código:";
            // 
            // txtCodigo
            // 
            this.txtCodigo.Location = new System.Drawing.Point(160, 21);
            this.txtCodigo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtCodigo.Name = "txtCodigo";
            this.txtCodigo.Size = new System.Drawing.Size(199, 22);
            this.txtCodigo.TabIndex = 1;
            // 
            // lblNombre
            // 
            this.lblNombre.AutoSize = true;
            this.lblNombre.Location = new System.Drawing.Point(28, 64);
            this.lblNombre.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.Size = new System.Drawing.Size(59, 16);
            this.lblNombre.TabIndex = 2;
            this.lblNombre.Text = "Nombre:";
            // 
            // txtNombre
            // 
            this.txtNombre.Location = new System.Drawing.Point(160, 60);
            this.txtNombre.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(372, 22);
            this.txtNombre.TabIndex = 3;
            // 
            // gpTipo
            // 
            this.gpTipo.Controls.Add(this.rbActivoCorriente);
            this.gpTipo.Controls.Add(this.rbActivoNoCorriente);
            this.gpTipo.Controls.Add(this.rbPasivoCorriente);
            this.gpTipo.Controls.Add(this.rbPasivoNoCorriente);
            this.gpTipo.Controls.Add(this.rbPatrimonio);
            this.gpTipo.Controls.Add(this.rbIngresos);
            this.gpTipo.Controls.Add(this.rbGastos);
            this.gpTipo.Location = new System.Drawing.Point(28, 105);
            this.gpTipo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gpTipo.Name = "gpTipo";
            this.gpTipo.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gpTipo.Size = new System.Drawing.Size(505, 271);
            this.gpTipo.TabIndex = 4;
            this.gpTipo.TabStop = false;
            this.gpTipo.Text = "Tipo de cuenta";
            // 
            // rbActivoCorriente
            // 
            this.rbActivoCorriente.AutoSize = true;
            this.rbActivoCorriente.Location = new System.Drawing.Point(29, 34);
            this.rbActivoCorriente.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rbActivoCorriente.Name = "rbActivoCorriente";
            this.rbActivoCorriente.Size = new System.Drawing.Size(117, 20);
            this.rbActivoCorriente.TabIndex = 0;
            this.rbActivoCorriente.TabStop = true;
            this.rbActivoCorriente.Text = "Activo corriente";
            this.rbActivoCorriente.UseVisualStyleBackColor = true;
            // 
            // rbActivoNoCorriente
            // 
            this.rbActivoNoCorriente.AutoSize = true;
            this.rbActivoNoCorriente.Location = new System.Drawing.Point(29, 63);
            this.rbActivoNoCorriente.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rbActivoNoCorriente.Name = "rbActivoNoCorriente";
            this.rbActivoNoCorriente.Size = new System.Drawing.Size(135, 20);
            this.rbActivoNoCorriente.TabIndex = 1;
            this.rbActivoNoCorriente.TabStop = true;
            this.rbActivoNoCorriente.Text = "Activo no corriente";
            this.rbActivoNoCorriente.UseVisualStyleBackColor = true;
            // 
            // rbPasivoCorriente
            // 
            this.rbPasivoCorriente.AutoSize = true;
            this.rbPasivoCorriente.Location = new System.Drawing.Point(29, 91);
            this.rbPasivoCorriente.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rbPasivoCorriente.Name = "rbPasivoCorriente";
            this.rbPasivoCorriente.Size = new System.Drawing.Size(122, 20);
            this.rbPasivoCorriente.TabIndex = 2;
            this.rbPasivoCorriente.TabStop = true;
            this.rbPasivoCorriente.Text = "Pasivo corriente";
            this.rbPasivoCorriente.UseVisualStyleBackColor = true;
            // 
            // rbPasivoNoCorriente
            // 
            this.rbPasivoNoCorriente.AutoSize = true;
            this.rbPasivoNoCorriente.Location = new System.Drawing.Point(29, 119);
            this.rbPasivoNoCorriente.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rbPasivoNoCorriente.Name = "rbPasivoNoCorriente";
            this.rbPasivoNoCorriente.Size = new System.Drawing.Size(140, 20);
            this.rbPasivoNoCorriente.TabIndex = 3;
            this.rbPasivoNoCorriente.TabStop = true;
            this.rbPasivoNoCorriente.Text = "Pasivo no corriente";
            this.rbPasivoNoCorriente.UseVisualStyleBackColor = true;
            // 
            // rbPatrimonio
            // 
            this.rbPatrimonio.AutoSize = true;
            this.rbPatrimonio.Location = new System.Drawing.Point(29, 148);
            this.rbPatrimonio.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rbPatrimonio.Name = "rbPatrimonio";
            this.rbPatrimonio.Size = new System.Drawing.Size(89, 20);
            this.rbPatrimonio.TabIndex = 4;
            this.rbPatrimonio.TabStop = true;
            this.rbPatrimonio.Text = "Patrimonio";
            this.rbPatrimonio.UseVisualStyleBackColor = true;
            // 
            // rbIngresos
            // 
            this.rbIngresos.AutoSize = true;
            this.rbIngresos.Location = new System.Drawing.Point(29, 176);
            this.rbIngresos.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rbIngresos.Name = "rbIngresos";
            this.rbIngresos.Size = new System.Drawing.Size(77, 20);
            this.rbIngresos.TabIndex = 5;
            this.rbIngresos.TabStop = true;
            this.rbIngresos.Text = "Ingresos";
            this.rbIngresos.UseVisualStyleBackColor = true;
            // 
            // rbGastos
            // 
            this.rbGastos.AutoSize = true;
            this.rbGastos.Location = new System.Drawing.Point(29, 204);
            this.rbGastos.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rbGastos.Name = "rbGastos";
            this.rbGastos.Size = new System.Drawing.Size(68, 20);
            this.rbGastos.TabIndex = 6;
            this.rbGastos.TabStop = true;
            this.rbGastos.Text = "Gastos";
            this.rbGastos.UseVisualStyleBackColor = true;
            // 
            // btnGuardarCuenta
            // 
            this.btnGuardarCuenta.Location = new System.Drawing.Point(160, 394);
            this.btnGuardarCuenta.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnGuardarCuenta.Name = "btnGuardarCuenta";
            this.btnGuardarCuenta.Size = new System.Drawing.Size(173, 37);
            this.btnGuardarCuenta.TabIndex = 5;
            this.btnGuardarCuenta.Text = "Guardar";
            this.btnGuardarCuenta.UseVisualStyleBackColor = true;
            this.btnGuardarCuenta.Click += new System.EventHandler(this.btnGuardarCuenta_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancelar.Location = new System.Drawing.Point(347, 394);
            this.btnCancelar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(133, 37);
            this.btnCancelar.TabIndex = 6;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // frmAgregarCuenta
            // 
            this.AcceptButton = this.btnGuardarCuenta;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancelar;
            this.ClientSize = new System.Drawing.Size(564, 462);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnGuardarCuenta);
            this.Controls.Add(this.gpTipo);
            this.Controls.Add(this.txtNombre);
            this.Controls.Add(this.lblNombre);
            this.Controls.Add(this.txtCodigo);
            this.Controls.Add(this.lblCodigo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAgregarCuenta";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "frmAgregarCuenta";
            this.Load += new System.EventHandler(this.frmAgregarCuenta_Load);
            this.gpTipo.ResumeLayout(false);
            this.gpTipo.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblCodigo;
        private System.Windows.Forms.TextBox txtCodigo;
        private System.Windows.Forms.Label lblNombre;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.GroupBox gpTipo;
        private System.Windows.Forms.RadioButton rbActivoCorriente;
        private System.Windows.Forms.RadioButton rbActivoNoCorriente;
        private System.Windows.Forms.RadioButton rbPasivoCorriente;
        private System.Windows.Forms.RadioButton rbPasivoNoCorriente;
        private System.Windows.Forms.RadioButton rbPatrimonio;
        private System.Windows.Forms.RadioButton rbIngresos;
        private System.Windows.Forms.RadioButton rbGastos;
        private System.Windows.Forms.Button btnGuardarCuenta;
        private System.Windows.Forms.Button btnCancelar;
    }
}
