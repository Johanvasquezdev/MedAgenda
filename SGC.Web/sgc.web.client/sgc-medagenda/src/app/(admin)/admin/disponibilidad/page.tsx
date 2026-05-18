"use client";

import { useEffect, useMemo, useState } from "react";
import { CalendarClock, Loader2, Plus } from "lucide-react";
import { MedicoDTO } from "@/types/medico.types";
import { DisponibilidadDTO, CreateDisponibilidadRequest } from "@/types/api.types";
import { MedicoService } from "@/services/medico.service";
import { DisponibilidadService } from "@/services/disponibilidad.service";
import { usePageTransition, AnimatedCard } from "@/components/animations/Animatedcomponents";

const DIAS_SEMANA = [
  { label: "Lunes", value: 0 },
  { label: "Martes", value: 1 },
  { label: "Miercoles", value: 2 },
  { label: "Jueves", value: 3 },
  { label: "Viernes", value: 4 },
  { label: "Sabado", value: 5 },
  { label: "Domingo", value: 6 },
];

const normalizeTime = (value: string) => {
  if (!value) return value;
  if (value.length === 5) return `${value}:00`;
  return value;
};

export default function AdminDisponibilidadPage() {
  usePageTransition();
  const [medicos, setMedicos] = useState<MedicoDTO[]>([]);
  const [medicoId, setMedicoId] = useState<number | "">("");
  const [disponibilidades, setDisponibilidades] = useState<DisponibilidadDTO[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [mensaje, setMensaje] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  const [form, setForm] = useState<Omit<CreateDisponibilidadRequest, "medicoId">>({
    diaSemana: 0,
    horaInicio: "08:00",
    horaFin: "12:00",
    duracionCitaMin: 30,
    esRecurrente: true,
  });

  const medicoSeleccionado = useMemo(
    () => medicos.find((m) => m.id === medicoId),
    [medicos, medicoId]
  );

  useEffect(() => {
    const cargar = async () => {
      setIsLoading(true);
      try {
        const data = await MedicoService.obtenerTodos();
        setMedicos(data);
      } catch (err) {
        console.error("Error cargando medicos:", err);
        setError("No se pudieron cargar los medicos.");
      } finally {
        setIsLoading(false);
      }
    };
    cargar();
  }, []);

  useEffect(() => {
    const cargarDisponibilidad = async () => {
      if (!medicoId) {
        setDisponibilidades([]);
        return;
      }
      try {
        const data = await DisponibilidadService.obtenerPorMedico(Number(medicoId));
        setDisponibilidades(data);
      } catch (err) {
        console.error("Error cargando disponibilidad:", err);
        setError("No se pudo cargar la disponibilidad del medico.");
      }
    };
    cargarDisponibilidad();
  }, [medicoId]);

  const onSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setMensaje(null);
    setError(null);

    if (!medicoId) {
      setError("Selecciona un medico.");
      return;
    }

    const payload: CreateDisponibilidadRequest = {
      medicoId: Number(medicoId),
      diaSemana: Number(form.diaSemana),
      horaInicio: normalizeTime(form.horaInicio),
      horaFin: normalizeTime(form.horaFin),
      duracionCitaMin: Number(form.duracionCitaMin),
      esRecurrente: Boolean(form.esRecurrente),
    };

    setIsSaving(true);
    try {
      await DisponibilidadService.crear(payload);
      setMensaje("Disponibilidad creada correctamente.");
      const data = await DisponibilidadService.obtenerPorMedico(Number(medicoId));
      setDisponibilidades(data);
    } catch (err: any) {
      console.error("Error creando disponibilidad:", err);
      setError(err?.message || "No se pudo crear la disponibilidad.");
    } finally {
      setIsSaving(false);
    }
  };

  return (
    <div className="p-6 max-w-6xl mx-auto space-y-6 page-content">
      <header className="relative overflow-hidden rounded-3xl border border-indigo-500/20 bg-gradient-to-br from-indigo-500/15 via-white dark:via-slate-950 to-purple-500/15 p-6 md:p-7 shadow-sm">
        <div className="absolute -right-16 -top-20 h-56 w-56 rounded-full bg-indigo-500/10 dark:bg-indigo-500/20 blur-3xl opacity-50" />
        <div className="absolute -bottom-20 -left-20 h-56 w-56 rounded-full bg-purple-500/10 dark:bg-purple-500/20 blur-3xl opacity-50" />

        <div className="relative z-10 flex flex-col gap-6 md:flex-row md:items-end md:justify-between">
          <div>
            <p className="text-xs font-black uppercase tracking-[0.2em] text-indigo-600 dark:text-indigo-400">
              Administración
            </p>
            <h1 className="mt-2 flex items-center gap-2 text-3xl font-black tracking-tight text-foreground">
              <CalendarClock className="h-8 w-8 text-indigo-600 dark:text-indigo-400" />
              Disponibilidad de Médicos
            </h1>
            <p className="mt-2 max-w-2xl text-muted-foreground font-medium">
              Asigna horarios a los doctores para habilitar el agendamiento de citas en el portal.
            </p>
          </div>
        </div>
      </header>

      <div className="grid gap-6 lg:grid-cols-[360px_1fr]">
        <AnimatedCard delay={100} className="h-fit">
          <form
            onSubmit={onSubmit}
            className="bg-card border border-border rounded-2xl p-6 space-y-5 shadow-sm"
          >
            <div className="space-y-2">
              <label className="text-xs font-black uppercase tracking-widest text-muted-foreground ml-1">Médico Especialista</label>
              <select
                className="w-full px-4 py-3 rounded-xl bg-background border border-border text-foreground font-medium focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-500 outline-none transition-all"
                value={medicoId}
                onChange={(e) => setMedicoId(e.target.value ? Number(e.target.value) : "")}
              >
                <option value="">Selecciona un médico</option>
                {medicos.map((m) => (
                  <option key={m.id} value={m.id}>
                    {m.nombre} {m.especialidadNombre ? `- ${m.especialidadNombre}` : ""}
                  </option>
                ))}
              </select>
            </div>

            <div className="space-y-2">
              <label className="text-xs font-black uppercase tracking-widest text-muted-foreground ml-1">Día Laboral</label>
              <select
                className="w-full px-4 py-3 rounded-xl bg-background border border-border text-foreground font-medium focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-500 outline-none transition-all"
                value={form.diaSemana}
                onChange={(e) => setForm({ ...form, diaSemana: Number(e.target.value) })}
              >
                {DIAS_SEMANA.map((d) => (
                  <option key={d.value} value={d.value}>
                    {d.label}
                  </option>
                ))}
              </select>
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div className="space-y-2">
                <label className="text-xs font-black uppercase tracking-widest text-muted-foreground ml-1">Hora Inicio</label>
                <input
                  type="time"
                  className="w-full px-4 py-3 rounded-xl bg-background border border-border text-foreground font-medium focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-500 outline-none transition-all"
                  value={form.horaInicio}
                  onChange={(e) => setForm({ ...form, horaInicio: e.target.value })}
                />
              </div>
              <div className="space-y-2">
                <label className="text-xs font-black uppercase tracking-widest text-muted-foreground ml-1">Hora Fin</label>
                <input
                  type="time"
                  className="w-full px-4 py-3 rounded-xl bg-background border border-border text-foreground font-medium focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-500 outline-none transition-all"
                  value={form.horaFin}
                  onChange={(e) => setForm({ ...form, horaFin: e.target.value })}
                />
              </div>
            </div>

            <div className="space-y-2">
              <label className="text-xs font-black uppercase tracking-widest text-muted-foreground ml-1">Duración de Cita (min)</label>
              <input
                type="number"
                min={10}
                step={5}
                className="w-full px-4 py-3 rounded-xl bg-background border border-border text-foreground font-medium focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-500 outline-none transition-all"
                value={form.duracionCitaMin}
                onChange={(e) => setForm({ ...form, duracionCitaMin: Number(e.target.value) })}
              />
            </div>

            <label className="flex items-center gap-3 p-3 bg-muted/30 rounded-xl border border-border cursor-pointer hover:bg-muted/50 transition-colors">
              <input
                type="checkbox"
                className="w-4 h-4 rounded border-border text-indigo-600 focus:ring-indigo-500"
                checked={form.esRecurrente}
                onChange={(e) => setForm({ ...form, esRecurrente: e.target.checked })}
              />
              <span className="text-sm font-bold text-foreground">Repetir semanalmente</span>
            </label>

            {mensaje && <p className="text-emerald-600 dark:text-emerald-400 text-xs font-bold bg-emerald-500/10 p-3 rounded-lg border border-emerald-500/20">{mensaje}</p>}
            {error && <p className="text-rose-600 dark:text-rose-400 text-xs font-bold bg-rose-500/10 p-3 rounded-lg border border-rose-500/20">{error}</p>}

            <button
              type="submit"
              disabled={isSaving}
              className="w-full flex items-center justify-center gap-2 rounded-xl bg-indigo-600 hover:bg-indigo-500 text-white px-5 py-3 disabled:opacity-60 font-black shadow-lg shadow-indigo-500/20 transition-all active:scale-95"
            >
              {isSaving ? <Loader2 className="w-5 h-5 animate-spin" /> : <Plus className="w-5 h-5" />}
              Habilitar Horario
            </button>
          </form>
        </AnimatedCard>

        <AnimatedCard delay={300} className="bg-card border border-border rounded-2xl p-6 shadow-sm min-h-[320px]">
          <div className="flex items-center justify-between mb-6">
            <h2 className="text-xl font-black text-foreground">Horarios Activos</h2>
            {medicoSeleccionado && (
              <span className="text-xs font-black uppercase tracking-widest bg-indigo-500/10 text-indigo-600 dark:text-indigo-400 px-3 py-1 rounded-full border border-indigo-500/20">
                Dr. {medicoSeleccionado.nombre.split(" ").pop()}
              </span>
            )}
          </div>

          {isLoading ? (
            <div className="flex flex-col items-center justify-center h-48 gap-3">
              <Loader2 className="w-10 h-10 animate-spin text-indigo-500/30" />
              <p className="text-muted-foreground font-bold">Cargando datos...</p>
            </div>
          ) : !medicoId ? (
            <div className="flex flex-col items-center justify-center h-48 text-center px-8">
               <CalendarClock className="w-12 h-12 text-muted-foreground/20 mb-3" />
               <p className="text-muted-foreground font-bold">Selecciona un médico para visualizar su agenda.</p>
            </div>
          ) : disponibilidades.length === 0 ? (
            <div className="flex flex-col items-center justify-center h-48 text-center px-8 border-2 border-dashed border-border rounded-2xl">
              <p className="text-muted-foreground font-bold">No se han registrado horarios para este médico.</p>
            </div>
          ) : (
            <div className="grid gap-3 sm:grid-cols-2">
              {disponibilidades.map((item) => (
                <div
                  key={item.id}
                  className="flex items-center justify-between bg-muted/30 border border-border rounded-2xl px-5 py-4 group transition-all hover:bg-muted/50"
                >
                  <div>
                    <p className="text-foreground font-black text-lg leading-none mb-1.5">{item.diaSemana}</p>
                    <div className="flex items-center gap-2 text-muted-foreground text-xs font-bold">
                      <span>{item.horaInicio} - {item.horaFin}</span>
                      <span className="h-1 w-1 rounded-full bg-border" />
                      <span>{item.duracionCitaMin} m</span>
                    </div>
                  </div>
                  <div className={`px-2.5 py-1 rounded-lg text-[10px] font-black uppercase tracking-tighter ${
                    item.esRecurrente ? "bg-emerald-500/10 text-emerald-600" : "bg-indigo-500/10 text-indigo-600"
                  }`}>
                    {item.esRecurrente ? "Semanal" : "Única"}
                  </div>
                </div>
              ))}
            </div>
          )}
        </AnimatedCard>
      </div>
    </div>
  );
}
