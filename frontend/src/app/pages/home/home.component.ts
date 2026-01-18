import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { NzCardModule } from 'ng-zorro-antd/card';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzIconModule } from 'ng-zorro-antd/icon';

import { PostService, Post } from '../../services/post.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    NzCardModule,
    NzAvatarModule,
    NzButtonModule,
    NzInputModule,
    NzIconModule,
  ],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.less']
})
export class HomeComponent implements OnInit {
  posts = signal<Post[]>([]);
  newPostContent = '';

  constructor(private postService: PostService, private auth: AuthService) {}

  ngOnInit() {
    this.loadPosts();
  }

  loadPosts() {
    this.postService.getFeed().subscribe({
      next: (data) => this.posts.set(data),
      error: (err) => console.error('Error loading posts', err)
    });
  }

  submitPost() {
    if (this.newPostContent.trim()) {
      this.postService.createPost({content: this.newPostContent, imageUrl: "https://placehold.co/600x400?text=asd"}).subscribe(() => {
        this.newPostContent = '';
        this.loadPosts(); // Refresh feed
      });
    }
  }
}
