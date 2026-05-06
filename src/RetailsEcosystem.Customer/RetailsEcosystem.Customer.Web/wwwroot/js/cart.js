// Cart page — quantity controls, remove, clear, and summary refresh.

let _qtyAbortController = null;
let _pendingRemoveId = null;

$(document).on('click', '[data-action="qty-dec"]', async function () {
    const id = $(this).data('item-id');
    const current = parseInt($('#qty-' + id).val());
    if (current - 1 < 1) {
        _pendingRemoveId = id;
        $('#confirmRemoveModal').modal('show');
        return;
    }
    await updateQty(id, current - 1, this);
});

$(document).on('click', '[data-action="qty-inc"]', async function () {
    const id = $(this).data('item-id');
    const current = parseInt($('#qty-' + id).val());
    await updateQty(id, current + 1, this);
});

$(document).on('click', '[data-action="remove-item"]', function () {
    _pendingRemoveId = $(this).data('item-id');
    $('#confirmRemoveModal').modal('show');
});

$(document).on('change', '[data-action="qty-change"]', async function () {
    const id = $(this).data('item-id');
    const qty = parseInt($(this).val());
    if (qty < 1) {
        $('#qty-' + id).val(1);
        _pendingRemoveId = id;
        $('#confirmRemoveModal').modal('show');
        return;
    }
    await updateQty(id, qty, null);
});

$(document).on('click', '#confirmRemoveBtn', async function () {
    if (_pendingRemoveId === null) return;
    const id = _pendingRemoveId;
    $(this).prop('disabled', true);
    $('#confirmRemoveModal').modal('hide');
    await removeItem(id, this);
    _pendingRemoveId = null;
});

$('#confirmRemoveModal').on('hidden.bs.modal', function () {
    $('#confirmRemoveBtn').prop('disabled', false);
});

$(document).on('click', '#clearCartBtn', function () {
    $('#confirmClearModal').modal('show');
});

$(document).on('click', '#confirmClearBtn', function () {
    $('#clearCartForm').trigger('submit');
});

async function updateQty(itemId, qty, triggerBtn) {
    if (qty < 1) return;

    if (_qtyAbortController) _qtyAbortController.abort();
    _qtyAbortController = new AbortController();

    if (triggerBtn) $(triggerBtn).prop('disabled', true);
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
        if (triggerBtn) $(triggerBtn).prop('disabled', false);
        _qtyAbortController = null;
    }
}

async function removeItem(itemId, btn) {
    $(btn).prop('disabled', true);
    try {
        const res = await fetch('/cart/items/' + itemId, { method: 'DELETE' });
        if (!res.ok) { showAlert('danger', 'Failed to remove item.'); return; }
        const cart = await res.json();
        $('#cart-row-' + itemId).remove();
        refreshCartUI(cart);
        if (cart.items.length === 0) location.reload();
    } catch {
        showAlert('danger', 'Failed to remove item. Please try again.');
    } finally {
        $(btn).prop('disabled', false);
    }
}

function refreshCartUI(cart) {
    let total = 0, count = 0;
    cart.items.forEach(i => {
        const line = i.unitPrice * i.quantity;
        total += line;
        count += i.quantity;
        $('#qty-' + i.id).val(i.quantity);
        $('#line-' + i.id).text(fmtCurrency(line));
    });
    $('#summary-count').text(count);
    $('#summary-total').text(fmtCurrency(total));
    $('#grand-total').text(fmtCurrency(total));
    renderHeaderCart({ itemCount: count, total: total, items: cart.items });
}

function showAlert(type, msg) {
    $('#cart-alert')
        .attr('class', 'alert alert-' + type)
        .text(msg)
        .removeClass('d-none');
    setTimeout(() => $('#cart-alert').addClass('d-none'), 4000);
}
