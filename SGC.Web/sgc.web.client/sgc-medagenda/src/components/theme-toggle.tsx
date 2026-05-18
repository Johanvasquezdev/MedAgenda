"use client";

import { Moon, Sun } from "lucide-react";
import { useTheme } from "next-themes";
import { useSyncExternalStore } from "react";

export function ThemeToggle() {
  const { resolvedTheme, setTheme } = useTheme();
  const mounted = useSyncExternalStore(
    () => () => undefined,
    () => true,
    () => false
  );

  const isDark = resolvedTheme !== "light";

  const baseClassName =
    "rounded-xl border border-border bg-secondary p-2 text-muted-foreground transition-all duration-200 hover:bg-secondary/80 hover:text-foreground";

  if (!mounted) {
    return (
      <button
        type="button"
        className={baseClassName}
        aria-label="Cambiar tema"
        disabled
      >
        <Sun className="h-5 w-5" />
      </button>
    );
  }

  return (
    <button
      type="button"
      onClick={() => setTheme(isDark ? "light" : "dark")}
      className={baseClassName}
      aria-label="Cambiar tema"
    >
      {isDark ? <Sun className="h-5 w-5" /> : <Moon className="h-5 w-5" />}
    </button>
  );
}
