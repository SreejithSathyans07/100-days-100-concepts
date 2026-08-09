# Day 04 — Signals & Change Detection [Angular]

## Definitions

**Signal:** A special box that holds a value, and *knows* whenever
something reads it or changes it. You get the value by calling it like
a function — `count()`, not `count`.

**Change Detection:** Angular's process of checking your component's
data and updating the screen (DOM) to match. The question change
detection always answers is: *"has anything changed, and if so, where
do I need to repaint?"*

---

## What we covered

1. **Before signals**, Angular used Zone.js to detect that "something
   happened" (a click, a timer, an HTTP call) and then re-checked the
   **entire** component tree, just in case anything changed anywhere.

2. **A signal is a box, not a value.** You read it by calling it:
```typescript
   count = signal(0);
   console.log(this.count()); // 0
```

3. **You change it with `.set()`:**
```typescript
   this.count.set(this.count() + 1);
```

4. **First surprise:** a plain property updated the screen just as
   well as a signal did — no visible difference at all:
```typescript
   signalCounter = signal(0);
   normalCounter = 0;

   incrementCount()  { this.signalCounter.set(this.signalCounter() + 1); }
   incrementCount2() { this.normalCounter += 1; }
```
   Why? Default change detection re-checks *everything* on any event
   — it doesn't care whether you used a signal or not.

5. **To actually see a difference, we needed `OnPush`** — a stricter
   mode where a component's view is *only* re-checked when specific
   things happen to it (an input changes, a template event fires, a
   signal it reads changes) — not on every random event in the app:
```typescript
   @Component({
     selector: 'app-root',
     changeDetection: ChangeDetectionStrategy.OnPush,
     ...
   })
```

6. **Even with `OnPush`, both counters still updated at first**,
   using a `setTimeout` (no click involved) to change both:
```typescript
   setTimeout(() => {
     this.signalCounter.set(this.signalCounter() + 1);
     this.normalCounter += 1;
   }, 3000);
```
   Because both values lived in the *same* template, one signal
   changing was enough to mark that whole view dirty, dragging the
   plain property along with it "for free."

7. **The real test needed two separate components.** We moved the
   plain counter into a child (`CounterDisplay`) with its own
   `OnPush`, and mutated it from the parent through a `@ViewChild`
   reference:
```typescript
   @ViewChild(CounterDisplay) counterChild!: CounterDisplay;

   setTimeout(() => {
     this.counterChild.counter += 1; // direct mutation from outside
   }, 3000);
```

8. **This revealed the actual gap:** the value truly changed in
   memory — confirmed with a second, delayed check —
```typescript
   setTimeout(() => {
     alert('The counterChild.counter is now ' + this.counterChild.counter);
   }, 4000);
```
   — but the child's `<h2>Count {{ counter }}</h2>` never updated on
   screen. Data changed; Angular just never found out, because none
   of `OnPush`'s trigger conditions were met.


9. **The takeaway:** `OnPush` alone is fast but risky — it's easy to
    silently break by mutating things Angular isn't watching (like
    step 7's `@ViewChild` mutation). Signals make `OnPush` *safe by
    default*, because a signal always tells Angular exactly when it
    changed and exactly what depends on it 
---

## computed() and effect()

10. **`computed()` creates a signal whose value is derived automatically**
    from other signals. It has no `.set()` — it's read-only:
```typescript
    signalCounter = signal(0);
    doubleCounter = computed(() => this.signalCounter() * 2);
```

11. **`effect()` runs a side effect automatically** whenever a signal it
    reads changes. It returns nothing, and must be created in an
    injection context (typically the constructor):
```typescript
    constructor() {
      effect(() => {
        console.log('signalCounter changed to', this.signalCounter());
      });
    }
```

12. **Both `computed()` and `effect()` auto-track EVERY signal they
    read** — not just one. There's no step where you manually say
    "watch this signal"; it's detected automatically based on which
    signals get called `()` inside the function:
```typescript
    fullName = computed(() => `${this.firstName()} ${this.lastName()}`);
    // depends on BOTH firstName and lastName -- changing either recalculates it
```
    Tracking is also re-evaluated fresh on every run, so a signal read
    inside an `if` branch is only tracked while that branch executes.

13. **⭐ The rule that actually decides computed vs effect: purity, not
    complexity.** Ask: "does this produce a value, or does it reach
    outside and DO something (console, network, localStorage, another
    signal, a plain property)?"
    - Produces a value you'll use → **`computed()`** — even if the
      calculation is genuinely complex (loops, conditionals, multiple
      derived numbers).
    - Touches the outside world in any way → **`effect()`** — even if
      it's a single, trivial line like a console log.
    "Simple = computed, complex = effect" is the wrong way to think
    about it — complexity has no bearing on which one is correct.

14. **The mistake we actually made:** writing a pure calculation
    inside `effect()`, just because it was being assigned to a plain
    property instead of a signal:
```typescript
    // WRONG -- this is pure math, shouldn't be in effect()
    finalBalance = 0;
    constructor() {
      effect(() => {
        this.finalBalance = this.balance() + this.interest();
      });
    }
```
    Fixed by making it a `computed()` instead — no constructor, no
    effect needed at all:
```typescript
    // RIGHT
    balance = signal(1000);
    interest = computed(() => this.balance() * 0.05);
    finalBalance = computed(() => this.balance() + this.interest());
```
    A genuinely correct `effect()` for the same component is logging
    or persisting — something that truly reaches outside:
```typescript
    constructor() {
      effect(() => {
        console.log(`Balance is now ${this.balance()}, interest ${this.interest()}`);
        localStorage.setItem('savedBalance', this.balance().toString());
      });
    }
```

15. **`computed()` may be called multiple times or skipped by Angular
    internally** — this is *why* it must stay pure. A side effect
    (API call, `.set()` on another signal) inside a `computed()` could
    fire an unpredictable number of times, or not at all.

16. **`untracked()`** lets you deliberately read a signal inside an
    `effect()` without tracking it as a dependency — rarely needed,
    but useful when you want to read a value without reacting to its
    changes:
```typescript
    effect(() => {
      console.log('age is', this.age());
      console.log('ignoring name changes:', untracked(() => this.name()));
    });
```

**Gotcha:** `effect()` must be created in an injection context
(constructor or field initializer) — calling it from inside a regular
method throws a runtime error.

**One-liner for interviews:** "I use `computed()` for anything that's
just derived data, no matter how complex the math, and `effect()`
only for side effects — logging, persistence, API calls — because
`computed()` isn't guaranteed to run exactly once per change the way
`effect()` is."