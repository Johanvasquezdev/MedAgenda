"use client";

import { usePathname } from "next/navigation";
import {
  Bell, Building2, Calendar, CreditCard, LayoutDashboard,
  MessageSquare, Settings, ShieldAlert, Stethoscope, Users,
} from "lucide-react";
import { AnimatedSidebar } from "@/components/animations/Animatedsidebar";
import { useAuth } from "@/components/providers/AuthProvider";

export function Sidebar() {
  const pathname = usePathname();
  const isAdmin = pathname.startsWith("/admin");
  const { user } = useAuth();

  const pacienteRoutes = [
    { label: "Dashboard", href: "/paciente/dashboard", icon: LayoutDashboard },
    { label: "Directorio Médico", href: "/paciente/medicos", icon: Stethoscope },
    { label: "Mis Citas", href: "/paciente/citas", icon: Calendar },
    { label: "Pagos y Facturación", href: "/paciente/pagos", icon: CreditCard },
    { label: "Notificaciones", href: "/paciente/notificaciones", icon: Bell },
    { label: "Configuración", href: "/paciente/settings", icon: Settings },
  ];

  const adminRoutes = [
    { label: "Panel General", href: "/admin/dashboard", icon: LayoutDashboard },
    { label: "Citas", href: "/admin/citas", icon: Calendar },
    { label: "Usuarios", href: "/admin/usuarios", icon: Users },
    { label: "Médicos", href: "/admin/medicos", icon: Stethoscope },
    { label: "Disponibilidad", href: "/admin/disponibilidad", icon: Calendar },
    { label: "Especialidades", href: "/admin/especialidades", icon: Stethoscope },
    { label: "Proveedores (ARS)", href: "/admin/proveedores", icon: Building2 },
    { label: "Transacciones", href: "/admin/pagos", icon: CreditCard },
    { label: "Auditoría", href: "/admin/auditoria", icon: ShieldAlert },
    { label: "Configuración", href: "/admin/settings", icon: Settings },
  ];

  const routes = isAdmin ? adminRoutes : pacienteRoutes;

  return (
    <AnimatedSidebar
      navItems={routes}
      title={isAdmin ? "Administración" : "Menú Principal"}
      nombreUsuario={user?.nombre || "Usuario"}
      rol={user?.rol || (isAdmin ? "Administrador" : "Paciente")}
    />
  );
}
