// Product listing page — Add to Cart.

$(document).on('click', '[data-action="add-to-cart"]', async function () {
    const productId = parseInt($(this).data('product-id'));
    const $btn = $(this);
    const original = $btn.text();
    $btn.prop('disabled', true).text('Adding…');

    try {
        const res = await fetch('/cart/items', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ productId, quantity: 1 })
        });
        if (res.ok) {
            const summary = await fetch('/cart/summary');
            if (summary.ok) renderHeaderCart(await summary.json());

            $btn.text('Added!');
            setTimeout(() => $btn.text(original).prop('disabled', false), 1500);
        } else {
            const err = await res.json().catch(() => ({}));
            showToast('danger', err.message || 'Could not add item.');
            $btn.text(original).prop('disabled', false);
        }
    } catch {
        $btn.text(original).prop('disabled', false);
    }
});
