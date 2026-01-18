import { Component, OnInit} from '@angular/core';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzFlexModule } from 'ng-zorro-antd/flex';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

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
  templateUrl: './login-page.component.html',
  styleUrls: ['./login-page.component.less']
})
export class LoginPageComponent implements OnInit {
  loginForm!: FormGroup;

  // Inject AuthService and Router
  constructor(private fb: FormBuilder, private auth: AuthService, private router: Router) {}

  ngOnInit(): void {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });

    console.log(this.auth.currentUser());
  }

  onSubmit(): void {
    if (this.loginForm.valid) {
      this.auth.login(this.loginForm.value).subscribe({
        next: (response) => {
          console.log('Login successful!', response);
          this.router.navigate(['/home']); // Redirect after success
        },
        error: (err) => {
          console.error('Login failed', err);
          // You could add an error message to the UI here
        }
      });
    }
  }
}
