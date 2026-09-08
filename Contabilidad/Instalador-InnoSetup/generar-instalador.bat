@echo off
REM Reconstruye Contabilidad en modo Release y arma el instalador (Contabilidad-Setup.exe)
REM con esos binarios. Hacer doble clic en este archivo cada vez que quieras generar un
REM instalador nuevo con los ultimos cambios del codigo.

setlocal

echo ============================================
echo  Paso 1/2: Compilando Contabilidad (Release)
echo ============================================

set "VSWHERE=%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe"
if not exist "%VSWHERE%" (
    echo No se encontro vswhere.exe. Esta instalado Visual Studio?
    pause
    exit /b 1
)

for /f "usebackq tokens=*" %%i in (`"%VSWHERE%" -latest -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe`) do set "MSBUILD=%%i"

if not defined MSBUILD (
    echo No se encontro MSBuild.exe. Revisa tu instalacion de Visual Studio.
    pause
    exit /b 1
)

"%MSBUILD%" "%~dp0..\Contabilidad\Contabilidad.csproj" /t:Rebuild /p:Configuration=Release /nologo /v:minimal
if errorlevel 1 (
    echo.
    echo La compilacion fallo. Revisa los errores de arriba antes de continuar.
    pause
    exit /b 1
)

echo.
echo ============================================
echo  Paso 2/2: Generando el instalador
echo ============================================

set "ISCC=%LocalAppData%\Programs\Inno Setup 6\ISCC.exe"
if not exist "%ISCC%" (
    set "ISCC=%ProgramFiles(x86)%\Inno Setup 6\ISCC.exe"
)
if not exist "%ISCC%" (
    echo No se encontro ISCC.exe de Inno Setup. Esta instalado?
    echo Descargalo de https://jrsoftware.org/isinfo.php
    pause
    exit /b 1
)

"%ISCC%" "%~dp0Contabilidad.iss"
if errorlevel 1 (
    echo.
    echo Fallo la generacion del instalador. Revisa los errores de arriba.
    pause
    exit /b 1
)

echo.
echo ============================================
echo  Listo. El instalador nuevo esta en:
echo  %~dp0Output\Contabilidad-Setup.exe
echo ============================================
pause
