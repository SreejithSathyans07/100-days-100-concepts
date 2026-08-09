import { Component, computed, OnInit, signal } from '@angular/core';

@Component({
  selector: 'app-computed-signals',
  standalone: true,
  imports: [],
  templateUrl: './computed-signals.html',
  styleUrl: './computed-signals.css',
})
export class ComputedSignals implements OnInit {
  timeOutInterval = 3;
  signalCounter = signal(2);
  computedSquareSignal = computed(() => this.signalCounter() * this.signalCounter());

  ngOnInit(): void {
    setTimeout(() => {
      this.signalCounter.set(5);
      //this.computedSquareSignal.set(4) // Won't work, computed signals are read-only
    }, this.timeOutInterval * 1000);
  }
}
