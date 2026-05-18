"use client";

import { useState, useEffect, useMemo } from "react";
import { Users, Loader2, Search, UserCheck, UserX } from "lucide-react";
import { UsuarioResponse } from "@/types/api.types";
import { UsuarioService } from "@/services/usuario.service";
import { AnimatedStatsCard } from "@/components/animations/Animatedstatscard";
import { AnimatedCard, usePageTransition } from "@/components/animations/Animatedcomponents";

export default function UsuariosPage() {
  usePageTransition();
  const [usuarios, setUsuarios] = useState<UsuarioResponse[]>([]);
  const [loading, setLoading] = useState(true);
  const [query, setQuery] = useState("");

  const cargar = async () => {
    setLoading(true);
    setUsuarios(await UsuarioService.obtenerTodos());
    setLoading(false);
  };

  useEffect(() => {
    queueMicrotask(() => {
      void cargar();
    });
  }, []);

  const toggleActivo = async (id: number, activo: boolean) => {
    setUsuarios((u) => u.map((x) => (x.id === id ? { ...x, activo: !activo } : x)));
    try {
      activo ? await UsuarioService.desactivar(id) : await UsuarioService.activar(id);
    } catch {
      cargar();
    }
  };

  const totalUsuarios = usuarios.length;
  const usuariosActivos = useMemo(() => usuarios.filter((u) => u.activo).length, [usuarios]);
  const usuariosInactivos = totalUsuarios - usuariosActivos;

  const usuariosFiltrados = useMemo(() => {
    const term = query.trim().toLowerCase();
    if (!term) return usuarios;

    return usuarios.filter((u) => {
      return (
        u.nombre.toLowerCase().includes(term) ||
        u.email.toLowerCase().includes(term) ||
        u.rol.toLowerCase().includes(term)
      );
    });
  }, [usuarios, query]);

  return (
    <div className="mx-auto max-w-7xl space-y-6 p-6 page-content">
      <header className="relative overflow-hidden rounded-3xl border border-indigo-500/20 bg-gradient-to-br from-indigo-500/15 via-white dark:via-slate-950 to-purple-500/15 p-6 md:p-7 shadow-sm">
        <div className="absolute -right-16 -top-16 h-52 w-52 rounded-full bg-indigo-500/10 dark:bg-indigo-500/20 blur-3xl opacity-50" />
        <div className="absolute -bottom-20 -left-20 h-56 w-56 rounded-full bg-purple-500/10 dark:bg-purple-500/20 blur-3xl opacity-50" />

        <div className="relative z-10 flex flex-col gap-6 md:flex-row md:items-end md:justify-between">
          <div>
            <p className="text-xs font-black uppercase tracking-[0.2em] text-indigo-600 dark:text-indigo-400">
              Administración
            </p>
            <h1 className="mt-2 flex items-center gap-2 text-3xl font-black tracking-tight text-foreground">
              <Users className="h-8 w-8 text-indigo-600 dark:text-indigo-400" />
              Gestión de Usuarios
            </h1>
            <p className="mt-2 max-w-2xl text-muted-foreground font-medium">
              Administra activación de cuentas y controla el acceso general de la plataforma.
            </p>
          </div>

          <div className="rounded-2xl border border-border bg-card/40 px-5 py-3 backdrop-blur-sm shadow-sm border-l-4 border-l-indigo-500/50">
            <p className="text-xs text-muted-foreground font-black uppercase tracking-widest">Registros Totales</p>
            <p className="text-2xl font-black text-foreground leading-tight">{loading ? "--" : totalUsuarios}</p>
          </div>
        </div>
      </header>

      <section className="grid gap-4 md:grid-cols-3">
        <AnimatedStatsCard
          title="Total usuarios"
          value={loading ? 0 : totalUsuarios}
          icon={Users}
          description="Registrados"
          delay={100}
          variant="purple"
        />

        <AnimatedStatsCard
          title="Activos"
          value={loading ? 0 : usuariosActivos}
          icon={UserCheck}
          description="Cuentas habilitadas"
          delay={200}
          variant="emerald"
        />

        <AnimatedStatsCard
          title="Inactivos"
          value={loading ? 0 : usuariosInactivos}
          icon={UserX}
          description="Esperando activacion"
          delay={300}
          variant="rose"
        />
      </section>

      <section className="rounded-2xl border border-border bg-card/50 p-4 shadow-sm">
        <label className="relative block">
          <Search className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
          <input
            value={query}
            onChange={(e) => setQuery(e.target.value)}
            placeholder="Buscar por nombre, email o rol"
            className="w-full rounded-xl border border-border bg-background/50 py-2.5 pl-10 pr-3 text-sm text-foreground placeholder:text-muted-foreground focus:ring-2 focus:ring-purple-500/20 transition-all outline-none"
          />
        </label>
      </section>

      <AnimatedCard className="overflow-hidden rounded-2xl border border-border bg-card shadow-sm">
        <table className="w-full text-left text-sm">
          <thead className="border-b border-border bg-muted/30">
            <tr>
              <th className="p-4 text-muted-foreground font-semibold">Usuario</th>
              <th className="p-4 text-muted-foreground font-semibold">Rol</th>
              <th className="p-4 text-muted-foreground font-semibold">Estado</th>
            </tr>
          </thead>
          <tbody>
            {loading ? (
              <tr>
                <td colSpan={3} className="p-8 text-center">
                  <Loader2 className="mx-auto h-5 w-5 animate-spin text-purple-500" />
                </td>
              </tr>
            ) : usuariosFiltrados.length === 0 ? (
              <tr>
                <td colSpan={3} className="p-6 text-center text-muted-foreground">
                  No se encontraron usuarios para el filtro actual.
                </td>
              </tr>
            ) : (
              usuariosFiltrados.map((u) => (
                <tr key={u.id} className="border-b border-border/50 hover:bg-muted/20 transition-colors">
                  <td className="p-4">
                    <p className="font-semibold text-foreground">{u.nombre}</p>
                    <p className="text-xs text-muted-foreground">{u.email}</p>
                  </td>
                  <td className="p-4 text-foreground/80 font-medium">{u.rol}</td>
                  <td className="p-4">
                     <button
                      onClick={() => toggleActivo(u.id, u.activo)}
                      className={`rounded-full border px-4 py-1.5 text-[10px] font-black uppercase tracking-widest transition-all active:scale-95 shadow-sm ${
                        u.activo
                          ? "border-emerald-500/30 bg-emerald-500/10 text-emerald-600 dark:text-emerald-400 hover:bg-emerald-500/20"
                          : "border-rose-500/30 bg-rose-500/10 text-rose-600 dark:text-rose-400 hover:bg-rose-500/20"
                      }`}
                    >
                      {u.activo ? "Activo" : "Inactivo"}
                    </button>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </AnimatedCard>
    </div>
  );
}
