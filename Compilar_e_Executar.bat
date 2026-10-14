@echo off
chcp 65001 > nul
echo =======================================================================
echo   Gestao de Equipamentos Recondicionados
echo =======================================================================
echo.

set MSBUILD_PATH=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe

if not exist "%MSBUILD_PATH%" (
    echo [ERRO] O MSBuild nao foi encontrado em: %MSBUILD_PATH%
    echo Por favor abra o projeto atraves do Visual Studio (GestaoInventarioRecondicionados.sln).
    pause
    exit /b 1
)

echo [1/2] A compilar o projeto em modo Debug...
"%MSBUILD_PATH%" "%~dp0GestaoInventarioRecondicionados.csproj" /p:Configuration=Debug /nologo

if %ERRORLEVEL% neq 0 (
    echo.
    echo [ERRO] A compilacao falhou. Verifique os erros acima.
    pause
    exit /b %ERRORLEVEL%
)

echo.
echo [2/2] Compilacao concluida com sucesso!
echo A iniciar a aplicacao...
start "" "%~dp0bin\Debug\GestaoInventarioRecondicionados.exe"
exit /b 0
