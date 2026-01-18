import { HttpInterceptorFn } from '@angular/common/http';

export interface UserInfo {
  userId: number;
  name: string;
  profilePic: string | null;
}

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  // Get the token from localStorage (where we saved it in AuthService)
  const token = localStorage.getItem('auth_token');

  // If the token exists, clone the request and add the header
  if (token) {
    const cloned = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
    return next(cloned);
  }

  // If no token, just pass the original request through
  return next(req);
};
