"use client";

import { BrandLockup } from "@/components/brand/BrandLockup";

import { useEffect, useRef, useState } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import anime from "animejs";
import {
  AlertTriangle,
  ArrowRight,
  Bell,
  MessageSquareText,
  Calendar,
  CreditCard,
  Eye,
  EyeOff,
  Lock,
  UserRound,
} from "lucide-react";
import { AuthService } from "@/services/auth.service";
import { getAuthFeedback } from "@/lib/auth-feedback";

type LoginForm = {
  identifier: string;
  password: string;
};

type LoginErrors = {
  identifier?: string;
  password?: string;
};

const initialForm: LoginForm = {
  identifier: "",
  password: "",
};

export default function LoginPage() {
  const router = useRouter();
  const [form, setForm] = useState<LoginForm>(initialForm);
  const [fieldErrors, setFieldErrors] = useState<LoginErrors>({});
  const [touched, setTouched] = useState({ identifier: false, password: false });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [showPassword, setShowPassword] = useState(false);

  const formRef = useRef<HTMLDivElement>(null);
  const logoRef = useRef<HTMLDivElement>(null);

  const textColor = "text-emerald-200";
  const focusRing = "focus:ring-emerald-500/30 focus:border-emerald-500";
  const textLink = "text-emerald-600 dark:text-emerald-400 hover:text-emerald-700 dark:hover:text-emerald-300";

  useEffect(() => {
    if (window.matchMedia("(prefers-reduced-motion: reduce)").matches) {
      if (logoRef.current) logoRef.current.style.opacity = "1";
      if (formRef.current) formRef.current.style.opacity = "1";
      document.querySelectorAll(".feature-item").forEach((node) => { (node as HTMLElement).style.opacity = "1"; });
      return;
    }
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

    anime({
      targets: ".feature-item",
      translateX: [-30, 0],
      opacity: [0, 1],
      duration: 600,
      delay: anime.stagger(100, { start: 400 }),
      easing: "easeOutExpo",
    });
  }, []);

  const getIdentifierError = (value: string) => {
    if (!value.trim()) return "Ingresa tu usuario o correo.";
    return "";
  };

  const getPasswordError = (value: string) => {
    if (!value) return "La contrasena es obligatoria.";
    if (value.length < 8) return "La contrasena debe tener al menos 8 caracteres.";
    return "";
  };

  const validateForm = () => {
    const nextErrors: LoginErrors = {};
    const identifierError = getIdentifierError(form.identifier);
    const passwordError = getPasswordError(form.password);

    if (identifierError) nextErrors.identifier = identifierError;
    if (passwordError) nextErrors.password = passwordError;

    return nextErrors;
  };

  const animateError = () => {
    anime({
      targets: formRef.current,
      translateX: [-10, 10, -8, 8, -4, 4, 0],
      duration: 500,
      easing: "easeOutElastic(1, .3)",
    });
  };

  const handleFieldChange = (field: keyof LoginForm, value: string) => {
    setForm((prev) => ({ ...prev, [field]: value }));
    setError("");

    if (!touched[field]) return;

    setFieldErrors((prev) => ({
      ...prev,
      [field]: field === "identifier" ? getIdentifierError(value) || undefined : getPasswordError(value) || undefined,
    }));
  };

  const handleFieldBlur = (field: keyof LoginForm) => {
    setTouched((prev) => ({ ...prev, [field]: true }));
    setFieldErrors((prev) => ({
      ...prev,
      [field]: field === "identifier" ? getIdentifierError(form.identifier) || undefined : getPasswordError(form.password) || undefined,
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setTouched({ identifier: true, password: true });

    const nextErrors = validateForm();
    if (Object.keys(nextErrors).length > 0) {
      setFieldErrors(nextErrors);
      setError("Revisa tus credenciales antes de continuar.");
      animateError();
      return;
    }

    setLoading(true);
    setError("");
    setFieldErrors({});

    anime({
      targets: ".submit-btn",
      scale: [1, 0.97, 1],
      duration: 300,
      easing: "easeOutElastic(1, .5)",
    });

    try {
      const response = await AuthService.login({
        identifier: form.identifier.trim(),
        password: form.password,
      });

      anime({
        targets: formRef.current,
        translateY: [0, -20],
        opacity: [1, 0],
        duration: 400,
        easing: "easeInExpo",
        complete: () => {
          if (response.rol === "Administrador") router.push("/admin/dashboard");
          else if (response.rol === "Paciente") router.push("/paciente/dashboard");
          else if (response.rol === "Medico") router.push("/medico/dashboard");
          else router.push("/");
        },
      });
    } catch (err: any) {
      animateError();
      setError(getAuthFeedback(err, "login"));
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex bg-transparent text-foreground">
      <div className="hidden lg:flex lg:w-1/2 relative overflow-hidden bg-[#0b261b]/85 backdrop-blur-[1px]">

        <div className="relative z-10 flex flex-col items-center justify-center p-12 w-full">
          <div ref={logoRef} className="text-center" style={{ opacity: 0 }}>
            <BrandLockup markSize="lg" className="mb-6 justify-center text-3xl text-white" />
            <p className={`text-base font-medium max-w-md mx-auto mb-12 transition-colors duration-500 ${textColor}`}>
              Gestiona y organiza tus citas medicas en un solo lugar, de forma rapida, segura e inteligente.
            </p>
          </div>

          <div className="space-y-4 w-full max-w-xs">
            {[
              { icon: <Calendar className="w-6 h-6 text-white/90" />, text: "Agenda tus citas facilmente" },
              { icon: <Bell className="w-6 h-6 text-white/90" />, text: "Recordatorios automaticos" },
              { icon: <CreditCard className="w-6 h-6 text-white/90" />, text: "Pagos seguros en linea" },
              { icon: <MessageSquareText className="w-6 h-6 text-white/90" />, text: "Asistente virtual 24/7" },
            ].map((item, index) => (
              <div
                key={index}
                className="feature-item flex items-center gap-3 bg-white/10 rounded-2xl px-5 py-3.5 border border-white/10 hover:bg-white/15 transition-colors duration-200"
                style={{ opacity: 0 }}
              >
                {item.icon}
                <span className="text-white/90 font-medium">{item.text}</span>
              </div>
            ))}
          </div>
        </div>
      </div>

      <div className="w-full lg:w-1/2 flex items-center justify-center p-8 relative overflow-hidden bg-transparent">

        <div className="w-full max-w-md relative z-10 lg:translate-y-10">
          <div className="lg:hidden text-center mb-8">
            <BrandLockup markSize="sm" className="justify-center text-xl text-foreground" />
            <p className="mt-3 max-w-[280px] mx-auto text-center text-sm leading-6 text-muted-foreground">
              Gestiona y organiza tus citas medicas en un solo lugar, de forma rapida, segura e inteligente.
            </p>
          </div>

          <div ref={formRef} className="bg-card rounded-2xl border border-border p-6 shadow-[0_20px_55px_-35px_rgba(0,0,0,0.55)] sm:p-8 " style={{ opacity: 0 }}>
            <div className="mb-8">
              <h2 className="text-2xl font-bold text-foreground">Bienvenido de nuevo</h2>
              <p className="text-muted-foreground text-sm mt-1">Ingresa tus credenciales para continuar</p>
            </div>

            {error ? (
              <div className="flex items-start gap-2 bg-destructive/10 border border-destructive/20 text-destructive rounded-2xl p-4 text-sm mb-6">
                <AlertTriangle className="w-5 h-5 flex-shrink-0" />
                <span>{error}</span>
              </div>
            ) : null}

            <form onSubmit={handleSubmit} className="space-y-5">
              <div>
                <label className="block text-sm font-medium text-foreground mb-1.5">Usuario o correo electrónico</label>
                <div className="relative">
                  <div className="absolute left-3.5 top-1/2 -translate-y-1/2 text-muted-foreground">
                    <UserRound className="w-5 h-5" />
                  </div>
                  <input
                    type="text"
                    placeholder="Usuario o correo@dominio.com"
                    value={form.identifier}
                    onChange={(e) => handleFieldChange("identifier", e.target.value)}
                    onBlur={() => handleFieldBlur("identifier")}
                    autoComplete="username"
                    required
                    className={`w-full pl-10 pr-4 py-3 bg-secondary border border-border rounded-xl text-sm focus:outline-none focus:ring-2 transition-all placeholder:text-muted-foreground/50 ${focusRing}`}
                  />
                </div>
                {fieldErrors.identifier ? <p className="mt-1.5 text-xs text-rose-400">{fieldErrors.identifier}</p> : null}
              </div>

              <div>
                <div className="flex items-center justify-between mb-1.5">
                  <label className="block text-sm font-medium text-foreground">Contrasena</label>
                  <Link href="/forgot-password" className={`text-xs hover:underline font-medium transition-colors ${textLink}`}>
                    Olvidaste tu contrasena?
                  </Link>
                </div>
                <div className="relative">
                  <div className="absolute left-3.5 top-1/2 -translate-y-1/2 text-muted-foreground">
                    <Lock className="w-5 h-5" />
                  </div>
                  <input
                    type={showPassword ? "text" : "password"}
                    placeholder="••••••••"
                    value={form.password}
                    onChange={(e) => handleFieldChange("password", e.target.value)}
                    onBlur={() => handleFieldBlur("password")}
                    required
                    className={`w-full pl-10 pr-12 py-3 bg-secondary border border-border rounded-xl text-sm focus:outline-none focus:ring-2 transition-all placeholder:text-muted-foreground/50 ${focusRing}`}
                  />
                  <button
                    type="button"
                    onClick={() => setShowPassword((prev) => !prev)}
                    className="absolute right-3.5 top-1/2 -translate-y-1/2 text-muted-foreground hover:text-foreground transition-colors p-1"
                  >
                    {showPassword ? <EyeOff className="w-5 h-5" /> : <Eye className="w-5 h-5" />}
                  </button>
                </div>
                {fieldErrors.password ? <p className="mt-1.5 text-xs text-rose-400">{fieldErrors.password}</p> : null}
              </div>

              <button
                type="submit"
                disabled={loading}
                className="submit-btn flex min-h-11 w-full items-center justify-center gap-2 rounded-xl bg-primary py-3 text-sm font-semibold text-primary-foreground transition hover:brightness-110 disabled:cursor-not-allowed disabled:opacity-60"
              >
                {loading ? (
                  <>
                    <span className="w-4 h-4 border-2 border-white/30 border-t-white rounded-full animate-spin" />
                    Iniciando sesion...
                  </>
                ) : (
                  <>
                    Iniciar Sesion <ArrowRight className="w-4 h-4 ml-1" />
                  </>
                )}
              </button>
            </form>

            <div className="mt-6 pt-6 border-t border-border text-center">
              <p className="text-sm text-muted-foreground">
                No tienes cuenta?{" "}
                <Link href="/registro" className={`font-semibold hover:underline transition-colors duration-500 ${textLink}`}>
                  Registrate gratis
                </Link>
              </p>
            </div>
          </div>

          <p className="mt-6 text-center text-xs text-muted-foreground">MedAgenda · 2026</p>
        </div>
      </div>
    </div>
  );
}
