// Disable the Place Order button on submit to prevent double-submit
document.getElementById('checkout-form').addEventListener('submit', function () {
    const btn = document.getElementById('place-order-btn');
    btn.disabled = true;
    btn.textContent = 'Placing order…';
});
