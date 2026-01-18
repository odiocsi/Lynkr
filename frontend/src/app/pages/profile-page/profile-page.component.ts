import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { NzDescriptionsModule } from 'ng-zorro-antd/descriptions';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { AuthService } from '../../services/auth.service';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [
    CommonModule,
    NzCardModule,
    NzAvatarModule,
    NzDescriptionsModule,
    NzButtonModule,
    RouterLink
  ],
  templateUrl: './profile-page.component.html',
  styleUrl: './profile-page.component.less'
})
export class ProfilePageComponent {
  // Inject the AuthService to get current user data
  public auth = inject(AuthService);
}
