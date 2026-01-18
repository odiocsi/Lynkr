import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

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
  private friendshipUrl = 'http://localhost:5223/api/friendship';
  private userUrl = 'http://localhost:5223/api/user';

  public friends = signal<Friend[]>([]);

  constructor(private http: HttpClient) {}


  getFriends(): Observable<Friend[]> {
    return this.http.get<Friend[]>(`${this.friendshipUrl}/list`);
  }

  getPendingRequests(): Observable<PendingRequest[]> {
    return this.http.get<PendingRequest[]>(`${this.friendshipUrl}/pending`);
  }

  sendFriendRequest(targetUserId: number): Observable<any> {
    return this.http.post(`${this.friendshipUrl}/request`, { targetUserId });
  }

  acceptFriendRequest(requesterId: number): Observable<any> {
    return this.http.put(`${this.friendshipUrl}/accept`, { requesterId } );
  }

  removeFriend(otherUserId: number): Observable<any> {
    // Rejects or deletes friendship
    return this.http.delete(`${this.friendshipUrl}/delete`, { body: { otherUserId } });
  }

  searchUsers(query: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.userUrl}/search?query=${query}`);
  }
}
