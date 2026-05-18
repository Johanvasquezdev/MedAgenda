"use client";

import { useState } from "react";
import { CreditCard, Loader2, Search } from "lucide-react";
import { PagoResponse } from "@/types/api.types";
import { PagoService } from "@/services/pago.service";
import { usePageTransition, AnimatedCard } from "@/components/animations/Animatedcomponents";

export default function PagosAdminPage() {
  usePageTransition();
  const [pacienteId, setPacienteId] = useState("");
  const [pagos, setPagos] = useState<PagoResponse[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const cargarPagos = async () => {
    if (!pacienteId) {
      setError("Ingrese un pacienteId válido.");
      return;
    }
    setError("");
    setLoading(true);
    try {
      const data = await PagoService.obtenerPorPaciente(Number(pacienteId));
      setPagos(data);
    } catch {
      setPagos([]);
      setError("No se pudo obtener pagos para ese paciente.");
    } finally {
      setLoading(false);
    }
  };

  const reembolsar = async (id: number) => {
    await PagoService.reembolsar(id);
    await cargarPagos();
  };

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
              <CreditCard className="h-8 w-8 text-indigo-600 dark:text-indigo-400" />
              Histórico de Pagos
            </h1>
            <p className="mt-2 max-w-2xl text-muted-foreground font-medium">
              Consulta transacciones por paciente, verifica estados y gestiona reembolsos de forma centralizada.
            </p>
          </div>
        </div>
      </header>

      <section className="bg-card/50 backdrop-blur-md border border-border rounded-2xl p-6 shadow-sm border-l-4 border-l-indigo-500/50">
        <div className="flex flex-col md:flex-row gap-4 items-start md:items-end">
          <div className="flex flex-col gap-2 w-full md:w-auto">
            <label className="text-xs font-black uppercase tracking-widest text-muted-foreground ml-1 flex items-center gap-2">
              <Search className="w-3 h-3" /> Identificador de Paciente
            </label>
            <input
              value={pacienteId}
              onChange={(e) => setPacienteId(e.target.value)}
              className="w-full md:w-64 px-4 py-3 rounded-xl bg-background border border-border text-foreground font-medium focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-500 outline-none transition-all placeholder:text-muted-foreground/30"
              placeholder="Ej: 17"
            />
          </div>
          <button
            onClick={cargarPagos}
            className="w-full md:w-auto px-8 py-3 rounded-xl bg-indigo-600 hover:bg-indigo-500 text-white font-black shadow-lg shadow-indigo-500/20 transition-all active:scale-95"
          >
            Buscar Transacciones
          </button>
          {error && <span className="text-rose-600 dark:text-rose-400 text-xs font-bold mb-3">{error}</span>}
        </div>
      </section>

      <AnimatedCard delay={200} className="border border-border rounded-2xl overflow-hidden bg-card shadow-sm transition-all">
        <div className="overflow-x-auto">
          <table className="w-full text-left text-sm">
            <thead className="bg-muted/50 border-b border-border text-muted-foreground">
              <tr>
                <th className="p-4 text-xs font-black uppercase tracking-widest">ID Transacción</th>
                <th className="p-4 text-xs font-black uppercase tracking-widest">Cita Relonada</th>
                <th className="p-4 text-xs font-black uppercase tracking-widest">Monto</th>
                <th className="p-4 text-xs font-black uppercase tracking-widest">Estado</th>
                <th className="p-4 text-xs font-black uppercase tracking-widest">Fecha</th>
                <th className="p-4 text-xs font-black uppercase tracking-widest text-right">Acciones</th>
              </tr>
            </thead>
            <tbody>
              {loading ? (
                <tr>
                  <td colSpan={6} className="p-16 text-center">
                    <div className="flex flex-col items-center gap-3">
                      <Loader2 className="animate-spin text-indigo-500 h-10 w-10" />
                      <span className="text-muted-foreground font-bold">Procesando consulta...</span>
                    </div>
                  </td>
                </tr>
              ) : pagos.length === 0 ? (
                <tr>
                  <td colSpan={6} className="p-16 text-center text-muted-foreground font-bold">
                    {pacienteId ? "No se encontraron transacciones para estos criterios." : "Inicia una búsqueda para visualizar los resultados."}
                  </td>
                </tr>
              ) : (
                pagos.map((p) => (
                  <tr key={p.id} className="border-b border-border/50 hover:bg-muted/30 transition-colors">
                    <td className="p-4 text-foreground font-mono text-xs font-bold leading-none">{p.id}</td>
                    <td className="p-4 text-foreground font-medium">#{p.citaId}</td>
                    <td className="p-4">
                      <span className="text-foreground font-black text-base">
                        {p.moneda} {p.monto.toLocaleString()}
                      </span>
                    </td>
                    <td className="p-4">
                      <span className={`px-3 py-1.5 rounded-full text-[10px] font-black uppercase tracking-widest border shadow-sm ${
                        p.estado === 'Completado' 
                          ? 'bg-emerald-500/10 border-emerald-500/30 text-emerald-600 dark:text-emerald-400'
                          : p.estado === 'Pendiente'
                          ? 'bg-amber-500/10 border-amber-500/30 text-amber-600 dark:text-amber-400'
                          : 'bg-rose-500/10 border-rose-500/30 text-rose-600 dark:text-rose-400'
                      }`}>
                        {p.estado}
                      </span>
                    </td>
                    <td className="p-4 text-muted-foreground font-medium">
                      {new Date(p.fechaCreacion).toLocaleDateString(undefined, {
                        year: 'numeric',
                        month: 'long',
                        day: 'numeric'
                      })}
                    </td>
                    <td className="p-4 text-right">
                      <button
                        onClick={() => reembolsar(p.id)}
                        className="px-4 py-2 rounded-xl text-xs font-black border border-rose-500/40 text-rose-600 dark:text-rose-400 hover:bg-rose-500/10 transition-all active:scale-95 shadow-sm"
                      >
                        Reembolsar
                      </button>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </AnimatedCard>
    </div>
  );
}
