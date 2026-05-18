"use client";

import { useEffect, useMemo, useState } from "react";
import { Building2, Plus, Loader2, X, Search, CheckCircle2, XCircle } from "lucide-react";
import { ProveedorSaludDTO, ProveedorSaludService } from "@/services/proveedor.service";
import { AnimatedStatsCard } from "@/components/animations/Animatedstatscard";
import { AnimatedCard, usePageTransition } from "@/components/animations/Animatedcomponents";

export default function ProveedoresPage() {
  usePageTransition();
  const busquedaInputId = "proveedores-busqueda";
  const [proveedores, setProveedores] = useState<ProveedorSaludDTO[]>([]);
  const [loading, setLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [modoEdicion, setModoEdicion] = useState(false);
  const [proveedorSeleccionado, setProveedorSeleccionado] = useState<ProveedorSaludDTO | null>(null);
  const [query, setQuery] = useState("");
  const [formData, setFormData] = useState({
    nombre: "",
    tipo: "",
    telefono: "",
    email: "",
  });

  const cargar = async () => {
    setLoading(true);
    try {
      const data = await ProveedorSaludService.obtenerTodos();
      setProveedores(data);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    cargar();
  }, []);

  const abrirCrear = () => {
    setModoEdicion(false);
    setProveedorSeleccionado(null);
    setFormData({ nombre: "", tipo: "", telefono: "", email: "" });
    setIsModalOpen(true);
  };

  const abrirEditar = (prov: ProveedorSaludDTO) => {
    setModoEdicion(true);
    setProveedorSeleccionado(prov);
    setFormData({
      nombre: prov.nombre,
      tipo: prov.tipo || "",
      telefono: prov.telefono || "",
      email: prov.email || "",
    });
    setIsModalOpen(true);
  };

  const cerrarModal = () => {
    setIsModalOpen(false);
    setProveedorSeleccionado(null);
  };

  const guardar = async (e: React.FormEvent) => {
    e.preventDefault();
    if (modoEdicion && proveedorSeleccionado) {
      await ProveedorSaludService.actualizar(proveedorSeleccionado.id, formData);
    } else {
      await ProveedorSaludService.crear(formData);
    }
    await cargar();
    cerrarModal();
  };

  const totalProveedores = proveedores.length;
  const proveedoresActivos = useMemo(() => proveedores.filter((p) => p.activo).length, [proveedores]);
  const proveedoresInactivos = totalProveedores - proveedoresActivos;

  const proveedoresFiltrados = useMemo(() => {
    const term = query.trim().toLowerCase();
    if (!term) return proveedores;

    return proveedores.filter((p) => {
      return (
        p.nombre.toLowerCase().includes(term) ||
        (p.tipo || "").toLowerCase().includes(term) ||
        (p.email || "").toLowerCase().includes(term) ||
        (p.telefono || "").toLowerCase().includes(term)
      );
    });
  }, [proveedores, query]);

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
              <Building2 className="h-8 w-8 text-indigo-600 dark:text-indigo-400" />
              Proveedores de Salud
            </h1>
            <p className="mt-2 max-w-2xl text-muted-foreground font-medium">
              Gestiona ARS, clínicas y hospitales para mantener actualizado el ecosistema de convenios.
            </p>
          </div>

          <button
            type="button"
            onClick={abrirCrear}
            className="inline-flex items-center gap-2 rounded-xl bg-indigo-600 hover:bg-indigo-500 px-5 py-2.5 text-sm font-bold text-white transition-all shadow-lg shadow-indigo-500/20 active:scale-95"
          >
            <Plus aria-hidden="true" className="h-4 w-4" /> Registrar proveedor
          </button>
        </div>
      </header>

      <section className="grid gap-4 md:grid-cols-3">
        <AnimatedStatsCard
          title="Total proveedores"
          value={loading ? 0 : totalProveedores}
          icon={Building2}
          description="En el sistema"
          delay={100}
        />

        <AnimatedStatsCard
          title="Activos"
          value={loading ? 0 : proveedoresActivos}
          icon={CheckCircle2}
          description="Convenios vigentes"
          delay={200}
          iconClassName="bg-emerald-500/10 text-emerald-400"
        />

        <AnimatedStatsCard
          title="Inactivos"
          value={loading ? 0 : proveedoresInactivos}
          icon={XCircle}
          description="Sin operacion"
          delay={300}
          iconClassName="bg-rose-500/10 text-rose-400"
        />
      </section>

      <section className="bg-card/50 backdrop-blur-md border border-border rounded-2xl p-4 shadow-sm border-l-4 border-l-indigo-500/50">
        <label htmlFor={busquedaInputId} className="relative block">
          <Search aria-hidden="true" className="pointer-events-none absolute left-4 h-5 w-5 top-1/2 -translate-y-1/2 text-muted-foreground" />
          <input
            id={busquedaInputId}
            aria-label="Buscar proveedores"
            value={query}
            onChange={(e) => setQuery(e.target.value)}
            placeholder="Buscar por nombre, tipo, teléfono o email..."
            className="w-full rounded-xl border border-border bg-background py-3 pl-12 pr-4 text-sm text-foreground placeholder:text-muted-foreground/50 focus:outline-none focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-500 transition-all shadow-sm"
          />
        </label>
      </section>

      <AnimatedCard delay={400} className="overflow-hidden rounded-2xl border border-border bg-card shadow-sm">
        <div className="overflow-x-auto">
          <table className="w-full text-left text-sm">
            <thead className="bg-muted/50 border-b border-border text-muted-foreground">
              <tr>
                <th className="p-4 text-xs font-black uppercase tracking-widest">Nombre</th>
                <th className="p-4 text-xs font-black uppercase tracking-widest">Tipo</th>
                <th className="p-4 text-xs font-black uppercase tracking-widest">Contacto</th>
                <th className="p-4 text-xs font-black uppercase tracking-widest text-center">Estado</th>
                <th className="p-4 text-xs font-black uppercase tracking-widest text-right">Acciones</th>
              </tr>
            </thead>
            <tbody>
              {loading ? (
                <tr>
                  <td colSpan={5} className="p-12 text-center">
                    <Loader2 className="animate-spin mx-auto text-indigo-500 h-8 w-8" />
                  </td>
                </tr>
              ) : proveedoresFiltrados.length === 0 ? (
                <tr>
                  <td colSpan={5} className="p-12 text-center text-muted-foreground font-bold">
                    No se encontraron proveedores para el filtro actual.
                  </td>
                </tr>
              ) : (
                proveedoresFiltrados.map((prov) => (
                  <tr key={prov.id} className="border-b border-border/50 hover:bg-muted/30 transition-colors group">
                    <td className="p-4">
                       <p className="font-bold text-foreground text-base group-hover:text-indigo-600 dark:group-hover:text-indigo-400 transition-colors">{prov.nombre}</p>
                    </td>
                    <td className="p-4">
                      <span className="px-3 py-1 bg-muted rounded-lg text-xs font-bold text-muted-foreground">{prov.tipo || "General"}</span>
                    </td>
                    <td className="p-4">
                      <div className="text-foreground font-semibold">{prov.telefono || "-"}</div>
                      <div className="text-xs text-muted-foreground font-medium">{prov.email || "-"}</div>
                    </td>
                    <td className="p-4 text-center">
                      <span
                        className={`px-3 py-1.5 rounded-full text-[10px] font-black uppercase tracking-widest shadow-sm ${
                          prov.activo
                            ? "bg-emerald-500/10 text-emerald-600 dark:text-emerald-400 border border-emerald-500/20"
                            : "bg-rose-500/10 text-rose-600 dark:text-rose-400 border border-rose-500/20"
                        }`}
                      >
                        {prov.activo ? "Activo" : "Inactivo"}
                      </span>
                    </td>
                    <td className="p-4 text-right">
                      <button
                        type="button"
                        onClick={() => abrirEditar(prov)}
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
                {modoEdicion ? "Editar Proveedor" : "Registrar Proveedor"}
              </h2>
              <button type="button" onClick={cerrarModal} aria-label="Cerrar modal" className="text-muted-foreground hover:text-foreground transition-colors">
                <X aria-hidden="true" className="w-6 h-6" />
              </button>
            </div>
            <form onSubmit={guardar} className="p-6 space-y-5">
              <div className="space-y-2">
                <label className="text-xs font-black uppercase tracking-widest text-muted-foreground ml-1">Nombre Comercial</label>
                <input
                  aria-label="Nombre del proveedor"
                  required
                  placeholder="Ej: Clinica Union Médica"
                  className="w-full px-4 py-3 rounded-xl bg-background border border-border text-foreground placeholder:text-muted-foreground/40 focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-500 transition-all outline-none font-medium"
                  value={formData.nombre}
                  onChange={(e) => setFormData({ ...formData, nombre: e.target.value })}
                />
              </div>
              <div className="space-y-2">
                <label className="text-xs font-black uppercase tracking-widest text-muted-foreground ml-1">Tipo de Institucion</label>
                <input
                  aria-label="Tipo de proveedor"
                  placeholder="Ej: ARS, Clínica, Hospital"
                  className="w-full px-4 py-3 rounded-xl bg-background border border-border text-foreground placeholder:text-muted-foreground/40 focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-500 transition-all outline-none font-medium"
                  value={formData.tipo}
                  onChange={(e) => setFormData({ ...formData, tipo: e.target.value })}
                />
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div className="space-y-2">
                  <label className="text-xs font-black uppercase tracking-widest text-muted-foreground ml-1">Teléfono</label>
                  <input
                    aria-label="Telefono"
                    placeholder="+1 (809)..."
                    className="w-full px-4 py-3 rounded-xl bg-background border border-border text-foreground placeholder:text-muted-foreground/40 focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-500 transition-all outline-none font-medium"
                    value={formData.telefono}
                    onChange={(e) => setFormData({ ...formData, telefono: e.target.value })}
                  />
                </div>
                <div className="space-y-2">
                  <label className="text-xs font-black uppercase tracking-widest text-muted-foreground ml-1">Email</label>
                  <input
                    aria-label="Correo electronico"
                    type="email"
                    placeholder="contacto@..."
                    className="w-full px-4 py-3 rounded-xl bg-background border border-border text-foreground placeholder:text-muted-foreground/40 focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-500 transition-all outline-none font-medium"
                    value={formData.email}
                    onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                  />
                </div>
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
                  {modoEdicion ? "Guardar Cambios" : "Completar Registro"}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
