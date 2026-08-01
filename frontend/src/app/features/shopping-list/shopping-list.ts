import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { ShoppingListService } from './shopping-list-service';
import { SHOPPING_CATEGORIES } from '../../shared/dtos/shopping.dtos';
import { Button } from '../../shared/ui/button';

/** Gemeinsame Einkaufsliste des Haushalts */
@Component({
  selector: 'app-shopping-list',
  imports: [Button],
  templateUrl: './shopping-list.html',
  styleUrl: './shopping-list.scss',
})
export class ShoppingList implements OnInit {
  private readonly service = inject(ShoppingListService);

  /** Artikel der Einkaufsliste */
  protected readonly openItems = this.service.openItems;
  protected readonly checkedItems = this.service.checkedItems;

  /** Auswahlmöglichkeit für das Kategoriefeld */
  protected readonly categories = SHOPPING_CATEGORIES;

  /** Offene Artikel gruppiert nach Kategorie */
  protected readonly groupedOpenItems = computed(() =>
    this.categories
      .map(category => ({
        label: category.label,
        items: this.openItems().filter(x => x.category === category.value),
      }))
      .filter(group => group.items.length > 0)
  );

  /** Name des neuen Artikels */
  protected readonly newItemName = signal("");

  /** Gewählte Kategorie */
  protected readonly newItemCategory = signal(6);

  /** Wird beim Anzeigen der Seite einmalig ausgeführt */
  async ngOnInit(): Promise<void> {
    await this.service.load();
  }

  /** Legt den eingegebenen Artikel an und leert das Eingabefeld */
  protected async add(): Promise<void> {
    const name = this.newItemName().trim();

    if(name === ""){
      return;
    }

    await this.service.add(name, this.newItemCategory());
    this.newItemName.set("");
  }

  /** Hakt einen Artikel ab oder öffnet ihn wieder */
  protected async toggle(id: string): Promise<void> {
    await this.service.toggle(id);
  }

  /** Löscht einen Artikel */
  protected async remove(id: string): Promise<void> {
    await this.service.remove(id);
  }
}
