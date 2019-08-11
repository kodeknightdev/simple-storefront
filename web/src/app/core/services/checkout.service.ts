import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { CheckoutSession, OrderStatus } from '../models/order';

@Injectable({
  providedIn: 'root'
})
export class CheckoutService {
  private readonly baseUrl = `${environment.apiUrl}/checkout`;

  constructor(private http: HttpClient) { }

  start(cartId: string, email: string): Observable<CheckoutSession> {
    return this.http.post<CheckoutSession>(this.baseUrl, { cartId, email });
  }

  confirm(orderId: number): Observable<OrderStatus> {
    return this.http.post<OrderStatus>(`${this.baseUrl}/${orderId}/confirm`, {});
  }
}
