"use client";
import { useState, useEffect, useRef } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { AuthService } from "@/services/auth.service";
import { ThreeBackground } from "@/components/animations/Threebackground";
import anime from "animejs";
import { Hospital, Calendar, Bell, CreditCard, Bot, AlertTriangle, Mail, Lock, Eye, EyeOff, ArrowRight } from "lucide-react";

export default function LoginPage() {
  const router = useRouter();
  const [form, setForm] = useState({ email: "", password: "" });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  
  const textColor = "text-emerald-200";
  const rightBgGrad = "from-emerald-50/50 dark:from-emerald-950/20";
  const focusRing = "focus:ring-emerald-500/30 focus:border-emerald-500";
  const textLink = "text-emerald-600 dark:text-emerald-400 hover:text-emerald-700 dark:hover:text-emerald-300";
  const btnGrad = "from-emerald-500 to-emerald-600 hover:from-emerald-600 hover:to-emerald-700 shadow-emerald-500/25 hover:shadow-emerald-500/30";

  const formRef = useRef<HTMLDivElement>(null);
  const logoRef = useRef<HTMLDivElement>(null);

  // ── Entrance animations ────────────────────────────────────
  useEffect(() => {
    // Logo bounce in
    anime({
      targets: logoRef.current,
      translateY: [-30, 0],
      opacity: [0, 1],
      duration: 800,
      easing: "easeOutElastic(1, .6)",
    });

    // Form slide up
    anime({
      targets: formRef.current,
      translateY: [40, 0],
      opacity: [0, 1],
      duration: 700,
      delay: 200,
      easing: "easeOutExpo",
    });

    // Feature items stagger
    anime({
      targets: ".feature-item",
      translateX: [-30, 0],
      opacity: [0, 1],
      duration: 600,
      delay: anime.stagger(100, { start: 400 }),
      easing: "easeOutExpo",
    });
  }, []);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError("");

    // Button press animation
    anime({
      targets: ".submit-btn",
      scale: [1, 0.97, 1],
      duration: 300,
      easing: "easeOutElastic(1, .5)",
    });

    try {
      const response = await AuthService.login({ email: form.email, password: form.password });

      // Success animation before redirect
      anime({
        targets: formRef.current,
        translateY: [0, -20],
        opacity: [1, 0],
        duration: 400,
        easing: "easeInExpo",
        complete: () => {
          if (response.rol === "Administrador") router.push("/admin/dashboard");
          else if (response.rol === "Paciente")
            router.push("/paciente/dashboard");
          else if (response.rol === "Medico") router.push("/medico/dashboard");
          else router.push("/");
        },
      });
    } catch (err: any) {
      // Error shake animation
      anime({
        targets: formRef.current,
        translateX: [-10, 10, -8, 8, -4, 4, 0],
        duration: 500,
        easing: "easeOutElastic(1, .3)",
      });

      const msg =
        err?.response?.data?.message ??
        "Credenciales incorrectas. Verifica tu email y contraseña.";
      setError(String(msg));
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex relative overflow-hidden">
      {/* ── Left Panel — Branding with 3D Background ── */}
      <div
        className="hidden lg:flex lg:w-1/2 relative overflow-hidden
                      bg-gradient-to-br from-[#064e3b] via-[#065f46] to-[#059669]"
      >
        {/* Three.js Particles */}
        <ThreeBackground />

        {/* Content over 3D */}
        <div
          className="relative z-10 flex flex-col items-center justify-center
                        p-12 w-full"
        >
          <div ref={logoRef} className="text-center" style={{ opacity: 0 }}>
            <div
              className="w-24 h-24 bg-white/10 rounded-3xl flex items-center
                            justify-center mx-auto mb-6 backdrop-blur-sm
                            border border-white/20 shadow-2xl
                            shadow-emerald-500/20"
            >
              <Hospital className="w-12 h-12 text-white" />
            </div>
            <h1 className="text-5xl font-bold text-white mb-2 tracking-tight">
              MedAgenda
            </h1>
            <p className={`text-base font-medium max-w-md mx-auto mb-12 transition-colors duration-500 ${textColor}`}>
              Gestiona y organiza tus citas médicas en un solo lugar, de forma rápida, segura e inteligente.
            </p>
          </div>

          <div className="space-y-4 w-full max-w-xs">
            {[
              { icon: <Calendar className="w-6 h-6 text-white/90" />, text: "Agenda tus citas fácilmente" },
              { icon: <Bell className="w-6 h-6 text-white/90" />, text: "Recordatorios automáticos" },
              { icon: <CreditCard className="w-6 h-6 text-white/90" />, text: "Pagos seguros en línea" },
              { icon: <Bot className="w-6 h-6 text-white/90" />, text: "Asistente virtual 24/7" },
            ].map((item, i) => (
              <div
                key={i}
                className="feature-item flex items-center gap-3
                           bg-white/10 rounded-2xl px-5 py-3.5
                           backdrop-blur-sm border border-white/10
                           hover:bg-white/15 transition-colors duration-200"
                style={{ opacity: 0 }}
              >
                {item.icon}
                <span className="text-white/90 font-medium">{item.text}</span>
              </div>
            ))}
          </div>
        </div>
      </div>

      {/* ── Right Panel — Form ── */}
      <div
        className="w-full lg:w-1/2 flex items-center justify-center
                      p-8 relative overflow-hidden
                      bg-gradient-to-br from-[#064e3b] via-[#065f46] to-[#059669] lg:bg-none lg:bg-background"
      >
        {/* 3D Background on mobile */}
        <div className="absolute inset-0 lg:hidden">
          <ThreeBackground />
        </div>

        {/* Subtle background pattern (desktop only) */}
        <div
          className={`hidden lg:block absolute inset-0 bg-gradient-to-br pointer-events-none transition-colors duration-500 ${rightBgGrad}`}
        />

        <div className="w-full max-w-md relative z-10 lg:translate-y-10">
          {/* Mobile logo */}
          <div className="lg:hidden text-center mb-8">
            <h1 className="text-4xl font-bold text-white tracking-tight">MedAgenda</h1>
            <p className="text-emerald-100/80 text-sm mt-3 font-medium max-w-[280px] mx-auto text-center">
              Gestiona y organiza tus citas médicas en un solo lugar, de forma rápida, segura e inteligente.
            </p>
          </div>

          <div
            ref={formRef}
            className="bg-card rounded-3xl shadow-2xl border border-border/50
                       p-8 backdrop-blur-sm"
            style={{ opacity: 0 }}
          >
            <div className="mb-8">
              <h2 className="text-2xl font-bold text-foreground">
                Bienvenido de nuevo
              </h2>
              <p className="text-muted-foreground text-sm mt-1">
                Ingresa tus credenciales para continuar
              </p>
            </div>

            {/* Error */}
            {error && (
              <div
                className="flex items-start gap-2 bg-destructive/10
                              border border-destructive/20 text-destructive
                              rounded-2xl p-4 text-sm mb-6"
              >
                <AlertTriangle className="w-5 h-5 flex-shrink-0" />
                <span>{error}</span>
              </div>
            )}

            <form onSubmit={handleSubmit} className="space-y-5">
              {/* Email */}
              <div>
                <label className="block text-sm font-medium text-foreground mb-1.5">
                  Correo Electrónico
                </label>
                <div className="relative">
                  <div
                    className="absolute left-3.5 top-1/2 -translate-y-1/2
                                   text-muted-foreground"
                  >
                    <Mail className="w-5 h-5" />
                  </div>
                  <input
                    type="email"
                    placeholder="tu@email.com"
                    value={form.email}
                    onChange={(e) =>
                      setForm({ ...form, email: e.target.value })
                    }
                    required
                    className={`w-full pl-10 pr-4 py-3 bg-secondary border border-border rounded-xl text-sm focus:outline-none focus:ring-2 transition-all placeholder:text-muted-foreground/50 ${focusRing}`}
                  />
                </div>
              </div>

              {/* Password */}
              <div>
                <div className="flex items-center justify-between mb-1.5">
                  <label className="block text-sm font-medium text-foreground">
                    Contraseña
                  </label>
                  <Link
                    href="/forgot-password"
                    className={`text-xs hover:underline font-medium transition-colors ${textLink}`}
                  >
                    ¿Olvidaste tu contraseña?
                  </Link>
                </div>
                <div className="relative">
                  <div
                    className="absolute left-3.5 top-1/2 -translate-y-1/2
                                   text-muted-foreground"
                  >
                    <Lock className="w-5 h-5" />
                  </div>
                  <input
                    type={showPassword ? "text" : "password"}
                    placeholder="••••••••"
                    value={form.password}
                    onChange={(e) =>
                      setForm({ ...form, password: e.target.value })
                    }
                    required
                    className={`w-full pl-10 pr-12 py-3 bg-secondary border border-border rounded-xl text-sm focus:outline-none focus:ring-2 transition-all placeholder:text-muted-foreground/50 ${focusRing}`}
                  />
                  <button
                    type="button"
                    onClick={() => setShowPassword(!showPassword)}
                    className="absolute right-3.5 top-1/2 -translate-y-1/2
                                text-muted-foreground hover:text-foreground
                                transition-colors p-1"
                  >
                    {showPassword ? <EyeOff className="w-5 h-5" /> : <Eye className="w-5 h-5" />}
                  </button>
                </div>
              </div>

              {/* Submit */}
              <button
                type="submit"
                disabled={loading}
                className={`submit-btn w-full bg-gradient-to-r text-white font-semibold py-3 rounded-xl transition-all duration-500 text-sm mt-2 disabled:opacity-60 disabled:cursor-not-allowed flex items-center justify-center gap-2 shadow-lg ${btnGrad}`}
              >
                {loading ? (
                  <>
                    <span
                      className="w-4 h-4 border-2 border-white/30
                                     border-t-white rounded-full animate-spin"
                    />
                    Iniciando sesión...
                  </>
                ) : (
                  <>
                    Iniciar Sesión <ArrowRight className="w-4 h-4 ml-1" />
                  </>
                )}
              </button>
            </form>

            <div className="mt-6 pt-6 border-t border-border text-center">
              <p className="text-sm text-muted-foreground">
                ¿No tienes cuenta?{" "}
                <Link
                  href="/registro"
                  className={`font-semibold hover:underline transition-colors duration-500 ${textLink}`}
                >
                  Regístrate gratis
                </Link>
              </p>
            </div>
          </div>

          <p className="text-center text-xs text-white/60 lg:text-muted-foreground mt-6">
            MedAgenda · 2026
          </p>
        </div>
      </div>
    </div>
  );
}
