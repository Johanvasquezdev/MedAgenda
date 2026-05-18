import api from '@/lib/api';
import { CrearPagoRequest, PagoResponse } from '@/types/api.types';

export class PagoService {
  static async crearIntento(request: CrearPagoRequest): Promise<string> {
    const response = await api.post<{ clientSecret: string }>('/api/pagos/crear-intento', request);
    return response.data.clientSecret;
  }
  static async confirmarPago(stripePaymentIntentId: string): Promise<boolean> {
    const response = await api.post<{ confirmado: boolean }>('/api/pagos/confirmar', `"${stripePaymentIntentId}"`, { headers: { 'Content-Type': 'application/json' } });
    return response.data.confirmado;
  }
  static async obtenerPorPaciente(pacienteId: number): Promise<PagoResponse[]> {
    if (!pacienteId || pacienteId <= 0) return [];
    const response = await api.get<PagoResponse[]>(`/api/pagos/paciente/${pacienteId}`);
    return response.data;
  }

  static async obtenerPorCita(citaId: number): Promise<PagoResponse | null> {
    try {
      const response = await api.get<PagoResponse>(`/api/pagos/cita/${citaId}`);
      return response.data;
    } catch {
      return null;
    }
  }

  static async reembolsar(id: number): Promise<boolean> {
    const response = await api.post<{ reembolsado: boolean }>(`/api/pagos/reembolsar/${id}`);
    return response.data.reembolsado;
  }
}
