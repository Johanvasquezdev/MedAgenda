import { Clock, MoreVertical, Calendar as CalendarIcon } from "lucide-react";
import { EstadoCita } from "@/types/api.types";

interface CitaCardProps {
  doctorNombre: string;
  especialidad: string;
  fecha: string;
  hora: string;
  estado: EstadoCita | string;
}

export function CitaCard({ doctorNombre, especialidad, fecha, hora, estado }: CitaCardProps) {
  // Determina colores del badge segun el estado de la cita.
  const getEstadoStyles = (estadoStr: string) => {
    switch (estadoStr.toLowerCase()) {
      case "confirmada":
        return "bg-emerald-100 text-emerald-700 dark:bg-emerald-900/30 dark:text-emerald-400";
      case "solicitada":
        return "bg-amber-100 text-amber-700 dark:bg-amber-900/30 dark:text-amber-400";
      case "enprogreso":
        return "bg-blue-100 text-blue-700 dark:bg-blue-900/30 dark:text-blue-400";
      case "cancelada":
        return "bg-rose-100 text-rose-700 dark:bg-rose-900/30 dark:text-rose-400";
      case "completada":
        return "bg-slate-100 text-slate-700 dark:bg-slate-800 dark:text-slate-300";
      case "rechazada":
        return "bg-rose-100 text-rose-700 dark:bg-rose-900/30 dark:text-rose-400";
      case "noasistio":
        return "bg-slate-100 text-slate-700 dark:bg-slate-800 dark:text-slate-300";
      default:
        return "bg-slate-100 text-slate-700 dark:bg-slate-800 dark:text-slate-300";
    }
  };

  return (
    <div className="bg-white dark:bg-slate-900 p-5 rounded-2xl border border-slate-200 dark:border-slate-800 flex flex-col md:flex-row gap-5 justify-between items-start md:items-center hover:shadow-md transition-shadow">
      <div className="flex gap-4 items-center">
        <div className="bg-emerald-50 dark:bg-emerald-900/20 w-12 h-12 rounded-xl flex items-center justify-center text-emerald-600 dark:text-emerald-400">
          <CalendarIcon className="w-6 h-6" />
        </div>
        <div>
          <h4 className="font-semibold text-slate-900 dark:text-white">{doctorNombre}</h4>
          <p className="text-sm text-emerald-600 dark:text-emerald-500">{especialidad}</p>
        </div>
      </div>

      <div className="flex flex-col sm:flex-row gap-4 sm:gap-8 text-sm text-slate-500 dark:text-slate-400">
        <div className="flex items-center gap-1.5">
          <CalendarIcon className="w-4 h-4 text-slate-400" />
          <span>{fecha}</span>
        </div>
        <div className="flex items-center gap-1.5">
          <Clock className="w-4 h-4 text-slate-400" />
          <span>{hora}</span>
        </div>
      </div>

      <div className="flex items-center justify-between w-full md:w-auto gap-4">
        <span className={`px-3 py-1 rounded-full text-xs font-semibold ${getEstadoStyles(estado)}`}>
          {estado}
        </span>
        <button className="p-2 text-slate-400 hover:text-emerald-600 hover:bg-slate-50 dark:hover:bg-slate-800 rounded-lg transition-colors">
          <MoreVertical aria-hidden="true" className="w-5 h-5" />
        </button>
      </div>
    </div>
  );
}
