/**
 * @file state-manager.js
 * @description Application state management for the dynamic report module.
 */

export const State = {
    tableName: '',
    columnsByTable: {},
    availableRelations: [],
    selectedColumns: [], // Array of { TableName, ColumnName, Aggregate, AliasName, HavingOperator, HavingValue }
    isAggregationEnabled: false,

    reset(tableName = '') {
        this.tableName = tableName;
        this.columnsByTable = {};
        this.availableRelations = [];
        this.selectedColumns = [];
        this.isAggregationEnabled = false;
    },

    setColumns(tableName, columns) {
        this.columnsByTable[tableName] = columns;
    },

    toggleColumn(tableName, colName, isChecked) {
        if (isChecked) {
            if (!this.selectedColumns.some(c => c.TableName === tableName && c.ColumnName === colName)) {
                this.selectedColumns.push({
                    TableName: tableName,
                    ColumnName: colName,
                    Aggregate: 'None',
                    AliasName: '',
                    HavingOperator: null,
                    HavingValue: ''
                });
            }
        } else {
            this.selectedColumns = this.selectedColumns.filter(c => !(c.TableName === tableName && c.ColumnName === colName));
        }
    },

    updateColumnProperty(index, prop, value) {
        if (this.selectedColumns[index]) {
            this.selectedColumns[index][prop] = value;
        }
    },

    addColumnAlias(tableName, colName, alias) {
        const col = this.selectedColumns.find(c => c.TableName === tableName && c.ColumnName === colName);
        if (col) col.AliasName = alias;
    },

    duplicateAggregation(index) {
        const base = this.selectedColumns[index];
        const newCol = { ...base, Aggregate: 'None', AliasName: '', HavingOperator: null, HavingValue: '' };
        this.selectedColumns.splice(index + 1, 0, newCol);
    },

    removeAggregation(index) {
        const colToRemove = this.selectedColumns[index];
        this.selectedColumns.splice(index, 1);
        
        // Return true if this column is no longer present in ANY aggregation row
        return !this.selectedColumns.some(c => c.TableName === colToRemove.TableName && c.ColumnName === colToRemove.ColumnName);
    }
};
