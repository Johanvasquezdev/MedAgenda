"use client";

import { useState, useRef, useEffect } from "react";
import { Send, Loader2, X, MessageSquare } from "lucide-react";
import { ChatbotService } from "@/services/chat.service";
import { useAuth } from "@/components/providers/AuthProvider";

export function GlobalChatbotWidget() {
  const { user } = useAuth();
  const [isOpen, setIsOpen] = useState(false);
  const [messages, setMessages] = useState([
    {
      id: "1",
      role: "bot",
      content: "Hola, soy la guía de MedAgenda. Puedo explicarte cómo usar las citas, el directorio médico, los pagos y otras secciones. No puedo consultar ni cambiar datos de tu cuenta.",
    },
  ]);
  const [input, setInput] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const bottomRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (isOpen) {
      setTimeout(() => {
        bottomRef.current?.scrollIntoView({ behavior: window.matchMedia("(prefers-reduced-motion: reduce)").matches ? "auto" : "smooth" });
      }, 100);
    }
  }, [messages, isOpen]);

  const handleSend = async (e?: React.FormEvent) => {
    e?.preventDefault();
    if (!input.trim() || isLoading) return;

    const messageToSend = input;
    setInput("");
    setMessages(p => [...p, { id: Date.now().toString(), role: "user", content: messageToSend }]);
    setIsLoading(true);

    try {
      const usuarioId = user?.id && user.id > 0 ? user.id : undefined;
      const res = await ChatbotService.enviarMensaje({ mensaje: messageToSend, usuarioId });
      setMessages(p => [...p, { id: Date.now().toString(), role: "bot", content: res.respuesta }]);
    } catch {
      setMessages(p => [...p, { id: Date.now().toString(), role: "bot", content: "Lo siento, hubo un error al conectar. Inténtalo de nuevo." }]);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <>
      {/* Floating Action Button */}
      <button
        onClick={() => setIsOpen(!isOpen)}
        type="button"
        aria-label={isOpen ? "Cerrar ayuda" : "Abrir ayuda y soporte"}
        className="fixed bottom-5 right-5 z-50 flex size-14 items-center justify-center rounded-2xl bg-primary text-primary-foreground shadow-[0_12px_30px_-14px_rgba(8,22,16,0.8)] transition-colors hover:brightness-110 focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-ring"
      >
        {isOpen ? <X className="h-6 w-6" /> : <MessageSquare className="h-6 w-6" />}
      </button>

      {/* Chat Window */}
      {isOpen && (
        <div className="fixed bottom-24 right-4 z-50 flex h-[min(520px,calc(100dvh-7rem))] w-[min(24rem,calc(100vw-2rem))] flex-col overflow-hidden rounded-2xl border border-border bg-card shadow-[0_24px_64px_-28px_rgba(0,0,0,0.7)] animate-in slide-in-from-bottom-3 fade-in-50 sm:right-6">
          {/* Header */}
          <div className="border-b border-border bg-card p-4">
            <div className="flex items-center justify-between">
              <div className="flex items-center gap-3">
                <div className="flex size-10 items-center justify-center rounded-xl bg-accent text-accent-foreground">
                  <MessageSquare className="size-5 text-primary" aria-hidden="true" />
                </div>
                <div>
                  <h3 className="text-sm font-semibold text-foreground">Ayuda de MedAgenda</h3>
                  <div className="flex items-center gap-1.5 mt-0.5">
                    <span className="h-1.5 w-1.5 rounded-full bg-emerald-500 animate-pulse" />
                    <p className="text-[10px] text-muted-foreground font-semibold">Atención en línea</p>
                  </div>
                </div>
              </div>
              <button
                onClick={() => setIsOpen(false)}
                aria-label="Cerrar ayuda"
                className="flex size-11 items-center justify-center rounded-lg text-muted-foreground transition-colors hover:bg-secondary hover:text-foreground"
              >
                <X className="h-5 w-5" />
              </button>
            </div>
          </div>

          {/* Messages */}
          <div className="flex-1 overflow-y-auto p-4 space-y-4 bg-background/50">
            {messages.map(m => (
              <div key={m.id} className={`flex ${m.role === "user" ? "justify-end" : "justify-start"}`}>
                <div className={`max-w-[85%] rounded-2xl px-4 py-3 shadow-sm text-sm ${
                  m.role === "user"
                    ? "bg-primary text-primary-foreground font-medium"
                    : "bg-muted/50 border border-border text-foreground font-medium"
                }`}>
                  {m.content}
                </div>
              </div>
            ))}
            {isLoading && (
              <div className="flex justify-start">
                <div className="bg-muted/50 border border-border rounded-2xl px-4 py-3">
                  <div className="flex items-center gap-1.5">
                    <span className="h-1.5 w-1.5 rounded-full bg-emerald-500/60 motion-safe:animate-pulse" />
                    <span className="h-1.5 w-1.5 rounded-full bg-emerald-500/60 motion-safe:animate-pulse [animation-delay:150ms]" />
                    <span className="h-1.5 w-1.5 rounded-full bg-emerald-500/60 motion-safe:animate-pulse [animation-delay:300ms]" />
                  </div>
                </div>
              </div>
            )}
            <div ref={bottomRef} />
          </div>

          {/* Input */}
          <form onSubmit={handleSend} className="bg-muted/20 border-t border-border p-3">
            <div className="flex items-center gap-2 bg-background border border-border rounded-xl p-1.5 pr-1.5 focus-within:ring-1 focus-within:ring-emerald-500/30">
              <input
                type="text"
                value={input}
                onChange={e => setInput(e.target.value)}
                placeholder="Escribe aquí..."
                className="flex-1 bg-transparent px-3 py-1 text-sm text-foreground placeholder:text-muted-foreground focus:outline-none"
              />
              <button
                type="submit"
                disabled={!input.trim() || isLoading}
                className="flex size-11 shrink-0 items-center justify-center rounded-lg bg-primary text-primary-foreground transition-colors hover:brightness-110 disabled:cursor-not-allowed disabled:opacity-50"
              >
                {isLoading ? <Loader2 className="h-4 w-4 animate-spin" /> : <Send className="h-4 w-4" />}
              </button>
            </div>
          </form>
        </div>
      )}
    </>
  );
}
