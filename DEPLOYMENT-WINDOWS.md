# 🖥️ Desplegar Gresst API en Windows Server

## 📋 **Requisitos del Servidor Windows**

1. ✅ Windows Server 2016 o superior
2. ✅ .NET 9 Runtime (ASP.NET Core)
3. ✅ IIS instalado
4. ✅ ASP.NET Core Hosting Bundle
5. ✅ Acceso a SQL Server (tu BD en AWS)

---

## 🎯 **OPCIÓN 1: IIS (Internet Information Services) - RECOMENDADO**

### **A. Preparar el Servidor Windows**

#### **1. Instalar .NET 9 Hosting Bundle**

En el servidor Windows, descarga e instala:

**👉 https://dotnet.microsoft.com/download/dotnet/9.0**

Busca: **"ASP.NET Core Runtime 9.0.x - Hosting Bundle"**

O descarga directo:
```powershell
# En PowerShell del servidor (como administrador)
Invoke-WebRequest -Uri "https://download.visualstudio.microsoft.com/download/pr/..." -OutFile "dotnet-hosting-9.0.0-win.exe"
.\dotnet-hosting-9.0.0-win.exe /quiet /install
```

Después de instalar, **reinicia IIS:**
```powershell
net stop was /y
net start w3svc
```

#### **2. Habilitar IIS (si no está instalado)**

```powershell
# En PowerShell como administrador
Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServerRole
Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServer
Enable-WindowsOptionalFeature -Online -FeatureName IIS-CommonHttpFeatures
Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpErrors
Enable-WindowsOptionalFeature -Online -FeatureName IIS-ApplicationDevelopment
Enable-WindowsOptionalFeature -Online -FeatureName IIS-NetFxExtensibility45
Enable-WindowsOptionalFeature -Online -FeatureName IIS-HealthAndDiagnostics
Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpLogging
Enable-WindowsOptionalFeature -Online -FeatureName IIS-Security
Enable-WindowsOptionalFeature -Online -FeatureName IIS-RequestFiltering
Enable-WindowsOptionalFeature -Online -FeatureName IIS-Performance
Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServerManagementTools
Enable-WindowsOptionalFeature -Online -FeatureName IIS-ManagementConsole
```

---

### **B. Transferir Archivos al Servidor**

#### **Opción 1: Desde GitHub (Recomendado)**

En el servidor Windows:

```powershell
# Instalar Git si no lo tienes
winget install Git.Git

# Clonar el repositorio
cd C:\inetpub\
git clone https://github.com/EdissonFonseca/GresstAPI.git

cd GresstAPI

# Publicar
dotnet publish src/Gresst.API/Gresst.API.csproj -c Release -o C:\inetpub\wwwroot\GresstAPI
```

#### **Opción 2: Copiar Manualmente**

Desde tu Mac, comprime la carpeta `publish`:

```bash
cd /Users/edissonfonseca/Development/Gresst/API
zip -r GresstAPI-deploy.zip publish/
```

Luego:
1. Transfiere `GresstAPI-deploy.zip` al servidor (RDP, FTP, etc.)
2. Descomprime en `C:\inetpub\wwwroot\GresstAPI`

---

### **C. Configurar IIS**

En el servidor Windows:

#### **1. Abrir IIS Manager**
```powershell
# Ejecutar como administrador
inetmgr
```

#### **2. Crear Application Pool**

- Click derecho en "Application Pools" → "Add Application Pool"
- Name: `GresstAPI`
- .NET CLR version: `No Managed Code`
- Managed pipeline mode: `Integrated`
- Click OK

#### **3. Configurar Application Pool**

- Click derecho en `GresstAPI` → "Advanced Settings"
- Identity: `ApplicationPoolIdentity` (o una cuenta específica)
- Start Mode: `AlwaysRunning`
- Click OK

#### **4. Crear el Sitio Web**

- Click derecho en "Sites" → "Add Website"
- Site name: `GresstAPI`
- Application pool: `GresstAPI`
- Physical path: `C:\inetpub\wwwroot\GresstAPI`
- Binding:
  - Type: `http`
  - IP: `All Unassigned`
  - Port: `80` (o el que prefieras, ej: 5000)
  - Host name: (vacío o tu dominio)
- Click OK

#### **5. Configurar el web.config**

Ya está incluido en la carpeta `publish`. El archivo está configurado para usar `inprocess` hosting.

---

### **D. Configurar Connection String**

Edita `C:\inetpub\wwwroot\GresstAPI\appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=ec2-18-224-46-73.us-east-2.compute.amazonaws.com;Database=QA.Gresst;User Id=Aranea;Password=$(2k20)$-Gr355t;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

**⚠️ IMPORTANTE:** En producción, usa variables de entorno o Azure Key Vault para passwords.

---

### **E. Dar Permisos**

En PowerShell como administrador:

```powershell
# Dar permisos al Application Pool Identity
icacls "C:\inetpub\wwwroot\GresstAPI" /grant "IIS AppPool\GresstAPI:(OI)(CI)F" /T

# Crear carpeta de logs
mkdir C:\inetpub\wwwroot\GresstAPI\logs
icacls "C:\inetpub\wwwroot\GresstAPI\logs" /grant "IIS AppPool\GresstAPI:(OI)(CI)F" /T
```

---

### **F. Probar**

```powershell
# Navega a:
http://TU_SERVIDOR
# o
http://TU_SERVIDOR:5000

