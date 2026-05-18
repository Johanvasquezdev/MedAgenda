import axios from 'axios';

const API_URL =
  process.env.NEXT_PUBLIC_API_BASE_URL?.trim() ||
  process.env.NEXT_PUBLIC_API_URL?.trim() ||
  '';

const api = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
  // Opcional: Agregar un timeout predeterminado evita que la UI se quede 
  // cargando infinitamente si el backend falla.
  timeout: 10000, 
});

// Interceptor de Peticiones (Request)
api.interceptors.request.use(
  (config) => {
    if (typeof window !== 'undefined') {
      const token = localStorage.getItem('medagenda_token');
      if (token) {
        config.headers.Authorization = `Bearer ${token}`;
      }
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Interceptor de Respuestas (Response)
api.interceptors.response.use(
  (response) => response,
  (error) => {
    // Manejo de sesión expirada / no autorizada
    if (error.response?.status === 401) {
      if (typeof window !== 'undefined') {
        // 1. Limpiar los datos de sesión comprometidos/expirados
        localStorage.removeItem('medagenda_token');
        localStorage.removeItem('medagenda_user');

        // 2. Redirigir al usuario al login solo si no está ya en la página de login.
        // Esto previene un bucle de redirecciones infinitas.
        const loginPath = '/login';
        if (window.location.pathname !== loginPath) {
          window.location.href = loginPath;
        }
      }
    }
    
    return Promise.reject(error);
  }
);

export default api;
