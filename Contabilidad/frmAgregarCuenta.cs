using Contabilidad.Data;
using Contabilidad.Models;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Contabilidad
{
    public partial class frmAgregarCuenta : Form
    {
        private readonly CuentaRepository _cuentaRepository = new CuentaRepository();
        private readonly ModoFormulario _modo;
        private readonly string _codigoOriginal;
        private string _cuentaPadreOriginal;
        private int _nivelOriginal = 3;

        public Cuenta Cuenta { get; private set; }

        /// <summary>
        /// Constructor sin parametros requerido por el diseñador de Windows Forms.
        /// El codigo de la aplicacion siempre debe usar el constructor con ModoFormulario.
        /// </summary>
        public frmAgregarCuenta() : this(ModoFormulario.Crear)
        {
        }

        public frmAgregarCuenta(ModoFormulario modo, Cuenta cuentaExistente = null)
        {
            InitializeComponent();

            if (!Contabilidad.UI.GridStyleHelper.EnDisenio)
            {
                Contabilidad.UI.IconHelper.AplicarIconoBoton(btnGuardarCuenta, "Save.ico");
            }

            _modo = modo;

            if (_modo == ModoFormulario.Crear)
            {
                Text = "Guardar cuenta";
                btnGuardarCuenta.Text = "Guardar cuenta";
                rbActivoCorriente.Checked = true;
            }
            else
            {
                Text = "Editar cuenta";
                btnGuardarCuenta.Text = "Guardar cambios";

                if (cuentaExistente == null)
                    throw new ArgumentNullException(nameof(cuentaExistente), "Debe proporcionar la cuenta a editar.");

                _codigoOriginal = cuentaExistente.Codigo;
                _cuentaPadreOriginal = cuentaExistente.CuentaPadre;
                _nivelOriginal = cuentaExistente.Nivel;
                txtCodigo.Text = cuentaExistente.Codigo;
                txtNombre.Text = cuentaExistente.Nombre;
                MarcarTipoSeleccionado(cuentaExistente.Tipo);
            }
        }

        private void MarcarTipoSeleccionado(TipoCuenta tipo)
        {
            switch (tipo)
            {
                case TipoCuenta.ActivoCorriente: rbActivoCorriente.Checked = true; break;
                case TipoCuenta.ActivoNoCorriente: rbActivoNoCorriente.Checked = true; break;
                case TipoCuenta.PasivoCorriente: rbPasivoCorriente.Checked = true; break;
                case TipoCuenta.PasivoNoCorriente: rbPasivoNoCorriente.Checked = true; break;
                case TipoCuenta.Patrimonio: rbPatrimonio.Checked = true; break;
                case TipoCuenta.Ingresos: rbIngresos.Checked = true; break;
                case TipoCuenta.Gastos: rbGastos.Checked = true; break;
            }
        }

        private TipoCuenta? ObtenerTipoSeleccionado()
        {
            if (rbActivoCorriente.Checked) return TipoCuenta.ActivoCorriente;
            if (rbActivoNoCorriente.Checked) return TipoCuenta.ActivoNoCorriente;
            if (rbPasivoCorriente.Checked) return TipoCuenta.PasivoCorriente;
            if (rbPasivoNoCorriente.Checked) return TipoCuenta.PasivoNoCorriente;
            if (rbPatrimonio.Checked) return TipoCuenta.Patrimonio;
            if (rbIngresos.Checked) return TipoCuenta.Ingresos;
            if (rbGastos.Checked) return TipoCuenta.Gastos;
            return null;
        }

        private void btnGuardarCuenta_Click(object sender, EventArgs e)
        {
            string codigo = txtCodigo.Text.Trim();
            string nombre = txtNombre.Text.Trim();
            var tipo = ObtenerTipoSeleccionado();

            if (string.IsNullOrEmpty(codigo) || string.IsNullOrEmpty(nombre) || tipo == null)
            {
                MessageBox.Show(this, "Debe ingresar el código, el nombre y seleccionar el tipo de cuenta.",
                    "Datos incompletos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var cuentasExistentes = _cuentaRepository.ObtenerTodas()
                .Where(c => !string.Equals(c.Codigo, _codigoOriginal, StringComparison.OrdinalIgnoreCase))
                .ToList();

            bool codigoDuplicado = cuentasExistentes.Any(c =>
                string.Equals(c.Codigo.Trim(), codigo, StringComparison.OrdinalIgnoreCase));

            if (codigoDuplicado)
            {
                MessageBox.Show(this, string.Format("Ya existe una cuenta con el código '{0}'. Ingrese un código distinto.", codigo),
                    "Código repetido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCodigo.Clear();
                txtCodigo.Focus();
                return;
            }

            bool nombreDuplicado = cuentasExistentes.Any(c =>
                NormalizarNombre(c.Nombre) == NormalizarNombre(nombre));

            if (nombreDuplicado)
            {
                MessageBox.Show(this, string.Format("Ya existe una cuenta con el nombre '{0}'. Ingrese un nombre distinto.", nombre),
                    "Nombre repetido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Clear();
                txtNombre.Focus();
                return;
            }

            Cuenta = new Cuenta
            {
                Codigo = codigo,
                Nombre = nombre,
                Tipo = tipo.Value,
                Elemento = tipo.Value.ToString(),
                Grupo = tipo.Value.ToTextoAmigable(),
                CuentaPadre = _cuentaPadreOriginal,
                Nivel = _nivelOriginal,
                Naturaleza = tipo.Value.ObtenerNaturaleza()
            };

            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// Normaliza un nombre de cuenta para comparar duplicados sin importar
        /// mayusculas/minusculas ni espacios extra al inicio, al final o entre palabras.
        /// </summary>
        private static string NormalizarNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre)) return string.Empty;
            var palabras = nombre.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            return string.Join(" ", palabras).ToUpperInvariant();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void frmAgregarCuenta_Load(object sender, EventArgs e)
        {

        }
    }
}
