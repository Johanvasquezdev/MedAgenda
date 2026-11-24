import { CalendarDays, Clock3 } from "lucide-react";
import { EstadoCita } from "@/types/api.types";

interface CitaCardProps {
  doctorNombre: string;
  especialidad: string;
  fecha: string;
  hora: string;
  estado: EstadoCita | string;
}

function estadoStyles(estado: string) {
  switch (estado.toLowerCase()) {
    case "confirmada": return "bg-emerald-100 text-emerald-800 dark:bg-emerald-950 dark:text-emerald-200";
    case "solicitada": return "bg-amber-100 text-amber-900 dark:bg-amber-950 dark:text-amber-200";
    case "enprogreso": return "bg-sky-100 text-sky-900 dark:bg-sky-950 dark:text-sky-200";
    case "cancelada":
    case "rechazada": return "bg-rose-100 text-rose-900 dark:bg-rose-950 dark:text-rose-200";
    default: return "bg-secondary text-secondary-foreground";
  }
}

export function CitaCard({ doctorNombre, especialidad, fecha, hora, estado }: CitaCardProps) {
  return (
    <article className="flex flex-col gap-5 rounded-2xl border border-border bg-card p-5 transition-shadow hover:shadow-[0_14px_34px_-28px_rgba(8,22,16,0.7)] md:flex-row md:items-center md:justify-between">
      <div className="flex min-w-0 items-center gap-4">
        <span aria-hidden="true" className="flex size-12 shrink-0 items-center justify-center rounded-xl bg-accent text-accent-foreground">
          <CalendarDays className="size-5" />
        </span>
        <div className="min-w-0">
          <h3 className="truncate font-semibold text-foreground">{doctorNombre}</h3>
          <p className="mt-1 truncate text-sm text-muted-foreground">{especialidad}</p>
        </div>
      </div>

      <div className="flex flex-wrap items-center gap-x-6 gap-y-2 text-sm text-foreground">
        <span className="inline-flex items-center gap-2"><CalendarDays className="size-4 text-primary" aria-hidden="true" />{fecha}</span>
        <span className="inline-flex items-center gap-2"><Clock3 className="size-4 text-primary" aria-hidden="true" />{hora}</span>
      </div>

      <span className={`inline-flex min-h-8 items-center self-start rounded-full px-3 text-xs font-semibold md:self-center ${estadoStyles(estado)}`}>
        {estado}
      </span>
    </article>
  );
}
