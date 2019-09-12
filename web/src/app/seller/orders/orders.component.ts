import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';

import { SellerOrder } from '../../core/models/seller-order';
import { SellerProductService } from '../../core/services/seller-product.service';

@Component({
  selector: 'app-orders',
  templateUrl: './orders.component.html',
  styleUrls: ['./orders.component.scss']
})
export class OrdersComponent implements OnInit {
  orders$: Observable<SellerOrder[]>;

  constructor(private sellerProducts: SellerProductService) { }

  ngOnInit() {
    this.orders$ = this.sellerProducts.orders();
  }
}
