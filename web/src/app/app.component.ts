import { Component, OnInit } from '@angular/core';

import { CartService } from './core/services/cart.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  cartCount = 0;

  constructor(private cartService: CartService) { }

  ngOnInit() {
    this.cartService.get().subscribe(
      cart => this.cartCount = cart ? cart.itemCount : 0,
      () => this.cartCount = 0
    );
    this.cartService.changed.subscribe(cart => this.cartCount = cart.itemCount);
  }
}
