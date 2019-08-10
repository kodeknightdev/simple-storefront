import { AfterViewInit, Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { Cart } from '../../core/models/cart';
import { CartService } from '../../core/services/cart.service';

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

  private stripe: any;
  private card: any;

  constructor(
    private fb: FormBuilder,
    private cartService: CartService
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
    // TODO: create order on the server + handleCardPayment
  }
}
