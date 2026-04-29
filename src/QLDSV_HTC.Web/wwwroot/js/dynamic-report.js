document.addEventListener('DOMContentLoaded', function () {
    const tableNameSelect = document.getElementById('tableNameSelect');
    const columnSelectionContainer = document.getElementById('columnSelectionContainer');
    const addFilterBtn = document.getElementById('addFilterBtn');
    const filtersContainer = document.getElementById('filtersContainer');
    const previewBtn = document.getElementById('previewBtn');
    const generatePdfBtn = document.getElementById('generatePdfBtn');
    const previewTableHead = document.getElementById('previewTableHead');
    const previewTableBody = document.getElementById('previewTableBody');
    const recordCountBadge = document.getElementById('recordCountBadge');
    const filterRowTemplate = document.getElementById('filterRowTemplate');

    let availableColumns = [];

    // Initialize
    loadTables();

    // Event Listeners
    tableNameSelect.addEventListener('change', onTableChange);
    addFilterBtn.addEventListener('click', addFilterRow);
    previewBtn.addEventListener('click', previewData);
    generatePdfBtn.addEventListener('click', generatePdf);

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
        filtersContainer.innerHTML = '';
        availableColumns = [];
        updateButtonsState();

        if (!tableName) {
            columnSelectionContainer.innerHTML = '<p class="text-muted small mb-0">Vui lòng chọn bảng trước.</p>';
            return;
        }

        columnSelectionContainer.innerHTML = '<div class="spinner-border spinner-border-sm text-primary" role="status"></div> Đang tải...';

        try {
            const response = await fetch(`/dynamic-report/columns?tableName=${encodeURIComponent(tableName)}`);
            const result = await response.json();

            if (result.success) {
                availableColumns = result.data;
                renderColumnCheckboxes(availableColumns);
                updateButtonsState();
            } else {
                columnSelectionContainer.innerHTML = `<p class="text-danger small mb-0">${result.message}</p>`;
                showAlert('Lỗi tải danh sách cột: ' + result.message, 'danger');
            }
        } catch (error) {
            console.error('Error loading columns:', error);
            columnSelectionContainer.innerHTML = '<p class="text-danger small mb-0">Lỗi kết nối.</p>';
        }
    }

    function renderColumnCheckboxes(columns) {
        if (!columns || columns.length === 0) {
            columnSelectionContainer.innerHTML = '<p class="text-muted small mb-0">Bảng này không có cột nào.</p>';
            return;
        }

        let html = '';
        columns.forEach((col, index) => {
            html += `
                <div class="form-check">
                    <input class="form-check-input column-checkbox" type="checkbox" value="${col}" id="col_${index}" checked>
                    <label class="form-check-label" for="col_${index}">
                        ${col}
                    </label>
                </div>
            `;
        });
        columnSelectionContainer.innerHTML = html;

        // Add change listener to checkboxes to enable/disable preview/generate buttons
        const checkboxes = columnSelectionContainer.querySelectorAll('.column-checkbox');
        checkboxes.forEach(cb => {
            cb.addEventListener('change', updateButtonsState);
        });
    }

    function addFilterRow() {
        const clone = filterRowTemplate.content.cloneNode(true);
        const filterRow = clone.querySelector('.filter-row');
        
        // Populate column options
        const columnSelect = filterRow.querySelector('.filter-column-select');
        availableColumns.forEach(col => {
            const option = document.createElement('option');
            option.value = col;
            option.textContent = col;
            columnSelect.appendChild(option);
        });

        // Add remove listener
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
        previewBtn.disabled = !(hasTable && hasSelectedColumns);
        generatePdfBtn.disabled = !(hasTable && hasSelectedColumns);
    }

    function getSelectedColumns() {
        const checkboxes = columnSelectionContainer.querySelectorAll('.column-checkbox:checked');
        return Array.from(checkboxes).map(cb => cb.value);
    }

    function getFilters() {
        const filterRows = filtersContainer.querySelectorAll('.filter-row');
        const filters = [];

        filterRows.forEach(row => {
            const column = row.querySelector('.filter-column-select').value;
            const operator = parseInt(row.querySelector('.filter-operator-select').value, 10);
            const value = row.querySelector('.filter-value-input').value;

            if (column && value) {
                filters.push({
                    ColumnName: column,
                    Operator: operator,
                    Value: value
                });
            }
        });

        return filters;
    }

    function getRequestPayload() {
        return {
            TableName: tableNameSelect.value,
            SelectColumns: getSelectedColumns(),
            Filters: getFilters(),
            PageNumber: 1, // Currently fixed, can be enhanced with pagination controls
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
                renderPreviewTable(result.data, payload.SelectColumns);
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
                // Suggest a filename
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
        // Create a temporary alert element using Bootstrap classes
        const alertDiv = document.createElement('div');
        alertDiv.className = `alert alert-${type} alert-dismissible fade show position-fixed top-0 end-0 m-3 z-3`;
        alertDiv.style.minWidth = '300px';
        alertDiv.innerHTML = `
            <strong>Thông báo:</strong> ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        `;
        document.body.appendChild(alertDiv);
        
        // Auto remove after 5 seconds
        setTimeout(() => {
            alertDiv.classList.remove('show');
            setTimeout(() => alertDiv.remove(), 150);
        }, 5000);
    }
});
