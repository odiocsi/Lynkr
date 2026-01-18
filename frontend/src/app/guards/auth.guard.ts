import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

function isTokenExpired(token: string): boolean {
  try {
    const payload = JSON.parse(atob(token.split('.')[1]));
    if (!payload.exp) return false;

    const expiry = payload.exp * 1000;
    return Date.now() > expiry;
  } catch {
    return true;
  }
}

export const authGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);

  const token = localStorage.getItem('auth_token');

  if (!token || isTokenExpired(token)) {
    localStorage.removeItem('auth_token');
    localStorage.removeItem('user_info');

    router.navigate(['/login'], {
      queryParams: { returnUrl: state.url }
    });

    return false;
  }

  return true;
};
