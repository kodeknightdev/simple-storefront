import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';

import { CartStore } from './core/state/cart.store';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  cartCount$: Observable<number>;

  constructor(private cartStore: CartStore) { }

  ngOnInit() {
    this.cartCount$ = this.cartStore.count$;
    this.cartStore.load();
  }
}
