// Disable the Place Order button on submit to prevent double-submit
$(function () {
    $('#checkout-form').on('submit', function () {
        if (!$(this).valid()) return;
        $('#place-order-btn').prop('disabled', true).text('Placing order…');
    });
});
