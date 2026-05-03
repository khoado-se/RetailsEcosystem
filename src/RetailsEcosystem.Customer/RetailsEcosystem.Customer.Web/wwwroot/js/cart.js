// Cart page — quantity controls, remove, and summary refresh.

const fmt = n => new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(n);

let _qtyAbortController = null;

document.addEventListener('click', async e => {
    const decBtn = e.target.closest('[data-action="qty-dec"]');
    const incBtn = e.target.closest('[data-action="qty-inc"]');
    const removeBtn = e.target.closest('[data-action="remove-item"]');

    if (decBtn) {
        const id = decBtn.dataset.itemId;
        const current = parseInt(document.getElementById('qty-' + id).value);
        await updateQty(id, current - 1, decBtn);
    } else if (incBtn) {
        const id = incBtn.dataset.itemId;
        const current = parseInt(document.getElementById('qty-' + id).value);
        await updateQty(id, current + 1, incBtn);
    } else if (removeBtn) {
        await removeItem(removeBtn.dataset.itemId, removeBtn);
    }
});

document.addEventListener('change', async e => {
    const input = e.target.closest('[data-action="qty-change"]');
    if (input) await updateQty(input.dataset.itemId, parseInt(input.value), null);
});

async function updateQty(itemId, qty, triggerBtn) {
    if (qty < 1) return;

    if (_qtyAbortController) _qtyAbortController.abort();
    _qtyAbortController = new AbortController();

    if (triggerBtn) triggerBtn.disabled = true;
    try {
        const res = await fetch('/cart/items/' + itemId, {
            method: 'PUT',
            signal: _qtyAbortController.signal,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ quantity: qty })
        });
        if (!res.ok) {
            const err = await res.json().catch(() => ({}));
            showAlert('danger', err.message || 'Failed to update quantity.');
            return;
        }
        refreshCartUI(await res.json());
    } catch (err) {
        if (err.name !== 'AbortError') showAlert('danger', 'Failed to update quantity.');
    } finally {
        if (triggerBtn) triggerBtn.disabled = false;
        _qtyAbortController = null;
    }
}

async function removeItem(itemId, btn) {
    btn.disabled = true;
    try {
        const res = await fetch('/cart/items/' + itemId, { method: 'DELETE' });
        if (!res.ok) { showAlert('danger', 'Failed to remove item.'); return; }
        const cart = await res.json();
        document.getElementById('cart-row-' + itemId)?.remove();
        refreshCartUI(cart);
        if (cart.items.length === 0) location.reload();
    } catch {
        showAlert('danger', 'Failed to remove item. Please try again.');
    } finally {
        btn.disabled = false;
    }
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
    el.classList.remove('d-none');
    setTimeout(() => el.classList.add('d-none'), 4000);
}
