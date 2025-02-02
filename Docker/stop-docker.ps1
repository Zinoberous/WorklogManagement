# Temporäre Erlaubnis für die Skriptausführung
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force

$containerName = "worklogmanagement-sqlserver"

# Prüfen, ob Docker läuft (damit keine unnötigen Fehler entstehen)
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
    Write-Host "Docker wird nicht ausgeführt. Bitte starte Docker Desktop zuerst." -ForegroundColor Red
    exit 1
}

# Prüfen, ob der Container existiert
$existingContainer = docker ps -a --format "{{.Names}}" | Select-String -Quiet $containerName

if (-not $existingContainer) {
    Write-Host "Container '$containerName' existiert nicht." -ForegroundColor Yellow
    exit 0
}

# Prüfen, ob der Container bereits gestoppt ist
$isRunning = docker ps --format "{{.Names}}" | Select-String -Quiet $containerName

if ($isRunning) {
    Write-Host "Container '$containerName' wird gestoppt..." -ForegroundColor Yellow
    docker stop $containerName *> $null
    Write-Host "Container '$containerName' wurde erfolgreich gestoppt." -ForegroundColor Green
} else {
    Write-Host "Container '$containerName' ist bereits gestoppt." -ForegroundColor Green
}
