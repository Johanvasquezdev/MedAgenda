"use client";
import { useState } from "react";
import { Star, MapPin, Clock, ArrowRight } from "lucide-react";
import Image from "next/image";
import { MedicoDTO } from "@/types/medico.types";

interface MedicoCardProps {
  medico: MedicoDTO;
  onAgendarClick: (medico: MedicoDTO) => void;
}

export function MedicoCard({ medico, onAgendarClick }: MedicoCardProps) {
  const fallbackUrl = `https://ui-avatars.com/api/?name=${encodeURIComponent(medico.nombre)}&background=10b981&color=fff`;
  const [imgSrc, setImgSrc] = useState(medico.foto || fallbackUrl);

  return (
    <div className="group bg-card rounded-2xl p-6 shadow-sm hover:shadow-md transition-all border border-border flex flex-col h-full">
      <div className="flex items-start gap-4">
        <Image
          src={imgSrc}
          alt={`Foto de ${medico.nombre}`}
          width={64}
          height={64}
          unoptimized
          onError={() => setImgSrc(fallbackUrl)}
          className="w-16 h-16 rounded-full object-cover border-2 border-emerald-200 dark:border-emerald-900"
        />
        <div>
          <h3 className="font-semibold text-lg text-foreground group-hover:text-emerald-500 transition-colors">
            {medico.nombre}
          </h3>
          <p className="text-emerald-600 dark:text-emerald-400 text-sm font-medium">
            {medico.especialidadNombre || "Especialista"}
          </p>
          <div className="flex items-center gap-1 text-muted-foreground mt-1 text-sm">
            <Star className="w-4 h-4 fill-amber-400 text-amber-400" />
            <span>4.9 (120 reseñas)</span>
          </div>
        </div>
      </div>
      
      <div className="mt-6 pt-4 border-t border-border flex-1 space-y-2">
        <div className="flex items-center gap-2 text-muted-foreground text-sm">
          <MapPin className="w-4 h-4 text-muted-foreground" />
          Centro Médico Principal
        </div>
        <div className="flex items-center gap-2 text-muted-foreground text-sm">
          <Clock className="w-4 h-4 text-muted-foreground" />
          Disponible mañana
        </div>
      </div>

      <div className="mt-6 flex gap-3">
        <button 
          onClick={() => onAgendarClick(medico)}
          className="flex-1 bg-emerald-600 hover:bg-emerald-700 text-white py-2.5 rounded-xl font-medium transition-colors shadow-sm shadow-emerald-500/20 flex justify-center items-center gap-2"
        >
          Agendar <ArrowRight className="w-4 h-4" />
        </button>
        <button className="px-4 py-2.5 rounded-xl font-medium text-foreground bg-secondary hover:bg-secondary/80 transition-colors">
          Perfil
        </button>
      </div>
    </div>
  );
}
