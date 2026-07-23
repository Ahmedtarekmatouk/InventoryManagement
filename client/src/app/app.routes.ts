import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'products', pathMatch: 'full' },
  {
    path: 'products',
    loadComponent: () =>
      import('./features/products/product-list/product-list').then(m => m.ProductListComponent)
  },
  {
    path: 'products/new',
    loadComponent: () =>
      import('./features/products/product-form/product-form').then(m => m.ProductFormComponent)
  },
  {
    path: 'products/:id/edit',
    loadComponent: () =>
      import('./features/products/product-form/product-form').then(m => m.ProductFormComponent)
  },
  { path: '**', redirectTo: 'products' }
];