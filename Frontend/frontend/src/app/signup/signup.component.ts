import { Component, OnInit} from '@angular/core';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzFlexModule } from 'ng-zorro-antd/flex';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';

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
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.less']
})
export class SignupComponent implements OnInit {
  signupForm!: FormGroup;

  constructor(private fb: FormBuilder) {}

  ngOnInit(): void {
    this.signupForm = this.fb.group({
      // Define controls and initial values/validators
      email: ['', [Validators.required, Validators.email]],
      name: ['', [Validators.required, Validators.minLength(6)]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  onSubmit(): void {
    if (this.signupForm.valid) {
      // The form value is a JavaScript object: {email: '...', password: '...'}
      console.log('Form Submitted!', this.signupForm.value);
      // e.g., Call an authentication service with this.signupForm.value
    } else {
      // Handle invalid form (e.g., highlight errors)
      console.log('Form is invalid.');
    }
  }



}
