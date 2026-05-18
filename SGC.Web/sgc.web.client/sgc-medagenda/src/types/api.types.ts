export enum RolUsuario { Paciente = 'Paciente', Medico = 'Medico', Administrador = 'Administrador' }
export enum EstadoCita {
  Solicitada = 'Solicitada',
  Confirmada = 'Confirmada',
  EnProgreso = 'EnProgreso',
  Completada = 'Completada',
  Cancelada = 'Cancelada',
  Rechazada = 'Rechazada',
  NoAsistio = 'NoAsistio',
}

export interface UsuarioBase { id: number; nombre: string; email: string; rol: RolUsuario | string; fechaCreacion: string; activo: boolean; }
export interface UsuarioResponse extends UsuarioBase { }

export interface PerfilUsuarioResponse {
  id: number;
  nombre: string;
  email: string;
  rol: RolUsuario | string;
  telefono?: string | null;
}

export interface ActualizarPerfilRequest {
  nombre: string;
  email: string;
  telefono?: string | null;
}

export interface CambiarPasswordRequest {
  passwordActual: string;
  passwordNueva: string;
}

export interface MedicoDTO extends UsuarioBase { especialidadId: number; especialidadNombre?: string; proveedorSaludId: number; exequatur: string; telefonoConsultorio: string; foto: string | null; }

export interface DisponibilidadDTO {
  id: number;
  medicoId: number;
  medicoNombre: string;
  diaSemana: string;
  horaInicio: string;
  horaFin: string;
  duracionCitaMin: number;
  esRecurrente: boolean;
}

export interface CreateDisponibilidadRequest {
  medicoId: number;
  diaSemana: number;
  horaInicio: string;
  horaFin: string;
  duracionCitaMin: number;
  esRecurrente: boolean;
}

export interface CitaDTO { id: number; pacienteId: number; medicoId: number; disponibilidadId?: number | null; fechaHora: string; estado: EstadoCita; motivo?: string | null; notas?: string | null; fechaCreacion: string; }
export interface CreateCitaDTO { pacienteId: number; medicoId: number; disponibilidadId?: number | null; fechaHora: string; motivo?: string; notas?: string; }

export interface ChatRequest { mensaje: string; usuarioId?: number; contexto?: string; }
export interface ChatResponse { respuesta: string; fechaRespuesta: string; }

export interface CrearPagoRequest { citaId: number; pacienteId: number; monto: number; moneda: string; }
export interface PagoResponse { id: number; citaId: number; pacienteId: number; monto: number; moneda: string; estado: string; stripePaymentIntentId?: string; fechaPago?: string; fechaCreacion: string; }

export interface AuditoriaResponse { id: number; usuarioId?: number; entidad: string; accion: string; valorAnterior?: string; valorNuevo?: string; fecha: string; direccionIP?: string; }

export interface NotificacionResponse {
  id: number;
  usuarioId: number;
  citaId?: number | null;
  tipo: string;
  mensaje: string;
  leida: boolean;
  fechaEnvio: string;
}

export interface CrearPacienteRequest {
  nombre: string;
  email: string;
  password: string;
  cedula?: string;
  telefono?: string;
  fechaNacimiento?: string;
  tipoSeguro?: string;
  numeroSeguro?: string;
}
