"use client";

import { UserSettingsForm } from "@/components/settings/UserSettingsForm";
import { Settings, Shield, User, Globe, MessageSquare } from "lucide-react";
import { usePageTransition, AnimatedCard } from "@/components/animations/Animatedcomponents";
import Link from "next/link";

export default function PacienteSettingsPage() {
  usePageTransition();

  return (
    <div className="p-6 max-w-6xl mx-auto space-y-6 page-content">
      <header className="relative overflow-hidden rounded-3xl border border-emerald-500/10 bg-gradient-to-br from-emerald-500/10 to-teal-500/10 p-6 md:p-7 shadow-sm transition-all duration-500 hover:shadow-lg hover:shadow-emerald-500/5 backdrop-blur-sm">
        <div className="absolute -right-16 -top-20 h-56 w-56 rounded-full bg-emerald-500/5 dark:bg-emerald-500/10 blur-3xl opacity-50" />
        <div className="absolute -bottom-20 -left-20 h-56 w-56 rounded-full bg-teal-500/5 dark:bg-teal-500/10 blur-3xl opacity-50" />

        <div className="relative z-10">
          <p className="text-[10px] font-black uppercase tracking-[0.3em] text-emerald-600/80 dark:text-emerald-400/80">
            Portal del Paciente
          </p>
          <h1 className="mt-2 flex items-center gap-2 text-3xl font-black tracking-tight text-foreground">
            <Settings className="h-8 w-8 text-emerald-600 dark:text-emerald-400" />
            Configuración Personal
          </h1>
          <p className="mt-2 max-w-2xl text-muted-foreground text-xs font-semibold leading-relaxed">
            Administra tu información de contacto, seguridad y preferencias de atención médica.
          </p>
        </div>
      </header>

      <div className="grid gap-6 lg:grid-cols-3">
        <div className="lg:col-span-2">
          <UserSettingsForm accentColor="emerald" />
        </div>

        <div className="space-y-6">
          <AnimatedCard delay={200} className="p-6 bg-card border border-border rounded-3xl shadow-sm overflow-hidden relative group">
            <div className="absolute top-0 right-0 p-4 opacity-10 group-hover:opacity-20 transition-opacity">
              <Shield className="w-20 h-20 text-emerald-500" />
            </div>
            <div className="flex items-center gap-3 mb-4">
               <div className="p-2 bg-emerald-500/10 rounded-xl">
                 <Shield className="w-5 h-5 text-emerald-600 dark:text-emerald-400" />
               </div>
               <h3 className="font-black text-foreground uppercase tracking-tight text-sm">Privacidad</h3>
            </div>
            <p className="text-xs text-muted-foreground mb-4 font-medium leading-relaxed">
              Tus datos médicos están protegidos bajo estrictas normativas de privacidad y cifrado.
            </p>
            <div className="flex items-center gap-2 px-4 py-2 bg-emerald-500/10 rounded-xl border border-emerald-500/20">
              <div className="h-2 w-2 rounded-full bg-emerald-500 animate-pulse" />
              <span className="text-[10px] font-black uppercase tracking-widest text-emerald-700 dark:text-emerald-400">Cuenta Verificada</span>
            </div>
          </AnimatedCard>

          <AnimatedCard delay={300} className="p-6 bg-card border border-border rounded-3xl shadow-sm border-l-4 border-l-emerald-500/50">
            <div className="flex items-center gap-3 mb-4">
               <div className="p-2 bg-teal-500/10 rounded-xl">
                 <Globe className="w-5 h-5 text-teal-600 dark:text-teal-400" />
               </div>
               <h3 className="font-black text-foreground uppercase tracking-tight text-sm">Idioma</h3>
            </div>
            <div className="space-y-4">
              <div className="flex items-center justify-between">
                <span className="text-xs font-black uppercase tracking-widest text-muted-foreground">Sistema</span>
                <span className="text-xs font-black text-foreground uppercase tracking-widest">Español</span>
              </div>
            </div>
          </AnimatedCard>

        </div>
      </div>
    </div>
  );
}