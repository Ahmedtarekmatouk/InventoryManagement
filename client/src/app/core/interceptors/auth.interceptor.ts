import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { from, switchMap } from 'rxjs';
import { AuthService } from '../services/auth.service';

export const authInterceptor: HttpInterceptorFn = (request, next) => {
  const authService = inject(AuthService);

  return from(authService.acquireAccessToken()).pipe(
    switchMap(accessToken => {
      if (!accessToken) {
        return next(request);
      }

      const authorizedRequest = request.clone({
        setHeaders: { Authorization: `Bearer ${accessToken}` }
      });

      return next(authorizedRequest);
    })
  );
};