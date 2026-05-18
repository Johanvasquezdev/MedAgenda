"use client";

import { useEffect, useRef, useState } from "react";
import Link from "next/link";
import axios from "axios";
import anime from "animejs";
import { AlertTriangle, ArrowLeft, ArrowRight, CheckCircle, Home, Hospital, Mail } from "lucide-react";
import { ThreeBackground } from "@/components/animations/Threebackground";

const API =
  process.env.NEXT_PUBLIC_API_BASE_URL?.trim() ||
  process.env.NEXT_PUBLIC_API_URL?.trim() ||
  "http://localhost:5189";

export default function ForgotPasswordPage() {
  const [email, setEmail] = useState("");
  const [loading, setLoading] = useState(false);
  const [sent, setSent] = useState(false);
  const [error, setError] = useState("");
  const formRef = useRef<HTMLDivElement>(null);
  const logoRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    anime({
      targets: logoRef.current,
      translateY: [-30, 0],
      opacity: [0, 1],
      duration: 800,
      easing: "easeOutElastic(1, .6)",
    });

    anime({
      targets: formRef.current,
      translateY: [40, 0],
      opacity: [0, 1],
      duration: 700,
      delay: 200,
      easing: "easeOutExpo",
    });
  }, []);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError("");

    try {
      await axios.post(`${API}/api/auth/forgot-password`, { email });
      setSent(true);
    } catch (error: any) {
      setError(
        error?.response?.data?.message ||
        error?.response?.data?.mensaje ||
        "No pudimos procesar la solicitud. Intentalo nuevamente."
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex relative overflow-hidden">
      <div className="hidden lg:flex lg:w-1/2 relative overflow-hidden bg-gradient-to-br from-[#064e3b] via-[#065f46] to-[#059669]">
        <ThreeBackground />
        <div className="relative z-10 flex flex-col items-center justify-center p-12 w-full">
          <div ref={logoRef} className="text-center" style={{ opacity: 0 }}>
            <div className="w-24 h-24 bg-white/10 rounded-3xl flex items-center justify-center mx-auto mb-6 backdrop-blur-sm border border-white/20 shadow-2xl shadow-emerald-500/20">
              <Hospital className="w-12 h-12 text-white" />
            </div>
            <h1 className="text-5xl font-bold text-white mb-2 tracking-tight">MedAgenda</h1>
            <p className="text-base font-medium text-emerald-200 max-w-md mx-auto">
              Recupera el acceso a tu cuenta de forma segura y vuelve al portal cuando quieras.
            </p>
          </div>
        </div>
      </div>

      <div className="w-full lg:w-1/2 flex items-center justify-center p-8 relative overflow-hidden bg-gradient-to-br from-[#064e3b] via-[#065f46] to-[#059669] lg:bg-none lg:bg-background">
        <div className="absolute inset-0 lg:hidden">
          <ThreeBackground />
        </div>
        <div className="hidden lg:block absolute inset-0 bg-gradient-to-br pointer-events-none from-emerald-50/50 dark:from-emerald-950/20" />

        <div className="w-full max-w-md relative z-10 lg:translate-y-10">
          <div className="lg:hidden text-center mb-8">
            <h1 className="text-4xl font-bold text-white tracking-tight">MedAgenda</h1>
          </div>

          <div ref={formRef} className="bg-card rounded-3xl shadow-2xl border border-border/50 p-8 backdrop-blur-sm" style={{ opacity: 0 }}>
            {sent ? (
              <div className="text-center py-4">
                <div className="w-16 h-16 bg-emerald-500/10 rounded-full flex items-center justify-center mx-auto mb-4">
                  <CheckCircle className="w-8 h-8 text-emerald-500" />
                </div>
                <h2 className="text-2xl font-bold text-foreground mb-2">Revisa tu correo</h2>
                <p className="text-muted-foreground text-sm mb-6">
                  Si <strong>{email}</strong> esta registrado, recibiras un enlace para restablecer tu contrasena en los proximos minutos.
                </p>
                <div className="flex flex-wrap items-center justify-center gap-3">
                  <Link href="/" className="inline-flex items-center gap-2 rounded-full border border-border px-4 py-2 text-sm font-semibold text-foreground transition-colors hover:bg-secondary">
                    <Home className="w-4 h-4" /> Ir a la landing
                  </Link>
                  <Link href="/login" className="inline-flex items-center gap-2 text-sm text-emerald-600 dark:text-emerald-400 font-semibold hover:underline">
                    <ArrowLeft className="w-4 h-4" /> Volver al inicio de sesion
                  </Link>
                </div>
              </div>
            ) : (
              <>
                <div className="mb-8">
                  <h2 className="text-2xl font-bold text-foreground">Olvidaste tu contrasena?</h2>
                  <p className="text-muted-foreground text-sm mt-1">
                    Ingresa tu correo y te enviaremos un enlace para restablecerla.
                  </p>
                </div>

                {error ? (
                  <div className="flex items-start gap-2 bg-destructive/10 border border-destructive/20 text-destructive rounded-2xl p-4 text-sm mb-6">
                    <AlertTriangle className="w-5 h-5 flex-shrink-0" />
                    <span>{error}</span>
                  </div>
                ) : null}

                <form onSubmit={handleSubmit} className="space-y-5">
                  <div>
                    <label className="block text-sm font-medium text-foreground mb-1.5">Correo Electronico</label>
                    <div className="relative">
                      <div className="absolute left-3.5 top-1/2 -translate-y-1/2 text-muted-foreground">
                        <Mail className="w-5 h-5" />
                      </div>
                      <input
                        id="forgot-email"
                        type="email"
                        placeholder="tu@email.com"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        required
                        className="w-full pl-10 pr-4 py-3 bg-secondary border border-border rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-emerald-500/30 focus:border-emerald-500 transition-all placeholder:text-muted-foreground/50"
                      />
                    </div>
                  </div>

                  <button
                    type="submit"
                    disabled={loading}
                    className="w-full bg-gradient-to-r from-emerald-500 to-emerald-600 hover:from-emerald-600 hover:to-emerald-700 text-white font-semibold py-3 rounded-xl transition-all duration-300 text-sm mt-2 disabled:opacity-60 disabled:cursor-not-allowed flex items-center justify-center gap-2 shadow-lg shadow-emerald-500/25"
                  >
                    {loading ? (
                      <>
                        <span className="w-4 h-4 border-2 border-white/30 border-t-white rounded-full animate-spin" />
                        Enviando...
                      </>
                    ) : (
                      <>
                        Enviar enlace <ArrowRight className="w-4 h-4 ml-1" />
                      </>
                    )}
                  </button>
                </form>

                <div className="mt-6 pt-6 border-t border-border text-center">
                  <div className="flex flex-wrap items-center justify-center gap-4">
                    <Link href="/" className="inline-flex items-center gap-1 text-sm text-muted-foreground hover:text-foreground transition-colors">
                      <Home className="w-4 h-4" /> Ir al inicio
                    </Link>
                    <Link href="/login" className="inline-flex items-center gap-1 text-sm text-muted-foreground hover:text-foreground transition-colors">
                      <ArrowLeft className="w-4 h-4" /> Volver al inicio de sesion
                    </Link>
                  </div>
                </div>
              </>
            )}
          </div>

          <p className="text-center text-xs text-white/60 lg:text-muted-foreground mt-6">MedAgenda · 2026</p>
        </div>
      </div>
    </div>
  );
}
