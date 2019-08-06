import { Component, OnInit } from '@angular/core';

import { Cart, CartItem } from '../../core/models/cart';
import { CartService } from '../../core/services/cart.service';

@Component({
  selector: 'app-cart-page',
  templateUrl: './cart-page.component.html',
  styleUrls: ['./cart-page.component.scss']
})
export class CartPageComponent implements OnInit {
  cart: Cart;
  loading = true;
  busy = false;

  constructor(private cartService: CartService) { }

  ngOnInit() {
    this.cartService.get().subscribe(cart => {
      this.cart = cart;
      this.loading = false;
    });
  }

  changeQuantity(item: CartItem, quantity: number) {
    this.busy = true;
    this.cartService.update(item.productId, quantity)
      .subscribe(cart => this.done(cart), () => this.busy = false);
  }

  remove(item: CartItem) {
    this.busy = true;
    this.cartService.remove(item.productId)
      .subscribe(cart => this.done(cart), () => this.busy = false);
  }

  private done(cart: Cart) {
    this.cart = cart;
    this.busy = false;
  }
}
