import { Component, computed, effect, input, output } from '@angular/core';

@Component({
  selector: 'app-new-way',
  imports: [],
  templateUrl: './new-way.html',
  styleUrl: './new-way.css',
})
export class NewWay {
  productName = input.required<string>();
  productQuantity = input<number>(0);
  toParent = output<string>();

  information = computed(()=> {
    if(this.productName() != ''){
      return 'This is a good purchase'
    }
    return '';
  })

  pushValueToParent(){
    this.toParent.emit('From New Way');
  }
}
