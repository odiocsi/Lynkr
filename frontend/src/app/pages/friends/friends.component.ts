import { Component, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';

import { FriendService } from '../../services/friend.service';

import { NzTabsModule } from 'ng-zorro-antd/tabs';
import { NzListModule } from 'ng-zorro-antd/list';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzSegmentedModule } from 'ng-zorro-antd/segmented';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzMessageService } from 'ng-zorro-antd/message';
import { AsyncPipe } from '@angular/common';
import { BehaviorSubject } from 'rxjs';

@Component({
  standalone: true,
  imports: [
    FormsModule,
    NzTabsModule,
    NzListModule,
    NzAvatarModule,
    NzInputModule,
    NzSegmentedModule,
    NzButtonModule,
    AsyncPipe
],
  templateUrl: './friends.component.html',
  styleUrls: ['./friends.component.less']
})
export class FriendsComponent implements OnInit {
  searchQuery : string = '';
  searchResults = signal<any[]>([]);

  selectedTab: number = 0 ;

  constructor(
    private message: NzMessageService,
    public friendService: FriendService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.refreshLists();
  }

  onTabChange(index: any) {
    this.selectedTab = index;
    this.refreshLists();
  }

  private refreshLists() {
    this.friendService.loadFriends().subscribe();
    this.friendService.loadPending().subscribe();
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
        this.searchQuery = '';
        this.searchResults.set([]);
        this.message.success('Friend request sent!');
      },
      error: err => {
        this.message.error(err)
        console.error(err)
      }

    });
  }

  acceptRequest(requesterId: number) {
    this.friendService.acceptFriendRequest(requesterId).subscribe({
      next: () => {
        this.message.success('You have accepted a friends request.');
      },
      error: (err) => {
        console.error(err)
        this.message.error('You have failed to accept the friend request.')
      }

    });
  }

  removeFriend(otherUserId: number) {
    this.friendService.removeFriend(otherUserId).subscribe({
      next: () => {
        if(this.selectedTab === 0) this.message.success('You have removed a friend.');
        if(this.selectedTab === 1) this.message.success('You have declined a friend request.');
      },
      error: () => this.message.error('Failed to remove friend.')
    })
  }


  goToChat(friendId: number, friendName: string, profilePictureUrl: string | null) {
    const data = {
      friendName: friendName,
      profilePictureUrl: profilePictureUrl
    }
    localStorage.setItem('chat_info', JSON.stringify(data));
    this.router.navigate(['/chat', friendId,]);
  }
}
