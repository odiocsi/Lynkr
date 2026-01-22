import { Component, OnInit } from '@angular/core';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzFlexModule } from 'ng-zorro-antd/flex';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { NzMessageService } from 'ng-zorro-antd/message';

@Component({
  selector: 'app-login',
  imports: [
    NzButtonModule,
    NzFormModule,
    NzInputModule,
    NzFlexModule,
    ReactiveFormsModule,
    RouterLink,
  ],
  standalone: true,
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.less'],
})
export class LoginComponent implements OnInit {
  loginForm!: FormGroup;

  // Inject AuthService and Router
  constructor(
    private fb: FormBuilder,
    private auth: AuthService,
    private router: Router,
    private message: NzMessageService
  ) {}

  ngOnInit(): void {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
    });

    // console.log(this.auth.currentUser());
  }

  onSubmit(): void {
    if (this.loginForm.valid) {
      this.auth.login(this.loginForm.value).subscribe({
        next: (response) => {
          this.message.success('Log in successful!');
          console.log('Login response', response);
          this.router.navigate(['/home']);
        },
        error: (err) => {
          this.message.error('Invalid Credantials!');
          console.error('Login failed', err);
        },
      });
    }
  }
}
