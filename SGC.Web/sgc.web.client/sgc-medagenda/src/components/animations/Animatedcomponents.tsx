"use client";
import { useEffect, useRef } from "react";
import anime from "animejs";

// ── Hook: useStaggerAnimation ─────────────────────────────────
// Use this on any list of cards to animate them in with stagger
export function useStaggerAnimation(selector: string, deps: any[] = []) {
  useEffect(() => {
    anime({
      targets: selector,
      translateY: [40, 0],
      opacity: [0, 1],
      duration: 700,
      delay: anime.stagger(120, { start: 100 }),
      easing: "easeOutExpo",
    });
  }, deps);
}

// ── Hook: usePageTransition ───────────────────────────────────
// Animates the entire page content on mount
export function usePageTransition() {
  useEffect(() => {
    anime({
      targets: ".page-content",
      translateY: [20, 0],
      opacity: [0, 1],
      duration: 600,
      easing: "easeOutExpo",
    });
  }, []);
}

// ── Component: AnimatedList ───────────────────────────────────
// Wrap any list of items to animate them in with stagger
interface AnimatedListProps {
  children: React.ReactNode;
  className?: string;
  staggerSelector?: string;
}

export function AnimatedList({
  children,
  className = "",
  staggerSelector = ".animated-item",
}: AnimatedListProps) {
  const containerRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (!containerRef.current) return;

    const items = containerRef.current.querySelectorAll(staggerSelector);
    
    anime({
      targets: items,
      translateY: [40, 0],
      opacity: [0, 1],
      duration: 700,
      delay: anime.stagger(100, { start: 150 }),
      easing: "easeOutExpo",
    });
  }, [staggerSelector]);

  return (
    <div ref={containerRef} className={className}>
      {children}
    </div>
  );
}

// ── Component: AnimatedCard ───────────────────────────────────
// Wrapper for individual cards with hover animation
interface AnimatedCardProps {
  children: React.ReactNode;
  className?: string;
  delay?: number;
}

export function AnimatedCard({
  children,
  className = "",
  delay = 0,
}: AnimatedCardProps) {
  const cardRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    anime({
      targets: cardRef.current,
      translateY: [40, 0],
      opacity: [0, 1],
      duration: 700,
      delay,
      easing: "easeOutExpo",
    });
  }, [delay]);

  const handleMouseEnter = () => {
    anime({
      targets: cardRef.current,
      translateY: -4,
      duration: 200,
      easing: "easeOutQuad",
    });
  };

  const handleMouseLeave = () => {
    anime({
      targets: cardRef.current,
      translateY: 0,
      duration: 200,
      easing: "easeOutQuad",
    });
  };

  return (
    <div
      ref={cardRef}
      className={`animated-item ${className}`}
      style={{ opacity: 0 }}
      onMouseEnter={handleMouseEnter}
      onMouseLeave={handleMouseLeave}
    >
      {children}
    </div>
  );
}

// ── Component: AnimatedButton ─────────────────────────────────
// Button with anime.js click ripple effect
interface AnimatedButtonProps {
  children: React.ReactNode;
  onClick?: () => void;
  className?: string;
  disabled?: boolean;
}

export function AnimatedButton({
  children,
  onClick,
  className = "",
  disabled = false,
}: AnimatedButtonProps) {
  const btnRef = useRef<HTMLButtonElement>(null);

  const handleClick = () => {
    if (disabled) return;

    anime({
      targets: btnRef.current,
      scale: [1, 0.95, 1],
      duration: 300,
      easing: "easeOutElastic(1, .5)",
    });

    onClick?.();
  };

  return (
    <button
      ref={btnRef}
      onClick={handleClick}
      disabled={disabled}
      className={className}
    >
      {children}
    </button>
  );
}