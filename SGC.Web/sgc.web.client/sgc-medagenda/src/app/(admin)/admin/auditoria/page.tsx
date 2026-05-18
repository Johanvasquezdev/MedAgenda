"use client";

import { useState, useEffect } from "react";
import { Shield, Loader2, History, Database, UserCheck } from "lucide-react";
import { AuditoriaResponse } from "@/types/api.types";
import { AuditoriaService } from "@/services/auditoria.service";
import { usePageTransition, AnimatedCard } from "@/components/animations/Animatedcomponents";

export default function AuditoriaPage() {
  usePageTransition();
  const [registros, setRegistros] = useState<AuditoriaResponse[]>([]);
  const [loading, setLoading] = useState(false);
  const [entidad, setEntidad] = useState("CITA");

  useEffect(() => {
    const fetch = async () => {
      setLoading(true);
      try {
        setRegistros(await AuditoriaService.obtenerRegistros(entidad));
      } catch {
        setRegistros([]);
      } finally {
        setLoading(false);
      }
    };
    fetch();
  }, [entidad]);

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
              <Shield className="h-8 w-8 text-indigo-600 dark:text-indigo-400" />
              Auditoría de Sistema
            </h1>
            <p className="mt-2 max-w-2xl text-muted-foreground font-medium">
              Trazabilidad completa de operaciones y cambios realizados en las entidades del negocio.
            </p>
          </div>

          <div className="flex flex-col gap-2 w-full md:w-64">
            <label htmlFor="entidad-filtro" className="text-[10px] font-black uppercase tracking-widest text-muted-foreground ml-1">
              Filtrar por Entidad
            </label>
            <select
              id="entidad-filtro"
              value={entidad}
              onChange={(e) => setEntidad(e.target.value)}
              className="px-4 py-2.5 rounded-xl bg-card border border-border text-foreground font-bold focus:ring-2 focus:ring-indigo-500/20 outline-none transition-all shadow-sm"
            >
              <option value="CITA">Citas</option>
              <option value="USUARIO">Usuarios</option>
              <option value="PAGO">Pagos</option>
            </select>
          </div>
        </div>
      </header>

      <AnimatedCard delay={200} className="border border-border rounded-2xl overflow-hidden bg-card shadow-sm transition-all">
        <div className="overflow-x-auto">
          <table className="w-full text-left text-sm">
            <thead className="bg-muted/50 border-b border-border text-muted-foreground">
              <tr>
                <th className="p-4 text-xs font-black uppercase tracking-widest"><div className="flex items-center gap-2"><History className="w-3.5 h-3.5" /> Acción</div></th>
                <th className="p-4 text-xs font-black uppercase tracking-widest"><div className="flex items-center gap-2"><UserCheck className="w-3.5 h-3.5" /> Usuario</div></th>
                <th className="p-4 text-xs font-black uppercase tracking-widest"><div className="flex items-center gap-2"><Database className="w-3.5 h-3.5" /> Entidad</div></th>
                <th className="p-4 text-xs font-black uppercase tracking-widest">Cambios Efectuados</th>
                <th className="p-4 text-xs font-black uppercase tracking-widest text-right">Fecha y Hora</th>
              </tr>
            </thead>
            <tbody>
              {loading ? (
                <tr>
                  <td colSpan={5} className="p-16 text-center">
                    <div className="flex flex-col items-center gap-3">
                      <Loader2 className="animate-spin text-indigo-500 h-10 w-10" />
                      <span className="text-muted-foreground font-bold">Cargando registros...</span>
                    </div>
                  </td>
                </tr>
              ) : registros.length === 0 ? (
                <tr>
                  <td colSpan={5} className="p-16 text-center text-muted-foreground font-bold">
                    No se encontraron registros de auditoría para la entidad "{entidad}".
                  </td>
                </tr>
              ) : (
                registros.map((r) => (
                  <tr key={r.id} className="border-b border-border/50 hover:bg-muted/20 transition-colors">
                    <td className="p-4">
                      <span className={`px-2.5 py-1 rounded-lg text-[10px] font-black uppercase tracking-widest border shadow-sm ${
                        r.accion === 'CREATE' ? 'bg-emerald-500/10 border-emerald-500/30 text-emerald-600 dark:text-emerald-400' :
                        r.accion === 'UPDATE' ? 'bg-indigo-500/10 border-indigo-500/30 text-indigo-600 dark:text-indigo-400' :
                        'bg-rose-500/10 border-rose-500/30 text-rose-600 dark:text-rose-400'
                      }`}>
                        {r.accion}
                      </span>
                    </td>
                    <td className="p-4 text-foreground font-bold">{r.usuarioId || "Sistema"}</td>
                    <td className="p-4 text-muted-foreground font-medium">{r.entidad}</td>
                    <td className="p-4">
                      <div className="flex items-center gap-2 text-xs">
                        {r.valorAnterior && (
                          <>
                            <span className="text-muted-foreground/50 line-through truncate max-w-[150px] italic">{r.valorAnterior}</span>
                            <span className="text-muted-foreground/30">→</span>
                          </>
                        )}
                        <span className="text-indigo-600 dark:text-indigo-400 font-black truncate max-w-[200px]">{r.valorNuevo || "-"}</span>
                      </div>
                    </td>
                    <td className="p-4 text-muted-foreground text-right font-mono text-[10px] font-bold">
                      {new Date(r.fecha).toLocaleString(undefined, {
                        dateStyle: 'medium',
                        timeStyle: 'short'
                      })}
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