# Swagger UI estará en:
http://TU_SERVIDOR/swagger
```

---

## 🔧 **OPCIÓN 2: Como Servicio de Windows**

### **Crear archivo de servicio**

```powershell
# En el servidor Windows, en PowerShell como administrador

# Navegar a la carpeta publicada
cd C:\GresstAPI

# Crear el servicio
sc.exe create GresstAPI binPath="C:\GresstAPI\Gresst.API.exe" start=auto

# Iniciar el servicio
sc.exe start GresstAPI

# Ver estado
sc.exe query GresstAPI
```

Para que funcione como servicio, necesitas modificar `Program.cs` para usar `UseWindowsService()`.

---

## 🚀 **OPCIÓN 3: Ejecutar Directamente (Solo Desarrollo/Pruebas)**

```powershell
# En el servidor Windows
cd C:\GresstAPI
dotnet Gresst.API.dll

# O con configuración específica
$env:ASPNETCORE_ENVIRONMENT="Production"
$env:ASPNETCORE_URLS="http://*:5000"
dotnet Gresst.API.dll
```

---

## 📦 **Script de Despliegue Automatizado**

```powershell
# deploy-windows.ps1
# Ejecutar en el servidor Windows como administrador

param(
    [string]$SourcePath = "C:\Temp\GresstAPI-deploy.zip",
    [string]$DeployPath = "C:\inetpub\wwwroot\GresstAPI",
    [string]$SiteName = "GresstAPI",
    [string]$AppPoolName = "GresstAPI",
    [int]$Port = 80
)

# 1. Detener el sitio si existe
Import-Module WebAdministration
if (Test-Path "IIS:\Sites\$SiteName") {
    Stop-WebSite -Name $SiteName
}

# 2. Limpiar carpeta de despliegue
if (Test-Path $DeployPath) {
    Remove-Item -Path "$DeployPath\*" -Recurse -Force
}
New-Item -ItemType Directory -Force -Path $DeployPath

# 3. Descomprimir archivos
Expand-Archive -Path $SourcePath -DestinationPath $DeployPath -Force

# 4. Crear Application Pool si no existe
if (-not (Test-Path "IIS:\AppPools\$AppPoolName")) {
    New-WebAppPool -Name $AppPoolName
    Set-ItemProperty "IIS:\AppPools\$AppPoolName" -Name managedRuntimeVersion -Value ""
}

# 5. Crear o actualizar sitio
if (Test-Path "IIS:\Sites\$SiteName") {
    Remove-WebSite -Name $SiteName
}

New-WebSite -Name $SiteName `
            -PhysicalPath $DeployPath `
            -ApplicationPool $AppPoolName `
            -Port $Port

# 6. Dar permisos
icacls $DeployPath /grant "IIS AppPool\$($AppPoolName):(OI)(CI)F" /T

# 7. Iniciar el sitio
Start-WebSite -Name $SiteName

Write-Host "✅ Despliegue completado!"
Write-Host "🌐 URL: http://localhost:$Port"
```

---

## 📋 **Pasos Resumidos**

### **1. En tu Mac (ya está hecho):**
```bash
cd /Users/edissonfonseca/Development/Gresst/API
# La carpeta publish/ ya está lista
```

### **2. Transferir al servidor:**

**Opción A - Comprimir y transferir:**
```bash
cd /Users/edissonfonseca/Development/Gresst/API
zip -r GresstAPI-deploy.zip publish/
```

Luego transfiere `GresstAPI-deploy.zip` al servidor usando:
- RDP (Remote Desktop) - copiar/pegar
- FTP/SFTP
- SCP: `scp GresstAPI-deploy.zip usuario@servidor:C:\Temp\`

**Opción B - Clonar desde GitHub (más fácil):**

En el servidor Windows:
```powershell
cd C:\inetpub
git clone https://github.com/EdissonFonseca/GresstAPI.git
cd GresstAPI
dotnet publish src/Gresst.API/Gresst.API.csproj -c Release -o C:\inetpub\wwwroot\GresstAPI
```

### **3. En el servidor Windows:**

```powershell
# Verificar .NET 9 instalado
dotnet --list-runtimes

# Si no está, instalar:
# https://dotnet.microsoft.com/download/dotnet/9.0

# Configurar IIS (ver instrucciones detalladas arriba)

# Probar que funciona
cd C:\inetpub\wwwroot\GresstAPI
dotnet Gresst.API.dll
```

---

## 🔐 **Configurar Connection String Seguro**

En Windows, usa variables de entorno:

```powershell
# Configurar variable de entorno a nivel de sistema
[System.Environment]::SetEnvironmentVariable(
    "ConnectionStrings__DefaultConnection",
    "Server=ec2-18-224-46-73.us-east-2.compute.amazonaws.com;Database=QA.Gresst;User Id=Aranea;Password=$(2k20)$-Gr355t;TrustServerCertificate=True",
    "Machine"
)

# Reiniciar IIS para que tome la variable
iisreset
```

Luego modifica `appsettings.json` para que NO tenga el password hardcodeado.

---

## 📦 **Archivos Listos para Transferir**

<function_calls>
<invoke name="run_terminal_cmd">
<parameter name="command">cd /Users/edissonfonseca/Development/Gresst/API && ls -lh publish/ | head -10 && echo "..." && echo "Total de archivos:" && ls -1 publish/ | wc -l
