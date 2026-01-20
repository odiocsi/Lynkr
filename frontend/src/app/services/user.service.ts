import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';

export interface UserUpdateDto {
  name: string;
  profilePictureUrl?: string | null;
}

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl: string = "";

  constructor(private http: HttpClient, private apiService: ApiService) {
    this.apiUrl = this.apiService.API_URL + "/User";
  }

  updateProfile(dto: UserUpdateDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/update`, dto);
  }

  getMe() {
    return this.http.get<{ userId: number; name: string; profilePictureUrl: string | null }>(
      `${this.apiUrl}/me`
    );
  }
}
