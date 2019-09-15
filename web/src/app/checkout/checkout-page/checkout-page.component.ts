import { AfterViewInit, Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { from, Observable } from 'rxjs';
import { switchMap } from 'rxjs/operators';

import { environment } from '../../../environments/environment';
import { Cart } from '../../core/models/cart';
import { CartService } from '../../core/services/cart.service';
import { CheckoutService } from '../../core/services/checkout.service';

// loaded from js.stripe.com in index.html
declare var Stripe: any;

@Component({
  selector: 'app-checkout-page',
  templateUrl: './checkout-page.component.html',
  styleUrls: ['./checkout-page.component.scss']
})
export class CheckoutPageComponent implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('cardElement', { static: true }) cardElement: ElementRef;

  cart$: Observable<Cart>;
  form: FormGroup;
  cardError: string;
  paying = false;

  private stripe: any;
  private card: any;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private cartService: CartService,
    private checkoutService: CheckoutService
  ) { }

  ngOnInit() {
    this.cart$ = this.cartService.get();
    this.form = this.fb.group({
      email: ['', [Validators.required, Validators.email]]
    });
  }

  ngAfterViewInit() {
    this.stripe = Stripe(environment.stripePublishableKey);
    const elements = this.stripe.elements();
    this.card = elements.create('card', {
      style: {
        base: { fontSize: '16px', color: '#3d3027' }
      }
    });
    this.card.mount(this.cardElement.nativeElement);
    this.card.on('change', event => {
      this.cardError = event.error ? event.error.message : null;
    });
  }

  ngOnDestroy() {
    if (this.card) {
      this.card.destroy();
    }
  }

  pay() {
    if (this.form.invalid) {
      return;
    }
    this.paying = true;
    this.cardError = null;

    let orderId: number;
    this.checkoutService.start(this.cartService.cartId, this.form.value.email).pipe(
      switchMap(session => {
        orderId = session.orderId;
        return from(this.stripe.handleCardPayment(session.clientSecret, this.card));
      })
    ).subscribe((result: any) => {
      if (result.error) {
        this.cardError = result.error.message;
        this.paying = false;
        return;
      }
      this.checkoutService.confirm(orderId).subscribe(status => {
        this.paying = false;
        if (status.status === 'Paid') {
          this.cartService.clear();
          this.router.navigate(['/checkout/complete', status.orderId]);
        }
      });
    }, err => {
      this.cardError = err.status === 409 && err.error
        ? err.error.message
        : 'Something went wrong starting checkout, please try again.';
      this.paying = false;
    });
  }
}
