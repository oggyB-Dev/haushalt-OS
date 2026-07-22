import { Component, computed, input } from '@angular/core';

/** Wiederverwendbarer Button in den Design-Token-Varianten. */
@Component({
  selector: 'app-button',
  templateUrl: './button.html',
})
export class Button {
  readonly variant = input<'primary' | 'ghost'>('primary');
  readonly type = input<'button' | 'submit'>('button');
  readonly disabled = input(false);

  protected readonly classes = computed(() => {
    const base = 'rounded-control px-4 py-3.5 text-base font-bold transition ' + 'active:scale-[0.98] disabled:pointer-events-none disabled:opacity-50';
    return this.variant() === 'primary'
      ? `${base} w-full bg-primary text-white hover:bg-primary-strong`
      : `${base} bg-transparent text-primary hover:bg-primary-soft`;
  });
}