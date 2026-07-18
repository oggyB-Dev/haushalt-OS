# Runbook

## Stack starten / aktualisieren

# baut Images neu und startet alles
docker compose up -d --build 

# Prüfen ob alle Services funktionieren
docker compose ps 

## Zugriff 
- Lokal: http://localhost:3000 
- Tailnet Handys: https://oggydev.tail597143.ts.net
(Tailscale Serve um es neu einzurichten: sudo tailscale serve --bg --https=443 http://localhost:3000)

## Logs
docker compose logs api --tail 50
docker compose logs -f api 

## Secrets 
Alle Werte in .env (Vorlage .env.example)
Lokale Dev Secrets : dotnet user-secrets 

## Datenbank
Migrations laufen automatisch beim API Start 
Volume Reset (VERNICHTET DATEN) : docker compose down && docker compose volume rm haushalt-os_postgres_data