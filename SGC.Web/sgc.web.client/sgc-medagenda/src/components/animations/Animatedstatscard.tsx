"use client";
import { useEffect, useRef } from "react";
import { LucideIcon } from "lucide-react";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { cn } from "@/lib/utils";
import anime from "animejs";

interface AnimatedStatsCardProps {
  title: string;
  value: number;
  prefix?: string;
  suffix?: string;
  description?: string;
  icon: LucideIcon;
  trend?: { value: number; isPositive: boolean };
  className?: string;
  iconClassName?: string;
  delay?: number; // stagger delay in ms
  variant?: "emerald" | "indigo" | "purple" | "cyan" | "rose" | "amber";
}

const variantStyles = {
  emerald: {
    bg: "bg-emerald-500/10 dark:bg-emerald-500/20",
    text: "text-emerald-600 dark:text-emerald-400",
    icon: "text-emerald-600 dark:text-emerald-400"
  },
  indigo: {
    bg: "bg-indigo-500/10 dark:bg-indigo-500/20",
    text: "text-indigo-600 dark:text-indigo-400",
    icon: "text-indigo-600 dark:text-indigo-400"
  },
  purple: {
    bg: "bg-purple-500/10 dark:bg-purple-500/20",
    text: "text-purple-600 dark:text-purple-400",
    icon: "text-purple-600 dark:text-purple-400"
  },
  cyan: {
    bg: "bg-cyan-500/10 dark:bg-cyan-500/20",
    text: "text-cyan-600 dark:text-cyan-400",
    icon: "text-cyan-600 dark:text-cyan-400"
  },
  rose: {
    bg: "bg-rose-500/10 dark:bg-rose-500/20",
    text: "text-rose-600 dark:text-rose-400",
    icon: "text-rose-600 dark:text-rose-400"
  },
  amber: {
    bg: "bg-amber-500/10 dark:bg-amber-500/20",
    text: "text-amber-600 dark:text-amber-400",
    icon: "text-amber-600 dark:text-amber-400"
  }
};

export function AnimatedStatsCard({
  title,
  value,
  prefix = "",
  suffix = "",
  description,
  icon: Icon,
  trend,
  className,
  iconClassName,
  delay = 0,
  variant = "emerald",
}: AnimatedStatsCardProps) {
  const numberRef = useRef<HTMLSpanElement>(null);
  const cardRef = useRef<HTMLDivElement>(null);

  const styles = variantStyles[variant];

  useEffect(() => {
    // Card entrance animation
    anime({
      targets: cardRef.current,
      translateY: [30, 0],
      opacity: [1, 1], // Changed from [0, 1] to avoid flash if used with Framer Motion or other loaders
      duration: 700,
      delay: delay,
      easing: "easeOutExpo",
    });

    // Number count up animation
    const obj = { val: 0 };
    anime({
      targets: obj,
      val: value,
      round: 1,
      duration: 2000,
      delay: delay + 200,
      easing: "easeOutExpo",
      update: () => {
        if (numberRef.current) {
          numberRef.current.textContent =
            prefix + obj.val.toLocaleString() + suffix;
        }
      },
    });
  }, [value, delay, prefix, suffix]);

  return (
    <div ref={cardRef}>
      <Card
        className={cn(
          "transition-all duration-300 hover:shadow-xl hover:-translate-y-1.5 group bg-card border-border/80 rounded-2xl shadow-sm",
          className
        )}
      >
        <CardHeader className="flex flex-row items-center justify-between pb-2">
          <CardTitle className="text-xs font-bold uppercase tracking-wider text-muted-foreground/80">
            {title}
          </CardTitle>
          <div
            className={cn(
              "flex items-center justify-center size-11 rounded-xl shadow-inner",
              styles.bg,
              "group-hover:scale-110 group-hover:rotate-12 transition-all duration-500",
              iconClassName
            )}
          >
            <Icon className={cn("size-5", styles.icon)} />
          </div>
        </CardHeader>
        <CardContent>
          <div className="text-3xl font-black tracking-tight text-foreground">
            <span ref={numberRef}>
              {prefix}0{suffix}
            </span>
          </div>
          {(description || trend) && (
            <div className="flex items-center gap-2 mt-2">
              {trend && (
                <div
                  className={cn(
                    "flex items-center gap-0.5 px-2 py-0.5 rounded-full text-[10px] font-bold",
                    trend.isPositive
                      ? "bg-emerald-500/10 text-emerald-600 dark:text-emerald-400"
                      : "bg-red-500/10 text-red-600 dark:text-red-400"
                  )}
                >
                  {trend.isPositive ? "▲" : "▼"} {Math.abs(trend.value)}%
                </div>
              )}
              {description && (
                <p className="text-xs text-muted-foreground font-medium">{description}</p>
              )}
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}