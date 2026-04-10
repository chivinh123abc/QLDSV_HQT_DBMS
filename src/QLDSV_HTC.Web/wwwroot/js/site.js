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
    toastEl.addEventListener('hidden.bs.toast', function() {
        toastEl.remove();
    });
};

/**
 * Hiển thị Hộp thoại Xác nhận (Custom Modal Promise)
 * @param {string} title Tiêu đề
 * @param {string} message Nội dung (chấp nhận HTML)
 * @param {object} options Các cấu hình nút bấm
 * @returns {Promise<boolean>} Resolves to true nếu bấm Xác nhận, false nếu Hủy
 */
window.showConfirmDialog = function(title, message, options = {}) {
    return new Promise((resolve) => {
        const confirmText = options.confirmText || "Xác nhận";
        const cancelText = options.cancelText || "Hủy bỏ";
        const isDanger = options.isDanger || false;
        
        document.getElementById('globalConfirmTitle').innerHTML = title;
        document.getElementById('globalConfirmMessage').innerHTML = message;
        
        const btnOk = document.getElementById('globalConfirmBtnOk');
        btnOk.innerHTML = `<i class="bi bi-check-lg me-1"></i>${confirmText}`;
        btnOk.className = `btn flex-fill ${isDanger ? 'btn-danger' : 'btn-primary'}`;
        
        const btnCancel = document.getElementById('globalConfirmBtnCancel');
        btnCancel.innerHTML = `<i class="bi bi-x-lg me-1"></i>${cancelText}`;

        const iconContainer = document.getElementById('globalConfirmIconContainer');
        const icon = document.getElementById('globalConfirmIcon');
        if (isDanger) {
            iconContainer.className = 'rounded-circle bg-danger bg-opacity-10 p-2 d-flex align-items-center justify-content-center';
            icon.className = 'bi bi-exclamation-triangle-fill text-danger fs-5';
        } else {
            iconContainer.className = 'rounded-circle bg-primary bg-opacity-10 p-2 d-flex align-items-center justify-content-center';
            icon.className = 'bi bi-question-circle-fill text-primary fs-5';
        }

        const modalEl = document.getElementById('globalConfirmModal');
        const modal = bootstrap.Modal.getOrCreateInstance(modalEl);

        const cleanup = () => {
            btnOk.removeEventListener('click', onConfirm);
            btnCancel.removeEventListener('click', onCancel);
        };

        const onConfirm = () => { cleanup(); resolve(true); modal.hide(); };
        const onCancel = () => { cleanup(); resolve(false); modal.hide(); };
        
        btnOk.addEventListener('click', onConfirm);
        btnCancel.addEventListener('click', onCancel);

        modal.show();
    });
};
