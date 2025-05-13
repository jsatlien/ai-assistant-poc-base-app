<template>
  <div class="inventory-manager">
    <div class="page-header">
      <h1>Inventory Management</h1>
      <div class="header-actions" v-if="canManageInventory">
        <button @click="openAddModal" class="btn btn-primary">
          <i class="fas fa-plus"></i> Add Inventory Item
        </button>
      </div>
    </div>

    <div class="group-info" v-if="currentGroup">
      <h2>Inventory for: {{ currentGroup.description }}</h2>
    </div>
    <div class="alert alert-warning" v-else>
      <i class="fas fa-exclamation-triangle"></i> Please select a group to manage inventory
    </div>

    <!-- Filters -->
    <div class="filters">
      <div class="filter-group">
        <label>Filter by Type:</label>
        <select v-model="filters.type" class="form-control">
          <option value="">All Types</option>
          <option value="Part">Parts</option>
          <option value="Device">Devices</option>
        </select>
      </div>
      <div class="filter-group">
        <label>Filter by Name:</label>
        <input type="text" v-model="filters.name" class="form-control" placeholder="Search by name...">
      </div>
      <div class="filter-group checkbox-group">
        <div class="checkbox-container">
          <input type="checkbox" id="lowStockFilter" v-model="filters.lowStock">
          <label for="lowStockFilter">Show Low Stock Only</label>
        </div>
      </div>
    </div>

    <!-- Inventory Table -->
    <DataTable 
      :columns="tableColumns" 
      :items="filteredInventory" 
      :row-class-function="getRowClass"
      :show-actions="canManageInventory"
      empty-message="No inventory items found"
    >
      <template #status="{ item }">
        <span :class="getStockStatusClass(item)">
          {{ getStockStatus(item) }}
        </span>
      </template>
      
      <template #manufacturer="{ item }">
        {{ getManufacturerName(item) }}
      </template>
      
      <template #lastUpdated="{ item }">
        {{ formatDate(item.lastUpdated) }}
      </template>
      
      <template #actions="{ item }">
        <ActionButton 
          type="info" 
          icon="edit" 
          label="Edit" 
          title="Edit inventory item"
          @click="openEditModal(item)" 
        />
        <ActionButton 
          type="danger" 
          icon="trash" 
          label="Delete" 
          title="Delete inventory item"
          margin="left"
          @click="confirmDelete(item)" 
        />
      </template>
    </DataTable>

    <!-- Add/Edit Modal -->
    <div class="modal" :class="{ 'show': showModal }" v-if="showModal">
      <div class="modal-dialog">
        <div class="modal-content inventory-modal">
          <div class="modal-header">
            <h2 class="modal-title">{{ isEditing ? 'Edit Inventory Item' : 'Add Inventory Item' }}</h2>
            <button type="button" class="close-btn" @click="closeModal">
              &times;
            </button>
          </div>
          <div class="modal-body">
            <form @submit.prevent="saveInventoryItem">
              <div class="form-group">
                <label for="catalogItem">Catalog Item</label>
                <select 
                  id="catalogItem"
                  v-model="currentItem.catalogItemId" 
                  class="form-control" 
                  :disabled="isEditing" 
                  required
                >
                  <option value="" disabled>Select an item</option>
                  <option v-for="item in catalogItems" :key="`${item.type}-${item.id}`" :value="item.id">
                    {{ item.name }} ({{ item.type }})
                  </option>
                </select>
              </div>
              <div class="form-group">
                <label for="quantity">Quantity</label>
                <input 
                  id="quantity"
                  type="number" 
                  v-model.number="currentItem.quantity" 
                  class="form-control" 
                  min="0" 
                  required
                >
              </div>
              <div class="form-group">
                <label for="minQuantity">Minimum Quantity</label>
                <input 
                  id="minQuantity"
                  type="number" 
                  v-model.number="currentItem.minimumQuantity" 
                  class="form-control" 
                  min="0" 
                  required
                >
              </div>
              <div class="form-actions">
                <ActionButton 
                  type="secondary" 
                  label="Cancel" 
                  margin="right"
                  @click="closeModal" 
                />
                <ActionButton 
                  type="primary" 
                  label="Save" 
                  @click="saveInventoryItem" 
                />
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>

    <!-- Delete Confirmation Modal -->
    <div class="modal" :class="{ 'show': showDeleteModal }" v-if="showDeleteModal">
      <div class="modal-dialog">
        <div class="modal-content confirmation-modal">
          <div class="modal-header">
            <h2 class="modal-title">Confirm Delete</h2>
            <button type="button" class="close-btn" @click="showDeleteModal = false">
              &times;
            </button>
          </div>
          <div class="modal-body">
            <p>Are you sure you want to delete the inventory record for <strong>{{ itemToDelete?.catalogItemName }}</strong>?</p>
            <div class="form-actions">
              <ActionButton 
                type="secondary" 
                label="Cancel" 
                margin="right"
                @click="showDeleteModal = false" 
              />
              <ActionButton 
                type="danger" 
                label="Delete" 
                @click="deleteItem" 
              />
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { mapGetters, mapActions } from 'vuex'
import DataTable from '@/components/DataTable.vue'
import ActionButton from '@/components/ActionButton.vue'

