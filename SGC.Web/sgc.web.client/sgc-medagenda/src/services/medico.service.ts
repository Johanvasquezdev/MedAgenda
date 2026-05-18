import api from '@/lib/api';
import { MedicoDTO, CreateMedicoRequest, UpdateMedicoRequest } from '@/types/medico.types';
import { EspecialidadService } from '@/services/especialidad.service';

export class MedicoService {
  static async obtenerTodos(): Promise<MedicoDTO[]> {
    const [medicosRes, especialidades] = await Promise.all([
      api.get<MedicoDTO[]>('/api/medicos'),
      EspecialidadService.obtenerTodas(),
    ]);
    const map = new Map<number, string>();
    especialidades.forEach((e) => map.set(e.id, e.nombre));
    return medicosRes.data.map((m: any) => ({
      ...m,
      especialidadNombre:
        m.especialidadNombre ||
        m.nombreEspecialidad ||
        m.especialidad ||
        (m.especialidadId ? map.get(m.especialidadId) || "" : ""),
    }));
  }

  static async crear(data: CreateMedicoRequest): Promise<MedicoDTO> {
    const response = await api.post<MedicoDTO>('/api/medicos', data);
    return response.data;
  }

  static async actualizar(id: number, data: UpdateMedicoRequest): Promise<void> {
    await api.put(`/api/medicos/${id}`, data);
  }

  static async activar(id: number): Promise<void> {
    await api.put(`/api/medicos/${id}/activar`);
  }

  static async desactivar(id: number): Promise<void> {
    await api.put(`/api/medicos/${id}/desactivar`);
  }
}
