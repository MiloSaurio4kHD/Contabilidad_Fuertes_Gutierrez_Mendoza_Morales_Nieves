using Contabilidad.Data;
using Contabilidad.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace Contabilidad
{
    /// <summary>
    /// Alta/edicion de un ajuste del Libro de Ajustes. Es practicamente identico a
    /// frmAgregarAsientoLibroDiario, salvo que numera con prefijo "A" (A1, A2, ...)
    /// y persiste contra AjusteRepository en vez de LibroDiarioRepository.
    /// </summary>
    public partial class frmAgregarAjusteLibroDiario : Form
    {
        private readonly CuentaRepository _cuentaRepository = new CuentaRepository();
        private readonly ModoFormulario _modo;
        private readonly int _numero;
        private readonly List<DetalleAsiento> _detalles = new List<DetalleAsiento>();
        private List<Cuenta> _todasLasCuentas = new List<Cuenta>();
        private bool _actualizandoListaCuentas;
        private decimal _valorCantidadAntesDeEditar;

        public Ajuste Ajuste { get; private set; }

        /// <summary>
        /// Constructor sin parametros requerido por el diseñador de Windows Forms.
        /// El codigo de la aplicacion siempre debe usar el constructor con ModoFormulario.
        /// </summary>
        public frmAgregarAjusteLibroDiario() : this(ModoFormulario.Crear, 1)
        {
        }

        public frmAgregarAjusteLibroDiario(ModoFormulario modo, int numero, Ajuste ajusteExistente = null)
        {
            InitializeComponent();

            _modo = modo;
            _numero = numero;

            ConfigurarGridDetalle();
            ConfigurarCampoCantidad();
            CargarCuentas();
            ConfigurarIconos();

            if (_modo == ModoFormulario.Crear)
            {
                Text = "Agregar ajuste";
                btnAgregarAjuste.Text = "Agregar ajuste";
                dtpFechaAjuste.Value = DateTime.Today;
            }
            else
            {
                Text = "Editar ajuste";
                btnAgregarAjuste.Text = "Guardar cambios";

                if (ajusteExistente == null)
                    throw new ArgumentNullException(nameof(ajusteExistente), "Debe proporcionar el ajuste a editar.");

                dtpFechaAjuste.Value = ajusteExistente.Fecha;
                txtGlosa.Text = ajusteExistente.Glosa;
                foreach (var detalle in ajusteExistente.Detalles)
                {
                    _detalles.Add(new DetalleAsiento
                    {
                        CuentaCodigo = detalle.CuentaCodigo,
                        CuentaNombre = detalle.CuentaNombre,
                        Debe = detalle.Debe,
                        Haber = detalle.Haber
                    });
                }
            }

            RefrescarGridDetalle();
        }

        private void ConfigurarIconos()
        {
            if (UI.GridStyleHelper.EnDisenio) return;

            UI.IconHelper.AplicarIconoBoton(btnAgregarCuenta, "Add.ico");
            UI.IconHelper.AplicarIconoBoton(btnAgregarAjuste, "Save.ico");
        }

        private void ConfigurarGridDetalle()
        {
            dgvAjusteIndividual.AutoGenerateColumns = false;
            dgvAjusteIndividual.Columns.Clear();
            dgvAjusteIndividual.ReadOnly = false;
            dgvAjusteIndividual.AllowUserToAddRows = false;
            dgvAjusteIndividual.AllowUserToDeleteRows = false;
            dgvAjusteIndividual.RowHeadersVisible = false;
            dgvAjusteIndividual.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAjusteIndividual.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvAjusteIndividual.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colCuenta",
                HeaderText = "Cuenta",
                ReadOnly = true,
                FillWeight = 50,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
            dgvAjusteIndividual.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDebe",
                HeaderText = "Debe",
                ReadOnly = true,
                FillWeight = 16,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight },
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
            dgvAjusteIndividual.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colHaber",
                HeaderText = "Haber",
                ReadOnly = true,
                FillWeight = 16,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight },
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
            dgvAjusteIndividual.Columns.Add(new UI.BotonConIconoColumn
            {
                Name = "colEditar",
                HeaderText = string.Empty,
                Texto = "Editar",
                Icono = UI.IconHelper.ObtenerBitmap("edit16px.ico", 16),
                ReadOnly = true,
                FillWeight = 13,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
            dgvAjusteIndividual.Columns.Add(new UI.BotonConIconoColumn
            {
                Name = "colQuitar",
                HeaderText = string.Empty,
                Texto = "Quitar",
                Icono = UI.IconHelper.ObtenerBitmap("Quitar16px.ico", 16),
                ReadOnly = true,
                FillWeight = 13,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
        }

        /// <summary>
        /// Permite ingresar montos grandes con decimales: al entrar al campo se borra
        /// el valor para escribir uno nuevo desde cero, y si se sale sin escribir nada
        /// se conserva el valor que ya tenia. Ademas "." y "," funcionan igual como
        /// separador decimal, sin importar la configuracion regional.
        /// </summary>
        private void ConfigurarCampoCantidad()
        {
            nudCantidad.Maximum = 999999999.99m;
            nudCantidad.DecimalPlaces = 2;
            nudCantidad.Increment = 1m;
            nudCantidad.Enter += NudCantidad_Enter;
            nudCantidad.Leave += NudCantidad_Leave;
            nudCantidad.KeyPress += NudCantidad_KeyPress;
        }

        private void NudCantidad_Enter(object sender, EventArgs e)
        {
            _valorCantidadAntesDeEditar = nudCantidad.Value;
            nudCantidad.Text = string.Empty;
        }

        private void NudCantidad_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nudCantidad.Text))
            {
                nudCantidad.Value = _valorCantidadAntesDeEditar;
            }
        }

        private void NudCantidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            char separadorDecimal = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
            if ((e.KeyChar == '.' || e.KeyChar == ',') && e.KeyChar != separadorDecimal)
            {
                e.KeyChar = separadorDecimal;
            }
        }

        private void CargarCuentas()
        {
            _todasLasCuentas = _cuentaRepository.ObtenerTodas();

            cmbCuenta.DropDownStyle = ComboBoxStyle.DropDown;
            cmbCuenta.AutoCompleteMode = AutoCompleteMode.None;
            cmbCuenta.AutoCompleteSource = AutoCompleteSource.None;
            cmbCuenta.TextChanged += CmbCuenta_TextChanged;

            AplicarFiltroCuentas(string.Empty);
        }

        /// <summary>
        /// Filtra las cuentas del combo por coincidencia al inicio del nombre o del codigo,
        /// conservando el texto que el usuario esta escribiendo.
        /// </summary>
        private void AplicarFiltroCuentas(string filtro)
        {
            var coincidencias = string.IsNullOrWhiteSpace(filtro)
                ? _todasLasCuentas
                : _todasLasCuentas.Where(c =>
                    c.Nombre.StartsWith(filtro, StringComparison.OrdinalIgnoreCase) ||
                    c.Codigo.StartsWith(filtro, StringComparison.OrdinalIgnoreCase)).ToList();

            _actualizandoListaCuentas = true;
            cmbCuenta.Items.Clear();
            cmbCuenta.Items.AddRange(coincidencias.Cast<object>().ToArray());
            _actualizandoListaCuentas = false;
        }

        private void CmbCuenta_TextChanged(object sender, EventArgs e)
        {
            if (_actualizandoListaCuentas) return;

            // Si el texto coincide con la cuenta actualmente seleccionada, el cambio vino de
            // elegir un elemento de la lista (mouse o teclado), no de escribir para buscar.
            // En ese caso no se debe volver a filtrar: eso vaciaba la lista y perdia la seleccion.
            var seleccionActual = cmbCuenta.SelectedItem as Cuenta;
            if (seleccionActual != null && cmbCuenta.Text == seleccionActual.CodigoNombre)
            {
                return;
            }

            string texto = cmbCuenta.Text;
            int posicionCursor = cmbCuenta.SelectionStart;

            // Si el desplegable esta abierto mientras se reemplazan los Items, la lista nativa
            // queda inestable (se vacia visualmente y no deja hacer clic en un elemento).
            // Por eso se cierra antes de tocar Items y se vuelve a abrir ya con la lista final.
            bool estabaDesplegado = cmbCuenta.DroppedDown;
            if (estabaDesplegado) cmbCuenta.DroppedDown = false;

            AplicarFiltroCuentas(texto);

            _actualizandoListaCuentas = true;
            cmbCuenta.Text = texto;
            cmbCuenta.SelectionStart = Math.Min(posicionCursor, texto.Length);
            cmbCuenta.SelectionLength = 0;
            _actualizandoListaCuentas = false;

            if (!string.IsNullOrEmpty(texto) && cmbCuenta.Items.Count > 0)
            {
                cmbCuenta.DroppedDown = true;
            }
        }

        private void RefrescarGridDetalle()
        {
            dgvAjusteIndividual.Rows.Clear();
            foreach (var detalle in _detalles)
            {
                int idx = dgvAjusteIndividual.Rows.Add(
                    detalle.CuentaCodigoNombre,
                    detalle.Debe > 0 ? detalle.Debe.ToString("N2", CultureInfo.InvariantCulture) : string.Empty,
                    detalle.Haber > 0 ? detalle.Haber.ToString("N2", CultureInfo.InvariantCulture) : string.Empty);
                dgvAjusteIndividual.Rows[idx].Tag = detalle;
            }

            decimal totalDebe = _detalles.Sum(d => d.Debe);
            decimal totalHaber = _detalles.Sum(d => d.Haber);
            txtSaldoDebito.Text = totalDebe.ToString("N2", CultureInfo.InvariantCulture);
            txtSaldoHaber.Text = totalHaber.ToString("N2", CultureInfo.InvariantCulture);
        }

        private void btnAgregarCuenta_Click(object sender, EventArgs e)
        {
            var cuenta = cmbCuenta.SelectedItem as Cuenta;
            bool ladoSeleccionado = rbDebe.Checked || rbHaber.Checked;

            if (cuenta == null || !ladoSeleccionado || nudCantidad.Value <= 0)
            {
                MessageBox.Show(this,
                    "Para agregar una línea debe seleccionar la cuenta, indicar si es Debe o Haber e ingresar un monto mayor a 0.",
                    "Datos incompletos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var detalle = new DetalleAsiento
            {
                CuentaCodigo = cuenta.Codigo,
                CuentaNombre = cuenta.Nombre,
                Debe = rbDebe.Checked ? nudCantidad.Value : 0,
                Haber = rbHaber.Checked ? nudCantidad.Value : 0
            };

            _detalles.Add(detalle);
            RefrescarGridDetalle();

            nudCantidad.Value = 0;
            rbDebe.Checked = false;
            rbHaber.Checked = false;

            _actualizandoListaCuentas = true;
            cmbCuenta.Text = string.Empty;
            _actualizandoListaCuentas = false;
            AplicarFiltroCuentas(string.Empty);
        }

        private void dgvAjusteIndividual_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            string nombreColumna = dgvAjusteIndividual.Columns[e.ColumnIndex].Name;
            if (nombreColumna != "colQuitar" && nombreColumna != "colEditar") return;

            var detalle = dgvAjusteIndividual.Rows[e.RowIndex].Tag as DetalleAsiento;
            if (detalle == null) return;

            _detalles.Remove(detalle);
            RefrescarGridDetalle();

            if (nombreColumna == "colEditar")
            {
                CargarLineaEnFormulario(detalle);
            }
        }

        /// <summary>
        /// Trae de vuelta a los campos de arriba (Cuenta, Debe/Haber, Monto) los datos
        /// de una linea que ya se habia agregado, para poder corregirla en vez de tener
        /// que borrarla y volver a escribirla desde cero.
        /// </summary>
        private void CargarLineaEnFormulario(DetalleAsiento detalle)
        {
            var cuenta = _todasLasCuentas.FirstOrDefault(c =>
                string.Equals(c.Codigo, detalle.CuentaCodigo, StringComparison.OrdinalIgnoreCase));

            _actualizandoListaCuentas = true;
            AplicarFiltroCuentas(string.Empty);
            cmbCuenta.SelectedItem = cuenta;
            cmbCuenta.Text = cuenta != null ? cuenta.CodigoNombre : detalle.CuentaCodigoNombre;
            _actualizandoListaCuentas = false;

            rbDebe.Checked = detalle.Debe > 0;
            rbHaber.Checked = detalle.Haber > 0;
            nudCantidad.Value = detalle.Debe > 0 ? detalle.Debe : detalle.Haber;

            cmbCuenta.Focus();
        }

        private void btnAgregarAjuste_Click(object sender, EventArgs e)
        {
            string glosa = txtGlosa.Text.Trim();
            decimal totalDebe = _detalles.Sum(d => d.Debe);
            decimal totalHaber = _detalles.Sum(d => d.Haber);

            if (_detalles.Count < 2)
            {
                MessageBox.Show(this, "El ajuste debe tener al menos dos líneas (una en el Debe y otra en el Haber).",
                    "Ajuste incompleto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(glosa))
            {
                MessageBox.Show(this, "Debe ingresar la glosa del ajuste.",
                    "Ajuste incompleto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (totalDebe <= 0 || totalHaber <= 0 || totalDebe != totalHaber)
            {
                MessageBox.Show(this,
                    string.Format("El ajuste no está balanceado. Debe: {0:N2}  Haber: {1:N2}", totalDebe, totalHaber),
                    "Ajuste descuadrado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Ajuste = new Ajuste
            {
                Numero = _numero,
                Fecha = dtpFechaAjuste.Value.Date,
                Glosa = glosa,
                Detalles = _detalles.ToList()
            };

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
