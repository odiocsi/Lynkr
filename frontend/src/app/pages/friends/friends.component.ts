import { Component, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';

import { FriendService, Friend, PendingRequest } from '../../services/friend.service';

import { NzTabsModule } from 'ng-zorro-antd/tabs';
import { NzListModule } from 'ng-zorro-antd/list';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzSegmentedModule } from 'ng-zorro-antd/segmented';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzMessageService } from 'ng-zorro-antd/message';

@Component({
  standalone: true,
  imports: [
    FormsModule,
    NzTabsModule,
    NzListModule,
    NzAvatarModule,
    NzInputModule,
    NzSegmentedModule,
    NzButtonModule
],
  templateUrl: './friends.component.html',
  styleUrls: ['./friends.component.less']
})
export class FriendsComponent implements OnInit {
  selectedTab: number = 0;

  friends = signal<Friend[]>([]);
  pendingRequests = signal<PendingRequest[]>([]);

  searchQuery : string = '';
  searchResults = signal<any[]>([]);


  constructor(
    private message: NzMessageService,
    private friendService: FriendService,
    private router: Router
  ) {}

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
      this.onSearch();
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
        this.selectedTab = 0;
        this.message.success('Friend request sent!');
      },
      error: err => this.message.error(err)
    });
  }

  acceptRequest(requesterId: number) {
    this.friendService.acceptFriendRequest(requesterId).subscribe({
    next: () => {
      this.loadPending();
      this.loadFriends();
      this.friendService.updateFriendsList();
      this.selectedTab = 0;
      this.message.success('You have accepted a friends request.');
    },
    error: err => this.message.error('You have failed to accept the friend request.')
  });
  }

  removeFriend(otherUserId: number) {
    this.friendService.removeFriend(otherUserId).subscribe({
      next: () => {
        this.friends.update(f =>
          f.filter(friend => friend.friendId !== otherUserId)
        );

        this.pendingRequests.update(p =>
          p.filter(req => req.requesterId !== otherUserId)
        );

        this.message.success('You have removed a friend.')
      },
      error: err => this.message.error('You have failed to remove a friend.')
    });
  }

  goToChat(friendId: number, friendName: string) {
    this.router.navigate(['/chat', friendId, {
      name: friendName
    }]);
  }
}
