"use client";

import { useState, useRef, useEffect } from "react";
import { Bot, Send, Loader2, X, MessageSquare } from "lucide-react";
import { ChatbotService } from "@/services/chat.service";
import { useAuth } from "@/components/providers/AuthProvider";

export function GlobalChatbotWidget() {
  const { user } = useAuth();
  const [isOpen, setIsOpen] = useState(false);
  const [messages, setMessages] = useState([
    {
      id: "1",
      role: "bot",
      content: "Hola, soy el asistente virtual de MedAgenda. ¿En qué puedo ayudarte hoy?",
    },
  ]);
  const [input, setInput] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const bottomRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (isOpen) {
      setTimeout(() => {
        bottomRef.current?.scrollIntoView({ behavior: "smooth" });
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
        className="fixed bottom-6 right-6 z-50 flex h-14 w-14 items-center justify-center rounded-2xl bg-emerald-600 text-white shadow-xl shadow-emerald-500/20 transition-all hover:scale-110 hover:bg-emerald-500 active:scale-95"
      >
        {isOpen ? <X className="h-6 w-6" /> : <MessageSquare className="h-6 w-6" />}
      </button>

      {/* Chat Window */}
      {isOpen && (
        <div className="fixed bottom-24 right-6 z-50 flex h-[500px] w-[350px] sm:w-[400px] max-w-[calc(100vw-3rem)] flex-col overflow-hidden rounded-3xl border border-border bg-card shadow-2xl shadow-black/50 animate-in slide-in-from-bottom-5 fade-in-50 backdrop-blur-xl">
          {/* Header */}
          <div className="relative overflow-hidden bg-gradient-to-br from-emerald-500/10 to-teal-500/10 p-4 border-b border-border">
            <div className="absolute -right-10 -top-10 h-32 w-32 rounded-full bg-emerald-500/10 blur-2xl" />
            <div className="relative z-10 flex items-center justify-between">
              <div className="flex items-center gap-3">
                <div className="flex h-10 w-10 items-center justify-center rounded-xl bg-emerald-500/10 border border-emerald-500/20">
                  <Bot className="h-5 w-5 text-emerald-500" />
                </div>
                <div>
                  <h3 className="font-black text-sm uppercase tracking-wider text-foreground">Asistente IA</h3>
                  <div className="flex items-center gap-1.5 mt-0.5">
                    <span className="h-1.5 w-1.5 rounded-full bg-emerald-500 animate-pulse" />
                    <p className="text-[10px] text-muted-foreground font-semibold">En línea</p>
                  </div>
                </div>
              </div>
              <button 
                onClick={() => setIsOpen(false)} 
                className="rounded-lg p-2 text-muted-foreground hover:bg-white/5 hover:text-foreground transition-colors"
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
                    ? "bg-emerald-600 text-white font-medium"
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
                    <span className="w-1.5 h-1.5 bg-emerald-500/60 rounded-full animate-bounce" style={{ animationDelay: "0ms" }} />
                    <span className="w-1.5 h-1.5 bg-emerald-500/60 rounded-full animate-bounce" style={{ animationDelay: "150ms" }} />
                    <span className="w-1.5 h-1.5 bg-emerald-500/60 rounded-full animate-bounce" style={{ animationDelay: "300ms" }} />
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
                className="flex h-8 w-8 shrink-0 items-center justify-center rounded-lg bg-emerald-600 text-white transition-all hover:bg-emerald-500 disabled:opacity-50 active:scale-95 shadow-md shadow-emerald-500/20"
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
