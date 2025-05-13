<template>
  <div class="inventory-manager">
    <div class="page-header">
      <h1>Inventory Management</h1>
      <div class="header-actions">
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
          <option value="Service">Services</option>
        </select>
      </div>
      <div class="filter-group">
        <label>Filter by Name:</label>
        <input type="text" v-model="filters.name" class="form-control" placeholder="Search by name...">
      </div>
      <div class="filter-group">
        <label>Show Low Stock Only:</label>
        <input type="checkbox" v-model="filters.lowStock">
      </div>
    </div>

    <!-- Inventory Table -->
    <div class="card">
      <div class="table-responsive">
        <table class="inventory-table">
          <thead>
            <tr>
              <th>Item Name</th>
              <th>Type</th>
              <th>Quantity</th>
              <th>Min. Quantity</th>
              <th>Status</th>
              <th>Last Updated</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="filteredInventory.length === 0">
              <td colspan="7" class="text-center">No inventory items found</td>
            </tr>
            <tr v-for="item in filteredInventory" :key="item.id" :class="{'low-stock': item.quantity < item.minimumQuantity}">
              <td>{{ item.catalogItemName }}</td>
              <td>{{ item.catalogItemType }}</td>
              <td>{{ item.quantity }}</td>
              <td>{{ item.minimumQuantity }}</td>
              <td>
                <span :class="getStockStatusClass(item)">
                  {{ getStockStatus(item) }}
                </span>
              </td>
              <td>{{ formatDate(item.lastUpdated) }}</td>
              <td class="action-buttons">
                <button @click="openEditModal(item)" class="btn-icon btn-edit">
                  <i class="fas fa-edit"></i>
                </button>
                <button @click="confirmDelete(item)" class="btn-icon btn-delete">
                  <i class="fas fa-trash"></i>
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Add/Edit Modal -->
    <div class="modal" :class="{ 'show': showModal }" v-if="showModal">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ isEditing ? 'Edit Inventory Item' : 'Add Inventory Item' }}</h5>
            <button type="button" class="close" @click="closeModal">
              <span>&times;</span>
            </button>
          </div>
          <div class="modal-body">
            <form @submit.prevent="saveInventoryItem">
              <div class="form-group">
                <label>Catalog Item</label>
                <select v-model="currentItem.catalogItemId" class="form-control" :disabled="isEditing" required>
                  <option value="" disabled>Select an item</option>
                  <option v-for="item in catalogItems" :key="`${item.type}-${item.id}`" :value="item.id">
                    {{ item.name }} ({{ item.type }})
                  </option>
                </select>
              </div>
              <div class="form-group">
                <label>Quantity</label>
                <input type="number" v-model.number="currentItem.quantity" class="form-control" min="0" required>
              </div>
              <div class="form-group">
                <label>Minimum Quantity</label>
                <input type="number" v-model.number="currentItem.minimumQuantity" class="form-control" min="0" required>
              </div>
              <div class="form-group text-right">
                <button type="button" class="btn btn-secondary mr-2" @click="closeModal">Cancel</button>
                <button type="submit" class="btn btn-primary">Save</button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>

    <!-- Delete Confirmation Modal -->
    <div class="modal" :class="{ 'show': showDeleteModal }" v-if="showDeleteModal">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Confirm Delete</h5>
            <button type="button" class="close" @click="showDeleteModal = false">
              <span>&times;</span>
            </button>
          </div>
          <div class="modal-body">
            <p>Are you sure you want to delete the inventory record for <strong>{{ itemToDelete?.catalogItemName }}</strong>?</p>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" @click="showDeleteModal = false">Cancel</button>
            <button type="button" class="btn btn-danger" @click="deleteItem">Delete</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { mapGetters, mapActions } from 'vuex'

export default {
  name: 'InventoryManager',
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
      }
    }
  },
  computed: {
    ...mapGetters([
      'getAllCatalogItems',
      'getCurrentGroup',
      'getCurrentGroupInventory'
    ]),
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

.btn-icon {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 32px;
  height: 32px;
  border-radius: 4px;
  border: none;
  cursor: pointer;
  transition: background-color 0.2s;
}

.btn-edit {
  background-color: #08c;
  color: white;
}

.btn-delete {
  background-color: #dc3545;
  color: white;
}

.btn-edit:hover {
  background-color: #0077b3;
}

.btn-delete:hover {
  background-color: #c82333;
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

.ml-2 {
  margin-left: 0.5rem;
}

.mr-2 {
  margin-right: 0.5rem;
}
</style>