export default {
  name: 'InventoryManager',
  components: {
    DataTable,
    ActionButton
  },
  data() {
    return {
      showModal: false,
      showDeleteModal: false,
      isEditing: false,
      currentItem: {
        catalogItemId: '',
        catalogItemType: '',
        catalogItemName: '',
        quantity: 0,
        minimumQuantity: 0
      },
      itemToDelete: null,
      filters: {
        type: '',
        name: '',
        lowStock: false
      },
      tableColumns: [
        { key: 'catalogItemName', label: 'Item Name' },
        { key: 'catalogItemType', label: 'Type' },
        { key: 'manufacturer', label: 'Manufacturer' },
        { key: 'quantity', label: 'Quantity' },
        { key: 'minimumQuantity', label: 'Min. Quantity' },
        { key: 'status', label: 'Status' },
        { key: 'lastUpdated', label: 'Last Updated' }
      ]
    }
  },
  computed: {
    ...mapGetters([
      'getAllCatalogItems',
      'getCurrentGroup',
      'getCurrentGroupInventory',
      'isAdmin',
      'getUserRoles',
      'getManufacturers'
    ]),
    canManageInventory() {
      // Admin users can always manage inventory
      if (this.isAdmin) {
        return true;
      }
      
      // Check if the user has a role with canManageInventory permission
      const currentUser = this.$store.state.user;
      if (currentUser && currentUser.roleId) {
        const userRole = this.getUserRoles.find(role => role.id === currentUser.roleId);
        return userRole && userRole.canManageInventory;
      }
      
      return false;
    },
    catalogItems() {
      return this.getAllCatalogItems
    },
    currentGroup() {
      return this.getCurrentGroup
    },
    filteredInventory() {
      let items = this.getCurrentGroupInventory

      // Apply type filter
      if (this.filters.type) {
        items = items.filter(item => item.catalogItemType === this.filters.type)
      }

      // Apply name filter
      if (this.filters.name) {
        const searchTerm = this.filters.name.toLowerCase()
        items = items.filter(item => 
          item.catalogItemName.toLowerCase().includes(searchTerm)
        )
      }

      // Apply low stock filter
      if (this.filters.lowStock) {
        items = items.filter(item => item.quantity < item.minimumQuantity)
      }

      return items
    }
  },
  methods: {
    ...mapActions([
      'fetchInventory',
      'addInventoryItem',
      'updateInventoryItem',
      'deleteInventoryItem'
    ]),
    formatDate(date) {
      if (!date) return 'N/A'
      const d = new Date(date)
      return d.toLocaleDateString() + ' ' + d.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
    },
    getStockStatus(item) {
      if (item.quantity <= 0) return 'Out of Stock'
      if (item.quantity < item.minimumQuantity) return 'Low Stock'
      return 'In Stock'
    },
    getStockStatusClass(item) {
      if (item.quantity <= 0) return 'status-out'
      if (item.quantity < item.minimumQuantity) return 'status-low'
      return 'status-ok'
    },
    getManufacturerName(item) {
      // Skip for services as they don't have manufacturers
      if (item.catalogItemType === 'Service') return '';
      
      // Find the catalog item
      const catalogItem = this.catalogItems.find(ci => ci.id === item.catalogItemId);
      if (!catalogItem || !catalogItem.manufacturerId) return 'N/A';
      
      // Find the manufacturer
      const manufacturer = this.getManufacturers.find(m => m.id === catalogItem.manufacturerId);
      return manufacturer ? manufacturer.name : 'N/A';
    },
    getRowClass(item) {
      return item.quantity < item.minimumQuantity ? 'low-stock' : ''
    },
    openAddModal() {
      if (!this.currentGroup) {
        alert('Please select a group first')
        return
      }
      
      this.isEditing = false
      this.currentItem = {
        catalogItemId: '',
        catalogItemType: '',
        catalogItemName: '',
        quantity: 0,
        minimumQuantity: 0
      }
      this.showModal = true
    },
    openEditModal(item) {
      this.isEditing = true
      this.currentItem = { ...item }
      this.showModal = true
    },
    closeModal() {
      this.showModal = false
      this.currentItem = {
        catalogItemId: '',
        catalogItemType: '',
        catalogItemName: '',
        quantity: 0,
        minimumQuantity: 0
      }
    },
    async saveInventoryItem() {
      if (!this.currentGroup) {
        alert('Please select a group first')
        return
      }
      
      try {
        // If adding a new item, we need to set the catalog item details
        if (!this.isEditing) {
          const selectedItem = this.catalogItems.find(item => item.id === this.currentItem.catalogItemId)
          if (selectedItem) {
            this.currentItem.catalogItemType = selectedItem.type
            this.currentItem.catalogItemName = selectedItem.name
            this.currentItem.groupId = this.currentGroup.id
          }
        }
        
        if (this.isEditing) {
          await this.updateInventoryItem(this.currentItem)
        } else {
          await this.addInventoryItem(this.currentItem)
        }
        
        this.closeModal()
      } catch (error) {
        console.error('Error saving inventory item:', error)
        alert('Failed to save inventory item')
      }
    },
    confirmDelete(item) {
      this.itemToDelete = item
      this.showDeleteModal = true
    },
    async deleteItem() {
      if (!this.itemToDelete) return
      
      try {
        await this.deleteInventoryItem({
          groupId: this.itemToDelete.groupId,
          catalogItemId: this.itemToDelete.catalogItemId,
          catalogItemType: this.itemToDelete.catalogItemType
        })
        
        this.showDeleteModal = false
        this.itemToDelete = null
      } catch (error) {
        console.error('Error deleting inventory item:', error)
        alert('Failed to delete inventory item')
      }
    }
  },
  created() {
    this.fetchInventory()
  }
}
</script>

