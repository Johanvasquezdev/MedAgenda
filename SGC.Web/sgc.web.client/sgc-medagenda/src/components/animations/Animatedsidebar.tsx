"use client";

import { useEffect, useRef } from "react";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { LogOut, Stethoscope } from "lucide-react";
import anime from "animejs";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
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
  const isAdmin = pathname?.includes("/admin");

  const themeBg = isAdmin ? "bg-purple-500" : "bg-emerald-500";
  const themeShadow = isAdmin ? "shadow-purple-500/25" : "shadow-emerald-500/25";
  const themeGradFrom = isAdmin ? "from-purple-600" : "from-emerald-600";
  const themeGradTo = isAdmin ? "to-purple-400" : "to-emerald-400";
  const themeGradDarkFrom = isAdmin ? "dark:from-purple-400" : "dark:from-emerald-400";
  const themeGradDarkTo = isAdmin ? "dark:to-purple-200" : "dark:to-emerald-200";
  const themeActiveBg = isAdmin ? "bg-purple-500/10" : "bg-emerald-500/10";
  const themeActiveText = isAdmin ? "text-purple-600 dark:text-purple-400" : "text-emerald-600 dark:text-emerald-400";
  const themeActiveBorder = isAdmin ? "border-purple-500/20" : "border-emerald-500/20";
  const themeActiveShadow = isAdmin ? "shadow-purple-500/30" : "shadow-emerald-500/30";

  useEffect(() => {
    if (hasAnimatedRef.current) return;
    hasAnimatedRef.current = true;

    anime({
      targets: sidebarRef.current,
      translateX: [-260, 0],
      opacity: [0, 1],
      duration: 600,
      easing: "easeOutExpo",
    });

    anime({
      targets: ".nav-item",
      translateX: [-20, 0],
      opacity: [0, 1],
      duration: 500,
      delay: anime.stagger(60, { start: 300 }),
      easing: "easeOutExpo",
    });
  }, []);

  const handleLogout = () => {
    anime({
      targets: sidebarRef.current,
      translateX: [0, -260],
      opacity: [1, 0],
      duration: 400,
      easing: "easeInExpo",
      complete: () => {
        AuthService.logout();
      },
    });
  };

  const handleNavHover = (element: HTMLElement, entering: boolean) => {
    anime({
      targets: element,
      translateX: entering ? 4 : 0,
      duration: 200,
      easing: "easeOutQuad",
    });
  };

  return (
    <aside
      ref={sidebarRef}
      className="fixed left-0 top-0 z-10 flex h-full w-64 flex-col border-r border-border/60 bg-card/70 backdrop-blur-sm"
      style={{ opacity: 0 }}
    >
      <div className="flex h-16 items-center gap-2 border-b border-border/50 px-6">
        <div className={`flex size-8 items-center justify-center rounded-lg ${themeBg} shadow-lg ${themeShadow}`}>
          <Stethoscope className="size-5 text-white" />
        </div>
        <span
          className={`bg-gradient-to-r ${themeGradFrom} ${themeGradTo} ${themeGradDarkFrom} ${themeGradDarkTo} bg-clip-text text-xl font-bold text-transparent`}
        >
          MedAgenda
        </span>
      </div>

      <div className="border-b border-border/50 px-6 py-3">
        <p className="text-xs font-medium uppercase tracking-wider text-muted-foreground">{title}</p>
      </div>

      <nav className="flex-1 space-y-1 overflow-y-auto p-4">
        {navItems.map((item) => {
          const active = pathname === item.href;
          const Icon = item.icon;

          return (
            <div key={item.href} className="nav-item" style={{ opacity: 0 }}>
              <Link
                href={item.href}
                onMouseEnter={(e) => handleNavHover(e.currentTarget as HTMLElement, true)}
                onMouseLeave={(e) => handleNavHover(e.currentTarget as HTMLElement, false)}
                className={`flex items-center gap-3 rounded-xl px-3 py-2.5 text-sm font-medium transition-colors duration-200 ${
                  active
                    ? `${themeActiveBg} ${themeActiveText} border ${themeActiveBorder}`
                    : "text-muted-foreground hover:bg-secondary hover:text-foreground"
                }`}
              >
                <div
                  className={`flex size-7 items-center justify-center rounded-lg transition-all duration-200 ${
                    active ? `${themeBg} shadow-sm ${themeActiveShadow}` : "bg-secondary"
                  }`}
                >
                  <Icon className={`size-4 ${active ? "text-white" : "text-muted-foreground"}`} />
                </div>
                {item.label}
                {active ? <div className={`ml-auto h-1.5 w-1.5 rounded-full ${themeBg}`} /> : null}
              </Link>
            </div>
          );
        })}
      </nav>

      <div className="border-t p-4">
        <div className="group flex items-center gap-3 rounded-xl px-2 py-2 transition-colors duration-200 hover:bg-secondary">
          <Avatar className={`size-9 border ${themeActiveBorder}`}>
            <AvatarFallback className={`${themeBg} text-white font-semibold`}>
              {nombreUsuario.charAt(0).toUpperCase()}
            </AvatarFallback>
          </Avatar>
          <div className="min-w-0 flex-1">
            <p className="truncate text-sm font-medium">{nombreUsuario}</p>
            <p className="truncate text-xs text-muted-foreground">{rol}</p>
          </div>
          <button
            onClick={handleLogout}
            className="rounded-lg p-1.5 opacity-0 transition-opacity duration-200 hover:bg-destructive/10 hover:text-destructive group-hover:opacity-100"
          >
            <LogOut className="size-4" />
          </button>
        </div>
      </div>
    </aside>
  );
}
