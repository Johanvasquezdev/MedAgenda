"use client";

import { useState } from "react";
import { CalendarCheck, Loader2, Search, Clock3, Activity } from "lucide-react";
import { usePageTransition } from "@/components/animations/Animatedcomponents";
import { CitaDTO, EstadoCita } from "@/types/api.types";
import { CitaService } from "@/services/cita.service";
import dayjs from "dayjs";

export default function AdminCitasPage() {
  usePageTransition();
  const [pacienteId, setPacienteId] = useState("");
  const [medicoId, setMedicoId] = useState("");
  const [fecha, setFecha] = useState("");
  const [citas, setCitas] = useState<CitaDTO[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const cargar = async () => {
    if (!pacienteId) {
      setError("Ingrese un pacienteId válido.");
      return;
    }
    setError("");
    setLoading(true);
    try {
      const data = await CitaService.obtenerCitasPorPaciente(Number(pacienteId));
      setCitas(data);
    } catch {
      setCitas([]);
      setError("No se pudieron cargar las citas.");
    } finally {
      setLoading(false);
    }
  };

  const confirmar = async (id: number) => {
    await CitaService.confirmarCita(id);
    setCitas((prev) => prev.map((c) => (c.id === id ? { ...c, estado: EstadoCita.Confirmada } : c)));
  };

  const iniciar = async (id: number) => {
    await CitaService.iniciarConsulta(id);
    setCitas((prev) => prev.map((c) => (c.id === id ? { ...c, estado: EstadoCita.EnProgreso } : c)));
  };

  const asistencia = async (id: number, asistio: boolean) => {
    await CitaService.marcarAsistencia(id, asistio);
    setCitas((prev) =>
      prev.map((c) =>
        c.id === id ? { ...c, estado: asistio ? EstadoCita.Completada : EstadoCita.NoAsistio } : c
      )
    );
  };

  const cancelar = async (id: number) => {
    const motivo = window.prompt("Motivo de cancelación:");
    if (!motivo) return;
    await CitaService.cancelarCita(id, motivo);
    setCitas((prev) => prev.map((c) => (c.id === id ? { ...c, estado: EstadoCita.Cancelada } : c)));
  };

  const reprogramar = async (id: number) => {
    const nuevaFecha = window.prompt("Nueva fecha y hora (YYYY-MM-DD HH:mm):");
    if (!nuevaFecha) return;
    const iso = new Date(nuevaFecha.replace(" ", "T")).toISOString();
    await CitaService.reprogramarCita(id, iso);
    setCitas((prev) => prev.map((c) => (c.id === id ? { ...c, fechaHora: iso, estado: EstadoCita.Solicitada } : c)));
  };

  const citasFiltradas = citas.filter((c) => {
    if (medicoId && c.medicoId !== Number(medicoId)) return false;
    if (fecha) {
      const fechaCita = dayjs(c.fechaHora).format("YYYY-MM-DD");
      if (fechaCita !== fecha) return false;
    }
    return true;
  });

  const totalFiltradas = citasFiltradas.length;
  const pendientes = citasFiltradas.filter((c) => c.estado === EstadoCita.Solicitada).length;
  const confirmadas = citasFiltradas.filter((c) => c.estado === EstadoCita.Confirmada).length;
  const enProgreso = citasFiltradas.filter((c) => c.estado === EstadoCita.EnProgreso).length;

  return (
    <div className="mx-auto max-w-7xl space-y-6 p-6 page-content">
      <header className="relative overflow-hidden rounded-3xl border border-indigo-500/20 bg-gradient-to-br from-indigo-500/15 via-white dark:via-slate-950 to-purple-500/15 p-6 md:p-7 shadow-sm">
        <div className="absolute -right-16 -top-20 h-56 w-56 rounded-full bg-indigo-500/10 dark:bg-indigo-500/20 blur-3xl opacity-50" />
        <div className="absolute -bottom-20 -left-20 h-56 w-56 rounded-full bg-purple-500/10 dark:bg-purple-500/20 blur-3xl opacity-50" />

        <div className="relative z-10 flex flex-col gap-6 md:flex-row md:items-end md:justify-between">
          <div>
            <p className="text-xs font-black uppercase tracking-[0.2em] text-indigo-600 dark:text-indigo-400">
              Administración
            </p>
            <h1 className="mt-2 flex items-center gap-2 text-3xl font-black tracking-tight text-foreground">
              <CalendarCheck className="h-8 w-8 text-indigo-600 dark:text-indigo-400" />
              Gestión de Citas
            </h1>
            <p className="mt-2 max-w-2xl text-muted-foreground font-medium">
              Consulta, filtra y opera citas de pacientes en una sola vista operativa centralizada.
            </p>
          </div>

          <div className="rounded-2xl border border-border bg-card/40 px-5 py-3 backdrop-blur-sm shadow-sm border-l-4 border-l-indigo-500/50">
            <p className="text-xs text-muted-foreground font-black uppercase tracking-widest">Resultados Actuales</p>
            <p className="text-2xl font-black text-foreground leading-tight">{loading ? "--" : totalFiltradas}</p>
          </div>
        </div>
      </header>

      <section className="grid gap-4 md:grid-cols-4">
        <article className="rounded-2xl border border-border bg-card p-5 shadow-sm">
          <p className="text-xs font-black uppercase tracking-widest text-muted-foreground">Total Filtradas</p>
          <p className="mt-1 text-2xl font-black text-foreground">{loading ? "--" : totalFiltradas}</p>
        </article>
        <article className="rounded-2xl border border-amber-500/30 bg-amber-500/10 p-5 shadow-sm">
          <div className="flex items-center gap-2 text-amber-600 dark:text-amber-400">
            <Clock3 className="h-4 w-4" />
            <p className="text-xs font-black uppercase tracking-widest">Solicitadas</p>
          </div>
          <p className="mt-1 text-2xl font-black text-foreground">{loading ? "--" : pendientes}</p>
        </article>
        <article className="rounded-2xl border border-emerald-500/30 bg-emerald-500/10 p-5 shadow-sm">
          <div className="flex items-center gap-2 text-emerald-600 dark:text-emerald-400">
            <CalendarCheck className="h-4 w-4" />
            <p className="text-xs font-black uppercase tracking-widest">Confirmadas</p>
          </div>
          <p className="mt-1 text-2xl font-black text-foreground">{loading ? "--" : confirmadas}</p>
        </article>
        <article className="rounded-2xl border border-indigo-500/30 bg-indigo-500/10 p-5 shadow-sm">
          <div className="flex items-center gap-2 text-indigo-600 dark:text-indigo-400">
            <Activity className="h-4 w-4" />
            <p className="text-xs font-black uppercase tracking-widest">En Progreso</p>
          </div>
          <p className="mt-1 text-2xl font-black text-foreground">{loading ? "--" : enProgreso}</p>
        </article>
      </section>

      <section className="rounded-2xl border border-border bg-card/50 p-6 shadow-sm border-l-4 border-l-indigo-500/50">
        <div className="mb-4 flex items-center gap-2 text-xs font-black uppercase tracking-widest text-muted-foreground">
          <Search className="h-3 w-3" />
          Filtros de Consulta Operativa
        </div>

        <div className="flex flex-col items-start gap-4 md:flex-row md:items-end">
          <div className="flex flex-col gap-1.5 w-full md:w-auto">
            <label className="text-[10px] font-black uppercase tracking-widest text-muted-foreground ml-1">Paciente ID</label>
            <input
              value={pacienteId}
              onChange={(e) => setPacienteId(e.target.value)}
              className="w-full md:w-48 px-4 py-2.5 rounded-xl bg-background border border-border text-foreground font-medium focus:ring-2 focus:ring-indigo-500/20 outline-none transition-all placeholder:text-muted-foreground/30"
              placeholder="Ej: 17"
            />
          </div>
          <div className="flex flex-col gap-1.5 w-full md:w-auto">
            <label className="text-[10px] font-black uppercase tracking-widest text-muted-foreground ml-1">Médico ID</label>
            <input
              value={medicoId}
              onChange={(e) => setMedicoId(e.target.value)}
              className="w-full md:w-48 px-4 py-2.5 rounded-xl bg-background border border-border text-foreground font-medium focus:ring-2 focus:ring-indigo-500/20 outline-none transition-all placeholder:text-muted-foreground/30"
              placeholder="Ej: 16"
            />
          </div>
          <div className="flex flex-col gap-1.5 w-full md:w-auto">
            <label className="text-[10px] font-black uppercase tracking-widest text-muted-foreground ml-1">Fecha</label>
            <input
              type="date"
              value={fecha}
              onChange={(e) => setFecha(e.target.value)}
              className="w-full md:w-48 px-4 py-2.5 rounded-xl bg-background border border-border text-foreground font-medium focus:ring-2 focus:ring-indigo-500/20 outline-none transition-all"
            />
          </div>
          <button onClick={cargar} className="w-full md:w-auto px-8 py-2.5 rounded-xl bg-indigo-600 hover:bg-indigo-500 text-white font-black shadow-lg shadow-indigo-500/20 transition-all active:scale-95">
            Buscar
          </button>
          {error && <span className="text-rose-600 dark:text-rose-400 text-xs font-bold mb-2.5">{error}</span>}
        </div>
      </section>

      <div className="overflow-hidden rounded-2xl border border-border bg-card shadow-sm">
        <div className="overflow-x-auto">
          <table className="w-full text-left text-sm">
            <thead className="bg-muted/50 border-b border-border text-muted-foreground">
              <tr>
                <th className="p-4 text-xs font-black uppercase tracking-widest">ID</th>
                <th className="p-4 text-xs font-black uppercase tracking-widest">Fecha y Hora</th>
                <th className="p-4 text-xs font-black uppercase tracking-widest">Estado</th>
                <th className="p-4 text-xs font-black uppercase tracking-widest text-right">Acciones Directas</th>
              </tr>
            </thead>
            <tbody>
              {loading ? (
                <tr>
                  <td colSpan={4} className="p-16 text-center">
                    <Loader2 className="animate-spin mx-auto h-10 w-10 text-indigo-500" />
                  </td>
                </tr>
              ) : citasFiltradas.length === 0 ? (
                <tr>
                  <td colSpan={4} className="p-16 text-center text-muted-foreground font-bold">
                    No se encontraron registros de citas para los criterios seleccionados.
                  </td>
                </tr>
              ) : (
                citasFiltradas.map((c) => (
                  <tr key={c.id} className="border-b border-border/50 hover:bg-muted/20 transition-colors">
                    <td className="p-4 text-foreground font-mono text-xs font-bold leading-none">{c.id}</td>
                    <td className="p-4 text-foreground font-medium">{dayjs(c.fechaHora).format("DD/MM/YYYY HH:mm")}</td>
                    <td className="p-4">
                      <span className="px-3 py-1.5 rounded-full text-[10px] font-black uppercase tracking-widest bg-muted border border-border text-foreground">
                        {c.estado}
                      </span>
                    </td>
                    <td className="p-4 text-right">
                      <div className="flex flex-wrap justify-end gap-2">
                        <button onClick={() => confirmar(c.id)} className="px-3 py-1.5 rounded-lg text-[10px] font-black uppercase tracking-widest border border-emerald-500/40 text-emerald-600 dark:text-emerald-400 hover:bg-emerald-500/10 transition-all active:scale-95 shadow-sm">
                          Confirmar
                        </button>
                        <button onClick={() => iniciar(c.id)} className="px-3 py-1.5 rounded-lg text-[10px] font-black uppercase tracking-widest border border-indigo-500/40 text-indigo-600 dark:text-indigo-400 hover:bg-indigo-500/10 transition-all active:scale-95 shadow-sm">
                          Iniciar
                        </button>
                        <button onClick={() => asistencia(c.id, true)} className="px-3 py-1.5 rounded-lg text-[10px] font-black uppercase tracking-widest border border-emerald-500/40 text-emerald-600 dark:text-emerald-400 hover:bg-emerald-500/10 transition-all active:scale-95 shadow-sm">
                          Asistió
                        </button>
                        <button onClick={() => asistencia(c.id, false)} className="px-3 py-1.5 rounded-lg text-[10px] font-black uppercase tracking-widest border border-amber-500/40 text-amber-600 dark:text-amber-400 hover:bg-amber-500/10 transition-all active:scale-95 shadow-sm">
                          No asistió
                        </button>
                        <button onClick={() => reprogramar(c.id)} className="px-3 py-1.5 rounded-lg text-[10px] font-black uppercase tracking-widest border border-border text-muted-foreground hover:bg-muted transition-all active:scale-95 shadow-sm">
                          Reprogramar
                        </button>
                        <button onClick={() => cancelar(c.id)} className="px-3 py-1.5 rounded-lg text-[10px] font-black uppercase tracking-widest border border-rose-500/40 text-rose-600 dark:text-rose-400 hover:bg-rose-500/10 transition-all active:scale-95 shadow-sm">
                          Cancelar
                        </button>
                      </div>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}
