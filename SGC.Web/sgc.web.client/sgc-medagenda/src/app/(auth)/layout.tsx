export default function AuthLayout({ children }: { children: React.ReactNode }) {
  return (
    <div className="min-h-screen w-full relative overflow-hidden bg-[#030a1f] text-white">
      <div className="pointer-events-none absolute inset-0">
        <div
          className="absolute inset-0"
          suppressHydrationWarning
          style={{ backgroundImage: "linear-gradient(90deg, rgba(3,10,31,0.96) 0%, rgba(3,10,31,0.88) 55%, rgba(7,45,72,0.62) 100%)" }}
        />
        <div
          className="absolute -top-40 right-12 h-[420px] w-[420px] rounded-full opacity-25"
          suppressHydrationWarning
          style={{ backgroundImage: "radial-gradient(circle, #10b981 0%, transparent 72%)" }}
        />
        <div
          className="absolute -bottom-52 left-1/2 h-[760px] w-[760px] -translate-x-1/2 rounded-full opacity-15"
          suppressHydrationWarning
          style={{ backgroundImage: "radial-gradient(circle, #1e3a8a 0%, transparent 66%)" }}
        />
      </div>
      <div className="relative z-10 min-h-screen w-full">{children}</div>
    </div>
  );
}
