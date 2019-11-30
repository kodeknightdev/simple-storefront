import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { AuthGuard } from '../core/guards/auth.guard';
import { DashboardComponent } from './dashboard/dashboard.component';
import { LoginComponent } from './login/login.component';
import { OrdersComponent } from './orders/orders.component';
import { ProductFormComponent } from './product-form/product-form.component';
import { ProfileComponent } from './profile/profile.component';
import { RegisterComponent } from './register/register.component';

const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  {
    path: '',
    canActivate: [AuthGuard],
    children: [
      { path: 'orders', component: OrdersComponent },
      { path: 'profile', component: ProfileComponent },
      { path: 'products/new', component: ProductFormComponent },
      { path: 'products/:id/edit', component: ProductFormComponent },
      { path: '', component: DashboardComponent }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SellerRoutingModule { }
