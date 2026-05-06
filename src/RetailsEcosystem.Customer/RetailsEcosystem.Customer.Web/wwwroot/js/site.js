// Site-wide utilities — available on every page.

const fmtCurrency = n =>
    new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND', maximumFractionDigits: 0 }).format(n);

function updateHeaderCartCount(count) {
    $('#header-cart-count, #header-cart-label').text(count);
}

function renderHeaderCart(data) {
    updateHeaderCartCount(data.itemCount ?? 0);

    $('#header-cart-list').html(
        data.items?.length
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
            : '<li class="text-center text-muted py-2 small">Your cart is empty.</li>'
    );

    $('#header-cart-total').text(fmtCurrency(data.total ?? 0));
}

$(function () {
    // User menu — click to open, click outside to close.
    $('.user-menu__trigger').on('click', function (e) {
        e.stopPropagation();
        $('.user-menu').toggleClass('is-open');
    });
    $(document).on('click', function () {
        $('.user-menu').removeClass('is-open');
    });

    // Sticky header compact state on scroll (hysteresis prevents flicker near threshold).
    const COLLAPSE_AT = 80;
    const EXPAND_AT = 56;
    let compact = false;
    function onScroll() {
        const y = $(window).scrollTop();
        if (!compact && y > COLLAPSE_AT) {
            compact = true;
            $('header.header').addClass('header--compact');
        } else if (compact && y < EXPAND_AT) {
            compact = false;
            $('header.header').removeClass('header--compact');
        }
    }
    $(window).on('scroll', onScroll);
    onScroll();
});

function showToast(type, message) {
    const $toast = $('#site-toast');
    $toast.removeClass('text-bg-success text-bg-danger text-bg-warning text-bg-info');
    $toast.addClass('text-bg-' + type);
    $('#site-toast-body').text(message);
    bootstrap.Toast.getOrCreateInstance($toast[0], { delay: 4000 }).show();
}

// Refresh header cart on every page load.
(async () => {
    try {
        const res = await fetch('/cart/summary');
        if (!res.ok) return;
        renderHeaderCart(await res.json());
    } catch {}
})();
