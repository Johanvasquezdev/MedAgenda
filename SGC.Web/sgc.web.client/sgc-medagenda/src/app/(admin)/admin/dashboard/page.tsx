"use client";

import { useEffect, useState } from "react";
import {
  Users,
  Stethoscope,
  ClipboardList,
  Building2,
  ArrowUpRight,
  ShieldAlert,
  CalendarDays,
  UserCog,
  ChevronRight,
} from "lucide-react";
import Link from "next/link";
import { UsuarioService } from "@/services/usuario.service";
import { MedicoService } from "@/services/medico.service";
import { EspecialidadService } from "@/services/especialidad.service";
import { ProveedorSaludService } from "@/services/proveedor.service";
import { AnimatedStatsCard } from "@/components/animations/Animatedstatscard";
import { AnimatedList, AnimatedCard, usePageTransition } from "@/components/animations/Animatedcomponents";

export default function AdminDashboard() {
  const [loading, setLoading] = useState(true);
  const [pacientes, setPacientes] = useState(0);
  const [medicosActivos, setMedicosActivos] = useState(0);
  const [especialidades, setEspecialidades] = useState(0);
  const [proveedores, setProveedores] = useState(0);

  useEffect(() => {
    const cargarStats = async () => {
      try {
        const [p, m, e, prov] = await Promise.all([
          UsuarioService.obtenerTodos("Paciente"),
          MedicoService.obtenerTodos(),
          EspecialidadService.obtenerTodas(),
          ProveedorSaludService.obtenerTodos(),
        ]);

        setPacientes(p.length);
        setMedicosActivos(m.filter((med) => med.activo).length);
        setEspecialidades(e.length);
        setProveedores(prov.length);
      } catch {
        // Fallback en caso de error
      } finally {
        setLoading(false);
      }
    };

    cargarStats();
  }, []);

  usePageTransition();

  return (
    <div className="mx-auto max-w-7xl space-y-8 page-content">
      <header className="rounded-2xl border border-border bg-card p-6 md:p-7 shadow-sm">

        <div className="relative z-10 flex flex-col gap-6 md:flex-row md:items-end md:justify-between">
          <div>
            <p className="text-[10px] font-black uppercase tracking-[0.3em] text-emerald-600/80 dark:text-emerald-400/80">
              Centro de Operaciones
            </p>
            <h1 className="mt-2 text-3xl font-black tracking-tight text-foreground md:text-4xl">
              Panel de Control General
            </h1>
            <p className="mt-2 max-w-2xl text-muted-foreground font-medium">
              Administración unificada de citas, médicos y auditoría de MedAgenda.
            </p>
          </div>

          <Link href="/admin/citas" className="inline-flex min-h-11 shrink-0 items-center justify-center gap-2 rounded-xl bg-primary px-4 text-sm font-semibold text-primary-foreground transition hover:brightness-110 focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-ring">
            Revisar agenda <ArrowUpRight className="size-4" aria-hidden="true" />
          </Link>
        </div>
      </header>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <AnimatedStatsCard
          title="Pacientes Activos"
          value={loading ? 0 : pacientes}
          icon={Users}
          description="Total Usuarios"
          delay={100}
          variant="cyan"
        />
        <AnimatedStatsCard
          title="Médicos Activos"
          value={loading ? 0 : medicosActivos}
          icon={Stethoscope}
          description="En Directorio"
          delay={200}
          variant="emerald"
        />
        <AnimatedStatsCard
          title="Especialidades"
          value={loading ? 0 : especialidades}
          icon={ClipboardList}
          description="Registradas"
          delay={300}
          variant="emerald"
        />
        <AnimatedStatsCard
          title="Proveedores (ARS)"
          value={loading ? 0 : proveedores}
          icon={Building2}
          description="Asociados"
          delay={400}
          variant="amber"
        />
      </div>

      <div className="grid gap-6">
        <div className="space-y-4">
          <AnimatedList className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
            <AnimatedCard>
              <Link
                href="/admin/usuarios"
                className="group flex h-full flex-col rounded-2xl border border-border bg-card/50 p-5 shadow-sm transition-all hover:shadow-md hover:border-emerald-500/40 hover:bg-card"
              >
                <div className="mb-3 flex h-11 w-11 items-center justify-center rounded-xl bg-emerald-500/15 text-emerald-600 dark:text-emerald-300">
                  <UserCog className="h-5 w-5" />
                </div>
                <p className="font-semibold text-foreground">Gestion de usuarios</p>
                <p className="mt-1 text-sm text-muted-foreground mb-4 flex-1">Pacientes, administradores y roles</p>
                <div className="flex items-center gap-1 text-sm font-medium text-emerald-600 dark:text-emerald-300">
                  Abrir modulo <ChevronRight className="h-4 w-4" />
                </div>
              </Link>
            </AnimatedCard>

            <AnimatedCard>
              <Link
                href="/admin/auditoria"
                className="group flex h-full flex-col rounded-2xl border border-border bg-card/50 p-5 shadow-sm transition-all hover:shadow-md hover:border-rose-500/40 hover:bg-card"
              >
                <div className="mb-3 flex h-11 w-11 items-center justify-center rounded-xl bg-rose-500/15 text-rose-600 dark:text-rose-300">
                  <ShieldAlert className="h-5 w-5" />
                </div>
                <p className="font-semibold text-foreground">Auditoria y seguridad</p>
                <p className="mt-1 text-sm text-muted-foreground mb-4 flex-1">Trazabilidad completa del sistema</p>
                <div className="flex items-center gap-1 text-sm font-medium text-rose-600 dark:text-rose-300">
                  Revisar eventos <ChevronRight className="h-4 w-4" />
                </div>
              </Link>
            </AnimatedCard>

            <AnimatedCard>
              <Link
                href="/admin/citas"
                className="group flex h-full flex-col rounded-2xl border border-border bg-card/50 p-5 shadow-sm transition-all hover:shadow-md hover:border-emerald-500/40 hover:bg-card"
              >
                <div className="mb-3 flex h-11 w-11 items-center justify-center rounded-xl bg-emerald-500/15 text-emerald-600 dark:text-emerald-300">
                  <CalendarDays className="h-5 w-5" />
                </div>
                <p className="font-semibold text-foreground">Agenda global</p>
                <p className="mt-1 text-sm text-muted-foreground mb-4 flex-1">Control de citas, estados y turnos</p>
                <div className="flex items-center gap-1 text-sm font-medium text-emerald-600 dark:text-emerald-300">
                  Ver calendario <ChevronRight className="h-4 w-4" />
                </div>
              </Link>
            </AnimatedCard>

            <AnimatedCard>
              <Link
                href="/admin/medicos"
                className="group flex h-full flex-col rounded-2xl border border-border bg-card/50 p-5 shadow-sm transition-all hover:shadow-md hover:border-cyan-500/40 hover:bg-card"
              >
                <div className="mb-3 flex h-11 w-11 items-center justify-center rounded-xl bg-cyan-500/15 text-cyan-600 dark:text-cyan-300">
                  <Stethoscope className="h-5 w-5" />
                </div>
                <p className="font-semibold text-foreground">Directorio medico</p>
                <p className="mt-1 text-sm text-muted-foreground mb-4 flex-1">Gestion de medicos y especialidades</p>
                <div className="flex items-center gap-1 text-sm font-medium text-cyan-600 dark:text-cyan-300">
                  Gestionar perfiles <ChevronRight className="h-4 w-4" />
                </div>
              </Link>
            </AnimatedCard>
          </AnimatedList>
        </div>

      </div>
    </div>
  );
}
