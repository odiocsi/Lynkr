import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subscription, switchMap } from 'rxjs';

import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzCommentModule } from 'ng-zorro-antd/comment';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzListModule } from 'ng-zorro-antd/list';
import { NzDropdownModule } from 'ng-zorro-antd/dropdown';

import { FaIconComponent } from '@fortawesome/angular-fontawesome';
import { faPaperPlane } from '@fortawesome/free-solid-svg-icons';

import { AuthService } from '../../services/auth.service';
import { ChatService, ChatMessage } from '../../services/chat.service';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    FaIconComponent,
    NzCardModule,
    NzListModule,
    NzAvatarModule,
    NzCommentModule,
    NzInputModule,
    NzIconModule,
    NzButtonModule,
    NzDropdownModule
  ],
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.less']
})
export class ChatComponent implements OnInit, OnDestroy {
  paperPlane = faPaperPlane;

  messages: ChatMessage[] = [];
  newMessage: string = '';
  conversationId: string | null = null;
  friendId: number | null = null;
  currentUserId: number | null = null;
  friendName: string | null = null;
  profilePictureUrl: string | null = null;

  private msgSubscription?: Subscription;
  private routeSubscription?: Subscription;

  constructor(
    private chatService: ChatService,
    private auth: AuthService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit() {
    this.currentUserId = this.auth.currentUser()?.userId ?? null;
    this.route.paramMap.subscribe((p) => {
      console.log(p)
      this.friendName = p.get('name');
      this.profilePictureUrl = p.get('profilePictureUrl');
    });

    this.routeSubscription = this.route.paramMap
    .pipe(
      switchMap((params) => {
        const friendIdStr = params.get('friendId');
        this.friendId = friendIdStr ? Number(friendIdStr) : null;

        if (!this.friendId || Number.isNaN(this.friendId)) {
            throw new Error('Invalid friendId route parameter');
          }

          this.messages = [];
          this.conversationId = null;
          return this.chatService.getOrCreateConversation(this.friendId.toString());
        })
      )
      .subscribe({
        next: (res) => {
          this.conversationId = res.conversationId;

          // Load message history
          this.chatService.loadMessages(this.conversationId).subscribe({
            next: (msgs) => {
              const history = msgs || [];
              this.chatService.setMessages(history);
              this.messages = history;
            },
            error: (err) => console.error('Failed to load chat messages', err)
          });
        },
        error: (err) => {
          console.error(err);
          this.router.navigate(['/friends']);
        }
      });

    // Listen for new messages
    this.msgSubscription = this.chatService.messages$.subscribe((msgs) => {
      const convIdNum = this.conversationId ? Number(this.conversationId) : NaN;
      this.messages = msgs.filter(m => m.conversationId === convIdNum);
    });
  }

  async send() {
    const text = this.newMessage.trim();
    if (!text || !this.friendId) return;

    await this.chatService.sendMessageToUser(this.friendId, text);
    this.newMessage = '';
  }

  ngOnDestroy() {
    this.msgSubscription?.unsubscribe();
    this.routeSubscription?.unsubscribe();
  }
}
