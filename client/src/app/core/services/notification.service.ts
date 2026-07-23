import { Injectable, signal } from '@angular/core';

export type NotificationType = 'success' | 'error';

export interface Notification {
  message: string;
  type: NotificationType;
}

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private readonly currentNotification = signal<Notification | null>(null);
  private hideTimeout?: ReturnType<typeof setTimeout>;

  readonly notification = this.currentNotification.asReadonly();

  showSuccess(message: string): void {
    this.show({ message, type: 'success' });
  }

  showError(message: string): void {
    this.show({ message, type: 'error' });
  }

  dismiss(): void {
    clearTimeout(this.hideTimeout);
    this.currentNotification.set(null);
  }

  private show(notification: Notification): void {
    clearTimeout(this.hideTimeout);
    this.currentNotification.set(notification);
    this.hideTimeout = setTimeout(() => this.currentNotification.set(null), 4000);
  }
}