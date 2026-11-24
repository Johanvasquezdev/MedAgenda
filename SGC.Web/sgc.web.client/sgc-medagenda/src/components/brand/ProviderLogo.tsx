import { cn } from "@/lib/utils";
import Image from "next/image";
import { getProviderLogo } from "./providerLogos";

interface ProviderLogoProps {
  name: string;
  className?: string;
  size?: "default" | "feature";
}

function initials(name: string) {
  const words = name.trim().split(/\s+/).filter(Boolean);
  return words.slice(0, 2).map((word) => word[0]).join("").toUpperCase() || "?";
}

export function ProviderLogo({ name, className, size = "default" }: ProviderLogoProps) {
  const logo = getProviderLogo(name);
  const featured = size === "feature";
  const logoSrc = featured ? logo?.featureSrc ?? logo?.src : logo?.src;

  return (
    <span className={cn("inline-flex min-w-0 items-center gap-3", className)}>
      {logo ? (
        <span className={cn(
          "flex shrink-0 items-center justify-center",
          featured ? "h-[76px] w-44 sm:h-[84px] sm:w-52" : "size-11 rounded-xl border border-border bg-white p-1.5"
        )}>
          <Image src={logoSrc!} alt={logo.alt} width={featured ? logo.featureWidth : 120} height={featured ? logo.featureHeight : 60} className={cn("max-h-full max-w-full object-contain", featured && "h-auto w-auto")} />
        </span>
      ) : (
        <span aria-hidden="true" className={cn("flex shrink-0 items-center justify-center rounded-xl border border-border bg-muted font-semibold tracking-wide text-muted-foreground", featured ? "size-16 text-base" : "size-11 text-xs")}>
          {initials(name)}
        </span>
      )}
      <span className={cn("truncate font-medium text-foreground", featured ? "text-base" : "text-sm")}>{name}</span>
    </span>
  );
}
