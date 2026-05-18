# Backlog: Figma/Stitch/v0 + DigitalOcean/Vercel

This backlog starts the product-design-to-deployment track for MedAgenda web.

## Scope

- Design workflow: Figma + Stitch + v0.
- Deployment workflow: Vercel (frontend) + DigitalOcean (API/runtime services).
- Output: executable backlog with acceptance criteria and phased delivery.

## Goals

- Establish a repeatable design-to-code pipeline.
- Reduce UI drift between design and implementation.
- Deploy web + API with clear environments (dev/staging/prod).
- Keep auth, payments, and CORS posture production-safe.

## Epics

### EPIC 1 - Design System Foundation (Figma)

1. Create Figma libraries for tokens (color, type, spacing, radius, shadows).
2. Define core components: buttons, inputs, cards, table, modal, tabs, toast.
3. Define role-based templates: Paciente, Medico, Admin layouts.
4. Establish naming + versioning conventions (`v1`, `v1.1`, etc.).

Acceptance criteria:

- Every major existing page maps to a Figma frame.
- Tokens are exported and traceable to implementation variables.
- Components include desktop + mobile variants.

### EPIC 2 - AI-Assisted UI Prototyping (Stitch + v0)

1. Create prompt templates for consistent generation style.
2. Generate first-pass screens in Stitch for new/legacy pages.
3. Generate implementation-oriented variants with v0 for React/Next.
4. Run design QA checklist (a11y, responsiveness, role workflows).

Acceptance criteria:

- At least one approved flow per actor (Paciente, Medico, Admin).
- Generated output is converted into reusable components, not one-off pages.
- Variants are documented with rationale (chosen vs discarded).

### EPIC 3 - Frontend Integration Track (Next.js)

1. Define component ingestion process from v0 output into repo structure.
2. Map generated UI to current routes and API contracts.
3. Add visual parity checklist against Figma source.
4. Add regression checks: lint + typecheck + smoke routes.

Acceptance criteria:

- No hardcoded local endpoints in runtime code.
- `NEXT_PUBLIC_API_URL`/`NEXT_PUBLIC_API_BASE_URL` documented and required.
- New UI compiles cleanly with existing auth and SignalR hooks.

### EPIC 4 - Deployment Architecture (Vercel + DigitalOcean)

1. Vercel setup for web app (envs, preview deployments, prod domain).
2. DigitalOcean target for API (App Platform or Droplet + Docker).
3. Managed/Postgres + Redis connectivity and secure secret injection.
4. DNS and CORS alignment across environments.

Acceptance criteria:

- Preview environment available per PR for web.
- API staging environment reachable from Vercel preview.
- Production domains configured with HTTPS and working CORS allowlist.

### EPIC 5 - Release and Operations Readiness

1. Define environment matrix (`dev`, `staging`, `prod`).
2. Add deployment runbooks (rollback, smoke tests, health checks).
3. Add release gates (build, tests, lint, smoke).
4. Add observability handoff (logs, correlation id, alert basics).

Acceptance criteria:

- One-command deployment path documented for staging.
- Rollback path documented and tested.
- Health endpoint + basic smoke suite required before promote-to-prod.

## Prioritized Execution Order

1. EPIC 1 (Design System Foundation)
2. EPIC 4 (Deployment Architecture baseline)
3. EPIC 2 (Stitch + v0 prototyping)
4. EPIC 3 (Frontend integration)
5. EPIC 5 (Release readiness hardening)

## Sprint 0 (Immediate Next Tasks)

1. Create Figma file structure and token baseline.
2. Define Stitch/v0 prompt packs for 3 role dashboards.
3. Configure Vercel project with required env vars.
4. Choose DigitalOcean runtime mode (App Platform vs Droplet) and document decision.
5. Create staging CORS/domain matrix and map into `Cors:AllowedOrigins`.

## Dependencies / Inputs Needed

- Final domain names for staging and production.
- DigitalOcean resource preference (managed services vs self-managed containers).
- Figma ownership/workspace for component library governance.
- Stripe and JWT production secret management strategy.

## Risks to Track

- Design drift if generated UI bypasses shared component layer.
- CORS and mixed-origin issues across preview/staging/prod.
- Environment mismatch between Vercel frontend and DO API.
- Over-reliance on generated code without accessibility review.

## Definition of Done (for this backlog track)

- Figma -> Stitch/v0 -> repo integration path documented and repeatable.
- Vercel preview + production deployments running with correct envs.
- DigitalOcean API runtime stable with health checks and logs.
- Staging end-to-end flow validated: login, appointments, SignalR events, payment intent creation.
