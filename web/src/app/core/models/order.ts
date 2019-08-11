export interface CheckoutSession {
  orderId: number;
  clientSecret: string;
  total: number;
  currency: string;
}

export interface OrderStatus {
  orderId: number;
  status: 'Pending' | 'Paid' | 'Failed';
  total: number;
}
