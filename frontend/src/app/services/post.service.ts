import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';

export interface Post {
  id: number;
  content: string;
  createdAt: string;
  userId: number;
  authorName: string;
  authorProfilePic: string | null;
  isLikedByCurrentUser: boolean;
  likeCount: number;
}

export interface PostCreateDto {
  content: string;
  imageUrl?: string | null;
}

@Injectable({
  providedIn: 'root'
})
export class PostService {
  private apiUrl: string;

  constructor(private http: HttpClient, private apiService: ApiService) {
    this.apiUrl = this.apiService.API_URL + "/Post"
  }

  // Fetches posts from the user and their friends
  getFeed(): Observable<Post[]> {
    return this.http.get<Post[]>(this.apiUrl + "/feed");
  }

  createPost(dto: PostCreateDto): Observable<any> {
    return this.http.post(this.apiUrl, dto );
  }

  LikePost(postId: number): Observable<{ isLiked: boolean; message: string }> {
  return this.http.post<{ isLiked: boolean; message: string }>(
    `${this.apiUrl}/${postId}/like`,
    {}
  );
}
}
