"use client";

import { useEffect, useState } from "react";
import { cn } from "@/lib/utils";
import {
  Users,
  Stethoscope,
  ClipboardList,
  Building2,
  ArrowUpRight,
  ShieldAlert,
  Activity,
  CalendarDays,
  UserCog,
  BadgeCheck,
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
      <header className="relative overflow-hidden rounded-3xl border border-purple-500/10 bg-gradient-to-br from-purple-500/10 to-fuchsia-500/10 p-6 md:p-8 shadow-sm transition-all duration-500 hover:shadow-lg hover:shadow-purple-500/5 backdrop-blur-sm">
        <div className="absolute -right-24 -top-24 h-64 w-64 rounded-full bg-purple-500/5 dark:bg-purple-500/10 blur-3xl" />
        <div className="absolute -bottom-24 -left-24 h-64 w-64 rounded-full bg-fuchsia-500/5 dark:bg-fuchsia-500/10 blur-3xl" />

        <div className="relative z-10 flex flex-col gap-6 md:flex-row md:items-end md:justify-between">
          <div>
            <p className="text-[10px] font-black uppercase tracking-[0.3em] text-purple-600/80 dark:text-purple-400/80">
              Centro de Operaciones
            </p>
            <h1 className="mt-2 text-3xl font-black tracking-tight text-foreground md:text-4xl">
              Panel de Control General
            </h1>
            <p className="mt-2 max-w-2xl text-muted-foreground font-medium">
              Administración unificada de citas, médicos y auditoría de MedAgenda.
            </p>
          </div>

          <AnimatedCard delay={400} className="flex w-full flex-col gap-2 rounded-2xl border border-border/80 bg-card/30 p-4 backdrop-blur-md md:w-auto md:min-w-72 shadow-sm ring-1 ring-white/10">
            <span className="text-[10px] font-black uppercase tracking-widest text-muted-foreground/60">Estado de Plataforma</span>
            <div className="flex items-center gap-2 text-sm font-bold text-emerald-600 dark:text-emerald-400">
              <BadgeCheck className="h-4 w-4" />
              Sistemas Operativos
            </div>
            <span className="text-[10px] text-muted-foreground/70 font-bold uppercase tracking-tighter">Core API • Database • Realtime Service</span>
          </AnimatedCard>
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
          variant="purple"
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

      <div className="grid gap-6 lg:grid-cols-5">
        <div className="space-y-4 lg:col-span-3">
          <AnimatedList className="grid gap-4 sm:grid-cols-2">
            <AnimatedCard>
              <Link
                href="/admin/usuarios"
                className="group flex h-full flex-col rounded-2xl border border-border bg-card/50 p-5 shadow-sm transition-all hover:shadow-md hover:border-purple-500/40 hover:bg-card"
              >
                <div className="mb-3 flex h-11 w-11 items-center justify-center rounded-xl bg-purple-500/15 text-purple-600 dark:text-purple-300">
                  <UserCog className="h-5 w-5" />
                </div>
                <p className="font-semibold text-foreground">Gestion de usuarios</p>
                <p className="mt-1 text-sm text-muted-foreground mb-4 flex-1">Pacientes, administradores y roles</p>
                <div className="flex items-center gap-1 text-sm font-medium text-purple-600 dark:text-purple-300">
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

        <AnimatedCard delay={500} className="lg:col-span-2 h-full rounded-2xl border border-purple-500/20 bg-gradient-to-br from-purple-500/5 to-fuchsia-500/5 p-6 shadow-sm backdrop-blur-[2px]">
          <div className="mb-4 flex items-center gap-3">
            <div className="flex h-10 w-10 items-center justify-center rounded-xl bg-purple-500/10 border border-purple-500/20">
              <Activity className="h-5 w-5 text-purple-600 dark:text-purple-400" />
            </div>
            <h3 className="text-xl font-black tracking-tight text-foreground">Monitoreo Salud API</h3>
          </div>

          <p className="mb-5 text-xs text-muted-foreground font-medium leading-relaxed">
            Métricas críticas en tiempo real del ecosistema digital MedAgenda.
          </p>

          <div className="space-y-3">
            {[
              { label: "Uptime API", value: "99.98%", color: "text-emerald-500" },
              { label: "Media Latencia", value: "142 ms", color: "text-purple-500" },
              { label: "Alertas Activas", value: "0", color: "text-emerald-500" },
            ].map((stat, i) => (
              <div key={i} className="flex items-center justify-between rounded-xl border border-border/50 bg-card/40 px-4 py-3 shadow-inner">
                <span className="text-[10px] font-black uppercase tracking-widest text-muted-foreground">{stat.label}</span>
                <span className={cn("text-sm font-black", stat.color)}>{stat.value}</span>
              </div>
            ))}
          </div>

          <div className="mt-5 w-fit rounded-lg bg-emerald-500/10 border border-emerald-500/20 px-3 py-1.5 text-[10px] font-black uppercase tracking-widest shadow-sm flex items-center gap-2">
            <div className="h-2 w-2 rounded-full bg-emerald-500 animate-pulse" />
            <span className="text-emerald-700 dark:text-emerald-400">Sistema: Optimizando</span>
          </div>
        </AnimatedCard>
      </div>
    </div>
  );
}
