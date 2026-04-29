// Product listing page — Add to Cart.

document.addEventListener('click', async e => {
    const btn = e.target.closest('[data-action="add-to-cart"]');
    if (!btn) return;

    const productId = parseInt(btn.dataset.productId);
    const original = btn.textContent;
    btn.disabled = true;
    btn.textContent = 'Adding…';

    try {
        const res = await fetch('/cart/items', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ productId, quantity: 1 })
        });
        if (res.ok) {
            const data = await res.json();
            updateHeaderCartCount(data.itemCount);
            btn.textContent = 'Added!';
            setTimeout(() => { btn.textContent = original; btn.disabled = false; }, 1500);
        } else {
            const err = await res.json().catch(() => ({}));
            alert(err.message || 'Could not add item.');
            btn.textContent = original;
            btn.disabled = false;
        }
    } catch {
        btn.textContent = original;
        btn.disabled = false;
    }
});
