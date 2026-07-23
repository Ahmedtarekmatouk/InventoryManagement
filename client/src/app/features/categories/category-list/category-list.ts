import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Category } from '../../../core/models/category.model';
import { CategoryService } from '../../../core/services/category.service';
import { NotificationService } from '../../../core/services/notification.service';
import { ConfirmDialogComponent } from '../../../shared/confirm-dialog/confirm-dialog';

@Component({
  selector: 'app-category-list',
  imports: [RouterLink, ConfirmDialogComponent],
  templateUrl: './category-list.html'
})
export class CategoryListComponent implements OnInit {
  private readonly categoryService = inject(CategoryService);
  private readonly notificationService = inject(NotificationService);

  readonly categories = signal<Category[]>([]);
  readonly isLoading = signal(false);
  readonly categoryPendingDeletion = signal<Category | null>(null);

  ngOnInit(): void {
    this.loadCategories();
  }

  requestDeletion(category: Category): void {
    this.categoryPendingDeletion.set(category);
  }

  cancelDeletion(): void {
    this.categoryPendingDeletion.set(null);
  }

  confirmDeletion(): void {
    const category = this.categoryPendingDeletion();

    if (!category) {
      return;
    }

    this.categoryService.deleteCategory(category.id).subscribe({
      next: () => {
        this.notificationService.showSuccess(category.name + ' was deleted.');
        this.categoryPendingDeletion.set(null);
        this.loadCategories();
      },
      error: () => this.categoryPendingDeletion.set(null)
    });
  }

  private loadCategories(): void {
    this.isLoading.set(true);

    this.categoryService.getCategories().subscribe({
      next: categories => {
        this.categories.set(categories);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }
}
