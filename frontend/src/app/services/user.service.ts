import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface UserUpdateDto {
  name: string;
  profilePictureUrl?: string | null;
}

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly baseUrl = 'http://localhost:5223';

  constructor(private http: HttpClient) {}

  updateProfile(dto: UserUpdateDto): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/api/User/update`, dto);
  }

  getMe() {
    return this.http.get<{ userId: number; name: string; profilePictureUrl: string | null }>(
      `${this.baseUrl}/api/user/me`
    );
  }
}
