// Product details page — gallery, quantity picker, and Add to Cart.

$(function () {
    // Gallery thumbnail click
    const $current = $('#current');
    if ($current.length) {
        const $thumbs = $('.pdp-thumb');
        $thumbs.first().addClass('pdp-thumb--active');
        $thumbs.on('click', function () {
            $thumbs.removeClass('pdp-thumb--active');
            $current.attr('src', $(this).attr('src'));
            $(this).addClass('pdp-thumb--active');
        });
    }
});

$(document).on('click', '[data-action="qty-dec"]', function () {
    adjustQty(-1);
});

$(document).on('click', '[data-action="qty-inc"]', function () {
    adjustQty(1);
});

$(document).on('click', '[data-action="add-to-cart"]', async function () {
    await addToCart(this);
});

function adjustQty(delta) {
    const $input = $('#quantity');
    if (!$input.length) return;
    const next = parseInt($input.val()) + delta;
    if (next >= parseInt($input.attr('min')) && next <= parseInt($input.attr('max'))) {
        $input.val(next);
    }
}

async function addToCart(btn) {
    const $btn = $(btn);
    const productId = parseInt($btn.data('product-id'));
    const qty = parseInt($('#quantity').val()) || 1;
    const original = $btn.text();
    $btn.prop('disabled', true).text('Adding…');

    try {
        const res = await fetch('/cart/items', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ productId, quantity: qty })
        });
        if (res.ok) {
            const summary = await fetch('/cart/summary');
            if (summary.ok) renderHeaderCart(await summary.json());
            showToast('success', 'Added ' + qty + ' item(s) to your cart.');
        } else {
            const err = await res.json().catch(() => ({}));
            showToast('danger', err.message || 'Could not add item.');
        }
    } catch {}

    $btn.text(original).prop('disabled', false);
}