<style scoped>
.inventory-manager {
  padding: 20px;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.group-info {
  margin-bottom: 20px;
  padding: 10px;
  background-color: #f8f9fa;
  border-radius: 4px;
}

.filters {
  display: flex;
  gap: 15px;
  margin-bottom: 20px;
  padding: 15px;
  background-color: #f8f9fa;
  border-radius: 4px;
}

.filter-group {
  display: flex;
  flex-direction: column;
  min-width: 200px;
}

.checkbox-group {
  display: flex;
  align-items: flex-end;
  padding-bottom: 8px;
}

.checkbox-container {
  display: flex;
  align-items: center;
  gap: 8px;
}

.checkbox-container input[type="checkbox"] {
  width: auto;
  margin-right: 5px;
}

/* Table Styles */
.table-responsive {
  overflow-x: auto;
  margin-bottom: 20px;
}

.inventory-table {
  width: 100%;
  border-collapse: collapse;
  border-spacing: 0;
  background-color: white;
}

.inventory-table th,
.inventory-table td {
  padding: 12px 15px;
  text-align: left;
  border-bottom: 1px solid #e0e0e0;
}

.inventory-table th {
  background-color: #f5f5f5;
  font-weight: 600;
  color: #333;
}

.inventory-table tbody tr:hover {
  background-color: rgba(0, 136, 204, 0.05);
}

.action-buttons {
  white-space: nowrap;
  display: flex;
  gap: 8px;
}

.low-stock {
  background-color: #fff3cd !important;
}

.status-out {
  color: #dc3545;
  font-weight: bold;
}

.status-low {
  color: #ffc107;
  font-weight: bold;
}

.status-ok {
  color: #28a745;
}

/* Modal Styles */
.modal {
  display: none;
  position: fixed;
  top: 0;
  left: 0;
  z-index: 1050;
  width: 100%;
  height: 100%;
  overflow: hidden;
  outline: 0;
  background-color: rgba(0, 0, 0, 0.5);
}

.modal.show {
  display: block;
}

.modal-dialog {
  position: relative;
  width: auto;
  margin: 1.75rem auto;
  max-width: 500px;
}

.inventory-modal {
  background-color: #fff;
  border-radius: 8px;
  width: 500px;
  max-width: 90%;
  max-height: 90vh;
  overflow-y: auto;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.15);
}

.confirmation-modal {
  width: 400px;
}

.modal-header {
  padding: 1.5rem;
  border-bottom: 1px solid #eee;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.modal-header h2 {
  margin: 0;
  font-size: 1.5rem;
  color: #333;
}

.close-btn {
  background: none;
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
  color: #999;
}

.close-btn:hover {
  color: #333;
}

.modal-body {
  padding: 1.5rem;
}

.form-group {
  margin-bottom: 1.5rem;
}

.form-group label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 500;
  color: #555;
}

.form-control {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 1rem;
}

.form-control:focus {
  border-color: #08c;
  outline: none;
  box-shadow: 0 0 0 2px rgba(0, 136, 204, 0.2);
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
  margin-top: 1.5rem;
}

.ml-2 {
  margin-left: 0.5rem;
}

.mr-2 {
  margin-right: 0.5rem;
}
</style>
