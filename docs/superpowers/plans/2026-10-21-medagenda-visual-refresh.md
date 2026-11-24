# MedAgenda Web Visual Refresh Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Modernize every MedAgenda web surface around the approved appointment-book visual language while retaining its forest/emerald identity and existing behavior.

**Architecture:** Keep the existing Next.js App Router, Tailwind, shadcn, and Lucide stack. Add shared brand and provider-logo components, centralize visual tokens, then restyle public/auth, patient, and admin surfaces in route groups. Do not change service calls, routes, registration values, access rules, or appointment/payment behavior.

**Tech Stack:** Next.js 16, React 19, TypeScript, Tailwind CSS 4, shadcn/Radix, Lucide React.

**Spec:** `docs/superpowers/specs/2026-10-21-medagenda-visual-refresh-design.md`; visual contract: `DESIGN.md`.

## Global Constraints

- Retain the MedAgenda identity and existing forest-green/emerald color family.
- Do not place Claude or Codex branding in the interface or new design assets.
- Preserve existing product behavior, Spanish-language content, and the current Next.js app structure.
- Keep service calls, route paths, model values, access rules, form validation, appointment actions, and payment behavior unchanged.
- The generic “Seguro Privado” option gets no invented logo.
- Unknown dynamic clinics/hospitals use initials; only exact verified brand matches may use a real logo.
- Meet 4.5:1 contrast for normal copy, use 44px primary mobile targets, and obey `prefers-reduced-motion`.
- Do not add a runtime dependency.

## Review Focus

- **Provider identity:** named insurers show only their verified marks; “Seguro Privado” and unknown dynamic providers never inherit a false identity. Check canonical and case/spacing variants in the provider-logo component.
- **Data preservation:** registration still submits the original provider values, including `ars-humano`. Check the registration form after logo integration.
- **Unavailable data:** landing-page provider/doctor requests can fail or return empty arrays without losing the public entry actions or showing fabricated counts. Check the catalog fallback state.
- **Theme contrast:** core actions, status chips, inputs, and logos stay legible in dark and light themes. Check one representative page in each theme.
- **Small/reduced-motion view:** public, booking, and navigation layouts stay usable at 390px and with reduced motion enabled. Check these states in the final screenshot round.

---

### Task 1: Brand foundation and verified provider marks

**Files:**
- Create: `SGC.Web/sgc.web.client/sgc-medagenda/src/components/brand/BrandMark.tsx`
- Create: `SGC.Web/sgc.web.client/sgc-medagenda/src/components/brand/BrandLockup.tsx`
- Create: `SGC.Web/sgc.web.client/sgc-medagenda/src/components/brand/ProviderLogo.tsx`
- Create: `SGC.Web/sgc.web.client/sgc-medagenda/src/components/brand/providerLogos.ts`
- Create: `SGC.Web/sgc.web.client/sgc-medagenda/public/provider-logos/` assets for Humano, ARS Universal, SeNaSa, and ARS Reservas
- Create: `SGC.Web/sgc.web.client/sgc-medagenda/public/provider-logos/SOURCES.md`
- Modify: `SGC.Web/sgc.web.client/sgc-medagenda/public/icon.svg`
- Modify: `SGC.Web/sgc.web.client/sgc-medagenda/src/app/globals.css`
- Modify: `SGC.Web/sgc.web.client/sgc-medagenda/src/app/layout.tsx`
- Modify: `SGC.Web/sgc.web.client/sgc-medagenda/src/components/layout/Sidebar.tsx`
- Modify: `SGC.Web/sgc.web.client/sgc-medagenda/src/components/animations/Animatedsidebar.tsx`
- Modify: `SGC.Web/sgc.web.client/sgc-medagenda/src/components/animations/Animatedstatscard.tsx`

**Interfaces:**
- `BrandMark` accepts `className?: string` and `decorative?: boolean`; renders the generated calendar emblem and exposes a product label only when not decorative.
- `BrandLockup` accepts `className?: string` and `markSize?: "sm" | "md" | "lg"`; renders the mark and “MedAgenda” wordmark.
- `providerLogos.ts` exports `getProviderLogo(name: string): { src: string; alt: string } | null`, using exact normalized aliases for the four named insurers.
- `ProviderLogo` accepts `name: string` and `className?: string`; unknown names return a neutral initials mark with the provider name still present as text.

