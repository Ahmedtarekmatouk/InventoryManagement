export interface Category {
  id: number;
  name: string;
  description: string | null;
  productCount: number;
  createdAt: string;
}

export interface CategoryRequest {
  name: string;
  description: string | null;
}