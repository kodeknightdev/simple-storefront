import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { PagedResult, Product, Shop } from '../models/product';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private readonly baseUrl = `${environment.apiUrl}/products`;

  constructor(private http: HttpClient) { }

  list(page = 1, pageSize = 12, category?: string): Observable<PagedResult<Product>> {
    let params = new HttpParams()
      .set('page', String(page))
      .set('pageSize', String(pageSize));
    if (category) {
      params = params.set('category', category);
    }
    return this.http.get<PagedResult<Product>>(this.baseUrl, { params });
  }

  categories(): Observable<string[]> {
    return this.http.get<string[]>(`${this.baseUrl}/categories`);
  }

  get(id: number): Observable<Product> {
    return this.http.get<Product>(`${this.baseUrl}/${id}`);
  }

  shop(slug: string): Observable<Shop> {
    return this.http.get<Shop>(`${environment.apiUrl}/shops/${encodeURIComponent(slug)}`);
  }
}
