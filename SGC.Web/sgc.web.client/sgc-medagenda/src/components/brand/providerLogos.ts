const normalized = (name: string) =>
  name.normalize("NFD").replace(/[\u0300-\u036f]/g, "").trim().toLowerCase().replace(/\s+/g, " ");

interface ProviderLogoData {
  src: string;
  featureSrc: string;
  featureWidth: number;
  featureHeight: number;
  alt: string;
}

const providerLogos: Record<string, ProviderLogoData> = {
  "ars humano": { src: "/provider-logos/humano.png", featureSrc: "/provider-logos/feature/humano.webp", featureWidth: 382, featureHeight: 52, alt: "ARS Primera, antes ARS Humano" },
  humano: { src: "/provider-logos/humano.png", featureSrc: "/provider-logos/feature/humano.webp", featureWidth: 382, featureHeight: 52, alt: "ARS Primera, antes ARS Humano" },
  "ars universal": { src: "/provider-logos/ars-universal.png", featureSrc: "/provider-logos/feature-transparent/ars-universal.png", featureWidth: 640, featureHeight: 191, alt: "ARS Universal" },
  universal: { src: "/provider-logos/ars-universal.png", featureSrc: "/provider-logos/feature-transparent/ars-universal.png", featureWidth: 640, featureHeight: 191, alt: "ARS Universal" },
  senasa: { src: "/provider-logos/senasa.png", featureSrc: "/provider-logos/feature-transparent/senasa.png", featureWidth: 175, featureHeight: 50, alt: "SeNaSa" },
  "se nasa": { src: "/provider-logos/senasa.png", featureSrc: "/provider-logos/feature-transparent/senasa.png", featureWidth: 175, featureHeight: 50, alt: "SeNaSa" },
  "ars senasa": { src: "/provider-logos/senasa.png", featureSrc: "/provider-logos/feature-transparent/senasa.png", featureWidth: 175, featureHeight: 50, alt: "SeNaSa" },
  "ars reservas": { src: "/provider-logos/ars-reservas.png", featureSrc: "/provider-logos/feature-transparent/ars-reservas.png", featureWidth: 535, featureHeight: 160, alt: "ARS Reservas" },
  reservas: { src: "/provider-logos/ars-reservas.png", featureSrc: "/provider-logos/feature-transparent/ars-reservas.png", featureWidth: 535, featureHeight: 160, alt: "ARS Reservas" },
};

export function getProviderLogo(name: string): ProviderLogoData | null {
  return providerLogos[normalized(name)] ?? null;
}

