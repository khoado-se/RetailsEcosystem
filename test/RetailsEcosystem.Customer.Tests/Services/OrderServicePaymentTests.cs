using FluentAssertions;
using Moq;
using RetailsEcosystem.Customer.Application.Exceptions;
using RetailsEcosystem.Customer.Application.Interfaces;
using RetailsEcosystem.Customer.Application.Mappings;
using RetailsEcosystem.Customer.Application.Services;
using RetailsEcosystem.Customer.Domain.Entities;
using RetailsEcosystem.Customer.Domain.Interface;
using RetailsEcosystem.Customer.Shared.DTOs.Order;
using RetailsEcosystem.Customer.Shared.Enums;
using RetailsEcosystem.Customer.Tests.Helpers.Builders;

namespace RetailsEcosystem.Customer.Tests.Services;

public class OrderServicePaymentTests
{
    private readonly Mock<IOrderRepository>          _orderRepoMock   = new();
    private readonly Mock<ICartRepository>           _cartRepoMock    = new();
    private readonly Mock<IPaymentAttemptRepository> _attemptRepoMock = new();
    private readonly Mock<IVnpayService>             _vnpayServiceMock = new();
    private readonly Mock<IUnitOfWork>               _unitOfWorkMock  = new();
    private readonly OrderService _sut;

