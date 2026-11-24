import { cn } from "@/lib/utils";
import Image from "next/image";

interface BrandMarkProps {
  className?: string;
  decorative?: boolean;
}

export function BrandMark({ className, decorative = false }: BrandMarkProps) {
  return (
    <span
      className={cn("relative inline-flex shrink-0 items-center justify-center overflow-hidden", className)}
      role={decorative ? undefined : "img"}
      aria-label={decorative ? undefined : "MedAgenda"}
      aria-hidden={decorative ? true : undefined}
    >
      <Image src="/medagenda-mark-generated.webp" alt="" width={512} height={512} className="size-full object-contain" />
    </span>
  );
}
