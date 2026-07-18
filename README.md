# Haushalts OS

![CI](https://github.com/oggyB-Dev/haushalt-OS/actions/workflows/ci.yml/badge.svg)

> Selbst gehostete Haushalts Plattform für zwei Nutzer: Einkaufsliste mit
> Echtzeit Sync, Dokumentenverwaltung mit lokaler KI (OCR, Fristen-Erkennung),
> Haushaltsbuch mit Kassenbon-Scan, gemeinsamer Kalender mit Push-Erinnerungen
> und Essensplaner. Die KI läuft komplett lokal keine Daten verlassen den
> eigenen Server.

**Stack:** .NET 10 · Angular 22 (PWA, Signals, zoneless) · PostgreSQL + pgvector · Redis · SignalR · Ollama (Qwen, bge-m3) · Docker

In aktiver Entwicklung — aktueller Stand: v0 (Fundament)

## Screenshots

*(folgt mit v1)*

## Architektur
- Modularer Monolith mit Vertical Slice Architecture
- PostgreSQL + pgvector statt separater Vektor-DB 
- KI niemals im Request Pfad
- Local first PWA

## Setup


## Roadmap

- [X] v0 — Fundament: Auth, Docker, CI, Deployment
- [ ] v1 — Einkaufsliste (Echtzeit + local-first)
- [ ] v2 — Dokumente (Upload, Kategorien, Access Control)
- [ ] v3 — KI: Dokument-Intelligenz + semantische Suche
- [ ] v4 — Kalender + Push-Erinnerungen
- [ ] v5 — Haushaltsbuch + Kassenbon-Scan
- [ ] v6 — Essensplaner