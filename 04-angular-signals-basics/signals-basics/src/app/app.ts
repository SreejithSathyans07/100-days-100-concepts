import { ChangeDetectionStrategy, Component, OnInit, signal, ViewChild } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CounterDisplay } from './counter-display/counter-display';
import { ComputedSignals } from './computed-signals/computed-signals';
import { EffectSignals } from './effect-signals/effect-signals';

@Component({
  selector: 'app-root',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CounterDisplay, ComputedSignals, EffectSignals],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App implements OnInit {
  protected readonly title = signal('signals-basics');
  signalCounter = signal(0);
  normalCounter: number = 0;
  @ViewChild(CounterDisplay) counterChild!: CounterDisplay;

  ngOnInit(): void {
    setTimeout(() => {
      this.signalCounter.set(this.signalCounter() + 1);
      this.normalCounter += 1;
      console.log('updated both, count2 is now', this.normalCounter);
      this.counterChild.counter += 1;
    }, 3000);
    setTimeout(() => {
      console.log('The counterChild.counter is now ' + this.counterChild.counter);
    }, 4000);


  }
}
