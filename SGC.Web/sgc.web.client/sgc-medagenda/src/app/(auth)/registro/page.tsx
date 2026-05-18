"use client";

import { useEffect, useMemo, useRef, useState } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import anime from "animejs";
import PhoneInput from "react-phone-number-input/input";
import { ThreeBackground } from "@/components/animations/Threebackground";
import { getAuthFeedback } from "@/lib/auth-feedback";
import { PacienteService } from "@/services/paciente.service";
import { toast } from "sonner";
import {
  AlertTriangle,
  ArrowRight,
  Building2,
  Calendar,
  CheckCircle2,
  ChevronDown,
  CreditCard,
  Eye,
  EyeOff,
  FileText,
  HeartPulse,
  Hospital,
  Loader2,
  Lock,
  Mail,
  Phone,
  ShieldCheck,
  Sparkles,
  Stethoscope,
  User,
} from "lucide-react";

type SeguroOption = {
  value: string;
  label: string;
  prefix: string;
  accent: string;
  icon: typeof ShieldCheck;
};

type FormData = {
  nombre: string;
  apellido: string;
  email: string;
  password: string;
  confirmPassword: string;
  cedula: string;
  telefono: string;
  fechaNacimiento: string;
  tipoSeguro: string;
  numeroSeguro: string;
};

type FormErrors = Partial<Record<keyof FormData, string>>;

const initialFormData: FormData = {
  nombre: "",
  apellido: "",
  email: "",
  password: "",
  confirmPassword: "",
  cedula: "",
  telefono: "",
  fechaNacimiento: "",
  tipoSeguro: "",
  numeroSeguro: "",
};

const seguroOptions: SeguroOption[] = [
  {
    value: "ars-humano",
    label: "ARS Humano",
    prefix: "HUM",
    accent: "from-sky-500/20 to-cyan-500/20 text-sky-200 border-sky-400/30",
    icon: HeartPulse,
  },
  {
    value: "ars-universal",
    label: "ARS Universal",
    prefix: "UNI",
    accent: "from-violet-500/20 to-indigo-500/20 text-violet-200 border-violet-400/30",
    icon: Sparkles,
  },
  {
    value: "ars-senasa",
    label: "SENASA",
    prefix: "SEN",
    accent: "from-emerald-500/20 to-teal-500/20 text-emerald-200 border-emerald-400/30",
    icon: ShieldCheck,
  },
  {
    value: "ars-reservas",
    label: "ARS Reservas",
    prefix: "ARS",
    accent: "from-amber-500/20 to-orange-500/20 text-amber-200 border-amber-400/30",
    icon: Building2,
  },
  {
    value: "privado",
    label: "Seguro Privado",
    prefix: "PRI",
    accent: "from-rose-500/20 to-fuchsia-500/20 text-rose-200 border-rose-400/30",
    icon: CreditCard,
  },
];

const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
const startsWithLetterRegex = /^[A-Za-zÁÉÍÓÚÜÑáéíóúüñ]/;

function formatCedula(value: string) {
  const digits = value.replace(/\D/g, "").slice(0, 11);
  if (digits.length <= 3) return digits;
  if (digits.length <= 10) return `${digits.slice(0, 3)}-${digits.slice(3)}`;
  return `${digits.slice(0, 3)}-${digits.slice(3, 10)}-${digits.slice(10)}`;
}

function normalizePhone(value: string | undefined) {
  return value ?? "";
}

function getDominicanPhoneDigits(value: string) {
  const digits = value.replace(/\D/g, "");
  return digits.length === 11 && digits.startsWith("1") ? digits.slice(1) : digits;
}

function formatDominicanPhone(value: string) {
  const digits = getDominicanPhoneDigits(value).slice(0, 10);
  if (digits.length <= 3) return digits;
  if (digits.length <= 6) return `${digits.slice(0, 3)}-${digits.slice(3)}`;
  return `${digits.slice(0, 3)}-${digits.slice(3, 6)}-${digits.slice(6)}`;
}

