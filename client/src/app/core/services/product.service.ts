import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PagedResult } from '../models/paged-result.model';
import { Product, ProductRequest } from '../models/product.model';
import { ProductQuery } from '../models/product-query.model';

@Injectable({ providedIn: 'root' })
export class ProductService {
  private readonly http = inject(HttpClient);
  private readonly endpoint = `${environment.apiBaseUrl}/products`;

  getProducts(query: ProductQuery): Observable<PagedResult<Product>> {
    return this.http.get<PagedResult<Product>>(this.endpoint, {
      params: this.buildQueryParams(query)
    });
  }

  getProductById(id: number): Observable<Product> {
    return this.http.get<Product>(`${this.endpoint}/${id}`);
  }

  createProduct(request: ProductRequest): Observable<Product> {
    return this.http.post<Product>(this.endpoint, request);
  }

  updateProduct(id: number, request: ProductRequest): Observable<Product> {
    return this.http.put<Product>(`${this.endpoint}/${id}`, request);
  }

  deleteProduct(id: number): Observable<void> {
    return this.http.delete<void>(`${this.endpoint}/${id}`);
  }

  private buildQueryParams(query: ProductQuery): HttpParams {
    let params = new HttpParams()
      .set('pageNumber', query.pageNumber)
      .set('pageSize', query.pageSize)
      .set('sortBy', query.sortBy)
      .set('sortDescending', query.sortDescending);

    if (query.searchTerm.trim().length > 0) {
      params = params.set('searchTerm', query.searchTerm.trim());
    }

    if (query.categoryId !== null) {
      params = params.set('categoryId', query.categoryId);
    }

    return params;
  }
}