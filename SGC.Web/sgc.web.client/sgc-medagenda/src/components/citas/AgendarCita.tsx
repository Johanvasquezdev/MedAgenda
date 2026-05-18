"use client";
import { useState } from "react";
import {
  X,
  Loader2,
  CheckCircle2,
} from "lucide-react";
import { MedicoDTO } from "@/types/api.types";
import { CitaService } from "@/services/cita.service";
import { PagoService } from "@/services/pago.service";
import { useAuth } from "@/components/providers/AuthProvider";
import { UsuarioService } from "@/services/usuario.service";
import { DisponibilidadService } from "@/services/disponibilidad.service";
import { toast } from "sonner";
import { useRouter } from "next/navigation";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";

interface Props {
  medico: MedicoDTO | null;
  isOpen: boolean;
  onClose: () => void;
}

export function AgendarCitaModal({ medico, isOpen, onClose }: Props) {
  const { user } = useAuth();
  const [motivo, setMotivo] = useState("");
  const [fechaHora, setFechaHora] = useState("");
  const [notas, setNotas] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [isSuccess, setIsSuccess] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [citaCreadaId, setCitaCreadaId] = useState<number | null>(null);
  const [pagoLoading, setPagoLoading] = useState(false);
  const [pagoCreado, setPagoCreado] = useState(false);
  const [pacienteActualId, setPacienteActualId] = useState<number | null>(null);
  const router = useRouter();
  const fechaHoraId = "agendar-fecha-hora";
  const motivoId = "agendar-motivo";
  const notasId = "agendar-notas";

  if (!isOpen || !medico) return null;

  const toLocalIso = (value: string) => (value.length === 16 ? `${value}:00` : value);
  const toLocalDate = (value: string) => value.split("T")[0];

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    let pacienteId = user?.id || 0;
    if (!pacienteId) {
      try {
        const email = user?.email;
        const rol = user?.rol;
        if (email && rol) {
          const usuarios = await UsuarioService.obtenerTodos(rol);
          const encontrado = usuarios.find(
            (u) => u.email?.toLowerCase() === email.toLowerCase()
          );
          if (encontrado) pacienteId = encontrado.id;
        }
      } catch {
        // Si falla, mostramos el mensaje de sesion.
      }
    }
    if (!pacienteId) {
      setError("Debes iniciar sesion para agendar.");
      toast.error("Inicia sesion para agendar tu cita");
      return;
    }
    if (!fechaHora) {
      setError("Selecciona fecha y hora.");
      return;
    }
    setIsLoading(true);
    setError(null);
    try {
      const fechaParam = toLocalDate(fechaHora);
      const disponibilidades = await DisponibilidadService.obtenerPorMedico(
        medico.id,
        fechaParam
      );
      if (!disponibilidades.length) {
        setError("No hay disponibilidad para ese dia.");
        return;
      }
      const disponibilidadId = disponibilidades[0].id;
      const citaCreada = await CitaService.crearCita({
        pacienteId,
        medicoId: medico.id,
        fechaHora: toLocalIso(fechaHora),
        disponibilidadId,
        motivo,
        notas,
      });
      setPacienteActualId(pacienteId);
      setCitaCreadaId(citaCreada.id);
      setIsSuccess(true);
    } catch (err: any) {
      const mensaje =
        err?.response?.data?.mensaje ||
        err?.response?.data?.message ||
        "Ocurrio un error al agendar la cita.";
      setError(mensaje);
    } finally {
      setIsLoading(false);
    }
  };

  const handleOpenChange = (open: boolean) => {
    if (!open) onClose();
  };

  return (
    <Dialog open={isOpen} onOpenChange={handleOpenChange}>
      <DialogContent className="sm:max-w-md border-border bg-card text-foreground">
        <DialogHeader>
          <DialogTitle className="text-xl font-bold">Agendar con {medico.nombre}</DialogTitle>
        </DialogHeader>

        <div className="py-2">
          {isSuccess ? (
            <div className="text-center py-6">
              <CheckCircle2 className="w-16 h-16 text-emerald-500 mx-auto" />
              <h3 className="text-lg font-semibold text-foreground mt-4">Cita Agendada</h3>
              <p className="text-muted-foreground text-sm mt-2">
                Puedes completar el pago de tu consulta ahora de forma segura.
              </p>
              <div className="mt-8 flex flex-col gap-3">
                <button
                  disabled={pagoLoading || pagoCreado || !citaCreadaId || !pacienteActualId}
                  onClick={async () => {
                    if (!citaCreadaId || !pacienteActualId) return;
                    try {
                      setPagoLoading(true);
                      await PagoService.crearIntento({
                        citaId: citaCreadaId,
                        pacienteId: pacienteActualId,
                        monto: 1000,
                        moneda: "DOP",
                      });
                      setPagoCreado(true);
                      toast.success("Redirigiendo a la pasarela de pagos...");
                      
                      // Redirect to checkout form
                      router.push(`/paciente/checkout/${citaCreadaId}`);
                    } catch (e: any) {
                      toast.error("No se pudo iniciar el proceso de pago.");
                    } finally {
                      setPagoLoading(false);
                    }
                  }}
                  className="w-full bg-emerald-600 text-white py-3 rounded-xl font-semibold hover:bg-emerald-500 transition-colors disabled:opacity-60 flex justify-center"
                >
                  {pagoLoading ? "Procesando..." : "Completar Pago Seguro"}
                </button>
                <button
                  onClick={() => {
                    onClose();
                    router.push("/paciente/pagos");
                  }}
                  className="w-full bg-muted text-foreground py-3 font-semibold rounded-xl hover:bg-secondary border border-border transition-colors"
                >
                  Ver mis pagos
                </button>
              </div>
            </div>
          ) : (
            <form onSubmit={handleSubmit} className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor={fechaHoraId}>Fecha y Hora</Label>
                <Input
                  id={fechaHoraId}
                  type="datetime-local"
                  required
                  value={fechaHora}
                  onChange={(e) => setFechaHora(e.target.value)}
                />
              </div>

              <div className="space-y-2">
                <Label htmlFor={motivoId}>Motivo de la Cita</Label>
                <Input
                  id={motivoId}
                  required
                  placeholder="Ej. Chequeo general, Dolor de cabeza..."
                  value={motivo}
                  onChange={(e) => setMotivo(e.target.value)}
                />
              </div>

              <div className="space-y-2">
                <Label htmlFor={notasId}>Notas Adicionales (Opcional)</Label>
                <textarea
                  id={notasId}
                  rows={3}
                  placeholder="Información relevante para el médico..."
                  className="w-full px-3 py-2 text-sm border border-input bg-background rounded-md shadow-sm placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring"
                  value={notas}
                  onChange={(e) => setNotas(e.target.value)}
                />
              </div>

              {error && <div className="text-destructive font-medium text-sm">{error}</div>}
              
              <button
                disabled={isLoading}
                className="w-full mt-4 bg-emerald-600 text-white py-2.5 rounded-xl font-bold flex justify-center hover:bg-emerald-500 transition-colors disabled:opacity-70"
              >
                {isLoading ? <Loader2 className="animate-spin w-5 h-5" /> : "Confirmar Cita"}
              </button>
            </form>
          )}
        </div>
      </DialogContent>
    </Dialog>
  );
}
