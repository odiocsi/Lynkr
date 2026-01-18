import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';

import { LoginPageComponent } from './pages/login-page/login-page.component';
import { FriendsComponent } from './pages/friends/friends.component';
import { ProfilePageComponent } from './pages/profile-page/profile-page.component';
import { HomeComponent } from './pages/home/home.component';
import { EditProfileComponent } from './pages/profile-edit/profile-edit.component';
import { ChatComponent } from './pages/chat/chat.component';

export const routes: Routes = [
  { path: 'login', component: LoginPageComponent },

  {
    path: 'home',
    component: HomeComponent,
    canActivate: [authGuard]
  },
  {
    path: 'friends',
    component: FriendsComponent,
    canActivate: [authGuard]
  },
  {
    path: 'profile',
    component: ProfilePageComponent,
    canActivate: [authGuard]
  },
  {
  path: 'profile/edit',
  component: EditProfileComponent,
  canActivate: [authGuard]
  },
  {
    path: 'chat/:friendId',
    component: ChatComponent,
    canActivate: [authGuard]
  },

  { path: '', redirectTo: 'home', pathMatch: 'full' },
  { path: '**', redirectTo: 'home' }
];
