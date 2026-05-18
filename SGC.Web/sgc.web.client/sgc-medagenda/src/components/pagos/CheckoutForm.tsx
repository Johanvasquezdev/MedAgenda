"use client";
import { useState } from "react";
import {
  PaymentElement,
  useStripe,
  useElements,
} from "@stripe/react-stripe-js";
import { Loader2, ShieldCheck } from "lucide-react";
import { PagoService } from "@/services/pago.service";

export function CheckoutForm({
  clientSecret,
  citaId,
}: {
  clientSecret: string;
  citaId: number;
}) {
  const stripe = useStripe();
  const elements = useElements();
  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [isSuccess, setIsSuccess] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!stripe || !elements) return;
    setIsLoading(true);
    setError(null);

    const { error: submitError, paymentIntent } = await stripe.confirmPayment({
      elements,
      confirmParams: {
        return_url: `${window.location.origin}/paciente/pagos?success=true`,
      },
      redirect: "if_required",
    });

    if (submitError) {
      setError(submitError.message || "Error al pagar.");
      setIsLoading(false);
      return;
    }

    if (paymentIntent?.status === "succeeded") {
      try {
        await PagoService.confirmarPago(paymentIntent.id);
        setIsSuccess(true);
      } catch (e) {
        setError("Error al registrar en backend.");
      }
    }
    setIsLoading(false);
  };

  if (isSuccess)
    return (
      <div className="text-center py-8">
        <ShieldCheck className="w-16 h-16 text-teal-500 mx-auto" />
        <h3>¡Pago Exitoso!</h3>
      </div>
    );

  return (
    <form
      onSubmit={handleSubmit}
      className="bg-white p-6 rounded-2xl border max-w-md mx-auto"
    >
      <PaymentElement options={{ layout: "tabs" }} />
      {error && <div className="mt-4 text-red-500 text-sm">{error}</div>}
      <button
        disabled={isLoading || !stripe}
        className="mt-6 w-full bg-teal-600 text-white py-3 rounded-xl"
      >
        {isLoading ? <Loader2 className="animate-spin" /> : "Pagar Ahora"}
      </button>
    </form>
  );
}
