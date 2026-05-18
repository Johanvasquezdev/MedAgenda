"use client";
import { useEffect, useRef, useState } from "react";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { useRouter } from "next/navigation";
import { Stethoscope, LogOut } from "lucide-react";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { AuthService } from "@/services/auth.service";
import anime from "animejs";

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

  const router = useRouter();
  const sidebarRef = useRef<HTMLElement>(null);
  const navItemsRef = useRef<HTMLDivElement>(null);
  const [mounted, setMounted] = useState(false);

  // ── Sidebar entrance animation ───────────────────────────
  useEffect(() => {
    setMounted(true);

    // Slide in sidebar
    anime({
      targets: sidebarRef.current,
      translateX: [-260, 0],
      opacity: [0, 1],
      duration: 600,
      easing: "easeOutExpo",
    });

    // Stagger nav items
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
    // Animate out before logout
    anime({
      targets: sidebarRef.current,
      translateX: [0, -260],
      opacity: [1, 0],
      duration: 400,
      easing: "easeInExpo",
      complete: () => {
        AuthService.logout();
        router.push("/login");
      },
    });
  };

  const handleNavHover = (el: HTMLElement, entering: boolean) => {
    anime({
      targets: el,
      translateX: entering ? 4 : 0,
      duration: 200,
      easing: "easeOutQuad",
    });
  };

  return (
    <aside
      ref={sidebarRef}
      className="fixed left-0 top-0 h-full w-64 bg-card/70 border-r border-border/60 backdrop-blur-sm
                 flex flex-col z-10"
      style={{ opacity: 0 }}
    >
      {/* Logo */}
      <div className="flex h-16 items-center gap-2 border-b border-border/50 px-6">
        <div className={`flex items-center justify-center size-8 rounded-lg ${themeBg} shadow-lg ${themeShadow}`}>
          <Stethoscope className="size-5 text-white" />
        </div>
        <span className={`text-xl font-bold bg-gradient-to-r ${themeGradFrom} ${themeGradTo} bg-clip-text text-transparent ${themeGradDarkFrom} ${themeGradDarkTo}`}>
          MedAgenda
        </span>
      </div>

      {/* Title */}
      <div className="px-6 py-3 border-b border-border/50">
        <p className="text-xs font-medium text-muted-foreground uppercase
                      tracking-wider">
          {title}
        </p>
      </div>

      {/* Nav */}
      <nav ref={navItemsRef} className="flex-1 p-4 space-y-1 overflow-y-auto">
        {navItems.map((item) => {
          const active = pathname === item.href;
          const Icon = item.icon;

          return (
            <div key={item.href} className="nav-item" style={{ opacity: 0 }}>
              <Link
                href={item.href}
                onMouseEnter={(e) =>
                  handleNavHover(e.currentTarget as HTMLElement, true)
                }
                onMouseLeave={(e) =>
                  handleNavHover(e.currentTarget as HTMLElement, false)
                }
                className={`flex items-center gap-3 px-3 py-2.5 rounded-xl
                            text-sm font-medium transition-colors duration-200
                            ${
                              active
                                ? `${themeActiveBg} ${themeActiveText} border ${themeActiveBorder}`
                                : "text-muted-foreground hover:bg-secondary hover:text-foreground"
                            }`}
              >
                <div
                  className={`flex items-center justify-center size-7 rounded-lg
                               transition-all duration-200
                               ${
                                 active
                                   ? `${themeBg} shadow-sm ${themeActiveShadow}`
                                   : "bg-secondary group-hover:bg-secondary/80"
                               }`}
                >
                  <Icon
                    className={`size-4 ${
                      active ? "text-white" : "text-muted-foreground"
                    }`}
                  />
                </div>
                {item.label}
                {active && (
                  <div className={`ml-auto w-1.5 h-1.5 rounded-full ${themeBg}`} />
                )}
              </Link>
            </div>
          );
        })}
      </nav>

      {/* User footer */}
      <div className="p-4 border-t">
        <div className="flex items-center gap-3 px-2 py-2 rounded-xl
                        hover:bg-secondary transition-colors duration-200 group">
          <Avatar className={`size-9 border ${themeActiveBorder}`}>
            <AvatarFallback className={`${themeBg} text-white font-semibold`}>
              {nombreUsuario.charAt(0).toUpperCase()}
            </AvatarFallback>
          </Avatar>
          <div className="flex-1 min-w-0">
            <p className="text-sm font-medium truncate">{nombreUsuario}</p>
            <p className="text-xs text-muted-foreground truncate">{rol}</p>
          </div>
          <button
            onClick={handleLogout}
            className="opacity-0 group-hover:opacity-100 transition-opacity
                       duration-200 p-1.5 rounded-lg hover:bg-destructive/10
                       hover:text-destructive"
          >
            <LogOut className="size-4" />
          </button>
        </div>
      </div>
    </aside>
  );
}