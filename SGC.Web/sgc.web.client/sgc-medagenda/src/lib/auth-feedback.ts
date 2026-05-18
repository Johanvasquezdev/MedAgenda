type ApiErrorPayload = {
  code?: string;
  message?: string;
  mensaje?: string;
};

type ApiLikeError = {
  code?: string;
  response?: {
    status?: number;
    data?: ApiErrorPayload;
  };
};

type AuthFeedbackMode = "login" | "register";

const defaultMessages: Record<AuthFeedbackMode, string> = {
  login: "No pudimos iniciar sesion en este momento. Intenta nuevamente.",
  register: "No pudimos crear tu cuenta en este momento. Intenta nuevamente.",
};

export function getAuthFeedback(error: ApiLikeError | undefined, mode: AuthFeedbackMode) {
  const status = error?.response?.status;
  const message = error?.response?.data?.message || error?.response?.data?.mensaje || "";
  const normalizedMessage = message.toLowerCase();

  if (error?.code === "ERR_NETWORK" || !error?.response) {
    return "No pudimos conectar con MedAgenda. Verifica tu conexion e intenta nuevamente.";
  }

  if (mode === "login") {
    if (status === 401) return "Correo o contrasena incorrectos. Verifica tus credenciales.";
    if (status === 404) return "No encontramos una cuenta asociada a ese correo.";
    if (status === 400 && normalizedMessage.includes("desactivado")) {
      return "Tu cuenta esta desactivada. Contacta al equipo de soporte.";
    }
  }

  if (mode === "register") {
    if (status === 409) return "Ya existe una cuenta con esos datos. Prueba con otro correo.";
    if (status === 400 && (normalizedMessage.includes("email") || normalizedMessage.includes("correo"))) {
      if (normalizedMessage.includes("uso") || normalizedMessage.includes("existe") || normalizedMessage.includes("registrado")) {
        return "Ese correo ya esta registrado. Intenta iniciar sesion o usa otro correo.";
      }
      return message || "Revisa el correo ingresado e intenta de nuevo.";
    }

    if (status === 400 && normalizedMessage.includes("cedula")) {
      return message || "La cedula ingresada no es valida.";
    }

    if (status === 400 && normalizedMessage.includes("telefono")) {
      return message || "El telefono ingresado no es valido.";
    }

    if (status === 400 && normalizedMessage.includes("fecha")) {
      return message || "La fecha de nacimiento no es valida.";
    }
  }

  if (status === 500) {
    return "Ahora mismo el servidor no pudo completar la solicitud. Intenta nuevamente en unos minutos.";
  }

  return message || defaultMessages[mode];
}
