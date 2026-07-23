export interface Product {
  id: number;
  name: string;
  description: string | null;
  price: number;
  quantity: number;
  categoryId: number;
  categoryName: string;
  createdAt: string;
  updatedAt: string | null;
}

export interface ProductRequest {
  name: string;
  description: string | null;
  price: number;
  quantity: number;
  categoryId: number;
}