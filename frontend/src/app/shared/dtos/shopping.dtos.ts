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