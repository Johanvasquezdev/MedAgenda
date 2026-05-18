import api from '@/lib/api';
import { DisponibilidadDTO, CreateDisponibilidadRequest } from '@/types/api.types';

export class DisponibilidadService {
  static async obtenerPorMedico(medicoId: number, fecha?: string): Promise<DisponibilidadDTO[]> {
    if (!medicoId || medicoId <= 0) return [];
    const params = fecha ? `?fecha=${fecha}` : '';
    const response = await api.get<DisponibilidadDTO[]>(`/api/disponibilidad/medico/${medicoId}${params}`);
    return response.data;
  }

  static async crear(data: CreateDisponibilidadRequest): Promise<DisponibilidadDTO> {
    const response = await api.post<DisponibilidadDTO>('/api/disponibilidad', data);
    return response.data;
  }
}
