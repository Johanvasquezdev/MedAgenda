"use client";

import { LucideIcon } from "lucide-react";

interface ChatActionCardProps {
  icon: LucideIcon;
  title: string;
  description: string;
  onClick: () => void;
  accentColor?: string;
}

export function ChatActionCard({
  icon: Icon,
  title,
  description,
  onClick,
  accentColor = "emerald"
}: ChatActionCardProps) {
  const accentClasses = {
    emerald: "from-emerald-500/10 via-emerald-500/5 text-emerald-600 dark:text-emerald-400 border-emerald-500/20 hover:border-emerald-500/40",
    indigo: "from-indigo-500/10 via-indigo-500/5 text-indigo-600 dark:text-indigo-400 border-indigo-500/20 hover:border-indigo-500/40",
  }[accentColor as "emerald" | "indigo"];

  return (
    <button
      onClick={onClick}
      className={`group relative flex items-center gap-4 w-full p-4 rounded-2xl border bg-gradient-to-br to-transparent backdrop-blur-sm transition-all duration-300 hover:scale-[1.01] hover:shadow-lg text-left active:scale-[0.99] ${accentClasses}`}
    >
      <div className={`p-3 rounded-xl bg-background border border-current/20 shadow-inner group-hover:scale-110 transition-transform`}>
        <Icon className="w-5 h-5 transition-colors" />
      </div>
      
      <div className="flex-1 min-w-0">
        <h3 className="font-black text-foreground text-sm uppercase tracking-tight mb-0.5 whitespace-nowrap overflow-hidden text-ellipsis">
          {title}
        </h3>
        <p className="text-muted-foreground text-xs font-medium leading-tight">
          {description}
        </p>
      </div>

      <div className="opacity-0 group-hover:opacity-100 transition-opacity absolute right-4 text-current">
        <svg
          xmlns="http://www.w3.org/2000/svg"
          width="16"
          height="16"
          viewBox="0 0 24 24"
          fill="none"
          stroke="currentColor"
          strokeWidth="3"
          strokeLinecap="round"
          strokeLinejoin="round"
        >
          <path d="M5 12h14" />
          <path d="m12 5 7 7-7 7" />
        </svg>
      </div>
    </button>
  );
}
