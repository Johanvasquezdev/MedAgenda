"use client";

import { UserSettingsForm } from "@/components/settings/UserSettingsForm";
import { Settings, Shield, Globe } from "lucide-react";
import { usePageTransition, AnimatedCard } from "@/components/animations/Animatedcomponents";

export default function SettingsPage() {
  usePageTransition();

  return (
    <div className="p-6 max-w-6xl mx-auto space-y-6 page-content">
      <header className="rounded-2xl border border-border bg-card p-6 md:p-7 shadow-sm">

        <div className="relative z-10">
          <p className="text-[10px] font-black uppercase tracking-[0.3em] text-emerald-600/80 dark:text-emerald-400/80">
            Ajustes Administrativos
          </p>
          <h1 className="mt-2 flex items-center gap-2 text-3xl font-black tracking-tight text-foreground">
            <Settings className="h-8 w-8 text-emerald-600 dark:text-emerald-400" />
            Configuración de Perfil
          </h1>
          <p className="mt-2 max-w-2xl text-muted-foreground text-xs font-semibold leading-relaxed">
            Gestión de identidad, seguridad y preferencias del sistema para administradores.
          </p>
        </div>
      </header>

      <div className="grid gap-6 lg:grid-cols-3">
        <div className="lg:col-span-2">
          <UserSettingsForm accentColor="emerald" />
        </div>

        <div className="space-y-6">
          <AnimatedCard delay={200} className="rounded-2xl border border-border bg-card p-6 shadow-sm">
            <div className="flex items-center gap-3 mb-4">
               <div className="p-2 bg-emerald-500/10 rounded-xl">
                 <Shield className="w-5 h-5 text-emerald-600 dark:text-emerald-400" />
               </div>
               <h3 className="text-sm font-semibold tracking-tight text-foreground">Seguridad de la cuenta</h3>
            </div>
            <p className="mb-4 text-sm leading-relaxed text-muted-foreground">
              Mantén tus datos de acceso y perfil actualizados desde esta página.
            </p>
          </AnimatedCard>

          <AnimatedCard delay={300} className="rounded-2xl border border-border bg-card p-6 shadow-sm">
            <div className="flex items-center gap-3 mb-4">
               <div className="p-2 bg-emerald-500/10 rounded-xl">
                 <Globe className="w-5 h-5 text-emerald-600 dark:text-emerald-400" />
               </div>
               <h3 className="text-sm font-semibold tracking-tight text-foreground">Preferencias</h3>
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
