import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { NotificationService } from '../services/notification.service';

export const errorInterceptor: HttpInterceptorFn = (request, next) => {
  const notificationService = inject(NotificationService);

  return next(request).pipe(
    catchError((error: HttpErrorResponse) => {
      notificationService.showError(extractMessage(error));
      return throwError(() => error);
    })
  );
};

function extractMessage(error: HttpErrorResponse): string {
  if (error.status === 0) {
    return 'Unable to reach the server. Please check that the API is running.';
  }

  if (error.error?.errors) {
    const validationMessages = Object.values(error.error.errors).flat() as string[];
    return validationMessages.join(' ');
  }

  return error.error?.message ?? 'An unexpected error occurred.';
}