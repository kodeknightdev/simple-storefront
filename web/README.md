# Shelf Simple - web

Angular 8 frontend for the Shelf Simple storefront.

## Setup

Node 10 LTS, then:

```
npm install
npm start
```

`npm start` runs `ng serve` on http://localhost:4200. It expects the API running from
Visual Studio on https://localhost:44362 (see `../server`). Both urls live in
`src/environments/environment.ts`.

Put your Stripe **test** publishable key (`pk_test_...`) in `stripePublishableKey` in the
environment files. Test card: `4242 4242 4242 4242`, any future date, any CVC.

## Layout

- `core/` - models, api services, the auth interceptor/guard and `CartStore` (cart state)
- `products/` - listing, product detail and the per-seller shop page
- `cart/` - cart page (lazy loaded)
- `checkout/` - Stripe Elements card form + order complete page (lazy loaded)
- `seller/` - login/sign up, dashboard, product form, orders, profile (lazy loaded, behind `AuthGuard`)

## Other commands

- `npm run build -- --prod` - production build into `dist/`
- `npm run lint`
- `npm test` - karma (there isn't much in the way of specs yet)
