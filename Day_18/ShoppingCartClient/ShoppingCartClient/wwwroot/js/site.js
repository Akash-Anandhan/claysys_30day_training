// ===== Cart Count Badge =====
function updateCartCount() {
    fetch('/Cart/GetCartCount')
        .then(res => res.json())
        .then(data => {
            const badge = document.getElementById('cart-count');
            if (badge) {
                badge.textContent = data.count;
                badge.style.display = data.count > 0 ? 'inline' : 'none';
            }
        })
        .catch(() => { });
}

// ===== Toast Notification =====
function showToast(message, type = 'success') {
    const container = document.getElementById('toast-container');
    if (!container) return;

    const id = 'toast-' + Date.now();
    const icons = { success: 'bi-check-circle-fill', danger: 'bi-x-circle-fill', warning: 'bi-exclamation-circle-fill' };
    const icon = icons[type] || icons.success;

    const html = `
        <div id="${id}" class="toast align-items-center text-bg-${type} border-0 mb-2" role="alert">
            <div class="d-flex">
                <div class="toast-body d-flex align-items-center gap-2">
                    <i class="bi ${icon}"></i>
                    <span>${message}</span>
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto"
                        data-bs-dismiss="toast"></button>
            </div>
        </div>`;

    container.insertAdjacentHTML('beforeend', html);
    const toastEl = document.getElementById(id);
    const toast = new bootstrap.Toast(toastEl, { delay: 3000 });
    toast.show();
    toastEl.addEventListener('hidden.bs.toast', () => toastEl.remove());
}

// ===== Add to Cart with AJAX =====
function setupAddToCart() {
    document.querySelectorAll('.add-to-cart-form').forEach(form => {
        form.addEventListener('submit', function (e) {
            e.preventDefault();

            const btn = form.querySelector('button[type="submit"]');
            const originalHtml = btn.innerHTML;
            btn.innerHTML = '<span class="spinner-border spinner-border-sm"></span>';
            btn.disabled = true;

            fetch(form.action, {
                method: 'POST',
                body: new FormData(form),
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            })
                .then(res => {
                    if (res.ok) {
                        showToast('Item added to cart!', 'success');
                        updateCartCount();
                    } else {
                        showToast('Something went wrong.', 'danger');
                    }
                })
                .catch(() => showToast('Something went wrong.', 'danger'))
                .finally(() => {
                    btn.innerHTML = originalHtml;
                    btn.disabled = false;
                });
        });
    });
}

// ===== Quantity Input Validation =====
function setupQuantityInputs() {
    document.querySelectorAll('input[type="number"]').forEach(input => {
        input.addEventListener('change', function () {
            const min = parseInt(this.min) || 1;
            const max = parseInt(this.max) || 999;
            if (this.value < min) this.value = min;
            if (this.value > max) this.value = max;
        });
    });
}

// ===== Confirm Delete =====
function setupRemoveButtons() {
    document.querySelectorAll('.remove-form').forEach(form => {
        form.addEventListener('submit', function (e) {
            if (!confirm('Remove this item from your cart?')) {
                e.preventDefault();
            }
        });
    });
}

// ===== Loading Spinner on Checkout =====
function setupCheckoutSpinner() {
    const checkoutForm = document.querySelector('.checkout-form');
    const spinner = document.querySelector('.spinner-overlay');

    if (checkoutForm && spinner) {
        checkoutForm.addEventListener('submit', function () {
            spinner.classList.add('show');
        });
    }
}

// ===== Wishlist Count Badge =====
function updateWishlistCount() {
    fetch('/Wishlist/GetWishlistCount')
        .then(res => res.json())
        .then(data => {
            const badge = document.getElementById('wishlist-count');
            if (badge) {
                badge.textContent = data.count;
                badge.style.display = data.count > 0 ? 'inline' : 'none';
            }
        })
        .catch(() => { });
}

// ===== Confirm Remove from Wishlist =====
function setupWishlistRemoveButtons() {
    document.querySelectorAll('.remove-wishlist-form').forEach(form => {
        form.addEventListener('submit', function (e) {
            if (!confirm('Remove this item from your wishlist?')) {
                e.preventDefault();
            }
        });
    });
}

// ===== Live Search Suggestions =====
function setupLiveSearch() {
    const input = document.getElementById('search-input');
    const suggestions = document.getElementById('search-suggestions');

    if (!input || !suggestions) return;

    let debounceTimer;

    input.addEventListener('input', function () {
        clearTimeout(debounceTimer);
        const query = this.value.trim();

        if (query.length < 2) {
            suggestions.style.display = 'none';
            return;
        }

        debounceTimer = setTimeout(() => {
            fetch(`/Search/Suggestions?query=${encodeURIComponent(query)}`)
                .then(res => res.json())
                .then(data => {
                    if (data.length === 0) {
                        suggestions.style.display = 'none';
                        return;
                    }

                    suggestions.innerHTML = data.map(item => `
                        <a href="/Product/Details/${item.id}"
                           class="d-flex justify-content-between align-items-center
                                  px-3 py-2 text-dark border-bottom suggestion-item"
                           style="text-decoration:none;">
                            <span><i class="bi bi-search me-2 text-muted"></i>${item.name}</span>
                            <span class="text-success fw-bold">$${item.price.toFixed(2)}</span>
                        </a>
                    `).join('');

                    suggestions.style.display = 'block';
                })
                .catch(() => { suggestions.style.display = 'none'; });
        }, 300);
    });

    // Hide suggestions when clicking outside
    document.addEventListener('click', function (e) {
        if (!input.contains(e.target) && !suggestions.contains(e.target)) {
            suggestions.style.display = 'none';
        }
    });

    // Hover effect on suggestions
    suggestions.addEventListener('mouseover', function (e) {
        const item = e.target.closest('.suggestion-item');
        if (item) item.style.backgroundColor = '#f8f9fa';
    });

    suggestions.addEventListener('mouseout', function (e) {
        const item = e.target.closest('.suggestion-item');
        if (item) item.style.backgroundColor = '';
    });
}

// ===== Run on Page Load =====
document.addEventListener('DOMContentLoaded', function () {
    updateCartCount();
    updateWishlistCount();
    setupAddToCart();
    setupQuantityInputs();
    setupRemoveButtons();
    setupWishlistRemoveButtons();
    setupCheckoutSpinner();
    setupLiveSearch();
});

