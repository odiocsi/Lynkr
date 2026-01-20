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

  updateProfile(username?: string, file?: File) {
    const form = new FormData();

    if (username && username.trim().length > 0) {
      form.append('username', username);
    }

    if (file) {
      form.append('file', file);
    }

    return this.http.put<{
      name: string;
      profilePictureUrl: string | null;
    }>(
      `${this.apiUrl}/update`,
      form
    );
  }
}
