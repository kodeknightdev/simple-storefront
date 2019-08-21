import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { Product } from '../models/product';

export interface ProductInput {
  title: string;
  description: string;
  imageUrl: string;
  category: string;
  price: number;
  quantity: number;
}

@Injectable({
  providedIn: 'root'
})
export class SellerProductService {
  private readonly baseUrl = `${environment.apiUrl}/seller/products`;

  constructor(private http: HttpClient) { }

  mine(): Observable<Product[]> {
    return this.http.get<Product[]>(this.baseUrl);
  }

  create(input: ProductInput): Observable<Product> {
    return this.http.post<Product>(this.baseUrl, input);
  }

  update(id: number, input: ProductInput): Observable<Product> {
    return this.http.put<Product>(`${this.baseUrl}/${id}`, input);
  }

  archive(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
