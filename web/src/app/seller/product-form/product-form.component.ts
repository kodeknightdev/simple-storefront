import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { map } from 'rxjs/operators';

import { SellerProductService } from '../../core/services/seller-product.service';

@Component({
  selector: 'app-product-form',
  templateUrl: './product-form.component.html',
  styleUrls: ['./product-form.component.scss']
})
export class ProductFormComponent implements OnInit {
  form: FormGroup;
  productId: number;
  saving = false;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private sellerProducts: SellerProductService
  ) { }

  get isEdit(): boolean {
    return !!this.productId;
  }

  ngOnInit() {
    this.form = this.fb.group({
      title: ['', [Validators.required, Validators.maxLength(200)]],
      description: ['', Validators.maxLength(4000)],
      imageUrl: ['', Validators.maxLength(500)],
      category: ['', Validators.maxLength(50)],
      price: [null, [Validators.required, Validators.min(0.5)]],
      quantity: [1, [Validators.required, Validators.min(0)]]
    });

    this.productId = +this.route.snapshot.paramMap.get('id') || null;
    if (this.productId) {
      // no single-product seller endpoint, the list is small enough
      this.sellerProducts.mine().pipe(
        map(products => products.find(p => p.id === this.productId))
      ).subscribe(product => {
        if (product) {
          this.form.patchValue(product);
        }
      });
    }
  }

  save() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.saving = true;
    const request = this.isEdit
      ? this.sellerProducts.update(this.productId, this.form.value)
      : this.sellerProducts.create(this.form.value);

    request.subscribe(
      () => this.router.navigate(['/seller']),
      () => this.saving = false
    );
  }
}
