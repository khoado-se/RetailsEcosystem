# RetailsEcosystem

A multi-client eCommerce platform built with **ASP.NET Core 10** and **React 19**. A single REST API serves two clients: a server-rendered MVC storefront for customers and a React SPA for administrators.

---

## Table of Contents

- [Features](#features)
- [Architecture](#architecture)
- [Tech Stack](#tech-stack)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
  - [1. Clone the Repository](#1-clone-the-repository)
  - [2. Configure the API](#2-configure-the-api)
  - [3. Apply Database Migrations](#3-apply-database-migrations)
  - [4. Run the API](#4-run-the-api)
  - [5. Run the Customer Storefront](#5-run-the-customer-storefront)
  - [6. Run the Admin Dashboard](#6-run-the-admin-dashboard)
- [Project Structure](#project-structure)
- [Configuration Reference](#configuration-reference)
- [Testing](#testing)
- [Contributing](#contributing)
- [License](#license)

---

## Features

**Customer Storefront (MVC)**
- Product browsing with category filtering, search, and featured products
- Product detail page with image gallery
- Shopping cart (add, update, remove, clear)
- Checkout with VNPay payment integration
- Order history and order detail
- User registration, login, profile management, and avatar upload

**Admin Dashboard (React SPA)**
- Product management (create, edit, delete, image upload via Cloudinary)
- Category management
- Order management with status transitions
- Customer management with status control
- Dashboard with revenue chart and KPI tiles

---

## Architecture

```
┌─────────────────────────┐   ┌─────────────────────────┐
│  Customer Web (MVC/SSR) │   │  Admin Dashboard (SPA)  │
│  ASP.NET Core MVC       │   │  React 19 + Vite        │
└────────────┬────────────┘   └────────────┬────────────┘
             │ HTTP + JWT cookie            │ Axios + JWT
             ▼                             ▼
     ┌───────────────────────────────────────────┐
     │         Customer API (REST)               │
     │         ASP.NET Core Web API              │
     └───────────────────┬───────────────────────┘
                         │
             ┌───────────┴───────────┐
             ▼                       ▼
     Application Layer         Infrastructure Layer
     (Services, Validators)    (EF Core, Repositories,
                                Cloudinary, Identity)
             │
             ▼
     Domain Layer (Entities, Interfaces)
```

The solution follows **Clean Architecture**:

```
Domain → Application → Infrastructure → API / Web
```

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| API & Web | ASP.NET Core 10, C# 13 |
| ORM | Entity Framework Core 10 |
| Database | SQL Server 2019+ |
| Auth | ASP.NET Identity + JWT Bearer |
| Validation | FluentValidation |
| Image storage | Cloudinary |
| Admin SPA | React 19, Vite 8, React Router v7 |
| HTTP client | Axios |
| Admin styling | Bootstrap 5, Bootstrap Icons |
| Charts | Chart.js |
| Backend tests | xUnit, Moq, FluentAssertions |
| Frontend tests | Vitest, Testing Library |

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/) with npm
- SQL Server 2019+ (local instance or Docker)
- A [Cloudinary](https://cloudinary.com/) account (free tier works)

---

## Getting Started

### 1. Clone the Repository

```bash
git clone <repository-url>
cd RetailsEcosystem
```

### 2. Configure the API

Copy the example config and fill in your values:

```bash
cp src/RetailsEcosystem.Customer/RetailsEcosystem.Customer.API/appsettings.Development.json.example \
   src/RetailsEcosystem.Customer/RetailsEcosystem.Customer.API/appsettings.Development.json
```

Edit `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=RetailsEcosystemDb;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "JwtSettings": {
    "Secret": "<32-char-minimum-secret>",
    "Issuer": "RetailsEcosystem",
    "Audience": "RetailsEcosystemClient",
    "ExpiryMinutes": 15,
    "RefreshTokenExpiryDays": 7
  },
  "Cloudinary": {
    "CloudName": "<your-cloud-name>",
    "ApiKey": "<your-api-key>",
    "ApiSecret": "<your-api-secret>"
  },
  "AllowedOrigins": [
    "https://localhost:7035",
    "http://localhost:3000"
  ]
}
```

### 3. Apply Database Migrations

Run from the solution's `src/RetailsEcosystem.Customer/` directory:

```bash
cd src/RetailsEcosystem.Customer

dotnet ef database update \
  --project RetailsEcosystem.Customer.Infrastructure \
  --startup-project RetailsEcosystem.Customer.API
```

This applies all migrations and seeds the database with ~5,000 sample products.

### 4. Run the API

```bash
cd src/RetailsEcosystem.Customer/RetailsEcosystem.Customer.API
dotnet run
```

API available at `https://localhost:7035`.  
Swagger UI (dev only): `https://localhost:7035/swagger`.

### 5. Run the Customer Storefront

```bash
cd src/RetailsEcosystem.Customer/RetailsEcosystem.Customer.Web
dotnet run
```

Storefront available at the URL printed in the terminal (typically `https://localhost:<port>`).

### 6. Run the Admin Dashboard

```bash
cd src/RetailsEcosystem.Admin
npm install
```

Create `.env` in `src/RetailsEcosystem.Admin/`:

```env
VITE_HOST_URL=https://localhost:7035/api
VITE_PRODUCT_PLACEHOLDER_IMAGE=https://placehold.co/400x400?text=No+Image
```

```bash
npm run dev
```

Admin dashboard available at `http://localhost:3000`.

**Default admin credentials** (seeded on first migration):

| Field | Value |
|-------|-------|
| Email | `admin@retailsecosystem.com` |
| Password | `Admin@123` |

---

## Project Structure

```
RetailsEcosystem/
├── src/
│   ├── RetailsEcosystem.Customer/
│   │   ├── RetailsEcosystem.Customer.API/          # REST API controllers, middleware, auth
│   │   ├── RetailsEcosystem.Customer.Application/  # Business logic, services, validators
│   │   ├── RetailsEcosystem.Customer.Domain/       # Entities, repository interfaces
│   │   ├── RetailsEcosystem.Customer.Infrastructure/ # EF Core, repositories, Cloudinary
│   │   ├── RetailsEcosystem.Customer.Shared/       # DTOs, enums shared across layers
│   │   └── RetailsEcosystem.Customer.Web/          # MVC storefront (Razor views)
│   └── RetailsEcosystem.Admin/                     # React 19 Admin SPA
│       ├── src/
│       │   ├── features/                           # Feature-sliced modules
│       │   ├── pages/                              # Route-level page components
│       │   ├── layouts/                            # Shell layouts (AdminLayout, Sidebar)
│       │   ├── services/                           # apiClient, tokenService
│       │   └── contexts/                           # AuthContext
│       └── tests/                                  # Vitest unit tests
├── test/
│   └── RetailsEcosystem.Customer.Tests/            # xUnit backend tests
└── documents/                                      # Architecture, roadmap, design system
```

---

## Configuration Reference

### API — `appsettings.Development.json`

| Key | Description |
|-----|-------------|
| `ConnectionStrings.DefaultConnection` | SQL Server connection string |
| `JwtSettings.Secret` | HMAC-SHA256 signing key (min 32 chars) |
| `JwtSettings.ExpiryMinutes` | Access token TTL (default: 15) |
| `JwtSettings.RefreshTokenExpiryDays` | Refresh token TTL (default: 7) |
| `Cloudinary.CloudName` | Cloudinary cloud name |
| `Cloudinary.ApiKey` | Cloudinary API key |
| `Cloudinary.ApiSecret` | Cloudinary API secret |
| `AllowedOrigins` | CORS allowed origins array |

### Admin SPA — `.env`

| Variable | Description |
|----------|-------------|
| `VITE_HOST_URL` | API base URL (e.g. `https://localhost:7035/api`) |
| `VITE_PRODUCT_PLACEHOLDER_IMAGE` | Fallback image URL for products without images |

---

## Testing

### Backend Tests

```bash
# Run all tests
dotnet test test/RetailsEcosystem.Customer.Tests

# Run with coverage
dotnet test test/RetailsEcosystem.Customer.Tests \
  --collect:"XPlat Code Coverage" \
  --settings test/RetailsEcosystem.Customer.Tests/coverlet.runsettings
```

Tests cover: services (Cart, Order, Product, Category, Customer, ProductImage, VnPay), controllers (all endpoints), validators (Register, Login), and middleware (GlobalExceptionMiddleware).

### Frontend Tests

```bash
cd src/RetailsEcosystem.Admin

# Run tests in watch mode
npm test

# Run once with coverage
npm run coverage
```

Tests cover: custom hooks (useProducts, useCategories, useOrders, useCustomers), utility functions, and form validation.

---

## API Reference

The API follows REST conventions. All protected endpoints require a `Bearer` token in the `Authorization` header.

| Prefix | Description |
|--------|-------------|
| `POST /api/auth/register` | Register new customer account |
| `POST /api/auth/login` | Authenticate, receive JWT + refresh cookie |
| `POST /api/auth/refresh` | Refresh access token via httpOnly cookie |
| `POST /api/auth/logout` | Revoke refresh token |
| `GET /api/products` | Paginated product list (public) |
| `GET /api/products/{id}` | Product detail (public) |
| `POST /api/products` | Create product (Admin) |
| `PUT /api/products/{id}` | Update product (Admin) |
| `DELETE /api/products/{id}` | Delete product (Admin) |
| `GET /api/categories` | List categories (public) |
| `GET /api/carts` | Get current user's cart |
| `POST /api/carts/items` | Add item to cart |
| `PUT /api/carts/items/{id}` | Update cart item quantity |
| `DELETE /api/carts/items/{id}` | Remove cart item |
| `POST /api/orders` | Place order |
| `GET /api/orders` | List orders (paginated, role-scoped) |
| `POST /api/orders/{id}/pay` | Initiate VNPay payment |
| `GET /api/customers/me` | Get current user profile |
| `PUT /api/customers/me` | Update profile |

Full OpenAPI documentation available at `/swagger` in development.

---

## Contributing

1. **Fork** the repository and create a feature branch:
   ```bash
   git checkout -b feat/your-feature-name
   ```

2. **Follow existing conventions:**
   - Clean Architecture dependency rule — outer layers depend inward only
   - Business logic belongs in Application services, not controllers
   - Controllers must remain thin — no business logic
   - Match existing coding style and naming patterns
   - No try/catch for business rules in controllers — throw domain exceptions (`NotFoundException`, `ConflictException`) and let the middleware handle them

3. **Write tests** for new behavior. Minimum: unit tests for new service methods.

4. **Verify before opening a PR:**
   ```bash
   dotnet build src/RetailsEcosystem.Customer
   dotnet test test/RetailsEcosystem.Customer.Tests
   cd src/RetailsEcosystem.Admin && npm run lint && npm run test:run
   ```

5. **Open a Pull Request** with a clear description of what changed and why.

---

## License

This project is for educational and assignment purposes. No license is granted for commercial use.

---

## Contact

For questions or issues related to this project, open a GitHub issue on the repository.
