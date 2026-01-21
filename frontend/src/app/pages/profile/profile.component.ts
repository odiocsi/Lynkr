import { Component, inject } from '@angular/core';

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
    NzCardModule,
    NzAvatarModule,
    NzDescriptionsModule,
    NzButtonModule,
    RouterLink
],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.less'
})
export class ProfileComponent {
  // Inject the AuthService to get current user data
  public auth = inject(AuthService);
}