- [ ] Integrate the generated transparent calendar emblem with a medical cross and appointment check, then use it in `BrandLockup` and favicon contexts.
- [ ] Map the four verified insurer names and aliases to local logo assets; leave generic and unmatched names unmapped.
- [ ] Add the original source URLs and provider names to `public/provider-logos/SOURCES.md`; preserve each mark’s aspect ratio and intrinsic brand colors.
- [ ] Define the forest, pine, emerald, neutral surface, type, radius, and focus tokens in `globals.css`; retain Geist and dark-mode support.
- [ ] Point the favicon metadata and `public/icon.svg` at the new MedAgenda mark.
- [ ] Apply the shared brand lockup and green active-state identity in the shared sidebar and stats-card component so patient/admin routes start from the same base.
- [ ] Review the rendered generated logo and verified insurer marks in the browser; confirm the larger public marks do not distort or introduce extra brand colors.

### Task 2: Public landing and authentication

**Files:**
- Modify: `SGC.Web/sgc.web.client/sgc-medagenda/src/app/page.tsx`
- Modify: `SGC.Web/sgc.web.client/sgc-medagenda/src/app/(auth)/layout.tsx`
- Modify: `SGC.Web/sgc.web.client/sgc-medagenda/src/app/(auth)/login/page.tsx`
- Modify: `SGC.Web/sgc.web.client/sgc-medagenda/src/app/(auth)/registro/page.tsx`
- Modify: `SGC.Web/sgc.web.client/sgc-medagenda/src/app/(auth)/forgot-password/page.tsx`
- Modify: `SGC.Web/sgc.web.client/sgc-medagenda/src/app/(auth)/reset-password/page.tsx`

**Interfaces:** Uses `BrandLockup`, `ProviderLogo`, and the tokens from Task 1. Keep existing service calls, link destinations, labels, form state, and validation.

- [ ] Restyle the landing page with the approved appointment-book hierarchy; retain a restrained `ThreeBackground`, remove sparkle decoration, and keep current content and account actions.
- [ ] Replace repeated hospital-icon branding on auth pages with `BrandLockup`; use solid, readable surfaces and keep the existing form layout responsive.
- [ ] Replace the remaining `Bot` and `Sparkles` icons in auth feature/insurance lists with neutral, task-relevant medical or scheduling icons; retain the Three.js backdrop on auth surfaces and honor reduced-motion preferences.
- [ ] Render insurer marks in the insurance list and registration choices using `ProviderLogo`; keep “Seguro Privado” generic and leave every submitted value unchanged.
- [ ] Restyle login, registration, forgot-password, and reset-password controls, loading/error feedback, focus, and reduced-motion states without changing their behavior.
- [ ] Check the public catalog unavailable/empty state and all auth links at desktop and 390px; ensure entry actions remain visible if API data fails.

### Task 3: Patient portal, booking, and help

**Files:**
- Modify: `SGC.Web/sgc.web.client/sgc-medagenda/src/app/(paciente)/layout.tsx`
- Modify: `SGC.Web/sgc.web.client/sgc-medagenda/src/app/(paciente)/paciente/dashboard/page.tsx`
- Modify: `SGC.Web/sgc.web.client/sgc-medagenda/src/app/(paciente)/paciente/medicos/page.tsx`
- Modify: `SGC.Web/sgc.web.client/sgc-medagenda/src/app/(paciente)/paciente/citas/page.tsx`
- Modify: `SGC.Web/sgc.web.client/sgc-medagenda/src/app/(paciente)/paciente/pagos/page.tsx`
- Modify: `SGC.Web/sgc.web.client/sgc-medagenda/src/app/(paciente)/paciente/notificaciones/page.tsx`
- Modify: `SGC.Web/sgc.web.client/sgc-medagenda/src/app/(paciente)/paciente/settings/page.tsx`
- Modify: `SGC.Web/sgc.web.client/sgc-medagenda/src/app/(paciente)/paciente/checkout/[id]/page.tsx`
- Modify: `SGC.Web/sgc.web.client/sgc-medagenda/src/components/citas/CitaCard.tsx`
- Modify: `SGC.Web/sgc.web.client/sgc-medagenda/src/components/citas/AgendarCita.tsx`
- Modify: `SGC.Web/sgc.web.client/sgc-medagenda/src/components/medicos/MedicoCard.tsx`
- Modify: `SGC.Web/sgc.web.client/sgc-medagenda/src/components/chat/GlobalChatbotWidget.tsx`
- Modify: `SGC.Web/sgc.web.client/sgc-medagenda/src/components/pagos/CheckoutForm.tsx`
- Modify: `SGC.Web/sgc.web.client/sgc-medagenda/src/components/notificaciones/NotificacionCard.tsx`
- Modify: `SGC.Web/sgc.web.client/sgc-medagenda/src/components/settings/UserSettingsForm.tsx`

