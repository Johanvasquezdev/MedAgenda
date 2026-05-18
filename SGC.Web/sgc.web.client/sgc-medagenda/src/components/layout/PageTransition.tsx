"use client";

import { usePathname } from "next/navigation";
import type { ReactNode } from "react";

type PageTransitionProps = {
  children: ReactNode;
};

export function PageTransition({ children }: PageTransitionProps) {
  const pathname = usePathname();

  return (
    <div
      key={pathname}
      className="animate-in fade-in slide-in-from-bottom-2 duration-300 ease-out motion-reduce:animate-none"
    >
      {children}
    </div>
  );
}
