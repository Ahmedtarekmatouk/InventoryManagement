import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Category } from '../../../core/models/category.model';
import { PagedResult } from '../../../core/models/paged-result.model';
import { Product } from '../../../core/models/product.model';
import { defaultProductQuery, ProductQuery, ProductSortField } from '../../../core/models/product-query.model';
import { CategoryService } from '../../../core/services/category.service';
import { NotificationService } from '../../../core/services/notification.service';
import { ProductService } from '../../../core/services/product.service';
import { ConfirmDialogComponent } from '../../../shared/confirm-dialog/confirm-dialog';
import { PaginationComponent } from '../../../shared/pagination/pagination';
import { CurrencyPipe } from '@angular/common';
@Component({
  selector: 'app-product-list',
  imports: [CurrencyPipe,FormsModule, RouterLink, ConfirmDialogComponent, PaginationComponent],
  templateUrl: './product-list.html',
  styleUrl: './product-list.scss'
})
export class ProductListComponent implements OnInit {
  private readonly productService = inject(ProductService);
  private readonly categoryService = inject(CategoryService);
  private readonly notificationService = inject(NotificationService);

  private query: ProductQuery = { ...defaultProductQuery };

  readonly pagedProducts = signal<PagedResult<Product> | null>(null);
  readonly categories = signal<Category[]>([]);
  readonly isLoading = signal(false);
  readonly productPendingDeletion = signal<Product | null>(null);

  searchTerm = '';
  selectedCategoryId: number | null = null;

  ngOnInit(): void {
    this.loadCategories();
    this.loadProducts();
  }

  applyFilters(): void {
    this.query = {
      ...this.query,
      searchTerm: this.searchTerm,
      categoryId: this.selectedCategoryId,
      pageNumber: 1
    };

    this.loadProducts();
  }

  clearFilters(): void {
    this.searchTerm = '';
    this.selectedCategoryId = null;
    this.query = { ...defaultProductQuery };

    this.loadProducts();
  }

  sortBy(field: ProductSortField): void {
    this.query = {
      ...this.query,
      sortBy: field,
      sortDescending: this.query.sortBy === field ? !this.query.sortDescending : false,
      pageNumber: 1
    };

    this.loadProducts();
  }

  goToPage(pageNumber: number): void {
    this.query = { ...this.query, pageNumber };
    this.loadProducts();
  }

  requestDeletion(product: Product): void {
    this.productPendingDeletion.set(product);
  }

  cancelDeletion(): void {
    this.productPendingDeletion.set(null);
  }

  confirmDeletion(): void {
    const product = this.productPendingDeletion();

    if (!product) {
      return;
    }

    this.productService.deleteProduct(product.id).subscribe({
      next: () => {
        this.notificationService.showSuccess(`${product.name} was deleted.`);
        this.productPendingDeletion.set(null);
        this.loadProducts();
      },
      error: () => this.productPendingDeletion.set(null)
    });
  }

  isSortedBy(field: ProductSortField): boolean {
    return this.query.sortBy === field;
  }

  get sortDescending(): boolean {
    return this.query.sortDescending;
  }

  private loadProducts(): void {
    this.isLoading.set(true);

    this.productService.getProducts(this.query).subscribe({
      next: result => {
        this.pagedProducts.set(result);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  private loadCategories(): void {
    this.categoryService.getCategories().subscribe({
      next: categories => this.categories.set(categories)
    });
  }
}