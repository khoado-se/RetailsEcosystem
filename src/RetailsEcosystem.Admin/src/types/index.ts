export interface AuthUser {
  id: string;
  fullName: string;
  email: string;
  roles: string[];
}

export interface LoginCredentials {
  email: string;
  password: string;
}

export interface AuthResponse {
  accessToken: string;
  user: AuthUser;
}

export interface PagedResult<T> {
  items: T[];
  totalPage: number;
}

export interface CategoryDto {
  id: number;
  name: string;
  description: string;
}

export interface ProductImageDto {
  id: number;
  url: string;
  productId: number;
}

export interface ProductDto {
  id: number;
  name: string;
  description: string | null;
  price: number;
  isFeatured: boolean;
  imageUrl: string | null;
  imageUrls: string[];
  createdDate: string;
  category: { id: number; name: string } | null;
}

export interface OrderItemDto {
  id: number;
  productId: number;
  productName: string;
  productImageUrl: string | null;
  quantity: number;
  unitPrice: number;
  lineTotal: number;
}

export interface OrderDto {
  id: number;
  userId: string;
  userEmail: string;
  status: number;
  statusLabel?: string;
  paymentMethod: number;
  paymentStatus: number;
  vnpayTransactionNo?: string;
  shippingAddress: string;
  totalAmount: number;
  createdDate: string;
  items: OrderItemDto[];
}

export interface DailyRevenue {
  date: string;
  revenue: number;
}

export interface OrderStatsDto {
  ordersToday: number;
  revenueThisMonth: number;
  pendingOrders: number;
  dailyRevenue: DailyRevenue[];
}

export interface CustomerDto {
  id: string;
  fullName: string;
  email: string;
  phoneNumber: string | null;
  address: string | null;
  avatarUrl: string | null;
  isActive: boolean;
  roles: string[];
}
