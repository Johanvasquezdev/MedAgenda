"use client";

import { useEffect, useRef } from "react";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { LogOut } from "lucide-react";
import anime from "animejs";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { BrandLockup } from "@/components/brand/BrandLockup";
import { AuthService } from "@/services/auth.service";

interface NavItem {
  label: string;
  href: string;
  icon: React.ElementType;
}

interface AnimatedSidebarProps {
  navItems: NavItem[];
  title: string;
  nombreUsuario?: string;
  rol?: string;
}

export function AnimatedSidebar({
  navItems,
  title,
  nombreUsuario = "Usuario",
  rol = "Paciente",
}: AnimatedSidebarProps) {
  const pathname = usePathname();
  const sidebarRef = useRef<HTMLElement>(null);
  const hasAnimatedRef = useRef(false);

  useEffect(() => {
    if (hasAnimatedRef.current) return;
    hasAnimatedRef.current = true;
    if (window.matchMedia("(prefers-reduced-motion: reduce)").matches) return;

    anime({ targets: sidebarRef.current, translateX: [-16, 0], opacity: [0, 1], duration: 420, easing: "easeOutExpo" });
    anime({ targets: ".nav-item", translateX: [-8, 0], opacity: [0, 1], duration: 360, delay: anime.stagger(35, { start: 120 }), easing: "easeOutExpo" });
  }, []);

  const handleLogout = () => {
    if (window.matchMedia("(prefers-reduced-motion: reduce)").matches) {
      AuthService.logout();
      return;
    }
    anime({
      targets: sidebarRef.current,
      translateX: [0, -16],
      opacity: [1, 0],
      duration: 260,
      easing: "easeInExpo",
      complete: () => AuthService.logout(),
    });
  };

  return (
    <aside
      ref={sidebarRef}
      className="fixed left-0 top-0 z-30 flex h-dvh w-64 flex-col border-r border-sidebar-border bg-sidebar text-sidebar-foreground shadow-[8px_0_32px_-28px_rgba(8,22,16,0.75)]"
    >
      <div className="flex min-h-20 items-center border-b border-sidebar-border px-5">
        <BrandLockup markSize="sm" className="text-lg" />
      </div>

      <div className="border-b border-sidebar-border px-5 py-4">
        <p className="text-xs font-semibold uppercase tracking-[0.12em] text-muted-foreground">{title}</p>
      </div>

      <nav aria-label={title} className="flex-1 space-y-1 overflow-y-auto p-3">
        {navItems.map((item) => {
          const active = pathname === item.href || pathname.startsWith(`${item.href}/`);
          const Icon = item.icon;

          return (
            <div key={item.href} className="nav-item">
              <Link
                href={item.href}
                aria-current={active ? "page" : undefined}
                className={`flex min-h-11 items-center gap-3 rounded-xl px-3 text-sm font-medium transition-colors focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-ring ${
                  active
                    ? "bg-sidebar-accent text-sidebar-primary"
                    : "text-muted-foreground hover:bg-sidebar-accent hover:text-foreground"
                }`}
              >
                <span className={`flex size-8 items-center justify-center rounded-lg ${active ? "bg-sidebar-primary text-sidebar-primary-foreground" : "text-muted-foreground"}`}>
                  <Icon className="size-4" aria-hidden="true" />
                </span>
                <span className="min-w-0 flex-1 truncate">{item.label}</span>
                {active ? <span aria-hidden="true" className="size-1.5 rounded-full bg-sidebar-primary" /> : null}
              </Link>
            </div>
          );
        })}
      </nav>

      <div className="border-t border-sidebar-border p-3">
        <div className="flex items-center gap-3 rounded-xl px-2 py-2">
          <Avatar className="size-9 border border-sidebar-border">
            <AvatarFallback className="bg-sidebar-accent font-semibold text-sidebar-primary">
              {nombreUsuario.charAt(0).toUpperCase()}
            </AvatarFallback>
          </Avatar>
          <div className="min-w-0 flex-1">
            <p className="truncate text-sm font-medium">{nombreUsuario}</p>
            <p className="truncate text-xs text-muted-foreground">{rol}</p>
          </div>
          <button
            type="button"
            onClick={handleLogout}
            aria-label="Cerrar sesión"
            title="Cerrar sesión"
            className="flex size-11 shrink-0 items-center justify-center rounded-lg text-muted-foreground transition-colors hover:bg-destructive/10 hover:text-destructive focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-ring"
          >
            <LogOut className="size-4" aria-hidden="true" />
          </button>
        </div>
      </div>
    </aside>
  );
}