    public OrderServicePaymentTests()
    {
        _vnpayServiceMock
            .Setup(v => v.BuildPaymentUrl(It.IsAny<OrderDto>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns("https://sandbox.vnpay.vn/pay?mock=1");

        MappingConfig.Configure();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        _sut = new OrderService(
            _orderRepoMock.Object,
            _cartRepoMock.Object,
            _attemptRepoMock.Object,
            _vnpayServiceMock.Object,
            _unitOfWorkMock.Object);
    }

    // ── InitiateVnpayPaymentAsync ────────────────────────────────────────────

    [Fact]
    public async Task InitiateVnpayPayment_OrderNotFound_ThrowsNotFoundException()
    {
        _orderRepoMock.Setup(r => r.GetOrderByIdAsync(99)).ReturnsAsync((Order?)null);

        var act = () => _sut.InitiateVnpayPaymentAsync(99, "user-123", "127.0.0.1");

        await act.Should().ThrowAsync<NotFoundException>().WithMessage("*99*");
    }

    [Fact]
    public async Task InitiateVnpayPayment_WrongUser_ThrowsUnauthorizedAccessException()
    {
        var order = new OrderBuilder()
            .WithUserId("other-user")
            .WithPaymentMethod(PaymentMethod.VNPay)
            .Build();
        _orderRepoMock.Setup(r => r.GetOrderByIdAsync(1)).ReturnsAsync(order);

        var act = () => _sut.InitiateVnpayPaymentAsync(1, "user-123", "127.0.0.1");

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task InitiateVnpayPayment_CodOrder_ThrowsInvalidOperationException()
    {
        var order = new OrderBuilder()
            .WithUserId("user-123")
            .WithPaymentMethod(PaymentMethod.COD)
            .Build();
        _orderRepoMock.Setup(r => r.GetOrderByIdAsync(1)).ReturnsAsync(order);

        var act = () => _sut.InitiateVnpayPaymentAsync(1, "user-123", "127.0.0.1");

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*does not use VNPay*");
    }

    [Fact]
    public async Task InitiateVnpayPayment_AlreadyPaid_ThrowsInvalidOperationException()
    {
        var order = new OrderBuilder()
            .WithUserId("user-123")
            .WithPaymentMethod(PaymentMethod.VNPay)
            .WithPaymentStatus(PaymentStatus.Paid)
            .Build();
        _orderRepoMock.Setup(r => r.GetOrderByIdAsync(1)).ReturnsAsync(order);

        var act = () => _sut.InitiateVnpayPaymentAsync(1, "user-123", "127.0.0.1");

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*cannot be initiated*");
    }

    [Fact]
    public async Task InitiateVnpayPayment_MaxAttemptsReached_AutoCancelsAndThrows()
    {
        var order = new OrderBuilder()
            .WithUserId("user-123")
            .WithPaymentMethod(PaymentMethod.VNPay)
            .WithPaymentStatus(PaymentStatus.Failed)
            .WithPaymentAttemptCount(3)
            .WithStatus(OrderStatus.Pending)
            .Build();
        _orderRepoMock.Setup(r => r.GetOrderByIdAsync(1)).ReturnsAsync(order);

        var act = () => _sut.InitiateVnpayPaymentAsync(1, "user-123", "127.0.0.1");

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*maximum number of payment attempts*");

        order.Status.Should().Be(OrderStatus.Cancelled);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task InitiateVnpayPayment_MaxAttemptsAlreadyCancelled_ThrowsWithoutDoubleCancel()
    {
        var order = new OrderBuilder()
            .WithUserId("user-123")
            .WithPaymentMethod(PaymentMethod.VNPay)
            .WithPaymentStatus(PaymentStatus.Failed)
            .WithPaymentAttemptCount(3)
            .WithStatus(OrderStatus.Cancelled)
            .Build();
        _orderRepoMock.Setup(r => r.GetOrderByIdAsync(1)).ReturnsAsync(order);

        var act = () => _sut.InitiateVnpayPaymentAsync(1, "user-123", "127.0.0.1");

        await act.Should().ThrowAsync<InvalidOperationException>();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task InitiateVnpayPayment_AwaitingPaymentRetry_AbandonsPreviousInitiatedAttempt()
    {
        const string prevTxnRef = "1-20260101000000";
        var order = new OrderBuilder()
            .WithUserId("user-123")
            .WithPaymentMethod(PaymentMethod.VNPay)
            .WithPaymentStatus(PaymentStatus.AwaitingPayment)
            .WithVnpayTxnRef(prevTxnRef)
            .WithPaymentAttemptCount(1)
            .Build();

        var prevAttempt = new PaymentAttempt
        {
            TxnRef = prevTxnRef,
            Status = PaymentAttemptStatus.Initiated
        };

        _orderRepoMock.Setup(r => r.GetOrderByIdAsync(1)).ReturnsAsync(order);
        _attemptRepoMock.Setup(r => r.GetByTxnRefAsync(prevTxnRef)).ReturnsAsync(prevAttempt);
        _orderRepoMock.Setup(r => r.GetOrderByIdAsync(It.IsAny<int>())).ReturnsAsync(order);

        await _sut.InitiateVnpayPaymentAsync(1, "user-123", "127.0.0.1");

        prevAttempt.Status.Should().Be(PaymentAttemptStatus.Cancelled);
        prevAttempt.ResolvedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task InitiateVnpayPayment_ValidPendingOrder_CreatesAttemptAndReturnsUrl()
    {
        var order = new OrderBuilder()
            .WithUserId("user-123")
            .WithPaymentMethod(PaymentMethod.VNPay)
            .WithPaymentStatus(PaymentStatus.Pending)
            .WithPaymentAttemptCount(0)
            .Build();

        _orderRepoMock.Setup(r => r.GetOrderByIdAsync(1)).ReturnsAsync(order);

        var result = await _sut.InitiateVnpayPaymentAsync(1, "user-123", "127.0.0.1");

        result.PaymentUrl.Should().NotBeNullOrEmpty();
        result.TxnRef.Should().StartWith("1-");
        order.PaymentAttemptCount.Should().Be(1);
        order.PaymentStatus.Should().Be(PaymentStatus.AwaitingPayment);
        _attemptRepoMock.Verify(r => r.AddAsync(It.IsAny<PaymentAttempt>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    // ── ConfirmVnpayPaymentAsync ─────────────────────────────────────────────

    [Fact]
    public async Task ConfirmVnpayPayment_OrderNotFound_ThrowsNotFoundException()
    {
        _orderRepoMock.Setup(r => r.GetOrderByIdAsync(99)).ReturnsAsync((Order?)null);

        var act = () => _sut.ConfirmVnpayPaymentAsync(99, "txnno-123", "99-20260101");

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task ConfirmVnpayPayment_AlreadyPaid_IsIdempotentAndDoesNothing()
    {
        var order = new OrderBuilder()
            .WithPaymentStatus(PaymentStatus.Paid)
            .WithItem(qty: 2)
            .Build();
        _orderRepoMock.Setup(r => r.GetOrderByIdAsync(1)).ReturnsAsync(order);

        await _sut.ConfirmVnpayPaymentAsync(1, "txnno-123", "1-20260101");

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _cartRepoMock.Verify(r => r.GetByUserIdAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ConfirmVnpayPayment_Success_SetsStatusPaidDecrementsStockClearsCart()
    {
        var order = new OrderBuilder()
            .WithUserId("user-123")
            .WithPaymentMethod(PaymentMethod.VNPay)
            .WithPaymentStatus(PaymentStatus.AwaitingPayment)
            .WithVnpayTxnRef("1-20260101")
            .WithItem(qty: 3, productId: 1)
            .Build();

        // Attach a product to the item so stock can be decremented
        var product = new Helpers.Builders.ProductBuilder().WithStock(10).Build();
        order.Items.First().Product = product;

        var cart = new CartBuilder().WithUserId("user-123").Build();
        var attempt = new PaymentAttempt { TxnRef = "1-20260101", Status = PaymentAttemptStatus.Initiated };

        _orderRepoMock.Setup(r => r.GetOrderByIdAsync(1)).ReturnsAsync(order);
        _cartRepoMock.Setup(r => r.GetByUserIdAsync("user-123")).ReturnsAsync(cart);
        _attemptRepoMock.Setup(r => r.GetByTxnRefAsync("1-20260101")).ReturnsAsync(attempt);

        await _sut.ConfirmVnpayPaymentAsync(1, "vnpay-txn-001", "1-20260101");

        order.PaymentStatus.Should().Be(PaymentStatus.Paid);
        order.VnpayTransactionNo.Should().Be("vnpay-txn-001");
        product.StockQuantity.Should().Be(7);
        cart.Items.Should().BeEmpty();
        attempt.Status.Should().Be(PaymentAttemptStatus.Succeeded);
        attempt.VnpayTxnNo.Should().Be("vnpay-txn-001");
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ConfirmVnpayPayment_NullCart_CompletesWithoutError()
    {
        var order = new OrderBuilder()
            .WithUserId("user-123")
            .WithPaymentStatus(PaymentStatus.AwaitingPayment)
            .Build();

        _orderRepoMock.Setup(r => r.GetOrderByIdAsync(1)).ReturnsAsync(order);
        _cartRepoMock.Setup(r => r.GetByUserIdAsync("user-123")).ReturnsAsync((Cart?)null);
        _attemptRepoMock.Setup(r => r.GetByTxnRefAsync(It.IsAny<string>())).ReturnsAsync((PaymentAttempt?)null);

        var act = () => _sut.ConfirmVnpayPaymentAsync(1, "txnno", "1-20260101");

        await act.Should().NotThrowAsync();
        order.PaymentStatus.Should().Be(PaymentStatus.Paid);
    }

    [Fact]
    public async Task ConfirmVnpayPayment_AttemptAlreadySucceeded_DoesNotChangeAttemptStatus()
    {
        var order = new OrderBuilder()
            .WithUserId("user-123")
            .WithPaymentStatus(PaymentStatus.AwaitingPayment)
            .WithVnpayTxnRef("1-20260101")
            .Build();

        var attempt = new PaymentAttempt { TxnRef = "1-20260101", Status = PaymentAttemptStatus.Succeeded };

        _orderRepoMock.Setup(r => r.GetOrderByIdAsync(1)).ReturnsAsync(order);
        _cartRepoMock.Setup(r => r.GetByUserIdAsync(It.IsAny<string>())).ReturnsAsync((Cart?)null);
        _attemptRepoMock.Setup(r => r.GetByTxnRefAsync("1-20260101")).ReturnsAsync(attempt);

        await _sut.ConfirmVnpayPaymentAsync(1, "txnno", "1-20260101");

        attempt.Status.Should().Be(PaymentAttemptStatus.Succeeded);
        attempt.VnpayTxnNo.Should().BeNull();
    }

    // ── RecordPaymentOutcomeAsync ────────────────────────────────────────────

    [Fact]
    public async Task RecordPaymentOutcome_NotAwaitingPayment_IsIdempotentAndDoesNothing()
    {
        var order = new OrderBuilder()
            .WithPaymentStatus(PaymentStatus.Paid)
            .Build();
        _orderRepoMock.Setup(r => r.GetOrderByIdAsync(1)).ReturnsAsync(order);

        await _sut.RecordPaymentOutcomeAsync(1, "1-20260101", "00", PaymentAttemptStatus.Succeeded);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RecordPaymentOutcome_RspCode24_SetsCancelled()
    {
        var order = new OrderBuilder()
            .WithPaymentStatus(PaymentStatus.AwaitingPayment)
            .WithPaymentAttemptCount(1)
            .Build();
        var attempt = new PaymentAttempt { TxnRef = "1-20260101", Status = PaymentAttemptStatus.Initiated };

        _orderRepoMock.Setup(r => r.GetOrderByIdAsync(1)).ReturnsAsync(order);
        _attemptRepoMock.Setup(r => r.GetByTxnRefAsync("1-20260101")).ReturnsAsync(attempt);

        await _sut.RecordPaymentOutcomeAsync(1, "1-20260101", "24", PaymentAttemptStatus.Cancelled);

        order.PaymentStatus.Should().Be(PaymentStatus.Cancelled);
        attempt.Status.Should().Be(PaymentAttemptStatus.Cancelled);
    }

    [Fact]
    public async Task RecordPaymentOutcome_OtherFailure_SetsFailed()
    {
        var order = new OrderBuilder()
            .WithPaymentStatus(PaymentStatus.AwaitingPayment)
            .WithPaymentAttemptCount(1)
            .Build();
        var attempt = new PaymentAttempt { TxnRef = "1-20260101", Status = PaymentAttemptStatus.Initiated };

        _orderRepoMock.Setup(r => r.GetOrderByIdAsync(1)).ReturnsAsync(order);
        _attemptRepoMock.Setup(r => r.GetByTxnRefAsync("1-20260101")).ReturnsAsync(attempt);

        await _sut.RecordPaymentOutcomeAsync(1, "1-20260101", "11", PaymentAttemptStatus.Failed);

        order.PaymentStatus.Should().Be(PaymentStatus.Failed);
        attempt.Status.Should().Be(PaymentAttemptStatus.Failed);
    }

    [Fact]
    public async Task RecordPaymentOutcome_ThirdAttemptFailure_AutoCancelsOrder()
    {
        var order = new OrderBuilder()
            .WithStatus(OrderStatus.Pending)
            .WithPaymentStatus(PaymentStatus.AwaitingPayment)
            .WithPaymentAttemptCount(3)
            .Build();

        _orderRepoMock.Setup(r => r.GetOrderByIdAsync(1)).ReturnsAsync(order);
        _attemptRepoMock.Setup(r => r.GetByTxnRefAsync(It.IsAny<string>())).ReturnsAsync((PaymentAttempt?)null);

        await _sut.RecordPaymentOutcomeAsync(1, "1-20260101", "11", PaymentAttemptStatus.Failed);

        order.Status.Should().Be(OrderStatus.Cancelled);
    }

    [Fact]
    public async Task RecordPaymentOutcome_ThirdAttemptAlreadyCancelled_StatusUnchanged()
    {
        var order = new OrderBuilder()
            .WithStatus(OrderStatus.Cancelled)
            .WithPaymentStatus(PaymentStatus.AwaitingPayment)
            .WithPaymentAttemptCount(3)
            .Build();

        _orderRepoMock.Setup(r => r.GetOrderByIdAsync(1)).ReturnsAsync(order);
        _attemptRepoMock.Setup(r => r.GetByTxnRefAsync(It.IsAny<string>())).ReturnsAsync((PaymentAttempt?)null);

        await _sut.RecordPaymentOutcomeAsync(1, "1-20260101", "11", PaymentAttemptStatus.Failed);

        order.Status.Should().Be(OrderStatus.Cancelled);
    }

    // ── GetOrderByIdAsync — on-access expiry ─────────────────────────────────

    [Fact]
    public async Task GetOrderById_AwaitingPaymentExpired_SetsPaymentStatusExpiredAndResolvesAttempt()
    {
        const string txnRef = "1-20260101";
        var order = new OrderBuilder()
            .WithUserId("user-123")
            .WithPaymentStatus(PaymentStatus.AwaitingPayment)
            .WithVnpayTxnRef(txnRef)
            .WithPaymentExpiresAt(DateTime.UtcNow.AddMinutes(-1))
            .Build();

        var attempt = new PaymentAttempt { TxnRef = txnRef, Status = PaymentAttemptStatus.Initiated };

        _orderRepoMock.Setup(r => r.GetOrderByIdAsync(1)).ReturnsAsync(order);
        _attemptRepoMock.Setup(r => r.GetByTxnRefAsync(txnRef)).ReturnsAsync(attempt);

        var result = await _sut.GetOrderByIdAsync(1, "user-123", "Customer");

        result.PaymentStatus.Should().Be(PaymentStatus.Expired);
        attempt.Status.Should().Be(PaymentAttemptStatus.Expired);
        attempt.ResolvedAt.Should().NotBeNull();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetOrderById_AwaitingPaymentNotYetExpired_NoStatusChange()
    {
        var order = new OrderBuilder()
            .WithUserId("user-123")
            .WithPaymentStatus(PaymentStatus.AwaitingPayment)
            .WithPaymentExpiresAt(DateTime.UtcNow.AddMinutes(10))
            .Build();

        _orderRepoMock.Setup(r => r.GetOrderByIdAsync(1)).ReturnsAsync(order);

        var result = await _sut.GetOrderByIdAsync(1, "user-123", "Customer");

        result.PaymentStatus.Should().Be(PaymentStatus.AwaitingPayment);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetOrderById_PendingPaymentStatus_ExpiryLogicDoesNotRun()
    {
        var order = new OrderBuilder()
            .WithUserId("user-123")
            .WithPaymentStatus(PaymentStatus.Pending)
            .WithPaymentExpiresAt(DateTime.UtcNow.AddMinutes(-1))
            .Build();

        _orderRepoMock.Setup(r => r.GetOrderByIdAsync(1)).ReturnsAsync(order);

        await _sut.GetOrderByIdAsync(1, "user-123", "Customer");

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _attemptRepoMock.Verify(r => r.GetByTxnRefAsync(It.IsAny<string>()), Times.Never);
    }
}
