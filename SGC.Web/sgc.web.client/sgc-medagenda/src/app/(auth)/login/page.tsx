"use client";

import { useEffect, useRef, useState } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import anime from "animejs";
import {
  AlertTriangle,
  ArrowRight,
  Bell,
  Bot,
  Calendar,
  CreditCard,
  Eye,
  EyeOff,
  Hospital,
  Lock,
  Mail,
} from "lucide-react";
import { ThreeBackground } from "@/components/animations/Threebackground";
import { AuthService } from "@/services/auth.service";
import { getAuthFeedback } from "@/lib/auth-feedback";

const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

type LoginForm = {
  email: string;
  password: string;
};

type LoginErrors = {
  email?: string;
  password?: string;
};

const initialForm: LoginForm = {
  email: "",
  password: "",
};

export default function LoginPage() {
  const router = useRouter();
  const [form, setForm] = useState<LoginForm>(initialForm);
  const [fieldErrors, setFieldErrors] = useState<LoginErrors>({});
  const [touched, setTouched] = useState({ email: false, password: false });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [showPassword, setShowPassword] = useState(false);

  const formRef = useRef<HTMLDivElement>(null);
  const logoRef = useRef<HTMLDivElement>(null);

  const textColor = "text-emerald-200";
  const rightBgGrad = "from-emerald-50/50 dark:from-emerald-950/20";
  const focusRing = "focus:ring-emerald-500/30 focus:border-emerald-500";
  const textLink = "text-emerald-600 dark:text-emerald-400 hover:text-emerald-700 dark:hover:text-emerald-300";
  const btnGrad =
    "from-emerald-500 to-emerald-600 hover:from-emerald-600 hover:to-emerald-700 shadow-emerald-500/25 hover:shadow-emerald-500/30";

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

    anime({
      targets: ".feature-item",
      translateX: [-30, 0],
      opacity: [0, 1],
      duration: 600,
      delay: anime.stagger(100, { start: 400 }),
      easing: "easeOutExpo",
    });
  }, []);

  const getEmailError = (value: string) => {
    const trimmedValue = value.trim();
    if (!trimmedValue) return "El correo es obligatorio.";
    if (!emailRegex.test(trimmedValue)) return "Ingresa un correo valido.";
    return "";
  };

  const getPasswordError = (value: string) => {
    if (!value) return "La contrasena es obligatoria.";
    if (value.length < 8) return "La contrasena debe tener al menos 8 caracteres.";
    return "";
  };

  const validateForm = () => {
    const nextErrors: LoginErrors = {};
    const emailError = getEmailError(form.email);
    const passwordError = getPasswordError(form.password);

    if (emailError) nextErrors.email = emailError;
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
      [field]: field === "email" ? getEmailError(value) || undefined : getPasswordError(value) || undefined,
    }));
  };

  const handleFieldBlur = (field: keyof LoginForm) => {
    setTouched((prev) => ({ ...prev, [field]: true }));
    setFieldErrors((prev) => ({
      ...prev,
      [field]: field === "email" ? getEmailError(form.email) || undefined : getPasswordError(form.password) || undefined,
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setTouched({ email: true, password: true });

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
        email: form.email.trim(),
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
    <div className="min-h-screen flex relative overflow-hidden">
      <div className="hidden lg:flex lg:w-1/2 relative overflow-hidden bg-gradient-to-br from-[#064e3b] via-[#065f46] to-[#059669]">
        <ThreeBackground />

        <div className="relative z-10 flex flex-col items-center justify-center p-12 w-full">
          <div ref={logoRef} className="text-center" style={{ opacity: 0 }}>
            <div className="w-24 h-24 bg-white/10 rounded-3xl flex items-center justify-center mx-auto mb-6 backdrop-blur-sm border border-white/20 shadow-2xl shadow-emerald-500/20">
              <Hospital className="w-12 h-12 text-white" />
            </div>
            <h1 className="text-5xl font-bold text-white mb-2 tracking-tight">MedAgenda</h1>
            <p className={`text-base font-medium max-w-md mx-auto mb-12 transition-colors duration-500 ${textColor}`}>
              Gestiona y organiza tus citas medicas en un solo lugar, de forma rapida, segura e inteligente.
            </p>
          </div>

          <div className="space-y-4 w-full max-w-xs">
            {[
              { icon: <Calendar className="w-6 h-6 text-white/90" />, text: "Agenda tus citas facilmente" },
              { icon: <Bell className="w-6 h-6 text-white/90" />, text: "Recordatorios automaticos" },
              { icon: <CreditCard className="w-6 h-6 text-white/90" />, text: "Pagos seguros en linea" },
              { icon: <Bot className="w-6 h-6 text-white/90" />, text: "Asistente virtual 24/7" },
            ].map((item, index) => (
              <div
                key={index}
                className="feature-item flex items-center gap-3 bg-white/10 rounded-2xl px-5 py-3.5 backdrop-blur-sm border border-white/10 hover:bg-white/15 transition-colors duration-200"
                style={{ opacity: 0 }}
              >
                {item.icon}
                <span className="text-white/90 font-medium">{item.text}</span>
              </div>
            ))}
          </div>
        </div>
      </div>

      <div className="w-full lg:w-1/2 flex items-center justify-center p-8 relative overflow-hidden bg-gradient-to-br from-[#064e3b] via-[#065f46] to-[#059669] lg:bg-none lg:bg-background">
        <div className="absolute inset-0 lg:hidden">
          <ThreeBackground />
        </div>

        <div className={`hidden lg:block absolute inset-0 bg-gradient-to-br pointer-events-none transition-colors duration-500 ${rightBgGrad}`} />

        <div className="w-full max-w-md relative z-10 lg:translate-y-10">
          <div className="lg:hidden text-center mb-8">
            <h1 className="text-4xl font-bold text-white tracking-tight">MedAgenda</h1>
            <p className="text-emerald-100/80 text-sm mt-3 font-medium max-w-[280px] mx-auto text-center">
              Gestiona y organiza tus citas medicas en un solo lugar, de forma rapida, segura e inteligente.
            </p>
          </div>

          <div ref={formRef} className="bg-card rounded-3xl shadow-2xl border border-border/50 p-8 backdrop-blur-sm" style={{ opacity: 0 }}>
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
                <label className="block text-sm font-medium text-foreground mb-1.5">Correo Electronico</label>
                <div className="relative">
                  <div className="absolute left-3.5 top-1/2 -translate-y-1/2 text-muted-foreground">
                    <Mail className="w-5 h-5" />
                  </div>
                  <input
                    type="email"
                    placeholder="tu@email.com"
                    value={form.email}
                    onChange={(e) => handleFieldChange("email", e.target.value)}
                    onBlur={() => handleFieldBlur("email")}
                    required
                    className={`w-full pl-10 pr-4 py-3 bg-secondary border border-border rounded-xl text-sm focus:outline-none focus:ring-2 transition-all placeholder:text-muted-foreground/50 ${focusRing}`}
                  />
                </div>
                {fieldErrors.email ? <p className="mt-1.5 text-xs text-rose-400">{fieldErrors.email}</p> : null}
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
                className={`submit-btn w-full bg-gradient-to-r text-white font-semibold py-3 rounded-xl transition-all duration-500 text-sm mt-2 disabled:opacity-60 disabled:cursor-not-allowed flex items-center justify-center gap-2 shadow-lg ${btnGrad}`}
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

          <p className="text-center text-xs text-white/60 lg:text-muted-foreground mt-6">MedAgenda · 2026</p>
        </div>
      </div>
    </div>
  );
}
