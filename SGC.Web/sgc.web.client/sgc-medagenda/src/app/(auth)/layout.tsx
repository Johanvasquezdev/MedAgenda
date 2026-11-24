import { ThreeBackground } from "@/components/animations/Threebackground";

export default function AuthLayout({ children }: { children: React.ReactNode }) {
  return (
    <div className="relative isolate min-h-screen w-full overflow-hidden bg-background text-foreground">
      <ThreeBackground />
      <div className="relative z-10">{children}</div>
    </div>
  );
}