**Interfaces:** Uses the shared brand, provider-logo, and CSS tokens from Task 1. Keep current DTOs and service behavior.

- [ ] Restyle the patient shell and sidebar with one MedAgenda-green identity, clear active-route state, visible mobile navigation, and the existing user/logout actions.
- [ ] Rework the patient dashboard and appointment/provider cards so upcoming visit date, time, status, and next action read in the same order.
- [ ] Restyle specialty/provider discovery and appointment booking around the selection-to-confirmation sequence; preserve slot selection, modal behavior, and API requests.
- [ ] Restyle payments, checkout, notifications, and settings with consistent forms, tables/cards, loading/error states, focus, and success feedback.
- [ ] Replace the chatbot’s robot/sparkle treatment with a neutral help/conversation mark and a clear Spanish label; preserve message handling and endpoints.
- [ ] Check one appointment-selection path, one form submission path, mobile navigation, dark/light contrast, and reduced-motion behavior in the browser.

### Task 4: Administrator portal and shared page patterns

**Files:**
- Modify: `SGC.Web/sgc.web.client/sgc-medagenda/src/app/(admin)/admin/dashboard/page.tsx`
- Modify: `SGC.Web/sgc.web.client/sgc-medagenda/src/app/(admin)/admin/layout.tsx`
- Modify: `SGC.Web/sgc.web.client/sgc-medagenda/src/app/(admin)/admin/citas/page.tsx`
- Modify: `SGC.Web/sgc.web.client/sgc-medagenda/src/app/(admin)/admin/usuarios/page.tsx`
- Modify: `SGC.Web/sgc.web.client/sgc-medagenda/src/app/(admin)/admin/medicos/page.tsx`
- Modify: `SGC.Web/sgc.web.client/sgc-medagenda/src/app/(admin)/admin/disponibilidad/page.tsx`
- Modify: `SGC.Web/sgc.web.client/sgc-medagenda/src/app/(admin)/admin/especialidades/page.tsx`
- Modify: `SGC.Web/sgc.web.client/sgc-medagenda/src/app/(admin)/admin/proveedores/page.tsx`
- Modify: `SGC.Web/sgc.web.client/sgc-medagenda/src/app/(admin)/admin/pagos/page.tsx`
- Modify: `SGC.Web/sgc.web.client/sgc-medagenda/src/app/(admin)/admin/auditoria/page.tsx`
- Modify: `SGC.Web/sgc.web.client/sgc-medagenda/src/app/(admin)/admin/settings/page.tsx`

**Interfaces:** Reuses the shared shell and tokens from Tasks 1 and 3. Preserve administrator links, service calls, table fields, dialogs, and mutations.

- [ ] Replace incidental violet accents with the existing MedAgenda green identity; remove glass/gradient page headers and keep page title, primary action, and status summary easy to find.
- [ ] Apply consistent search/filter, table, dialog, and form styling across appointments, users, doctors, availability, specialties, providers, payments, audit, and settings.
- [ ] Keep provider management data-driven; use the verified provider logo map only for exact known insurer names and initials for all unrecognized records.
- [ ] Check administrator navigation, a searchable list, a create/edit dialog, loading/empty/error states, and 390px layout without altering data operations.

### Task 5: Production visual review and final correction pass

**Files:** All frontend files changed above.

**Interfaces:** Runs after Tasks 1–4.

- [ ] Create a production build using the existing Next.js package scripts; resolve build errors without changing product behavior.
- [ ] Capture and inspect desktop and 390px mobile views for the public landing page, one auth page, patient dashboard/booking, and administrator list/form.
- [ ] Inspect dark and light theme, reduced-motion state, interactive focus, contrast, overflow, image loading/fallback, and visible API failure states.
- [ ] Apply one batch of visual corrections based on those captures, then take one final capture round of the same surfaces.
- [ ] Confirm the final page set contains no Claude/Codex marks or sparkle/robot imagery; confirm the Three.js backdrop and larger verified insurer marks render clearly, with generic provider fallbacks intact.

## Execution note

Use the Native single-agent approach after the user approves this plan. The work shares a single visual token and brand-component foundation across the route groups, so keeping implementation in one session reduces interface drift. Do not modify the existing unrelated `SGC.sln` change.

