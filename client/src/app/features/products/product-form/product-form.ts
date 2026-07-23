import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Category } from '../../../core/models/category.model';
import { ProductRequest } from '../../../core/models/product.model';
import { CategoryService } from '../../../core/services/category.service';
import { NotificationService } from '../../../core/services/notification.service';
import { ProductService } from '../../../core/services/product.service';

@Component({
  selector: 'app-product-form',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './product-form.html',
})
export class ProductFormComponent implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly productService = inject(ProductService);
  private readonly categoryService = inject(CategoryService);
  private readonly notificationService = inject(NotificationService);
  private readonly router = inject(Router);
  private readonly activatedRoute = inject(ActivatedRoute);

  private productId: number | null = null;

  readonly categories = signal<Category[]>([]);
  readonly isSaving = signal(false);
  readonly isLoading = signal(false);

  readonly form = this.formBuilder.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(150)]],
    description: ['', Validators.maxLength(1000)],
    price: [0, [Validators.required, Validators.min(0.01)]],
    quantity: [0, [Validators.required, Validators.min(0)]],
    categoryId: [0, [Validators.required, Validators.min(1)]]
  });

  get isEditMode(): boolean {
    return this.productId !== null;
  }

  ngOnInit(): void {
    this.loadCategories();

    const routeId = this.activatedRoute.snapshot.paramMap.get('id');

    if (routeId) {
      this.productId = Number(routeId);
      this.loadProduct(this.productId);
    }
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSaving.set(true);

    const request = this.buildRequest();

    const save$ = this.isEditMode
      ? this.productService.updateProduct(this.productId!, request)
      : this.productService.createProduct(request);

    save$.subscribe({
      next: product => {
        this.notificationService.showSuccess(
          this.isEditMode ? `${product.name} was updated.` : `${product.name} was created.`
        );
        this.router.navigate(['/products']);
      },
      error: () => this.isSaving.set(false)
    });
  }

  hasError(controlName: string, errorName: string): boolean {
    const control = this.form.get(controlName);

    return control !== null && control.touched && control.hasError(errorName);
  }

  private buildRequest(): ProductRequest {
    const value = this.form.getRawValue();

    return {
      name: value.name.trim(),
      description: value.description.trim() || null,
      price: value.price,
      quantity: value.quantity,
      categoryId: value.categoryId
    };
  }

  private loadProduct(id: number): void {
    this.isLoading.set(true);

    this.productService.getProductById(id).subscribe({
      next: product => {
        this.form.patchValue({
          name: product.name,
          description: product.description ?? '',
          price: product.price,
          quantity: product.quantity,
          categoryId: product.categoryId
        });
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
        this.router.navigate(['/products']);
      }
    });
  }

  private loadCategories(): void {
    this.categoryService.getCategories().subscribe({
      next: categories => this.categories.set(categories)
    });
  }
}
