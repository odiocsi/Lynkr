import { Component, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { NgIf, NgFor } from '@angular/common';
import { FriendService, Friend, PendingRequest } from '../../services/friend.service';

import { NzTabsModule } from 'ng-zorro-antd/tabs';
import { NzListModule } from 'ng-zorro-antd/list';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzSegmentedModule } from 'ng-zorro-antd/segmented';
import { NzButtonModule } from 'ng-zorro-antd/button';

@Component({
  standalone: true,
  imports:[
    FormsModule,
    NgIf,
    NgFor,
    NzTabsModule,
    NzListModule,
    NzAvatarModule,
    NzInputModule,
    NzSegmentedModule,
    NzButtonModule,
  ],
  templateUrl: './friends.component.html',
  styleUrls: ['./friends.component.less']
})
export class FriendsComponent implements OnInit {

  friends = signal<Friend[]>([]);
  pendingRequests = signal<PendingRequest[]>([]);
  searchResults = signal<any[]>([]);

  searchQuery = '';

  constructor(private friendService: FriendService, private router: Router) {}

  ngOnInit(): void {
    this.loadFriends();
    this.loadPending();
  }

  loadFriends() {
    this.friendService.getFriends().subscribe({
      next: (res) => this.friends.set(res),
      error: (err) => console.error('Could not load friends', err)
    });
  }

  loadPending() {
    this.friendService.getPendingRequests().subscribe({
      next: (res) => this.pendingRequests.set(res || []),
      error: (err) => console.error('getPendingRequests failed', err)
    });
  }

  onSearch() {
    const query = this.searchQuery.trim();
    if (!query) return;

    this.friendService.searchUsers(query).subscribe({
      next: res => this.searchResults.set(res || []),
      error: err => console.error('searchUsers failed', err)
    });
  }

  onQueryChange() {
    if (!this.searchQuery.trim()) {
      this.searchResults.set([]);
    }
  }

  sendRequest(targetUserId: number) {
    this.friendService.sendFriendRequest(targetUserId).subscribe({
      next: () => {
        this.searchResults.update(users =>
          users.filter(u => u.id !== targetUserId)
        );
        this.searchQuery = '';
        this.searchResults.set([]);
        this.router.navigate(["/friends"]);
      },
      error: err => console.error('sendFriendRequest failed', err)
    });
  }

  acceptRequest(requesterId: number) {
    this.friendService.acceptFriendRequest(requesterId).subscribe({
    next: () => {
      this.loadPending();
      this.loadFriends();
    },
    error: err => console.error('acceptFriendRequest failed', err)
  });
  }

  removeFriend(otherUserId: number) {
    this.friendService.removeFriend(otherUserId).subscribe({
      next: () => {
        // remove from friends (if present)
        this.friends.update(f =>
          f.filter(friend => friend.friendId !== otherUserId)
        );

        // remove from pending (if present)
        this.pendingRequests.update(p =>
          p.filter(req => req.requesterId !== otherUserId)
        );
      },
      error: err => console.error('removeFriend failed', err)
    });
  }

  goToChat(friendId: number, friendName: string) {
    this.router.navigate(['/chat', friendId, {
      name: friendName
    }]);
  }
}
