"use client";

import { useEffect, useMemo, useState } from "react";
import Link from "next/link";
import {
  ArrowRight,
  BellRing,
  Building2,
  CalendarDays,
  CheckCircle2,
  ChevronRight,
  CircleDollarSign,
  Clock3,
  HeartPulse,
  MapPin,
  ShieldCheck,
  Stethoscope,
  UserRoundCheck,
} from "lucide-react";
import { BrandLockup } from "@/components/brand/BrandLockup";
import { ProviderLogo } from "@/components/brand/ProviderLogo";
import { ThreeBackground } from "@/components/animations/Threebackground";
import { ProveedorSaludDTO, ProveedorSaludService } from "@/services/proveedor.service";
import { MedicoService } from "@/services/medico.service";
import type { MedicoDTO } from "@/types/api.types";

const seguros = [
  { nombre: "ARS Humano", descripcion: "Registra tu cobertura y consulta la disponibilidad desde tu cuenta." },
  { nombre: "ARS Universal", descripcion: "Selecciona tu plan al crear una cuenta personal o familiar." },
  { nombre: "SeNaSa", descripcion: "Accede al flujo de atención con tu seguro de salud." },
  { nombre: "ARS Reservas", descripcion: "Incluye la información de tu cobertura durante el registro." },
  { nombre: "Seguro Privado", descripcion: "Registra tu cobertura corporativa o un plan personalizado." },
];

const flujo = [
  { titulo: "Crea tu perfil", descripcion: "Regístrate con tus datos de acceso, seguro médico y contacto principal.", icono: UserRoundCheck },
  { titulo: "Explora médicos y centros", descripcion: "Revisa especialistas, proveedores y disponibilidad.", icono: Stethoscope },
  { titulo: "Agenda y confirma", descripcion: "Selecciona un horario y recibe los detalles de tu cita.", icono: CalendarDays },
  { titulo: "Gestiona tu seguimiento", descripcion: "Consulta notificaciones, pagos y cambios desde tu portal.", icono: BellRing },
];

const beneficios = [
  "Agenda desde cualquier dispositivo",
  "Recibe recordatorios y cambios en tiempo real",
  "Administra seguros, pagos y próximas citas",
  "Descubre especialistas y centros de salud disponibles",
];

