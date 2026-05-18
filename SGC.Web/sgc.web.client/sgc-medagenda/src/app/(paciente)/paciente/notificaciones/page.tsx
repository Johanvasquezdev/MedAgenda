"use client";

import { useEffect, useState } from "react";
import { Bell, CalendarClock, CreditCard, Info, CheckCircle } from "lucide-react";
import dayjs from "dayjs";
import { NotificacionResponse } from "@/types/api.types";
import { NotificacionService } from "@/services/notificacion.service";
import { useAuth } from "@/components/providers/AuthProvider";
import { usePageTransition } from "@/components/animations/Animatedcomponents";

const filterTabs = ["Todas", "No leídas", "Leídas"];

function getIcon(tipo: string) {
  switch (tipo?.toLowerCase()) {
    case "email": return <CalendarClock className="w-5 h-5" />;
    case "sms": return <CreditCard className="w-5 h-5" />;
    default: return <Info className="w-5 h-5" />;
  }
}

function getIconColor(tipo: string) {
  switch (tipo?.toLowerCase()) {
    case "email": return "bg-emerald-500/10 text-emerald-600 dark:text-emerald-400";
    case "sms": return "bg-amber-500/10 text-amber-600 dark:text-amber-400";
    default: return "bg-indigo-500/10 text-indigo-600 dark:text-indigo-400";
  }
}

export default function NotificacionesPage() {
  const { user } = useAuth();
  const [notificaciones, setNotificaciones] = useState<NotificacionResponse[]>([]);
  const [loading, setLoading] = useState(true);
  const [activeTab, setActiveTab] = useState("Todas");

  useEffect(() => {
    const fetchNotificaciones = async () => {
      if (!user?.id) { setLoading(false); return; }
      try {
        const data = await NotificacionService.obtenerPorUsuario(user.id);
        setNotificaciones(data);
      } catch {
        setNotificaciones([]);
      } finally {
        setLoading(false);
      }
    };
    fetchNotificaciones();
  }, [user?.id]);

  const marcarComoLeida = (id: number) =>
    setNotificaciones(n => n.map(x => x.id === id ? { ...x, leida: true } : x));

  const marcarTodasLeidas = () =>
    setNotificaciones(n => n.map(x => ({ ...x, leida: true })));

  const unreadCount = notificaciones.filter(n => !n.leida).length;

  const filtered = notificaciones.filter(n => {
    if (activeTab === "No leídas") return !n.leida;
    if (activeTab === "Leídas") return n.leida;
    return true;
  });

  usePageTransition();

  return (
    <div className="space-y-8 page-content animate-in fade-in duration-500">
      <header className="relative overflow-hidden rounded-3xl border border-emerald-500/20 bg-gradient-to-br from-emerald-500/15 via-white dark:via-slate-950 to-teal-500/15 p-6 md:p-7 shadow-sm">
        <div className="absolute -right-16 -top-20 h-56 w-56 rounded-full bg-emerald-500/10 dark:bg-emerald-500/20 blur-3xl opacity-50" />
        <div className="relative z-10 flex flex-col md:flex-row md:items-center justify-between gap-6">
          <div>
            <div className="flex items-center gap-3">
              <Bell className="w-8 h-8 text-emerald-600 dark:text-emerald-400" />
              <h1 className="text-3xl font-black tracking-tight text-foreground">
                Mis Notificaciones
              </h1>
              {unreadCount > 0 && (
                <span className="px-3 py-1 bg-emerald-600 text-white text-[10px] font-black uppercase tracking-widest rounded-full shadow-lg shadow-emerald-500/20">
                  {unreadCount} Pendientes
                </span>
              )}
            </div>
            <p className="text-muted-foreground font-medium mt-1">Mantente al tanto de tus citas y pagos en tiempo real.</p>
          </div>
          {unreadCount > 0 && (
            <button
              onClick={marcarTodasLeidas}
              className="flex items-center gap-2 px-5 py-2.5 bg-emerald-600/10 hover:bg-emerald-600/20 text-emerald-600 dark:text-emerald-400 rounded-xl transition-all text-sm font-bold shadow-sm active:scale-95"
            >
              <CheckCircle className="w-4 h-4" />
              Marcar todas como leídas
            </button>
          )}
        </div>
      </header>

      <div className="flex gap-2 overflow-x-auto pb-2 scrollbar-none">
        {filterTabs.map(tab => (
          <button
            key={tab}
            onClick={() => setActiveTab(tab)}
            className={`px-5 py-2.5 rounded-xl text-sm font-bold transition-all shadow-sm flex-none ${
              activeTab === tab
                ? "bg-emerald-600 text-white shadow-emerald-500/20"
                : "bg-muted/50 text-muted-foreground hover:bg-muted hover:text-foreground border border-border"
            }`}
          >
            {tab}
          </button>
        ))}
      </div>

      <div className="space-y-3">
        {loading ? (
          <div className="backdrop-blur-md bg-white/5 border border-white/10 rounded-2xl p-12 text-center">
            <p className="text-white/60">Cargando notificaciones...</p>
          </div>
        ) : filtered.length === 0 ? (
          <div className="backdrop-blur-md bg-white/5 border border-white/10 rounded-2xl p-12 text-center">
            <Bell className="w-12 h-12 text-white/20 mx-auto mb-4" />
            <p className="text-white/60">No tienes notificaciones.</p>
          </div>
        ) : (
          filtered.map(n => (
            <div
              key={n.id}
              onClick={() => marcarComoLeida(n.id)}
              className={`bg-card border rounded-2xl p-5 cursor-pointer transition-all hover:shadow-lg hover:-translate-y-0.5 shadow-sm group ${
                n.leida
                  ? "border-border/50 opacity-80"
                  : "border-emerald-500/30 p-5 bg-gradient-to-r from-emerald-500/5 to-transparent"
              }`}
            >
              <div className="flex items-start gap-5">
                <div className={`w-12 h-12 rounded-xl flex items-center justify-center shadow-inner group-hover:scale-110 transition-transform ${getIconColor(n.tipo)}`}>
                  {getIcon(n.tipo)}
                </div>
                <div className="flex-1">
                  <div className="flex items-start justify-between">
                    <div>
                      <h3 className={`font-black text-lg ${n.leida ? "text-muted-foreground" : "text-foreground"}`}>
                        {n.tipo}
                      </h3>
                      <p className={`text-sm mt-1 font-medium leading-relaxed ${n.leida ? "text-muted-foreground/70" : "text-foreground"}`}>
                        {n.mensaje}
                      </p>
                    </div>
                    {!n.leida && <div className="w-2.5 h-2.5 rounded-full bg-emerald-500 mt-2 animate-pulse" />}
                  </div>
                  <div className="flex items-center gap-2 mt-3 p-1.5 bg-muted/30 rounded-lg w-fit border border-border/50">
                    <span className="text-muted-foreground text-[10px] font-black uppercase tracking-widest leading-none">
                      {dayjs(n.fechaEnvio).format("DD MMM, HH:mm")}
                    </span>
                  </div>
                </div>
              </div>
            </div>
          ))
        )}
      </div>
    </div>
  );
}