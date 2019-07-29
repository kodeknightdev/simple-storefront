import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable } from 'rxjs';

import { Product } from '../../core/models/product';
import { ProductService } from '../../core/services/product.service';

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss']
})
export class ProductListComponent implements OnInit {
  readonly pageSize = 12;

  products: Product[] = [];
  categories$: Observable<string[]>;
  category: string = null;
  page = 1;
  totalPages = 1;
  loading = false;

  constructor(
    private productService: ProductService,
    private route: ActivatedRoute,
    private router: Router
  ) { }

  ngOnInit() {
    this.categories$ = this.productService.categories();

    // keep filter + page in the url so back/refresh work
    this.route.queryParamMap.subscribe(params => {
      this.category = params.get('category');
      this.page = +params.get('page') || 1;
      this.load();
    });
  }

  selectCategory(category: string) {
    this.router.navigate([], { queryParams: { category, page: null } });
  }

  goToPage(page: number) {
    this.router.navigate([], { queryParams: { page }, queryParamsHandling: 'merge' });
  }

  private load() {
    this.loading = true;
    this.productService.list(this.page, this.pageSize, this.category).subscribe(result => {
      this.products = result.items;
      this.totalPages = Math.max(1, Math.ceil(result.totalCount / result.pageSize));
      this.loading = false;
    });
  }
}
