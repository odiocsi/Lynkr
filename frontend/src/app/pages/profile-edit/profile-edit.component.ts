import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgIf } from '@angular/common';
import { Router } from '@angular/router';

import { NzCardModule } from 'ng-zorro-antd/card';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzMessageService } from 'ng-zorro-antd/message';

import { UserService } from '../../services/user.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-edit-profile',
  standalone: true,
  imports: [FormsModule, NgIf, NzCardModule, NzInputModule, NzButtonModule],
  templateUrl: './profile-edit.component.html',
  styleUrls: ['./profile-edit.component.less']
})
export class EditProfileComponent implements OnInit {
  username = '';
  profilePictureUrl = '';

  saving = false;

  selectedFile?: File;
  uploading: boolean = false;

  constructor(
    private auth: AuthService,
    private userService: UserService,
    private msg: NzMessageService,
    private router: Router
  ) {}

  ngOnInit(): void {
    const storedUser = localStorage.getItem('user');
    if (storedUser) {
      try {
        const u = JSON.parse(storedUser);
        this.username = u.username ?? u.name ?? '';
        this.profilePictureUrl = u.profilePictureUrl ?? u.profilePicture ?? '';
      } catch {}
    }
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;

    const allowedTypes = ['image/jpeg', 'image/png', 'image/webp'];
    if (!allowedTypes.includes(file.type)) {
      this.msg.error('Only JPG, PNG, or WEBP images are allowed.');
      input.value = '';
      return;
    }

    const maxSize = 5 * 1024 * 1024;
    if (file.size > maxSize) {
      this.msg.error('Max file size is 5MB.');
      input.value = '';
      return;
    }

    this.selectedFile = file;
  }

  save() {
    if (!this.username && !this.selectedFile) {
      this.msg.warning('Nothing to update');
      return;
    }

    this.saving = true;

    this.userService.updateProfile(this.username, this.selectedFile).subscribe({
      next: (res) => {
        this.profilePictureUrl = res.profilePictureUrl ?? '';

        const storedUser = localStorage.getItem('user_info');
        let userObj: any = {};
        if (storedUser) {
          try { userObj = JSON.parse(storedUser); } catch { userObj = {}; }
        }

        userObj = {
          ...userObj,
          name: res.name ?? this.username,
          profilePic: res.profilePictureUrl ?? null
        };

        localStorage.setItem('user_info', JSON.stringify(userObj));

        this.auth.updateCurrentUer();

        this.msg.success('Profile updated');
        this.router.navigate(['/profile']);
      },
      error: (err) => {
        this.msg.error(err?.error?.message ?? 'Update failed');
      },
      complete: () => {
        this.saving = false;
      }
    });
  }

  cancel() {
    this.router.navigate(['/profile']);
  }
}
