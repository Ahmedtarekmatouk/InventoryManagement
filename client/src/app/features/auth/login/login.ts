import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
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
  private readonly router = inject(Router);

  readonly isSigningIn = signal(false);

async signIn(): Promise<void> {
    this.isSigningIn.set(true);

    try {
      await this.authService.login();
    } catch (error) {
      console.error('MSAL login failed', error);
      this.notificationService.showError('Sign in was cancelled or failed.');
      this.isSigningIn.set(false);
    }
  }
}
