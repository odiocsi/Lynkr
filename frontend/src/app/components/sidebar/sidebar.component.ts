import { Component, OnInit} from '@angular/core';
import { AsyncPipe } from '@angular/common';
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
    NzAvatarModule,
    AsyncPipe
],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.less'
})
export class SidebarComponent implements OnInit {
  isCollapsed = true;
  friends$: Observable<Friend[]>;

  constructor(public friendService: FriendService, private router: Router) {
    this.friends$ = this.friendService.friends$;
  }

  ngOnInit() {
    this.friendService.loadFriends().subscribe();
  }

  goToChat(friendId: number, friendName: string, profilePictureUrl: string | null) {
    this.router.navigate(['/chat', friendId, {
      name: friendName,
      profilePictureUrl: profilePictureUrl
    }]);
  }
}
