"use client";
import { useState, useEffect, useRef, Suspense } from "react";
import Link from "next/link";
import { useSearchParams, useRouter } from "next/navigation";
import { ThreeBackground } from "@/components/animations/Threebackground";
import anime from "animejs";
import { Hospital, Lock, Eye, EyeOff, ArrowRight, ArrowLeft, CheckCircle, AlertTriangle } from "lucide-react";
import axios from "axios";

const API = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5189";

function ResetPasswordForm() {
  const searchParams = useSearchParams();
  const router = useRouter();
  const token = searchParams.get("token") ?? "";
  const email = searchParams.get("email") ?? "";

  const [password, setPassword] = useState("");
  const [confirm, setConfirm] = useState("");
  const [showPass, setShowPass] = useState(false);
  const [showConfirm, setShowConfirm] = useState(false);
  const [loading, setLoading] = useState(false);
  const [success, setSuccess] = useState(false);
  const [error, setError] = useState("");
  const formRef = useRef<HTMLDivElement>(null);
  const logoRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    anime({ targets: logoRef.current, translateY: [-30, 0], opacity: [0, 1], duration: 800, easing: "easeOutElastic(1, .6)" });
    anime({ targets: formRef.current, translateY: [40, 0], opacity: [0, 1], duration: 700, delay: 200, easing: "easeOutExpo" });
  }, []);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (password !== confirm) {
      setError("Las contraseñas no coinciden.");
      return;
    }
    if (password.length < 8) {
      setError("La contraseña debe tener al menos 8 caracteres.");
      return;
    }
    setLoading(true);
    setError("");
    try {
      await axios.post(`${API}/api/auth/reset-password`, { token, email, nuevaPassword: password });
      setSuccess(true);
      setTimeout(() => router.push("/login"), 3000);
    } catch (err: unknown) {
      const axiosErr = err as { response?: { data?: { mensaje?: string } } };
      setError(axiosErr?.response?.data?.mensaje ?? "Token inválido o expirado. Solicita uno nuevo.");
    } finally {
      setLoading(false);
    }
  };

  if (!token || !email) {
    return (
      <div className="text-center py-8">
        <AlertTriangle className="w-12 h-12 text-amber-500 mx-auto mb-4" />
        <h2 className="text-xl font-bold text-foreground mb-2">Enlace inválido</h2>
        <p className="text-muted-foreground text-sm mb-4">El enlace de restablecimiento es incorrecto o ha expirado.</p>
        <Link href="/forgot-password" className="text-emerald-600 dark:text-emerald-400 font-semibold hover:underline text-sm">
          Solicitar nuevo enlace
        </Link>
      </div>
    );
  }

  return (
    <>
      {success ? (
        <div className="text-center py-4">
          <div className="w-16 h-16 bg-emerald-500/10 rounded-full flex items-center justify-center mx-auto mb-4">
            <CheckCircle className="w-8 h-8 text-emerald-500" />
          </div>
          <h2 className="text-2xl font-bold text-foreground mb-2">Contraseña actualizada</h2>
          <p className="text-muted-foreground text-sm mb-4">Tu contraseña fue restablecida correctamente. Serás redirigido al login en unos segundos.</p>
          <Link href="/login" className="inline-flex items-center gap-2 text-sm text-emerald-600 dark:text-emerald-400 font-semibold hover:underline">
            <ArrowLeft className="w-4 h-4" /> Ir al inicio de sesión
          </Link>
        </div>
      ) : (
        <>
          <div className="mb-8">
            <h2 className="text-2xl font-bold text-foreground">Nueva contraseña</h2>
            <p className="text-muted-foreground text-sm mt-1">Elige una contraseña segura para tu cuenta.</p>
          </div>

          {error && (
            <div className="flex items-start gap-2 bg-destructive/10 border border-destructive/20 text-destructive rounded-2xl p-4 text-sm mb-6">
              <AlertTriangle className="w-5 h-5 flex-shrink-0" />
              <span>{error}</span>
            </div>
          )}

          <form onSubmit={handleSubmit} className="space-y-5">
            {/* Nueva contraseña */}
            <div>
              <label className="block text-sm font-medium text-foreground mb-1.5">Nueva contraseña</label>
              <div className="relative">
                <div className="absolute left-3.5 top-1/2 -translate-y-1/2 text-muted-foreground"><Lock className="w-5 h-5" /></div>
                <input
                  id="new-password"
                  type={showPass ? "text" : "password"}
                  placeholder="Mínimo 8 caracteres"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  required
                  className="w-full pl-10 pr-12 py-3 bg-secondary border border-border rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-emerald-500/30 focus:border-emerald-500 transition-all placeholder:text-muted-foreground/50"
                />
                <button type="button" onClick={() => setShowPass(!showPass)} className="absolute right-3.5 top-1/2 -translate-y-1/2 text-muted-foreground hover:text-foreground transition-colors p-1">
                  {showPass ? <EyeOff className="w-5 h-5" /> : <Eye className="w-5 h-5" />}
                </button>
              </div>
            </div>

            {/* Confirmar contraseña */}
            <div>
              <label className="block text-sm font-medium text-foreground mb-1.5">Confirmar contraseña</label>
              <div className="relative">
                <div className="absolute left-3.5 top-1/2 -translate-y-1/2 text-muted-foreground"><Lock className="w-5 h-5" /></div>
                <input
                  id="confirm-password"
                  type={showConfirm ? "text" : "password"}
                  placeholder="Repite la contraseña"
                  value={confirm}
                  onChange={(e) => setConfirm(e.target.value)}
                  required
                  className="w-full pl-10 pr-12 py-3 bg-secondary border border-border rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-emerald-500/30 focus:border-emerald-500 transition-all placeholder:text-muted-foreground/50"
                />
                <button type="button" onClick={() => setShowConfirm(!showConfirm)} className="absolute right-3.5 top-1/2 -translate-y-1/2 text-muted-foreground hover:text-foreground transition-colors p-1">
                  {showConfirm ? <EyeOff className="w-5 h-5" /> : <Eye className="w-5 h-5" />}
                </button>
              </div>
            </div>

            <button
              type="submit"
              disabled={loading}
              className="w-full bg-gradient-to-r from-emerald-500 to-emerald-600 hover:from-emerald-600 hover:to-emerald-700 text-white font-semibold py-3 rounded-xl transition-all duration-300 text-sm mt-2 disabled:opacity-60 disabled:cursor-not-allowed flex items-center justify-center gap-2 shadow-lg shadow-emerald-500/25"
            >
              {loading ? (
                <><span className="w-4 h-4 border-2 border-white/30 border-t-white rounded-full animate-spin" />Guardando...</>
              ) : (
                <>Cambiar contraseña <ArrowRight className="w-4 h-4 ml-1" /></>
              )}
            </button>
          </form>

          <div className="mt-6 pt-6 border-t border-border text-center">
            <Link href="/login" className="inline-flex items-center gap-1 text-sm text-muted-foreground hover:text-foreground transition-colors">
              <ArrowLeft className="w-4 h-4" /> Volver al inicio de sesión
            </Link>
          </div>
        </>
      )}
    </>
  );
}

