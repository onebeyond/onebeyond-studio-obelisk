# Obelisk Context Pack Instructions

Start in [`docs/context/overview.md`](../docs/context/overview.md).

Use `docs/context/_generated/` for repository facts such as:
- project and folder layout
- dependency inventory
- configuration keys
- authentication and authorization surface

Use the handwritten files in `docs/context/` for:
- architecture explanations
- delivery guidance
- patterns and anti-patterns
- AI usage boundaries

When code and guidance appear to disagree:
- trust code for current factual behavior
- trust the handwritten docs for intended delivery conventions
- call out the mismatch explicitly instead of guessing

If you change inputs that affect generated facts, refresh them with:

```powershell
dotnet run --project tools/Obelisk.ContextPack -- generate-template --repo .
```
