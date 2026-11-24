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
      <div className="rounded-2xl border border-border bg-card p-8 text-center">
        <ShieldCheck className="w-16 h-16 text-teal-500 mx-auto" />
        <h3 className="mt-4 text-xl font-semibold text-foreground">¡Pago exitoso!</h3>
      </div>
    );

  return (
    <form
      onSubmit={handleSubmit}
      className="mx-auto max-w-md rounded-2xl border border-border bg-card p-5 text-foreground sm:p-6"
    >
      <PaymentElement options={{ layout: "tabs" }} />
      {error && <div role="alert" className="mt-4 rounded-xl border border-destructive/20 bg-destructive/10 p-3 text-sm text-destructive">{error}</div>}
      <button
        disabled={isLoading || !stripe}
        className="mt-6 min-h-11 w-full rounded-xl bg-primary px-4 font-semibold text-primary-foreground transition hover:brightness-110 disabled:cursor-not-allowed disabled:opacity-60"
      >
        {isLoading ? <Loader2 className="animate-spin" /> : "Pagar Ahora"}
      </button>
    </form>
  );
}
