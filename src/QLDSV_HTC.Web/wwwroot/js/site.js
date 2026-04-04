/**
 * site.js - Global JavaScript utilities for QLDSV_HTC
 */

/**
 * Hiển thị thông báo Toast
 * @param {string} message Nội dung thông báo
 * @param {string} type Loại thông báo ('success', 'danger', 'warning', 'info')
 */
window.showToast = function(message, type = 'success') {
    const container = document.querySelector('.toast-container');
    if (!container) return;

    // Map type to Bootstrap background classes
    const bgClass = type === 'danger' ? 'text-bg-danger' : 
                   type === 'warning' ? 'text-bg-warning' : 
                   type === 'info' ? 'text-bg-info' : 'text-bg-success';
    
    const iconClass = type === 'danger' ? 'bi-exclamation-triangle-fill' : 
                     type === 'warning' ? 'bi-exclamation-circle' : 
                     type === 'info' ? 'bi-info-circle-fill' : 'bi-check-circle-fill';

    const toastId = 'toast-' + Date.now();
    const html = `
        <div id="${toastId}" class="toast align-items-center ${bgClass} border-0 shadow" role="alert" aria-live="assertive" aria-atomic="true">
            <div class="d-flex">
                <div class="toast-body fw-medium">
                    <i class="bi ${iconClass} me-2"></i> ${message}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
        </div>
    `;

    container.insertAdjacentHTML('beforeend', html);
    const toastEl = document.getElementById(toastId);
    const toast = new bootstrap.Toast(toastEl, { delay: 5000 });
    toast.show();

    // Remove from DOM after hide
    toastEl.addEventListener('hidden.bs.modal', function() {
        toastEl.remove();
    });
};
