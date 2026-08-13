## Inputs & Outputs with Signals [Angular]

**What it is:** `input()`/`output()` are the signal-based replacement
for `@Input()`/`@Output()` decorators.

**My comparison — old vs new, full input+output cycle both ways:**
```typescript
// OLD -- old-way.ts
export class OldWay implements OnInit, OnChanges {
  @Input() productName!: string;        // '!' is a workaround, NOT enforced
  @Input() productQuantity!: number;
  @Output() toParent = new EventEmitter<string>();
  information: string = '';

  ngOnChanges(changes: SimpleChanges): void {
    if (this.productName != '') {
      this.information = 'This is a good purchase'; // manually re-derived
    }
  }

  pushValueToParent() {
    this.toParent.emit('From Old Way');
  }
}
```
```typescript
// NEW -- new-way.ts
export class NewWay {
  productName = input.required<string>();   // enforced -- won't compile if parent omits it
  productQuantity = input<number>(0);         // optional WITH a real default
  toParent = output<string>();

  information = computed(() => {
    return this.productName() !== '' ? 'This is a good purchase' : '';
  });

  pushValueToParent() {
    this.toParent.emit('From New Way');
  }
}
```
```html
<!-- app.html -->
<app-old-way [productName]="productName" [productQuantity]="productQuantity"
             (toParent)="emittedFromChild($event)"></app-old-way>
<app-new-way [productName]="productName" [productQuantity]="productQuantity"
             (toParent)="emittedFromChild($event)"></app-new-way>
```

**Gotcha 1 — `input<T>()` with no argument isn't "optional with a
default," it's just "optional, undefined if omitted":**
```typescript
productQuantity = input<number>();     // WRONG for "default" -- type is number | undefined
productQuantity = input<number>(0);    // RIGHT -- actual default value
```

**Gotcha 2 — a template can silently reference an output that doesn't
exist on the component.** Bound `(toParent)` to `OldWay` before it had
an `@Output() toParent` declared — worth confirming your build actually
catches this (strict template checking should flag it); fixed by
adding the missing `@Output()` + `EventEmitter` + emit method.

**Gotcha 3 — don't throw away type info with `any` on an event
handler.** Since `output<string>()` guarantees a `string`, the handler
should say so:
```typescript
emittedFromChild(value: string) { this.messageFromChild = value; } // not `any`
```

**Why the new way is genuinely better, not just shorter syntax:**
1. **`required` is actually enforced.** Removing `[productName]` from
   `<app-new-way>` fails to **compile**. Removing it from
   `<app-old-way>` compiles fine and silently renders `undefined` at
   runtime — a bug you'd only catch by looking at the screen.
2. **No lifecycle hooks needed.** `OldWay` needs `OnInit` + `OnChanges`
   + manual re-derivation logic in `ngOnChanges` — which re-runs on
   *any* input change, even unrelated ones. `computed()` only
   recalculates when the specific signals it reads actually change.
3. **Composability.** A `computed()` can feed into another
   `computed()` and stay reactive automatically. A plain field like
   `OldWay.information` only updates if you remember to update it
   manually — forget one line, it silently goes stale.
4. **`output()` itself is basically unchanged from `EventEmitter`** —
   same `.emit()` usage, just created via a function instead of
   `new EventEmitter()` + decorator. The real wins today are on the
   input side.

**One-liner for interviews:** "`input()`/`output()` make required
inputs actually enforced at compile time instead of relying on the
`!` operator, and since inputs are signals, they compose directly with
`computed()` without needing `ngOnChanges` or manual re-derivation.
`output()` itself is nearly identical to `EventEmitter` — just
function-based instead of decorator-based."