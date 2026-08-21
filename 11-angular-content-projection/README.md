# Content Projection [Angular]

**What it is**
`<ng-content>` lets a component's caller inject markup into it — the component controls *where* content renders, the caller controls *what* it is. Contrast with `@Input()`, where the component controls both what data comes in and how it's rendered.

**Key code snippets**

Child (projecting component):
```typescript
@Component({
  selector: 'app-child',
  imports: [],
  templateUrl: './child.html',
  styleUrl: './child.css',
})
export class Child {}
```
```html
<div class="panel-header">
  <ng-content select="[header]"></ng-content>
</div>
<div class="panel-content">
  <ng-content></ng-content>
</div>
```

Parent (usage):
```html
<app-child>
  <div header>Panel Title</div>
  <p>Some body text</p>
  <p>Some body text</p>
  <span header>Extra header bit</span>
</app-child>
```

- `select="[header]"` matches any projected element carrying a plain `header` attribute — no directive needed, just an attribute selector.
- Unnamed `<ng-content>` is the catch-all — only **one** allowed per template.
- Wrapping `<ng-content>` in your own `<div>` (e.g. `.panel-header`) is purely a styling hook owned by the child — it has zero effect on who owns the projected content.

**Gotcha(s)**
- Projected content is compiled and scoped to the **parent's** component instance, not the child's — even though it renders inside the child's DOM. A `{{ counter }}` inside projected markup always refers to the parent's `counter`, never the child's.
- Projection re-groups elements **by slot**, not by original source order. `<span header>` written *after* a `<p>` in the parent will still render *before* it visually, because the `header` `<ng-content>` sits earlier in the child's template.
- The real `@Input()` vs projection distinction is **who authors the markup structure** — not "static vs dynamic" content. Data in → component renders it (`@Input()`). Caller-authored markup → component just provides a slot (projection).

**One-liner for interviews**
"`@Input()` passes data for the component to render; content projection (`<ng-content>`) lets the caller pass markup for the component to place — ownership of the rendering logic is the deciding factor."