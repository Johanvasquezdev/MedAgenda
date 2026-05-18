"use client";
import { useEffect, useRef } from "react";
import * as THREE from "three";

export function ThreeBackground() {
  const mountRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const mount = mountRef.current!;
    if (!mount) return;

    // ── Scene Setup ──────────────────────────────────────────
    const scene = new THREE.Scene();
    const camera = new THREE.PerspectiveCamera(
      75,
      mount.clientWidth / mount.clientHeight,
      0.1,
      1000
    );
    const renderer = new THREE.WebGLRenderer({
      alpha: true,
      antialias: true,
    });

    renderer.setSize(mount.clientWidth, mount.clientHeight);
    renderer.setPixelRatio(Math.min(window.devicePixelRatio, 2));
    renderer.setClearColor(0x000000, 0);
    mount.appendChild(renderer.domElement);

    // ── Particles ────────────────────────────────────────────
    const particleCount = 300;
    const positions = new Float32Array(particleCount * 3);
    const colors = new Float32Array(particleCount * 3);

    // Cyan and emerald colors
    const colorPalette = [
      new THREE.Color(0x06b6d4), // emerald-500
      new THREE.Color(0x14b8a6), // emerald-500
      new THREE.Color(0x22d3ee), // emerald-400
      new THREE.Color(0x2dd4bf), // emerald-400
      new THREE.Color(0x67e8f9), // emerald-300
    ];

    for (let i = 0; i < particleCount; i++) {
      positions[i * 3]     = (Math.random() - 0.5) * 30;
      positions[i * 3 + 1] = (Math.random() - 0.5) * 30;
      positions[i * 3 + 2] = (Math.random() - 0.5) * 30;

      const color = colorPalette[Math.floor(Math.random() * colorPalette.length)];
      colors[i * 3]     = color.r;
      colors[i * 3 + 1] = color.g;
      colors[i * 3 + 2] = color.b;
    }

    const geometry = new THREE.BufferGeometry();
    geometry.setAttribute("position", new THREE.BufferAttribute(positions, 3));
    geometry.setAttribute("color", new THREE.BufferAttribute(colors, 3));

    const material = new THREE.PointsMaterial({
      size: 0.08,
      vertexColors: true,
      transparent: true,
      opacity: 0.7,
      sizeAttenuation: true,
    });

    const particles = new THREE.Points(geometry, material);
    scene.add(particles);

    // ── Connecting Lines (network effect) ────────────────────
    const lineMaterial = new THREE.LineBasicMaterial({
      color: 0x06b6d4,
      transparent: true,
      opacity: 0.08,
    });

    const lineGeometry = new THREE.BufferGeometry();
    const linePositions: number[] = [];

    for (let i = 0; i < 60; i++) {
      const x1 = (Math.random() - 0.5) * 20;
      const y1 = (Math.random() - 0.5) * 20;
      const z1 = (Math.random() - 0.5) * 20;
      const x2 = x1 + (Math.random() - 0.5) * 5;
      const y2 = y1 + (Math.random() - 0.5) * 5;
      const z2 = z1 + (Math.random() - 0.5) * 5;
      linePositions.push(x1, y1, z1, x2, y2, z2);
    }

    lineGeometry.setAttribute(
      "position",
      new THREE.BufferAttribute(new Float32Array(linePositions), 3)
    );
    const lines = new THREE.LineSegments(lineGeometry, lineMaterial);
    scene.add(lines);

    camera.position.z = 8;

    // ── Mouse interaction ────────────────────────────────────
    const mouse = { x: 0, y: 0 };
    const handleMouseMove = (e: MouseEvent) => {
      mouse.x = (e.clientX / window.innerWidth - 0.5) * 0.3;
      mouse.y = (e.clientY / window.innerHeight - 0.5) * 0.3;
    };
    window.addEventListener("mousemove", handleMouseMove);

    // ── Resize handler ───────────────────────────────────────
    const handleResize = () => {
      camera.aspect = mount.clientWidth / mount.clientHeight;
      camera.updateProjectionMatrix();
      renderer.setSize(mount.clientWidth, mount.clientHeight);
    };
    window.addEventListener("resize", handleResize);

    // ── Animation Loop ───────────────────────────────────────
    let animId: number;
    const clock = new THREE.Clock();

    const animate = () => {
      animId = requestAnimationFrame(animate);
      const elapsed = clock.getElapsedTime();

      particles.rotation.x = elapsed * 0.04 + mouse.y;
      particles.rotation.y = elapsed * 0.06 + mouse.x;
      lines.rotation.x = elapsed * 0.02 + mouse.y * 0.5;
      lines.rotation.y = elapsed * 0.03 + mouse.x * 0.5;

      // Breathing scale effect
      const scale = 1 + Math.sin(elapsed * 0.5) * 0.03;
      particles.scale.setScalar(scale);

      renderer.render(scene, camera);
    };
    animate();

    // ── Cleanup ───────────────────────────────────────────────
    return () => {
      cancelAnimationFrame(animId);
      window.removeEventListener("mousemove", handleMouseMove);
      window.removeEventListener("resize", handleResize);
      if (mount.contains(renderer.domElement)) {
        mount.removeChild(renderer.domElement);
      }
      geometry.dispose();
      material.dispose();
      lineGeometry.dispose();
      lineMaterial.dispose();
      renderer.dispose();
    };
  }, []);

  return (
    <div
      ref={mountRef}
      className="absolute inset-0 z-0"
      style={{ pointerEvents: "none" }}
    />
  );
}