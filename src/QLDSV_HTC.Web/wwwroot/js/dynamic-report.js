document.addEventListener('DOMContentLoaded', function () {
    const tableNameSelect = document.getElementById('tableNameSelect');
    const joinSelectionGroup = document.getElementById('joinSelectionGroup');
    const addJoinBtn = document.getElementById('addJoinBtn');
    const joinsContainer = document.getElementById('joinsContainer');
    const columnSelectionContainer = document.getElementById('columnSelectionContainer');
    const addFilterBtn = document.getElementById('addFilterBtn');
    const filtersContainer = document.getElementById('filtersContainer');
    const previewBtn = document.getElementById('previewBtn');
    const generatePdfBtn = document.getElementById('generatePdfBtn');
    const previewTableHead = document.getElementById('previewTableHead');
    const previewTableBody = document.getElementById('previewTableBody');
    const recordCountBadge = document.getElementById('recordCountBadge');
    const aggregationGroup = document.getElementById('aggregationGroup');
    const aggregationContainer = document.getElementById('aggregationContainer');
    const toggleAggregationBtn = document.getElementById('toggleAggregationBtn');

    const joinRowTemplate = document.getElementById('joinRowTemplate');
    const filterRowTemplate = document.getElementById('filterRowTemplate');

    // State
    let columnsByTable = {}; // { 'SINHVIEN': ['MASV', 'HO', ...], 'LOP': [...] }
    let availableRelations = []; // ['LOP', 'DANGKY']
    let selectedColumnsState = []; // [{ TableName, ColumnName, Aggregate, AliasName, HavingOperator, HavingValue }]
    let isAggregationEnabled = false;

    // Initialize
    loadTables();

    // Event Listeners
    tableNameSelect.addEventListener('change', onTableChange);
    addJoinBtn.addEventListener('click', addJoinRow);
    addFilterBtn.addEventListener('click', addFilterRow);
    previewBtn.addEventListener('click', previewData);
    generatePdfBtn.addEventListener('click', generatePdf);
    toggleAggregationBtn.addEventListener('click', onToggleAggregation);

    async function loadTables() {
        try {
            const response = await fetch('/dynamic-report/tables');
            const result = await response.json();

            if (result.success) {
                tableNameSelect.innerHTML = '<option value="">-- Chọn bảng / View --</option>';
                result.data.forEach(table => {
                    const option = document.createElement('option');
                    option.value = table;
                    option.textContent = table;
                    tableNameSelect.appendChild(option);
                });
            } else {
                showAlert('Lỗi tải danh sách bảng: ' + result.message, 'danger');
            }
        } catch (error) {
            console.error('Error loading tables:', error);
            showAlert('Đã xảy ra lỗi khi tải danh sách bảng.', 'danger');
        }
    }

    async function onTableChange() {
        const tableName = tableNameSelect.value;

        // Reset state
        joinsContainer.innerHTML = '';
        filtersContainer.innerHTML = '';
        columnsByTable = {};
        availableRelations = [];
        selectedColumnsState = [];
        isAggregationEnabled = false;
        joinSelectionGroup.style.display = 'none';
        aggregationGroup.style.display = 'none';
        aggregationContainer.innerHTML = '';
        resetAggregationButton();
        updateButtonsState();

        if (!tableName) {
            columnSelectionContainer.innerHTML = '<p class="text-muted small mb-0">Vui lòng chọn bảng trước.</p>';
            return;
        }

        columnSelectionContainer.innerHTML = '<div class="spinner-border spinner-border-sm text-primary" role="status"></div> Đang tải...';

        try {
            // Load columns for base table
            const colResponse = await fetch(`/dynamic-report/columns?tableName=${encodeURIComponent(tableName)}`);
            const colResult = await colResponse.json();

            if (colResult.success) {
                columnsByTable[tableName] = colResult.data;
                renderAllColumns();
            }

            // Load available relations
            const relResponse = await fetch(`/dynamic-report/relations?tableName=${encodeURIComponent(tableName)}`);
            const relResult = await relResponse.json();

            if (relResult.success && relResult.data.length > 0) {
                availableRelations = relResult.data;
                joinSelectionGroup.style.display = 'block';
            }

            updateButtonsState();
        } catch (error) {
            console.error('Error:', error);
            columnSelectionContainer.innerHTML = '<p class="text-danger small mb-0">Lỗi kết nối.</p>';
        }
    }

    function addJoinRow() {
        if (availableRelations.length === 0) return;

        const clone = joinRowTemplate.content.cloneNode(true);
        const joinRow = clone.querySelector('.join-row');

        const joinSelect = joinRow.querySelector('.join-table-select');
        availableRelations.forEach(table => {
            const option = document.createElement('option');
            option.value = table;
            option.textContent = table;
            joinSelect.appendChild(option);
        });

        joinSelect.addEventListener('change', async function () {
            const selectedTable = this.value;
            if (selectedTable) {
                try {
                    // Pre-fetch columns for the new table
                    if (!columnsByTable[selectedTable]) {
                        const res = await fetch(`/dynamic-report/columns?tableName=${encodeURIComponent(selectedTable)}`);
                        const result = await res.json();
                        if (result.success) {
                            columnsByTable[selectedTable] = result.data;
                        }
                    }
                } catch (err) {
                    console.error(err);
                }
            }
            rebuildColumnsFromActiveTables();
        });

        const removeBtn = joinRow.querySelector('.remove-join-btn');
        removeBtn.addEventListener('click', () => {
            joinRow.remove();
            rebuildColumnsFromActiveTables();
        });

        joinsContainer.appendChild(joinRow);
        updateJoinSelectOptions();
    }

    async function rebuildColumnsFromActiveTables() {
        const baseTable = tableNameSelect.value;
        const activeTables = [baseTable];

        joinsContainer.querySelectorAll('.join-table-select').forEach(select => {
            if (select.value) activeTables.push(select.value);
        });

        // Update columnsByTable to only keep active ones (plus base)
        const newColumnsByTable = {};
        for (const t of activeTables) {
            if (columnsByTable[t]) {
                newColumnsByTable[t] = columnsByTable[t];
            }
        }
        columnsByTable = newColumnsByTable;
        
        // --- SECURITY & PERFORMANCE: Update availableRelations from ALL active tables ---
        const newAvailableSet = new Set();
        for (const table of activeTables) {
            try {
                const res = await fetch(`/dynamic-report/relations?tableName=${encodeURIComponent(table)}`);
                const result = await res.json();
                if (result.success) {
                    result.data.forEach(target => {
                        // Only add if not already in the query
                        if (!activeTables.includes(target)) {
                            newAvailableSet.add(target);
                        }
                    });
                }
            } catch (err) { console.error(err); }
        }
        availableRelations = Array.from(newAvailableSet);
        // -----------------------------------------------------------------------------

        // Remove selected columns that are no longer in active tables
        selectedColumnsState = selectedColumnsState.filter(c => activeTables.includes(c.TableName));

        renderAllColumns();
        renderAggregationSection();
        updateButtonsState();
        updateJoinSelectOptions();
    }

    function updateJoinSelectOptions() {

        joinSelects.forEach(select => {
            Array.from(select.options).forEach(option => {
                if (!option.value) return; // Skip placeholder

                // Hide option if selected in another dropdown
                if (selectedValues.includes(option.value) && select.value !== option.value) {
                    option.style.display = 'none';
                } else {
                    option.style.display = '';
                }
            });
        });
    }

    function renderAllColumns() {
        // We use selectedColumnsState to determine checked status
        const isSelected = (tableName, colName) =>
            selectedColumnsState.some(c => c.TableName === tableName && c.ColumnName === colName);

        let html = '';
        const tables = Object.keys(columnsByTable);

        if (tables.length === 0) {
            columnSelectionContainer.innerHTML = '<p class="text-muted small mb-0">Không có cột nào.</p>';
            return;
        }

        tables.forEach(table => {
            html += `<h6 class="mt-2 mb-1 text-primary small fw-bold">Bảng: ${table}</h6>`;
            html += `<div class="d-flex flex-wrap gap-2 mb-2">`;
            columnsByTable[table].forEach((col, index) => {
                const id = `col_${table}_${index}`;
                const val = `${table}.${col}`;
                const checkedAttr = isSelected(table, col) ? 'checked' : '';
                html += `
                    <div class="border rounded p-1 px-2" style="background:#f8f9fa; min-width:120px">
                        <div class="form-check mb-0">
                            <input class="form-check-input column-checkbox" type="checkbox" 
                                value="${val}" data-table="${table}" data-column="${col}" id="${id}" ${checkedAttr}>
                            <label class="form-check-label small fw-semibold text-truncate" style="max-width:110px; cursor:pointer" for="${id}" title="${col}">${col}</label>
                        </div>
                    </div>
                `;
            });
            html += `</div>`;
        });

        columnSelectionContainer.innerHTML = html;

        // Listen for checkbox changes
        columnSelectionContainer.querySelectorAll('.column-checkbox').forEach(cb => {
            cb.addEventListener('change', function() {
                const table = this.getAttribute('data-table');
                const col = this.getAttribute('data-column');
                
                if (this.checked) {
                    // Add if not already present
                    if (!selectedColumnsState.some(c => c.TableName === table && c.ColumnName === col)) {
                        selectedColumnsState.push({
                            TableName: table,
                            ColumnName: col,
                            Aggregate: 'None',
                            AliasName: '',
                            HavingOperator: null,
                            HavingValue: ''
                        });
                    }
                } else {
                    // Remove
                    selectedColumnsState = selectedColumnsState.filter(c => !(c.TableName === table && c.ColumnName === col));
                }
                
                renderAggregationSection();
                updateButtonsState();
            });
        });
    }

    function onToggleAggregation() {
        isAggregationEnabled = !isAggregationEnabled;
        
        if (isAggregationEnabled) {
            aggregationGroup.style.display = 'block';
            toggleAggregationBtn.classList.replace('btn-outline-secondary', 'btn-primary');
            toggleAggregationBtn.innerHTML = '<i class="bi bi-calculator-fill"></i> Đang bật Thống kê';
        } else {
            aggregationGroup.style.display = 'none';
            resetAggregationButton();
            // Reset state
            selectedColumnsState.forEach(c => {
                c.Aggregate = 'None';
                c.AliasName = '';
                c.HavingOperator = null;
                c.HavingValue = '';
            });
        }
        renderAggregationSection();
    }

    function resetAggregationButton() {
        toggleAggregationBtn.classList.remove('btn-primary');
        toggleAggregationBtn.classList.add('btn-outline-secondary');
        toggleAggregationBtn.innerHTML = '<i class="bi bi-calculator"></i> Chế độ Thống kê (Group By)';
    }

    function renderAggregationSection() {
        if (selectedColumnsState.length === 0 || !isAggregationEnabled) {
            aggregationGroup.style.display = 'none';
            // aggregationContainer.innerHTML = ''; // Keep it for faster toggle? No, better clear
            return;
        }

        aggregationGroup.style.display = 'block';
        
        const aggregateOptions = [
            { value: 'None',  label: '— (Gom nhóm)' },
            { value: 'Count', label: 'Đếm (COUNT)' },
            { value: 'Sum',   label: 'Tổng (SUM)' },
            { value: 'Avg',   label: 'T.Bình (AVG)' },
            { value: 'Max',   label: 'Lớn nhất (MAX)' },
            { value: 'Min',   label: 'Nhỏ nhất (MIN)' },
        ];

        const havingOpOptionsList = [
            { value: '',  label: '-- Lọc kết quả --' },
            { value: '0', label: 'Bằng (=)' },
            { value: '1', label: 'Khác (!=)' },
            { value: '5', label: 'Lớn hơn (>)' },
            { value: '6', label: 'Lớn bằng (>=)' },
            { value: '7', label: 'Nhỏ hơn (<)' },
            { value: '8', label: 'Nhỏ bằng (<=)' }
        ];

        let html = '<div class="small text-muted mb-2"><i class="bi bi-info-circle"></i> Các cột không chọn hàm thống kê sẽ tự động được đưa vào <b>GROUP BY</b>.</div>';
        
        selectedColumnsState.forEach((col, index) => {
            const aggOptionsHtml = aggregateOptions.map(o => 
                `<option value="${o.value}" ${col.Aggregate === o.value ? 'selected' : ''}>${o.label}</option>`
            ).join('');

            const havingOpHtml = havingOpOptionsList.map(o => 
                `<option value="${o.value}" ${col.HavingOperator == o.value ? 'selected' : ''}>${o.label}</option>`
            ).join('');
            
            const hasAgg = col.Aggregate !== 'None';
            
            html += `
                <div class="agg-item border rounded p-2 mb-2 bg-white shadow-sm" data-index="${index}">
                    <div class="d-flex align-items-center justify-content-between mb-1">
                        <span class="small fw-bold text-primary text-truncate" title="${col.TableName}.${col.ColumnName}">
                            ${col.ColumnName} <small class="text-muted fw-normal">(${col.TableName})</small>
                        </span>
                        <div class="btn-group btn-group-sm">
                            <button type="button" class="btn btn-outline-success py-0 px-1 add-extra-agg-btn" title="Thêm phép tính khác cho cột này">
                                <i class="bi bi-plus-circle"></i>
                            </button>
                            <button type="button" class="btn btn-outline-danger py-0 px-1 remove-agg-btn" title="Xóa">
                                <i class="bi bi-trash"></i>
                            </button>
                        </div>
                    </div>
                    <div class="row g-2 align-items-center mb-1">
                        <div class="col-5">
                            <select class="form-select form-select-sm item-agg-select">
                                ${aggOptionsHtml}
                            </select>
                        </div>
                        <div class="col-7">
                            <input type="text" class="form-control form-control-sm item-alias-input" 
                                placeholder="Tên hiển thị..." 
                                value="${col.AliasName}"
                                style="display:${hasAgg ? 'block' : 'none'}">
                        </div>
                    </div>
                    <div class="row g-1 align-items-center mt-1 pt-1 border-top item-having-row" style="display:${hasAgg ? 'flex' : 'none'}">
                        <div class="col-5">
                            <select class="form-select form-select-sm item-having-op">
                                ${havingOpHtml}
                            </select>
                        </div>
                        <div class="col-7">
                            <input type="text" class="form-control form-control-sm item-having-val" 
                                placeholder="Lọc giá trị..." 
                                value="${col.HavingValue || ''}">
                        </div>
                    </div>
                </div>
            `;
        });

        aggregationContainer.innerHTML = html;

        // Sync changes back to state
        aggregationContainer.querySelectorAll('.item-agg-select').forEach(sel => {
            sel.addEventListener('change', function() {
                const index = parseInt(this.closest('.agg-item').getAttribute('data-index'));
                selectedColumnsState[index].Aggregate = this.value;
                
                // Show/hide alias & having row
                const itemRow = this.closest('.agg-item');
                const aliasInput = itemRow.querySelector('.item-alias-input');
                const havingRow = itemRow.querySelector('.item-having-row');
                
                aliasInput.style.display = this.value !== 'None' ? 'block' : 'none';
                havingRow.style.display = this.value !== 'None' ? 'flex' : 'none';
                
                if (this.value === 'None') {
                    selectedColumnsState[index].AliasName = '';
                    selectedColumnsState[index].HavingOperator = null;
                    selectedColumnsState[index].HavingValue = '';
                    aliasInput.value = '';
                    havingRow.querySelector('.item-having-op').value = '';
                    havingRow.querySelector('.item-having-val').value = '';
                }
            });
        });

        aggregationContainer.querySelectorAll('.item-alias-input').forEach(input => {
            input.addEventListener('input', function() {
                const index = parseInt(this.closest('.agg-item').getAttribute('data-index'));
                selectedColumnsState[index].AliasName = this.value;
            });
        });

        aggregationContainer.querySelectorAll('.item-having-op').forEach(sel => {
            sel.addEventListener('change', function() {
                const index = parseInt(this.closest('.agg-item').getAttribute('data-index'));
                selectedColumnsState[index].HavingOperator = this.value ? parseInt(this.value, 10) : null;
            });
        });

        aggregationContainer.querySelectorAll('.item-having-val').forEach(input => {
            input.addEventListener('input', function() {
                const index = parseInt(this.closest('.agg-item').getAttribute('data-index'));
                selectedColumnsState[index].HavingValue = this.value;
            });
        });

        // Add extra aggregation rule
        aggregationContainer.querySelectorAll('.add-extra-agg-btn').forEach(btn => {
            btn.addEventListener('click', function() {
                const index = parseInt(this.closest('.agg-item').getAttribute('data-index'));
                const baseCol = selectedColumnsState[index];
                
                // Add a duplicate entry to state
                selectedColumnsState.splice(index + 1, 0, {
                    TableName: baseCol.TableName,
                    ColumnName: baseCol.ColumnName,
                    Aggregate: 'None',
                    AliasName: '',
                    HavingOperator: null,
                    HavingValue: ''
                });
                
                renderAggregationSection();
            });
        });

        // Remove aggregation rule
        aggregationContainer.querySelectorAll('.remove-agg-btn').forEach(btn => {
            btn.addEventListener('click', function() {
                const index = parseInt(this.closest('.agg-item').getAttribute('data-index'));
                const colToRemove = selectedColumnsState[index];
                
                selectedColumnsState.splice(index, 1);
                
                // If no more entries for this column exist, uncheck it in Section 3
                if (!selectedColumnsState.some(c => c.TableName === colToRemove.TableName && c.ColumnName === colToRemove.ColumnName)) {
                    const checkbox = columnSelectionContainer.querySelector(`.column-checkbox[data-table="${colToRemove.TableName}"][data-column="${colToRemove.ColumnName}"]`);
                    if (checkbox) checkbox.checked = false;
                }
                
                renderAggregationSection();
                updateButtonsState();
            });
        });
    }

    function addFilterRow() {
        const clone = filterRowTemplate.content.cloneNode(true);
        const filterRow = clone.querySelector('.filter-row');

        // Populate column options from all active tables
        const columnSelect = filterRow.querySelector('.filter-column-select');
        Object.keys(columnsByTable).forEach(table => {
            const optgroup = document.createElement('optgroup');
            optgroup.label = `Bảng: ${table}`;
            columnsByTable[table].forEach(col => {
                const option = document.createElement('option');
                option.value = `${table}.${col}`;
                option.textContent = col;
                optgroup.appendChild(option);
            });
            columnSelect.appendChild(optgroup);
        });

        const valueInput = filterRow.querySelector('.filter-value-input');
        columnSelect.addEventListener('change', function () {
            const val = this.value;
            if (!val) {
                valueInput.placeholder = "Nhập giá trị...";
                return;
            }
            const colName = val.split('.')[1];
            valueInput.placeholder = getFilterPlaceholder(colName);
        });

        // Trigger once to set initial placeholder
        columnSelect.dispatchEvent(new Event('change'));

        const removeBtn = filterRow.querySelector('.remove-filter-btn');
        removeBtn.addEventListener('click', () => {
            filterRow.remove();
        });

        filtersContainer.appendChild(filterRow);
    }

    function updateButtonsState() {
        const hasTable = tableNameSelect.value !== '';
        const hasSelectedColumns = getSelectedColumns().length > 0;
        
        addFilterBtn.disabled = !hasTable;
        toggleAggregationBtn.disabled = !hasSelectedColumns;
        previewBtn.disabled = !(hasTable && hasSelectedColumns);
        generatePdfBtn.disabled = !(hasTable && hasSelectedColumns);
    }

    function getSelectedColumns() {
        return selectedColumnsState;
    }

    function getFilters() {
        const filterRows = filtersContainer.querySelectorAll('.filter-row');
        const filters = [];

        filterRows.forEach(row => {
            const colVal = row.querySelector('.filter-column-select').value;
            const operator = parseInt(row.querySelector('.filter-operator-select').value, 10);
            const value = row.querySelector('.filter-value-input').value;

            if (colVal && value) {
                const parts = colVal.split('.');
                filters.push({
                    TableName: parts[0],
                    ColumnName: parts[1],
                    Operator: operator,
                    Value: value
                });
            }
        });

        return filters;
    }

    function getJoins() {
        const joinSelects = joinsContainer.querySelectorAll('.join-table-select');
        const joins = [];
        joinSelects.forEach(select => {
            if (select.value) {
                joins.push({
                    JoinTable: select.value,
                    JoinType: "INNER JOIN"
                });
            }
        });
        return joins;
    }

    function getRequestPayload() {
        return {
            TableName: tableNameSelect.value,
            Joins: getJoins(),
            AdvancedSelectColumns: getSelectedColumns(),
            Filters: getFilters(),
            PageNumber: 1,
            PageSize: 50
        };
    }

    async function previewData() {
        const payload = getRequestPayload();

        previewBtn.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Đang tải...';
        previewBtn.disabled = true;

        try {
            const response = await fetch('/dynamic-report/preview', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': getAntiForgeryToken()
                },
                body: JSON.stringify(payload)
            });

            const result = await response.json();

            if (result.success) {
                renderPreviewTable(result.data, result.columns);
            } else {
                showAlert('Lỗi xem trước dữ liệu: ' + result.message, 'danger');
            }
        } catch (error) {
            console.error('Error previewing data:', error);
            showAlert('Đã xảy ra lỗi khi xem trước dữ liệu.', 'danger');
        } finally {
            previewBtn.innerHTML = '<i class="bi bi-table"></i> Xem trước dữ liệu';
            updateButtonsState();
        }
    }

    function renderPreviewTable(data, columns) {
        recordCountBadge.textContent = `${data.length} dòng`;

        if (data.length === 0) {
            previewTableHead.innerHTML = '<tr><th class="text-center text-muted">Không có dữ liệu phù hợp</th></tr>';
            previewTableBody.innerHTML = '<tr><td class="text-center text-muted py-4">Thử thay đổi điều kiện lọc.</td></tr>';
            return;
        }

        // Render Head
        let headHtml = '<tr>';
        columns.forEach(col => {
            headHtml += `<th>${col}</th>`;
        });
        headHtml += '</tr>';
        previewTableHead.innerHTML = headHtml;

        // Render Body
        let bodyHtml = '';
        data.forEach(row => {
            bodyHtml += '<tr>';
            columns.forEach(col => {
                const val = row[col];
                bodyHtml += `<td>${val !== null && val !== undefined ? val : '<em>NULL</em>'}</td>`;
            });
            bodyHtml += '</tr>';
        });
        previewTableBody.innerHTML = bodyHtml;
    }

    async function generatePdf() {
        const payload = getRequestPayload();

        generatePdfBtn.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Đang tạo...';
        generatePdfBtn.disabled = true;

        try {
            const response = await fetch('/dynamic-report/generate', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': getAntiForgeryToken()
                },
                body: JSON.stringify(payload)
            });

            if (response.ok) {
                const blob = await response.blob();
                const url = window.URL.createObjectURL(blob);
                const a = document.createElement('a');
                a.style.display = 'none';
                a.href = url;
                const filename = `DynamicReport_${payload.TableName}_${new Date().getTime()}.pdf`;
                a.download = filename;
                document.body.appendChild(a);
                a.click();
                window.URL.revokeObjectURL(url);
            } else {
                const errorText = await response.text();
                showAlert('Lỗi tạo báo cáo: ' + errorText, 'danger');
            }
        } catch (error) {
            console.error('Error generating PDF:', error);
            showAlert('Đã xảy ra lỗi khi tạo báo cáo PDF.', 'danger');
        } finally {
            generatePdfBtn.innerHTML = '<i class="bi bi-file-pdf"></i> Xuất PDF';
            updateButtonsState();
        }
    }

    function getAntiForgeryToken() {
        const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
        return tokenInput ? tokenInput.value : '';
    }

    function showAlert(message, type) {
        const alertDiv = document.createElement('div');
        alertDiv.className = `alert alert-${type} alert-dismissible fade show`;
        alertDiv.style.position = 'fixed';
        alertDiv.style.top = '20px';
        alertDiv.style.right = '20px';
        alertDiv.style.zIndex = '9999';
        alertDiv.style.minWidth = '300px';
        alertDiv.style.boxShadow = '0 4px 6px rgba(0,0,0,0.1)';
        alertDiv.innerHTML = `
            <strong>Thông báo:</strong> ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        `;
        document.body.appendChild(alertDiv);

        setTimeout(() => {
            alertDiv.classList.remove('show');
            setTimeout(() => alertDiv.remove(), 150);
        }, 5000);
    }

    function getFilterPlaceholder(columnName) {
        if (!columnName) return "Nhập giá trị...";
        const colName = columnName.toUpperCase();

        if (colName.includes('NGAY')) {
            return "YYYY-MM-DD (VD: 2004-12-31)";
        } else if (colName.includes('MASV')) {
            return "VD: N20DCVT001";
        } else if (colName.includes('MAGV')) {
            return "VD: GV01";
        } else if (colName.includes('MALOP')) {
            return "VD: D20CQCN01-N";
        } else if (colName.includes('PHAI')) {
            return "VD: True (Nam) / False (Nữ)";
        } else if (colName.includes('TEN') || colName.includes('HO')) {
            return "VD: Nguyễn Văn A";
        } else if (colName.includes('DIEM') || colName.includes('HESO') || colName.includes('SOTC')) {
            return "VD: 8.5";
        } else {
            return "Nhập giá trị...";
        }
    }
});
