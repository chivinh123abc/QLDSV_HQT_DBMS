/**
 * @file state-manager.js
 * @description Application state management for the dynamic report module.
 */

export const State = {
    tableName: '',
    columnsByTable: {},
    availableRelations: [],
    selectedColumns: [], // Array of { TableName, ColumnName, Expression, Aggregate, AliasName, HavingOperator, HavingValue, IsComputed }
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
            if (!this.selectedColumns.some(c => c.TableName === tableName && c.ColumnName === colName && !c.IsComputed)) {
                this.selectedColumns.push({
                    TableName: tableName,
                    ColumnName: colName,
                    Expression: null,
                    IsComputed: false,
                    Aggregate: 'None',
                    AliasName: '',
                    HavingOperator: null,
                    HavingValue: '',
                    ShowInAgg: false
                });
            }
        } else {
            this.selectedColumns = this.selectedColumns.filter(c => !(c.TableName === tableName && c.ColumnName === colName && !c.IsComputed));
        }
    },

    addComputedColumn(expression, alias, parts = null) {
        this.selectedColumns.push({
            TableName: this.tableName, // Bind to primary table by default
            ColumnName: alias, // Provide a fake name for UI purposes if needed, though backend uses Expression
            Expression: expression,
            IsComputed: true,
            Aggregate: 'None',
            AliasName: alias, // Backend requires alias if Expression is not null
            HavingOperator: null,
            HavingValue: '',
            ShowInAgg: false,
            ComputedParts: parts
        });
    },

    removeComputedColumn(expression, alias) {
        this.selectedColumns = this.selectedColumns.filter(c => !(c.IsComputed && c.Expression === expression && c.AliasName === alias));
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
        const newCol = { ...base, Aggregate: 'Count', AliasName: '', HavingOperator: null, HavingValue: '', ShowInAgg: true };
        this.selectedColumns.splice(index + 1, 0, newCol);
    },

    removeAggregation(index) {
        const colToRemove = this.selectedColumns[index];
        const copies = this.selectedColumns.filter(c => c.TableName === colToRemove.TableName && c.ColumnName === colToRemove.ColumnName && c.Expression === colToRemove.Expression);
        
        if (copies.length > 1) {
            this.selectedColumns.splice(index, 1);
            return false;
        } else {
            colToRemove.Aggregate = 'None';
            colToRemove.AliasName = colToRemove.IsComputed ? colToRemove.AliasName : '';
            colToRemove.HavingOperator = null;
            colToRemove.HavingValue = '';
            colToRemove.ShowInAgg = false;
            return false;
        }
    }
};
