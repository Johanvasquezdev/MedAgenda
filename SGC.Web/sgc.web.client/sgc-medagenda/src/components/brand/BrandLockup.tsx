import { cn } from "@/lib/utils";
import { BrandMark } from "./BrandMark";

interface BrandLockupProps {
  className?: string;
  markSize?: "sm" | "md" | "lg";
}

const markSizes = { sm: "size-10", md: "size-12", lg: "size-16" };

export function BrandLockup({ className, markSize = "md" }: BrandLockupProps) {
  return (
    <div className={cn("inline-flex items-center gap-3", className)}>
      <BrandMark className={cn(markSizes[markSize], "text-primary")} />
      <span className="font-bold tracking-[-0.035em] text-foreground">MedAgenda</span>
    </div>
  );
}
