import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { Product } from '../../core/models/product';
import { AuthService } from '../../core/services/auth.service';
import { SellerProductService } from '../../core/services/seller-product.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  products: Product[] = [];
  loading = true;

  constructor(
    private sellerProducts: SellerProductService,
    private auth: AuthService,
    private router: Router
  ) { }

  ngOnInit() {
    this.load();
  }

  archive(product: Product) {
    if (!confirm(`Remove "${product.title}" from your shop?`)) {
      return;
    }
    this.sellerProducts.archive(product.id).subscribe(() => this.load());
  }

  logout() {
    this.auth.logout();
    this.router.navigate(['/']);
  }

  private load() {
    this.sellerProducts.mine().subscribe(products => {
      this.products = products;
      this.loading = false;
    });
  }
}
