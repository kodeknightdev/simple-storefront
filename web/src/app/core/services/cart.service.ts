import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of, Subject } from 'rxjs';
import { map, switchMap, tap } from 'rxjs/operators';

import { environment } from '../../../environments/environment';
import { Cart } from '../models/cart';

const CART_KEY = 'shelf.cartId';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private readonly baseUrl = `${environment.apiUrl}/carts`;

  // fires whenever we get a fresh cart back from the api (header badge listens to this)
  readonly changed = new Subject<Cart>();

  constructor(private http: HttpClient) { }

  get cartId(): string {
    return localStorage.getItem(CART_KEY);
  }

  get(): Observable<Cart> {
    if (!this.cartId) {
      return of(null);
    }
    return this.http.get<Cart>(`${this.baseUrl}/${this.cartId}`);
  }

  add(productId: number, quantity = 1): Observable<Cart> {
    return this.ensureCart().pipe(
      switchMap(id => this.http.post<Cart>(`${this.baseUrl}/${id}/items`, { productId, quantity })),
      tap(cart => this.changed.next(cart))
    );
  }

  update(productId: number, quantity: number): Observable<Cart> {
    return this.http.put<Cart>(`${this.baseUrl}/${this.cartId}/items/${productId}`, { productId, quantity })
      .pipe(tap(cart => this.changed.next(cart)));
  }

  remove(productId: number): Observable<Cart> {
    return this.http.delete<Cart>(`${this.baseUrl}/${this.cartId}/items/${productId}`)
      .pipe(tap(cart => this.changed.next(cart)));
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
