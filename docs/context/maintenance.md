# Maintenance

Use this checklist whenever the template is prepared for release or when repository facts materially change.

## Release checklist

- Refresh generated facts:

```powershell
dotnet run --project tools/Obelisk.ContextPack -- generate-template --repo .
```

- Review `_generated` changes for unexpected drift.
- Check whether new projects, packages, config areas, or auth changes require updates to the handwritten docs.
- Re-read `examples/create-user-flow.md` if account-management flow changed.
- Run the solution tests.
- Make sure `.github/copilot-instructions.md` and `AGENTS.md` still point to the correct entrypoint.
- Keep `docs/context` focused on backend/template truth; do not let unrelated project notes accumulate here.

## Ownership model

- Generated files are owned by the tool and refreshed from repository inputs.
- Handwritten files are owned by template maintainers.
- Brownfield comparison output should only be committed in repositories where brownfield extraction is being used.