export default function ResetPasswordPage() {
  const formRef = useRef<HTMLDivElement>(null);
  const logoRef = useRef<HTMLDivElement>(null);

  return (
    <div className="min-h-screen flex relative overflow-hidden">
      {/* Left Panel */}
      <div className="hidden lg:flex lg:w-1/2 relative overflow-hidden bg-gradient-to-br from-[#064e3b] via-[#065f46] to-[#059669]">
        <ThreeBackground />
        <div className="relative z-10 flex flex-col items-center justify-center p-12 w-full">
          <div ref={logoRef} className="text-center">
            <div className="w-24 h-24 bg-white/10 rounded-3xl flex items-center justify-center mx-auto mb-6 backdrop-blur-sm border border-white/20 shadow-2xl shadow-emerald-500/20">
              <Hospital className="w-12 h-12 text-white" />
            </div>
            <h1 className="text-5xl font-bold text-white mb-2 tracking-tight">MedAgenda</h1>
            <p className="text-base font-medium text-emerald-200 max-w-md mx-auto">
              Crea una nueva contraseña segura para proteger tu cuenta.
            </p>
          </div>
        </div>
      </div>

      {/* Right Panel */}
      <div className="w-full lg:w-1/2 flex items-center justify-center p-8 relative overflow-hidden bg-gradient-to-br from-[#064e3b] via-[#065f46] to-[#059669] lg:bg-none lg:bg-background">
        <div className="absolute inset-0 lg:hidden"><ThreeBackground /></div>
        <div className="hidden lg:block absolute inset-0 bg-gradient-to-br pointer-events-none from-emerald-50/50 dark:from-emerald-950/20" />

        <div className="w-full max-w-md relative z-10 lg:translate-y-10">
          <div className="lg:hidden text-center mb-8">
            <h1 className="text-4xl font-bold text-white tracking-tight">MedAgenda</h1>
          </div>

          <div ref={formRef} className="bg-card rounded-3xl shadow-2xl border border-border/50 p-8 backdrop-blur-sm">
            <Suspense fallback={<div className="text-center py-8 text-muted-foreground">Cargando...</div>}>
              <ResetPasswordForm />
            </Suspense>
          </div>

          <p className="text-center text-xs text-white/60 lg:text-muted-foreground mt-6">MedAgenda · 2026</p>
        </div>
      </div>
    </div>
  );
}
