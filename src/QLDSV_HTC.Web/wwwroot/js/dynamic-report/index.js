/**
 * @file index.js
 * @description Main entry point. Initializes state, UI, and event listeners.
 */

import { ApiService } from './modules/api.js';
import { State } from './modules/state-manager.js';
import { UIManager, initDom } from './modules/ui-manager.js';
import { highlightSql, getFilterPlaceholder, getAntiForgeryToken } from './modules/helpers.js';

let sqlPreviewTimer = null;

document.addEventListener('DOMContentLoaded', async () => {
    initDom();
    const DOM = UIManager.DOM;

    // --- Initialization ---
    try {
        const result = await ApiService.getTables();
        if (result.success) {
            UIManager.renderTableOptions(result.data);
        } else {
            UIManager.showAlert('Lỗi tải danh sách bảng: ' + result.message);
        }
    } catch (err) {
        UIManager.showAlert('Đã xảy ra lỗi khi tải danh sách bảng.');
    }

    // --- Event Listeners ---
    DOM.tableNameSelect.addEventListener('change', onTableChange);
    DOM.addJoinBtn.addEventListener('click', addJoinRow);
    DOM.addFilterBtn.addEventListener('click', addFilterRow);
    DOM.previewBtn.addEventListener('click', previewData);
    DOM.generatePdfBtn.addEventListener('click', generatePdf);
    DOM.toggleAggregationBtn.addEventListener('click', onToggleAggregation);

    DOM.copySqlBtn.addEventListener('click', async () => {
        try {
            await navigator.clipboard.writeText(DOM.sqlPreviewCode.textContent);
            const originalText = DOM.copySqlBtnText.textContent;
            const originalIcon = DOM.copySqlIcon.className;
            
            DOM.copySqlBtnText.textContent = 'Copied!';
            DOM.copySqlIcon.className = 'bi bi-check-lg';
            DOM.copySqlBtn.classList.replace('border-secondary', 'border-success');
            DOM.copySqlBtn.classList.replace('text-light', 'text-success');

            setTimeout(() => {
                DOM.copySqlBtnText.textContent = originalText;
                DOM.copySqlIcon.className = originalIcon;
                DOM.copySqlBtn.classList.replace('border-success', 'border-secondary');
                DOM.copySqlBtn.classList.replace('text-success', 'text-light');
            }, 2000);
        } catch (err) {
            UIManager.showAlert('Lỗi khi copy SQL.', 'danger');
        }
    });

    // --- Core Logic Functions ---

    function stateChanged() {
        UIManager.updateButtonStates(State, State.selectedColumns.length > 0);
        triggerSqlPreview();
    }

    function triggerSqlPreview() {
        if (sqlPreviewTimer) clearTimeout(sqlPreviewTimer);
        
        const payload = getRequestPayload();
        if (!payload.TableName || payload.AdvancedSelectColumns.length === 0) {
            DOM.sqlPreviewCode.textContent = "-- Cấu hình báo cáo để xem câu lệnh SQL tự động tạo tại đây...";
            DOM.copySqlBtn.style.display = 'none';
            return;
        }

        sqlPreviewTimer = setTimeout(async () => {
            try {
                const result = await ApiService.getSqlPreview(payload);
                if (result.success && result.sql) {
                    DOM.sqlPreviewCode.innerHTML = highlightSql(result.sql);
                    DOM.copySqlBtn.style.display = 'inline-flex';
                }
            } catch (err) {
                console.error('SQL Preview Error:', err);
            }
        }, 300);
    }

    async function onTableChange() {
        const tableName = DOM.tableNameSelect.value;
        DOM.joinsContainer.innerHTML = '';
        DOM.filtersContainer.innerHTML = '';
        DOM.aggregationContainer.innerHTML = '';
        
        State.reset(tableName);
        
        DOM.joinSelectionGroup.style.display = 'none';
        DOM.aggregationGroup.style.display = 'none';
        resetAggregationButton();
        stateChanged();

        if (!tableName) {
            DOM.columnSelectionContainer.innerHTML = '<p class="text-muted small mb-0 empty-state-text">Vui lòng chọn bảng trước.</p>';
            return;
        }

        DOM.columnSelectionContainer.innerHTML = '<div class="spinner-border spinner-border-sm text-primary" role="status"></div> Đang tải...';

        try {
            const [colResult, relResult] = await Promise.all([
                ApiService.getColumns(tableName),
                ApiService.getRelations(tableName)
            ]);

            if (colResult.success) {
                State.setColumns(tableName, colResult.data);
                UIManager.renderColumns(State.columnsByTable, State.selectedColumns);
            }

            if (relResult.success && relResult.data.length > 0) {
                State.availableRelations = relResult.data;
                DOM.joinSelectionGroup.style.display = 'block';
            }
            stateChanged();
        } catch (error) {
            DOM.columnSelectionContainer.innerHTML = '<p class="text-danger small mb-0">Lỗi kết nối.</p>';
        }
    }

    // --- Column & Aggregation Handlers (Using Event Delegation) ---
    DOM.columnSelectionContainer.addEventListener('change', async (e) => {
        const target = e.target;
        if (target.classList.contains('select-all-checkbox')) {
            const table = target.dataset.table;
            const isChecked = target.checked;
            State.columnsByTable[table].forEach(col => State.toggleColumn(table, col, isChecked));
            UIManager.renderColumns(State.columnsByTable, State.selectedColumns);
            UIManager.renderAggregation(State.selectedColumns, State.isAggregationEnabled);
            stateChanged();
        } else if (target.classList.contains('column-checkbox')) {
            const { table, column } = target.dataset;
            State.toggleColumn(table, column, target.checked);
            UIManager.renderColumns(State.columnsByTable, State.selectedColumns);
            UIManager.renderAggregation(State.selectedColumns, State.isAggregationEnabled);
            stateChanged();
        } else if (target.classList.contains('alias-input')) {
            const { table, column } = target.dataset;
            State.addColumnAlias(table, column, target.value);
            UIManager.renderAggregation(State.selectedColumns, State.isAggregationEnabled);
            stateChanged();
        }
    });

    DOM.aggregationContainer.addEventListener('change', (e) => {
        const target = e.target;
        const index = parseInt(target.closest('.agg-item')?.dataset.index);
        if (isNaN(index)) return;

        if (target.classList.contains('item-agg-select')) {
            State.updateColumnProperty(index, 'Aggregate', target.value);
            UIManager.renderAggregation(State.selectedColumns, State.isAggregationEnabled);
            stateChanged();
        } else if (target.classList.contains('item-having-op')) {
            State.updateColumnProperty(index, 'HavingOperator', target.value ? parseInt(target.value, 10) : null);
            stateChanged();
        }
    });

    DOM.aggregationContainer.addEventListener('input', (e) => {
        const target = e.target;
        const index = parseInt(target.closest('.agg-item')?.dataset.index);
        if (isNaN(index)) return;

        if (target.classList.contains('item-alias-input')) {
            State.updateColumnProperty(index, 'AliasName', target.value);
            stateChanged();
        } else if (target.classList.contains('item-having-val')) {
            State.updateColumnProperty(index, 'HavingValue', target.value);
            stateChanged();
        }
    });

    DOM.aggregationContainer.addEventListener('click', (e) => {
        const btn = e.target.closest('button');
        if (!btn) return;
        const index = parseInt(btn.closest('.agg-item')?.dataset.index);
        if (isNaN(index)) return;

        if (btn.classList.contains('add-extra-agg-btn')) {
            State.duplicateAggregation(index);
            UIManager.renderAggregation(State.selectedColumns, State.isAggregationEnabled);
            stateChanged();
        } else if (btn.classList.contains('remove-agg-btn')) {
            const shouldUncheck = State.removeAggregation(index);
            if (shouldUncheck) {
                // Find and uncheck column in column container if needed
                UIManager.renderColumns(State.columnsByTable, State.selectedColumns);
            }
            UIManager.renderAggregation(State.selectedColumns, State.isAggregationEnabled);
            stateChanged();
        }
    });

    function onToggleAggregation() {
        State.isAggregationEnabled = !State.isAggregationEnabled;
        if (State.isAggregationEnabled) {
            DOM.toggleAggregationBtn.classList.replace('dr-btn-add', 'dr-btn-primary');
            DOM.toggleAggregationBtn.classList.add('dr-btn', 'px-3');
            DOM.toggleAggregationBtn.innerHTML = '<i class="bi bi-calculator-fill"></i> Đang bật Thống kê';
        } else {
            resetAggregationButton();
            State.selectedColumns.forEach(c => {
                c.Aggregate = 'None';
                c.AliasName = '';
                c.HavingOperator = null;
                c.HavingValue = '';
            });
            stateChanged();
        }
        UIManager.renderAggregation(State.selectedColumns, State.isAggregationEnabled);
    }

    function resetAggregationButton() {
        DOM.toggleAggregationBtn.className = 'dr-btn-add';
        DOM.toggleAggregationBtn.innerHTML = '<i class="bi bi-calculator"></i> Thống kê (Group By)';
    }

    // --- Joins & Filters (Direct DOM Manipulation) ---

    function addJoinRow() {
        if (State.availableRelations.length === 0) return;
        const clone = DOM.joinRowTemplate.content.cloneNode(true);
        const row = clone.querySelector('.join-row');
        const select = row.querySelector('.join-table-select');

        State.availableRelations.forEach(table => {
            select.add(new Option(table, table));
        });

        select.addEventListener('change', async function() {
            const table = this.value;
            if (table && !State.columnsByTable[table]) {
                try {
                    const res = await ApiService.getColumns(table);
                    if (res.success) State.setColumns(table, res.data);
                } catch (err) {}
            }
            await rebuildColumnsFromActiveTables();
            stateChanged();
        });

        row.querySelector('.join-type-select').addEventListener('change', stateChanged);
        row.querySelector('.remove-join-btn').addEventListener('click', async () => {
            row.remove();
            await rebuildColumnsFromActiveTables();
            stateChanged();
        });

        DOM.joinsContainer.appendChild(row);
        stateChanged();
    }

    async function rebuildColumnsFromActiveTables() {
        const activeTables = [DOM.tableNameSelect.value];
        DOM.joinsContainer.querySelectorAll('.join-table-select').forEach(s => {
            if (s.value) activeTables.push(s.value);
        });

        // Update available relations for NEW joins
        const newAvailableSet = new Set();
        for (const table of activeTables) {
            try {
                const res = await ApiService.getRelations(table);
                if (res.success) res.data.forEach(t => {
                    if (!activeTables.includes(t)) newAvailableSet.add(t);
                });
            } catch (err) {}
        }
        State.availableRelations = Array.from(newAvailableSet);

        // Filter out columns from tables no longer joined
        State.selectedColumns = State.selectedColumns.filter(c => activeTables.includes(c.TableName));
        
        UIManager.renderColumns(State.columnsByTable, State.selectedColumns);
        UIManager.renderAggregation(State.selectedColumns, State.isAggregationEnabled);
    }

    function addFilterRow() {
        const clone = DOM.filterRowTemplate.content.cloneNode(true);
        const row = clone.querySelector('.filter-row');
        const colSelect = row.querySelector('.filter-column-select');
        const opSelect = row.querySelector('.filter-operator-select');
        const valInput = row.querySelector('.filter-value-input');

        Object.keys(State.columnsByTable).forEach(table => {
            const group = document.createElement('optgroup');
            group.label = `Bảng: ${table}`;
            State.columnsByTable[table].forEach(col => {
                group.appendChild(new Option(col, `${table}.${col}`));
            });
            colSelect.appendChild(group);
        });

        colSelect.addEventListener('change', () => {
            const col = colSelect.value.split('.')[1];
            valInput.placeholder = getFilterPlaceholder(col);
            stateChanged();
        });

        opSelect.addEventListener('change', () => {
            const op = parseInt(opSelect.value, 10);
            const isNullOp = op === 9 || op === 10;
            valInput.style.display = isNullOp ? 'none' : 'block';
            valInput.required = !isNullOp;
            if (!isNullOp) {
                if (op === 11 || op === 12) valInput.placeholder = "val1, val2, ...";
                else if (op === 13) valInput.placeholder = "val1, val2";
                else valInput.placeholder = getFilterPlaceholder(colSelect.value.split('.')[1]);
            }
            stateChanged();
        });

        valInput.addEventListener('input', stateChanged);
        row.querySelector('.remove-filter-btn').addEventListener('click', () => {
            row.remove();
            stateChanged();
        });

        DOM.filtersContainer.appendChild(row);
        colSelect.dispatchEvent(new Event('change'));
    }

    // --- Action Handlers ---

    function getRequestPayload() {
        const joins = Array.from(DOM.joinsContainer.querySelectorAll('.join-row')).map(r => ({
            JoinTable: r.querySelector('.join-table-select').value,
            JoinType: r.querySelector('.join-type-select').value
        })).filter(j => j.JoinTable);

        const filters = Array.from(DOM.filtersContainer.querySelectorAll('.filter-row')).map(r => {
            const colVal = r.querySelector('.filter-column-select').value;
            const op = parseInt(r.querySelector('.filter-operator-select').value, 10);
            const val = r.querySelector('.filter-value-input').value;
            if (!colVal) return null;
            const [tbl, col] = colVal.split('.');
            return { TableName: tbl, ColumnName: col, Operator: op, Value: val || '' };
        }).filter(f => f && (f.Value || f.Operator === 9 || f.Operator === 10));

        return {
            TableName: State.tableName,
            Joins: joins,
            AdvancedSelectColumns: State.selectedColumns,
            Filters: filters,
            PageNumber: 1,
            PageSize: 50
        };
    }

    async function previewData() {
        const payload = getRequestPayload();
        UIManager.setLoading('previewBtn', true, 'Xem trước dữ liệu', 'bi bi-table');
        DOM.previewTableBody.innerHTML = '<tr><td colspan="100%" class="text-center py-4"><div class="spinner-border text-primary" role="status"></div></td></tr>';

        try {
            const result = await ApiService.previewData(payload);
            if (result.success) {
                UIManager.renderPreviewTable(result.data, result.columns);
            } else {
                UIManager.showAlert('Lỗi: ' + result.message);
            }
        } catch (error) {
            UIManager.showAlert('Đã xảy ra lỗi khi tải dữ liệu.');
        } finally {
            UIManager.setLoading('previewBtn', false, 'Xem trước dữ liệu', 'bi bi-table');
        }
    }

    async function generatePdf() {
        const payload = getRequestPayload();
        UIManager.setLoading('generatePdfBtn', true, 'Đang xuất PDF...', 'bi bi-file-earmark-pdf');

        try {
            const blob = await ApiService.generatePdf(payload);
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = `Report_${State.tableName}_${Date.now()}.pdf`;
            a.click();
            window.URL.revokeObjectURL(url);
        } catch (error) {
            UIManager.showAlert(error.message);
        } finally {
            UIManager.setLoading('generatePdfBtn', false, 'Xuất PDF', 'bi bi-file-earmark-pdf');
        }
    }
});
