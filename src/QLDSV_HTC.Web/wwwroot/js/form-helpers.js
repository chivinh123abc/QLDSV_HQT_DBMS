/**
 * form-helpers.js — Shared helpers for CRUD form modals
 * Eliminates duplicated JS between Class/Index and Student/Index.
 *
 * Usage:
 *   const helpers = createFormHelpers({
 *       alertEl:    document.getElementById('formAlert'),
 *       modeLabelEl: document.getElementById('formModeLabel'),
 *       buttons:    { them, xoa, ghi, phucHoi, huy },
 *       allFields:  [fld1, fld2, ...],        // toggled disabled on mode change
 *       requiredFields: [fld1, fld2],          // validated on submit
 *       getSelectedId: () => selectedId,       // getter for current selected entity ID
 *   });
 *
 *   helpers.setMode('add');
 *   helpers.showAlert('Saved!', 'success');
 *   if (helpers.validateForm()) { ... }
 */

'use strict';

/**
 * Factory: creates a set of reusable form helpers bound to the given config.
 * @param {Object} config
 * @returns {{ setMode, showAlert, hideAlert, validateForm, clearValidation, getMode }}
 */
function createFormHelpers(config) {
    const {
        alertEl,
        modeLabelEl,
        buttons,            // { them, xoa, ghi, phucHoi, huy }
        allFields,          // HTMLElement[]
        requiredFields,     // HTMLElement[]
        getSelectedId,      // () => string | null
    } = config;

    let _mode = 'view';

    const MODE_LABELS = {
        view: 'Chế độ: Xem',
        add:  'Chế độ: Thêm',
        edit: 'Chế độ: Sửa',
    };

    const MODE_CLASSES = {
        view: 'badge bg-white text-primary ms-2 border',
        add:  'badge bg-success text-white ms-2',
        edit: 'badge bg-warning text-dark ms-2',
    };

    const ALERT_ICONS = {
        success: 'bi-check-circle-fill',
        danger:  'bi-x-circle-fill',
        warning: 'bi-exclamation-triangle-fill',
        info:    'bi-info-circle-fill',
    };

    /**
     * Switch form mode: 'view' | 'add' | 'edit'
     * Toggles field disabled state, button states, and mode badge with animation.
     */
    function setMode(m) {
        _mode = m;

        // Update mode badge with animation
        if (modeLabelEl) {
            modeLabelEl.textContent = MODE_LABELS[m];
            modeLabelEl.className = MODE_CLASSES[m];
            modeLabelEl.style.fontSize = '.7rem';

            // Micro-animation on mode change
            modeLabelEl.style.transform = 'scale(1.15)';
            modeLabelEl.style.transition = 'transform 0.2s ease';
            setTimeout(() => { modeLabelEl.style.transform = 'scale(1)'; }, 200);
        }

        const editing = m !== 'view';

        // Toggle fields
        allFields.forEach(f => { if (f) f.disabled = !editing; });

        // Toggle buttons
        if (buttons.them)    buttons.them.disabled    = (m !== 'view');
        if (buttons.xoa)     buttons.xoa.disabled     = !(m === 'edit' && !!getSelectedId());
        if (buttons.ghi)     buttons.ghi.disabled     = !editing;
        if (buttons.phucHoi) buttons.phucHoi.disabled = !editing;
        if (buttons.huy)     buttons.huy.disabled     = (m === 'view');

        // Animate form fields on mode transition
        allFields.forEach(f => {
            if (!f) return;
            f.style.transition = 'border-color 0.3s ease, box-shadow 0.3s ease';
        });
    }

    /**
     * Show inline alert banner inside the form modal.
     */
    function showAlert(msg, type) {
        if (!alertEl) return;
        alertEl.className = `alert alert-${type} d-flex align-items-center gap-2 animate__animated animate__fadeIn`;
        alertEl.innerHTML = `<i class="bi ${ALERT_ICONS[type]}"></i><span>${msg}</span>`;
    }

    /**
     * Hide inline alert banner.
     */
    function hideAlert() {
        if (!alertEl) return;
        alertEl.className = 'alert d-none';
        alertEl.innerHTML = '';
    }

    /**
     * Validate required fields — adds 'is-invalid' class on empty fields.
     * @returns {boolean} true if all required fields are filled
     */
    function validateForm() {
        let ok = true;
        requiredFields.forEach(f => {
            if (!f) return;
            if (!f.value.trim()) {
                f.classList.add('is-invalid');
                ok = false;
            } else {
                f.classList.remove('is-invalid');
            }
        });
        return ok;
    }

    /**
     * Clear validation state from all required fields.
     */
    function clearValidation() {
        requiredFields.forEach(f => {
            if (f) f.classList.remove('is-invalid', 'is-valid');
        });
    }

    /**
     * Get current mode.
     */
    function getMode() {
        return _mode;
    }

    return { setMode, showAlert, hideAlert, validateForm, clearValidation, getMode };
}

/**
 * HTML-escape a string to prevent XSS in template literals.
 * @param {string} str
 * @returns {string}
 */
function escapeHtml(str) {
    return String(str ?? '')
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;');
}
