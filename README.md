# Contabilidad

Aplicación de escritorio en **C# (WinForms / .NET Framework 4.8.1)** para llevar la contabilidad completa de una empresa: desde el registro de asientos en el Libro Diario hasta el Balance General, con todos los reportes intermedios calculados automáticamente y un Dashboard con gráficos para ver el estado general de un vistazo.

Es un proyecto académico (materia de Contabilidad), pensado para reproducir el flujo de trabajo contable manual que se enseña en clase, pero automatizando todos los cálculos derivados para evitar errores de arrastre.

## ¿Qué hace?

La app se organiza en pestañas, cada una correspondiente a un libro o reporte contable:

| Pestaña | Descripción |
|---|---|
| **DashBoard** | Pantalla de inicio con gráficos: Balance (Activos/Pasivos/Patrimonio), Rendimiento Operativo (Ingresos/Gastos y la cascada tributaria hasta la Utilidad del Ejercicio), verificación de la ecuación contable, cantidad de cuentas del catálogo, una línea de tiempo del ciclo contable (Diario → Ajustes → Mayor → Balances → Resultados, clickeable para saltar a cada pestaña) y la tendencia mensual de Ingresos/Gastos. También reúne los accesos a Guardar/Cargar/Eliminar estado y Exportar a Excel. |
| **Libro Diario** | Registro de asientos contables (Cuenta, Debe, Haber, Fecha, Glosa). No deja guardar un asiento si Debe ≠ Haber. |
| **Libro Ajustes** | Asientos de ajuste (depreciación, amortización, etc.), numerados como A1, A2, A3... Igual validación que el Libro Diario. |
| **Libro Mayor** / **Libro Mayor Ajustado** | Cuentas T de cada cuenta con movimiento, con su saldo deudor/acreedor. La versión "sin ajustar" usa solo el Libro Diario; la "Ajustado" combina Libro Diario + Libro de Ajustes. Se recalculan solas cada vez que cambia alguno de los dos libros. Se muestran en dos columnas y son responsivas al tamaño de la ventana. |
| **Balance de Comprobación** / **Balance de Comprobación Ajustado** | Suma de Debe/Haber y saldos por cuenta, con fila de totales. Mismo criterio sin ajustar/ajustado que el Libro Mayor. Automático. |
| **Estado de Resultados** | Ingresos − Gastos → utilidad, aplicando la cascada tributaria ecuatoriana (15% participación trabajadores, 25% impuesto a la renta) hasta la Utilidad del Ejercicio. Automático. |
| **Balance General** | Activos, Pasivos y Patrimonio agrupados, incluyendo el pasivo por impuestos calculado y la utilidad del ejercicio como patrimonio, con verificación de la ecuación contable (Activo = Pasivo + Patrimonio). Automático. |
| **Configuración** | Catálogo de cuentas (código, nombre, tipo, naturaleza), precargado con ~90 cuentas típicas repartidas en los 7 tipos: Activo Corriente, Activo No Corriente, Pasivo Corriente, Pasivo No Corriente, Patrimonio, Ingresos y Gastos. Tiene un buscador por código o nombre arriba de la grilla. No deja eliminar una cuenta del catálogo original ni una cuenta que ya esté en uso en algún asiento o ajuste (avisa con un mensaje en cada caso). Al editar el código o nombre de una cuenta, el cambio se propaga automáticamente a todos los asientos y ajustes ya guardados que la usaban. |

### Respaldo y exportación

Desde la pestaña Configuración o desde la tarjeta "Herramientas de Exportación y Respaldo" del Dashboard:

- **Guardar estado**: exporta cuentas + asientos + ajustes a un único archivo `.json`, para tener un respaldo o compartir un caso de estudio completo.
- **Cargar estado**: restaura todo desde un archivo guardado con lo anterior (reemplaza los datos actuales, con confirmación).
- **Eliminar estado**: borra todos los asientos y ajustes para empezar de cero, **sin tocar el catálogo de cuentas**.
- **Exportar a Excel**: genera un `.xlsx` con una hoja por cada pestaña, con formato (bordes, colores, encabezados) y **fórmulas reales de Excel** — no números pegados. Los totales de cada hoja son `SUM`/`IF` calculados ahí mismo, y varias hojas se referencian entre sí (Balance de Comprobación y los reportes financieros toman sus cifras directo del Libro Mayor), de modo que cualquier número se puede rastrear haciendo clic hasta llegar al asiento de origen en Libro Diario o Libro Ajustes.

