import { Component, EventEmitter, Input, input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';

@Component({
  selector: 'app-old-way',
  imports: [],
  templateUrl: './old-way.html',
  styleUrl: './old-way.css',
})
export class OldWay implements OnInit, OnChanges {
  @Input() productName!: string;
  @Input() productQuantity!: number;
  @Output() toParent = new EventEmitter<string>();
  information: string = '';
  ngOnInit(): void {
    console.log(this.productName);
    console.log(this.productQuantity);
  }
  ngOnChanges(changes: SimpleChanges): void {
    if(this.productName != ''){

      this.information = 'This is a good purchase';
    }
  }

  pushValueToParent(){
    this.toParent.emit('From Old Way');
  }
}
