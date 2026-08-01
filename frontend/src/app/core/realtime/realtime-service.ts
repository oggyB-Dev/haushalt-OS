import { inject, Service } from '@angular/core';
import { TokenStoreService } from '../auth/token-store-service';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { MessagePackHubProtocol } from '@microsoft/signalr-protocol-msgpack';
import { ShoppingItemResponse } from '../../shared/dtos/shopping.dtos';

/* Hält die Echtzeitverbindung zur Einkaufsliste und verteilt eingehende Ereignisse */
@Service()
export class RealtimeService {
    private readonly tokenStore = inject(TokenStoreService);
    private connection: HubConnection | null = null;

    /* Baut die Verbindung auf, falls noch keine besteht */
    async connect(): Promise<void> {
        if (this.connection) {
            return;
        }

        this.connection = new HubConnectionBuilder()
            .withUrl("/api/hubs/shopping-list", {
                accessTokenFactory: () => this.tokenStore.accessToken() ?? "",
            })
            .withHubProtocol(new MessagePackHubProtocol())
            .withAutomaticReconnect()
            .build();
        
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
        this.connection?.on("ItemAdded", handler);
    }

    /* Registriert einen Handler für abgehakte oder wieder geöffnete Artikel */
    onItemToggled(handler: (item: ShoppingItemResponse) => void) : void {
        this.connection?.on("ItemToggled", handler);
    }

    /* Registriert einen Handler für gelöschte Artikel */
    onItemDeleted(handler: (id: string) => void) : void {
        this.connection?.on("ItemDeleted", handler);
    }
}
