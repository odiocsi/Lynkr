import { Component, OnInit} from '@angular/core';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzFlexModule } from 'ng-zorro-antd/flex';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';

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
  styleUrls: ['./login.component.less']
})
export class LoginComponent implements OnInit {
  loginForm!: FormGroup;

  constructor(private fb: FormBuilder) {}

  ngOnInit(): void {
    this.loginForm = this.fb.group({
      // Define controls and initial values/validators
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  onSubmit(): void {
    if (this.loginForm.valid) {
      // The form value is a JavaScript object: {email: '...', password: '...'}
      console.log('Form Submitted!', this.loginForm.value);
      // e.g., Call an authentication service with this.loginForm.value
    } else {
      // Handle invalid form (e.g., highlight errors)
      console.log('Form is invalid.');
    }
  }



}
