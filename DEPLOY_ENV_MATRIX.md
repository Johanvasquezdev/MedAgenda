# Deployment Environment Matrix

This matrix defines canonical environment variables and platform mapping for
MedAgenda web (Vercel) and API (DigitalOcean).

## Environments

- `dev`: local development.
- `staging`: pre-production verification.
- `prod`: production.

## Frontend (Vercel)

Required variables:

- `NEXT_PUBLIC_API_URL` (preferred)
- `NEXT_PUBLIC_API_BASE_URL` (fallback/compat)

Per environment:

- `dev`: `http://localhost:5189`
- `staging`: `https://api-staging.<your-domain>`
- `prod`: `https://api.<your-domain>`

Notes:

- Keep only one of `NEXT_PUBLIC_API_URL` or `NEXT_PUBLIC_API_BASE_URL` as source of truth where possible.
- Preview environments should point to staging API unless ephemeral backend is available.

## API (DigitalOcean)

Required variables:

- `ConnectionStrings__SGCConnection`
- `ConnectionStrings__Redis`
- `Jwt__Key`
- `Jwt__Issuer`
- `Jwt__Audience`
- `Jwt__ExpireMinutes`
- `Stripe__PublicKey`
- `Stripe__SecretKey`
- `Stripe__WebhookSecret`
- `AI__Provider`
- `AI__BaseUrl`
- `AI__Model`
- `AI__ApiKey`
- `AI__MaxTokens`
- `AI__Temperature`
- `Cors__AllowedOrigins__0`
- `Cors__AllowedOrigins__1` (optional second origin)

Optional/production hardening variables:

- `MessageBroker__Provider` (`RabbitMQ` | `Kafka` | `None`)
- `RabbitMQ__Host`
- `RabbitMQ__Port`
- `RabbitMQ__Username`
- `RabbitMQ__Password`
- `RabbitMQ__VirtualHost`
- `Kafka__BootstrapServers`
- `Kafka__Username` (if SASL)
- `Kafka__Password` (if SASL)
- `Kafka__SecurityProtocol`
- `Twilio__AccountSid`
- `Twilio__AuthToken`
- `Twilio__FromPhoneNumber`
- `Smtp__Host`
- `Smtp__Port`
- `Smtp__User`
- `Smtp__Password`
- `Smtp__FromEmail`

Per environment examples:

- `dev`: localhost origins and local DB/Redis.
- `staging`: Vercel preview + staging web domain origins.
- `prod`: production web domains only.

## CORS mapping rule

- Vercel web origin must be explicitly listed in API `Cors:AllowedOrigins`.
- For preview deployments, either:
  - allow only known preview domain pattern via controlled gateway, or
  - route preview web to staging API with a stable domain.

## Promotion checklist

1. Build and tests pass.
2. Frontend env points to target API env.
3. API has correct CORS origins for that frontend env.
4. Health endpoint responds: `/health`.
5. Smoke flows pass: auth, citas CRUD happy path, SignalR connect, payment intent creation.
6. Dependency probes pass: Redis, Twilio, SMTP, SignalR.
7. Broker probes pass for enabled provider (RabbitMQ and/or Kafka).
