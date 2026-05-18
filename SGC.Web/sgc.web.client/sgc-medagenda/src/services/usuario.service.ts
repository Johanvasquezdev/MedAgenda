import api from '@/lib/api';
import { UsuarioResponse } from '@/types/api.types';

export class UsuarioService {
  static async obtenerTodos(rol?: string): Promise<UsuarioResponse[]> {
    const params = rol ? `?rol=${rol}` : '';
    const response = await api.get<UsuarioResponse[]>(`/api/usuarios${params}`);
    return response.data;
  }
  static async desactivar(id: number): Promise<void> { await api.put(`/api/usuarios/${id}/desactivar`); }
  static async activar(id: number): Promise<void> { await api.put(`/api/usuarios/${id}/activar`); }
}