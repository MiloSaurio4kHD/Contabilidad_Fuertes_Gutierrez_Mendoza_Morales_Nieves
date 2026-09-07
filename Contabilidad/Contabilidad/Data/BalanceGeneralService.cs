using System;
using System.Linq;
using Contabilidad.Models;

namespace Contabilidad.Data
{
    /// <summary>
    /// Calcula el Balance General a partir del Libro Mayor y del Estado de Resultados:
    /// agrupa las cuentas de activo/pasivo/patrimonio con movimiento, y agrega como
    /// pasivo corriente calculado la participacion de trabajadores y el impuesto a la
    /// renta, y como patrimonio la utilidad del ejercicio (todavia no hay asiento de
    /// cierre que los registre en el Libro Diario). No persiste nada: siempre se recalcula.
    /// </summary>
    public class BalanceGeneralService
    {
        private readonly LibroMayorService _libroMayorService = new LibroMayorService();
        private readonly CuentaRepository _cuentaRepository = new CuentaRepository();
        private readonly EstadoResultadoService _estadoResultadoService = new EstadoResultadoService();

        public BalanceGeneral Generar()
        {
            var cuentasMayor = _libroMayorService.ObtenerLibroMayor();
            var catalogo = _cuentaRepository.ObtenerTodas()
                .GroupBy(c => c.Codigo, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);
            var estadoResultado = _estadoResultadoService.Generar();

            var balance = new BalanceGeneral();

            foreach (var cuentaMayor in cuentasMayor.OrderBy(c => c.Codigo))
            {
                Cuenta cuenta;
                if (!catalogo.TryGetValue(cuentaMayor.Codigo, out cuenta)) continue;
                if (cuentaMayor.Saldo == 0) continue;

                switch (cuenta.Tipo)
                {
                    case TipoCuenta.ActivoCorriente:
                        balance.ActivosCorrientes.Add(new LineaCuentaMonto { Nombre = cuenta.Nombre, Monto = cuentaMayor.Saldo, Codigo = cuentaMayor.Codigo });
                        break;

                    case TipoCuenta.ActivoNoCorriente:
                        // Si el saldo neto quedo acreedor (depreciacion/amortizacion acumulada,
                        // por ejemplo), Saldo ya es negativo y se resta solo al sumar la lista.
                        balance.ActivosNoCorrientes.Add(new LineaCuentaMonto { Nombre = cuenta.Nombre, Monto = cuentaMayor.Saldo, Codigo = cuentaMayor.Codigo });
                        break;

                    case TipoCuenta.PasivoCorriente:
                        balance.PasivosCorrientes.Add(new LineaCuentaMonto { Nombre = cuenta.Nombre, Monto = -cuentaMayor.Saldo, Codigo = cuentaMayor.Codigo });
                        break;

                    case TipoCuenta.PasivoNoCorriente:
                        balance.PasivosNoCorrientes.Add(new LineaCuentaMonto { Nombre = cuenta.Nombre, Monto = -cuentaMayor.Saldo, Codigo = cuentaMayor.Codigo });
                        break;

                    case TipoCuenta.Patrimonio:
                        balance.Patrimonio.Add(new LineaCuentaMonto { Nombre = cuenta.Nombre, Monto = -cuentaMayor.Saldo, Codigo = cuentaMayor.Codigo });
                        break;
                }
            }

            if (estadoResultado.Participacion15 > 0)
            {
                balance.PasivosCorrientes.Add(new LineaCuentaMonto
                {
                    Nombre = "15% Participación Trabajadores por Pagar",
                    Monto = estadoResultado.Participacion15
                });
            }

            if (estadoResultado.ImpuestoRenta25 > 0)
            {
                balance.PasivosCorrientes.Add(new LineaCuentaMonto
                {
                    Nombre = "25% Impuesto a la Renta por Pagar",
                    Monto = estadoResultado.ImpuestoRenta25
                });
            }

            balance.Patrimonio.Add(new LineaCuentaMonto
            {
                Nombre = estadoResultado.UtilidadEjercicio >= 0 ? "Utilidad del Ejercicio" : "Pérdida del Ejercicio",
                Monto = estadoResultado.UtilidadEjercicio
            });

            balance.TotalActivoCorriente = balance.ActivosCorrientes.Sum(l => l.Monto);
            balance.TotalActivoNoCorriente = balance.ActivosNoCorrientes.Sum(l => l.Monto);
            balance.TotalActivo = balance.TotalActivoCorriente + balance.TotalActivoNoCorriente;

            balance.TotalPasivoCorriente = balance.PasivosCorrientes.Sum(l => l.Monto);
            balance.TotalPasivoNoCorriente = balance.PasivosNoCorrientes.Sum(l => l.Monto);
            balance.TotalPasivo = balance.TotalPasivoCorriente + balance.TotalPasivoNoCorriente;

            balance.TotalPatrimonio = balance.Patrimonio.Sum(l => l.Monto);

            balance.TotalPasivoMasPatrimonio = balance.TotalPasivo + balance.TotalPatrimonio;

            return balance;
        }
    }
}
