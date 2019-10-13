import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { catchError, map, switchMap, tap } from 'rxjs/operators';

import { environment } from '../../../environments/environment';
import { Cart } from '../models/cart';

const CART_KEY = 'shelf.cartId';

// Thin wrapper around the carts api + the id we keep in localStorage.
// Components should go through CartStore instead of using this directly.
@Injectable({
  providedIn: 'root'
})
export class CartService {
  private readonly baseUrl = `${environment.apiUrl}/carts`;

  constructor(private http: HttpClient) { }

  get cartId(): string {
    return localStorage.getItem(CART_KEY);
  }

  get(): Observable<Cart> {
    if (!this.cartId) {
      return of(null);
    }
    return this.http.get<Cart>(`${this.baseUrl}/${this.cartId}`).pipe(
      catchError((err: HttpErrorResponse) => {
        // cart got cleaned up on the server (or the db was reset) - start over
        if (err.status === 404) {
          this.clear();
          return of(null);
        }
        return throwError(err);
      })
    );
  }

  add(productId: number, quantity = 1): Observable<Cart> {
    return this.ensureCart().pipe(
      switchMap(id => this.addTo(id, productId, quantity)),
      catchError((err: HttpErrorResponse) => {
        // stale cart id - make a new cart and try once more
        if (err.status === 404 && err.error && !err.error.message) {
          this.clear();
          return this.ensureCart().pipe(switchMap(id => this.addTo(id, productId, quantity)));
        }
        return throwError(err);
      })
    );
  }

  update(productId: number, quantity: number): Observable<Cart> {
    return this.http.put<Cart>(`${this.baseUrl}/${this.cartId}/items/${productId}`, { productId, quantity });
  }

  remove(productId: number): Observable<Cart> {
    return this.http.delete<Cart>(`${this.baseUrl}/${this.cartId}/items/${productId}`);
  }

  // after a successful payment - next add will start a new cart
  clear() {
    localStorage.removeItem(CART_KEY);
  }

  private addTo(cartId: string, productId: number, quantity: number): Observable<Cart> {
    return this.http.post<Cart>(`${this.baseUrl}/${cartId}/items`, { productId, quantity });
  }

  // the server hands out the cart id, we just hang on to it
  private ensureCart(): Observable<string> {
    if (this.cartId) {
      return of(this.cartId);
    }
    return this.http.post<Cart>(this.baseUrl, {}).pipe(
      tap(cart => localStorage.setItem(CART_KEY, cart.id)),
      map(cart => cart.id)
    );
  }
}
