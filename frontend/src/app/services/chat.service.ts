import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {
  HubConnection,
  HubConnectionBuilder,
  HubConnectionState
} from '@microsoft/signalr';
import { BehaviorSubject, Observable } from 'rxjs';
import { ApiService } from './api.service';

export interface ChatMessage {
  messageId: number;
  conversationId: number;
  senderId: number;
  senderName: string;
  senderProfilePictureUrl?: string | null;
  content: string;
  sentAt: string;
}

@Injectable({
  providedIn: 'root'
})
export class ChatService {

  private apiUrl : string;
  private hubUrl : string;

  private connection!: HubConnection;
  private startPromise: Promise<void> | null = null;

  private messagesSubject = new BehaviorSubject<ChatMessage[]>([]);
  messages$ = this.messagesSubject.asObservable();

  constructor(private http: HttpClient, private apiSerivce: ApiService) {
    this.apiUrl = this.apiSerivce.API_URL;
    this.hubUrl = this.apiSerivce.HUB_URL;
    this.createConnection();
  }

  // signalR setup
  private createConnection(): void {
    this.connection = new HubConnectionBuilder()
      .withUrl(this.hubUrl, {
        accessTokenFactory: () => localStorage.getItem('auth_token') ?? ''
      })
      .withAutomaticReconnect()
      .build();

    this.connection.on('ReceiveMessage', (message: ChatMessage) => {
      const current = this.messagesSubject.value;
      this.messagesSubject.next([...current, message]);
    });

    this.connection.onreconnected(() => {
      console.log('[SignalR] Reconnected');
    });

    this.connection.onclose(err => {
      console.error('[SignalR] Connection closed', err);
      this.startPromise = null;
    });
  }

  // check connection with hub
  private ensureConnection(): Promise<void> {
    if (this.connection.state === HubConnectionState.Connected) {
      return Promise.resolve();
    }

    if (!this.startPromise) {
      this.startPromise = this.connection.start()
        .then(() => console.log('[SignalR] Connected'))
        .catch(err => {
          console.error('[SignalR] Start failed', err);
          this.startPromise = null;
          throw err;
        });
    }

    return this.startPromise;
  }

  // Load Conversation or create if none existent
  getOrCreateConversation(friendUserId: string): Observable<{ conversationId: string }> {
    return this.http.get<{ conversationId: string }>(
      `${this.apiUrl}/conversations/with/${friendUserId}`
    );
  }

  // Load messages
  loadMessages(conversationId: string): Observable<ChatMessage[]> {
    return this.http.get<ChatMessage[]>(
      `${this.apiUrl}/conversations/${conversationId}/messages`
    );
  }

  // signalR
  async sendMessageToUser(recipientUserId: number, content: string): Promise<void> {
    await this.ensureConnection();

    if (this.connection.state !== HubConnectionState.Connected) return;

    await this.connection.invoke('SendDirectMessage', recipientUserId, content);
  }

  setMessages(messages: ChatMessage[]): void {
    this.messagesSubject.next(messages);
  }

}
