#Requires -Version 5.1

# ============================================================
# Script de Mantenimiento de Archivos Gresst
# Versión PowerShell Pura
# ============================================================
# Descripción:
#   - Mueve archivos antiguos (>3 años) de ACTIVOS e HISTORICOS a DESTINO
#   - Elimina archivos antiguos (>1 mes) en QA
#   - Limpia carpetas vacías resultantes
#   - Excluye carpetas "Configuracion"
# ============================================================

# Configuración
$ErrorActionPreference = "Stop"
$script:ACTIVOS = "D:\Sitios.IIS\Aplications.Sites\Gresst\archivos.gresst.com\Cuentas"
$script:QA = "D:\Sitios.IIS\Aplications.Sites.qa\Gresst\archivos\Cuentas"
$script:HISTORICOS = "D:\Sitios.IIS\Aplications.Sites\GresstHistorico\archivos.gresst.com\Cuentas"
$script:DESTINO = "D:\Backups\GresstFiles"

# Configurar log en la misma carpeta del script, con fecha en el nombre
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$logDate = Get-Date -Format "yyyy-MM-dd"
$script:LOG_FILE = Join-Path $scriptPath "gresst-backup-$logDate.log"

# ============================================================
# FUNCIONES
# ============================================================

function Write-Log {
    param([string]$Message)
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss.ff"
    $logMessage = "[$timestamp] $Message"
    Add-Content -Path $script:LOG_FILE -Value $logMessage
    Write-Host $logMessage
}

function Test-PathAndLog {
    param([string]$Path, [string]$Name)
    if (-not (Test-Path $Path)) {
        Write-Log "[ERROR] Ruta no encontrada ($Name): $Path"
        return $false
    }
    Write-Log "[OK] Ruta valida ($Name): $Path"
    return $true
}

function Remove-EmptyFolders {
    param([string]$RootPath)
    Write-Log "[CLEANUP] Eliminando carpetas vacias en: $RootPath"
    $emptyFolders = 0
    try {
        # Deshabilitar confirmaciones temporalmente
        $ConfirmPreference = $global:ConfirmPreference
        $global:ConfirmPreference = 'None'
        
        Get-ChildItem -Path $RootPath -Recurse -Directory -ErrorAction SilentlyContinue |
            Where-Object { $_.FullName -notmatch '\\Configuracion(\\|$)' } |
            Sort-Object -Property FullName -Descending |
            ForEach-Object {
                $folder = $_
                # Verificar si la carpeta está realmente vacía (sin archivos ni subcarpetas con contenido)
                $allItems = Get-ChildItem -Path $folder.FullName -Recurse -Force -ErrorAction SilentlyContinue |
                    Where-Object { -not $_.PSIsContainer }
                if (-not $allItems) {
                    # La carpeta está vacía, eliminar recursivamente
                    Remove-Item -Path $folder.FullName -Recurse -Force -ErrorAction SilentlyContinue
                    $emptyFolders++
                }
            }
        
        # Restaurar preferencia de confirmación
        $global:ConfirmPreference = $ConfirmPreference
        
        Write-Log "[RESULT] Carpetas vacias eliminadas: $emptyFolders"
    } catch {
        Write-Log "[WARN] Error al limpiar carpetas vacias: $($_.Exception.Message)"
    }
}

