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
- `BrandMark` accepts `className?: string` and `decorative?: boolean`; renders the custom calendar monogram and exposes a product label only when not decorative.
- `BrandLockup` accepts `className?: string` and `markSize?: "sm" | "md" | "lg"`; renders the mark and “MedAgenda” wordmark.
- `providerLogos.ts` exports `getProviderLogo(name: string): { src: string; alt: string } | null`, using exact normalized aliases for the four named insurers.
- `ProviderLogo` accepts `name: string` and `className?: string`; unknown names return a neutral initials mark with the provider name still present as text.

- [ ] Create the calendar monogram as a hand-authored SVG/React mark with a small medical cross, then use it in `BrandLockup`.
- [ ] Map the four verified insurer names and aliases to local logo assets; leave generic and unmatched names unmapped.
- [ ] Add the original source URLs and provider names to `public/provider-logos/SOURCES.md`; preserve each mark’s aspect ratio and intrinsic brand colors.
- [ ] Define the forest, pine, emerald, neutral surface, type, radius, and focus tokens in `globals.css`; retain Geist and dark-mode support.
- [ ] Point the favicon metadata and `public/icon.svg` at the new MedAgenda mark.
- [ ] Apply the shared brand lockup and green active-state identity in the shared sidebar and stats-card component so patient/admin routes start from the same base.
- [ ] Review the rendered logo variants and the `ProviderLogo` known/unknown cases in a small browser preview; confirm the marks do not distort or introduce extra brand colors.


