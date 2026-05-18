"use client";

import { UserSettingsForm } from "@/components/settings/UserSettingsForm";
import { Settings, Shield, UserCircle, Globe, Lock } from "lucide-react";
import { usePageTransition, AnimatedCard } from "@/components/animations/Animatedcomponents";

export default function SettingsPage() {
  usePageTransition();

  return (
    <div className="p-6 max-w-6xl mx-auto space-y-6 page-content">
      <header className="relative overflow-hidden rounded-3xl border border-indigo-500/10 bg-gradient-to-br from-indigo-500/10 to-purple-500/10 p-6 md:p-7 shadow-sm transition-all duration-500 hover:shadow-lg hover:shadow-indigo-500/5 backdrop-blur-sm">
        <div className="absolute -right-16 -top-20 h-56 w-56 rounded-full bg-indigo-500/5 dark:bg-indigo-500/10 blur-3xl opacity-50" />
        <div className="absolute -bottom-20 -left-20 h-56 w-56 rounded-full bg-purple-500/5 dark:bg-purple-500/10 blur-3xl opacity-50" />

        <div className="relative z-10">
          <p className="text-[10px] font-black uppercase tracking-[0.3em] text-indigo-600/80 dark:text-indigo-400/80">
            Ajustes Administrativos
          </p>
          <h1 className="mt-2 flex items-center gap-2 text-3xl font-black tracking-tight text-foreground">
            <Settings className="h-8 w-8 text-indigo-600 dark:text-indigo-400" />
            Configuración de Perfil
          </h1>
          <p className="mt-2 max-w-2xl text-muted-foreground text-xs font-semibold leading-relaxed">
            Gestión de identidad, seguridad y preferencias del sistema para administradores.
          </p>
        </div>
      </header>

      <div className="grid gap-6 lg:grid-cols-3">
        <div className="lg:col-span-2">
          <UserSettingsForm accentColor="indigo" />
        </div>

        <div className="space-y-6">
          <AnimatedCard delay={200} className="p-6 bg-card border border-border rounded-3xl shadow-sm border-t-4 border-t-emerald-500/50">
            <div className="flex items-center gap-3 mb-4">
               <div className="p-2 bg-emerald-500/10 rounded-xl">
                 <Shield className="w-5 h-5 text-emerald-600 dark:text-emerald-400" />
               </div>
               <h3 className="font-black text-foreground uppercase tracking-tight text-sm">Estado de Seguridad</h3>
            </div>
            <p className="text-xs text-muted-foreground mb-4 font-medium leading-relaxed">
              Tu cuenta está protegida por políticas de seguridad administrativa avanzada.
            </p>
            <div className="space-y-3">
              <div className="flex items-center justify-between p-3 rounded-xl bg-muted/30 border border-border">
                <div className="flex items-center gap-2">
                  <Lock className="w-3.5 h-3.5 text-emerald-500" />
                  <span className="text-[10px] font-black uppercase tracking-widest text-foreground">2FA Activo</span>
                </div>
                <div className="w-2 h-2 rounded-full bg-emerald-500 animate-pulse" />
              </div>
            </div>
          </AnimatedCard>

          <AnimatedCard delay={300} className="p-6 bg-card border border-border rounded-3xl shadow-sm border-l-4 border-l-indigo-500/50">
            <div className="flex items-center gap-3 mb-4">
               <div className="p-2 bg-indigo-500/10 rounded-xl">
                 <Globe className="w-5 h-5 text-indigo-600 dark:text-indigo-400" />
               </div>
               <h3 className="font-black text-foreground uppercase tracking-tight text-sm">Preferencias</h3>
            </div>
            <div className="space-y-4">
              <div className="flex items-center justify-between">
                <span className="text-xs font-black uppercase tracking-widest text-muted-foreground">Idioma</span>
                <span className="text-xs font-black text-foreground uppercase tracking-widest">Español</span>
              </div>
              <div className="flex items-center justify-between border-t border-border pt-4">
                <span className="text-xs font-black uppercase tracking-widest text-muted-foreground">Zona Horaria</span>
                <span className="text-xs font-black text-foreground uppercase tracking-widest">GMT-4</span>
              </div>
            </div>
          </AnimatedCard>
        </div>
      </div>
    </div>
  );
}
