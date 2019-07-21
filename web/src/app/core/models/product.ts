export interface Product {
  id: number;
  title: string;
  description: string;
  imageUrl: string;
  category: string;
  price: number;
  quantity: number;
  sellerId: number;
  sellerName: string;
}

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
}
