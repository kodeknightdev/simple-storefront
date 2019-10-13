import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';

import { Cart, CartItem } from '../../core/models/cart';
import { CartStore } from '../../core/state/cart.store';

@Component({
  selector: 'app-cart-page',
  templateUrl: './cart-page.component.html',
  styleUrls: ['./cart-page.component.scss']
})
export class CartPageComponent implements OnInit {
  cart$: Observable<Cart>;
  busy = false;

  constructor(private cartStore: CartStore) { }

  ngOnInit() {
    this.cart$ = this.cartStore.cart$;
    this.cartStore.load(true);
  }

  changeQuantity(item: CartItem, quantity: number) {
    this.busy = true;
    this.cartStore.update(item.productId, quantity)
      .subscribe(() => this.busy = false, () => this.busy = false);
  }

  remove(item: CartItem) {
    this.busy = true;
    this.cartStore.remove(item.productId)
      .subscribe(() => this.busy = false, () => this.busy = false);
  }
}
