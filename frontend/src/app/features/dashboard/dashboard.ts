import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { HouseholdService } from '../household/household-service';
import { ShoppingListService } from '../shopping-list/shopping-list-service';
import { CurrentUserService } from '../../core/auth/current-user-service';
import { Card } from '../../shared/ui/card';
import { Button } from '../../shared/ui/button';

/** Begrüßung je Tageszeit, `bis` ist die erste Stunde, die nicht mehr dazugehört */
const GREETINGS = [
  { bis: 5,  salutation: "Gute Nacht",   subline: "Noch wach?" },
  { bis: 11, salutation: "Guten Morgen", subline: "Gut geschlafen?" },
  { bis: 18, salutation: "Guten Tag",    subline: "Was steht heute an?" },
  { bis: 22, salutation: "Guten Abend",  subline: "Feierabend!" },
  { bis: 24, salutation: "Gute Nacht",   subline: "Noch wach?" },
];

@Component({
  selector: 'app-dashboard',
  imports: [Card, Button, RouterLink],
  templateUrl: './dashboard.html',
})
export class Dashboard implements OnInit {
  private readonly householdService = inject(HouseholdService);
  protected readonly household = this.householdService.household;

  private readonly shoppingListService = inject(ShoppingListService);

  private readonly currentUserService = inject(CurrentUserService);
  protected readonly firstName = this.currentUserService.firstName;

  // Begrüßung passend zur aktuellen Tageszeit
  protected readonly greeting = computed(() => {
    const hour = new Date().getHours();
    return GREETINGS.find(x => hour < x.bis)!;
  });

  // Anzahl der offenen Artikel, aktualisiert sich dank Echtzeitereignissen von selbst
  protected readonly openItemCount = computed(() => this.shoppingListService.openItems().length);

  // Erst nach dem Laden zählen wir die Artikel, sonst stünde kurz "Alles besorgt" da
  protected readonly shoppingListLoaded = signal(false);

  // Flag, um anzuzeigen, dass der Einladungscode kopiert wurde
  protected readonly copied = signal(false);

  // Wird beim Anzeigen der Seite einmalig ausgeführt
  async ngOnInit(): Promise<void> {
    await Promise.all([
      this.householdService.load(),
      this.currentUserService.load(),
      this.loadShoppingList(),
    ]);
  }

  // Lädt die Einkaufsliste und gibt die Kachel danach frei
  private async loadShoppingList(): Promise<void> {
    await this.shoppingListService.load();
    this.shoppingListLoaded.set(true);
  }

  // Kopiert den Einladungscode in die Zwischenablage und zeigt eine Bestätigung an
  protected async copyInviteCode(): Promise<void> {
    await navigator.clipboard.writeText(this.household()!.inviteCode);
    this.copied.set(true);
    setTimeout(() => {
      this.copied.set(false);
    }, 2000);
  }
}