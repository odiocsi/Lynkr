import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';

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
  imports: [FormsModule, NzCardModule, NzInputModule, NzButtonModule],
  templateUrl: './profile-edit.component.html',
  styleUrls: ['./profile-edit.component.less']
})
export class EditProfileComponent implements OnInit {
  public user: any;
  saving = false;

  newUserName?: string;
  selectedFile?: File;

  constructor(
    private auth: AuthService,
    private userService: UserService,
    private message: NzMessageService,
    private router: Router
  ) {}

  ngOnInit(): void {
    const data = this.auth.currentUser();
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;

    const allowedTypes = ['image/jpeg', 'image/png', 'image/webp'];
    if (!allowedTypes.includes(file.type)) {
      this.message.error('Only JPG, PNG, or WEBP images are allowed.');
      input.value = '';
      return;
    }

    const maxSize = 5 * 1024 * 1024;
    if (file.size > maxSize) {
      this.message.error('Max file size is 5MB.');
      input.value = '';
      return;
    }

    this.selectedFile = file;
  }

  save() {
    if (!this.newUserName && !this.selectedFile) {
      this.message.warning('Nothing to update');
      return;
    }

    this.saving = true;

    this.userService.updateProfile(this.newUserName, this.selectedFile).subscribe({
      next: (res) => {
        const newUserName = res.name ?? null;
        const newUserProfilePictureUrl = res.profilePictureUrl ?? null;

        const storedUser = this.auth.currentUser()
        if(!storedUser){
          console.error("no current user");
          return;
        }

        const newUser = {
          userId: storedUser.userId,
          name: newUserName ?? this.user.name,
          profilePic: newUserProfilePictureUrl ?? this.user.profilePic
        };
        this.auth.updateCurrentUser(newUser)
        this.message.success('Profile updated');

        this.router.navigate(['/profile']);
      },
      error: (err) => {
        this.message.error(err?.error?.message ?? 'Update failed');
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
