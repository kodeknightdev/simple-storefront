import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { map, tap } from 'rxjs/operators';

import { Cart } from '../models/cart';
import { CartService } from '../services/cart.service';

// Single place that holds the current cart. Everything that shows the cart
// (header badge, cart page, checkout summary) subscribes to cart$ rather than
// fetching it again.
@Injectable({
  providedIn: 'root'
})
export class CartStore {
  private readonly state = new BehaviorSubject<Cart>(null);
  private loaded = false;

  readonly cart$: Observable<Cart> = this.state.asObservable();
  readonly count$: Observable<number> = this.cart$.pipe(map(cart => cart ? cart.itemCount : 0));

  constructor(private cartService: CartService) { }

  get snapshot(): Cart {
    return this.state.value;
  }

  get cartId(): string {
    return this.cartService.cartId;
  }

  load(force = false) {
    if (this.loaded && !force) {
      return;
    }
    this.loaded = true;
    this.cartService.get().subscribe(cart => this.state.next(cart), () => this.state.next(null));
  }

  add(productId: number, quantity = 1): Observable<Cart> {
    return this.cartService.add(productId, quantity).pipe(tap(cart => this.state.next(cart)));
  }

  update(productId: number, quantity: number): Observable<Cart> {
    return this.cartService.update(productId, quantity).pipe(tap(cart => this.state.next(cart)));
  }

  remove(productId: number): Observable<Cart> {
    return this.cartService.remove(productId).pipe(tap(cart => this.state.next(cart)));
  }

  // after checkout - forget the cart id and empty the state so the badge resets
  reset() {
    this.cartService.clear();
    this.state.next(null);
  }
}
