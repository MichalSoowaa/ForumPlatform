# [WORK IN PROGRESS]

# ForumPlatform

A discussion forum combining traditional forum structure with a reputation/expertise
system and AI-assisted content quality scoring.

## What it does

- Subforums, threads, and two-level nested comments
- Every post is scored for substance: fast heuristics first (length, structure,
  sources), (potentially escalating to an LLM for ambigous cases?).
- Reputation and vote weight scale with a user's own contribution quality, not just
  raw vote counts
- Hybrid full-text + vector (pgvector) search, including duplicate-thread detection

## Stack

**Backend:** ASP.NET Core (.NET 10) · EF Core · PostgreSQL + pgvector · MediatR (CQRS-lite) · FluentValidation · Hangfire · JWT auth
**Frontend:** React · Vite · TanStack Query
**Architecture:** modular monolith — one deployable, schema-per-module (`Users`, `Forum`, `Reputation`, `Social`), designed so any module can later be extracted into its own service without a redesign

## Running locally

1. Start Postgres:
   ```bash
   docker compose up -d
   ```
2. Set backend secrets (from `src/ForumPlatform.WebApi`):
   ```bash
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:Postgres" "Host=localhost;Port=5432;Database=forumplatform;Username=forumplatform;Password=forumplatform_dev_password"
   dotnet user-secrets set "Jwt:SigningKey" "<32+ char random string>"
   dotnet user-secrets set "Jwt:Issuer" "ForumPlatform"
   dotnet user-secrets set "Jwt:Audience" "ForumPlatform.Client"
   ```
3. Apply migrations (repeat `--project` per module as new ones are added):
   ```bash
   dotnet ef database update --project src/ForumPlatform.Users.Infrastructure --startup-project src/ForumPlatform.WebApi
   dotnet ef database update --project src/ForumPlatform.Forum.Infrastructure --startup-project src/ForumPlatform.WebApi
   ```
4. Run the backend:
   ```bash
   dotnet run --project src/ForumPlatform.WebApi
   ```
5. Run the frontend:
   ```bash
   cd frontend && npm install && npm run dev
   ```
