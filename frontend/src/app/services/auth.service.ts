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
  private apiUrl : string;
  currentUser = signal<UserInfo | null>(null);

  constructor(private http: HttpClient, private router: Router, private apiService: ApiService) {
    this.apiUrl = this.apiService.API_URL + "/User";
    const data = localStorage.getItem('user_info');
    if(data){
      this.updateCurrentUser(JSON.parse(data));
    } else{
      this.router.navigate(['/login']);
    }
  }

  // register user
  register(userData: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, userData);
  }

  // login with user
  login(credentials: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/login`, credentials).pipe(
      tap(response => {
        localStorage.setItem('auth_token', response.token);

        // save user info to local storage
        const userInfo: UserInfo = {
          userId: response.userId,
          name: response.name,
          profilePic: response.profilePic
        };

        this.updateCurrentUser(userInfo);
      })
    );
  }

  // logout => remove data from localstorage
  logout(): void {
    localStorage.removeItem('auth_token');
    localStorage.removeItem('user_info');
    this.currentUser.set(null);
    this.router.navigate(['/login'], { replaceUrl: true });
  }

  // update current user_info
  public updateCurrentUser( newUser: UserInfo): void {
    this.currentUser.set(newUser);
    localStorage.setItem('user_info', JSON.stringify(this.currentUser()))
  }

}
