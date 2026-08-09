import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
  selector: 'app-counter-display',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [],
  templateUrl: './counter-display.html',
  styleUrl: './counter-display.css',
})
export class CounterDisplay {
  counter = 0;
}