function getPasswordChecks(password: string) {
  return [
    { key: "length", label: "Mínimo 8 caracteres", valid: password.length >= 8 },
    { key: "upper", label: "Al menos 1 mayúscula", valid: /[A-Z]/.test(password) },
    { key: "lower", label: "Al menos 1 minúscula", valid: /[a-z]/.test(password) },
    { key: "number", label: "Al menos 1 número", valid: /\d/.test(password) },
    { key: "special", label: "Al menos 1 carácter especial", valid: /[^A-Za-z0-9]/.test(password) },
  ];
}

function ValidationItem({ valid, label }: { valid: boolean; label: string }) {
  return (
    <div className={`flex items-center gap-2 text-xs ${valid ? "text-emerald-400" : "text-muted-foreground"}`}>
      <span
        className={`h-1.5 w-1.5 rounded-full ${valid ? "bg-emerald-400 shadow-[0_0_10px_rgba(16,185,129,0.55)]" : "bg-white/20"}`}
      />
      <span>{label}</span>
    </div>
  );
}

export default function RegistroPage() {
  const router = useRouter();
  const [formData, setFormData] = useState<FormData>(initialFormData);
  const [errors, setErrors] = useState<FormErrors>({});
  const [error, setError] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [isSuccess, setIsSuccess] = useState(false);
  const [step, setStep] = useState(1);
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const [showSeguroOptions, setShowSeguroOptions] = useState(false);

  const formRef = useRef<HTMLDivElement>(null);
  const logoRef = useRef<HTMLDivElement>(null);
  const seguroMenuRef = useRef<HTMLDivElement>(null);

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

  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (seguroMenuRef.current && !seguroMenuRef.current.contains(event.target as Node)) {
        setShowSeguroOptions(false);
      }
    };

    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  const selectedSeguro = useMemo(
    () => seguroOptions.find((option) => option.value === formData.tipoSeguro) ?? null,
    [formData.tipoSeguro]
  );

  const passwordChecks = useMemo(() => getPasswordChecks(formData.password), [formData.password]);
  const isPasswordValid = passwordChecks.every((check) => check.valid);
  const passwordStrength = passwordChecks.filter((check) => check.valid).length;
  const confirmMatches = formData.confirmPassword.length > 0 && formData.password === formData.confirmPassword;

  const setFieldValue = <K extends keyof FormData>(field: K, value: FormData[K]) => {
    setFormData((prev) => ({ ...prev, [field]: value }));
    setErrors((prev) => ({ ...prev, [field]: undefined }));
    setError("");
  };

  const validateStep1 = () => {
    const nextErrors: FormErrors = {};

    if (!formData.nombre.trim()) nextErrors.nombre = "El nombre es obligatorio.";
    else if (!startsWithLetterRegex.test(formData.nombre.trim())) nextErrors.nombre = "El nombre debe iniciar con una letra.";

    if (!formData.apellido.trim()) nextErrors.apellido = "El apellido es obligatorio.";
    else if (!startsWithLetterRegex.test(formData.apellido.trim())) nextErrors.apellido = "El apellido debe iniciar con una letra.";

    if (!formData.email.trim()) nextErrors.email = "El correo es obligatorio.";
    else if (!emailRegex.test(formData.email.trim())) nextErrors.email = "Ingresa un correo válido.";

    if (!formData.password) nextErrors.password = "La contraseña es obligatoria.";
    else if (!isPasswordValid) nextErrors.password = "La contraseña aún no cumple todos los requisitos.";

    if (!formData.confirmPassword) nextErrors.confirmPassword = "Confirma tu contraseña.";
    else if (formData.password !== formData.confirmPassword) nextErrors.confirmPassword = "Las contraseñas no coinciden.";

    return nextErrors;
  };

  const validateStep2 = () => {
    const nextErrors: FormErrors = {};
    const cedulaDigits = formData.cedula.replace(/\D/g, "");
    const insuranceDigits = formData.numeroSeguro.replace(/\D/g, "");

    if (!cedulaDigits) nextErrors.cedula = "La cédula es obligatoria.";
    else if (cedulaDigits.length !== 11) nextErrors.cedula = "La cédula debe tener 11 dígitos.";

    if (!formData.telefono) nextErrors.telefono = "El teléfono es obligatorio.";
    else if (getDominicanPhoneDigits(formData.telefono).length !== 10) nextErrors.telefono = "El teléfono debe tener 10 dígitos.";

    if (!formData.fechaNacimiento) nextErrors.fechaNacimiento = "Selecciona tu fecha de nacimiento.";

    if (!formData.tipoSeguro) nextErrors.tipoSeguro = "Selecciona tu tipo de seguro.";

    if (formData.tipoSeguro && !insuranceDigits) nextErrors.numeroSeguro = "Ingresa el número de tu seguro.";

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

  const handleNextStep = () => {
    const nextErrors = validateStep1();
    if (Object.keys(nextErrors).length > 0) {
      setErrors(nextErrors);
      setError("Revisa los datos de acceso antes de continuar.");
      animateError();
      return;
    }

    setError("");
    setErrors({});
    setStep(2);
  };

  const handleRegister = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);
    setError("");

    anime({
      targets: ".submit-btn",
      scale: [1, 0.97, 1],
      duration: 300,
      easing: "easeOutElastic(1, .5)",
    });

    const step1Errors = validateStep1();
    const step2Errors = validateStep2();
    const nextErrors = { ...step1Errors, ...step2Errors };

    if (Object.keys(nextErrors).length > 0) {
      setErrors(nextErrors);
      setError("Completa los campos requeridos antes de crear la cuenta.");
      setIsLoading(false);
      animateError();
      return;
    }

    try {
      await PacienteService.crear({
        nombre: `${formData.nombre.trim()} ${formData.apellido.trim()}`,
        email: formData.email.trim(),
        password: formData.password,
        cedula: formData.cedula,
        telefono: formatDominicanPhone(formData.telefono),
        fechaNacimiento: formData.fechaNacimiento || undefined,
        tipoSeguro: selectedSeguro?.label || undefined,
        numeroSeguro: selectedSeguro ? `${selectedSeguro.prefix}-${formData.numeroSeguro.replace(/\D/g, "")}` : undefined,
      });

      setIsSuccess(true);
      toast.success("Cuenta creada correctamente");

      anime({
        targets: formRef.current,
        translateY: [0, -20],
        opacity: [1, 0],
        duration: 400,
        easing: "easeInExpo",
        complete: () => router.push("/login"),
      });
    } catch (err: any) {
      animateError();
      const backendMessage = getAuthFeedback(err, "register");
      setError(String(backendMessage));
      toast.error(String(backendMessage));
    } finally {
      setIsLoading(false);
    }
  };

  const renderFieldError = (field: keyof FormErrors) =>
    errors[field] ? <p className="mt-1.5 text-xs text-rose-400">{errors[field]}</p> : null;

  return (
    <div className="min-h-screen flex relative overflow-hidden bg-[#0a0f1e] lg:bg-background">
      <div className="hidden lg:flex lg:w-1/2 relative overflow-hidden bg-gradient-to-br from-[#064e3b] via-[#065f46] to-[#059669]">
        <ThreeBackground />

        <div className="relative z-10 flex flex-col items-center justify-center p-12 w-full">
          <div ref={logoRef} className="text-center" style={{ opacity: 0 }}>
            <div className="w-24 h-24 bg-white/10 rounded-3xl flex items-center justify-center mx-auto mb-6 backdrop-blur-sm border border-white/20 shadow-2xl shadow-emerald-500/20">
              <Hospital className="w-12 h-12 text-white" />
            </div>
            <h1 className="text-5xl font-bold text-white mb-2 tracking-tight">MedAgenda</h1>
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

      <div className="w-full lg:w-1/2 flex items-center justify-center p-8 relative overflow-y-auto bg-gradient-to-br from-[#064e3b] via-[#065f46] to-[#059669] lg:bg-none lg:bg-background">
        <div className="absolute inset-0 lg:hidden z-0">
          <div className="fixed inset-0 pointer-events-none">
            <ThreeBackground />
          </div>
        </div>

        <div className="hidden lg:block absolute inset-0 bg-gradient-to-br from-emerald-50/50 to-transparent dark:from-emerald-950/20 pointer-events-none" />

        <div className="w-full max-w-md relative z-10 my-8 lg:translate-y-10">
          <div className="lg:hidden text-center mb-8">
            <div className="w-16 h-16 bg-white/10 rounded-3xl flex items-center justify-center mx-auto mb-4 backdrop-blur-sm border border-white/20 shadow-xl shadow-emerald-500/20">
              <Hospital className="w-8 h-8 text-white" />
            </div>
            <h1 className="text-3xl font-bold text-white">MedAgenda</h1>
            <p className="text-emerald-100/80 text-xs mt-2 max-w-[280px] mx-auto text-center">
              Gestiona y organiza tus citas médicas en un solo lugar, de forma rápida, segura e inteligente.
            </p>
          </div>

          <div ref={formRef} className="bg-card rounded-3xl shadow-2xl border border-border/50 p-8 backdrop-blur-sm" style={{ opacity: 0 }}>
            {isSuccess ? (
              <div className="text-center py-6">
                <div className="w-16 h-16 bg-emerald-500/20 rounded-full flex items-center justify-center mx-auto mb-4">
                  <CheckCircle2 className="w-9 h-9 text-emerald-500" />
                </div>
                <h2 className="text-2xl font-bold text-foreground">¡Cuenta creada!</h2>
                <p className="text-muted-foreground mt-2">Serás redirigido al inicio de sesión...</p>
              </div>
            ) : (
              <>
                <div className="mb-6">
                  <h2 className="text-2xl font-bold text-foreground">{step === 1 ? "Crear Cuenta" : "Información Adicional"}</h2>
                  <p className="text-muted-foreground text-sm mt-1">
                    {step === 1 ? "Paso 1 de 2: Datos de acceso" : "Paso 2 de 2: Perfil médico"}
                  </p>
                </div>

                {error && (
                  <div className="flex items-start gap-2 bg-destructive/10 border border-destructive/20 text-destructive rounded-2xl p-4 text-sm mb-6">
                    <AlertTriangle className="w-5 h-5 flex-shrink-0" />
                    <span>{error}</span>
                  </div>
                )}

                <form onSubmit={handleRegister} className="space-y-4">
                  {step === 1 ? (
                    <>
                      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
                        <div>
                          <label className="block text-sm font-medium text-foreground mb-1.5 flex items-center gap-1.5">
                            <User className="w-3.5 h-3.5 text-emerald-600 dark:text-emerald-400" /> Nombre
                          </label>
                          <input
                            name="nombre"
                            type="text"
                            value={formData.nombre}
                            onChange={(e) => setFieldValue("nombre", e.target.value)}
                            className="w-full px-4 py-3 bg-secondary border border-border rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-emerald-500/30 focus:border-emerald-500 transition-all placeholder:text-muted-foreground/50"
                            placeholder="María"
                            required
                          />
                          {renderFieldError("nombre")}
                        </div>

                        <div>
                          <label className="block text-sm font-medium text-foreground mb-1.5 flex items-center gap-1.5">
                            <User className="w-3.5 h-3.5 text-emerald-600 dark:text-emerald-400" /> Apellido
                          </label>
                          <input
                            name="apellido"
                            type="text"
                            value={formData.apellido}
                            onChange={(e) => setFieldValue("apellido", e.target.value)}
                            className="w-full px-4 py-3 bg-secondary border border-border rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-emerald-500/30 focus:border-emerald-500 transition-all placeholder:text-muted-foreground/50"
                            placeholder="López"
                            required
                          />
                          {renderFieldError("apellido")}
                        </div>
                      </div>

                      <div>
                        <label className="block text-sm font-medium text-foreground mb-1.5 flex items-center gap-1.5">
                          <Mail className="w-3.5 h-3.5 text-emerald-600 dark:text-emerald-400" /> Correo Electrónico
                        </label>
                        <input
                          name="email"
                          type="email"
                          value={formData.email}
                          onChange={(e) => setFieldValue("email", e.target.value)}
                          className="w-full px-4 py-3 bg-secondary border border-border rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-emerald-500/30 focus:border-emerald-500 transition-all placeholder:text-muted-foreground/50"
                          placeholder="correo@ejemplo.com"
                          required
                        />
                        {renderFieldError("email")}
                      </div>

                      <div>
                        <label className="block text-sm font-medium text-foreground mb-1.5 flex items-center gap-1.5">
                          <Lock className="w-3.5 h-3.5 text-emerald-600 dark:text-emerald-400" /> Contraseña
                        </label>
                        <div className="relative">
                          <input
                            name="password"
                            type={showPassword ? "text" : "password"}
                            value={formData.password}
                            onChange={(e) => setFieldValue("password", e.target.value)}
                            className="w-full px-4 pr-12 py-3 bg-secondary border border-border rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-emerald-500/30 focus:border-emerald-500 transition-all placeholder:text-muted-foreground/50"
                            placeholder="••••••••"
                            required
                          />
                          <button
                            type="button"
                            onClick={() => setShowPassword(!showPassword)}
                            className="absolute right-3.5 top-1/2 -translate-y-1/2 text-muted-foreground hover:text-foreground transition-colors"
                          >
                            {showPassword ? <EyeOff className="w-4 h-4" /> : <Eye className="w-4 h-4" />}
                          </button>
                        </div>
                        {renderFieldError("password")}
                        <div className="mt-3 rounded-2xl border border-border/60 bg-secondary/40 p-3 space-y-2">
                          <div className="flex items-center gap-3">
                            <div className="flex-1 h-1.5 rounded-full overflow-hidden bg-white/10">
                              <div
                                className={`h-full transition-all duration-300 ${
                                  passwordStrength <= 2 ? "bg-rose-500" : passwordStrength <= 4 ? "bg-amber-500" : "bg-emerald-500"
                                }`}
                                style={{ width: `${(passwordStrength / passwordChecks.length) * 100}%` }}
                              />
                            </div>
                            <span className="text-xs text-muted-foreground">{passwordStrength}/{passwordChecks.length}</span>
                          </div>
                          <div className="grid grid-cols-1 gap-2 sm:grid-cols-2">
                            {passwordChecks.map((check) => (
                              <ValidationItem key={check.key} valid={check.valid} label={check.label} />
                            ))}
                          </div>
                        </div>
                      </div>

                      <div>
                        <label className="block text-sm font-medium text-foreground mb-1.5 flex items-center gap-1.5">
                          <Lock className="w-3.5 h-3.5 text-emerald-600 dark:text-emerald-400" /> Confirmar Contraseña
                        </label>
                        <div className="relative">
                          <input
                            name="confirmPassword"
                            type={showConfirmPassword ? "text" : "password"}
                            value={formData.confirmPassword}
                            onChange={(e) => setFieldValue("confirmPassword", e.target.value)}
                            className="w-full px-4 pr-12 py-3 bg-secondary border border-border rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-emerald-500/30 focus:border-emerald-500 transition-all placeholder:text-muted-foreground/50"
                            placeholder="••••••••"
                            required
                          />
                          <button
                            type="button"
                            onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                            className="absolute right-3.5 top-1/2 -translate-y-1/2 text-muted-foreground hover:text-foreground transition-colors"
                          >
                            {showConfirmPassword ? <EyeOff className="w-4 h-4" /> : <Eye className="w-4 h-4" />}
                          </button>
                        </div>
                        {renderFieldError("confirmPassword")}
                        {formData.confirmPassword ? (
                          <p className={`mt-1.5 text-xs ${confirmMatches ? "text-emerald-400" : "text-rose-400"}`}>
                            {confirmMatches ? "Las contraseñas coinciden." : "La confirmación no coincide con la contraseña."}
                          </p>
                        ) : null}
                      </div>

                      <button
                        type="button"
                        onClick={handleNextStep}
                        className="w-full bg-gradient-to-r from-emerald-500 to-emerald-500 hover:from-emerald-600 hover:to-emerald-600 text-white font-semibold py-3 rounded-xl transition-all duration-200 flex items-center justify-center gap-2 shadow-lg shadow-emerald-500/25 hover:shadow-xl hover:shadow-emerald-500/30 mt-6"
                      >
                        Continuar <ArrowRight className="w-4 h-4" />
                      </button>
                    </>
                  ) : (
                    <>
                      <div>
                        <label className="block text-sm font-medium text-foreground mb-1.5 flex items-center gap-1.5">
                          <FileText className="w-3.5 h-3.5 text-emerald-600 dark:text-emerald-400" /> Cédula de Identidad
                        </label>
                        <input
                          name="cedula"
                          type="text"
                          value={formData.cedula}
                          onChange={(e) => setFieldValue("cedula", formatCedula(e.target.value))}
                          className="w-full px-4 py-3 bg-secondary border border-border rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-emerald-500/30 focus:border-emerald-500 transition-all placeholder:text-muted-foreground/50"
                          placeholder="000-0000000-0"
                          inputMode="numeric"
                        />
                        {renderFieldError("cedula")}
                      </div>

                      <div>
                        <label className="block text-sm font-medium text-foreground mb-1.5 flex items-center gap-1.5">
                          <Phone className="w-3.5 h-3.5 text-emerald-600 dark:text-emerald-400" /> Teléfono
                        </label>
                        <div className="rounded-xl border border-border bg-secondary px-4 py-3 focus-within:ring-2 focus-within:ring-emerald-500/30 focus-within:border-emerald-500 transition-all">
                          <PhoneInput
                            country="DO"
                            international={false}
                            value={formData.telefono}
                            onChange={(value) => setFieldValue("telefono", normalizePhone(value))}
                            className="medagenda-phone-input w-full bg-transparent text-sm text-foreground outline-none placeholder:text-muted-foreground/50"
                            placeholder="809 555 1101"
                          />
                        </div>
                        {renderFieldError("telefono")}
                      </div>

                      <div>
                        <label className="block text-sm font-medium text-foreground mb-1.5 flex items-center gap-1.5">
                          <Calendar className="w-3.5 h-3.5 text-emerald-600 dark:text-emerald-400" /> Fecha de Nacimiento
                        </label>
                        <input
                          name="fechaNacimiento"
                          type="date"
                          value={formData.fechaNacimiento}
                          onChange={(e) => setFieldValue("fechaNacimiento", e.target.value)}
                          className="w-full px-4 py-3 bg-secondary border border-border rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-emerald-500/30 focus:border-emerald-500 transition-all [color-scheme:dark]"
                        />
                        {renderFieldError("fechaNacimiento")}
                      </div>

                      <div ref={seguroMenuRef}>
                        <label className="block text-sm font-medium text-foreground mb-1.5 flex items-center gap-1.5">
                          <CreditCard className="w-3.5 h-3.5 text-emerald-600 dark:text-emerald-400" /> Tipo de Seguro
                        </label>
                        <button
                          type="button"
                          onClick={() => setShowSeguroOptions((prev) => !prev)}
                          className="w-full px-4 py-3 bg-secondary border border-border rounded-xl text-sm text-left flex items-center justify-between gap-3 focus:outline-none focus:ring-2 focus:ring-emerald-500/30 focus:border-emerald-500 transition-all"
                        >
                          {selectedSeguro ? (
                            <span className="flex items-center gap-3">
                              <span className={`flex h-10 w-10 items-center justify-center rounded-xl border bg-gradient-to-br ${selectedSeguro.accent}`}>
                                <selectedSeguro.icon className="h-4 w-4" />
                              </span>
                              <span className="flex flex-col">
                                <span className="font-medium text-foreground">{selectedSeguro.label}</span>
                                <span className="text-xs text-muted-foreground">Prefijo automático: {selectedSeguro.prefix}</span>
                              </span>
                            </span>
                          ) : (
                            <span className="text-muted-foreground">Seleccionar seguro...</span>
                          )}
                          <ChevronDown className={`h-4 w-4 text-muted-foreground transition-transform ${showSeguroOptions ? "rotate-180" : ""}`} />
                        </button>
                        {renderFieldError("tipoSeguro")}

                        {showSeguroOptions ? (
                          <div className="mt-2 rounded-2xl border border-border bg-card/95 p-2 backdrop-blur-xl shadow-2xl">
                            {seguroOptions.map((option) => (
                              <button
                                key={option.value}
                                type="button"
                                onClick={() => {
                                  setFieldValue("tipoSeguro", option.value);
                                  setFieldValue("numeroSeguro", "");
                                  setShowSeguroOptions(false);
                                }}
                                className="w-full rounded-xl px-3 py-2.5 text-left hover:bg-secondary/70 transition-colors"
                              >
                                <span className="flex items-center gap-3">
                                  <span className={`flex h-10 w-10 items-center justify-center rounded-xl border bg-gradient-to-br ${option.accent}`}>
                                    <option.icon className="h-4 w-4" />
                                  </span>
                                  <span className="flex flex-col">
                                    <span className="font-medium text-foreground">{option.label}</span>
                                    <span className="text-xs text-muted-foreground">{option.prefix} · número de afiliación</span>
                                  </span>
                                </span>
                              </button>
                            ))}
                          </div>
                        ) : null}
                      </div>

                      <div>
                        <label className="block text-sm font-medium text-foreground mb-1.5 flex items-center gap-1.5">
                          <FileText className="w-3.5 h-3.5 text-emerald-600 dark:text-emerald-400" /> Número de Seguro
                        </label>
                        <div className="flex overflow-hidden rounded-xl border border-border bg-secondary focus-within:ring-2 focus-within:ring-emerald-500/30 focus-within:border-emerald-500 transition-all">
                          <div className="flex items-center border-r border-border/70 px-4 text-sm font-semibold text-emerald-400 min-w-[76px] justify-center">
                            {selectedSeguro?.prefix ?? "---"}
                          </div>
                          <input
                            name="numeroSeguro"
                            type="text"
                            value={formData.numeroSeguro}
                            onChange={(e) => setFieldValue("numeroSeguro", e.target.value.replace(/\D/g, "").slice(0, 12))}
                            className="w-full px-4 py-3 bg-transparent text-sm focus:outline-none placeholder:text-muted-foreground/50"
                            placeholder="Solo números"
                            inputMode="numeric"
                          />
                        </div>
                        {selectedSeguro ? (
                          <p className="mt-1.5 text-xs text-muted-foreground">Se guardará como {selectedSeguro.prefix}-{formData.numeroSeguro || "..."}</p>
                        ) : null}
                        {renderFieldError("numeroSeguro")}
                      </div>

                      <div className="flex gap-3 mt-6">
                        <button
                          type="button"
                          onClick={() => setStep(1)}
                          className="flex-1 bg-secondary hover:bg-secondary/80 border border-border text-foreground font-medium py-3 rounded-xl transition-all"
                        >
                          Atrás
                        </button>
                        <button
                          type="submit"
                          disabled={isLoading}
                          className="submit-btn flex-[2] bg-gradient-to-r from-emerald-500 to-emerald-500 hover:from-emerald-600 hover:to-emerald-600 text-white font-semibold py-3 rounded-xl transition-all flex items-center justify-center gap-2 shadow-lg shadow-emerald-500/25 disabled:opacity-70 disabled:cursor-not-allowed"
                        >
                          {isLoading ? (
                            <>
                              <Loader2 className="h-4 w-4 animate-spin" />
                              <span>Creando...</span>
                            </>
                          ) : (
                            "Crear Cuenta"
                          )}
                        </button>
                      </div>
                    </>
                  )}
                </form>

                <div className="mt-6 pt-6 border-t border-border text-center">
                  <p className="text-sm text-muted-foreground">
                    ¿Ya tienes una cuenta?{" "}
                    <Link
                      href="/login"
                      className="text-emerald-600 dark:text-emerald-400 hover:text-emerald-700 dark:hover:text-emerald-300 font-semibold hover:underline transition-colors"
                    >
                      Iniciar Sesión
                    </Link>
                  </p>
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

