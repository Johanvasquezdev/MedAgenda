"use client";

import { useEffect, useMemo, useState } from "react";
import { CheckCircle2, CircleUserRound, Loader2, Shield } from "lucide-react";
import { PerfilService } from "@/services/perfil.service";
import { useAuth } from "@/components/providers/AuthProvider";

interface UserSettingsFormProps {
  accentColor?: "indigo" | "emerald";
}

export function UserSettingsForm({ accentColor = "indigo" }: UserSettingsFormProps) {
  const { user } = useAuth();
  const [loading, setLoading] = useState(true);
  const [savingProfile, setSavingProfile] = useState(false);
  const [savingPassword, setSavingPassword] = useState(false);
  const [profileMessage, setProfileMessage] = useState<string | null>(null);
  const [passwordMessage, setPasswordMessage] = useState<string | null>(null);

  const [nombre, setNombre] = useState("");
  const [email, setEmail] = useState("");
  const [telefono, setTelefono] = useState("");
  const [rol, setRol] = useState("");

  const [passwordActual, setPasswordActual] = useState("");
  const [passwordNueva, setPasswordNueva] = useState("");
  const [passwordConfirmacion, setPasswordConfirmacion] = useState("");

  const initials = useMemo(() => {
    const base = nombre || user?.nombre || "U";
    return base.trim().charAt(0).toUpperCase();
  }, [nombre, user?.nombre]);

  const telefonoHabilitado = rol.toLowerCase() === "paciente" || rol.toLowerCase() === "medico";

  useEffect(() => {
    const cargarPerfil = async () => {
      try {
        const perfil = await PerfilService.obtenerMiPerfil();
        setNombre(perfil.nombre || "");
        setEmail(perfil.email || "");
        setTelefono(perfil.telefono || "");
        setRol(String(perfil.rol || ""));
      } catch (error: any) {
        setProfileMessage(error?.response?.data?.message || "No se pudo cargar el perfil.");
      } finally {
        setLoading(false);
      }
    };

    void cargarPerfil();
  }, []);

  const handleGuardarPerfil = async (e: React.FormEvent) => {
    e.preventDefault();
    setSavingProfile(true);
    setProfileMessage(null);

    try {
      const perfil = await PerfilService.actualizarMiPerfil({
        nombre,
        email,
        telefono: telefonoHabilitado ? telefono : null,
      });

      localStorage.setItem(
        "medagenda_user",
        JSON.stringify({
          id: perfil.id,
          nombre: perfil.nombre,
          rol: perfil.rol,
          email: perfil.email,
        })
      );
      window.dispatchEvent(new Event("auth-changed"));
      setProfileMessage("Perfil actualizado correctamente.");
    } catch (error: any) {
      setProfileMessage(error?.response?.data?.message || "No se pudo actualizar el perfil.");
    } finally {
      setSavingProfile(false);
    }
  };

  const handleCambiarPassword = async (e: React.FormEvent) => {
    e.preventDefault();
    setPasswordMessage(null);

    if (passwordNueva !== passwordConfirmacion) {
      setPasswordMessage("La confirmación de contraseña no coincide.");
      return;
    }

    setSavingPassword(true);
    try {
      await PerfilService.cambiarMiPassword({
        passwordActual,
        passwordNueva,
      });
      setPasswordActual("");
      setPasswordNueva("");
      setPasswordConfirmacion("");
      setPasswordMessage("Contraseña actualizada correctamente.");
    } catch (error: any) {
      setPasswordMessage(error?.response?.data?.message || "No se pudo cambiar la contraseña.");
    } finally {
      setSavingPassword(false);
    }
  };

  if (loading) {
    return (
      <div className="rounded-3xl border border-border bg-card/50 p-12 text-center text-muted-foreground shadow-sm">
        <Loader2 className={`mx-auto mb-4 h-8 w-8 animate-spin ${accentColor === 'indigo' ? 'text-indigo-500' : 'text-emerald-500'}`} />
        <p className="text-sm font-bold uppercase tracking-widest opacity-60">Cargando perfil...</p>
      </div>
    );
  }

  const ringColor = accentColor === "indigo" ? "focus:ring-indigo-500/20 focus:border-indigo-500" : "focus:ring-emerald-500/20 focus:border-emerald-500";
  const bgColor = accentColor === "indigo" ? "bg-indigo-500/10 text-indigo-600 dark:text-indigo-400 border-indigo-500/20" : "bg-emerald-500/10 text-emerald-600 dark:text-emerald-400 border-emerald-500/20";
  const btnColor = accentColor === "indigo" ? "bg-indigo-600 hover:bg-indigo-500 shadow-indigo-500/20" : "bg-emerald-600 hover:bg-emerald-500 shadow-emerald-500/20";

  return (
    <div className="space-y-6">
      <section className="rounded-2xl border border-border bg-card p-6 shadow-sm relative overflow-hidden group">
        <div className={`absolute -right-8 -top-8 h-24 w-24 rounded-full blur-2xl opacity-10 transition-opacity group-hover:opacity-20 ${accentColor === 'indigo' ? 'bg-indigo-500' : 'bg-emerald-500'}`} />
        
        <div className="flex items-center gap-5 relative z-10">
          <div className={`h-20 w-20 rounded-2xl border-2 font-black text-3xl flex items-center justify-center shadow-inner transition-transform group-hover:scale-105 duration-300 ${bgColor}`}>
            {initials}
          </div>
          <div>
            <h2 className="text-xl font-black text-foreground flex items-center gap-2 uppercase tracking-tight">
              <CircleUserRound className={`h-6 w-6 ${accentColor === 'indigo' ? 'text-indigo-500' : 'text-emerald-500'}`} />
              Foto de Perfil
            </h2>
            <p className="text-sm text-muted-foreground font-medium mt-1">
              Personaliza tu identidad visual en la plataforma.
              <span className={`block mt-1 text-[10px] uppercase font-black tracking-widest ${accentColor === 'indigo' ? 'text-indigo-600 dark:text-indigo-400' : 'text-emerald-600 dark:text-emerald-400'}`}>Próximamente disponible</span>
            </p>
          </div>
        </div>
      </section>

      <section className="rounded-2xl border border-border bg-card p-6 space-y-6 shadow-sm">
        <div className="flex items-center gap-2 mb-2">
          <div className={`h-8 w-1.5 rounded-full ${accentColor === 'indigo' ? 'bg-indigo-500' : 'bg-emerald-500'}`} />
          <h2 className="text-xl font-black text-foreground uppercase tracking-tight">Datos del Perfil</h2>
        </div>
        
        <form className="grid gap-6" onSubmit={handleGuardarPerfil}>
          <div className="grid gap-5 md:grid-cols-2">
            <div className="space-y-2">
              <label className="text-[10px] font-black uppercase tracking-[0.2em] text-muted-foreground ml-1">Nombre Completo</label>
              <input
                value={nombre}
                onChange={(e) => setNombre(e.target.value)}
                className={`w-full rounded-xl border border-border bg-background px-4 py-3 text-sm text-foreground font-medium outline-none transition-all placeholder:text-muted-foreground/30 shadow-sm ${ringColor}`}
                required
                placeholder="Tu nombre completo"
              />
            </div>

            <div className="space-y-2">
              <label className="text-[10px] font-black uppercase tracking-[0.2em] text-muted-foreground ml-1">Correo Electrónico</label>
              <input
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                className={`w-full rounded-xl border border-border bg-background px-4 py-3 text-sm text-foreground font-medium outline-none transition-all placeholder:text-muted-foreground/30 shadow-sm ${ringColor}`}
                required
                placeholder="correo@ejemplo.com"
              />
            </div>

            <div className="space-y-2">
              <label className="text-[10px] font-black uppercase tracking-[0.2em] text-muted-foreground ml-1">Teléfono</label>
              <input
                value={telefono}
                onChange={(e) => setTelefono(e.target.value)}
                disabled={!telefonoHabilitado}
                className={`w-full rounded-xl border border-border bg-background px-4 py-3 text-sm text-foreground font-medium outline-none transition-all disabled:opacity-50 disabled:bg-muted/30 shadow-sm ${ringColor}`}
                placeholder={telefonoHabilitado ? "809-555-0000" : "No aplica"}
              />
            </div>

            <div className="space-y-2">
              <label className="text-[10px] font-black uppercase tracking-[0.2em] text-muted-foreground ml-1">Rol de Usuario</label>
              <div className="rounded-xl border border-border bg-muted/30 px-4 py-3 text-sm font-black text-muted-foreground uppercase tracking-widest flex items-center gap-2">
                <Shield className="w-4 h-4 opacity-50" />
                {rol || "No definido"}
              </div>
            </div>
          </div>

          <div className="flex items-center gap-4 pt-6 border-t border-border">
            <button
              type="submit"
              disabled={savingProfile}
              className={`inline-flex items-center justify-center rounded-xl px-8 py-3 text-sm font-black uppercase tracking-widest text-white transition-all active:scale-95 disabled:opacity-50 shadow-lg ${btnColor}`}
            >
              {savingProfile ? <Loader2 className="h-4 w-4 animate-spin mr-2" /> : "Guardar Cambios"}
            </button>

            {profileMessage && (
              <p className="text-sm text-emerald-600 dark:text-emerald-400 font-semibold flex items-center gap-2 animate-in fade-in slide-in-from-left-2">
                <CheckCircle2 className="h-4 w-4" />
                {profileMessage}
              </p>
            )}
          </div>
        </form>
      </section>

      <section className="rounded-2xl border border-border bg-gradient-to-br from-indigo-500/5 via-card to-card p-6 space-y-6 shadow-sm border-l-4 border-l-indigo-500">
        <div className="flex items-center gap-2 mb-2">
          <h2 className="text-xl font-black text-foreground flex items-center gap-2 uppercase tracking-tight">
            <Shield className="h-6 w-6 text-indigo-500" />
            Seguridad y Contraseña
          </h2>
        </div>

        <form className="grid gap-6" onSubmit={handleCambiarPassword}>
          <div className="grid gap-5 md:grid-cols-2">
            <div className="space-y-2 md:col-span-2">
              <label className="text-[10px] font-black uppercase tracking-[0.2em] text-muted-foreground ml-1">Contraseña Actual</label>
              <input
                type="password"
                value={passwordActual}
                onChange={(e) => setPasswordActual(e.target.value)}
                className="w-full rounded-xl border border-border bg-background px-4 py-3 text-sm text-foreground font-medium focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-500 outline-none transition-all placeholder:text-muted-foreground/30 shadow-sm"
                required
                placeholder="••••••••"
              />
            </div>

            <div className="space-y-2">
              <label className="text-[10px] font-black uppercase tracking-[0.2em] text-muted-foreground ml-1">Nueva Contraseña</label>
              <input
                type="password"
                value={passwordNueva}
                onChange={(e) => setPasswordNueva(e.target.value)}
                className="w-full rounded-xl border border-border bg-background px-4 py-3 text-sm text-foreground font-medium focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-500 outline-none transition-all placeholder:text-muted-foreground/30 shadow-sm"
                required
                placeholder="Mínimo 8 caracteres"
              />
            </div>

            <div className="space-y-2">
              <label className="text-[10px] font-black uppercase tracking-[0.2em] text-muted-foreground ml-1">Confirmar Nueva Contraseña</label>
              <input
                type="password"
                value={passwordConfirmacion}
                onChange={(e) => setPasswordConfirmacion(e.target.value)}
                className="w-full rounded-xl border border-border bg-background px-4 py-3 text-sm text-foreground font-medium focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-500 outline-none transition-all placeholder:text-muted-foreground/30 shadow-sm"
                required
                placeholder="Repite la nueva contraseña"
              />
            </div>
          </div>

          <div className="flex items-center gap-4 pt-6 border-t border-border">
            <button
              type="submit"
              disabled={savingPassword}
              className="inline-flex items-center justify-center rounded-xl bg-indigo-600 px-8 py-3 text-sm font-black uppercase tracking-widest text-white hover:bg-indigo-500 transition-all active:scale-95 disabled:opacity-50 shadow-lg shadow-indigo-500/20"
            >
              {savingPassword ? <Loader2 className="h-4 w-4 animate-spin mr-2" /> : "Actualizar Contraseña"}
            </button>

            {passwordMessage && (
              <p className={`text-sm font-black px-4 py-2 rounded-lg ${passwordMessage.includes("correctamente") ? "bg-emerald-500/10 text-emerald-600 dark:text-emerald-400 border border-emerald-500/20" : "bg-rose-500/10 text-rose-600 dark:text-rose-400 border border-rose-500/20"}`}>
                {passwordMessage}
              </p>
            )}
          </div>
        </form>
      </section>
    </div>
  );
}
