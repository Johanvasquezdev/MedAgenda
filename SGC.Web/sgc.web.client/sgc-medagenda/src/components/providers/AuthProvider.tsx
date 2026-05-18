"use client";

import { createContext, useCallback, useContext, useEffect, useMemo, useState } from "react";
import { decodeJwt, getUserIdFromPayload } from "@/lib/jwt";
import { AuthUser } from "@/types/auth.types";

interface AuthContextValue {
  user: AuthUser | null;
  token: string | null;
  isAuthenticated: boolean;
  refresh: () => void;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

function normalizeUserId(value: unknown): number {
  const parsed = Number(value);
  return Number.isFinite(parsed) && parsed > 0 ? parsed : 0;
}

function getUserFromStorage(): AuthUser | null {
  // Lee el usuario del storage o lo reconstruye desde el JWT.
  if (typeof window === "undefined") return null;
  const raw = localStorage.getItem("medagenda_user");
  if (raw) {
    try {
      const parsed = JSON.parse(raw) as Partial<AuthUser> & { id?: unknown };
      const storedUserId = normalizeUserId(parsed?.id);
      if (storedUserId) {
        return {
          id: storedUserId,
          nombre: parsed.nombre || "",
          rol: parsed.rol || "",
          email: parsed.email,
        };
      }
      const token = localStorage.getItem("medagenda_token");
      if (!token) return null;
      const payload = decodeJwt(token);
      const userId = getUserIdFromPayload(payload);
      if (!userId) return null;
      return {
        id: userId,
        nombre: parsed.nombre || payload?.unique_name || "",
        rol: parsed.rol || payload?.role || "",
        email: parsed.email || payload?.email,
      };
    } catch {
      return null;
    }
  }
  const token = localStorage.getItem("medagenda_token");
  if (!token) return null;
  const payload = decodeJwt(token);
  const userId = getUserIdFromPayload(payload);
  if (!userId) return null;
  return {
    id: userId,
    nombre: payload.unique_name || "",
    rol: payload.role || "",
    email: payload.email,
  };
}

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<AuthUser | null>(null);
  const [token, setToken] = useState<string | null>(null);

  const refresh = useCallback(() => {
    // Sincroniza token/usuario desde storage en tiempo real.
    if (typeof window === "undefined") return;
    const nextToken = localStorage.getItem("medagenda_token");
    setToken(nextToken);
    setUser(getUserFromStorage());
  }, []);

  useEffect(() => {
    const onStorage = () => refresh();
    const onAuthChanged = () => refresh();
    queueMicrotask(refresh);
    window.addEventListener("storage", onStorage);
    window.addEventListener("auth-changed", onAuthChanged);
    return () => {
      window.removeEventListener("storage", onStorage);
      window.removeEventListener("auth-changed", onAuthChanged);
    };
  }, [refresh]);

  const value = useMemo<AuthContextValue>(
    () => ({
      user,
      token,
      isAuthenticated: Boolean(token),
      refresh,
    }),
    [user, token, refresh]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth debe usarse dentro de AuthProvider");
  return ctx;
}
