// Order detail page — cancel order confirmation modal.

$(function () {
    $('#cancelOrderBtn').on('click', function () {
        $('#confirmCancelOrderModal').modal('show');
    });

    $('#confirmCancelOrderBtn').on('click', function () {
        $(this).prop('disabled', true);
        $('#cancelOrderForm').trigger('submit');
    });
});
