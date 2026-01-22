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
import { RouterLink, Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { NzMessageService } from 'ng-zorro-antd/message';

@Component({
  selector: 'app-signup',
  imports: [
    NzButtonModule,
    NzFormModule,
    NzInputModule,
    NzFlexModule,
    ReactiveFormsModule,
    RouterLink,
  ],
  standalone: true,
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.less'],
})
export class SignupComponent implements OnInit {
  signupForm!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private message: NzMessageService
  ) {}

  ngOnInit(): void {
    this.signupForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      name: ['', [Validators.required]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]],
    });
  }

  onSubmit(): void {
    if (this.signupForm.valid) {
      if(this.signupForm.value.password !== this.signupForm.value.confirmPassword){
        this.message.error('Password and it\'s confirmation must match!');
        return;
      }
      const { email, name, password } = this.signupForm.value;

      this.authService.register({ email, name, password }).subscribe({
        next: (data) => {
          console.log(data);
          this.message.success('Registration successful!');
          this.router.navigate(['/login']);
        },
        error: (err) => {
          this.message.error('Registration failed');
          console.error(err);
        },
      });
    }
  }
}
