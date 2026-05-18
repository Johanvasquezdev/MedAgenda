"use client";

import Link from "next/link";
import { Bell, Search } from "lucide-react";
import { Sidebar } from "@/components/layout/Sidebar";
import { useAuth } from "@/components/providers/AuthProvider";
import { ThemeToggle } from "@/components/theme-toggle";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { Sheet, SheetContent, SheetTrigger, SheetTitle, SheetDescription } from "@/components/ui/sheet";
import { Button } from "@/components/ui/button";
import { Menu, Home, Settings as SettingsIcon, LogOut } from "lucide-react";
import { AuthService } from "@/services/auth.service";
import { useRouter } from "next/navigation";

export default function AdminLayout({ children }: { children: React.ReactNode }) {
  const { user } = useAuth();
  const router = useRouter();
  const initials = user?.nombre?.trim().charAt(0).toUpperCase() || "A";

  const handleLogout = () => {
    AuthService.logout();
    router.push("/login");
  };

  return (
    <div className="min-h-screen bg-background text-foreground transition-colors duration-300">
      <div className="flex">
        <aside className="sticky top-0 hidden h-screen w-72 lg:block">
          <Sidebar />
        </aside>

        <div className="flex-1 min-h-screen">
          <header className="sticky top-0 z-30 border-b border-border bg-background/50 backdrop-blur-xl">
            <div className="px-4 py-4 md:px-6">
              <div className="mx-auto flex w-full max-w-4xl items-center gap-4">
                {/* Mobile Menu & Home */}
                <div className="flex shrink-0 lg:hidden items-center gap-2">
                  <Sheet>
                    <SheetTrigger asChild>
                      <Button variant="outline" size="icon" className="shrink-0 bg-secondary/50">
                        <Menu className="h-5 w-5" />
                        <span className="sr-only">Toggle navigation menu</span>
                      </Button>
                    </SheetTrigger>
                    <SheetContent side="left" className="w-72 p-0 border-r-0">
                      <SheetTitle className="sr-only">Menú de Navegación</SheetTitle>
                      <SheetDescription className="sr-only">Navegación principal de MedAgenda</SheetDescription>
                      <Sidebar />
                    </SheetContent>
                  </Sheet>
                  <Button variant="outline" size="icon" className="shrink-0 bg-secondary/50" onClick={() => router.push("/admin/dashboard")}>
                    <Home className="h-5 w-5 text-indigo-500" />
                    <span className="sr-only">Dashboard Home</span>
                  </Button>
                </div>

                <div className="relative min-w-0 max-w-2xl flex-1">
                  <Search className="absolute left-4 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
                  <input
                    placeholder="Buscar usuarios, médicos, citas..."
                    className="w-full rounded-xl border border-border bg-secondary py-2.5 pl-11 pr-4 text-sm text-foreground placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-indigo-500/50 focus:border-transparent"
                  />
                </div>

                <div className="flex shrink-0 items-center gap-3">
                  <ThemeToggle />

                  <Link
                    href="/admin/auditoria"
                    className="relative rounded-xl border border-border bg-secondary p-2 text-muted-foreground transition-all hover:bg-secondary/80 hover:text-foreground"
                    aria-label="Abrir notificaciones"
                  >
                    <Bell className="h-5 w-5" />
                    <span className="absolute -right-1 -top-1 flex h-5 w-5 items-center justify-center rounded-full bg-indigo-500 text-[10px] font-bold text-white">
                      3
                    </span>
                  </Link>

                  <DropdownMenu>
                    <DropdownMenuTrigger asChild>
                      <button
                        aria-label="Abrir menú de usuario"
                        className="flex h-10 w-10 items-center justify-center rounded-full border border-indigo-500/40 bg-indigo-500/20 text-sm font-semibold text-indigo-200 transition-all hover:bg-indigo-500/30 outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2 focus:ring-offset-background"
                      >
                        {initials}
                      </button>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent align="end" className="w-56">
                      <DropdownMenuLabel>
                        <div className="flex flex-col space-y-1">
                          <p className="text-sm font-medium leading-none">{user?.nombre || "Administrador"}</p>
                          <p className="text-xs leading-none text-muted-foreground">{user?.email || "admin@ejemplo.com"}</p>
                        </div>
                      </DropdownMenuLabel>
                      <DropdownMenuSeparator />
                      <DropdownMenuItem asChild>
                        <Link href="/admin/settings" className="cursor-pointer flex items-center w-full">
                          <SettingsIcon className="mr-2 h-4 w-4" />
                          <span>Configuración</span>
                        </Link>
                      </DropdownMenuItem>
                      <DropdownMenuSeparator />
                      <DropdownMenuItem onClick={handleLogout} className="text-destructive focus:bg-destructive/10 cursor-pointer">
                        <LogOut className="mr-2 h-4 w-4" />
                        <span>Cerrar sesión</span>
                      </DropdownMenuItem>
                    </DropdownMenuContent>
                  </DropdownMenu>
                </div>
              </div>
            </div>
          </header>

          <main className="px-6 py-8">{children}</main>
        </div>
      </div>
    </div>
  );
}
