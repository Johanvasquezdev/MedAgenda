import api from '@/lib/api';

export interface EspecialidadDTO {
  id: number;
  nombre: string;
  descripcion: string;
  activo: boolean;
}

export class EspecialidadService {
  static async obtenerTodas(): Promise<EspecialidadDTO[]> {
    const response = await api.get<EspecialidadDTO[]>('/api/especialidades');
    return response.data;
  }

  static async crear(data: Partial<EspecialidadDTO>): Promise<EspecialidadDTO> {
    const response = await api.post<EspecialidadDTO>('/api/especialidades', data);
    return response.data;
  }

  static async actualizar(id: number, data: Partial<EspecialidadDTO>): Promise<void> {
    await api.put(`/api/especialidades/${id}`, data);
  }
}
