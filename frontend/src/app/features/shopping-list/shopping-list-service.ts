import { HttpClient } from '@angular/common/http';
import { computed, inject, Service, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { RealtimeService } from '../../core/realtime/realtime-service';
import { ShoppingItemResponse } from '../../shared/dtos/shopping.dtos';
import { db } from '../../core/persistence/app-db';

/** Verwaltet die Artikel der Einkaufsliste. Der lokale Speicher antwortet sofort, der Server gleicht im Hintergrund ab. */
@Service()
export class ShoppingListService {
    private readonly http = inject(HttpClient);
    private readonly realtime = inject(RealtimeService);

    private readonly _items = signal<ShoppingItemResponse[]>([]);
    readonly items = this._items.asReadonly();

    /** Meldet, ob der letzte Aufruf am Server fehlgeschlagen ist, etwa weil das Gerät kein Netz hat. */
    private readonly _offline = signal(false);
    readonly offline = this._offline.asReadonly();

    /** Noch offene Artikel, sortiert nach Kategorie */
    readonly openItems = computed(() =>
        this._items()
            .filter(x => !x.isChecked)
            .sort((a, b) => a.category - b.category || a.sortOrder - b.sortOrder)
    );

    /** Bereits abgehakte Artikel, unabhängig von der Sortierung der offenen Liste. */
    readonly checkedItems = computed(() => this._items().filter(x => x.isChecked));

    constructor() {
        this.registerRealtimeHandlers();
    }

    /**
     * Füllt die Liste zunächst aus dem lokalen Speicher, damit die Oberfläche nie leer startet,
     * und holt anschließend den maßgeblichen Stand vom Server nach.
     */
    async load(): Promise<void> {
        // Der zuletzt gespeicherte Stand ist sofort verfügbar und funktioniert auch ohne Netz
        const local = await db.shoppingItems.toArray();
        this._items.set(local);

        try {
            const remote = await firstValueFrom(
                this.http.get<ShoppingItemResponse[]>('/api/shopping-items')
            );
            // Andere Geräte können zwischenzeitlich Artikel gelöscht haben, deshalb wird der lokale Stand
            // komplett ersetzt statt zusammengeführt
            await db.transaction('rw', db.shoppingItems, async () => {
                await db.shoppingItems.clear();
                await db.shoppingItems.bulkPut(remote);
            });
            this._items.set(remote);
            this._offline.set(false);
        } catch {
            // Ohne Antwort vom Server bleibt der lokale Stand sichtbar und die Oberfläche zeigt den Offlinehinweis
            this._offline.set(true);
        }
    }

    /** Legt einen Artikel an. Er erscheint sofort in der Liste und wird danach am Server gespeichert. */
    async add(name: string, category: number): Promise<void> {
        // Die echte Id vergibt der Server, bis dahin trägt der Eintrag eine selbst erzeugte
        const temp: ShoppingItemResponse = {
            id: crypto.randomUUID(),
            name,
            category,
            sortOrder: 0,
            isChecked: false,
            checkedByUserId: null,
            checkedAtUtc: null,
        };
        await this.upsert(temp);

        try {
            const saved = await firstValueFrom(
                this.http.post<ShoppingItemResponse>('/api/shopping-items', { name, category })
            );
            // Der gespeicherte Artikel trägt die Id des Servers, der vorläufige muss dafür weichen
            await this.removeLocal(temp.id);
            await this.upsert(saved);
            this._offline.set(false);
        } catch {
            // Am Server ist nichts entstanden, ein Artikel ohne Gegenstück wäre irreführend
            await this.removeLocal(temp.id);
            this._offline.set(true);
        }
    }

    /** Hakt einen Artikel ab oder öffnet ihn wieder. */
    async toggle(id: string): Promise<void> {
        const previous = this._items().find(x => x.id === id);
        if (previous === undefined) {
            return;
        }

        // Der Haken erscheint sofort, damit das Abhaken im Laden nicht spürbar verzögert wirkt
        await this.upsert({
            ...previous,
            isChecked: !previous.isChecked,
            checkedAtUtc: previous.isChecked ? null : new Date().toISOString(),
        });

        try {
            const saved = await firstValueFrom(
                this.http.post<ShoppingItemResponse>(`/api/shopping-items/${id}/toggle`, {})
            );
            await this.upsert(saved);
            this._offline.set(false);
        } catch {
            await this.upsert(previous);   // Der gemerkte Stand ist der letzte, den auch der Server kennt
            this._offline.set(true);
        }
    }

    /** Löscht einen Artikel, zuerst lokal und danach am Server. */
    async remove(id: string): Promise<void> {
        const previous = this._items().find(x => x.id === id);
        if (previous === undefined) {
            return;
        }

        await this.removeLocal(id);

        try {
            await firstValueFrom(this.http.delete(`/api/shopping-items/${id}`));
            this._offline.set(false);
        } catch {
            await this.upsert(previous);   // Der Artikel liegt weiterhin am Server, also gehört er zurück in die Liste
            this._offline.set(true);
        }
    }

    /** Legt einen Artikel im lokalen Speicher ab und aktualisiert den sichtbaren Zustand. */
    private async upsert(item: ShoppingItemResponse): Promise<void> {
        await db.shoppingItems.put(item);
        this._items.update(items =>
            items.some(x => x.id === item.id)
                ? items.map(x => (x.id === item.id ? item : x))
                : [...items, item]
        );
    }

    /** Entfernt einen Artikel aus dem lokalen Speicher und aus dem sichtbaren Zustand. */
    private async removeLocal(id: string): Promise<void> {
        await db.shoppingItems.delete(id);
        this._items.update(items => items.filter(x => x.id !== id));
    }

    /** Änderungen anderer Geräte laufen über denselben Weg wie eigene Änderungen und landen so auch im lokalen Speicher. */
    private registerRealtimeHandlers(): void {
        this.realtime.onItemAdded(item => void this.upsert(item));
        this.realtime.onItemToggled(item => void this.upsert(item));
        this.realtime.onItemDeleted(id => void this.removeLocal(id));
    }
}