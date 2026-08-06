## Day 03 — Pattern Matching [.NET]

**What it is:** `switch` expressions (`x switch {...}`) replace verbose `switch`
statements for "pick one value based on shape/type" logic. The underlying
feature is called **pattern matching** — switch expressions are just one
place it's used (also works in plain `if (x is Pattern)`).

```typescript
if (user is UserDtoNew { Age: >= 60 } u)
{
    Console.WriteLine($"{u.EmpName} is a senior citizen");
}
```

**Patterns used:**
- **Type** — `UserDto u`
- **Property** — `UserDto { Department: "Health" } u`
- **Relational** — `Age: >= 60`
- **Combinators** — `Age: >= 0 and < 18`

**Introduced across versions:**
- C# 7 — type & constant patterns
- C# 8 — switch expressions, property patterns, tuple patterns
- C# 9 — relational patterns, combinators (`and`/`or`/`not`)

**Gotcha:** Patterns are checked top to bottom, first match wins. Broader
patterns must come *after* narrower ones, or the narrow one never gets hit.

**Also learned:** Always handle `null` explicitly as its own pattern —
the compiler warns (CS8509) if a reference-type switch doesn't cover it.

**One-liner for interviews:** "I reach for switch expressions with property
and relational patterns instead of if/else chains — they're more readable
and the compiler flags unhandled cases."