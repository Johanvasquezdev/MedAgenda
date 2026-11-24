# MedAgenda visual system

## Direction

**Appointment book** — a calm, precise medical scheduling interface that keeps the existing MedAgenda name and forest-green/emerald identity. The appointment is the organizing object: availability, chosen time, confirmation, and upcoming visits share one clear visual grammar.

- **Metaphor:** a well-kept appointment book used at a contemporary clinic.
- **Signature moment:** a selected time settles into a confirmed appointment card; the same date and status hierarchy carries through booking and appointment management.
- **Emotion:** calm, clear, and human.
- **Motion verb:** settle into place.
- **Shareable frame:** the MedAgenda wordmark beside a clean appointment card, with the green identity carrying the composition.

## Concept exploration

Scores are originality / fit / feasibility / portability (lower portability is more specific).

| Concept | Rule | Signature | Scores |
|---|---|---|---|
| Appointment book | Appointments and available times define the layout grammar on every surface. | A selected slot settles into a confirmed visit card. | 4 / 5 / 5 / 1 |
| Clinic wayfinding | Provider discovery uses clear directory routes and visual signposts. | A provider row reveals its location and available action. | 4 / 4 / 4 / 2 |
| Care timeline | A single time rail organizes visits and admin status views. | The next event receives focus as the timeline changes state. | 3 / 5 / 4 / 2 |

**Chosen:** Appointment book. It fits the product name and makes appointment state, rather than decoration, the signature element.

## Color and type

Keep the established pine and emerald identity. Use neutral solid surfaces to make type, schedules, and forms easier to read; do not introduce a new brand hue.

| Role | Value | Use |
|---|---|---|
| Forest | `#071510` | Existing dark landing-page ground |
| Pine | `#0B1F18` | Existing deep-green surface |
| Emerald | `#10B981` | Existing primary action and active state |
| Emerald light | `#34D399` | Existing highlights on dark surfaces |
| Surface | `#F8FBF9` | Neutral form and content surface |
| Ink | `#102019` | High-contrast text on light surfaces |

Use the existing Geist family loaded by `next/font/google`. Display and section headings use a compact 600–700 weight; interface copy stays 400–500 at 16px or larger; small labels are informational and remain high contrast. Do not add a decorative font.

## Layout, shape, and motion

- Keep the existing Next.js App Router, Spanish-language content, dark-mode support, and 8px spacing base.
- Use a 12-column desktop grid with a 24px gutter and the current centered content width. Collapse to a single-column reading order on mobile; prioritize 44px primary tap targets.
- Use 16px control/card radii, with a larger 24px radius only for major page sections. Avoid repeating the same card treatment for every content family.
- Build hierarchy with solid pine surfaces, clear dividers, whitespace, typography, and emerald states. Retain a restrained Three.js particle background behind the public hero and auth surfaces; render one static frame when reduced motion is preferred. Avoid glass blur and ornamental gradient washes.
- Keep motion to 140–220ms state changes with an ease-out curve. Respect `prefers-reduced-motion`; never delay the availability of forms or content.
- Keep the current workflows, labels, routes, and data behavior. Do not add medical claims, social proof, or credentials.

## Surface composition

```text
Public:     brand / clear care statement + appointment-book illustration → real insurer marks → real provider directory → account action
Auth:       brand + concise form in a split layout → responsive stacked form on mobile
Patient:    persistent navigation → current task / next appointment → supporting schedule and provider information
Admin:      persistent navigation → page title and action → status summary → searchable table or form
Booking:    provider and specialty → availability → selected time → confirmation; same date/status hierarchy throughout
```

## Logo and provider marks

- Use the generated MedAgenda calendar emblem with a medical cross and appointment check, paired with the rendered wordmark. It must work in navigation, auth, favicon, and compact mobile layouts.
- Do not include Claude or Codex marks. Remove sparkle and robot imagery from the product interface while keeping the existing chat/help capability.
- Use verified local assets for the named insurance providers: ARS Humano, ARS Universal, SeNaSa, and ARS Reservas. Preserve their own brand colors inside the logo file; do not recolor the marks.
- Give insurer marks generous display area on the public coverage page; use margin-trimmed, aspect-preserving derivatives without altering the source marks.
- The generic “Seguro Privado” option gets no invented logo. The API provider DTO has no logo field and the source tree contains no named hospitals; only exact verified brand matches may use a real logo. Other dynamic providers use a restrained initials fallback until a verified identity is available.
- Keep logo proportions, include meaningful alt text when the mark adds identity, and hide redundant decorative marks from assistive technology.

## Genericness check

The appointment-book metaphor, the calendar emblem, the existing pine/emerald palette, and the slot-to-appointment transition tie the system to MedAgenda's actual name and workflow. The visual refresh should feel recognizable from the existing product identity while replacing AI-template decoration with useful medical scheduling structure.

