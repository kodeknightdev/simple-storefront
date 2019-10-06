import { Component, OnInit } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { switchMap } from 'rxjs/operators';

import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['../login/login.component.scss']
})
export class RegisterComponent implements OnInit {
  form: FormGroup;
  error: string;
  submitting = false;

  constructor(
    private fb: FormBuilder,
    private auth: AuthService,
    private router: Router
  ) { }

  ngOnInit() {
    this.form = this.fb.group({
      displayName: ['', [Validators.required, Validators.maxLength(100)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]]
    });
  }

  submit() {
    if (this.form.invalid) {
      return;
    }
    this.submitting = true;
    this.error = null;
    const { displayName, email, password } = this.form.value;

    // register, then log straight in
    this.auth.register(displayName, email, password).pipe(
      switchMap(() => this.auth.login(email, password))
    ).subscribe(
      () => this.router.navigate(['/seller']),
      (err: HttpErrorResponse) => {
        this.submitting = false;
        this.error = err.status === 409
          ? 'There is already a shop with that email.'
          : 'Could not create your shop, please try again.';
      }
    );
  }
}
