# Standalone Components

- Before standalone components existed, every component had to belong to an NgModule. 
- A component couldn't just... exist on its own — it needed a module to declare it, and anything that wanted to use it needed to import that module.
```typescript
// order-card.component.ts
@Component({
  selector: 'app-order-card',
  template: `<div class="card">Order #{{ id }}</div>`,
  // no "standalone" flag -- this was just the only option
})
export class OrderCardComponent {
  id = 1;
}
```

```typescript
// order.module.ts -- a SEPARATE file, required
@NgModule({
  declarations: [OrderCardComponent],   // "this module owns this component"
  exports: [OrderCardComponent],        // "other modules may use it"
  imports: [CommonModule],
})
export class OrderModule {}
```

```typescript
// somewhere-else.module.ts -- to use OrderCardComponent, you import the MODULE
@NgModule({
  imports: [OrderModule],  // can't import the component directly
  declarations: [SomewhereElseComponent],
})
export class SomewhereElseModule {}
```

> In the legacy way, a component should always belong to one module. When we want to use this component in another component, we need to import this whole module into the other module (which holds that particular component where we need to import the component). Once you import the module, every component that module exports becomes available to all components declared in your module — automatically.

Now is this correct?

#### The new way: Standalone

```typescript
@Component({
  selector: 'app-order-card',
  standalone: true, // default since Angular 19, but fine to write explicitly
  template: `<div class="card">Order #{{ id }}</div>`,
})
export class OrderCardComponent {
  id = 1;
}
```
```typescript
// somewhere-else.component.ts -- import the COMPONENT directly, no module needed
@Component({
  selector: 'app-somewhere-else',
  standalone: true,
  imports: [OrderCardComponent], // straight to the component
  template: `<app-order-card />`,
})
export class SomewhereElseComponent {}
```

- Angular ≤ 13 — Standalone components don't exist. NgModules are mandatory.
- Angular 14 — Standalone components introduced, but as Developer Preview (experimental, API could still change).
- Angular 15 — Standalone components become stable, production-ready.
- Angular 15–18 — Stable and available, but you must explicitly write standalone: true (default is still false if omitted).
- Angular 19+ — standalone: true becomes the default when the flag is omitted.
- Angular 20 (your version) — Stable + default. Omitting standalone = standalone by default.