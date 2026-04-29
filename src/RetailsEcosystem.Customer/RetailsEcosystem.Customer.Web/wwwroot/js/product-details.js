// Product details page — gallery, quantity picker, and Add to Cart.

// Gallery thumbnail click
document.addEventListener('DOMContentLoaded', () => {
    const current = document.getElementById('current');
    if (!current) return;
    const thumbnails = document.querySelectorAll('.images .img');
    if (thumbnails.length === 0) return;
    thumbnails.forEach(img => {
        img.addEventListener('click', e => {
            thumbnails.forEach(i => i.style.opacity = '1');
            current.src = e.target.src;
            e.target.style.opacity = '0.6';
        });
    });
});

document.addEventListener('click', async e => {
    if (e.target.closest('[data-action="qty-dec"]')) {
        adjustQty(-1);
    } else if (e.target.closest('[data-action="qty-inc"]')) {
        adjustQty(1);
    } else if (e.target.closest('[data-action="add-to-cart"]')) {
        await addToCart(e.target.closest('[data-action="add-to-cart"]'));
    }
});

function adjustQty(delta) {
    const input = document.getElementById('quantity');
    if (!input) return;
    const next = parseInt(input.value) + delta;
    if (next >= parseInt(input.min) && next <= parseInt(input.max)) {
        input.value = next;
    }
}

async function addToCart(btn) {
    const productId = parseInt(btn.dataset.productId);
    const qty = parseInt(document.getElementById('quantity')?.value) || 1;
    const original = btn.textContent;
    btn.disabled = true;
    btn.textContent = 'Adding…';

    try {
        const res = await fetch('/cart/items', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ productId, quantity: qty })
        });
        const alertEl = document.getElementById('details-alert');
        if (res.ok) {
            const data = await res.json();
            updateHeaderCartCount(data.itemCount);
            if (alertEl) {
                alertEl.className = 'alert alert-success';
                alertEl.textContent = 'Added ' + qty + ' item(s) to your cart.';
            }
        } else {
            const err = await res.json().catch(() => ({}));
            if (alertEl) {
                alertEl.className = 'alert alert-danger';
                alertEl.textContent = err.message || 'Could not add item.';
            }
        }
        if (alertEl) setTimeout(() => alertEl.className = 'alert d-none', 4000);
    } catch {}

    btn.textContent = original;
    btn.disabled = false;
}
