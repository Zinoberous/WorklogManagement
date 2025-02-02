# Temporäre Erlaubnis für die Skriptausführung
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force

$dockerDesktopPath = "C:\Program Files\Docker\Docker\Docker Desktop.exe"
$containerName = "worklogmanagement-sqlserver"

# Prüfen, ob Docker läuft
function Test-DockerRunning {
    try {
        $output = docker info 2>&1  # Fängt Standard- & Fehlerausgabe ab
        if ($output -match "Server Version:") { return $true }  # Docker ist aktiv
    } catch {
        return $false
    }
    return $false
}

if (-not (Test-DockerRunning)) {
    Write-Host "Docker Desktop wird gestartet..." -ForegroundColor Yellow
    Start-Process -FilePath $dockerDesktopPath -NoNewWindow

    # Warten, bis Docker gestartet ist
    $attempts = 0
    while (-not (Test-DockerRunning)) {
        Start-Sleep -Seconds 2
        $attempts++
        if ($attempts -ge 15) {
            Write-Host "Docker konnte nicht innerhalb von 30 Sekunden gestartet werden. Bitte manuell starten." -ForegroundColor Red
            exit 1
        }
    }
    Write-Host "Docker wurde erfolgreich gestartet." -ForegroundColor Green
}

# Prüfen, ob der Container existiert
$existingContainer = docker ps -a --format "{{.Names}}" | Select-String -Quiet $containerName

if ($existingContainer) {
    # Prüfen, ob der Container läuft
    $isRunning = docker ps --format "{{.Names}}" | Select-String -Quiet $containerName
    if ($isRunning) {
        Write-Host "Container '$containerName' ist bereits gestartet." -ForegroundColor Green
    } else {
        Write-Host "Container '$containerName' wird gestartet..." -ForegroundColor Yellow
        docker start $containerName *> $null
        Write-Host "Container '$containerName' wurde erfolgreich gestartet." -ForegroundColor Green
    }
} else {
    Write-Host "Container '$containerName' existiert nicht, starte docker-compose..." -ForegroundColor Green
    docker-compose up -d *> $null
    Write-Host "Container '$containerName' wurde erfolgreich gestartet." -ForegroundColor Green
}
