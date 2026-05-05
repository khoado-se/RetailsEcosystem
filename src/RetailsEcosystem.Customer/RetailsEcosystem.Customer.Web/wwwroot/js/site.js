// Site-wide utilities — available on every page.

const fmtCurrency = n =>
    new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND', maximumFractionDigits: 0 }).format(n);

function updateHeaderCartCount(count) {
    const countEl = document.getElementById('header-cart-count');
    const labelEl = document.getElementById('header-cart-label');
    if (countEl) countEl.textContent = count;
    if (labelEl) labelEl.textContent = count;
}

function renderHeaderCart(data) {
    updateHeaderCartCount(data.itemCount ?? 0);

    const list  = document.getElementById('header-cart-list');
    const total = document.getElementById('header-cart-total');

    if (list) {
        list.innerHTML = data.items?.length
            ? data.items.map(i => `
                <li>
                    <div class="shopping-img">
                        <img src="${i.productImageUrl || '/images/placeholder.png'}"
                             alt="${i.productName}" />
                    </div>
                    <div class="shopping-info">
                        <h6>${i.productName}</h6>
                        <span>${i.quantity} &times; ${fmtCurrency(i.unitPrice)}</span>
                    </div>
                </li>`).join('')
            : '<li class="text-center text-muted py-2 small">Your cart is empty.</li>';
    }

    if (total) total.textContent = fmtCurrency(data.total ?? 0);
}

// Refresh header cart on every page load.
(async () => {
    try {
        const res = await fetch('/cart/summary');
        if (!res.ok) return;
        renderHeaderCart(await res.json());
    } catch {}
})();
