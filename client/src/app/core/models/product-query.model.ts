export type ProductSortField = 'name' | 'price' | 'quantity' | 'createdAt';

export interface ProductQuery {
  searchTerm: string;
  categoryId: number | null;
  sortBy: ProductSortField;
  sortDescending: boolean;
  pageNumber: number;
  pageSize: number;
}

export const defaultProductQuery: ProductQuery = {
  searchTerm: '',
  categoryId: null,
  sortBy: 'name',
  sortDescending: false,
  pageNumber: 1,
  pageSize: 10
};