export default function HomePage() {
  const [proveedores, setProveedores] = useState<ProveedorSaludDTO[]>([]);
  const [medicos, setMedicos] = useState<MedicoDTO[]>([]);
  const [catalogoDisponible, setCatalogoDisponible] = useState(true);
  const [cargandoCatalogo, setCargandoCatalogo] = useState(true);

  useEffect(() => {
    let active = true;

    const loadCatalog = async () => {
      try {
        const [proveedoresData, medicosData] = await Promise.all([
          ProveedorSaludService.obtenerTodos(),
          MedicoService.obtenerTodos(),
        ]);
        if (!active) return;
        setProveedores(proveedoresData.filter((item) => item.activo));
        setMedicos(medicosData.filter((item) => item.activo));
      } catch {
        if (active) setCatalogoDisponible(false);
      } finally {
        if (active) setCargandoCatalogo(false);
      }
    };

    loadCatalog();
    return () => { active = false; };
  }, []);

  const especialidades = useMemo(() => {
    const unique = medicos
      .map((medico) => medico.especialidadNombre)
      .filter((value): value is string => Boolean(value && value.trim()));
    return Array.from(new Set(unique)).slice(0, 8);
  }, [medicos]);

  return (
    <main className="min-h-screen overflow-hidden bg-background text-foreground">
      <header className="border-b border-border bg-card">
        <div className="mx-auto flex min-h-20 max-w-7xl flex-wrap items-center justify-between gap-4 px-5 py-4 sm:px-8">
          <Link href="/" aria-label="MedAgenda, inicio" className="rounded-md focus-visible:outline-2 focus-visible:outline-offset-4 focus-visible:outline-ring">
            <BrandLockup markSize="md" className="text-xl" />
          </Link>
          <nav aria-label="Acceso a la cuenta" className="flex items-center gap-2 sm:gap-3">
            <Link href="/login" className="inline-flex min-h-11 items-center rounded-xl px-4 text-sm font-semibold text-muted-foreground transition hover:bg-secondary hover:text-foreground">
              Iniciar sesión
            </Link>
            <Link href="/registro" className="inline-flex min-h-11 items-center gap-2 rounded-xl bg-primary px-4 text-sm font-semibold text-primary-foreground shadow-sm transition hover:brightness-110 focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-ring">
              Crear cuenta <ArrowRight className="size-4" aria-hidden="true" />
            </Link>
          </nav>
        </div>
      </header>

      <section className="relative isolate border-b border-border bg-[#0b261b] text-[#f2f8f3] dark:bg-[#0b261b]">
        <ThreeBackground />
        <div className="absolute inset-y-0 right-0 z-[1] hidden w-[42%] border-l border-white/10 bg-[#102f22]/80 backdrop-blur-[1px] lg:block" aria-hidden="true" />
        <div className="relative z-10 mx-auto grid max-w-7xl gap-12 px-5 py-16 sm:px-8 sm:py-20 lg:grid-cols-[1.15fr_0.85fr] lg:items-center lg:gap-16 lg:py-24">
          <div>
            <h1 className="max-w-3xl text-4xl font-semibold leading-[1.08] tracking-[-0.035em] sm:text-5xl lg:text-[3.75rem]">
              Tu próxima cita médica, organizada desde un solo lugar.
            </h1>
            <p className="mt-6 max-w-2xl text-base leading-7 text-emerald-50/85 sm:text-lg sm:leading-8">
              Encuentra especialistas, revisa la disponibilidad y gestiona tus citas, cobertura y pagos en MedAgenda.
            </p>
            <div className="mt-8 flex flex-wrap gap-3">
              <Link href="/registro" className="inline-flex min-h-12 items-center gap-2 rounded-xl bg-[#37bd87] px-5 text-sm font-semibold text-[#062419] shadow-[0_8px_24px_-12px_rgba(55,189,135,0.7)] transition hover:bg-[#5ad29f] focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-white">
                Agenda tu primera cita <ArrowRight className="size-4" aria-hidden="true" />
              </Link>
              <Link href="/login" className="inline-flex min-h-12 items-center rounded-xl border border-white/25 px-5 text-sm font-semibold text-white transition hover:bg-white/10 focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-white">
                Ya tengo cuenta
              </Link>
            </div>
            <ul className="mt-9 grid max-w-2xl gap-x-7 gap-y-3 sm:grid-cols-2">
              {beneficios.map((beneficio) => (
                <li key={beneficio} className="flex items-start gap-2.5 text-sm leading-6 text-emerald-50/90">
                  <CheckCircle2 className="mt-1 size-4 shrink-0 text-[#69d8a8]" aria-hidden="true" />
                  <span>{beneficio}</span>
                </li>
              ))}
            </ul>
          </div>

          <figure className="relative mx-auto w-full max-w-[22rem] overflow-hidden rounded-2xl border border-white/15 bg-[#071510] shadow-[0_24px_60px_-32px_rgba(0,0,0,0.8)]">
            <video
              aria-label="Anuncio de MedAgenda: una cita anotada en papel pasa a la agenda digital"
              className="aspect-[9/16] w-full object-cover"
              controls
              autoPlay
              muted
              loop
              playsInline
              preload="metadata"
              poster="/media/medagenda-ocho-y-media-poster.jpg"
            >
              <source src="/media/medagenda-ocho-y-media.mp4" type="video/mp4" />
              Tu navegador no puede reproducir este video. Encuentra y agenda tu cita en MedAgenda.
            </video>
            <figcaption className="sr-only">Tu próxima cita, a la vista. Encuentra tu médico y agenda tu cita en MedAgenda.</figcaption>
          </figure>
        </div>
      </section>

      <section aria-labelledby="seguros-title" className="mx-auto max-w-7xl px-5 py-16 sm:px-8 sm:py-20">
        <div className="max-w-2xl">
          <h2 id="seguros-title" className="text-3xl font-semibold tracking-tight sm:text-4xl">Regístrate con tu cobertura médica</h2>
          <p className="mt-4 text-base leading-7 text-muted-foreground">Selecciona tu seguro durante el registro y añade la información que necesitas para tu atención.</p>
        </div>
        <div className="mt-8 grid gap-x-8 md:grid-cols-2 xl:grid-cols-3">
          {seguros.map((seguro) => (
            <article key={seguro.nombre} className="flex flex-col items-start gap-3 border-b border-border py-5">
              {seguro.nombre === "Seguro Privado" ? (
                <div className="flex items-center gap-3">
                  <span aria-hidden="true" className="flex size-11 items-center justify-center rounded-xl bg-secondary text-secondary-foreground"><CircleDollarSign className="size-5" /></span>
                  <h3 className="font-semibold">Seguro Privado</h3>
                </div>
              ) : <ProviderLogo name={seguro.nombre} size="feature" />}
              <p className="text-sm leading-6 text-muted-foreground">{seguro.descripcion}</p>
            </article>
          ))}
        </div>
      </section>

      <section aria-labelledby="centros-title" className="border-y border-border bg-card">
        <div className="mx-auto grid max-w-7xl gap-12 px-5 py-16 sm:px-8 sm:py-20 lg:grid-cols-[0.8fr_1.2fr]">
          <div>
            <h2 id="centros-title" className="text-3xl font-semibold tracking-tight">Médicos y centros disponibles</h2>
            <p className="mt-4 text-base leading-7 text-muted-foreground">Consulta el directorio y conoce las especialidades disponibles.</p>
            <div className="mt-6 flex flex-wrap gap-2">
              {especialidades.length > 0 ? especialidades.map((especialidad) => (
                <span key={especialidad} className="rounded-lg border border-border bg-background px-3 py-2 text-sm font-medium">{especialidad}</span>
              )) : <p className="text-sm leading-6 text-muted-foreground">{cargandoCatalogo ? "Cargando el directorio…" : "Las especialidades aparecerán aquí cuando el directorio esté disponible."}</p>}
            </div>
            <Link href="/registro" className="mt-8 inline-flex min-h-11 items-center gap-2 rounded-xl bg-primary px-4 text-sm font-semibold text-primary-foreground transition hover:brightness-110">
              Crear una cuenta <ChevronRight className="size-4" aria-hidden="true" />
            </Link>
          </div>

          <div>
            <div className="flex items-center gap-3 border-b border-border pb-4">
              <span className="flex size-10 items-center justify-center rounded-xl bg-accent text-accent-foreground"><Building2 className="size-5" aria-hidden="true" /></span>
              <div>
                <h3 className="font-semibold">Centros de salud</h3>
                <p className="mt-1 text-sm text-muted-foreground">Proveedores activos en el directorio</p>
              </div>
            </div>
            <div className="divide-y divide-border">
              {catalogoDisponible && proveedores.length > 0 ? proveedores.map((proveedor) => (
                <article key={proveedor.id} className="flex items-center justify-between gap-4 py-4">
                  <ProviderLogo name={proveedor.nombre} />
                  <span className="inline-flex shrink-0 items-center gap-2 text-sm text-muted-foreground">
                    {proveedor.telefono ? <><MapPin className="size-4" aria-hidden="true" /> Contacto disponible</> : <><HeartPulse className="size-4" aria-hidden="true" /> {proveedor.tipo || "Proveedor de salud"}</>}
                  </span>
                </article>
              )) : (
                <div className="py-6 text-sm leading-6 text-muted-foreground">
                  {catalogoDisponible ? "Los centros activos aparecerán aquí cuando estén disponibles." : "No pudimos cargar el directorio ahora. Puedes crear tu cuenta o volver a intentarlo más tarde."}
                </div>
              )}
            </div>
            {medicos.length > 0 ? <p className="mt-4 flex items-center gap-2 text-sm text-muted-foreground"><Stethoscope className="size-4 text-primary" aria-hidden="true" />{medicos.length} profesionales disponibles</p> : null}
          </div>
        </div>
      </section>

      <footer className="mx-auto flex max-w-7xl flex-col gap-5 px-5 py-10 sm:flex-row sm:items-center sm:justify-between sm:px-8">
        <BrandLockup markSize="sm" className="text-base" />
        <div className="flex flex-wrap items-center gap-x-5 gap-y-2 text-sm text-muted-foreground">
          <span className="inline-flex items-center gap-2"><ShieldCheck className="size-4 text-primary" aria-hidden="true" />Gestión de citas y seguimiento</span>
          <span className="inline-flex items-center gap-2"><Clock3 className="size-4 text-primary" aria-hidden="true" />Acceso cuando lo necesites</span>
        </div>
      </footer>
    </main>
  );
}
