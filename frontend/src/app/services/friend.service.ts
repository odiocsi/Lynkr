import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap, switchMap } from 'rxjs';
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

  private friendsSubject = new BehaviorSubject<Friend[]>([]);
  friends$ = this.friendsSubject.asObservable();

  private pendingSubject = new BehaviorSubject<PendingRequest[]>([]);
  pending$ = this.pendingSubject.asObservable();

  constructor(private http: HttpClient, private apiService: ApiService) {
    this.userUrl = this.apiService.API_URL + '/User';
    this.friendshipUrl = this.apiService.API_URL + '/Friendship';
  }

  loadFriends(): Observable<Friend[]> {
    return this.http.get<Friend[]>(`${this.friendshipUrl}/list`).pipe(
      tap(friends => this.friendsSubject.next(friends ?? []))
    );
  }

  loadPending(): Observable<PendingRequest[]> {
    return this.http.get<PendingRequest[]>(`${this.friendshipUrl}/pending`).pipe(
      tap(reqs => this.pendingSubject.next(reqs ?? []))
    );
  }

  sendFriendRequest(targetUserId: number): Observable<any> {
    return this.http.post(`${this.friendshipUrl}/request`, { targetUserId }, { responseType: 'text' });
  }

  // IMPORTANT: backend is PUT accept and expects { requesterId }
  acceptFriendRequest(requesterId: number): Observable<any> {
    return this.http.put(`${this.friendshipUrl}/accept`, { requesterId }, {responseType: 'text'}).pipe(
      switchMap(() => this.loadPending()), // refresh pending after accept
      tap(() => { this.loadFriends().subscribe(); }) // refresh friends too
    );
  }

  // IMPORTANT: backend is DELETE delete and expects body { otherUserId }
  removeFriend(otherUserId: number): Observable<any> {
    return this.http.delete(`${this.friendshipUrl}/delete`, {
      body: { otherUserId }
    }).pipe(
      switchMap(() => this.loadPending()),
      tap(() => { this.loadFriends().subscribe(); })
    );
  }

  searchUsers(query: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.userUrl}/search?query=${query}`);
  }
}
