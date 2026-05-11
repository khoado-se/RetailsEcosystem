// Disable the Place Order button on submit to prevent double-submit
$(function () {
    $('#checkout-form').on('submit', function () {
        if (!$(this).valid()) return;
        var isVnpay = $('input[name="PaymentMethod"]:checked').val() === '1';
        var label   = isVnpay ? 'Processing your order…' : 'Placing order…';
        $('#place-order-btn').prop('disabled', true).text(label);
    });
});

// Highlight the selected payment option card
document.addEventListener('DOMContentLoaded', function () {
    var group = document.getElementById('paymentMethodGroup');
    if (!group) return;

    group.addEventListener('change', function (e) {
        if (e.target.type !== 'radio') return;
        group.querySelectorAll('.payment-option').forEach(function (el) {
            el.classList.remove('active', 'border-primary');
        });
        e.target.closest('.payment-option').classList.add('active', 'border-primary');
    });
});
