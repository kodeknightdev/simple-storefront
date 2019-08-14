import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-order-complete',
  templateUrl: './order-complete.component.html',
  styleUrls: ['./order-complete.component.scss']
})
export class OrderCompleteComponent implements OnInit {
  orderId: number;

  constructor(private route: ActivatedRoute) { }

  ngOnInit() {
    this.orderId = +this.route.snapshot.paramMap.get('orderId');
  }
}
