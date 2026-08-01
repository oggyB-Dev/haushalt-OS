/* Ein Artikel der Einkaufsliste */
export interface ShoppingItemResponse {
    id: string;
    name: string;
    category: number;
    sortOrder: number;
    isChecked: boolean;
    checkedByUserId: string | null;
    checkedAtUtc: string | null;
}

/** Kategorien für die Auswahl */
export enum ShoppingCategory {
    Obst = 0,
    Backwaren = 1,
    Fleisch = 2,
    Tiefkühl = 3,
    Drogerie = 4,
    Getraenke = 5,
    Sonstiges = 6
}

/** Anzeigetexte je Kategorie */
export const CATEGORY_LABELS: Record<ShoppingCategory, string> = {
    [ShoppingCategory.Obst]: "Obst",
    [ShoppingCategory.Backwaren]: "Backwaren",
    [ShoppingCategory.Fleisch]: "Fleisch",
    [ShoppingCategory.Tiefkühl]: "Tiefkühl",
    [ShoppingCategory.Drogerie]: "Drogerie",
    [ShoppingCategory.Getraenke]: "Getränke",
    [ShoppingCategory.Sonstiges]: "Sonstiges"
}

export const SHOPPING_CATEGORIES = Object.entries(CATEGORY_LABELS)
    .map(([value, label]) => ({ value: Number(value) as ShoppingCategory, label })
);