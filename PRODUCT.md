# Product

<!-- impeccable:product-schema 1 -->

## Platform

web

## Stack

Next.js and React frontend in `SGC.Web/sgc.web.client/sgc-medagenda`; ASP.NET Core API; PostgreSQL.

## Users

- Patients use the web portal to find providers, schedule and manage appointments, and handle related notifications and payments.
- Administrators manage users, doctors, specialties, providers, availability, appointments, payments, and audit records.
- Clinicians are part of the wider product audience; the repository describes a separate DoctorApp and clinician workflows.

## Product Purpose

MedAgenda centralizes medical appointment scheduling and its supporting patient and administrative workflows, reducing scheduling conflicts and administrative effort. The public site introduces the service and routes people into registration or sign-in.

## Positioning

The product brings provider discovery, appointment scheduling, reminders, notifications, and payment management into one appointment service. The repository lists a virtual assistant as an included capability.

## Operating Context

The web client is Spanish-language and serves a Dominican Republic academic project. The web routes include a public landing page, authentication pages, patient pages, and administrator pages. The user confirmed the visual refresh should cover all web surfaces.

## Capabilities and Constraints

- Preserve the existing public, authentication, patient, and administrator workflows during the visual refresh.
- The project supports provider discovery, scheduling, appointment management, insurance selection, payments, notifications, and virtual-assistant chat.
- Full clinical records are outside the documented project scope.
- Keep factual claims and provider identities grounded in the data and project documentation; do not invent metrics, endorsements, or trust certifications.

## Brand Commitments

The product name is MedAgenda. The user asked to retain its existing identity and green color family, modernize the interface, remove Claude and Codex branding from the design, and give MedAgenda a distinctive medical scheduling logo.

## Evidence on Hand

- Project feature and audience description: `README.md`.
- Provider and insurance names in the web client: `SGC.Web/sgc.web.client/sgc-medagenda/src/app/page.tsx` and `src/app/(auth)/registro/page.tsx`.
- Provider records are loaded dynamically through `src/services/proveedor.service.ts`; the current DTO has no logo field.
- The visible named insurance options are ARS Humano, ARS Universal, SENASA, and ARS Reservas. “Seguro Privado” is generic rather than a verifiable provider brand.

## Product Principles

- Make finding and booking care straightforward.
- Keep appointment status, timing, and next actions easy to scan.
- Preserve existing product behavior during visual changes.
- Use real provider identities only when their names match a verified brand.

