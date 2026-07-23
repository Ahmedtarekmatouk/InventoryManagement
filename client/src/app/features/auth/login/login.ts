import { Component, inject, signal } from '@angular/core';
import { AuthService } from '../../../core/services/auth.service';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class LoginComponent {
  private readonly authService = inject(AuthService);
  private readonly notificationService = inject(NotificationService);

  readonly isSigningIn = signal(false);

  async signIn(): Promise<void> {
    this.isSigningIn.set(true);

    try {
      await this.authService.login();
    } catch {
      this.notificationService.showError('Sign in was cancelled or failed.');
      this.isSigningIn.set(false);
    }
  }
}
