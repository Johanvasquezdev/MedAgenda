"use client";

import { useEffect, useState } from "react";
import { Bell, CheckCircle } from "lucide-react";
import dayjs from "dayjs";
import { NotificacionResponse } from "@/types/api.types";
import { NotificacionService } from "@/services/notificacion.service";
import { useAuth } from "@/components/providers/AuthProvider";
import { usePageTransition } from "@/components/animations/Animatedcomponents";
import { NotificacionCard } from "@/components/notificaciones/NotificacionCard";

const filterTabs = ["Todas", "No leídas", "Leídas"];

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
      <header className="rounded-2xl border border-border bg-card p-6 md:p-7 shadow-sm">
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
          <div className="rounded-2xl border border-border bg-card p-12 text-center">
            <p className="text-muted-foreground">Cargando notificaciones...</p>
          </div>
        ) : filtered.length === 0 ? (
          <div className="rounded-2xl border border-border bg-card p-12 text-center">
            <Bell className="mx-auto mb-4 size-12 text-muted-foreground" />
            <p className="text-muted-foreground">No tienes notificaciones.</p>
          </div>
        ) : (
          filtered.map(n => (
            <NotificacionCard
              key={n.id}
              tipo={n.tipo}
              titulo={n.tipo}
              mensaje={n.mensaje}
              fecha={dayjs(n.fechaEnvio).format("DD MMM, HH:mm")}
              leida={n.leida}
              onClick={() => marcarComoLeida(n.id)}
            />
          ))
        )}
      </div>
    </div>
  );
}
