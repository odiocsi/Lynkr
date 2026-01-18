import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Post {
  id: number;
  content: string;
  createdAt: string;
  userId: number;
  userName: string;
  profilePictureUrl: string | null;
  likesCount: number;
}

export interface PostCreateDto {
  content: string;
  imageUrl?: string | null;
}

@Injectable({
  providedIn: 'root'
})
export class PostService {
  private apiUrl = 'http://localhost:5223/api/Post';

  constructor(private http: HttpClient) {}

  // Fetches posts from the user and their friends
  getFeed(): Observable<Post[]> {
    return this.http.get<Post[]>(this.apiUrl + "/feed");
  }

  createPost(dto: PostCreateDto): Observable<any> {
    return this.http.post(this.apiUrl, dto );
  }
}
