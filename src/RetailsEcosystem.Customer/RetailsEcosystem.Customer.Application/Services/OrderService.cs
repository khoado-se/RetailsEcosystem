using Mapster;
using RetailsEcosystem.Customer.Application.Exceptions;
using RetailsEcosystem.Customer.Application.Interfaces;
using RetailsEcosystem.Customer.Domain.Entities;
using RetailsEcosystem.Customer.Domain.Interface;
using RetailsEcosystem.Customer.Shared;
using RetailsEcosystem.Customer.Shared.DTOs;
using RetailsEcosystem.Customer.Shared.DTOs.Order;
using RetailsEcosystem.Customer.Shared.Enums;

namespace RetailsEcosystem.Customer.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository           _orderRepo;
        private readonly ICartRepository            _cartRepo;
        private readonly IPaymentAttemptRepository  _attemptRepo;
        private readonly IVnpayService              _vnpayService;
        private readonly IUnitOfWork                _unitOfWork;

        public OrderService(
            IOrderRepository orderRepo,
            ICartRepository cartRepo,
            IPaymentAttemptRepository attemptRepo,
            IVnpayService vnpayService,
            IUnitOfWork unitOfWork)
        {
            _orderRepo    = orderRepo;
            _cartRepo     = cartRepo;
            _attemptRepo  = attemptRepo;
            _vnpayService = vnpayService;
            _unitOfWork   = unitOfWork;
        }

        public async Task<OrderDto> CreateOrderAsync(string userId, CreateOrderDto dto)
        {
            var cart = await _cartRepo.GetByUserIdAsync(userId);
            if (cart == null || !cart.Items.Any())
                throw new InvalidOperationException("Your cart is empty.");

            foreach (var item in cart.Items)
            {
                if (item.Product!.StockQuantity < item.Quantity)
                    throw new InvalidOperationException(
                        $"Insufficient stock for '{item.Product.Name}'. Available: {item.Product.StockQuantity}.");
            }

            var isVnpay = dto.PaymentMethod == PaymentMethod.VNPay;

            var order = new Order
            {
                UserId          = userId,
                ShippingAddress = dto.ShippingAddress,
                Status          = OrderStatus.Pending,
                PaymentMethod   = dto.PaymentMethod,
                PaymentStatus   = PaymentStatus.Pending,
                CreatedDate     = DateTime.UtcNow,
                UpdatedDate     = DateTime.UtcNow,
            };

            foreach (var item in cart.Items)
            {
                order.Items.Add(new OrderItem
                {
                    ProductId   = item.ProductId,
                    ProductName = item.Product!.Name,
                    UnitPrice   = item.UnitPrice,
                    Quantity    = item.Quantity,
                });

                // COD: commit stock immediately. VNPay: defer until IPN confirmation.
                if (!isVnpay)
                {
                    item.Product.StockQuantity -= item.Quantity;
                    item.Product.SoldCount     += item.Quantity;
                    item.Product.UpdatedDate    = DateTime.UtcNow;
                }
            }

            order.TotalAmount = order.Items.Sum(i => i.UnitPrice * i.Quantity);

            await _orderRepo.CreateOrderAsync(order);

            // COD: clear cart immediately. VNPay: defer until payment confirmed.
            if (!isVnpay)
                cart.Items.Clear();

            await _unitOfWork.SaveChangesAsync();

            return await GetOrderByIdAsync(order.Id, userId, "Customer");
        }

        public async Task<InitiatePaymentResult> InitiateVnpayPaymentAsync(int orderId, string userId, string ipAddress)
        {
            var order = await _orderRepo.GetOrderByIdAsync(orderId)
                ?? throw new NotFoundException($"Order {orderId} not found.");

            if (order.UserId != userId)
                throw new UnauthorizedAccessException("Access denied.");

            if (order.PaymentMethod != PaymentMethod.VNPay)
                throw new InvalidOperationException("This order does not use VNPay as the payment method.");

            var retryable = new[] {
                PaymentStatus.Pending,
                PaymentStatus.AwaitingPayment,
                PaymentStatus.Failed,
                PaymentStatus.Cancelled,
                PaymentStatus.Expired,
                PaymentStatus.Abandoned
            };
            if (!retryable.Contains(order.PaymentStatus))
                throw new InvalidOperationException($"Payment cannot be initiated for an order in {order.PaymentStatus} status.");

            if (order.PaymentAttemptCount >= 3)
            {
                if (order.Status == OrderStatus.Pending)
                {
                    order.Status      = OrderStatus.Cancelled;
                    order.UpdatedDate = DateTime.UtcNow;
                    await _unitOfWork.SaveChangesAsync();
                }
                throw new InvalidOperationException("This order has reached the maximum number of payment attempts (3).");
            }

            // Abandon the previous Initiated attempt if retrying from AwaitingPayment
            if (order.PaymentStatus == PaymentStatus.AwaitingPayment && order.VnpayTxnRef != null)
            {
                var prevAttempt = await _attemptRepo.GetByTxnRefAsync(order.VnpayTxnRef);
                if (prevAttempt?.Status == PaymentAttemptStatus.Initiated)
                {
                    prevAttempt.Status     = PaymentAttemptStatus.Cancelled;
                    prevAttempt.ResolvedAt = DateTime.UtcNow;
                }
            }

            var vnTime  = DateTime.UtcNow.AddHours(7);
            var txnRef  = $"{orderId}-{vnTime:yyyyMMddHHmmss}";
            var expires = vnTime.AddMinutes(15);

            var attempt = new PaymentAttempt
            {
                OrderId   = orderId,
                TxnRef    = txnRef,
                IpAddress = ipAddress,
                Status    = PaymentAttemptStatus.Initiated,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = expires.ToUniversalTime(),
            };
            await _attemptRepo.AddAsync(attempt);

            order.PaymentStatus        = PaymentStatus.AwaitingPayment;
            order.VnpayTxnRef          = txnRef;
            order.PaymentAttemptCount  += 1;
            order.LastPaymentAttemptAt = DateTime.UtcNow;
            order.PaymentExpiresAt     = expires.ToUniversalTime();
            order.UpdatedDate          = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync();

            var orderDto   = MapToDto(order);
            var paymentUrl = _vnpayService.BuildPaymentUrl(orderDto, txnRef, ipAddress);

            return new InitiatePaymentResult { PaymentUrl = paymentUrl, TxnRef = txnRef };
        }

        public async Task<PagedResult<OrderDto>> GetOrdersAsync(string userId, string role, PagedRequest pageRequest, OrderStatus? status = null)
        {
            IEnumerable<Order> orders;
            int totalCount;

            if (role == "Admin")
            {
                orders     = await _orderRepo.GetAllOrdersAsync(pageRequest.PageNumber, pageRequest.PageSize, status);
                totalCount = await _orderRepo.GetAllOrderCountAsync(status);
            }
            else
            {
                orders     = await _orderRepo.GetOrdersByUserIdAsync(userId, pageRequest.PageNumber, pageRequest.PageSize);
                totalCount = await _orderRepo.GetOrderCountByUserIdAsync(userId);
            }

            return new PagedResult<OrderDto>(orders.Select(MapToDto), pageRequest, totalCount);
        }

        public async Task<OrderDto> GetOrderByIdAsync(int orderId, string userId, string role)
        {
            var order = await _orderRepo.GetOrderByIdAsync(orderId)
                ?? throw new NotFoundException($"Order {orderId} not found.");

            if (role != "Admin" && order.UserId != userId)
                throw new UnauthorizedAccessException("Access denied.");

            // On-access expiry: if awaiting payment and URL has expired, mark Expired
            if (order.PaymentStatus == PaymentStatus.AwaitingPayment
                && order.PaymentExpiresAt.HasValue
                && order.PaymentExpiresAt.Value < DateTime.UtcNow)
            {
                order.PaymentStatus = PaymentStatus.Expired;
                order.UpdatedDate   = DateTime.UtcNow;

                var attempt = order.VnpayTxnRef != null
                    ? await _attemptRepo.GetByTxnRefAsync(order.VnpayTxnRef)
                    : null;
                if (attempt?.Status == PaymentAttemptStatus.Initiated)
                {
                    attempt.Status     = PaymentAttemptStatus.Expired;
                    attempt.ResolvedAt = DateTime.UtcNow;
                }

                await _unitOfWork.SaveChangesAsync();
            }

            return MapToDto(order);
        }

        public async Task<OrderDto> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto dto)
        {
            var order = await _orderRepo.GetOrderByIdAsync(orderId)
                ?? throw new NotFoundException($"Order {orderId} not found.");

            if (!IsValidTransition(order.Status, dto.Status))
                throw new InvalidOperationException($"Cannot transition order from {order.Status} to {dto.Status}.");

            order.Status      = dto.Status;
            order.UpdatedDate = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync();
            return MapToDto(order);
        }

        private static bool IsValidTransition(OrderStatus from, OrderStatus to) => (from, to) switch
        {
            (OrderStatus.Pending,   OrderStatus.Confirmed) => true,
            (OrderStatus.Pending,   OrderStatus.Cancelled) => true,
            (OrderStatus.Confirmed, OrderStatus.Shipped)   => true,
            (OrderStatus.Confirmed, OrderStatus.Cancelled) => true,
            (OrderStatus.Shipped,   OrderStatus.Delivered) => true,
            _ => false
        };

        public async Task<OrderDto> CancelOrderAsync(int orderId, string userId)
        {
            var order = await _orderRepo.GetOrderByIdAsync(orderId)
                ?? throw new NotFoundException($"Order {orderId} not found.");

            if (order.UserId != userId)
                throw new UnauthorizedAccessException("Access denied.");

            if (order.Status != OrderStatus.Pending)
                throw new InvalidOperationException("Only pending orders can be cancelled.");

            order.Status      = OrderStatus.Cancelled;
            order.UpdatedDate = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync();
            return MapToDto(order);
        }

        public async Task<OrderStatsDto> GetStatsAsync()
        {
            var ordersToday  = await _orderRepo.GetOrdersTodayCountAsync();
            var revenueMonth = await _orderRepo.GetRevenueThisMonthAsync();
            var pending      = await _orderRepo.GetPendingOrderCountAsync();
            var daily        = await _orderRepo.GetDailyRevenueAsync(7);

            return new OrderStatsDto
            {
                OrdersToday      = ordersToday,
                RevenueThisMonth = revenueMonth,
                PendingOrders    = pending,
                DailyRevenue     = daily,
            };
        }

        public async Task ConfirmVnpayPaymentAsync(int orderId, string vnpayTransactionNo, string txnRef)
        {
            var order = await _orderRepo.GetOrderByIdAsync(orderId)
                ?? throw new NotFoundException($"Order {orderId} not found.");

            // Idempotency: already confirmed
            if (order.PaymentStatus == PaymentStatus.Paid)
                return;

            // Decrement stock now that payment is confirmed
            foreach (var item in order.Items)
            {
                if (item.Product != null)
                {
                    item.Product.StockQuantity -= item.Quantity;
                    item.Product.SoldCount     += item.Quantity;
                    item.Product.UpdatedDate    = DateTime.UtcNow;
                }
            }

            // Clear the cart
            var cart = await _cartRepo.GetByUserIdAsync(order.UserId);
            if (cart != null)
                cart.Items.Clear();

            // Resolve the attempt record
            var attempt = await _attemptRepo.GetByTxnRefAsync(txnRef);
            if (attempt?.Status == PaymentAttemptStatus.Initiated)
            {
                attempt.Status      = PaymentAttemptStatus.Succeeded;
                attempt.VnpayTxnNo  = vnpayTransactionNo;
                attempt.ResolvedAt  = DateTime.UtcNow;
            }

            order.PaymentStatus      = PaymentStatus.Paid;
            order.VnpayTransactionNo = vnpayTransactionNo;
            order.UpdatedDate        = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RecordPaymentOutcomeAsync(int orderId, string txnRef, string responseCode, PaymentAttemptStatus attemptStatus)
        {
            var order = await _orderRepo.GetOrderByIdAsync(orderId)
                ?? throw new NotFoundException($"Order {orderId} not found.");

            // Idempotency: already resolved
            if (order.PaymentStatus != PaymentStatus.AwaitingPayment)
                return;

            var attempt = await _attemptRepo.GetByTxnRefAsync(txnRef);
            if (attempt?.Status == PaymentAttemptStatus.Initiated)
            {
                attempt.Status       = attemptStatus;
                attempt.ResponseCode = responseCode;
                attempt.ResolvedAt   = DateTime.UtcNow;
            }

            order.PaymentStatus = attemptStatus == PaymentAttemptStatus.Cancelled
                ? PaymentStatus.Cancelled
                : PaymentStatus.Failed;
            order.UpdatedDate = DateTime.UtcNow;

            // Auto-cancel when max attempts exhausted and no payment received
            if (order.PaymentAttemptCount >= 3 && order.Status == OrderStatus.Pending)
                order.Status = OrderStatus.Cancelled;

            await _unitOfWork.SaveChangesAsync();
        }

        public Task<PaymentAttempt?> GetPaymentAttemptByTxnRefAsync(string txnRef) =>
            _attemptRepo.GetByTxnRefAsync(txnRef);

        private static OrderDto MapToDto(Order order) => order.Adapt<OrderDto>();
    }
}
