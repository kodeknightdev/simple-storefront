import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';

import { SellerRoutingModule } from './seller-routing.module';
import { LoginComponent } from './login/login.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { ProductFormComponent } from './product-form/product-form.component';
import { OrdersComponent } from './orders/orders.component';

@NgModule({
  declarations: [LoginComponent, DashboardComponent, ProductFormComponent, OrdersComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    SellerRoutingModule
  ]
})
export class SellerModule { }