## Tecnología

- **C# / WinForms**, apuntando a **.NET Framework 4.8.1**.
- **Sin dependencias externas ni paquetes NuGet**: la persistencia usa `JavaScriptSerializer` (incluido en el Framework) para leer/escribir JSON, la exportación a Excel arma el archivo `.xlsx` a mano (un `.xlsx` es en realidad un `.zip` con XML adentro) usando `System.IO.Compression`, y los gráficos del Dashboard usan `System.Windows.Forms.DataVisualization.Charting` — los tres vienen incluidos en el Framework, sin instalar nada aparte.
- Los datos editables (`cuentas.json`, `asientos.json`, `ajustes.json`) se guardan en `%AppData%\Roaming\Contabilidad`, **no** junto al ejecutable — así funcionan bien tanto ejecutando desde Visual Studio como instalada en `Program Files`, donde esa carpeta es de solo lectura para un usuario normal (ver `Contabilidad/Data/RutasApp.cs`). El catálogo semilla (`cuentas_catalogo.json`) sí viaja junto al `.exe`, de solo lectura.

## Estructura del proyecto

```
Contabilidad.slnx
Contabilidad/
├── frmPrincipal.cs / .Designer.cs      # Ventana principal (todas las pestañas y el Dashboard)
├── frmAgregarAsientoLibroDiario.cs      # Alta/edición de asientos del Libro Diario
├── frmAgregarAjusteLibroDiario.cs       # Alta/edición de ajustes
├── frmAgregarCuenta.cs                  # Alta/edición de cuentas del catálogo
├── Models/                              # Cuenta, AsientoContable, Ajuste, CuentaMayor,
│                                         # EstadoResultado, BalanceGeneral, PuntoEvolucionMensual, etc.
├── Data/                                # Repositorios (JSON), servicios de cálculo
│                                         # (Libro Mayor, Estado de Resultados, Balance
│                                         # General, Evolución Mensual), guardado/carga de
│                                         # estado, el exportador a Excel y RutasApp (rutas
│                                         # de datos del usuario vs. catálogo semilla)
├── UI/                                  # DataGridView personalizados con el estilo
│                                         # visual de la app (verde/blanco), tarjetas y
│                                         # controles del Dashboard, helpers de iconos y estilos
└── Icons/                               # Iconos de botones, pestañas e ícono de la app (.ico)

Instalador-InnoSetup/
├── Contabilidad.iss                     # Script de Inno Setup
└── generar-instalador.bat               # Recompila la app en Release y genera el instalador
```

## Cómo ejecutarlo

1. Abrir `Contabilidad.slnx` con Visual Studio (2022 o superior recomendado).
2. Compilar (`Ctrl+Shift+B`) — no requiere restaurar ningún paquete NuGet.
3. Ejecutar (`F5`). La primera vez se crea automáticamente la carpeta de datos en `%AppData%\Roaming\Contabilidad` con el catálogo de cuentas precargado.

## Instalador

El proyecto incluye un instalador armado con [Inno Setup](https://jrsoftware.org/isinfo.php) (gratuito, sin dependencias de pago). No requiere permisos de administrador: se instala en la carpeta del usuario actual (`%LocalAppData%\Programs\Contabilidad`), con acceso directo en el Menú Inicio (y opcional en el Escritorio) y su propio desinstalador.

Para generar un instalador nuevo después de modificar el código:

1. Tener [Inno Setup 6](https://jrsoftware.org/isinfo.php) instalado (una sola vez).
2. Hacer doble clic en `Instalador-InnoSetup/generar-instalador.bat`. Ese script recompila `Contabilidad` en modo Release y arma el `.exe` del instalador con esos binarios — no hace falta correr nada por separado.
3. El instalador queda en `Instalador-InnoSetup/Output/Contabilidad-Setup.exe`.

## Notas

- Los cálculos de Estado de Resultados y Balance General siguen la normativa ecuatoriana: 15% de participación a trabajadores sobre la utilidad (solo si es positiva), y 25% de impuesto a la renta sobre lo que queda después de esa participación.
- Como todavía no existe un asiento de cierre contable dentro de la app, el pasivo por impuestos y la utilidad del ejercicio se calculan e insertan automáticamente en el Balance General a partir del Estado de Resultados, en vez de venir de un asiento real en el Libro Diario.
- El tamaño mínimo de la ventana es 1400×700: por debajo de eso las tarjetas del Dashboard no entran bien.
