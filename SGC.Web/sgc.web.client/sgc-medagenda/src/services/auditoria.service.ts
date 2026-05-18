import api from '@/lib/api';
import { AuditoriaResponse } from '@/types/api.types';

export class AuditoriaService {
  static async obtenerRegistros(entidad?: string, usuarioId?: number): Promise<AuditoriaResponse[]> {
    const params = new URLSearchParams();
    if (entidad) params.append('entidad', entidad);
    if (usuarioId) params.append('usuarioId', usuarioId.toString());
    const response = await api.get<AuditoriaResponse[]>(`/api/auditoria?${params.toString()}`);
    return response.data;
  }
}