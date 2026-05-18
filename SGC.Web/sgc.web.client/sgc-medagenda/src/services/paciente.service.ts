import api from '@/lib/api';
import { CrearPacienteRequest, UsuarioResponse } from '@/types/api.types';

export class PacienteService {
  // Crea un paciente usando la API real.
  static async crear(request: CrearPacienteRequest): Promise<UsuarioResponse> {
    const response = await api.post<UsuarioResponse>('/api/pacientes', request);
    return response.data;
  }
}
