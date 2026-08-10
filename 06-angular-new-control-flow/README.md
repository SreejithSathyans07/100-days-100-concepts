## New Control Flow (@if/@for/@switch) [Angular]

**What it is:** Built-in template syntax for conditionals and loops —
`@if`, `@for`, `@switch` — replacing the old `*ngIf`/`*ngFor`/`*ngSwitch`
structural directives. Introduced in **Angular 17** (Nov 2023).

**Old way — structural directives:**
```html
<div *ngIf="myOrder.length > 0">
  <p *ngFor="let order of myOrder; trackBy: trackById">{{ order.productName }}</p>
</div>
```
Problems with this approach:
- Needed `CommonModule` (or `NgIf`/`NgFor`) explicitly imported — easy to
  forget, especially in standalone components.
- `trackBy` was **optional** — easy to skip, which silently hurt
  performance (see Gotcha 1 below).
- `*` syntax is sugar hiding an `<ng-template>` underneath — extra
  indirection, slower to parse.

**New way — what I actually built:**
```typescript
interface Order {
  orderId: number;
  productName: string;
  quantity: number;
  deliveryStatus: 'Order confirmed' | 'Shipped' | 'Delivered' | 'Canceled';
}

myOrder = signal<Order[]>([]);

ngOnInit(): void {
  this.myOrder.set([
    { orderId: 1, productName: 'Laptop', quantity: 5, deliveryStatus: 'Order confirmed' },
    { orderId: 2, productName: 'Mobile', quantity: 45, deliveryStatus: 'Shipped' },
    { orderId: 3, productName: 'Fridge', quantity: 2, deliveryStatus: 'Delivered' },
  ]);
}
```
```html
@if (myOrder().length > 0) {
  <h2>Please find the order</h2>
  <hr />
  @for (order of myOrder(); track order.orderId) {
    <h3>Order Id: {{ order.orderId }}</h3>
    <h3>Product Name: {{ order.productName }}</h3>
    <h3>Order Quantity: {{ order.quantity }}</h3>
    @switch (order.deliveryStatus) {
      @case ('Order confirmed') { <h4>Your order is confirmed and will be dispatched sooner</h4> }
      @case ('Shipped') { <h4>Your order is already shipped to your address</h4> }
      @case ('Delivered') { <h4>Your order is delivered</h4> }
      @case ('Canceled') { <h4>You have cancelled your order</h4> }
    }
    <hr />
  }
}
```
- No import needed — works in every component automatically.
- Faster (compiled directly, not through the generic directive system).
- `@switch`/`@case`/`@default` replaces `*ngSwitch` — same idea as
  Day 3's C# switch, but no `_`, uses `@default` instead.

**Gotcha 1 — `track` is MANDATORY, not optional like old `trackBy`.**
Without `track order.orderId`, Angular can't tell which array items
actually changed, and may destroy/recreate DOM nodes unnecessarily on
every update — even for rows whose data didn't change. Angular made it
a compile error to forget, instead of trusting developers to remember.

**`trackBy` vs `track` — not interchangeable syntax, same underlying goal:**

| | `*ngFor` | `@for` |
|---|---|---|
| Keyword | `trackBy` | `track` |
| Needs a separate function? | Yes | No — inline expression |
| Optional or mandatory? | **Optional** — easy to forget | **Mandatory** — won't compile without it |
| Behavior once correctly added | Same tracking benefit | Same tracking benefit |

`*ngFor="let order of myOrder(); track order.orderId"` is **invalid** —
`*ngFor` only understands `trackBy`, pointing to a function:
```html
<!-- old, WITHOUT trackBy -- all rows re-created on any array refresh -->
<div *ngFor="let order of myOrder()">...</div>

<!-- old, WITH trackBy -- had to remember this + write a function -->
<div *ngFor="let order of myOrder(); trackBy: trackByOrderId">...</div>
```
```typescript
trackByOrderId(index: number, order: Order): number {
  return order.orderId;
}
```
```html
<!-- new -- one inline expression, mandatory -->
@for (order of myOrder(); track order.orderId) { ... }
```
**Bottom line:** the *behavior* converges once `trackBy` is correctly
added — the ergonomics and safety don't. `@for` forces the correct,
performant behavior by default; `*ngFor` only gets there if you
remember the extra step.

**Concrete example — what `track` actually prevents:**
Refreshing the order list with a brand-new array (e.g. from an API),
where only the Fridge's `quantity` really changed:
```typescript
refreshOrders() {
  this.myOrder.set([
    { orderId: 1, productName: 'Laptop', quantity: 5, deliveryStatus: 'Order confirmed' },
    { orderId: 2, productName: 'Mobile', quantity: 45, deliveryStatus: 'Shipped' },
    { orderId: 3, productName: 'Fridge', quantity: 99, deliveryStatus: 'Delivered' }, // only this changed
  ]);
}
```
- **No `track`/`trackBy`:** all 3 rows are new object references, so
  Angular destroys and recreates **all 3** DOM blocks, even though
  Laptop/Mobile's data didn't change.
- **With `track order.orderId`:** Angular matches by `orderId` —
  **only the Fridge row** updates; Laptop/Mobile's DOM nodes are left
  untouched.
- **Verify visually:** Chrome DevTools → Elements panel, click a
  refresh button, watch which nodes flash on update. Try
  `track $index` instead of `track order.orderId` — since `$index`
  carries no real data identity, all 3 rows flash again, same as no
  tracking at all.

**Gotcha 2 — once a value is a signal, `()` is needed EVERYWHERE it's
read, no exceptions.** This bug actually happened while building this:
```html
<!-- WRONG -- myOrder is the signal itself, not the array -->
@if (myOrder.length > 0) { ... }

<!-- RIGHT -->
@if (myOrder().length > 0) { ... }
```
Missing it on just the `@if` line (while the `@for` line already had it
correct) silently broke rendering entirely.

**Gotcha 3 — mutating a signal's array in place doesn't work, same
trap as Day 4's `@ViewChild` mutation:**
```typescript
// WRONG -- Angular never sees this change
this.myOrder().push(newOrder);

// RIGHT -- replace with a new array
this.myOrder.update(list => [...list, newOrder]);
```

**One-liner for interviews:** "I use `@if`/`@for`/`@switch` over the
old structural directives — no imports needed, better performance, and
`track` being mandatory removes a whole class of accidental
re-rendering bugs that `trackBy` used to let slip through."