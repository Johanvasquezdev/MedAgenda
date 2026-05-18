"use client";
import { Banknote, CreditCard, Lock, ShieldCheck, MapPin, User, Mail, AlertCircle } from "lucide-react";
import Link from "next/link";

import { useState, use } from "react";
import { useRouter } from "next/navigation";
import { toast } from "sonner";
import anime from "animejs";

export default function CheckoutPage({ params }: { params: Promise<{ id: string }> }) {
  const resolvedParams = use(params);
  const router = useRouter();
  const [loading, setLoading] = useState(false);

  // Form states
  const [nombre, setNombre] = useState("");
  const [email, setEmail] = useState("");
  const [direccion, setDireccion] = useState("");
  const [tarjeta, setTarjeta] = useState("");
  const [expiracion, setExpiracion] = useState("");
  const [cvc, setCvc] = useState("");

  const handleCheckout = (e: React.FormEvent) => {
    e.preventDefault();
    if (!nombre || !tarjeta || !expiracion || !cvc) {
      toast.error("Por favor completa los campos obligatorios de la tarjeta");
      return;
    }
    
    setLoading(true);
    
    // Animate button
    anime({
      targets: '.checkout-btn',
      scale: [1, 0.98, 1],
      duration: 300,
      easing: 'easeInOutQuad'
    });

    // Simulate Stripe payment processing
    setTimeout(() => {
      setLoading(false);
      toast.success("Pago completado exitosamente");
      router.push("/paciente/pagos");
    }, 2000);
  };

  // Format card number with spaces
  const handleCardChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const val = e.target.value.replace(/\D/g, "");
    const formatted = val.match(/.{1,4}/g)?.join(" ") || val;
    if (formatted && formatted.length <= 19) setTarjeta(formatted);
  };

  // Format expiration date MM/YY
  const handleExpChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const val = e.target.value.replace(/\D/g, "");
    if (val.length >= 2) {
      setExpiracion(`${val.slice(0, 2)}/${val.slice(2, 4)}`);
    } else {
      setExpiracion(val);
    }
  };

  return (
    <div className={`page-content p-6 lg:p-10 animate-in`}>
      <div className="max-w-5xl mx-auto">
        <div className="mb-8">
          <h1 className="text-2xl font-bold text-foreground">Completar Pago Seguro</h1>
          <p className="text-muted-foreground mt-1">Finaliza tu transacción para la cita #{resolvedParams.id}</p>
        </div>

        <div className="flex flex-col lg:flex-row gap-8">
          {/* Right Panel - Checkout Form (Main interaction) */}
          <div className="flex-1 order-2 lg:order-1">
            <form onSubmit={handleCheckout} className="space-y-6">
              
              {/* Información Personal */}
              <div className="bg-card border border-border rounded-2xl p-6 shadow-sm">
                <h2 className="text-lg font-semibold text-foreground mb-4">Información de Facturación</h2>
                
                <div className="space-y-4">
                  <div>
                    <label className="block text-sm font-medium text-foreground mb-1.5">Nombre Completo</label>
                    <div className="relative">
                      <User className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
                      <input 
                        type="text" 
                        required
                        value={nombre}
                        onChange={(e) => setNombre(e.target.value)}
                        placeholder="Como aparece en la tarjeta" 
                        className="w-full pl-10 pr-4 py-2.5 bg-secondary border border-border rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-emerald-500/30 focus:border-emerald-500 transition-all placeholder:text-muted-foreground/50"
                      />
                    </div>
                  </div>

                  <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div>
                      <label className="block text-sm font-medium text-foreground mb-1.5">Correo Electrónico</label>
                      <div className="relative">
                        <Mail className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
                        <input 
                          type="email" 
                          required
                          value={email}
                          onChange={(e) => setEmail(e.target.value)}
                          placeholder="tu@email.com" 
                          className="w-full pl-10 pr-4 py-2.5 bg-secondary border border-border rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-emerald-500/30 focus:border-emerald-500 transition-all placeholder:text-muted-foreground/50"
                        />
                      </div>
                    </div>
                    <div>
                      <label className="block text-sm font-medium text-foreground mb-1.5">Dirección</label>
                      <div className="relative">
                        <MapPin className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
                        <input 
                          type="text" 
                          value={direccion}
                          onChange={(e) => setDireccion(e.target.value)}
                          placeholder="Calle, Número" 
                          className="w-full pl-10 pr-4 py-2.5 bg-secondary border border-border rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-emerald-500/30 focus:border-emerald-500 transition-all placeholder:text-muted-foreground/50"
                        />
                      </div>
                    </div>
                  </div>
                </div>
              </div>

              {/* Detalles de la Tarjeta */}
              <div className="bg-card border border-border rounded-2xl p-6 shadow-sm">
                <div className="flex items-center justify-between mb-4">
                  <h2 className="text-lg font-semibold text-foreground">Detalles de Pago</h2>
                  <div className="flex gap-2">
                    <div className="w-8 h-5 bg-secondary rounded flex items-center justify-center border border-border">
                      <CreditCard className="w-4 h-4 text-emerald-500" />
                    </div>
                  </div>
                </div>
                
                <div className="space-y-4">
                  <div>
                    <label className="block text-sm font-medium text-foreground mb-1.5">Número de Tarjeta</label>
                    <div className="relative">
                      <CreditCard className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
                      <input 
                        type="text" 
                        required
                        value={tarjeta}
                        onChange={handleCardChange}
                        placeholder="0000 0000 0000 0000" 
                        className="w-full pl-10 pr-4 py-2.5 bg-secondary border border-border rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-emerald-500/30 focus:border-emerald-500 transition-all placeholder:text-muted-foreground/50 font-mono"
                      />
                    </div>
                  </div>

                  <div className="grid grid-cols-2 gap-4">
                    <div>
                      <label className="block text-sm font-medium text-foreground mb-1.5">Expiración</label>
                      <input 
                        type="text" 
                        required
                        value={expiracion}
                        onChange={handleExpChange}
                        placeholder="MM/YY" 
                        maxLength={5}
                        className="w-full px-4 py-2.5 bg-secondary border border-border rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-emerald-500/30 focus:border-emerald-500 transition-all placeholder:text-muted-foreground/50 text-center font-mono"
                      />
                    </div>
                    <div>
                      <label className="block text-sm font-medium text-foreground mb-1.5">CVC</label>
                      <input 
                        type="password" 
                        required
                        value={cvc}
                        onChange={(e) => setCvc(e.target.value.replace(/\D/g, "").slice(0, 4))}
                        placeholder="123" 
                        maxLength={4}
                        className="w-full px-4 py-2.5 bg-secondary border border-border rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-emerald-500/30 focus:border-emerald-500 transition-all placeholder:text-muted-foreground/50 text-center font-mono"
                      />
                    </div>
                  </div>
                </div>
              </div>

              <button
                type="submit"
                disabled={loading}
                className="checkout-btn w-full bg-emerald-600 hover:bg-emerald-500 text-white font-medium py-3.5 rounded-xl transition-all shadow-lg shadow-emerald-500/20 disabled:opacity-70 disabled:cursor-not-allowed flex items-center justify-center gap-2"
              >
                {loading ? (
                  <div className="w-5 h-5 border-2 border-white/30 border-t-white rounded-full animate-spin" />
                ) : (
                  <>
                    <Lock className="w-4 h-4" />
                    Pagar DOP 1,000.00
                  </>
                )}
              </button>
            </form>
          </div>

          {/* Left Panel - Order Summary */}
          <div className="w-full lg:w-96 order-1 lg:order-2">
            <div className="bg-card border border-border rounded-2xl p-6 shadow-sm sticky top-24">
              <div className="flex items-center gap-3 mb-6">
                <div className="w-10 h-10 rounded-full bg-emerald-500/10 flex items-center justify-center">
                  <Banknote className="w-5 h-5 text-emerald-500" />
                </div>
                <div>
                  <h2 className="text-lg font-semibold text-foreground">Resumen</h2>
                  <p className="text-xs text-muted-foreground">ID de Transacción: #{resolvedParams.id}</p>
                </div>
              </div>

              <div className="space-y-4 mb-6">
                <div className="flex justify-between text-sm">
                  <span className="text-muted-foreground">Consulta Médica</span>
                  <span className="text-foreground font-medium">DOP 1,000.00</span>
                </div>
                <div className="flex justify-between text-sm">
                  <span className="text-muted-foreground">Impuestos (Calculados al final)</span>
                  <span className="text-foreground font-medium">DOP 0.00</span>
                </div>
                <div className="pt-4 border-t border-border flex justify-between">
                  <span className="font-semibold text-foreground">Total a pagar</span>
                  <span className="font-bold text-emerald-500">DOP 1,000.00</span>
                </div>
              </div>

              <div className="bg-secondary rounded-xl p-4 flex gap-3 text-sm border border-border">
                <ShieldCheck className="w-6 h-6 text-emerald-500 shrink-0" />
                <p className="text-muted-foreground text-xs leading-relaxed">
                  Pagos seguros mediante encriptación TLS de 256 bits. Tus datos no se almacenan en nuestros servidores.
                </p>
              </div>

              <div className="mt-4 flex gap-2 justify-center">
                <AlertCircle className="w-4 h-4 text-muted-foreground" />
                <span className="text-xs text-muted-foreground">Prueba la pasarela generada por MedAgenda</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
