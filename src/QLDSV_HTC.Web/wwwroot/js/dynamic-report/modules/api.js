/**
 * @file api.js
 * @description All network requests for the dynamic report module.
 */

import { API_ENDPOINTS } from '../constants.js';
import { getAntiForgeryToken } from './helpers.js';

async function fetchJson(url, options = {}) {
    const response = await fetch(url, options);
    if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
    }
    return await response.json();
}

export const ApiService = {
    async getTables() {
        return await fetchJson(API_ENDPOINTS.tables);
    },

    async getColumns(tableName) {
        return await fetchJson(API_ENDPOINTS.columns(tableName));
    },

    async getRelations(tableName) {
        return await fetchJson(API_ENDPOINTS.relations(tableName));
    },

    async getSqlPreview(payload) {
        return await fetchJson(API_ENDPOINTS.sqlPreview, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': getAntiForgeryToken()
            },
            body: JSON.stringify(payload)
        });
    },

    async previewData(payload) {
        return await fetchJson(API_ENDPOINTS.preview, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': getAntiForgeryToken()
            },
            body: JSON.stringify(payload)
        });
    },

    async generatePdf(payload) {
        const response = await fetch(API_ENDPOINTS.generate, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': getAntiForgeryToken()
            },
            body: JSON.stringify(payload)
        });
        
        if (!response.ok) {
            const errorText = await response.text();
            throw new Error(errorText || 'Lỗi tạo báo cáo PDF');
        }
        
        return await response.blob();
    }
};
