"use client";

import { useEffect, useState } from "react";
import { Search, Star, MapPin, Clock, Stethoscope, Loader2 } from "lucide-react";
import { MedicoDTO } from "@/types/api.types";
import { MedicoService } from "@/services/medico.service";
import { AgendarCitaModal } from "@/components/citas/AgendarCita";
import { usePageTransition } from "@/components/animations/Animatedcomponents";

const specialties = ["Todos"];

export default function BuscarMedicosPage() {
  const [medicos, setMedicos] = useState<MedicoDTO[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState("");
  const [selectedSpecialty, setSelectedSpecialty] = useState("Todos");
  const [selectedMedico, setSelectedMedico] = useState<MedicoDTO | null>(null);

  useEffect(() => {
    const fetchMedicos = async () => {
      try {
        const data = await MedicoService.obtenerTodos();
        setMedicos(data.filter(m => m.activo));
      } catch {
      } finally {
        setLoading(false);
      }
    };
    fetchMedicos();
  }, []);

  const allSpecialties = ["Todos", ...Array.from(new Set(medicos.map(m => m.especialidadNombre).filter(Boolean)))];

  const filtered = medicos.filter(m => {
    const matchesSearch =
      m.nombre.toLowerCase().includes(searchTerm.toLowerCase()) ||
      m.especialidadNombre?.toLowerCase().includes(searchTerm.toLowerCase());
    const matchesSpecialty = selectedSpecialty === "Todos" || m.especialidadNombre === selectedSpecialty;
    return matchesSearch && matchesSpecialty;
  });

  usePageTransition();

  return (
    <div className="space-y-8 page-content animate-in fade-in duration-500">
      <header className="relative overflow-hidden rounded-3xl border border-emerald-500/20 bg-gradient-to-br from-emerald-500/15 via-white dark:via-slate-950 to-teal-500/15 p-6 md:p-7 shadow-sm">
        <div className="absolute -right-16 -top-20 h-56 w-56 rounded-full bg-emerald-500/10 dark:bg-emerald-500/20 blur-3xl opacity-50" />
        <div className="relative z-10">
          <h1 className="text-3xl font-black tracking-tight text-foreground flex items-center gap-2">
            <Stethoscope className="w-8 h-8 text-emerald-600 dark:text-emerald-400" />
            Directorio Médico
          </h1>
          <p className="text-muted-foreground font-medium mt-1">Encuentra y agenda citas con nuestros médicos especialistas</p>
        </div>
      </header>

      <div className="bg-card/50 backdrop-blur-md border border-border rounded-2xl p-5 mb-6 shadow-sm">
        <div className="relative mb-4">
          <Search className="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-muted-foreground" />
          <input
            type="text"
            value={searchTerm}
            onChange={e => setSearchTerm(e.target.value)}
            placeholder="Buscar médico o especialidad..."
            className="w-full bg-background border border-border rounded-xl py-3 pl-12 pr-4 text-foreground placeholder:text-muted-foreground/50 focus:outline-none focus:ring-2 focus:ring-emerald-500/20 focus:border-emerald-500 transition-all shadow-sm"
          />
        </div>
        <div className="flex flex-wrap gap-2">
          {allSpecialties.map(s => (
            <button
              key={s}
              onClick={() => setSelectedSpecialty(s)}
              className={`px-4 py-2 rounded-xl text-sm font-bold transition-all ${
                selectedSpecialty === s
                  ? "bg-emerald-600 text-white shadow-lg shadow-emerald-500/20"
                  : "bg-muted/50 text-muted-foreground hover:bg-muted hover:text-foreground border border-border"
              }`}
            >
              {s}
            </button>
          ))}
        </div>
      </div>

      {loading ? (
        <div className="flex justify-center py-12">
          <Loader2 className="w-8 h-8 animate-spin text-[#10b981]" />
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {filtered.map(medico => (
            <div
              key={medico.id}
              className="bg-card border border-border rounded-2xl p-5 hover:shadow-xl transition-all group hover:-translate-y-1"
            >
              <div className="flex items-start gap-4 mb-5">
                <div className="w-16 h-16 rounded-2xl bg-emerald-500/10 border-2 border-emerald-500/10 flex items-center justify-center text-emerald-600 dark:text-emerald-400 font-black text-xl shadow-inner group-hover:scale-110 transition-transform">
                  {medico.nombre.split(" ").map((n: string) => n[0]).join("").slice(0, 2).toUpperCase()}
                </div>
                <div className="flex-1">
                  <p className="text-foreground font-black text-lg leading-tight group-hover:text-emerald-500 transition-colors">{medico.nombre}</p>
                  <p className="text-emerald-600 dark:text-emerald-400 text-xs font-black uppercase tracking-widest mt-0.5">{medico.especialidadNombre || "Especialista"}</p>
                  <div className="flex items-center gap-1.5 mt-2">
                    <Star className="w-4 h-4 text-amber-400 fill-amber-400" />
                    <span className="text-muted-foreground text-xs font-bold">4.9 (120 reseñas)</span>
                  </div>
                </div>
              </div>

              <div className="space-y-2.5 mb-6 pt-4 border-t border-border">
                <div className="flex items-center gap-2.5 text-muted-foreground text-sm font-semibold">
                  <MapPin className="w-4 h-4 text-emerald-500" />
                  <span>Consultorio Principal</span>
                </div>
                <div className="flex items-center gap-2.5 text-sm font-bold">
                  <Clock className="w-4 h-4 text-emerald-500" />
                  <span className="text-emerald-600 dark:text-emerald-400">Disponible mañana</span>
                </div>
              </div>

              <div className="flex gap-3">
                <button
                  onClick={() => setSelectedMedico(medico)}
                  className="flex-1 bg-emerald-600 hover:bg-emerald-500 text-white text-sm font-bold py-3 rounded-xl transition-all shadow-lg shadow-emerald-500/20 active:scale-95 flex items-center justify-center gap-2"
                >
                  Agendar Cita
                </button>
              </div>
            </div>
          ))}
        </div>
      )}

      {!loading && filtered.length === 0 && (
        <div className="backdrop-blur-md bg-white/5 border border-white/10 rounded-2xl p-12 text-center">
          <Stethoscope className="w-12 h-12 text-white/20 mx-auto mb-4" />
          <p className="text-white/60">No se encontraron médicos con los filtros seleccionados.</p>
        </div>
      )}

      <AgendarCitaModal
        medico={selectedMedico}
        isOpen={!!selectedMedico}
        onClose={() => setSelectedMedico(null)}
      />
    </div>
  );
}