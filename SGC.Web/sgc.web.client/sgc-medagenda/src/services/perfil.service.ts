import api from "@/lib/api";
import {
  PerfilUsuarioResponse,
  ActualizarPerfilRequest,
  CambiarPasswordRequest,
} from "@/types/api.types";

export class PerfilService {
  static async obtenerMiPerfil(): Promise<PerfilUsuarioResponse> {
    const response = await api.get<PerfilUsuarioResponse>("/api/perfil/me");
    return response.data;
  }

  static async actualizarMiPerfil(request: ActualizarPerfilRequest): Promise<PerfilUsuarioResponse> {
    const response = await api.put<PerfilUsuarioResponse>("/api/perfil/me", request);
    return response.data;
  }

  static async cambiarMiPassword(request: CambiarPasswordRequest): Promise<void> {
    await api.put("/api/perfil/me/password", request);
  }
}
