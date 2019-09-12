export interface SellerOrderItem {
  productId: number;
  title: string;
  quantity: number;
  unitPrice: number;
}

export interface SellerOrder {
  orderId: number;
  buyerEmail: string;
  paidAt: string;
  total: number;
  items: SellerOrderItem[];
}
