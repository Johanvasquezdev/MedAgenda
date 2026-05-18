"use client";

import { useEffect, useMemo, useState } from "react";
import { Stethoscope, Plus, Loader2, X, Search, CheckCircle2, XCircle } from "lucide-react";
import { EspecialidadDTO, EspecialidadService } from "@/services/especialidad.service";
import { AnimatedStatsCard } from "@/components/animations/Animatedstatscard";
import { AnimatedCard, usePageTransition } from "@/components/animations/Animatedcomponents";

export default function EspecialidadesPage() {
  usePageTransition();
  const nombreInputId = "especialidad-nombre";
  const descripcionInputId = "especialidad-descripcion";
  const busquedaInputId = "especialidades-busqueda";
  const [especialidades, setEspecialidades] = useState<EspecialidadDTO[]>([]);
  const [loading, setLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [modoEdicion, setModoEdicion] = useState(false);
  const [especialidadSeleccionada, setEspecialidadSeleccionada] = useState<EspecialidadDTO | null>(null);
  const [query, setQuery] = useState("");
  const [formData, setFormData] = useState({ nombre: "", descripcion: "" });

  const cargar = async () => {
    setLoading(true);
    try {
      const data = await EspecialidadService.obtenerTodas();
      setEspecialidades(data);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    cargar();
  }, []);

  const abrirCrear = () => {
    setModoEdicion(false);
    setEspecialidadSeleccionada(null);
    setFormData({ nombre: "", descripcion: "" });
    setIsModalOpen(true);
  };

  const abrirEditar = (esp: EspecialidadDTO) => {
    setModoEdicion(true);
    setEspecialidadSeleccionada(esp);
    setFormData({ nombre: esp.nombre, descripcion: esp.descripcion || "" });
    setIsModalOpen(true);
  };

  const cerrarModal = () => {
    setIsModalOpen(false);
    setEspecialidadSeleccionada(null);
  };

  const guardar = async (e: React.FormEvent) => {
    e.preventDefault();
    if (modoEdicion && especialidadSeleccionada) {
      await EspecialidadService.actualizar(especialidadSeleccionada.id, formData);
    } else {
      await EspecialidadService.crear(formData);
    }
    await cargar();
    cerrarModal();
  };

  const totalEspecialidades = especialidades.length;
  const activas = especialidades.filter((e) => e.activo).length;
  const inactivas = totalEspecialidades - activas;

  const especialidadesFiltradas = useMemo(() => {
    const term = query.trim().toLowerCase();
    if (!term) return especialidades;

    return especialidades.filter((e) => {
      return (
        e.nombre.toLowerCase().includes(term) ||
        (e.descripcion || "").toLowerCase().includes(term)
      );
    });
  }, [especialidades, query]);

  return (
    <div className="p-6 max-w-7xl mx-auto space-y-6 page-content">
      <header className="relative overflow-hidden rounded-3xl border border-indigo-500/20 bg-gradient-to-br from-indigo-500/15 via-white dark:via-slate-950 to-purple-500/15 p-6 md:p-7 shadow-sm">
        <div className="absolute -right-16 -top-20 h-56 w-56 rounded-full bg-indigo-500/10 dark:bg-indigo-500/20 blur-3xl opacity-50" />
        <div className="absolute -bottom-20 -left-20 h-56 w-56 rounded-full bg-purple-500/10 dark:bg-purple-500/20 blur-3xl opacity-50" />

        <div className="relative z-10 flex flex-col gap-6 md:flex-row md:items-end md:justify-between">
          <div>
            <p className="text-xs font-black uppercase tracking-[0.2em] text-indigo-600 dark:text-indigo-400">
              Administración
            </p>
            <h1 className="mt-2 flex items-center gap-2 text-3xl font-black tracking-tight text-foreground">
              <Stethoscope className="h-8 w-8 text-indigo-600 dark:text-indigo-400" />
              Especialidades Médicas
            </h1>
            <p className="mt-2 max-w-2xl text-muted-foreground font-medium">
              Gestiona el catálogo de especialidades médicas para asignación en perfiles y agendas.
            </p>
          </div>

          <button
            type="button"
            onClick={abrirCrear}
            className="inline-flex items-center gap-2 rounded-xl bg-indigo-600 hover:bg-indigo-500 px-5 py-2.5 text-sm font-bold text-white transition-all shadow-lg shadow-indigo-500/20 active:scale-95"
          >
            <Plus aria-hidden="true" className="h-4 w-4" /> Añadir nueva
          </button>
        </div>
      </header>

      <section className="grid gap-4 md:grid-cols-3">
        <AnimatedStatsCard
          title="Total especialidades"
          value={loading ? 0 : totalEspecialidades}
          icon={Stethoscope}
          description="En el sistema"
          delay={100}
        />
        <AnimatedStatsCard
          title="Activas"
          value={loading ? 0 : activas}
          icon={CheckCircle2}
          description="Habilitadas"
          delay={200}
          iconClassName="bg-emerald-500/10 text-emerald-400"
        />
        <AnimatedStatsCard
          title="Inactivas"
          value={loading ? 0 : inactivas}
          icon={XCircle}
          description="Fuera de uso"
          delay={300}
          iconClassName="bg-rose-500/10 text-rose-400"
        />
      </section>

      <section className="bg-card/50 backdrop-blur-md border border-border rounded-2xl p-4 shadow-sm border-l-4 border-l-indigo-500/50">
        <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
          <label htmlFor={busquedaInputId} className="relative block w-full sm:max-w-sm">
            <Search aria-hidden="true" className="pointer-events-none absolute left-4 h-5 w-5 top-1/2 -translate-y-1/2 text-muted-foreground" />
            <input
              id={busquedaInputId}
              aria-label="Buscar especialidades"
              value={query}
              onChange={(e) => setQuery(e.target.value)}
              placeholder="Buscar por nombre o descripción..."
              className="w-full rounded-xl border border-border bg-background py-3 pl-12 pr-4 text-sm text-foreground placeholder:text-muted-foreground/50 focus:outline-none focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-500 transition-all shadow-sm"
            />
          </label>
          <p className="text-xs font-bold text-muted-foreground bg-muted px-3 py-1 rounded-full border border-border">
            {loading ? "Cargando..." : `${especialidadesFiltradas.length} especialidades encontradas`}
          </p>
        </div>
      </section>

      <AnimatedCard delay={400} className="overflow-hidden rounded-2xl border border-border bg-card shadow-sm">
        <div className="overflow-x-auto">
          <table className="w-full text-left text-sm">
            <thead className="bg-muted/50 border-b border-border text-muted-foreground">
              <tr>
                <th className="p-4 text-xs font-black uppercase tracking-widest">Especialidad</th>
                <th className="p-4 text-xs font-black uppercase tracking-widest">Descripción</th>
                <th className="p-4 text-xs font-black uppercase tracking-widest text-center">Estado</th>
                <th className="p-4 text-xs font-black uppercase tracking-widest text-right">Acciones</th>
              </tr>
            </thead>
            <tbody>
              {loading ? (
                <tr>
                  <td colSpan={4} className="p-12 text-center">
                    <Loader2 className="animate-spin mx-auto text-indigo-500 h-8 w-8" />
                  </td>
                </tr>
              ) : especialidadesFiltradas.length === 0 ? (
                <tr>
                  <td colSpan={4} className="p-12 text-center text-muted-foreground font-bold">
                    No se encontraron especialidades para el filtro actual.
                  </td>
                </tr>
              ) : (
                especialidadesFiltradas.map((esp) => (
                  <tr key={esp.id} className="border-b border-border/50 hover:bg-muted/30 transition-colors group">
                    <td className="p-4">
                      <p className="font-bold text-foreground text-base group-hover:text-indigo-600 dark:group-hover:text-indigo-400 transition-colors">{esp.nombre}</p>
                    </td>
                    <td className="p-4 text-muted-foreground font-medium truncate max-w-xs">{esp.descripcion || "Sin descripción"}</td>
                    <td className="p-4 text-center">
                      <span
                        className={`px-3 py-1.5 rounded-full text-[10px] font-black uppercase tracking-widest shadow-sm ${
                          esp.activo
                            ? "bg-emerald-500/10 text-emerald-600 dark:text-emerald-400 border border-emerald-500/20"
                            : "bg-rose-500/10 text-rose-600 dark:text-rose-400 border border-rose-500/20"
                        }`}
                      >
                        {esp.activo ? "Activa" : "Inactiva"}
                      </span>
                    </td>
                    <td className="p-4 text-right">
                      <button
                        type="button"
                        onClick={() => abrirEditar(esp)}
                        className="px-4 py-2 rounded-xl text-xs font-bold border border-border text-foreground hover:bg-muted transition-all active:scale-95 shadow-sm"
                      >
                        Editar
                      </button>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </AnimatedCard>

      {isModalOpen && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-background/80 backdrop-blur-sm p-4 animate-in fade-in duration-300">
          <div className="w-full max-w-lg bg-card border border-border rounded-3xl overflow-hidden shadow-2xl animate-in zoom-in-95 duration-300">
            <div className="flex items-center justify-between px-6 py-5 border-b border-border bg-muted/30">
              <h2 className="text-xl font-black text-foreground">
                {modoEdicion ? "Editar Especialidad" : "Nueva Especialidad"}
              </h2>
              <button type="button" onClick={cerrarModal} aria-label="Cerrar modal" className="text-muted-foreground hover:text-foreground transition-colors">
                <X aria-hidden="true" className="w-6 h-6" />
              </button>
            </div>
            <form onSubmit={guardar} className="p-6 space-y-5">
              <div className="space-y-2">
                <label className="text-xs font-black uppercase tracking-widest text-muted-foreground ml-1">Nombre de la Especialidad</label>
                <input
                  id={nombreInputId}
                  required
                  placeholder="Ej: Cardiología"
                  className="w-full px-4 py-3 rounded-xl bg-background border border-border text-foreground placeholder:text-muted-foreground/40 focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-500 transition-all outline-none font-medium"
                  value={formData.nombre}
                  onChange={(e) => setFormData({ ...formData, nombre: e.target.value })}
                />
              </div>
              <div className="space-y-2">
                <label className="text-xs font-black uppercase tracking-widest text-muted-foreground ml-1">Descripción</label>
                <textarea
                  id={descripcionInputId}
                  placeholder="Describe brevemente esta especialidad..."
                  rows={4}
                  className="w-full px-4 py-3 rounded-xl bg-background border border-border text-foreground placeholder:text-muted-foreground/40 focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-500 transition-all outline-none font-medium resize-none"
                  value={formData.descripcion}
                  onChange={(e) => setFormData({ ...formData, descripcion: e.target.value })}
                />
              </div>

              <div className="flex justify-end gap-3 pt-4 border-t border-border mt-6 font-bold">
                <button
                  type="button"
                  onClick={cerrarModal}
                  className="px-6 py-3 rounded-xl border border-border text-foreground hover:bg-muted transition-all active:scale-95"
                >
                  Cancelar
                </button>
                <button
                  type="submit"
                  className="px-6 py-3 rounded-xl bg-indigo-600 hover:bg-indigo-500 text-white shadow-lg shadow-indigo-500/20 transition-all active:scale-95"
                >
                  {modoEdicion ? "Guardar Cambios" : "Crear Especialidad"}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
