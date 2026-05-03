/**
 * @file ui-manager.js
 * @description Handles all DOM manipulations, event binding and rendering logic.
 */

import { SELECTORS, AGGREGATE_FUNCTIONS, HAVING_OPERATORS } from '../constants.js';
import { escapeHtml } from './helpers.js';

const DOM = {};
// Cache DOM elements
export function initDom() {
    Object.keys(SELECTORS).forEach(key => {
        DOM[key] = document.querySelector(SELECTORS[key]);
    });
}

export const UIManager = {
    get DOM() { return DOM; },

    showAlert(message, type = 'danger') {
        const alertDiv = document.createElement('div');
        alertDiv.className = `alert alert-${type} alert-dismissible fade show position-fixed top-0 start-50 translate-middle-x mt-3`;
        alertDiv.style.zIndex = '9999';
        alertDiv.innerHTML = `
            <i class="bi ${type === 'success' ? 'bi-check-circle-fill' : 'bi-exclamation-triangle-fill'} me-2"></i>
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        `;
        document.body.appendChild(alertDiv);
        setTimeout(() => {
            const bsAlert = new bootstrap.Alert(alertDiv);
            bsAlert.close();
        }, 4000);
    },

    renderTableOptions(tables) {
        DOM.tableNameSelect.innerHTML = '<option value="">-- Chọn bảng / View --</option>';
        tables.forEach(table => {
            const option = new Option(table, table);
            DOM.tableNameSelect.add(option);
        });
    },

    /**
     * Optimized render for columns. 
     * Uses template strings and event delegation from index.js
     */
    renderColumns(columnsByTable, selectedColumns) {
        const isSelected = (tableName, colName) =>
            selectedColumns.some(c => c.TableName === tableName && c.ColumnName === colName);

        const tables = Object.keys(columnsByTable);
        if (tables.length === 0) {
            DOM.columnSelectionContainer.innerHTML = '<p class="text-muted small mb-0 empty-state-text">Không có cột nào.</p>';
            return;
        }

        let html = tables.map(table => {
            const cols = columnsByTable[table];
            const allSelected = cols.length > 0 && cols.every(col => isSelected(table, col));
            
            const colsHtml = cols.map((col, index) => {
                const id = `col_${table}_${index}`;
                const checked = isSelected(table, col);
                const selectedCol = selectedColumns.find(c => c.TableName === table && c.ColumnName === col && !c.IsComputed);
                const aliasVal = selectedCol ? selectedCol.AliasName : '';
                
                return `
                    <div class="col-chip" style="${checked ? 'padding-right: 4px;' : ''}">
                        <div class="form-check mb-0 d-flex align-items-center">
                            <input class="form-check-input column-checkbox me-2" type="checkbox" 
                                data-table="${table}" data-column="${col}" id="${id}" ${checked ? 'checked' : ''}>
                            <label class="form-check-label text-truncate" style="max-width:140px; margin-right: ${checked ? '8px' : '0'}" for="${id}" title="${col}">
                                ${col}
                            </label>
                            ${checked ? `<input type="text" class="alias-input" data-table="${table}" data-column="${col}" value="${aliasVal}" placeholder="Alias..." title="Nhập Alias" />` : ''}
                        </div>
                    </div>
                `;
            }).join('');

            return `
                <div class="table-group-label d-flex justify-content-between align-items-center">Bảng: ${table}</div>
                <div class="select-all-row mb-2">
                    <div class="form-check mb-0">
                        <input class="form-check-input select-all-checkbox" type="checkbox" id="select_all_${table}" data-table="${table}" ${allSelected ? 'checked' : ''}>
                        <label class="form-check-label" for="select_all_${table}" style="cursor:pointer">Chọn tất cả cột</label>
                    </div>
                </div>
                <div class="d-flex flex-wrap gap-2 mb-3">${colsHtml}</div>
            `;
        }).join('');

        const computedCols = selectedColumns.filter(c => c.IsComputed);
        if (computedCols.length > 0) {
            html += `
                <div class="table-group-label d-flex justify-content-between align-items-center mt-3 text-warning border-warning">
                    <span><i class="bi bi-calculator me-1"></i> Cột tính toán</span>
                </div>
                <div class="d-flex flex-wrap gap-2 mb-3">
                    ${computedCols.map(c => {
                        const realIndex = selectedColumns.indexOf(c);
                        return `
                        <div class="col-chip pe-2 border-warning d-flex align-items-center">
                            <span class="text-truncate text-warning ms-2 me-2" style="max-width:200px;" title="${escapeHtml(c.Expression)}">
                                ${escapeHtml(c.AliasName)}
                            </span>
                            <button type="button" class="btn btn-sm btn-link text-warning p-0 me-2 edit-computed-col-btn" 
                                data-real-index="${realIndex}" title="Sửa">
                                <i class="bi bi-pencil-square"></i>
                            </button>
                            <button type="button" class="btn btn-sm btn-link text-danger p-0 delete-computed-col-btn" 
                                data-expression="${escapeHtml(c.Expression)}" data-alias="${escapeHtml(c.AliasName)}" title="Xóa">
                                <i class="bi bi-x-lg"></i>
                            </button>
                        </div>
                    `}).join('')}
                </div>
            `;
        }

        DOM.columnSelectionContainer.innerHTML = html;

        if (DOM.sortableColumns && DOM.selectedColumnsOrderContainer) {
            if (selectedColumns.length === 0) {
                DOM.selectedColumnsOrderContainer.style.display = 'none';
            } else {
                DOM.selectedColumnsOrderContainer.style.display = 'block';
                DOM.sortableColumns.innerHTML = selectedColumns.map((col, index) => {
                    const label = col.IsComputed ? col.AliasName : `${col.TableName}.${col.ColumnName}`;
                    const badgeClass = col.IsComputed ? 'border-warning text-dark' : 'border-secondary text-dark';
                    return `
                        <div class="badge bg-light p-2 sortable-item ${badgeClass}" draggable="true" data-index="${index}" style="cursor: move; user-select: none;">
                            <i class="bi bi-grip-vertical text-muted me-1"></i>${escapeHtml(label)}
                        </div>
                    `;
                }).join('');
            }
        }
    },

    renderAggregation(selectedColumns, isEnabled) {
        if (selectedColumns.length === 0 || !isEnabled) {
            DOM.aggregationGroup.style.display = 'none';
            return;
        }

        DOM.aggregationGroup.style.display = 'block';
        
        let html = '<div class="small text-muted mb-3"><i class="bi bi-info-circle-fill text-primary"></i> Các cột không chọn hàm thống kê sẽ tự động được gom nhóm (GROUP BY).</div>';
        
        // Dropdown to add aggregation
        const availableColsOptions = selectedColumns.map((c, i) => {
            const label = c.IsComputed ? c.AliasName : `${c.TableName}.${c.ColumnName}`;
            return `<option value="${i}">${escapeHtml(label)}</option>`;
        }).join('');

        html += `
            <div class="d-flex gap-2 mb-3">
                <select class="form-select form-select-sm" id="newAggColumnSelect">
                    <option value="">-- Chọn cột để thống kê --</option>
                    ${availableColsOptions}
                </select>
                <button type="button" class="btn btn-sm btn-primary" id="addNewAggBtn">
                    <i class="bi bi-plus"></i> Thêm
                </button>
            </div>
            <div id="aggItemsContainer">
        `;

        html += selectedColumns.map((col, index) => {
            if (!col.ShowInAgg) return ''; // Only render if explicitly shown

            const hasAgg = col.Aggregate !== 'None';
            
            const aggOptions = AGGREGATE_FUNCTIONS.map(o => 
                `<option value="${o.value}" ${col.Aggregate === o.value ? 'selected' : ''}>${o.label}</option>`
            ).join('');

            const havingOpOptions = HAVING_OPERATORS.map(o => 
                `<option value="${o.value}" ${col.HavingOperator == o.value ? 'selected' : ''}>${o.label}</option>`
            ).join('');

            return `
                <div class="agg-item" data-index="${index}">
                    <div class="d-flex align-items-center justify-content-between mb-2">
                        <span class="small fw-bold ${col.IsComputed ? 'text-warning' : 'text-primary'} text-truncate" title="${col.IsComputed ? escapeHtml(col.Expression) : `${col.TableName}.${col.ColumnName}`}">
                            ${col.IsComputed ? escapeHtml(col.AliasName) : col.ColumnName} 
                            ${!col.IsComputed ? `<span class="badge bg-secondary ms-1 fw-normal">${col.TableName}</span>` : `<span class="badge bg-warning text-dark ms-1 fw-normal">Computed</span>`}
                        </span>
                        <div class="btn-group btn-group-sm">
                            <button type="button" class="btn btn-sm btn-outline-primary py-0 px-2 add-extra-agg-btn" title="Nhân bản">
                                <i class="bi bi-plus"></i>
                            </button>
                            <button type="button" class="btn btn-sm btn-outline-danger py-0 px-2 remove-agg-btn" title="Xóa">
                                <i class="bi bi-trash"></i>
                            </button>
                        </div>
                    </div>
                    <div class="row g-2 align-items-center mb-1">
                        <div class="col-md-5">
                            <select class="form-select form-select-sm item-agg-select">${aggOptions}</select>
                        </div>
                        <div class="col-md-7">
                            <input type="text" class="form-control form-control-sm item-alias-input" 
                                placeholder="Alias..." value="${col.AliasName}" style="display:${hasAgg ? 'block' : 'none'}">
                        </div>
                    </div>
                    <div class="row g-2 align-items-center mt-1 pt-2 border-top item-having-row" style="display:${hasAgg ? 'flex' : 'none'}">
                        <div class="col-md-5">
                            <select class="form-select form-select-sm item-having-op">${havingOpOptions}</select>
                        </div>
                        <div class="col-md-7">
                            <input type="text" class="form-control form-control-sm item-having-val" 
                                placeholder="Giá trị lọc..." value="${col.HavingValue || ''}">
                        </div>
                    </div>
                </div>
            `;
        }).join('');

        html += '</div>';
        DOM.aggregationContainer.innerHTML = html;
    },

    renderPreviewTable(data, columns) {
        DOM.recordCountBadge.textContent = `${data.length} dòng`;

        if (data.length === 0) {
            DOM.previewTableHead.innerHTML = '<tr><th class="text-center text-muted fw-normal">Không có dữ liệu phù hợp</th></tr>';
            DOM.previewTableBody.innerHTML = '<tr><td class="text-center text-muted py-4">Thử thay đổi điều kiện lọc.</td></tr>';
            return;
        }

        DOM.previewTableHead.innerHTML = `<tr>${columns.map(col => `<th>${escapeHtml(col)}</th>`).join('')}</tr>`;

        DOM.previewTableBody.innerHTML = data.map(row => `
            <tr>
                ${columns.map(col => {
                    const val = row[col];
                    return `<td>${val !== null && val !== undefined ? escapeHtml(String(val)) : '<em class="text-muted">NULL</em>'}</td>`;
                }).join('')}
            </tr>
        `).join('');
    },

    updateButtonStates(state, hasSelectedColumns) {
        const hasTable = !!state.tableName;
        if (DOM.addComputedColumnBtn) DOM.addComputedColumnBtn.disabled = !hasTable;
        DOM.addFilterBtn.disabled = !hasTable;
        DOM.toggleAggregationBtn.disabled = !hasSelectedColumns;
        DOM.previewBtn.disabled = !(hasTable && hasSelectedColumns);
        DOM.generatePdfBtn.disabled = !(hasTable && hasSelectedColumns);
    },

    setLoading(btnId, isLoading, originalText, iconClass) {
        const btn = DOM[btnId];
        const textSpan = DOM[`${btnId}Text`];
        const icon = btn.querySelector('i');

        if (isLoading) {
            textSpan.textContent = 'Đang xử lý...';
            icon.className = 'spinner-border spinner-border-sm';
            btn.disabled = true;
        } else {
            textSpan.textContent = originalText;
            icon.className = iconClass;
            btn.disabled = false;
        }
    }
};
