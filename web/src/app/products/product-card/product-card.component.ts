import { Component, Input } from '@angular/core';

import { Product } from '../../core/models/product';

@Component({
  selector: 'app-product-card',
  templateUrl: './product-card.component.html',
  styleUrls: ['./product-card.component.scss']
})
export class ProductCardComponent {
  @Input() product: Product;

  readonly placeholder = 'assets/placeholder.svg';

  // seller pasted a dead link - don't show the broken image icon
  onImageError(event: Event) {
    const img = event.target as HTMLImageElement;
    if (!img.src.endsWith(this.placeholder)) {
      img.src = this.placeholder;
    }
  }
}
