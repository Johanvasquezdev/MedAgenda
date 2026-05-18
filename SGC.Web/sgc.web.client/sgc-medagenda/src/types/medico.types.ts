export type { MedicoDTO } from './api.types';

export interface EspecialidadDTO {
  id: number;
  nombre: string;
  descripcion: string;
  activo: boolean;
}

export interface CreateMedicoRequest {
  nombre: string;
  email: string;
  password: string;
  exequatur?: string;
  especialidadId?: number | null;
  proveedorSaludId?: number | null;
  telefonoConsultorio?: string;
  foto?: string | null;
}

export interface UpdateMedicoRequest {
  nombre: string;
  email: string;
  exequatur?: string;
  especialidadId?: number | null;
  proveedorSaludId?: number | null;
  telefonoConsultorio?: string;
  foto?: string | null;
}
