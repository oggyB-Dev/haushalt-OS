import Dexie, { Table } from 'dexie';
import { ShoppingItemResponse } from '../../shared/dtos/shopping.dtos';

/** Lokaler Browserspeicher für den offline Betrieb */
export class AppDb extends Dexie {
    /* Artikel der Einkaufsliste */
    shoppingItems!: Table<ShoppingItemResponse, string>;

    constructor() {
        super("haushalts-os");
        this.version(1).stores({
            shoppingItems: 'id, category, isChecked',
        });
    }
}

export const db = new AppDb();