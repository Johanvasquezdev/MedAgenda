export enum EstadoCita {
  Pendiente = 'Pendiente',
  Confirmada = 'Confirmada',
  Completada = 'Completada',
  Cancelada = 'Cancelada'
}

export interface CitaDTO {
  id: number;
  pacienteId: number;
  medicoId: number;
  disponibilidadId: number;
  fechaHora: string;
  estado: EstadoCita;
  motivo: string;
  notas: string | null;
  fechaCreacion: string;
}

export interface CreateCitaDTO {
  pacienteId: number;
  medicoId: number;
  disponibilidadId: number;
  motivo: string;
  notas?: string;
}