import api from '@/lib/api';
import { decodeJwt, getUserIdFromPayload } from '@/lib/jwt';
import { AuthUser } from '@/types/auth.types';
import { UsuarioService } from '@/services/usuario.service';

export interface LoginRequest {
  identifier: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  nombreUsuario: string;
  rol: string;
  expiracion: string;
}

export class AuthService {
  static async login(credenciales: LoginRequest): Promise<LoginResponse> {
    const response = await api.post<LoginResponse>('/api/auth/login', {
      email: credenciales.identifier,
      password: credenciales.password,
    });

    // Guarda token y datos basicos del usuario para el resto de la app.
    if (response.data.token) {
      const payload = decodeJwt(response.data.token);
      let userId = getUserIdFromPayload(payload);
      if (!userId) {
        try {
          const usuarios = await UsuarioService.obtenerTodos(response.data.rol);
          const identifier = credenciales.identifier.trim().toLowerCase();
          const encontrado = usuarios.find(
            (u) => u.email?.toLowerCase() === identifier || u.nombre?.trim().toLowerCase() === identifier
          );
          if (encontrado) userId = encontrado.id;
        } catch {
          // Si falla la resolucion, dejamos el ID en 0.
        }
      }
      const usuario: AuthUser = {
        id: userId,
        nombre: response.data.nombreUsuario || payload?.unique_name || '',
        rol: response.data.rol || payload?.role || '',
        email: payload?.email || credenciales.identifier,
      };

      localStorage.setItem('medagenda_token', response.data.token);
      localStorage.setItem('medagenda_user', JSON.stringify(usuario));

      // Cookies para middleware (no HttpOnly porque es cliente).
      document.cookie = `medagenda_token=${response.data.token}; path=/`;
      document.cookie = `medagenda_role=${usuario.rol}; path=/`;

      window.dispatchEvent(new Event('auth-changed'));
    }

    return response.data;
  }

  static logout() {
    localStorage.removeItem('medagenda_token');
    localStorage.removeItem('medagenda_user');
    document.cookie = 'medagenda_token=; path=/; max-age=0';
    document.cookie = 'medagenda_role=; path=/; max-age=0';
    window.dispatchEvent(new Event('auth-changed'));
    window.location.href = '/';
  }
}
