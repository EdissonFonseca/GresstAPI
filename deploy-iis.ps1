# Script de despliegue para IIS en Windows Server
# Ejecutar como Administrador en el servidor Windows

param(
    [string]$DeployPath = "C:\inetpub\wwwroot\GresstAPI",
    [string]$SiteName = "GresstAPI",
    [string]$AppPoolName = "GresstAPI",
    [int]$Port = 80,
    [string]$ConnectionString = ""
)

Write-Host "🚀 Desplegando Gresst API en IIS..." -ForegroundColor Green
Write-Host ""

# Verificar que se ejecuta como administrador
if (-NOT ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")) {
    Write-Host "❌ Este script debe ejecutarse como Administrador" -ForegroundColor Red
    exit 1
}

# Importar módulo de IIS
Import-Module WebAdministration

# 1. Crear Application Pool
Write-Host "📦 Creando Application Pool..."
if (Test-Path "IIS:\AppPools\$AppPoolName") {
    Remove-WebAppPool -Name $AppPoolName
}

New-WebAppPool -Name $AppPoolName
Set-ItemProperty "IIS:\AppPools\$AppPoolName" -Name managedRuntimeVersion -Value ""
Set-ItemProperty "IIS:\AppPools\$AppPoolName" -Name startMode -Value "AlwaysRunning"

# 2. Crear directorio de despliegue
Write-Host "📁 Creando directorio de despliegue..."
if (-not (Test-Path $DeployPath)) {
    New-Item -ItemType Directory -Force -Path $DeployPath | Out-Null
}

# Crear carpeta de logs
$LogPath = Join-Path $DeployPath "logs"
if (-not (Test-Path $LogPath)) {
    New-Item -ItemType Directory -Force -Path $LogPath | Out-Null
}

# 3. Dar permisos
Write-Host "🔐 Configurando permisos..."
icacls $DeployPath /grant "IIS AppPool\$($AppPoolName):(OI)(CI)F" /T | Out-Null
icacls $LogPath /grant "IIS AppPool\$($AppPoolName):(OI)(CI)F" /T | Out-Null

# 4. Crear o actualizar sitio
Write-Host "🌐 Configurando sitio IIS..."
if (Test-Path "IIS:\Sites\$SiteName") {
    Remove-WebSite -Name $SiteName
}

$Site = New-WebSite -Name $SiteName `
                    -PhysicalPath $DeployPath `
                    -ApplicationPool $AppPoolName `
                    -Port $Port

# 5. Configurar variables de entorno (si se proporciona connection string)
if ($ConnectionString -ne "") {
    Write-Host "🔗 Configurando Connection String..."
    [System.Environment]::SetEnvironmentVariable(
        "ConnectionStrings__DefaultConnection",
        $ConnectionString,
        "Machine"
    )
}

# 6. Reiniciar IIS
Write-Host "🔄 Reiniciando IIS..."
iisreset | Out-Null

Write-Host ""
Write-Host "✅ ¡Despliegue completado exitosamente!" -ForegroundColor Green
Write-Host ""
Write-Host "📊 Información del despliegue:" -ForegroundColor Cyan
Write-Host "   Sitio: $SiteName"
Write-Host "   Path: $DeployPath"
Write-Host "   Puerto: $Port"
Write-Host "   URL: http://localhost:$Port"
Write-Host ""
Write-Host "📚 Swagger UI disponible en:" -ForegroundColor Cyan
Write-Host "   http://localhost:$Port/"
Write-Host ""
Write-Host "🔍 Logs en: $LogPath" -ForegroundColor Yellow
Write-Host ""
Write-Host "⚠️  IMPORTANTE: Copia los archivos de publish/ a $DeployPath" -ForegroundColor Yellow
Write-Host ""

# Abrir navegador
Start-Process "http://localhost:$Port"

