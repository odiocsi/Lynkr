import { Component, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';

import { CommonModule } from '@angular/common';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { NzMenuModule } from 'ng-zorro-antd/menu';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';

import { FriendService } from '../../services/friend.service';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [
    CommonModule,
    NzIconModule,
    NzLayoutModule,
    NzMenuModule,
    NzAvatarModule
  ],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.less'
})
export class SidebarComponent implements OnInit {
  isCollapsed = true;

  constructor(public friendService: FriendService, private router: Router) {}
  ngOnInit() {
    this.loadFriends();
  }

  loadFriends() {
    this.friendService.getFriends().subscribe({
      next: (res) => this.friendService.friends.set(res),
      error: (err) => console.error('Could not load friends', err)
    });
  }
  goToChat(friendId: number, friendName: string) {
    this.router.navigate(['/chat', friendId, {
      name: friendName
    }]);
  }
}
