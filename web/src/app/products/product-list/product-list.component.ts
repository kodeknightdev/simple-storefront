import { Component, OnInit } from '@angular/core';

import { Product } from '../../core/models/product';
import { ProductService } from '../../core/services/product.service';

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss']
})
export class ProductListComponent implements OnInit {
  products: Product[] = [];
  loading = false;

  constructor(private productService: ProductService) { }

  ngOnInit() {
    this.loading = true;
    this.productService.list().subscribe(result => {
      this.products = result.items;
      this.loading = false;
    });
  }
}
