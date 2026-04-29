// Site-wide utilities — available on every page.

function updateHeaderCartCount(count) {
    const countEl = document.getElementById('header-cart-count');
    const labelEl = document.getElementById('header-cart-label');
    if (countEl) countEl.textContent = count;
    if (labelEl) labelEl.textContent = count;
}

// Refresh the header cart badge on every page load.
(async () => {
    try {
        const res = await fetch('/cart/count');
        if (!res.ok) return;
        const data = await res.json();
        updateHeaderCartCount(data.itemCount);
    } catch {}
})();
