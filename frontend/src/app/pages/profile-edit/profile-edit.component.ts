import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgIf } from '@angular/common';
import { Router } from '@angular/router';

import { NzCardModule } from 'ng-zorro-antd/card';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzMessageService } from 'ng-zorro-antd/message';

import { UserService, UserUpdateDto } from '../../services/user.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-edit-profile',
  standalone: true,
  imports: [FormsModule, NzCardModule, NzInputModule, NzButtonModule],
  templateUrl: './profile-edit.component.html',
  styleUrls: ['./profile-edit.component.less']
})
export class EditProfileComponent implements OnInit {
  username = '';
  profilePictureUrl = '';

  saving = false;

  constructor(
    private auth: AuthService,
    private userService: UserService,
    private msg: NzMessageService,
    private router: Router
  ) {}

  ngOnInit(): void {
    // If you already have "get current user" endpoint, load it here.
    // If not, you can prefill from localStorage (if you store user there).
    const storedUser = localStorage.getItem('user');
    if (storedUser) {
      try {
        const u = JSON.parse(storedUser);
        this.username = u.username ?? u.name ?? '';
        this.profilePictureUrl = u.profilePictureUrl ?? u.profilePicture ?? '';
      } catch {}
    }
  }

  save() {
    const name = this.username.trim();
    const pic = this.profilePictureUrl.trim();

    if (!name) {
      this.msg.error('Name is required.');
      return;
    }

    this.saving = true;

    this.userService.updateProfile({ name, profilePictureUrl: pic || null }).subscribe({
      next: () => {
        // ✅ pull fresh user from backend
        this.userService.getMe().subscribe({
          next: (me) => {
            localStorage.setItem('user_info', JSON.stringify(me));
            this.auth.updateCurrentUer();
            console.log(me)
            this.msg.success('Profile updated!');
            this.router.navigate(['/profile']);
          },
          error: (err) => {
            console.error(err);
            // even if refresh fails, update succeeded
            this.msg.success('Profile updated!');
            this.router.navigate(['/profile']);
          }
        });
      },
      error: (err) => {
        console.error(err);
        this.msg.error('Could not update profile.');
        this.saving = false;
      },
      complete: () => (this.saving = false)
    });
  }

  cancel() {
    this.router.navigate(['/profile']);
  }
}
