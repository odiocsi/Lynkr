import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';

export interface Friend {
  friendId: number;
  friendName: string;
  friendProfilePic: string | null;
}

export interface PendingRequest {
  requesterId: number;
  requesterName: string;
  requesterPic: string | null;
}

@Injectable({
  providedIn: 'root'
})
export class FriendService {
  private friendshipUrl: string;
  private userUrl: string;

  public friends = signal<Friend[]>([]);

  constructor(private http: HttpClient, private apiService: ApiService) {
    this.userUrl = this.apiService.API_URL + "/User";
    this.friendshipUrl = this.apiService.API_URL + "/Friendship";
  }


  getFriends(): Observable<Friend[]> {
    return this.http.get<Friend[]>(`${this.friendshipUrl}/list`);
  }

  getPendingRequests(): Observable<PendingRequest[]> {
    return this.http.get<PendingRequest[]>(`${this.friendshipUrl}/pending`);
  }

  sendFriendRequest(targetUserId: number): Observable<any> {
    return this.http.post(`${this.friendshipUrl}/request`, { targetUserId }, { responseType: 'text' });
  }

  acceptFriendRequest(requesterId: number): Observable<any> {
    return this.http.put(`${this.friendshipUrl}/accept`, { requesterId }, { responseType: 'text' });
  }

  removeFriend(otherUserId: number): Observable<any> {
    // Rejects or deletes friendship
    return this.http.delete(`${this.friendshipUrl}/delete`, { body: { otherUserId } });
  }

  searchUsers(query: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.userUrl}/search?query=${query}`);
  }
}
