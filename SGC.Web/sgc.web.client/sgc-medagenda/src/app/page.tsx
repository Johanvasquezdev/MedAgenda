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
  HeartPulse,
  Hospital,
  MapPin,
  ShieldCheck,
  Sparkles,
  Stethoscope,
  UserRoundCheck,
} from "lucide-react";
import { ThreeBackground } from "@/components/animations/Threebackground";
import { ProveedorSaludDTO, ProveedorSaludService } from "@/services/proveedor.service";
import { MedicoService } from "@/services/medico.service";
import type { MedicoDTO } from "@/types/api.types";

const seguros = [
  {
    nombre: "ARS Humano",
    prefijo: "HUM",
    descripcion: "Cobertura privada con procesos rapidos y seguimiento digital.",
    icono: HeartPulse,
    accent: "from-sky-500/20 to-cyan-500/20 text-sky-100 border-sky-400/30",
  },
  {
    nombre: "ARS Universal",
    prefijo: "UNI",
    descripcion: "Afiliaciones flexibles para pacientes individuales y familias.",
    icono: Sparkles,
    accent: "from-violet-500/20 to-indigo-500/20 text-violet-100 border-violet-400/30",
  },
  {
    nombre: "SENASA",
    prefijo: "SEN",
    descripcion: "Acceso integrado para usuarios del sistema publico de salud.",
    icono: ShieldCheck,
    accent: "from-emerald-500/20 to-teal-500/20 text-emerald-100 border-emerald-400/30",
  },
  {
    nombre: "ARS Reservas",
    prefijo: "ARS",
    descripcion: "Cobertura respaldada para consultas, agendas y pagos en linea.",
    icono: Building2,
    accent: "from-amber-500/20 to-orange-500/20 text-amber-100 border-amber-400/30",
  },
  {
    nombre: "Seguro Privado",
    prefijo: "PRI",
    descripcion: "Registro rapido para seguros corporativos o planes personalizados.",
    icono: CircleDollarSign,
    accent: "from-rose-500/20 to-fuchsia-500/20 text-rose-100 border-rose-400/30",
  },
];

const flujo = [
  {
    paso: "01",
    titulo: "Crea tu perfil",
    descripcion: "Registrate con tus datos de acceso, seguro medico y contacto principal.",
    icono: UserRoundCheck,
  },
  {
    paso: "02",
    titulo: "Explora medicos y centros",
    descripcion: "Revisa especialistas, hospitales aliados y disponibilidad antes de agendar.",
    icono: Stethoscope,
  },
  {
    paso: "03",
    titulo: "Agenda y confirma",
    descripcion: "Selecciona horario, confirma tu cita y recibe recordatorios automaticos.",
    icono: CalendarDays,
  },
  {
    paso: "04",
    titulo: "Gestiona tu seguimiento",
    descripcion: "Consulta notificaciones, pagos y cambios desde tu portal personal.",
    icono: BellRing,
  },
];

const beneficios = [
  "Agenda desde cualquier dispositivo",
  "Recibe recordatorios y cambios en tiempo real",
  "Administra seguros, pagos y proximas citas",
  "Descubre especialistas y centros de salud disponibles",
];

