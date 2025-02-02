## Worklog Management - Docker Setup

### 1. **Passwort setzen**
Erstelle eine `.env`-Datei und setze das Passwort für den SQL Server:
```
SA_PASSWORD=DeinSicheresPasswort123!
```

### 2. **Docker starten und Container hochfahren**

#### **Automatische Variante (empfohlen)**
Falls Docker noch nicht läuft oder der Container gestoppt ist, starte das folgende PowerShell-Skript:
```powershell
.\start-docker-compose.ps1
```
Dieses Skript:
- Startet Docker Desktop, falls es nicht läuft
- Prüft, ob der Container bereits existiert und startet ihn
- Falls kein Container existiert, wird `docker-compose up -d` ausgeführt
- Setzt temporär die PowerShell-Ausführungsrichtlinie, falls nötig

#### **Manuelle Variante**
Falls du Docker manuell starten möchtest, kannst du folgende Befehle nutzen:

1. **Docker-Dienst starten** (falls nicht bereits aktiv):
   ```powershell
   Start-Process "C:\Program Files\Docker\Docker\Docker Desktop.exe"
   ```

2. **Container starten, falls bereits vorhanden:**
   ```sh
   docker start worklogmanagement-sqlserver
   ```

3. **Falls kein Container existiert, `docker-compose` starten:**
   ```sh
   docker-compose up -d
   ```

### 3. **Container stoppen oder entfernen**
- **Container nur stoppen (aber nicht löschen):**
  ```sh
  docker stop worklogmanagement-sqlserver
  ```

- **Container entfernen (Daten bleiben erhalten, aber Container wird gelöscht):**
  ```sh
  docker rm worklogmanagement-sqlserver
  ```

- **Kompletten Docker-Compose-Stack stoppen und alle zugehörigen Container entfernen:**
  ```sh
  docker-compose down
  ```

- **Kompletten Docker-Compose-Stack inklusive Volumes (Datenverlust!):**
  ```sh
  docker-compose down -v
  ```

### 4. **Daten zurücksetzen**
Falls du den gesamten Datenbank-Container inklusive aller gespeicherten Daten entfernen möchtest:
```sh
docker volume rm sqlserver_data
```
**Achtung:** Dies löscht alle gespeicherten Daten endgültig!

### 5. **Container-Status prüfen**

- **Laufende Container anzeigen:**
  ```sh
  docker ps
  ```
- **Alle Container (inkl. gestoppte) anzeigen:**
  ```sh
  docker ps -a
  ```

---
Mit diesem Setup bleibt deine SQL Server Instanz auch nach Neustarts erhalten und du kannst sie einfach mit `start-docker-compose.ps1` wieder starten.

