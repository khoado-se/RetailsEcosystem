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
                    <div class="cart-img-head">
                        <div class="cart-img">
                            <img src="${i.productImageUrl || '/images/product-placeholder.jpg'}"
                                 alt="${i.productName}" />
                        </div>
                    </div>
                    <div class="content">
                        <h4><a href="/Products/ProductDetails/${i.id}">${i.productName}</a></h4>
                        <span class="quantity">${i.quantity} &times; ${fmtCurrency(i.unitPrice)}</span>
                    </div>
                </li>`).join('')
            : '<li class="text-center text-muted py-2 small">Your cart is empty.</li>';
    }

    if (total) total.textContent = fmtCurrency(data.total ?? 0);
}

// User menu — click to open, click outside to close.
(function () {
    const menu = document.querySelector('.user-menu');
    if (!menu) return;
    const trigger = menu.querySelector('.user-menu__trigger');
    if (!trigger) return;
    trigger.addEventListener('click', e => {
        e.stopPropagation();
        menu.classList.toggle('is-open');
    });
    document.addEventListener('click', () => menu.classList.remove('is-open'));
})();

// Sticky header compact state on scroll (hysteresis prevents flicker near threshold).
(function () {
    const header = document.querySelector('header.header');
    if (!header) return;
    const COLLAPSE_AT = 80;
    const EXPAND_AT = 56;
    let compact = false;
    const onScroll = () => {
        const y = window.scrollY;
        if (!compact && y > COLLAPSE_AT) {
            compact = true;
            header.classList.add('header--compact');
        } else if (compact && y < EXPAND_AT) {
            compact = false;
            header.classList.remove('header--compact');
        }
    };
    window.addEventListener('scroll', onScroll, { passive: true });
    onScroll();
})();

// Refresh header cart on every page load.
(async () => {
    try {
        const res = await fetch('/cart/summary');
        if (!res.ok) return;
        renderHeaderCart(await res.json());
    } catch {}
})();
