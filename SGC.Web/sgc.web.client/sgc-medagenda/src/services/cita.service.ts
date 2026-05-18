import api from '@/lib/api';
import { CitaDTO, CreateCitaDTO, EstadoCita } from '@/types/api.types';

export class CitaService {
  private static normalizeEstado(cita: CitaDTO): CitaDTO {
    const estado = (cita.estado || "").toString();
    const fecha = new Date(cita.fechaHora);
    const ahora = new Date();

    if (!estado.trim()) {
      return { ...cita, estado: EstadoCita.Solicitada };
    }

    // Una cita futura no debería mostrarse como "NoAsistio".
    if (estado.toLowerCase() === EstadoCita.NoAsistio.toLowerCase() && fecha > ahora) {
      return { ...cita, estado: EstadoCita.Solicitada };
    }

    return cita;
  }

  static async obtenerCitasPorPaciente(pacienteId: number): Promise<CitaDTO[]> {
    if (!pacienteId || pacienteId <= 0) return [];
    const response = await api.get<CitaDTO[]>(`/api/citas/paciente/${pacienteId}`);
    return response.data.map((cita) => this.normalizeEstado(cita));
  }

  static async crearCita(citaData: CreateCitaDTO): Promise<CitaDTO> {
    const response = await api.post<CitaDTO>('/api/citas', citaData);
    return this.normalizeEstado(response.data);
  }
  static async obtenerCitasPorMedico(medicoId: number): Promise<CitaDTO[]> {
    const response = await api.get<CitaDTO[]>(`/api/citas/medico/${medicoId}`);
    return response.data.map((cita) => this.normalizeEstado(cita));
  }
  static async cancelarCita(citaId: number, motivo: string): Promise<void> {
    await api.put(`/api/citas/${citaId}/cancelar`, { motivo });
  }

  static async reprogramarCita(citaId: number, fechaHora: string): Promise<void> {
    await api.put(`/api/citas/${citaId}/reprogramar`, { fechaHora });
  }

  static async confirmarCita(citaId: number): Promise<void> {
    await api.put(`/api/citas/${citaId}/confirmar`);
  }

  static async iniciarConsulta(citaId: number): Promise<void> {
    await api.put(`/api/citas/${citaId}/iniciar-consulta`);
  }

  static async marcarAsistencia(citaId: number, asistio: boolean): Promise<void> {
    await api.put(`/api/citas/${citaId}/asistencia?asistio=${asistio}`);
  }
}
