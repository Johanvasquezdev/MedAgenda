import { CalendarClock, CreditCard, Info } from "lucide-react";

interface NotificacionCardProps {
  tipo: "cita" | "pago" | "info" | string;
  titulo: string;
  mensaje: string;
  fecha: string;
  leida: boolean;
  onClick: () => void;
}

export function NotificacionCard({ tipo, titulo, mensaje, fecha, leida, onClick }: NotificacionCardProps) {
  const Icon = tipo === "cita" ? CalendarClock : tipo === "pago" ? CreditCard : Info;

  return (
    <button
      type="button"
      onClick={onClick}
      aria-label={`${leida ? "Abrir" : "Marcar como leída y abrir"}: ${titulo}`}
      className={`flex w-full items-start gap-4 rounded-2xl border p-4 text-left transition-colors focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-ring sm:p-5 ${
        leida ? "border-border bg-card hover:bg-muted/70" : "border-primary/25 bg-accent/60 hover:bg-accent"
      }`}
    >
      <span aria-hidden="true" className="flex size-11 shrink-0 items-center justify-center rounded-xl bg-background text-primary ring-1 ring-border">
        <Icon className="size-5" />
      </span>
      <span className="min-w-0 flex-1">
        <span className="flex flex-wrap items-start justify-between gap-x-4 gap-y-1">
          <span className="font-semibold text-foreground">{titulo}</span>
          <span className="shrink-0 text-xs text-muted-foreground">{fecha}</span>
        </span>
        <span className="mt-1 block text-sm leading-6 text-muted-foreground">{mensaje}</span>
      </span>
      {!leida ? <span aria-label="Sin leer" className="mt-2 size-2.5 shrink-0 rounded-full bg-primary" /> : null}
    </button>
  );
}
