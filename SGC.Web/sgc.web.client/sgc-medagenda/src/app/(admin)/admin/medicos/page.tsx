"use client";

import { useState, useEffect, useMemo } from "react";
import { Stethoscope, Search, Plus, CheckCircle2, XCircle, Loader2, Phone, X } from "lucide-react";
import Image from "next/image";
import { MedicoDTO, CreateMedicoRequest, UpdateMedicoRequest } from "@/types/medico.types";
import { MedicoService } from "@/services/medico.service";
import { EspecialidadService, EspecialidadDTO } from "@/services/especialidad.service";
import { ProveedorSaludService, ProveedorSaludDTO } from "@/services/proveedor.service";
import { AnimatedStatsCard } from "@/components/animations/Animatedstatscard";
import { AnimatedCard, usePageTransition } from "@/components/animations/Animatedcomponents";

export default function AdminMedicosPage() {
  usePageTransition();
  const [medicos, setMedicos] = useState<MedicoDTO[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [busqueda, setBusqueda] = useState("");
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [modoEdicion, setModoEdicion] = useState(false);
  const [especialidades, setEspecialidades] = useState<EspecialidadDTO[]>([]);
  const [proveedores, setProveedores] = useState<ProveedorSaludDTO[]>([]);
  const [formData, setFormData] = useState<CreateMedicoRequest>({
    nombre: "",
    email: "",
    password: "",
    exequatur: "",
    especialidadId: null,
    proveedorSaludId: null,
    telefonoConsultorio: "",
    foto: "",
  });
  const [medicoSeleccionado, setMedicoSeleccionado] = useState<MedicoDTO | null>(null);

  const cargarMedicos = async () => {
    setIsLoading(true);
    try {
      const data = await MedicoService.obtenerTodos();
      setMedicos(data);
    } catch (error) {
      console.error("Error cargando médicos:", error);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    cargarMedicos();
    const cargarCatalogos = async () => {
      const [esp, prov] = await Promise.all([
        EspecialidadService.obtenerTodas(),
        ProveedorSaludService.obtenerTodos(),
      ]);
      setEspecialidades(esp.filter((e) => e.activo));
      setProveedores(prov.filter((p) => p.activo));
    };
    cargarCatalogos();
  }, []);

  // Filtrado dinámico en el cliente
  const medicosFiltrados = useMemo(() => {
    const term = busqueda.trim().toLowerCase();
    if (!term) return medicos;

    return medicos.filter((m) => {
      return (
        m.nombre.toLowerCase().includes(term) ||
        (m.especialidadNombre || "").toLowerCase().includes(term) ||
        m.exequatur.toLowerCase().includes(term) ||
        (m.email || "").toLowerCase().includes(term)
      );
    });
  }, [medicos, busqueda]);

  const abrirCrear = () => {
    setModoEdicion(false);
    setMedicoSeleccionado(null);
    setFormData({
      nombre: "",
      email: "",
      password: "",
      exequatur: "",
      especialidadId: null,
      proveedorSaludId: null,
      telefonoConsultorio: "",
      foto: "",
    });
    setIsModalOpen(true);
  };

  const abrirEditar = (medico: MedicoDTO) => {
    setModoEdicion(true);
    setMedicoSeleccionado(medico);
    setFormData({
      nombre: medico.nombre,
      email: medico.email,
      password: "",
      exequatur: medico.exequatur,
      especialidadId: medico.especialidadId ?? null,
      proveedorSaludId: medico.proveedorSaludId ?? null,
      telefonoConsultorio: medico.telefonoConsultorio || "",
      foto: medico.foto || "",
    });
    setIsModalOpen(true);
  };

  const cerrarModal = () => {
    setIsModalOpen(false);
    setMedicoSeleccionado(null);
  };

  const guardarMedico = async (e: React.FormEvent) => {
    e.preventDefault();
    if (modoEdicion && medicoSeleccionado) {
      const updateData: UpdateMedicoRequest = {
        nombre: formData.nombre,
        email: formData.email,
        exequatur: formData.exequatur,
        especialidadId: formData.especialidadId,
        proveedorSaludId: formData.proveedorSaludId,
        telefonoConsultorio: formData.telefonoConsultorio,
        foto: formData.foto,
      };
      await MedicoService.actualizar(medicoSeleccionado.id, updateData);
    } else {
      await MedicoService.crear(formData);
    }
    await cargarMedicos();
    cerrarModal();
  };

  const toggleActivo = async (medico: MedicoDTO) => {
    if (medico.activo) {
      await MedicoService.desactivar(medico.id);
    } else {
      await MedicoService.activar(medico.id);
    }
    await cargarMedicos();
  };

  const totalMedicos = medicos.length;
  const medicosActivos = medicos.filter((m) => m.activo).length;
  const medicosInactivos = totalMedicos - medicosActivos;

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
              Directorio de Médicos
            </h1>
            <p className="mt-2 max-w-2xl text-muted-foreground font-medium">
              Gestiona perfiles profesionales, especialidades y estado operativo del personal médico.
            </p>
          </div>

          <button
            type="button"
            onClick={abrirCrear}
            className="inline-flex items-center gap-2 rounded-xl bg-indigo-600 hover:bg-indigo-500 px-5 py-2.5 text-sm font-bold text-white transition-all shadow-lg shadow-indigo-500/20 active:scale-95"
          >
            <Plus aria-hidden="true" className="h-4 w-4" /> Registrar médico
          </button>
        </div>
      </header>

      <section className="grid gap-4 md:grid-cols-3">
        <AnimatedStatsCard
          title="Total medicos"
          value={isLoading ? 0 : totalMedicos}
          icon={Stethoscope}
          description="En el Directorio"
          delay={100}
        />
        <AnimatedStatsCard
          title="Activos"
          value={isLoading ? 0 : medicosActivos}
          icon={CheckCircle2}
          description="Habilitados"
          delay={200}
          iconClassName="bg-emerald-500/10 text-emerald-400"
        />
        <AnimatedStatsCard
          title="Inactivos"
          value={isLoading ? 0 : medicosInactivos}
          icon={XCircle}
          description="Fuera de servicio"
          delay={300}
          iconClassName="bg-rose-500/10 text-rose-400"
        />
      </section>

      <AnimatedCard delay={400} className="bg-card/50 backdrop-blur-sm rounded-2xl shadow-sm border border-border overflow-hidden">
        <div className="p-4 border-b border-border flex flex-col gap-3 bg-muted/20 sm:flex-row sm:items-center sm:justify-between">
          <div className="relative w-full max-w-sm">
            <Search aria-hidden="true" className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
            <input 
              id="medicos-busqueda"
              aria-label="Buscar medicos"
              type="text"
              placeholder="Buscar por nombre, especialidad, exequatur o email"
              className="w-full pl-9 pr-4 py-2 rounded-lg border border-border bg-background text-sm focus:ring-2 focus:ring-purple-500/20 outline-none transition-all text-foreground placeholder:text-muted-foreground"
              value={busqueda}
              onChange={(e) => setBusqueda(e.target.value)}
            />
          </div>
          <p className="text-xs text-muted-foreground font-medium">
            {isLoading ? "Cargando..." : `${medicosFiltrados.length} resultado(s)`}
          </p>
        </div>

        <div className="overflow-x-auto">
          <table className="w-full text-sm text-left">
            <thead className="text-xs text-muted-foreground uppercase bg-muted/30 border-b border-border">
              <tr>
                <th className="px-6 py-4 font-semibold">Médico</th>
                <th className="px-6 py-4 font-semibold">Especialidad</th>
                <th className="px-6 py-4 font-semibold">Credenciales</th>
                <th className="px-6 py-4 font-semibold text-center">Estado</th>
                <th className="px-6 py-4 font-semibold text-right">Acciones</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-border/50">
              {isLoading ? (
                <tr>
                  <td colSpan={5} className="px-6 py-12 text-center">
                    <Loader2 className="w-6 h-6 animate-spin text-indigo-400 mx-auto" />
                    <p className="text-slate-400 mt-2">Cargando directorio...</p>
                  </td>
                </tr>
              ) : medicosFiltrados.length === 0 ? (
                <tr>
                  <td colSpan={5} className="px-6 py-8 text-center text-slate-400">
                    No se encontraron medicos para el filtro actual.
                  </td>
                </tr>
              ) : (
                medicosFiltrados.map((medico) => (
                  <tr key={medico.id} className="hover:bg-muted/10 transition-colors">
                    <td className="px-6 py-4">
                      <div className="flex items-center gap-3">
                        <Image
                          src={medico.foto || `https://ui-avatars.com/api/?name=${encodeURIComponent(medico.nombre)}&background=e0e7ff&color=4338ca`}
                          alt="Avatar"
                          width={40}
                          height={40}
                          unoptimized
                          className="w-10 h-10 rounded-full object-cover border border-border shadow-sm"
                        />
                        <div>
                          <div className="font-bold text-foreground">{medico.nombre}</div>
                          <div className="text-muted-foreground text-xs mt-0.5">{medico.email}</div>
                        </div>
                      </div>
                    </td>
                    <td className="px-6 py-4">
                      <span className="bg-emerald-500/10 text-emerald-600 dark:text-emerald-300 px-2.5 py-1 rounded-md text-xs font-semibold border border-emerald-500/20 dark:border-emerald-500/30">
                        {medico.especialidadNombre || "No asignada"}
                      </span>
                    </td>
                    <td className="px-6 py-4">
                      <div className="text-foreground text-xs font-mono font-bold mb-1">
                        Exq: {medico.exequatur}
                      </div>
                      <div className="text-muted-foreground text-xs flex items-center gap-1 font-medium">
                        <Phone className="w-3 h-3" /> {medico.telefonoConsultorio}
                      </div>
                    </td>
                    <td className="px-6 py-4 text-center">
                       <span className={`inline-flex items-center gap-1.5 px-3 py-1 rounded-full text-[10px] font-black uppercase tracking-widest border shadow-sm ${
                          medico.activo 
                            ? 'bg-emerald-500/10 border-emerald-500/30 text-emerald-600 dark:text-emerald-400' 
                            : 'bg-rose-500/10 border-rose-500/30 text-rose-600 dark:text-rose-400'
                        }`}>
                        {medico.activo ? <CheckCircle2 className="w-3.5 h-3.5" /> : <XCircle className="w-3.5 h-3.5" />}
                        {medico.activo ? "Activo" : "Inactivo"}
                      </span>
                    </td>
                    <td className="px-6 py-4 text-right">
                      <div className="flex items-center justify-end gap-2">
                        <button
                          type="button"
                          onClick={() => abrirEditar(medico)}
                          className="px-3 py-1.5 rounded-lg text-xs font-semibold border border-border text-foreground hover:bg-muted transition-colors"
                        >
                          Editar
                        </button>
                        <button
                          type="button"
                          onClick={() => toggleActivo(medico)}
                          className={`px-3 py-1.5 rounded-lg text-xs font-bold border transition-all active:scale-95 ${
                            medico.activo
                              ? "border-rose-500/40 text-rose-600 dark:text-rose-400 hover:bg-rose-500/10"
                              : "border-emerald-500/40 text-emerald-600 dark:text-emerald-400 hover:bg-emerald-500/10"
                          }`}
                        >
                          {medico.activo ? "Desactivar" : "Activar"}
                        </button>
                      </div>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </AnimatedCard>

      {isModalOpen && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-background/80 backdrop-blur-md p-4">
          <div className="w-full max-w-lg bg-card border border-border rounded-2xl overflow-hidden shadow-2xl scale-in-center">
            <div className="flex items-center justify-between px-5 py-4 border-b border-border bg-muted/30">
              <h2 className="text-xl font-black text-foreground">
                {modoEdicion ? "Editar Perfil Médico" : "Registro de Nuevo Médico"}
              </h2>
              <button type="button" onClick={cerrarModal} aria-label="Cerrar modal" className="text-muted-foreground hover:text-foreground transition-colors">
                <X aria-hidden="true" className="w-5 h-5" />
              </button>
            </div>
            <form onSubmit={guardarMedico} className="p-6 space-y-4">
              <div className="grid gap-4">
                <div className="space-y-1">
                  <label className="text-xs font-bold text-muted-foreground uppercase ml-1">Nombre Completo</label>
                  <input
                    aria-label="Nombre completo"
                    required
                    placeholder="Ej. Dr. Juan Pérez"
                    className="w-full px-4 py-2.5 rounded-xl bg-background border border-border text-foreground placeholder:text-muted-foreground focus:ring-2 focus:ring-purple-500/20 outline-none transition-all"
                    value={formData.nombre}
                    onChange={(e) => setFormData({ ...formData, nombre: e.target.value })}
                  />
                </div>
                <div className="space-y-1">
                  <label className="text-xs font-bold text-muted-foreground uppercase ml-1">Correo Electrónico</label>
                  <input
                    aria-label="Correo electronico"
                    required
                    type="email"
                    placeholder="medico@ejemplo.com"
                    className="w-full px-4 py-2.5 rounded-xl bg-background border border-border text-foreground placeholder:text-muted-foreground focus:ring-2 focus:ring-purple-500/20 outline-none transition-all"
                    value={formData.email}
                    onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                  />
                </div>
                {!modoEdicion && (
                  <div className="space-y-1">
                    <label className="text-xs font-bold text-muted-foreground uppercase ml-1">Contraseña</label>
                    <input
                      aria-label="Contrasena"
                      required
                      type="password"
                      placeholder="••••••••"
                      className="w-full px-4 py-2.5 rounded-xl bg-background border border-border text-foreground placeholder:text-muted-foreground focus:ring-2 focus:ring-purple-500/20 outline-none transition-all"
                      value={formData.password}
                      onChange={(e) => setFormData({ ...formData, password: e.target.value })}
                    />
                  </div>
                )}
                <div className="grid grid-cols-2 gap-4">
                  <div className="space-y-1">
                    <label className="text-xs font-bold text-muted-foreground uppercase ml-1">Exequatur</label>
                    <input
                      aria-label="Exequatur"
                      placeholder="000000"
                      className="w-full px-4 py-2.5 rounded-xl bg-background border border-border text-foreground placeholder:text-muted-foreground focus:ring-2 focus:ring-purple-500/20 outline-none transition-all"
                      value={formData.exequatur || ""}
                      onChange={(e) => setFormData({ ...formData, exequatur: e.target.value })}
                    />
                  </div>
                  <div className="space-y-1">
                    <label className="text-xs font-bold text-muted-foreground uppercase ml-1">Teléfono</label>
                    <input
                      aria-label="Telefono de consultorio"
                      placeholder="809-000-0000"
                      className="w-full px-4 py-2.5 rounded-xl bg-background border border-border text-foreground placeholder:text-muted-foreground focus:ring-2 focus:ring-purple-500/20 outline-none transition-all"
                      value={formData.telefonoConsultorio || ""}
                      onChange={(e) => setFormData({ ...formData, telefonoConsultorio: e.target.value })}
                    />
                  </div>
                </div>
                <div className="grid grid-cols-2 gap-4">
                   <div className="space-y-1">
                    <label className="text-xs font-bold text-muted-foreground uppercase ml-1">Especialidad</label>
                    <select
                      aria-label="Seleccionar especialidad"
                      className="w-full px-4 py-2.5 rounded-xl bg-background border border-border text-foreground focus:ring-2 focus:ring-purple-500/20 outline-none transition-all"
                      value={formData.especialidadId ?? ""}
                      onChange={(e) =>
                        setFormData({ ...formData, especialidadId: e.target.value ? Number(e.target.value) : null })
                      }
                    >
                      <option value="">Seleccionar</option>
                      {especialidades.map((esp) => (
                        <option key={esp.id} value={esp.id}>
                          {esp.nombre}
                        </option>
                      ))}
                    </select>
                  </div>
                  <div className="space-y-1">
                    <label className="text-xs font-bold text-muted-foreground uppercase ml-1">Proveedor (ARS)</label>
                    <select
                      aria-label="Seleccionar proveedor de salud"
                      className="w-full px-4 py-2.5 rounded-xl bg-background border border-border text-foreground focus:ring-2 focus:ring-purple-500/20 outline-none transition-all"
                      value={formData.proveedorSaludId ?? ""}
                      onChange={(e) =>
                        setFormData({ ...formData, proveedorSaludId: e.target.value ? Number(e.target.value) : null })
                      }
                    >
                      <option value="">Seleccionar</option>
                      {proveedores.map((prov) => (
                        <option key={prov.id} value={prov.id}>
                          {prov.nombre}
                        </option>
                      ))}
                    </select>
                  </div>
                </div>
              </div>

              <div className="flex justify-end gap-3 pt-6 border-t border-border mt-6">
                <button
                  type="button"
                  onClick={cerrarModal}
                  className="px-6 py-2.5 rounded-xl border border-border text-foreground font-semibold hover:bg-muted transition-all"
                >
                  Cancelar
                </button>
                <button
                  type="submit"
                  className="px-8 py-2.5 rounded-xl bg-indigo-600 hover:bg-indigo-500 text-white font-black shadow-lg shadow-indigo-500/20 transition-all active:scale-95"
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
