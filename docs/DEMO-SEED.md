# MedAgenda demo seed

The API seeds fictional demo content automatically only in the Development environment. The seed is repeatable: it reuses its demo accounts and catalog entries and adds each sample appointment only once. It does not create tables or run migrations; the local database schema must already exist.

For safety, the seed runs only against a loopback/local PostgreSQL host by default. Set `DemoSeed__AllowRemote=true` only when the configured database is a dedicated disposable demo database. Set `DemoSeed__Enabled=false` to turn off development seeding.

The demo administrator is:

- Email or account name: `admin.demo@medagenda.test`
- Password: `MedAgendaDemo2026!`

Sample patient and doctor accounts use the same demo password. All names, contact details, insurance identifiers, emails, clinics, and appointments are fictional and use the reserved `.test` domain or demo markers.

The seed creates three specialties, three clinics, three patients, three doctors, and four appointments so the admin dashboard and its related lists have sample records to display.
