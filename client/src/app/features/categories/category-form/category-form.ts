import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CategoryRequest } from '../../../core/models/category.model';
import { CategoryService } from '../../../core/services/category.service';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  selector: 'app-category-form',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './category-form.html',
  styleUrl: './category-form.scss'
})
export class CategoryFormComponent implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly categoryService = inject(CategoryService);
  private readonly notificationService = inject(NotificationService);
  private readonly router = inject(Router);
  private readonly activatedRoute = inject(ActivatedRoute);

  private categoryId: number | null = null;

  readonly isSaving = signal(false);
  readonly isLoading = signal(false);

  readonly form = this.formBuilder.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(100)]],
    description: ['', Validators.maxLength(500)]
  });

  get isEditMode(): boolean {
    return this.categoryId !== null;
  }

  ngOnInit(): void {
    const routeId = this.activatedRoute.snapshot.paramMap.get('id');

    if (routeId) {
      this.categoryId = Number(routeId);
      this.loadCategory(this.categoryId);
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
      ? this.categoryService.updateCategory(this.categoryId!, request)
      : this.categoryService.createCategory(request);

    save$.subscribe({
      next: category => {
        this.notificationService.showSuccess(
          this.isEditMode ? category.name + ' was updated.' : category.name + ' was created.'
        );
        this.router.navigate(['/categories']);
      },
      error: () => this.isSaving.set(false)
    });
  }

  hasError(controlName: string, errorName: string): boolean {
    const control = this.form.get(controlName);

    return control !== null && control.touched && control.hasError(errorName);
  }

  private buildRequest(): CategoryRequest {
    const value = this.form.getRawValue();

    return {
      name: value.name.trim(),
      description: value.description.trim() || null
    };
  }

  private loadCategory(id: number): void {
    this.isLoading.set(true);

    this.categoryService.getCategoryById(id).subscribe({
      next: category => {
        this.form.patchValue({
          name: category.name,
          description: category.description ?? ''
        });
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
        this.router.navigate(['/categories']);
      }
    });
  }
}
