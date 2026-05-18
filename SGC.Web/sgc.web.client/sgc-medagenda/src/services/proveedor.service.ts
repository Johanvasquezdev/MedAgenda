import api from '@/lib/api';

export interface ProveedorSaludDTO {
  id: number;
  nombre: string;
  tipo?: string;
  telefono?: string;
  email?: string;
  activo: boolean;
}

export class ProveedorSaludService {
  static async obtenerTodos(): Promise<ProveedorSaludDTO[]> {
    const response = await api.get<ProveedorSaludDTO[]>('/api/proveedores');
    return response.data;
  }

  static async crear(data: Partial<ProveedorSaludDTO>): Promise<ProveedorSaludDTO> {
    const response = await api.post<ProveedorSaludDTO>('/api/proveedores', data);
    return response.data;
  }

  static async actualizar(id: number, data: Partial<ProveedorSaludDTO>): Promise<void> {
    await api.put(`/api/proveedores/${id}`, data);
  }
}
