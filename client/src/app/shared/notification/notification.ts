import { Component, inject } from '@angular/core';
import { NotificationService } from '../../core/services/notification.service';

@Component({
  selector: 'app-notification',
  templateUrl: './notification.html',
  styleUrl: './notification.scss'
})
export class NotificationComponent {
  private readonly notificationService = inject(NotificationService);

  readonly notification = this.notificationService.notification;

  dismiss(): void {
    this.notificationService.dismiss();
  }
}