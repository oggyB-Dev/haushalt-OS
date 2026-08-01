import { HttpClient } from "@angular/common/http";
import { computed, inject, Service, signal } from "@angular/core";
import { RealtimeService } from "../../core/realtime/realtime-service";
import { ShoppingItemResponse } from "../../shared/dtos/shopping.dtos";
import { firstValueFrom } from "rxjs";

/* Verwaltet die Artikel der Einkaufsliste und hält sie über Echtzeit-Ereignisse aktuell */
@Service()
export class ShoppingListService {
    private readonly http = inject(HttpClient);
    private readonly realtime = inject(RealtimeService);

    /** Alle Artikel des Haushalts */
    private readonly _items = signal<ShoppingItemResponse[]>([]);
    readonly items = this._items.asReadonly();

    /** Offene Artikel */
    readonly openItems = computed(() =>
        this._items()
            .filter(x => !x.isChecked)
            .sort((a,b) => a.category - b.category || a.sortOrder - b.sortOrder)
    );

    /** Bereits abgehakte Artikel */
    readonly checkedItems = computed(() =>
        this._items()
            .filter(x => x.isChecked)
    );

    constructor() {
        this.registerRealtimeHandlers();
    }

    /** Lädt die Liste vom Server */
    async load(): Promise<void> {
        const items = await firstValueFrom(
            this.http.get<ShoppingItemResponse[]>("/api/shopping-items")
        );
        this._items.set(items);
    }

    /** Legt einen Artikel an */
    async add(name: string, category: number): Promise<void> {
        const item = await firstValueFrom(
            this.http.post<ShoppingItemResponse>("/api/shopping-items", {name, category})
        );
        this.upsert(item);
    }

    /** Hakt einen Artikel ab oder öffnet ihn wieder */
    async toggle(id: string): Promise<void> {
        const item = await firstValueFrom(
            this.http.post<ShoppingItemResponse>(`/api/shopping-items/${id}/toggle`, {})
        );
        this.upsert(item);
    }

    /** Löscht einen Artikel */
    async remove(id: string): Promise<void> {
        await firstValueFrom(
            this.http.delete(`/api/shopping-items/${id}`)
        );
        this.removeLocal(id);
    }

    /** Fügt einen Artikel ein oder ersetzt die vorhandene Fassung */
    private upsert(item: ShoppingItemResponse): void {
        this._items.update(items =>
            items.some(x => x.id === item.id)
                ? items.map(x => x.id === item.id ? item : x)
                : [...items, item]
        );
    }

    /** Entfernt einen Artikel aus dem lokalen Zustand */
    private removeLocal(id: string): void {
        this._items.update(items => items.filter(x => x.id !== id));
    }

    /** Verbindet die Echtzeit-Ereignisse mit dem lokalen Zustand */
    private registerRealtimeHandlers(): void {
        this.realtime.onItemAdded(item => this.upsert(item));
        this.realtime.onItemToggled(item => this.upsert(item));
        this.realtime.onItemDeleted(id => this.removeLocal(id));
    }

}
