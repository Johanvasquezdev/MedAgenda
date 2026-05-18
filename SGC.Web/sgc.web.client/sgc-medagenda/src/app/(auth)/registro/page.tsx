"use client";
import { useState, useEffect, useRef } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { ThreeBackground } from "@/components/animations/Threebackground";
import anime from "animejs";
import {
  Loader2, Mail, Lock, User, Phone, Calendar,
  CreditCard, FileText, Eye, EyeOff, ArrowRight,
  Hospital, Sparkles, Stethoscope, HeartPulse, AlertTriangle
} from "lucide-react";
import { toast } from "sonner";
import { PacienteService } from "@/services/paciente.service";

export default function RegistroPage() {
  const router = useRouter();
  const [formData, setFormData] = useState({
    nombre: "", email: "", password: "", confirmPassword: "",
    cedula: "", telefono: "", fechaNacimiento: "", tipoSeguro: "", numeroSeguro: "",
  });
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const [error, setError] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [isSuccess, setIsSuccess] = useState(false);
  const [step, setStep] = useState(1);
  
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

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const passwordStrength = () => {
    const { password } = formData;
    if (password.length === 0) return { strength: 0, label: "", color: "" };
    if (password.length < 6) return { strength: 25, label: "Débil", color: "bg-red-500" };
    if (password.length < 8) return { strength: 50, label: "Regular", color: "bg-yellow-500" };
    if (password.length < 12) return { strength: 75, label: "Buena", color: "bg-blue-500" };
    return { strength: 100, label: "Fuerte", color: "bg-emerald-500" };
  };

  const { strength, label, color } = passwordStrength();

  const handleRegister = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);
    setError("");

    // Button press animation
    anime({
      targets: ".submit-btn",
      scale: [1, 0.97, 1],
      duration: 300,
      easing: "easeOutElastic(1, .5)",
    });

    if (formData.password !== formData.confirmPassword) {
      setError("Las contraseñas no coinciden.");
      setIsLoading(false);
      
      // Error shake animation
      anime({
        targets: formRef.current,
        translateX: [-10, 10, -8, 8, -4, 4, 0],
        duration: 500,
        easing: "easeOutElastic(1, .3)",
      });
      return;
    }

    try {
      await PacienteService.crear({
        nombre: formData.nombre,
        email: formData.email,
        password: formData.password,
        cedula: formData.cedula || undefined,
        telefono: formData.telefono || undefined,
        fechaNacimiento: formData.fechaNacimiento || undefined,
        tipoSeguro: formData.tipoSeguro || undefined,
        numeroSeguro: formData.numeroSeguro || undefined,
      });
      
      setIsSuccess(true);
      toast.success("Cuenta creada correctamente");
      
      // Success animation before redirect
      anime({
        targets: formRef.current,
        translateY: [0, -20],
        opacity: [1, 0],
        duration: 400,
        easing: "easeInExpo",
        complete: () => {
          router.push("/login");
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

      setError("Ocurrió un error al crear la cuenta. Por favor, intenta de nuevo.");
      toast.error("No se pudo crear la cuenta");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex relative overflow-hidden bg-[#0a0f1e] lg:bg-background">
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
            <p className="text-emerald-200 text-base font-medium max-w-md mx-auto mb-12">
              Gestiona y organiza tus citas médicas en un solo lugar, de forma rápida, segura e inteligente.
            </p>
          </div>

          <div className="space-y-4 w-full max-w-xs">
            {[
              { icon: <Sparkles className="w-6 h-6 text-white/95" />, text: "Acceso inmediato al portal" },
              { icon: <Calendar className="w-6 h-6 text-white/95" />, text: "Gestión unificada de citas" },
              { icon: <Stethoscope className="w-6 h-6 text-white/95" />, text: "Directorio de especialistas" },
              { icon: <HeartPulse className="w-6 h-6 text-white/95" />, text: "Historial médico asegurado" },
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
                      p-8 relative overflow-y-auto
                      bg-gradient-to-br from-[#064e3b] via-[#065f46] to-[#059669] lg:bg-none lg:bg-background"
      >
        {/* 3D Background on mobile */}
        <div className="absolute inset-0 lg:hidden z-0">
          <div className="fixed inset-0 pointer-events-none">
             <ThreeBackground />
          </div>
        </div>

        {/* Subtle background pattern (desktop only) */}
        <div
          className="hidden lg:block absolute inset-0 bg-gradient-to-br from-emerald-50/50
                        to-transparent dark:from-emerald-950/20 pointer-events-none"
        />

        <div className="w-full max-w-md relative z-10 my-8 lg:translate-y-10">
          {/* Mobile logo */}
          <div className="lg:hidden text-center mb-8">
            <div className="w-16 h-16 bg-white/10 rounded-3xl flex items-center justify-center mx-auto mb-4 backdrop-blur-sm border border-white/20 shadow-xl shadow-emerald-500/20">
               <Hospital className="w-8 h-8 text-white" />
            </div>
            <h1 className="text-3xl font-bold text-white">MedAgenda</h1>
            <p className="text-emerald-100/80 text-xs mt-2 max-w-[280px] mx-auto text-center">
              Gestiona y organiza tus citas médicas en un solo lugar, de forma rápida, segura e inteligente.
            </p>
          </div>

          <div
            ref={formRef}
            className="bg-card rounded-3xl shadow-2xl border border-border/50
                       p-8 backdrop-blur-sm"
            style={{ opacity: 0 }}
          >
            {isSuccess ? (
               <div className="text-center py-6">
                 <div className="w-16 h-16 bg-emerald-500/20 rounded-full flex items-center justify-center mx-auto mb-4">
                    <span className="text-emerald-500 text-3xl">✓</span>
                 </div>
                 <h2 className="text-2xl font-bold text-foreground">¡Cuenta Creada!</h2>
                 <p className="text-muted-foreground mt-2">Serás redirigido al inicio de sesión...</p>
               </div>
            ) : (
                <>
                    <div className="mb-6">
                        <h2 className="text-2xl font-bold text-foreground">
                            {step === 1 ? "Crear Cuenta" : "Información Adicional"}
                        </h2>
                        <p className="text-muted-foreground text-sm mt-1">
                            {step === 1 ? "Paso 1 de 2: Datos de acceso" : "Paso 2 de 2: Perfil médico"}
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

                    <form onSubmit={handleRegister} className="space-y-4">
                    {step === 1 ? (
                        <>
                        {/* Nombre */}
                        <div>
                            <label className="block text-sm font-medium text-foreground mb-1.5 flex items-center gap-1.5">
                                <User className="w-3.5 h-3.5 text-emerald-600 dark:text-emerald-400" /> Nombre Completo
                            </label>
                            <div className="relative">
                                <input name="nombre" type="text" value={formData.nombre} onChange={handleChange}
                                    className="w-full px-4 py-3 bg-secondary border border-border rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-emerald-500/30 focus:border-emerald-500 transition-all placeholder:text-muted-foreground/50"
                                    placeholder="Juan Pérez" required />
                            </div>
                        </div>

                        {/* Email */}
                        <div>
                            <label className="block text-sm font-medium text-foreground mb-1.5 flex items-center gap-1.5">
                                <Mail className="w-3.5 h-3.5 text-emerald-600 dark:text-emerald-400" /> Correo Electrónico
                            </label>
                            <div className="relative">
                                <input name="email" type="email" value={formData.email} onChange={handleChange}
                                    className="w-full px-4 py-3 bg-secondary border border-border rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-emerald-500/30 focus:border-emerald-500 transition-all placeholder:text-muted-foreground/50"
                                    placeholder="correo@ejemplo.com" required />
                            </div>
                        </div>

                        {/* Password */}
                        <div>
                            <label className="block text-sm font-medium text-foreground mb-1.5 flex items-center gap-1.5">
                                <Lock className="w-3.5 h-3.5 text-emerald-600 dark:text-emerald-400" /> Contraseña
                            </label>
                            <div className="relative">
                                <input name="password" type={showPassword ? "text" : "password"} value={formData.password} onChange={handleChange}
                                    className="w-full px-4 pr-12 py-3 bg-secondary border border-border rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-emerald-500/30 focus:border-emerald-500 transition-all placeholder:text-muted-foreground/50"
                                    placeholder="••••••••" required minLength={6} />
                                <button type="button" onClick={() => setShowPassword(!showPassword)}
                                    className="absolute right-3.5 top-1/2 -translate-y-1/2 text-muted-foreground hover:text-foreground transition-colors">
                                    {showPassword ? <EyeOff className="w-4 h-4" /> : <Eye className="w-4 h-4" />}
                                </button>
                            </div>
                            {formData.password && (
                                <div className="flex items-center gap-3 mt-2">
                                    <div className="flex-1 h-1 bg-secondary rounded-full overflow-hidden">
                                        <div className={`h-full ${color} transition-all duration-300`} style={{ width: `${strength}%` }} />
                                    </div>
                                    <span className={`text-xs ${color.replace("bg-", "text-")}`}>{label}</span>
                                </div>
                            )}
                        </div>

                        {/* Confirm Password */}
                        <div>
                            <label className="block text-sm font-medium text-foreground mb-1.5 flex items-center gap-1.5">
                                <Lock className="w-3.5 h-3.5 text-emerald-600 dark:text-emerald-400" /> Confirmar Contraseña
                            </label>
                            <div className="relative">
                                <input name="confirmPassword" type={showConfirmPassword ? "text" : "password"} value={formData.confirmPassword} onChange={handleChange}
                                    className="w-full px-4 pr-12 py-3 bg-secondary border border-border rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-emerald-500/30 focus:border-emerald-500 transition-all placeholder:text-muted-foreground/50"
                                    placeholder="••••••••" required minLength={6} />
                                <button type="button" onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                                    className="absolute right-3.5 top-1/2 -translate-y-1/2 text-muted-foreground hover:text-foreground transition-colors">
                                    {showConfirmPassword ? <EyeOff className="w-4 h-4" /> : <Eye className="w-4 h-4" />}
                                </button>
                            </div>
                        </div>

                        <button type="button" onClick={() => setStep(2)}
                            className="w-full bg-gradient-to-r from-emerald-500 to-emerald-500 hover:from-emerald-600 hover:to-emerald-600 text-white font-semibold py-3 rounded-xl transition-all duration-200 flex items-center justify-center gap-2 shadow-lg shadow-emerald-500/25 hover:shadow-xl hover:shadow-emerald-500/30 mt-6">
                            Continuar <ArrowRight className="w-4 h-4" />
                        </button>
                        </>
                    ) : (
                        <>
                        {/* Cédula */}
                        <div>
                            <label className="block text-sm font-medium text-foreground mb-1.5 flex items-center gap-1.5">
                                <FileText className="w-3.5 h-3.5 text-emerald-600 dark:text-emerald-400" /> Cédula de Identidad
                            </label>
                            <div className="relative">
                                <input name="cedula" type="text" value={formData.cedula} onChange={handleChange}
                                    className="w-full px-4 py-3 bg-secondary border border-border rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-emerald-500/30 focus:border-emerald-500 transition-all placeholder:text-muted-foreground/50"
                                    placeholder="000-0000000-0" />
                            </div>
                        </div>

                        {/* Teléfono */}
                        <div>
                            <label className="block text-sm font-medium text-foreground mb-1.5 flex items-center gap-1.5">
                                <Phone className="w-3.5 h-3.5 text-emerald-600 dark:text-emerald-400" /> Teléfono
                            </label>
                            <div className="relative">
                                <input name="telefono" type="tel" value={formData.telefono} onChange={handleChange}
                                    className="w-full px-4 py-3 bg-secondary border border-border rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-emerald-500/30 focus:border-emerald-500 transition-all placeholder:text-muted-foreground/50"
                                    placeholder="(809) 000-0000" />
                            </div>
                        </div>

                        {/* Fecha de Nacimiento */}
                        <div>
                            <label className="block text-sm font-medium text-foreground mb-1.5 flex items-center gap-1.5">
                                <Calendar className="w-3.5 h-3.5 text-emerald-600 dark:text-emerald-400" /> Fecha de Nacimiento
                            </label>
                            <div className="relative">
                                <input name="fechaNacimiento" type="date" value={formData.fechaNacimiento} onChange={handleChange}
                                    className="w-full px-4 py-3 bg-secondary border border-border rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-emerald-500/30 focus:border-emerald-500 transition-all [color-scheme:dark]" />
                            </div>
                        </div>

                        {/* Tipo de Seguro */}
                        <div>
                            <label className="block text-sm font-medium text-foreground mb-1.5 flex items-center gap-1.5">
                                <CreditCard className="w-3.5 h-3.5 text-emerald-600 dark:text-emerald-400" /> Tipo de Seguro
                            </label>
                            <div className="relative">
                                <select name="tipoSeguro" value={formData.tipoSeguro} onChange={handleChange}
                                    className="w-full px-4 py-3 bg-secondary border border-border rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-emerald-500/30 focus:border-emerald-500 transition-all appearance-none cursor-pointer">
                                    <option value="">Seleccionar...</option>
                                    <option value="ars-humano">ARS Humano</option>
                                    <option value="ars-universal">ARS Universal</option>
                                    <option value="ars-senasa">SENASA</option>
                                    <option value="ars-reservas">ARS Reservas</option>
                                    <option value="privado">Seguro Privado</option>
                                    <option value="ninguno">Sin Seguro</option>
                                </select>
                            </div>
                        </div>

                        {/* Número de Seguro */}
                        <div>
                            <label className="block text-sm font-medium text-foreground mb-1.5 flex items-center gap-1.5">
                                <FileText className="w-3.5 h-3.5 text-emerald-600 dark:text-emerald-400" /> Número de Seguro
                            </label>
                            <div className="relative">
                                <input name="numeroSeguro" type="text" value={formData.numeroSeguro} onChange={handleChange}
                                    className="w-full px-4 py-3 bg-secondary border border-border rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-emerald-500/30 focus:border-emerald-500 transition-all placeholder:text-muted-foreground/50"
                                    placeholder="Número de afiliación" />
                            </div>
                        </div>

                        <div className="flex gap-3 mt-6">
                            <button type="button" onClick={() => setStep(1)}
                                className="flex-1 bg-secondary hover:bg-secondary/80 border border-border text-foreground font-medium py-3 rounded-xl transition-all">
                                Atrás
                            </button>
                            <button type="submit" disabled={isLoading}
                                className="submit-btn flex-[2] bg-gradient-to-r from-emerald-500 to-emerald-500 hover:from-emerald-600 hover:to-emerald-600 text-white font-semibold py-3 rounded-xl transition-all flex items-center justify-center gap-2 shadow-lg shadow-emerald-500/25 disabled:opacity-70 disabled:cursor-not-allowed">
                                {isLoading ? <><Loader2 className="h-4 w-4 animate-spin" /><span>Creando...</span></> : "Crear Cuenta"}
                            </button>
                        </div>
                        </>
                    )}
                    </form>

                    <div className="mt-6 pt-6 border-t border-border text-center">
                        <p className="text-sm text-muted-foreground">
                            ¿Ya tienes una cuenta?{" "}
                            <Link href="/login" className="text-emerald-600 dark:text-emerald-400 hover:text-emerald-700 dark:hover:text-emerald-300 font-semibold hover:underline transition-colors">
                                Iniciar Sesión
                            </Link>
                        </p>
                    </div>
                </>
            )}
          </div>
          
          <p className="text-center text-xs text-white/60 lg:text-muted-foreground mt-6">
            MedAgenda · 2026
          </p>
        </div>
      </div>
    </div>
  );
}