/**
 * @file helpers.js
 * @description Utility functions for string manipulation and formatting.
 */

/**
 * Escapes HTML special characters to prevent XSS.
 * @param {string} str 
 * @returns {string}
 */
export function escapeHtml(str) {
    if (!str) return "";
    const map = {
        '&': '&amp;',
        '<': '&lt;',
        '>': '&gt;',
        '"': '&quot;',
        "'": '&#039;'
    };
    return str.replace(/[&<>"']/g, m => map[m]);
}

/**
 * Highlights SQL syntax for preview.
 * @param {string} sql 
 * @returns {string}
 */
export function highlightSql(sql) {
    if (!sql) return "";
    
    // Keywords
    const keywords = ["SELECT", "FROM", "WHERE", "INNER JOIN", "LEFT JOIN", "GROUP BY", "HAVING", "ORDER BY", "AS", "ON", "AND", "OR", "IN", "BETWEEN", "IS NULL", "IS NOT NULL"];
    let highlighted = escapeHtml(sql);

    keywords.forEach(kw => {
        const regex = new RegExp(`\\b${kw}\\b`, 'gi');
        highlighted = highlighted.replace(regex, `<span class="sql-keyword">${kw}</span>`);
    });

    // Functions
    const funcs = ["COUNT", "SUM", "AVG", "MAX", "MIN"];
    funcs.forEach(f => {
        const regex = new RegExp(`\\b${f}\\b`, 'gi');
        highlighted = highlighted.replace(regex, `<span class="sql-func">${f}</span>`);
    });

    // Strings
    highlighted = highlighted.replace(/'(.*?)'/g, `<span class="sql-string">'$1'</span>`);

    // Numbers
    highlighted = highlighted.replace(/\b(\d+)\b/g, `<span class="sql-number">$1</span>`);

    return highlighted;
}

/**
 * Gets placeholder text for filter value input based on column name.
 * @param {string} colName 
 * @returns {string}
 */
export function getFilterPlaceholder(colName) {
    if (!colName) return "Nhập giá trị...";
    const lower = colName.toLowerCase();
    if (lower.includes('ngay') || lower.includes('date')) return "YYYY-MM-DD";
    if (lower.includes('ma') || lower.includes('id')) return "Nhập mã...";
    return "Nhập giá trị...";
}

/**
 * Gets Anti-Forgery Token from hidden input.
 * @returns {string}
 */
export function getAntiForgeryToken() {
    const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
    return tokenInput ? tokenInput.value : '';
}
