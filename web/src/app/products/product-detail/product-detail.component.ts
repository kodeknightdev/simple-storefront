import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';
import { switchMap } from 'rxjs/operators';

import { Product } from '../../core/models/product';
import { ProductService } from '../../core/services/product.service';
import { CartStore } from '../../core/state/cart.store';

@Component({
  selector: 'app-product-detail',
  templateUrl: './product-detail.component.html',
  styleUrls: ['./product-detail.component.scss']
})
export class ProductDetailComponent implements OnInit {
  product$: Observable<Product>;
  quantity = 1;
  adding = false;
  added = false;

  constructor(
    private route: ActivatedRoute,
    private productService: ProductService,
    private cartStore: CartStore
  ) { }

  ngOnInit() {
    this.product$ = this.route.paramMap.pipe(
      switchMap(params => this.productService.get(+params.get('id')))
    );
  }

  addToCart(product: Product) {
    this.adding = true;
    this.added = false;
    this.cartStore.add(product.id, +this.quantity).subscribe(() => {
      this.adding = false;
      this.added = true;
    }, () => this.adding = false);
  }

  range(n: number): number[] {
    return Array.from({ length: Math.min(n, 10) }, (_, i) => i + 1);
  }
}
