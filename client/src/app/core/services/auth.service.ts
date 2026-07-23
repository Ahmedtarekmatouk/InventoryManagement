import { Injectable, signal } from '@angular/core';
import { AccountInfo, PublicClientApplication } from '@azure/msal-browser';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AuthService {
private readonly msalInstance = new PublicClientApplication({
    auth: {
      clientId: environment.auth.clientId,
      authority: environment.auth.authority,
      redirectUri: environment.auth.redirectUri
    },
    cache: {
      cacheLocation: 'sessionStorage'
    }
  });

  private readonly activeAccount = signal<AccountInfo | null>(null);

  readonly account = this.activeAccount.asReadonly();

  async initialize(): Promise<void> {
    await this.msalInstance.initialize();

    try {
      const redirectResult = await this.msalInstance.handleRedirectPromise();

      if (redirectResult?.account) {
        this.setActiveAccount(redirectResult.account);
        return;
      }
    } catch {
      await this.msalInstance.clearCache();
    }

    const [existingAccount] = this.msalInstance.getAllAccounts();

    if (existingAccount) {
      this.setActiveAccount(existingAccount);
    }
  }

  async login(): Promise<void> {
    await this.msalInstance.loginRedirect({
      scopes: [environment.auth.apiScope]
    });
  }

  async logout(): Promise<void> {
    this.setActiveAccount(null);
    await this.msalInstance.logoutRedirect();
  }

  isAuthenticated(): boolean {
    return this.activeAccount() !== null;
  }

  async acquireAccessToken(): Promise<string | null> {
    const account = this.activeAccount();

    if (!account) {
      return null;
    }

    const request = { scopes: [environment.auth.apiScope], account };

    try {
      const silentResult = await this.msalInstance.acquireTokenSilent(request);
      return silentResult.accessToken;
    } catch {
      await this.msalInstance.acquireTokenRedirect(request);
      return null;
    }
  }

  private setActiveAccount(account: AccountInfo | null): void {
    this.activeAccount.set(account);
    this.msalInstance.setActiveAccount(account);
  }
}
