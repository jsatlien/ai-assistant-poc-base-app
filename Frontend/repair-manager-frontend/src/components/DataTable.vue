<template>
  <div class="data-table-container">
    <div class="card">
      <div class="table-responsive">
        <table class="data-table">
          <thead>
            <tr>
              <th v-for="column in columns" :key="column.key">{{ column.label }}</th>
              <th v-if="showActions">Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="items.length === 0">
              <td :colspan="showActions ? columns.length + 1 : columns.length" class="text-center">
                {{ emptyMessage || 'No data available' }}
              </td>
            </tr>
            <tr v-for="(item, index) in items" :key="index" :class="getRowClass(item)">
              <td v-for="column in columns" :key="column.key">
                <slot :name="column.key" :item="item">
                  <span v-if="column.formatter" v-html="column.formatter(item[column.key], item)"></span>
                  <span v-else>{{ item[column.key] }}</span>
                </slot>
              </td>
              <td v-if="showActions" class="action-buttons">
                <slot name="actions" :item="item"></slot>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'DataTable',
  props: {
    columns: {
      type: Array,
      required: true,
      // Each column should have { key, label, formatter? }
    },
    items: {
      type: Array,
      required: true
    },
    showActions: {
      type: Boolean,
      default: true
    },
    emptyMessage: {
      type: String,
      default: 'No data available'
    },
    rowClassFunction: {
      type: Function,
      default: null
    }
  },
  methods: {
    getRowClass(item) {
      if (this.rowClassFunction) {
        return this.rowClassFunction(item);
      }
      return '';
    }
  }
}
</script>

<style scoped>
.data-table-container {
  margin-bottom: 20px;
}

.table-responsive {
  overflow-x: auto;
  margin-bottom: 20px;
}

.data-table {
  width: 100%;
  border-collapse: collapse;
  border-spacing: 0;
  background-color: white;
}

.data-table th,
.data-table td {
  padding: 12px 15px;
  text-align: left;
  border-bottom: 1px solid #e0e0e0;
}

.data-table th {
  background-color: #f5f5f5;
  font-weight: 600;
  color: #333;
}

.data-table tbody tr:hover {
  background-color: rgba(0, 136, 204, 0.05);
}

.action-buttons {
  white-space: nowrap;
  display: flex;
  gap: 8px;
}

.text-center {
  text-align: center;
}
</style>
