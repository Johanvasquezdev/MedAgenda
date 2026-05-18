"use client";

import { useEffect, useMemo, useState } from "react";
import dayjs from "dayjs";
import {
  Calendar,
  CalendarDays,
  CalendarCheck,
  Clock,
  MoreVertical,
  Stethoscope,
} from "lucide-react";
import { CitaDTO, EstadoCita, MedicoDTO } from "@/types/api.types";
import { CitaService } from "@/services/cita.service";
import { MedicoService } from "@/services/medico.service";
import { useAuth } from "@/components/providers/AuthProvider";
import { MedicoCard } from "@/components/medicos/MedicoCard";
import { AgendarCitaModal } from "@/components/citas/AgendarCita";
import { toast } from "sonner";
import { AnimatedStatsCard } from "@/components/animations/Animatedstatscard";
import { AnimatedList, AnimatedCard, usePageTransition } from "@/components/animations/Animatedcomponents";

export default function PacienteDashboard() {
  const { user } = useAuth();
  const [citas, setCitas] = useState<CitaDTO[]>([]);
  const [medicos, setMedicos] = useState<MedicoDTO[]>([]);
  const [loading, setLoading] = useState(true);
  const [selectedMedico, setSelectedMedico] = useState<MedicoDTO | null>(null);

  useEffect(() => {
    const fetchData = async () => {
      if (!user?.id) {
        setCitas([]);
        setMedicos([]);
        setLoading(false);
        return;
      }
      try {
        const [citasData, medicosData] = await Promise.all([
          CitaService.obtenerCitasPorPaciente(user.id),
          MedicoService.obtenerTodos(),
        ]);
        setCitas(citasData);
        setMedicos(medicosData.filter(m => m.activo));
      } catch {
        setCitas([]);
        setMedicos([]);
      } finally {
        setLoading(false);
      }
    };
    fetchData();
  }, [user?.id]);

  usePageTransition();

  const medicosById = useMemo(() => {
    const map = new Map<number, MedicoDTO>();
    medicos.forEach(m => map.set(m.id, m));
    return map;
  }, [medicos]);

  const citasHoy = citas.filter(c => dayjs(c.fechaHora).isSame(dayjs(), "day"));
  const citasPendientes = citas.filter(c => c.estado === EstadoCita.Solicitada);
  const proximasCitas = citas
    .filter(c => dayjs(c.fechaHora).isAfter(dayjs()))
    .sort((a, b) => dayjs(a.fechaHora).valueOf() - dayjs(b.fechaHora).valueOf())
    .slice(0, 3);

  const medicosDestacados = medicos.slice(0, 3);

  const getStatusStyles = (estado: EstadoCita | string) => {
    switch ((estado || "").toString().toLowerCase()) {
      case "confirmada":
        return "bg-emerald-500/10 text-emerald-600 dark:text-emerald-300 border border-emerald-500/20";
      case "solicitada":
        return "bg-amber-500/10 text-amber-600 dark:text-amber-300 border border-amber-500/20";
      case "cancelada":
        return "bg-rose-500/10 text-rose-600 dark:text-rose-300 border border-rose-500/20";
      case "noasistio":
        return "bg-muted text-muted-foreground border border-border";
      default:
        return "bg-muted text-muted-foreground border border-border";
    }
  };

  const cancelarCita = async (citaId: number) => {
    try {
      await CitaService.cancelarCita(citaId, "Cancelada por paciente");
      setCitas(citas.map(c => c.id === citaId ? { ...c, estado: EstadoCita.Cancelada } : c));
      toast.success("Cita cancelada");
    } catch {
      toast.error("No se pudo cancelar la cita");
    }
  };

  return (
    <div className="space-y-8 page-content animate-in fade-in duration-500">
      <header className="relative overflow-hidden rounded-3xl border border-emerald-500/20 bg-gradient-to-br from-emerald-500/15 via-white dark:via-slate-950 to-teal-500/15 p-6 md:p-8 shadow-sm">
        <div className="absolute -right-16 -top-20 h-56 w-56 rounded-full bg-emerald-500/10 dark:bg-emerald-500/20 blur-3xl opacity-50" />
        <div className="absolute -bottom-20 -left-20 h-56 w-56 rounded-full bg-teal-500/10 dark:bg-teal-500/20 blur-3xl opacity-50" />
        
        <div className="relative z-10">
          <p className="text-xs font-bold uppercase tracking-[0.2em] text-emerald-600 dark:text-emerald-400 mb-2">
            Panel de Control
          </p>
          <h1 className="text-3xl md:text-4xl font-black tracking-tight text-foreground">
            Hola, <span className="text-emerald-600 dark:text-emerald-400">{user?.nombre?.split(" ")[0] || "Paciente"}</span>
          </h1>
          <p className="mt-2 text-muted-foreground font-medium max-w-xl leading-relaxed">
            Gestiona tus citas de forma inteligente y mantén un seguimiento detallado de tu salud.
          </p>
        </div>
      </header>

      <div className="grid grid-cols-1 gap-6 md:grid-cols-3">
        <AnimatedStatsCard
          title="Citas Hoy"
          value={loading ? 0 : citasHoy.length}
          icon={CalendarCheck}
          delay={100}
          variant="emerald"
        />
        <AnimatedStatsCard
          title="Citas Pendientes"
          value={loading ? 0 : citasPendientes.length}
          icon={Clock}
          delay={200}
          variant="amber"
        />
        <AnimatedStatsCard
          title="Médicos Disponibles"
          value={loading ? 0 : medicos.length}
          icon={Stethoscope}
          delay={300}
          variant="cyan"
        />
      </div>

      <div className="grid grid-cols-1 gap-6 lg:grid-cols-5">
        <div className="space-y-4 lg:col-span-3">
          <div className="flex items-center justify-between mb-4">
            <h2 className="text-xl font-bold text-foreground">Próximas Citas</h2>
            <div className="flex items-center gap-4">
              <a
                href="/paciente/medicos"
                className="text-xs font-bold uppercase tracking-wider text-emerald-600 dark:text-emerald-400 hover:underline"
              >
                Agendar cita
              </a>
              <a
                href="/paciente/citas"
                className="text-xs font-bold uppercase tracking-wider text-muted-foreground hover:text-foreground transition-colors"
              >
                Ver todas
              </a>
            </div>
          </div>

          {proximasCitas.length === 0 ? (
            <div className="rounded-3xl border border-border bg-card/50 p-12 text-center text-muted-foreground shadow-sm">
              <Calendar className="w-12 h-12 text-muted-foreground/20 mx-auto mb-4" />
              <p className="font-medium text-sm">No tienes citas próximas. Agenda una con un médico disponible.</p>
            </div>
          ) : (
            <AnimatedList className="space-y-3">
              {proximasCitas.map((cita) => {
                const medico = medicosById.get(cita.medicoId);
                return (
                  <AnimatedCard key={cita.id} className="rounded-2xl border border-border bg-card p-5 shadow-sm hover:shadow-md transition-all">
                    <div className="flex flex-col gap-4 md:flex-row md:items-center md:justify-between">
                      <div className="flex items-center gap-5">
                        <div className="rounded-2xl bg-emerald-500/10 p-3 shadow-inner">
                          <CalendarDays className="h-6 w-6 text-emerald-600 dark:text-emerald-400" />
                        </div>
                        <div>
                          <p className="text-lg font-bold text-foreground leading-none">{medico?.nombre || "Médico"}</p>
                          <p className="text-sm font-semibold text-emerald-600 dark:text-emerald-400 mt-1">
                            {medico?.especialidadNombre || "Especialidad"}
                          </p>
                        </div>
                      </div>

                      <div className="flex flex-wrap items-center gap-4 text-sm text-muted-foreground md:gap-6">
                        <div className="flex items-center gap-1.5">
                          <Calendar className="h-4 w-4" />
                          <span>{dayjs(cita.fechaHora).format("DD MMM YYYY")}</span>
                        </div>
                        <div className="flex items-center gap-1.5">
                          <Clock className="h-4 w-4" />
                          <span>{dayjs(cita.fechaHora).format("hh:mm A")}</span>
                        </div>
                        <span
                          className={`rounded-full px-3 py-1 text-xs font-semibold ${getStatusStyles(cita.estado)}`}
                        >
                          {cita.estado}
                        </span>
                        <button
                          type="button"
                          className="rounded-lg p-1.5 text-muted-foreground transition-colors hover:bg-secondary hover:text-foreground"
                          aria-label="Acciones de cita"
                        >
                          <MoreVertical className="h-4 w-4" />
                        </button>
                      </div>
                    </div>

                    {(cita.estado === EstadoCita.Confirmada || cita.estado === EstadoCita.Solicitada) && (
                      <button
                        type="button"
                        onClick={() => cancelarCita(cita.id)}
                        className="mt-3 border-t border-border/60 pt-3 text-sm text-rose-500 hover:text-rose-400"
                      >
                        Cancelar cita
                      </button>
                    )}
                  </AnimatedCard>
                );
              })}
            </AnimatedList>
          )}
        </div>

        <div className="space-y-4 lg:col-span-2">
          <div className="flex items-center justify-between">
            <h2 className="text-xl font-semibold text-foreground">Médicos Destacados</h2>
            <a
              href="/paciente/medicos"
              className="text-sm font-medium text-emerald-500 hover:text-emerald-400"
            >
              Ver todos
            </a>
          </div>

          <AnimatedList className="grid gap-4">
            {medicosDestacados.map((medico) => (
              <AnimatedCard key={medico.id}>
                <MedicoCard medico={medico} onAgendarClick={setSelectedMedico} />
              </AnimatedCard>
            ))}
          </AnimatedList>
        </div>
      </div>

      <AgendarCitaModal medico={selectedMedico} isOpen={!!selectedMedico} onClose={() => setSelectedMedico(null)} />
    </div>
  );
}
