"use client";

import Link from "next/link";
import { Bell, Search } from "lucide-react";
import { Sidebar } from "@/components/layout/Sidebar";
import { useAuth } from "@/components/providers/AuthProvider";
import { ThemeToggle } from "@/components/theme-toggle";
import { PageTransition } from "@/components/layout/PageTransition";
import { BrandLockup } from "@/components/brand/BrandLockup";
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
import { Menu, Settings as SettingsIcon, LogOut } from "lucide-react";
import { AuthService } from "@/services/auth.service";
import { useRouter } from "next/navigation";

import { GlobalChatbotWidget } from "@/components/chat/GlobalChatbotWidget";

export default function PacienteLayout({ children }: { children: React.ReactNode }) {
  const { user } = useAuth();
  const router = useRouter();
  const initials = user?.nombre?.trim().charAt(0).toUpperCase() || "P";

  const handleLogout = () => {
    AuthService.logout();
    router.push("/login");
  };

  return (
    <div className="relative min-h-screen overflow-hidden bg-background text-foreground transition-colors duration-300">
      <div className="relative z-10 flex min-h-screen">
        <aside className="hidden w-64 shrink-0 lg:block">
          <div className="sticky top-0 h-dvh">
            <Sidebar />
          </div>
        </aside>

        <div className="flex min-w-0 min-h-screen flex-1 flex-col">
          <header className="sticky top-0 z-30 border-b border-border bg-background">
            <div className="px-4 py-4 md:px-6">
              <div className="mx-auto flex w-full max-w-7xl items-center gap-3 sm:gap-4">
                {/* Mobile navigation and brand */}
                <div className="flex shrink-0 lg:hidden items-center gap-2">
                  <Sheet>
                    <SheetTrigger asChild>
                      <Button variant="outline" size="icon" className="size-11 shrink-0 bg-secondary/50">
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
                  <Link href="/paciente/dashboard" aria-label="MedAgenda, ir al dashboard" className="rounded-md focus-visible:outline-2 focus-visible:outline-offset-4 focus-visible:outline-ring">
                    <BrandLockup markSize="sm" className="text-sm" />
                  </Link>
                </div>

                <div className="relative hidden min-w-0 max-w-2xl flex-1 sm:block">
                  <Search className="absolute left-4 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
                  <input
                    placeholder="Buscar médicos, citas..."
                    className="min-h-11 w-full rounded-xl border border-border bg-secondary pl-11 pr-4 text-sm text-foreground placeholder:text-muted-foreground focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-ring"
                  />
                </div>

                <div className="flex shrink-0 items-center gap-3">
                  <ThemeToggle />

                  <Link
                    href="/paciente/notificaciones"
                    className="relative flex size-11 items-center justify-center rounded-xl border border-border bg-secondary text-muted-foreground transition-colors hover:bg-secondary/80 hover:text-foreground"
                    aria-label="Abrir notificaciones"
                  >
                    <Bell className="h-5 w-5" />
                  </Link>

                  <DropdownMenu>
                    <DropdownMenuTrigger asChild>
                      <button
                        aria-label="Abrir menú de usuario"
                        className="flex size-11 items-center justify-center rounded-full bg-primary text-sm font-semibold text-primary-foreground transition-colors hover:brightness-110 outline-none focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-ring"
                      >
                        {initials}
                      </button>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent align="end" className="w-56">
                      <DropdownMenuLabel>
                        <div className="flex flex-col space-y-1">
                          <p className="text-sm font-medium leading-none">{user?.nombre || "Usuario"}</p>
                          <p className="text-xs leading-none text-muted-foreground">{user?.email || "usuario@ejemplo.com"}</p>
                        </div>
                      </DropdownMenuLabel>
                      <DropdownMenuSeparator />
                      <DropdownMenuItem asChild>
                        <Link href="/paciente/settings" className="cursor-pointer flex items-center w-full">
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

          <main className="flex-1 w-full min-w-0 px-4 py-6 md:px-6 md:py-8">
            <PageTransition>{children}</PageTransition>
          </main>
        </div>

        {/* Global Chatbot Floating Action Button & Window */}
        <GlobalChatbotWidget />
      </div>
    </div>
  );
}
