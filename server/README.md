# Shelf Simple API

ASP.NET Web API 2 on .NET Framework 4.7.2, Entity Framework 6, SQL Server.
Payments go through Stripe in **test mode only**.

## Running it

1. Open `ShelfSimple.sln` in Visual Studio 2019 and restore NuGet packages.
2. The default connection string points at LocalDB (`(localdb)\MSSQLLocalDB`, database `ShelfSimple`).
   Create the database from the Package Manager Console:

   ```
   Update-Database
   ```

   This also seeds two dev sellers (`maple@example.com` / `june@example.com`, password `devpassword1`)
   and a handful of products.
3. Copy `AppSettings.secret.config.example` to `AppSettings.secret.config` and put your Stripe
   **test** keys in it (`sk_test_...`, `whsec_...`). That file is git-ignored.
4. F5. IIS Express serves the API on https://localhost:44362.

## Webhooks

With the Stripe CLI:

```
stripe listen --forward-to https://localhost:44362/api/stripe/webhook
```

and use the `whsec_...` it prints as `stripe:WebhookSecret`.

Test card: `4242 4242 4242 4242`, any future expiry, any CVC.

## Endpoints

| Method | Route | Notes |
| --- | --- | --- |
| GET | /api/products | `?category=&page=&pageSize=` |
| GET | /api/products/categories | |
| GET | /api/products/{id} | |
| GET | /api/shops/{slug} | seller's public page |
| POST | /api/carts | new anonymous cart |
| GET | /api/carts/{id} | |
| POST | /api/carts/{id}/items | `{ productId, quantity }` |
| PUT/DELETE | /api/carts/{id}/items/{productId} | |
| POST | /api/checkout | `{ cartId, email }` -> client secret |
| POST | /api/checkout/{orderId}/confirm | |
| POST | /api/stripe/webhook | |
| POST | /api/sellers/register | |
| POST | /token | `grant_type=password` |
| GET/PUT | /api/sellers/me | auth |
| GET/POST/PUT/DELETE | /api/seller/products | auth |
| GET | /api/seller/orders | auth |

## Migrations

Code First migrations live in `Migrations/`. After changing a model:

```
Add-Migration SomeName
Update-Database
```
