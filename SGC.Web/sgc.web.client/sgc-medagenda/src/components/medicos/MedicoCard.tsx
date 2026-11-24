"use client";
import { useState } from "react";
import { ArrowRight } from "lucide-react";
import Image from "next/image";
import { MedicoDTO } from "@/types/medico.types";

interface MedicoCardProps {
  medico: MedicoDTO;
  onAgendarClick: (medico: MedicoDTO) => void;
}

function getInitials(name: string) {
  return name.trim().split(/\s+/).filter(Boolean).slice(0, 2).map((part) => part[0]).join("").toUpperCase() || "M";
}

export function MedicoCard({ medico, onAgendarClick }: MedicoCardProps) {
  const [imgSrc, setImgSrc] = useState(medico.foto || "");

  return (
    <article className="group flex h-full flex-col rounded-2xl border border-border bg-card p-5 shadow-sm transition-shadow hover:shadow-[0_14px_34px_-28px_rgba(8,22,16,0.7)] sm:p-6">
      <div className="flex items-center gap-4">
        {imgSrc ? (
          <Image
            src={imgSrc}
            alt={`Foto de ${medico.nombre}`}
            width={64}
            height={64}
            unoptimized
            onError={() => setImgSrc("")}
            className="size-16 shrink-0 rounded-2xl border border-border object-cover"
          />
        ) : (
          <span aria-hidden="true" className="flex size-16 shrink-0 items-center justify-center rounded-2xl bg-accent text-lg font-semibold text-accent-foreground">
            {getInitials(medico.nombre)}
          </span>
        )}
        <div className="min-w-0">
          <h3 className="truncate text-lg font-semibold text-foreground transition-colors group-hover:text-primary">{medico.nombre}</h3>
          <p className="mt-1 truncate text-sm font-medium text-muted-foreground">{medico.especialidadNombre || "Especialista"}</p>
        </div>
      </div>

      <div className="mt-6 flex-1 border-t border-border pt-4">
        <p className="text-sm leading-6 text-muted-foreground">Consulta la disponibilidad y elige el horario que mejor se adapte a tu día.</p>
      </div>

      <button
        type="button"
        onClick={() => onAgendarClick(medico)}
        className="mt-5 inline-flex min-h-11 w-full items-center justify-center gap-2 rounded-xl bg-primary px-4 text-sm font-semibold text-primary-foreground transition hover:brightness-110 focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-ring"
      >
        Agendar cita <ArrowRight className="size-4" aria-hidden="true" />
      </button>
    </article>
  );
}
