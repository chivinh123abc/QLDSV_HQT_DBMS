/**
 * @file constants.js
 * @description Centralized configuration for Selectors, Endpoints and Enums.
 */

export const SELECTORS = {
    tableNameSelect: '#tableNameSelect',
    joinSelectionGroup: '#joinSelectionGroup',
    addJoinBtn: '#addJoinBtn',
    joinsContainer: '#joinsContainer',
    columnSelectionContainer: '#columnSelectionContainer',
    addFilterBtn: '#addFilterBtn',
    filtersContainer: '#filtersContainer',
    previewBtn: '#previewBtn',
    generatePdfBtn: '#generatePdfBtn',
    previewTableHead: '#previewTableHead',
    previewTableBody: '#previewTableBody',
    recordCountBadge: '#recordCountBadge',
    aggregationGroup: '#aggregationGroup',
    aggregationContainer: '#aggregationContainer',
    toggleAggregationBtn: '#toggleAggregationBtn',
    sqlPreviewCode: '#sqlPreviewCode',
    copySqlBtn: '#copySqlBtn',
    copySqlBtnText: '#copySqlBtnText',
    copySqlIcon: '#copySqlIcon',
    previewBtnText: '#previewBtnText',
    generatePdfBtnText: '#generatePdfBtnText',
    joinRowTemplate: '#joinRowTemplate',
    filterRowTemplate: '#filterRowTemplate',
    // New fields
    addComputedColumnBtn: '#addComputedColumnBtn',
    computedColumnModal: '#computedColumnModal',
    computedColumnForm: '#computedColumnForm',
    computedCol1: '#computedCol1',
    computedOp: '#computedOp',
    computedCol2: '#computedCol2',
    computedLiteral: '#computedLiteral',
    computedAlias: '#computedAlias',
    reportTitleInput: '#reportTitleInput',
    fileNameInput: '#fileNameInput',
    fileNamePreview: '#fileNamePreview',
    selectedColumnsOrderContainer: '#selectedColumnsOrderContainer',
    sortableColumns: '#sortableColumns',
};

export const API_ENDPOINTS = {
    tables: '/dynamic-report/tables',
    columns: (table) => `/dynamic-report/columns?tableName=${encodeURIComponent(table)}`,
    relations: (table) => `/dynamic-report/relations?tableName=${encodeURIComponent(table)}`,
    sqlPreview: '/dynamic-report/sql',
    preview: '/dynamic-report/preview',
    generate: '/dynamic-report/generate',
};

export const OPERATORS = [
    { value: '0', label: 'Bằng (=)' },
    { value: '1', label: 'Không bằng (!=)' },
    { value: '2', label: 'Chứa' },
    { value: '3', label: 'Bắt đầu bằng' },
    { value: '4', label: 'Kết thúc bằng' },
    { value: '5', label: 'Lớn hơn (>)' },
    { value: '6', label: 'Lớn hơn hoặc bằng (>=)' },
    { value: '7', label: 'Nhỏ hơn (<)' },
    { value: '8', label: 'Nhỏ hơn hoặc bằng (<=)' },
    { value: '9', label: 'Trống (IS NULL)' },
    { value: '10', label: 'Không trống (IS NOT NULL)' },
    { value: '11', label: 'Thuộc danh sách (IN)' },
    { value: '12', label: 'Không thuộc DS (NOT IN)' },
    { value: '13', label: 'Trong khoảng (BETWEEN)' },
    { value: '14', label: 'Không chứa (NOT LIKE)' }
];

export const AGGREGATE_FUNCTIONS = [
    { value: 'None',  label: '— (Gom nhóm / Group By)' },
    { value: 'Count', label: 'Đếm (COUNT)' },
    { value: 'Sum',   label: 'Tổng (SUM)' },
    { value: 'Avg',   label: 'Trung bình (AVG)' },
    { value: 'Max',   label: 'Lớn nhất (MAX)' },
    { value: 'Min',   label: 'Nhỏ nhất (MIN)' }
];

export const HAVING_OPERATORS = [
    { value: '',  label: '-- Lọc kết quả (HAVING) --' },
    { value: '0', label: 'Bằng (=)' },
    { value: '1', label: 'Khác (!=)' },
    { value: '5', label: 'Lớn hơn (>)' },
    { value: '6', label: 'Lớn bằng (>=)' },
    { value: '7', label: 'Nhỏ hơn (<)' },
    { value: '8', label: 'Nhỏ bằng (<=)' }
];
