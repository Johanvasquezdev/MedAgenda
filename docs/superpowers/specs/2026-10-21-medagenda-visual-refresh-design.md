# MedAgenda web visual refresh

## Goal

Modernize the complete MedAgenda web frontend while retaining its existing name, forest-green/emerald identity, Spanish-language product context, and current route behavior. The main visual change is to replace generic AI-coded decoration with a useful medical scheduling language organized around the appointment book.

## Confirmed requirements

- Refresh the public landing page, authentication pages, patient portal, and administrator portal.
- Retain the MedAgenda identity and existing green color family.
- Add a distinctive MedAgenda logo.
- Do not place Claude or Codex branding in the interface or new design assets.
- Use actual insurer/hospital logos where there is an identifiable and verifiable provider; do not invent marks for generic or unknown data.
- Preserve existing product behavior. The chat/help capability remains available but loses the bot/sparkle visual treatment.
- Keep Spanish-language content and the current Next.js app structure.

## Repository findings

- The landing page hard-codes ARS Humano, ARS Universal, SeNaSa, ARS Reservas, and the generic “Seguro Privado” option.
- Registration sends stable values (`ars-humano`, `ars-universal`, `ars-senasa`, and `ars-reservas`) to the existing application flow.
- Provider records are fetched at runtime. `ProveedorSaludDTO` has no logo or image field, and the repository contains no specific hospital names to verify.
- The current landing/auth surfaces use a Three.js particle backdrop; the app uses dark mode by default and emerald accents in the patient portal. The administrator portal has inconsistent violet accents.
- No Claude or Codex brand marks are present in the current frontend source. The decorative `Sparkles` and `Bot` icons are present.

## Proposed design

Use the approved **Appointment book** concept. Keep the current deep-green/emerald color family and dark-mode option. Use solid, legible surfaces, the existing Geist family, a consistent appointment/status grammar, and short state transitions. Retain the existing Three.js particle background as a restrained layer behind the public hero and auth screens, with a static render for reduced-motion preferences. Remove decorative gradient washes, sparkle marks, and robot imagery. Align incidental admin accent colors with MedAgenda green to make the existing brand consistent across all routes.

The bespoke logo is a generated calendar emblem with a medical cross and appointment check, paired with a rendered MedAgenda wordmark. Give verified insurer marks enough display area to read clearly while preserving their original proportions. Booking availability and confirmed visits share the same date, time, and status hierarchy. Patient and admin page layouts retain their navigation and functions while gaining consistent content widths, page headers, controls, tables, dialogs, feedback, and mobile behavior.

## Provider logo sourcing

Use local copies of official or government-hosted source assets with a source ledger in the frontend public assets folder:

- Humano's official site exposes an ARS logo asset at `https://www.humano.com.do/Style%20Library/humano/imagenes/logo_ars.png`.
- Grupo Universal's product repository hosts its ARS logo at `https://promociones.universal.com.do/hubfs/universal-assets-ars-1-800x600.png`.
- SeNaSa's official portal exposes its logo at `https://arssenasa2.gob.do/Intranet/cdn/img/logoSenasa.jpg`.
- The SISALRIL regulator page exposes the ARS Reservas logo at `https://www.sisalril.gob.do/app/uploads/2025/08/reservas.png`.

Use these only to identify the corresponding provider, preserve their proportions, and avoid implying endorsement. “Seguro Privado” remains unbranded. Unknown dynamic clinics/hospitals use initials; the data model does not currently carry a trusted image URL.

## Scope

Restyle shared tokens, public/auth layouts, patient/admin shells, reusable navigation and controls, route-level headings/cards/forms/tables, provider presentation, and the logo/icon usage across the frontend. Keep service calls, route paths, model values, access rules, form validation, appointment actions, and payment behavior unchanged. Do not redesign the separate DoctorApp.

## Accessibility and responsive behavior

- Retain semantic landmarks and keyboard access; add explicit accessible labels for icon-only controls and visible focus states.
- Meet 4.5:1 text contrast for normal copy and 44px target sizing for primary mobile actions.
- Ensure forms and feedback remain usable without motion; obey reduced-motion preferences.
- At 390px wide, use a deliberate single-column layout without horizontal overflow; preserve the key appointment selection/confirmation interaction.

## Implementation approach

Keep the existing Next.js/Tailwind/shadcn stack. Use shared brand and provider-logo components so marks render consistently. Centralize design tokens in `globals.css`; restyle the shared shells before updating route-specific surfaces. Source real logos into `public` with attribution URLs, map exact known provider names, and retain stable registration values. Avoid adding a new runtime dependency.

## Risks and open facts

- Hospital/provider names arrive from the runtime API and are not in the repository, so no specific hospital logo can be responsibly mapped without a name match.
- “ARS Humano” is the current UI label/value. The official Group Humano site currently describes its health-risk administrator as ARS Primera. This proposal preserves the UI label and backend value for compatibility; no product-name copy change is included.
- The existing frontend has broad inline styling across routes, so the styling pass must check representative public, auth, patient, admin, and booking flows for consistency.

## Acceptance criteria

- Every web route uses the retained forest/emerald MedAgenda identity and a consistent, medical appointment-oriented visual system.
- The custom logo appears consistently in public, auth, patient, admin, and favicon contexts.
- No Claude/Codex branding, sparkle decoration, or robot imagery remains; the existing Three.js particle background remains visible in the public hero and auth surfaces.
- Actual insurer logos are used only for verified exact-name matches; generic/unknown providers do not receive invented marks.
- Existing workflows and backend values are unchanged.
- Desktop and mobile views remain readable, keyboard accessible, and usable with reduced motion.
