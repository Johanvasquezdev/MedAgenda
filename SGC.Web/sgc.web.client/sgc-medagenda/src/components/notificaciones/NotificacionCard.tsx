import { CalendarClock, CreditCard, Info } from "lucide-react";

interface NotificacionCardProps {
  tipo: "cita" | "pago" | "info" | string;
  titulo: string;
  mensaje: string;
  fecha: string;
  leida: boolean;
  onClick: () => void;
}

export function NotificacionCard({
  tipo,
  titulo,
  mensaje,
  fecha,
  leida,
  onClick,
}: NotificacionCardProps) {
  const getIcon = () => {
    switch (tipo) {
      case "cita":
        return (
          <CalendarClock className="w-5 h-5 text-emerald-600 dark:text-emerald-400" />
        );
      case "pago":
        return (
          <CreditCard className="w-5 h-5 text-emerald-600 dark:text-emerald-400" />
        );
      default:
        return <Info className="w-5 h-5 text-blue-600 dark:text-blue-400" />;
    }
  };

  const getBgColor = () => {
    switch (tipo) {
      case "cita":
        return "bg-emerald-100 dark:bg-emerald-900/30";
      case "pago":
        return "bg-emerald-100 dark:bg-emerald-900/30";
      default:
        return "bg-blue-100 dark:bg-blue-900/30";
    }
  };

  return (
    <div
      onClick={onClick}
      className={`p-5 rounded-2xl border transition-all cursor-pointer flex gap-4 ${
        leida
          ? "bg-white dark:bg-slate-900 border-slate-200 dark:border-slate-800 hover:bg-slate-50 dark:hover:bg-slate-800/50"
          : "bg-emerald-50/50 dark:bg-emerald-900/10 border-emerald-100 dark:border-emerald-800/50 shadow-sm hover:shadow-md"
      }`}
    >
      <div className={`p-3 rounded-xl h-fit ${getBgColor()}`}>{getIcon()}</div>

      <div className="flex-1">
        <div className="flex justify-between items-start">
          <h3
            className={`font-semibold ${leida ? "text-slate-700 dark:text-slate-300" : "text-slate-900 dark:text-white"}`}
          >
            {titulo}
          </h3>
          <span className="text-xs text-slate-400">{fecha}</span>
        </div>
        <p
          className={`text-sm mt-1 ${leida ? "text-slate-500" : "text-slate-600 dark:text-slate-300"}`}
        >
          {mensaje}
        </p>
      </div>

      {!leida && (
        <div className="w-2.5 h-2.5 rounded-full bg-emerald-500 self-center flex-shrink-0 animate-pulse"></div>
      )}
    </div>
  );
}

