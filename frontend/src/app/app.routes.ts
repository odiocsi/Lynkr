import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./pages/login-page/login-page.component')
    .then(c => c.LoginPageComponent),
  },
  {
    path: 'signup',
    loadComponent: () => import('./pages/signup-page/signup-page.component')
    .then(c => c.SignupPageComponent),
  },
  {
    path: 'home',
    loadComponent: () => import('./pages/home/home.component')
    .then(c => c.HomeComponent),
    canActivate: [authGuard]
  },
  {
    path: 'friends',
    loadComponent: () => import('./pages/friends/friends.component')
    .then(c => c.FriendsComponent),
    canActivate: [authGuard]
  },
  {
    path: 'profile',
    loadComponent: () => import('./pages/profile-page/profile-page.component')
    .then(c => c.ProfilePageComponent),
    canActivate: [authGuard]
  },
  {
  path: 'profile/edit',
  loadComponent: () => import('./pages/profile-edit/profile-edit.component')
  .then( c=> c.EditProfileComponent),
  canActivate: [authGuard]
  },
  {
    path: 'chat/:friendId',
    loadComponent: () => import('./pages/chat/chat.component')
    .then( c=> c.ChatComponent),
    canActivate: [authGuard]
  },

  { path: '', redirectTo: 'home', pathMatch: 'full' },
  { path: '**', redirectTo: 'home' }
];
