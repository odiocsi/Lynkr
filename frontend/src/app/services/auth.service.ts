import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { Router } from '@angular/router';
import { ApiService } from './api.service';

export interface UserInfo {
  userId: number;
  name: string;
  profilePic: string | null;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private apiUrl : string = "";
  // A signal to hold the current user info
  currentUser = signal<UserInfo | null>(this.getUserFromStorage());

  constructor(private http: HttpClient, private router: Router, private apiService: ApiService) {
    this.apiUrl = this.apiService.API_URL + "/User";
  }

  register(userData: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, userData);
  }

  login(credentials: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/login`, credentials).pipe(
      tap(response => {
        // Save everything to localStorage
        localStorage.setItem('auth_token', response.token);
        const userInfo: UserInfo = {
          userId: response.userId,
          name: response.name,
          profilePic: response.profilePic
        };
        localStorage.setItem('user_info', JSON.stringify(userInfo));

        // Update the signal so the whole app knows who is logged in
        this.currentUser.set(userInfo);
      })
    );
  }

  logout(): void {
    localStorage.removeItem('auth_token');
    localStorage.removeItem('user_info');
    this.currentUser.set(null);
    this.router.navigate(['/login'], { replaceUrl: true });
  }

  public updateCurrentUer(): void {
    this.currentUser = signal<UserInfo | null>(this.getUserFromStorage());
  }

  private getUserFromStorage(): UserInfo | null {
    const data = localStorage.getItem('user_info');
    return data ? JSON.parse(data) : null;
  }
}
