# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

ASP.NET Core 8.0 MVC portfolio management application with a BackOffice admin panel and public-facing frontend. Uses a hybrid database approach: MongoDB for application data (Products, Pictures) and SQLite with Entity Framework Core for ASP.NET Identity authentication.

## Common Commands

### Running the Application

```bash
# Development (HTTP on localhost:5125)
dotnet run --launch-profile http

# Development with HTTPS (localhost:7140)
dotnet run --launch-profile https

# Build for production
dotnet build -c Release
```

### Prerequisites

- MongoDB running on `localhost:27017` (database: `MongoDemoDb`)
- Run `dotnet ef database update` for Identity tables (SQLite)

### Database Migrations (Identity only)

```bash
dotnet ef database update              # Apply migrations
dotnet ef migrations add <Name>        # Create new migration
dotnet ef migrations list              # View status
```

### Docker

```bash
docker build -t testportfolio .
```

## Architecture

**Hybrid Database Pattern:**
- MongoDB (`MongoDemoDb`) - Products and Pictures collections via `Services/`
- SQLite (`App.db`) - ASP.NET Identity tables only via `Data/ApplicationDbContext.cs`

**Service Layer:**
- `Services/ProductService.cs` - MongoDB CRUD for products (singleton)
- `Services/PictureService.cs` - MongoDB CRUD for pictures (singleton)
- `MongoSettings.cs` - Configuration model bound from `appsettings.json`

**Key Controllers:**
- `BackOffice.cs` - Admin panel (requires `[Authorize]`), product/image management
- `ProductController.cs` - Product CRUD operations
- `HomeController.cs` - Public portfolio display
- `PictureController.cs` - Image handling

**Models:**
- `Product` - Has images list (one-to-many with Picture), isPublic flag for visibility
- `Picture` - Linked to product via productId, has PictureUsage enum

**Static Files:**
- `wwwroot/uploads/` - User-uploaded product images
- `wwwroot/lib/` - Bootstrap 5, jQuery

**Security Middleware:**
- `Middleware/SecurityHeadersMiddleware.cs` - Ajoute les headers de sécurité (CSP, X-Frame-Options, etc.)

## Configuration

- MongoDB settings in `appsettings.json` under `MongoDbSettings`
- SQLite connection string under `ConnectionStrings.DefaultConnection`
- Identity requires confirmed email accounts (`RequireConfirmedAccount = true`)

### User Secrets (Développement)

Pour stocker les connexions sensibles en développement :

```bash
# Initialiser (déjà fait)
dotnet user-secrets init

# Stocker la connection string MongoDB
dotnet user-secrets set "MongoDbSettings:ConnectionString" "mongodb://user:password@host:27017"

# Voir les secrets
dotnet user-secrets list
```

### Production

En production, utiliser des variables d'environnement :

```bash
export MongoDbSettings__ConnectionString="mongodb://..."
export ConnectionStrings__DefaultConnection="Data Source=..."
```

## Security

- **Authentication** : ASP.NET Identity avec cookies sécurisés (HttpOnly, Secure, SameSite=Strict)
- **Authorization** : `[Authorize]` sur les endpoints sensibles (BackOffice, ProductController)
- **CSRF** : `[ValidateAntiForgeryToken]` + `@Html.AntiForgeryToken()` sur tous les formulaires POST
- **File Upload** : Validation whitelist (JPEG, PNG, GIF, WebP), limite 10 MB
- **Headers** : CSP, X-Frame-Options, X-Content-Type-Options via middleware
