# Vercel + DigitalOcean Setup Checklist

Practical checklist to execute Sprint 0 for hosted deployment.

## 1) Vercel Project Setup (Web)

- [ ] Import `SGC.Web/sgc.web.client/sgc-medagenda` as a Vercel project.
- [ ] Set framework preset to Next.js.
- [ ] Configure env vars:
  - [ ] `NEXT_PUBLIC_API_URL`
  - [ ] `NEXT_PUBLIC_API_BASE_URL` (optional legacy fallback)
- [ ] Enable preview deployments.
- [ ] Bind staging and production domains.

## 2) DigitalOcean API Setup

Choose one runtime:

- [ ] Option A: App Platform (recommended for faster setup)
- [ ] Option B: Droplet + Docker Compose (more control)

Then:

- [ ] Deploy API from repo root using `SGC.API/Dockerfile`.
- [ ] Inject required API env vars (JWT, DB, Redis, Stripe, AI, CORS).
- [ ] Expose `/health` endpoint publicly or through trusted network.
- [ ] Verify API startup succeeds with non-dev CORS configuration.

## 3) Managed Services

- [ ] Provision PostgreSQL (DO managed or external).
- [ ] Provision Redis (DO managed or external).
- [ ] Validate connectivity from API runtime.

## 4) Domain + TLS + CORS

- [ ] Point `api-staging.<domain>` and `api.<domain>` to DO runtime.
- [ ] Ensure HTTPS certificates are active.
- [ ] Set `Cors__AllowedOrigins__0/1` for Vercel domains.
- [ ] Confirm preflight requests succeed from browser.

## 5) Smoke Validation

- [ ] Web loads and can call API.
- [ ] Login works.
- [ ] Patient citas list works.
- [ ] SignalR connection to `/citahub` works.
- [ ] Payment intent creation endpoint works.

## 6) Release Gates

- [ ] `dotnet build SGC.sln` passes.
- [ ] `dotnet test SGC.ApplicationTest/SGC.ApplicationTest.csproj` passes.
- [ ] `npm run lint` passes.
- [ ] Deployment notes updated in README/backlog docs.

## 7) Production Ready Checkmarks

- [ ] Message broker strategy defined:
  - [ ] RabbitMQ for background jobs/commands.
  - [ ] Kafka for event streaming/audit (only if required).
  - [ ] If both are enabled, ownership split is documented.
- [ ] Broker runtime checks implemented:
  - [ ] RabbitMQ publish/consume health probe passes.
  - [ ] Kafka producer/consumer test topic probe passes.
- [ ] Dependency health checks are exposed and monitored:
  - [ ] Redis connectivity + set/get + TTL probe.
  - [ ] Twilio API auth + test message/status callback probe.
  - [ ] SMTP auth + test email delivery probe.
  - [ ] SignalR `/citahub` negotiate/connect + test broadcast probe.
- [ ] Logging security hardening:
  - [ ] No PII (email/phone) at `INF` level.
  - [ ] Correlation id present in request and dependency logs.
- [ ] CI/CD security gates active on every PR/main deployment:
  - [ ] SAST (CodeQL or equivalent).
  - [ ] Dependency vulnerability scan for .NET and npm.
  - [ ] Secret scanning (gitleaks/trufflehog).
- [ ] Post-deploy smoke test in staging and production includes:
  - [ ] Auth flow.
  - [ ] Citas read/write flow.
  - [ ] SignalR real-time event.
  - [ ] External dependency probes (Redis/Twilio/SMTP/broker).
