import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { anonymousGuard } from './core/guards/anonymous.guard';
export const routes: Routes = [
  { path: '', redirectTo: 'products', pathMatch: 'full' },
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login/login').then(m => m.LoginComponent)
  },
  {
    path: 'products',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/products/product-list/product-list').then(m => m.ProductListComponent)
  },
  {
    path: 'products/new',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/products/product-form/product-form').then(m => m.ProductFormComponent)
  },
  {
    path: 'products/:id/edit',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/products/product-form/product-form').then(m => m.ProductFormComponent)
  },
  {
    path: 'categories',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/categories/category-list/category-list').then(m => m.CategoryListComponent)
  },
  {
    path: 'categories/new',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/categories/category-form/category-form').then(m => m.CategoryFormComponent)
  },
  {
    path: 'categories/:id/edit',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/categories/category-form/category-form').then(m => m.CategoryFormComponent)
  },
  { path: '**', redirectTo: 'products' },
  {
    path: 'login',
    canActivate: [anonymousGuard],
    loadComponent: () => import('./features/auth/login/login').then(m => m.LoginComponent)
  },
];