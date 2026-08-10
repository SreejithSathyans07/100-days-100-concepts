import { ChangeDetectionStrategy, Component, OnInit, signal } from '@angular/core';

interface Order {
  orderId: number;
  productName: string;
  quantity: number;
  deliveryStatus: 'Order confirmed' | 'Shipped' | 'Delivered' | 'Canceled';
}
@Component({
  selector: 'app-root',
  imports: [],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App implements OnInit {
  protected readonly title = signal('new-control-flow');
  myOrder = signal<Order[]>([]);

  ngOnInit(): void {
    this.myOrder.set([
      { orderId: 1, productName: 'Laptop', quantity: 5, deliveryStatus: 'Order confirmed' },
      { orderId: 2, productName: 'Mobile', quantity: 45, deliveryStatus: 'Shipped' },
      { orderId: 3, productName: 'Fridge', quantity: 2, deliveryStatus: 'Delivered' },
    ]);
    console.log(this.myOrder());
  }
}
