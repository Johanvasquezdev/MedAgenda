import api from '@/lib/api';
import { NotificacionResponse } from '@/types/api.types';

export class NotificacionService {
  // Obtiene todas las notificaciones de un usuario.
  static async obtenerPorUsuario(usuarioId: number): Promise<NotificacionResponse[]> {
    if (!usuarioId || usuarioId <= 0) return [];
    const response = await api.get<NotificacionResponse[]>(`/api/notificaciones/usuario/${usuarioId}`);
    return response.data;
  }

  // Obtiene notificaciones no leidas.
  static async obtenerNoLeidas(usuarioId: number): Promise<NotificacionResponse[]> {
    if (!usuarioId || usuarioId <= 0) return [];
    const response = await api.get<NotificacionResponse[]>(`/api/notificaciones/usuario/${usuarioId}/no-leidas`);
    return response.data;
  }
}