function Move-OldFiles {
    param(
        [string]$SourcePath,
        [string]$DestinationPath,
        [int]$YearsOld = 3
    )
    
    $moved = 0
    $deleted = 0
    $errors = 0
    
    try {
        Write-Log "[PROCESS] Procesando: $SourcePath"
        
        Get-ChildItem -Path $SourcePath -Directory -ErrorAction SilentlyContinue |
            Where-Object { $_.Name -match '^[0-9]' } |
            ForEach-Object {
                $folder = $_
                Get-ChildItem -Path $folder.FullName -Recurse -File -ErrorAction SilentlyContinue |
                    Where-Object {
                        $_.FullName -notmatch '\\Configuracion(\\|$)' -and
                        $_.CreationTime -lt (Get-Date).AddYears(-$YearsOld)
                    } |
                    ForEach-Object {
                        try {
                            $file = $_
                            $relativePath = $file.FullName.Substring($SourcePath.Length).TrimStart('\')
                            $targetPath = Join-Path $DestinationPath $relativePath
                            $targetDir = Split-Path $targetPath -Parent
                            
                            # Crear directorio destino si no existe
                            if (-not (Test-Path $targetDir)) {
                                New-Item -ItemType Directory -Path $targetDir -Force | Out-Null
                            }
                            
                            # Si existe en destino, eliminar del origen; si no, mover
                            if (Test-Path $targetPath) {
                                Remove-Item -Path $file.FullName -Force
                                Write-Log "[REMOVED] Ya existia en destino: $($file.FullName)"
                                $deleted++
                            } else {
                                Move-Item -Path $file.FullName -Destination $targetPath -Force
                                Write-Log "[MOVED] $($file.FullName) -> $targetPath"
                                $moved++
                            }
                        } catch {
                            Write-Log "[ERROR] No se pudo procesar: $($_.FullName) - $($_.Exception.Message)"
                            $errors++
                        }
                    }
            }
        
        Write-Log "[RESULT] Movidos: $moved, Eliminados (duplicados): $deleted, Errores: $errors"
        
        # Limpiar carpetas vacías
        Remove-EmptyFolders -RootPath $SourcePath
        
        return $errors -eq 0
    } catch {
        Write-Log "[ERROR CRITICO] $($_.Exception.Message)"
        return $false
    }
}

function Remove-OldFiles {
    param(
        [string]$SourcePath,
        [int]$MonthsOld = 1
    )
    
    $deleted = 0
    $errors = 0
    
    try {
        Write-Log "[PROCESS] Procesando: $SourcePath"
        
        Get-ChildItem -Path $SourcePath -Directory -ErrorAction SilentlyContinue |
            Where-Object { $_.Name -match '^[0-9]' } |
            ForEach-Object {
                $folder = $_
                Get-ChildItem -Path $folder.FullName -Recurse -File -ErrorAction SilentlyContinue |
                    Where-Object {
                        $_.FullName -notmatch '\\Configuracion(\\|$)' -and
                        $_.CreationTime -lt (Get-Date).AddMonths(-$MonthsOld)
                    } |
                    ForEach-Object {
                        try {
                            Remove-Item -Path $_.FullName -Force
                            Write-Log "[DELETED] $($_.FullName)"
                            $deleted++
                        } catch {
                            Write-Log "[ERROR] No se pudo eliminar: $($_.FullName) - $($_.Exception.Message)"
                            $errors++
                        }
                    }
            }
        
        Write-Log "[RESULT] Eliminados: $deleted, Errores: $errors"
        
        # Limpiar carpetas vacías
        Remove-EmptyFolders -RootPath $SourcePath
        
        return $errors -eq 0
    } catch {
        Write-Log "[ERROR CRITICO] $($_.Exception.Message)"
        return $false
    }
}

# ============================================================
# MAIN
# ============================================================

try {
    Write-Log "============================================================"
    Write-Log "INICIO MANTENIMIENTO GRESST"
    Write-Log "============================================================"
    
    # Verificar rutas
    $allPathsValid = $true
    $allPathsValid = (Test-PathAndLog -Path $script:ACTIVOS -Name "ACTIVOS") -and $allPathsValid
    $allPathsValid = (Test-PathAndLog -Path $script:QA -Name "QA") -and $allPathsValid
    $allPathsValid = (Test-PathAndLog -Path $script:HISTORICOS -Name "HISTORICOS") -and $allPathsValid
    
    if (-not $allPathsValid) {
        throw "Una o mas rutas no son validas"
    }
    
    # Crear directorio destino si no existe
    if (-not (Test-Path $script:DESTINO)) {
        Write-Log "[INFO] Creando DESTINO: $script:DESTINO"
        New-Item -ItemType Directory -Path $script:DESTINO -Force | Out-Null
    }
    
    if (-not (Test-PathAndLog -Path $script:DESTINO -Name "DESTINO")) {
        throw "No se pudo crear/verificar DESTINO"
    }
    
    # Procesar ACTIVOS
    Write-Log "============================================================"
    Write-Log "[MOVE] ACTIVOS (>3 años)"
    Write-Log "============================================================"
    $ErrorActionPreference = "Continue"
    $activosOk = Move-OldFiles -SourcePath $script:ACTIVOS -DestinationPath $script:DESTINO -YearsOld 3
    if (-not $activosOk) {
        Write-Log "[WARN] Hubo errores al procesar ACTIVOS"
    }
    
    # Procesar HISTORICOS
    Write-Log "============================================================"
    Write-Log "[MOVE] HISTORICOS (>3 años)"
    Write-Log "============================================================"
    $historicosOk = Move-OldFiles -SourcePath $script:HISTORICOS -DestinationPath $script:DESTINO -YearsOld 3
    if (-not $historicosOk) {
        Write-Log "[WARN] Hubo errores al procesar HISTORICOS"
    }
    
    # Procesar QA
    Write-Log "============================================================"
    Write-Log "[CLEAN] QA (>1 mes)"
    Write-Log "============================================================"
    $qaOk = Remove-OldFiles -SourcePath $script:QA -MonthsOld 1
    if (-not $qaOk) {
        Write-Log "[WARN] Hubo errores al procesar QA"
    }
    
    # Finalización
    Write-Log "============================================================"
    Write-Log "PROCESO FINALIZADO CORRECTAMENTE"
    Write-Log "DESTINO: $script:DESTINO"
    Write-Log "LOG: $script:LOG_FILE"
    Write-Log "============================================================"
    
    Write-Host "`nPresione cualquier tecla para cerrar..."
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    exit 0
    
} catch {
    Write-Log "[FATAL] Proceso abortado por error: $($_.Exception.Message)"
    Write-Host "`nPresione cualquier tecla para cerrar..."
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    exit 1
}
