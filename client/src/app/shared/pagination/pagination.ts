import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-pagination',
  templateUrl: './pagination.html',
  styleUrl: './pagination.scss'
})
export class PaginationComponent {
  readonly pageNumber = input.required<number>();
  readonly totalPages = input.required<number>();
  readonly totalCount = input.required<number>();
  readonly hasPreviousPage = input.required<boolean>();
  readonly hasNextPage = input.required<boolean>();

  readonly pageChanged = output<number>();

  goToPreviousPage(): void {
    this.pageChanged.emit(this.pageNumber() - 1);
  }

  goToNextPage(): void {
    this.pageChanged.emit(this.pageNumber() + 1);
  }
}