import { Component, OnInit } from '@angular/core';

import { Cart } from '../../core/models/cart';
import { CartService } from '../../core/services/cart.service';

@Component({
  selector: 'app-cart-page',
  templateUrl: './cart-page.component.html',
  styleUrls: ['./cart-page.component.scss']
})
export class CartPageComponent implements OnInit {
  cart: Cart;
  loading = true;

  constructor(private cartService: CartService) { }

  ngOnInit() {
    this.cartService.get().subscribe(cart => {
      this.cart = cart;
      this.loading = false;
    });
  }
}