export default function HomePage() {
  const [proveedores, setProveedores] = useState<ProveedorSaludDTO[]>([]);
  const [medicos, setMedicos] = useState<MedicoDTO[]>([]);
  const [catalogoDisponible, setCatalogoDisponible] = useState(true);

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
        if (!active) return;
        setCatalogoDisponible(false);
      }
    };

    loadCatalog();

    return () => {
      active = false;
    };
  }, []);

  const especialidades = useMemo(() => {
    const unique = medicos
      .map((medico) => medico.especialidadNombre)
      .filter((value): value is string => Boolean(value && value.trim()));

    return Array.from(new Set(unique)).slice(0, 8);
  }, [medicos]);

  return (
    <main className="min-h-screen overflow-hidden bg-[#071510] text-white">
      <section className="relative isolate overflow-hidden">
        <div className="absolute inset-0 bg-[radial-gradient(circle_at_top_left,_rgba(16,185,129,0.2),_transparent_42%),radial-gradient(circle_at_bottom_right,_rgba(6,78,59,0.55),_transparent_46%),linear-gradient(135deg,_#071510_10%,_#0b1f18_45%,_#071510_100%)]" />
        <div className="absolute inset-0 opacity-70">
          <ThreeBackground />
        </div>

        <div className="relative z-10 mx-auto max-w-7xl px-6 py-8 lg:px-10 lg:py-10">
          <header className="mb-14 flex flex-col gap-6 rounded-[2rem] border border-white/10 bg-white/5 px-6 py-5 backdrop-blur-xl lg:flex-row lg:items-center lg:justify-between">
            <div className="flex items-center gap-4">
              <div className="flex h-14 w-14 items-center justify-center rounded-2xl bg-emerald-500 shadow-[0_20px_60px_rgba(16,185,129,0.35)]">
                <Hospital className="h-7 w-7 text-white" />
              </div>
              <div>
                <p className="text-2xl font-black tracking-tight">MedAgenda</p>
                <p className="text-sm text-emerald-100/70">Portal de gestion clinica y agenda medica</p>
              </div>
            </div>

            <div className="flex flex-wrap items-center gap-3">
              <Link
                href="/registro"
                className="rounded-full border border-white/15 px-5 py-2.5 text-sm font-semibold text-white transition hover:bg-white/10"
              >
                Crear cuenta
              </Link>
              <Link
                href="/login"
                className="inline-flex items-center gap-2 rounded-full bg-emerald-500 px-5 py-2.5 text-sm font-semibold text-white shadow-[0_16px_40px_rgba(16,185,129,0.35)] transition hover:bg-emerald-400"
              >
                Iniciar sesion
                <ArrowRight className="h-4 w-4" />
              </Link>
            </div>
          </header>

          <div className="grid gap-10 lg:grid-cols-[1.1fr_0.9fr] lg:items-center">
            <div>
              <div className="mb-6 inline-flex items-center gap-2 rounded-full border border-emerald-400/20 bg-emerald-500/10 px-4 py-2 text-xs font-bold uppercase tracking-[0.22em] text-emerald-100/85">
                <Sparkles className="h-3.5 w-3.5" />
                Todo tu flujo medico en un solo lugar
              </div>

              <h1 className="max-w-4xl text-5xl font-black leading-[1.05] tracking-tight text-white sm:text-6xl">
                Agenda, administra y da seguimiento a tu salud con una experiencia clara desde el primer clic.
              </h1>

              <p className="mt-6 max-w-2xl text-lg leading-8 text-emerald-50/78">
                MedAgenda centraliza registro, busqueda de medicos, seguros, recordatorios, pagos y seguimiento de citas
                para pacientes, medicos y equipos administrativos.
              </p>

              <div className="mt-8 grid gap-3 sm:grid-cols-2">
                {beneficios.map((beneficio) => (
                  <div
                    key={beneficio}
                    className="flex items-center gap-3 rounded-2xl border border-white/10 bg-white/6 px-4 py-3 text-sm text-emerald-50/86 backdrop-blur-md"
                  >
                    <CheckCircle2 className="h-4 w-4 shrink-0 text-emerald-300" />
                    <span>{beneficio}</span>
                  </div>
                ))}
              </div>

              <div className="mt-10 flex flex-wrap gap-4">
                <Link
                  href="/registro"
                  className="inline-flex items-center gap-2 rounded-full bg-white px-6 py-3 text-sm font-bold text-[#063d2d] transition hover:bg-emerald-50"
                >
                  Comenzar ahora
                  <ChevronRight className="h-4 w-4" />
                </Link>
                <Link
                  href="/login"
                  className="inline-flex items-center gap-2 rounded-full border border-white/15 px-6 py-3 text-sm font-semibold text-white transition hover:bg-white/8"
                >
                  Ya tengo cuenta
                </Link>
              </div>
            </div>

            <div className="grid gap-5">
              <div className="rounded-[2rem] border border-white/10 bg-white/8 p-6 shadow-[0_24px_60px_rgba(0,0,0,0.25)] backdrop-blur-xl">
                <p className="text-xs font-bold uppercase tracking-[0.24em] text-emerald-200/70">Resumen rapido</p>
                <div className="mt-5 grid gap-4 sm:grid-cols-2">
                  <article className="rounded-2xl border border-white/10 bg-black/20 p-5">
                    <p className="text-3xl font-black text-white">{seguros.length}</p>
                    <p className="mt-2 text-sm text-emerald-50/70">tipos de seguro listos para registro</p>
                  </article>
                  <article className="rounded-2xl border border-white/10 bg-black/20 p-5">
                    <p className="text-3xl font-black text-white">{proveedores.length || "--"}</p>
                    <p className="mt-2 text-sm text-emerald-50/70">centros y hospitales cargados</p>
                  </article>
                  <article className="rounded-2xl border border-white/10 bg-black/20 p-5">
                    <p className="text-3xl font-black text-white">{medicos.length || "--"}</p>
                    <p className="mt-2 text-sm text-emerald-50/70">medicos disponibles en el directorio</p>
                  </article>
                  <article className="rounded-2xl border border-white/10 bg-black/20 p-5">
                    <p className="text-3xl font-black text-white">{especialidades.length || "--"}</p>
                    <p className="mt-2 text-sm text-emerald-50/70">especialidades visibles en la plataforma</p>
                  </article>
                </div>
              </div>

              <div className="rounded-[2rem] border border-white/10 bg-gradient-to-br from-white/10 via-white/6 to-transparent p-6 backdrop-blur-xl">
                <p className="text-xs font-bold uppercase tracking-[0.24em] text-emerald-200/70">Como funciona</p>
                <div className="mt-5 space-y-4">
                  {flujo.map((item) => (
                    <div key={item.paso} className="flex gap-4 rounded-2xl border border-white/8 bg-black/15 p-4">
                      <div className="flex h-12 w-12 shrink-0 items-center justify-center rounded-2xl bg-emerald-500/15 text-emerald-200">
                        <item.icono className="h-5 w-5" />
                      </div>
                      <div>
                        <p className="text-xs font-black uppercase tracking-[0.22em] text-emerald-200/60">{item.paso}</p>
                        <h3 className="mt-1 text-lg font-bold text-white">{item.titulo}</h3>
                        <p className="mt-1 text-sm leading-6 text-emerald-50/72">{item.descripcion}</p>
                      </div>
                    </div>
                  ))}
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      <section className="mx-auto max-w-7xl px-6 py-20 lg:px-10">
        <div className="flex flex-col gap-3">
          <p className="text-xs font-black uppercase tracking-[0.24em] text-emerald-300/80">Seguros compatibles</p>
          <h2 className="text-3xl font-black tracking-tight text-white sm:text-4xl">
            El registro se adapta al seguro medico que uses.
          </h2>
          <p className="max-w-2xl text-base leading-7 text-white/64">
            Cada opcion en MedAgenda prepara automaticamente el prefijo del numero de seguro y mantiene el flujo claro
            para el paciente.
          </p>
        </div>

        <div className="mt-10 grid gap-5 md:grid-cols-2 xl:grid-cols-3">
          {seguros.map((seguro) => (
            <article key={seguro.nombre} className="rounded-[1.75rem] border border-white/8 bg-white/6 p-6 backdrop-blur-xl">
              <div className={`flex h-14 w-14 items-center justify-center rounded-2xl border bg-gradient-to-br ${seguro.accent}`}>
                <seguro.icono className="h-6 w-6" />
              </div>
              <div className="mt-5 flex items-center justify-between gap-3">
                <h3 className="text-xl font-bold text-white">{seguro.nombre}</h3>
                <span className="rounded-full border border-emerald-400/25 bg-emerald-500/12 px-3 py-1 text-xs font-black tracking-[0.2em] text-emerald-100">
                  {seguro.prefijo}
                </span>
              </div>
              <p className="mt-3 text-sm leading-6 text-white/65">{seguro.descripcion}</p>
            </article>
          ))}
        </div>
      </section>

      <section className="mx-auto grid max-w-7xl gap-8 px-6 pb-20 lg:grid-cols-[0.95fr_1.05fr] lg:px-10">
        <div className="rounded-[2rem] border border-white/8 bg-white/6 p-8 backdrop-blur-xl">
          <div className="flex items-center gap-3">
            <div className="flex h-12 w-12 items-center justify-center rounded-2xl bg-emerald-500/15 text-emerald-200">
              <Hospital className="h-5 w-5" />
            </div>
            <div>
              <p className="text-xs font-black uppercase tracking-[0.22em] text-emerald-300/75">Centros disponibles</p>
              <h2 className="text-2xl font-black tracking-tight text-white">Hospitales y proveedores aliados</h2>
            </div>
          </div>

          <div className="mt-6 space-y-3">
            {catalogoDisponible && proveedores.length > 0 ? (
              proveedores.map((proveedor) => (
                <div key={proveedor.id} className="flex items-start justify-between gap-4 rounded-2xl border border-white/8 bg-black/15 px-4 py-4">
                  <div>
                    <p className="font-bold text-white">{proveedor.nombre}</p>
                    <div className="mt-2 flex flex-wrap items-center gap-3 text-sm text-white/58">
                      <span className="inline-flex items-center gap-1.5">
                        <Building2 className="h-4 w-4 text-emerald-300/70" />
                        {proveedor.tipo || "Proveedor de salud"}
                      </span>
                      {proveedor.telefono ? (
                        <span className="inline-flex items-center gap-1.5">
                          <MapPin className="h-4 w-4 text-emerald-300/70" />
                          Contacto disponible
                        </span>
                      ) : null}
                    </div>
                  </div>
                  <span className="rounded-full bg-emerald-500/10 px-3 py-1 text-xs font-bold text-emerald-200">
                    Activo
                  </span>
                </div>
              ))
            ) : (
              <div className="rounded-2xl border border-dashed border-white/12 bg-black/10 p-5 text-sm leading-7 text-white/62">
                Estamos sincronizando el catalogo de centros en este entorno. La portada sigue lista y el portal
                continuara mostrando seguros, flujo y acceso al sistema mientras la API completa la carga.
              </div>
            )}
          </div>
        </div>

        <div className="rounded-[2rem] border border-white/8 bg-gradient-to-br from-white/7 via-white/4 to-transparent p-8 backdrop-blur-xl">
          <p className="text-xs font-black uppercase tracking-[0.22em] text-emerald-300/75">Especialidades y agenda</p>
          <h2 className="mt-2 text-2xl font-black tracking-tight text-white">Explora el directorio antes de iniciar sesion</h2>
          <p className="mt-3 max-w-2xl text-sm leading-7 text-white/64">
            La plataforma fue pensada para que el paciente entienda rapidamente que puede hacer: buscar especialistas,
            revisar centros aliados, agendar una cita y recibir seguimiento sin friccion.
          </p>

          <div className="mt-6 flex flex-wrap gap-3">
            {especialidades.length > 0 ? (
              especialidades.map((especialidad) => (
                <span
                  key={especialidad}
                  className="rounded-full border border-white/10 bg-black/18 px-4 py-2 text-sm font-semibold text-emerald-50/88"
                >
                  {especialidad}
                </span>
              ))
            ) : (
              <span className="rounded-full border border-white/10 bg-black/18 px-4 py-2 text-sm font-semibold text-emerald-50/70">
                El directorio se mostrara automaticamente cuando la API este disponible.
              </span>
            )}
          </div>

          <div className="mt-8 rounded-[1.75rem] border border-emerald-400/15 bg-emerald-500/10 p-6">
            <h3 className="text-lg font-black text-white">Listo para entrar al portal?</h3>
            <p className="mt-2 text-sm leading-6 text-emerald-50/72">
              Registra tu cuenta, selecciona tu seguro y empieza a gestionar citas, notificaciones y pagos en un mismo flujo.
            </p>
            <div className="mt-5 flex flex-wrap gap-3">
              <Link
                href="/login"
                className="inline-flex items-center gap-2 rounded-full bg-emerald-500 px-5 py-3 text-sm font-bold text-white transition hover:bg-emerald-400"
              >
                Ir al login
                <ArrowRight className="h-4 w-4" />
              </Link>
              <Link
                href="/registro"
                className="inline-flex items-center gap-2 rounded-full border border-white/10 px-5 py-3 text-sm font-semibold text-white transition hover:bg-white/8"
              >
                Crear una cuenta
              </Link>
            </div>
          </div>
        </div>
      </section>
    </main>
  );
}
