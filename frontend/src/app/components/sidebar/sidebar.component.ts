import { Component, inject, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';


import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { NzMenuModule } from 'ng-zorro-antd/menu';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';

import { Friend, FriendService } from '../../services/friend.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [
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
  friends: any;

  constructor(public friendService: FriendService, private router: Router) {
    this.friends = inject(this.friendService.friends);
  }
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
