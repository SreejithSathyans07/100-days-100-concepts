import { Component, computed, effect, signal } from '@angular/core';

@Component({
  selector: 'app-effect-signals',
  standalone: true,
  imports: [],
  templateUrl: './effect-signals.html',
  styleUrl: './effect-signals.css',
})
export class EffectSignals {
  balance = signal(1000);
  interest = computed(() => this.balance() * 0.07);
  finalBalance = computed(() => this.balance() + this.interest());

  constructor() {
    effect(() => {
      console.log(`Balance is now ${this.balance()}, with interest ${this.interest()}`);
    });
  }

  addToBalance(amountText: string): void {
    const amount = Number(amountText);

    if (!Number.isNaN(amount)) {
      this.balance.update((current) => current + amount);
    }
  }
}
