import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { Seller } from '../../core/models/seller';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['../product-form/product-form.component.scss']
})
export class ProfileComponent implements OnInit {
  form: FormGroup;
  seller: Seller;
  saving = false;
  saved = false;

  constructor(private fb: FormBuilder, private auth: AuthService) { }

  ngOnInit() {
    this.form = this.fb.group({
      displayName: ['', [Validators.required, Validators.maxLength(100)]],
      bio: ['', Validators.maxLength(1000)]
    });
    this.auth.me().subscribe(seller => {
      this.seller = seller;
      this.form.patchValue(seller);
    });
  }

  save() {
    if (this.form.invalid) {
      return;
    }
    this.saving = true;
    this.saved = false;
    const { displayName, bio } = this.form.value;
    this.auth.updateProfile(displayName, bio).subscribe(seller => {
      this.seller = seller;
      this.saving = false;
      this.saved = true;
    }, () => this.saving = false);
  }
}
