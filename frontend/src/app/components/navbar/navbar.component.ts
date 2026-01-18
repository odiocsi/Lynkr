import { Component } from '@angular/core';

import { NgIf } from '@angular/common';
import { RouterLink } from '@angular/router';
import { NzFlexModule } from 'ng-zorro-antd/flex';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { NzPageHeaderModule } from 'ng-zorro-antd/page-header';

import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-navbar',
  imports: [NzAvatarModule, NzFlexModule, NgIf, NzPageHeaderModule, RouterLink],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.less',
})
export class NavbarComponent {

  constructor(public auth: AuthService){}
}
