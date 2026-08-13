import { Component, signal } from '@angular/core';
import { OldWay } from './old-way/old-way';
import { NewWay } from './new-way/new-way';

@Component({
  selector: 'app-root',
  imports: [OldWay, NewWay],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('input-output-signals');
  productName: string;
  productQuantity: number;
  messageFromChild: string = '';

  constructor(){
    this.productName = '';
    this.productQuantity = 0;
  }
  addProduct(name: string, quantity: string): void {
    this.productName = name;
    this.productQuantity = Number(quantity);
    console.log(this.productName, quantity)
  }

  emittedFromChild(value: any){
    this.messageFromChild = value;
  }
}
