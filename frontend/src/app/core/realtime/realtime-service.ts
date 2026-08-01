import { inject, Service } from '@angular/core';
import { TokenStoreService } from '../auth/token-store-service';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ShoppingItemResponse } from '../../shared/dtos/shopping.dtos';

/* Hält die Echtzeitverbindung zur Einkaufsliste und verteilt eingehende Ereignisse */
@Service()
export class RealtimeService {
    private readonly tokenStore = inject(TokenStoreService);
    private connection: HubConnection | null = null;

    /** Gemerkte Handler je Ereignis, damit sie nach Logout und erneutem Login an der neuen Verbindung ankommen */
    private readonly handlers = new Map<string, ((...args: any[]) => void)[]>();

    /* Baut die Verbindung auf, falls noch keine besteht */
    async connect(): Promise<void> {
        if (this.connection) {
            return;
        }

        this.connection = new HubConnectionBuilder()
            .withUrl("/api/hubs/shopping-list", {
                accessTokenFactory: () => this.tokenStore.accessToken() ?? "",
            })
            .withAutomaticReconnect()
            .build();

        // Gemerkte Handler an der neuen Verbindung anmelden
        for (const [event, eventHandlers] of this.handlers) {
            for (const handler of eventHandlers) {
                this.connection.on(event, handler);
            }
        }

        await this.connection.start();
    }

    /* Trennt die Verbindung */
    async disconnect(): Promise<void> {
        if (this.connection === null) {
            return;
        }

        await this.connection.stop();
        this.connection = null;
    }

    /* Registriert einen Handler für neu hinzugefügte Artikel */
    onItemAdded(handler: (item: ShoppingItemResponse) => void) : void {
        this.on("ItemAdded", handler);
    }

    /* Registriert einen Handler für abgehakte oder wieder geöffnete Artikel */
    onItemToggled(handler: (item: ShoppingItemResponse) => void) : void {
        this.on("ItemToggled", handler);
    }

    /* Registriert einen Handler für gelöschte Artikel */
    onItemDeleted(handler: (id: string) => void) : void {
        this.on("ItemDeleted", handler);
    }

    /* Merkt sich einen Handler und meldet ihn an der aktuellen Verbindung an */
    private on(event: string, handler: (...args: any[]) => void): void {
        const eventHandlers = this.handlers.get(event) ?? [];
        eventHandlers.push(handler);
        this.handlers.set(event, eventHandlers);

        this.connection?.on(event, handler);
    }
}
