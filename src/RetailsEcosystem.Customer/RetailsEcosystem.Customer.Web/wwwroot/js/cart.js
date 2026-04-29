// Cart page — quantity controls, remove, and summary refresh.

const fmt = n => new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(n);

document.addEventListener('click', async e => {
    const decBtn = e.target.closest('[data-action="qty-dec"]');
    const incBtn = e.target.closest('[data-action="qty-inc"]');
    const removeBtn = e.target.closest('[data-action="remove-item"]');

    if (decBtn) {
        const id = decBtn.dataset.itemId;
        const current = parseInt(document.getElementById('qty-' + id).value);
        await updateQty(id, current - 1);
    } else if (incBtn) {
        const id = incBtn.dataset.itemId;
        const current = parseInt(document.getElementById('qty-' + id).value);
        await updateQty(id, current + 1);
    } else if (removeBtn) {
        await removeItem(removeBtn.dataset.itemId);
    }
});

document.addEventListener('change', async e => {
    const input = e.target.closest('[data-action="qty-change"]');
    if (input) await updateQty(input.dataset.itemId, parseInt(input.value));
});

async function updateQty(itemId, qty) {
    if (qty < 1) return;
    const res = await fetch('/cart/items/' + itemId, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ quantity: qty })
    });
    if (!res.ok) {
        const err = await res.json().catch(() => ({}));
        showAlert('danger', err.message || 'Failed to update quantity.');
        return;
    }
    refreshCartUI(await res.json());
}

async function removeItem(itemId) {
    const res = await fetch('/cart/items/' + itemId, { method: 'DELETE' });
    if (!res.ok) { showAlert('danger', 'Failed to remove item.'); return; }
    const cart = await res.json();
    document.getElementById('cart-row-' + itemId)?.remove();
    refreshCartUI(cart);
    if (cart.items.length === 0) location.reload();
}

function refreshCartUI(cart) {
    let total = 0, count = 0;
    cart.items.forEach(i => {
        const line = i.unitPrice * i.quantity;
        total += line;
        count += i.quantity;
        const qtyEl = document.getElementById('qty-' + i.id);
        const lineEl = document.getElementById('line-' + i.id);
        if (qtyEl) qtyEl.value = i.quantity;
        if (lineEl) lineEl.textContent = fmt(line);
    });
    const summaryCount = document.getElementById('summary-count');
    const summaryTotal = document.getElementById('summary-total');
    const grandTotal = document.getElementById('grand-total');
    if (summaryCount) summaryCount.textContent = count;
    if (summaryTotal) summaryTotal.textContent = fmt(total);
    if (grandTotal) grandTotal.textContent = fmt(total);
    updateHeaderCartCount(count);
}

function showAlert(type, msg) {
    const el = document.getElementById('cart-alert');
    if (!el) return;
    el.className = 'alert alert-' + type;
    el.textContent = msg;
    setTimeout(() => el.classList.add('d-none'), 4000);
}
