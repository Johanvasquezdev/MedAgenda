export interface JwtPayload {
  nameid?: string;
  sub?: string;
  uid?: string;
  id?: string;
  email?: string;
  role?: string;
  unique_name?: string;
}

// Intenta extraer el ID del usuario desde distintos claims comunes.
export function getUserIdFromPayload(payload: JwtPayload | null): number {
  if (!payload) return 0;
  const extended = payload as Record<string, string | undefined>;
  const raw =
    payload.nameid ||
    payload.sub ||
    payload.uid ||
    payload.id ||
    extended["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] ||
    "";
  const parsed = Number(raw);
  return Number.isFinite(parsed) ? parsed : 0;
}

// Decodifica el payload de un JWT (solo lectura, sin validacion criptografica).
export function decodeJwt(token: string): JwtPayload | null {
  try {
    const parts = token.split(".");
    if (parts.length !== 3) return null;
    const base64 = parts[1].replace(/-/g, "+").replace(/_/g, "/");
    const json = decodeURIComponent(
      atob(base64)
        .split("")
        .map((c) => `%${("00" + c.charCodeAt(0).toString(16)).slice(-2)}`)
        .join("")
    );
    return JSON.parse(json) as JwtPayload;
  } catch {
    return null;
  }
}
