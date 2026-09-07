# Contabilidad

Aplicación de escritorio en **C# (WinForms / .NET Framework 4.8.1)** para llevar la contabilidad completa de una empresa: desde el registro de asientos en el Libro Diario hasta el Balance General, con todos los reportes intermedios calculados automáticamente.

Es un proyecto académico (materia de Contabilidad), pensado para reproducir el flujo de trabajo contable manual que se enseña en clase, pero automatizando todos los cálculos derivados para evitar errores de arrastre.

## ¿Qué hace?

La app se organiza en pestañas, cada una correspondiente a un libro o reporte contable:

| Pestaña | Descripción |
|---|---|
| **Libro Diario** | Registro de asientos contables (Cuenta, Debe, Haber, Fecha, Glosa). No deja guardar un asiento si Debe ≠ Haber. |
| **Libro Ajustes** | Asientos de ajuste (depreciación, amortización, etc.), numerados como A1, A2, A3... Igual validación que el Libro Diario. |
| **Libro Mayor** | Cuentas T de cada cuenta con movimiento, con su saldo deudor/acreedor. Se recalcula solo cada vez que cambia el Libro Diario o el de Ajustes. Se muestra en dos columnas y es responsivo al tamaño de la ventana. |
| **Balance de Comprobación** | Suma de Debe/Haber y saldos por cuenta, con fila de totales. Automático. |
| **Estado de Resultados** | Ingresos − Gastos → utilidad, aplicando la cascada tributaria ecuatoriana (15% participación trabajadores, 25% impuesto a la renta) hasta la Utilidad del Ejercicio. Automático. |
| **Balance General** | Activos, Pasivos y Patrimonio agrupados, incluyendo el pasivo por impuestos calculado y la utilidad del ejercicio como patrimonio, con verificación de la ecuación contable (Activo = Pasivo + Patrimonio). Automático. |
| **Configuración** | Catálogo de cuentas (código, nombre, tipo, naturaleza), precargado con ~90 cuentas típicas repartidas en los 7 tipos: Activo Corriente, Activo No Corriente, Pasivo Corriente, Pasivo No Corriente, Patrimonio, Ingresos y Gastos. |

### Respaldo y exportación

Desde la pestaña Configuración:

- **Guardar estado**: exporta cuentas + asientos + ajustes a un único archivo `.json`, para tener un respaldo o compartir un caso de estudio completo.
- **Cargar estado**: restaura todo desde un archivo guardado con lo anterior (reemplaza los datos actuales, con confirmación).
- **Eliminar estado**: borra todos los asientos y ajustes para empezar de cero, **sin tocar el catálogo de cuentas**.
- **Exportar a Excel**: genera un `.xlsx` con una hoja por cada pestaña, con formato (bordes, colores, encabezados) y **fórmulas reales de Excel** — no números pegados. Los totales de cada hoja son `SUM`/`IF` calculados ahí mismo, y varias hojas se referencian entre sí (Balance de Comprobación y los reportes financieros toman sus cifras directo del Libro Mayor), de modo que cualquier número se puede rastrear haciendo clic hasta llegar al asiento de origen en Libro Diario o Libro Ajustes.

## Tecnología

- **C# / WinForms**, apuntando a **.NET Framework 4.8.1**.
- **Sin dependencias externas ni paquetes NuGet**: la persistencia usa `JavaScriptSerializer` (incluido en el Framework) para leer/escribir JSON, y la exportación a Excel arma el archivo `.xlsx` a mano (un `.xlsx` es en realidad un `.zip` con XML adentro) usando `System.IO.Compression`, también incluido en el Framework.
- Los datos se guardan como archivos JSON en `bin/Debug/Data/` (`cuentas.json`, `asientos.json`, `ajustes.json`), separados del catálogo semilla (`cuentas_catalogo.json`) para no perder cuentas creadas por el usuario.

## Estructura del proyecto

```
Contabilidad/
├── Contabilidad.slnx
└── Contabilidad/
    ├── frmPrincipal.cs / .Designer.cs      # Ventana principal (todas las pestañas)
    ├── frmAgregarAsientoLibroDiario.cs      # Alta/edición de asientos del Libro Diario
    ├── frmAgregarAjusteLibroDiario.cs       # Alta/edición de ajustes
    ├── frmAgregarCuenta.cs                  # Alta/edición de cuentas del catálogo
    ├── Models/                              # Cuenta, AsientoContable, Ajuste, CuentaMayor,
    │                                         # EstadoResultado, BalanceGeneral, etc.
    ├── Data/                                # Repositorios (JSON), servicios de cálculo
    │                                         # (Libro Mayor, Estado de Resultados, Balance
    │                                         # General), guardado/carga de estado y el
    │                                         # exportador a Excel
    ├── UI/                                  # DataGridView personalizados con el estilo
    │                                         # visual de la app (verde/blanco), helpers de
    │                                         # iconos y estilos
    └── Icons/                               # Iconos de botones y pestañas (.ico)
```

## Cómo ejecutarlo

1. Abrir `Contabilidad.slnx` con Visual Studio (2022 o superior recomendado).
2. Compilar (`Ctrl+Shift+B`) — no requiere restaurar ningún paquete NuGet.
3. Ejecutar (`F5`). La primera vez se crea automáticamente la carpeta `Data` con el catálogo de cuentas precargado.

## Notas

- Los cálculos de Estado de Resultados y Balance General siguen la normativa ecuatoriana: 15% de participación a trabajadores sobre la utilidad (solo si es positiva), y 25% de impuesto a la renta sobre lo que queda después de esa participación.
- Como todavía no existe un asiento de cierre contable dentro de la app, el pasivo por impuestos y la utilidad del ejercicio se calculan e insertan automáticamente en el Balance General a partir del Estado de Resultados, en vez de venir de un asiento real en el Libro Diario.
