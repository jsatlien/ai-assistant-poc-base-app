<template>
  <div class="manufacturers-page">
    <h1>Manufacturers</h1>
    <div class="page-actions">
      <button class="btn-primary" @click="openManufacturerModal()">Add Manufacturer</button>
      <div class="search-container">
        <input type="text" v-model="searchQuery" placeholder="Search manufacturers..." class="search-input" />
      </div>
    </div>

    <div v-if="filteredManufacturers.length > 0" class="table-container">
      <table class="data-table">
        <colgroup>
          <col style="width: 25%">
          <col style="width: 45%">
          <col style="width: 30%">
        </colgroup>
        <thead>
          <tr>
            <th>Name</th>
            <th>Description</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="manufacturer in filteredManufacturers" :key="manufacturer.id">
            <td>{{ manufacturer.name }}</td>
            <td>{{ manufacturer.description }}</td>
            <td>
              <div class="actions-cell">
                <button class="btn-edit" @click="editManufacturer(manufacturer)">Edit</button>
                <button class="btn-delete" @click="confirmDeleteManufacturer(manufacturer)">Delete</button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <div v-else class="empty-state">
      <p>No manufacturers found. Add your first manufacturer to get started.</p>
    </div>

    <!-- Manufacturer Modal -->
    <div v-if="showManufacturerModal" class="modal">
      <div class="modal-content">
        <div class="modal-header">
          <h2>{{ isEditing ? 'Edit Manufacturer' : 'Add Manufacturer' }}</h2>
          <button class="close-btn" @click="closeManufacturerModal">&times;</button>
        </div>
        <div class="modal-body">
          <form @submit.prevent="saveManufacturer">
            <div class="form-group">
              <label for="manufacturerName">Name</label>
              <input 
                id="manufacturerName" 
                v-model="currentManufacturer.name" 
                class="form-control"
                required
              />
            </div>
            <div class="form-group">
              <label for="manufacturerDescription">Description</label>
              <textarea 
                id="manufacturerDescription" 
                v-model="currentManufacturer.description" 
                class="form-control"
                rows="3"
              ></textarea>
            </div>
            <div class="form-actions">
              <button type="button" class="btn-secondary" @click="closeManufacturerModal">Cancel</button>
              <button type="submit" class="btn-primary">Save</button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <!-- Confirmation Modal -->
    <div v-if="showConfirmationModal" class="modal">
      <div class="modal-content confirmation-modal">
        <div class="modal-header">
          <h2>Confirm Delete</h2>
          <button class="close-btn" @click="closeConfirmationModal">&times;</button>
        </div>
        <div class="modal-body">
          <p>Are you sure you want to delete the manufacturer "{{ manufacturerToDelete?.name }}"?</p>
          <div class="form-actions">
            <button class="btn-secondary" @click="closeConfirmationModal">Cancel</button>
            <button class="btn-delete" @click="deleteManufacturer">Delete</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'ManufacturersList',
  data() {
    return {
      searchQuery: '',
      showManufacturerModal: false,
      showConfirmationModal: false,
      isEditing: false,
      currentManufacturer: {
        id: null,
        name: '',
        description: ''
      },
      manufacturerToDelete: null
    }
  },
  computed: {
    manufacturers() {
      return this.$store.getters.getManufacturers
    },
    filteredManufacturers() {
      if (!this.searchQuery) return this.manufacturers
      
      const query = this.searchQuery.toLowerCase()
      return this.manufacturers.filter(manufacturer => 
        manufacturer.name.toLowerCase().includes(query) || 
        (manufacturer.description && manufacturer.description.toLowerCase().includes(query))
      )
    }
  },
  created() {
    this.$store.dispatch('fetchManufacturers')
  },
  methods: {
    openManufacturerModal(manufacturer = null) {
      this.isEditing = !!manufacturer
      this.currentManufacturer = manufacturer 
        ? { ...manufacturer } 
        : { id: null, name: '', description: '' }
      this.showManufacturerModal = true
    },
    closeManufacturerModal() {
      this.showManufacturerModal = false
      this.currentManufacturer = { id: null, name: '', description: '' }
    },
    async saveManufacturer() {
      if (!this.currentManufacturer.name.trim()) {
        alert('Manufacturer name is required')
        return
      }

      try {
        if (this.isEditing) {
          await this.$store.dispatch('updateManufacturer', this.currentManufacturer)
        } else {
          await this.$store.dispatch('addManufacturer', this.currentManufacturer)
        }
        this.closeManufacturerModal()
      } catch (error) {
        console.error('Error saving manufacturer:', error)
        alert('Failed to save manufacturer. Please try again.')
      }
    },
    editManufacturer(manufacturer) {
      this.openManufacturerModal(manufacturer)
    },
    confirmDeleteManufacturer(manufacturer) {
      this.manufacturerToDelete = manufacturer
      this.showConfirmationModal = true
    },
    closeConfirmationModal() {
      this.showConfirmationModal = false
      this.manufacturerToDelete = null
    },
    async deleteManufacturer() {
      if (!this.manufacturerToDelete) return

      try {
        await this.$store.dispatch('deleteManufacturer', this.manufacturerToDelete.id)
        this.closeConfirmationModal()
      } catch (error) {
        console.error('Error deleting manufacturer:', error)
        alert('Failed to delete manufacturer. Please try again.')
      }
    }
  }
}
</script>

<style scoped>
.manufacturers-page {
  padding: 2rem;
}

h1 {
  margin-bottom: 1.5rem;
  color: #333;
  font-weight: 600;
}

.page-actions {
  display: flex;
  justify-content: space-between;
  margin-bottom: 2rem;
}

.search-container {
  flex: 0 0 300px;
}

.search-input {
  width: 100%;
  padding: 0.75rem 1rem;
  border: 1px solid #ddd;
  border-radius: 8px;
  font-size: 1rem;
}

.table-container {
  margin-top: 1rem;
  border-radius: 8px;
  overflow: auto;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  margin-bottom: 2rem;
  background-color: #fff;
}

.data-table {
  width: 100%;
  border-collapse: collapse;
  table-layout: fixed;
}

.data-table th,
.data-table td {
  padding: 1rem 0.75rem;
  text-align: left;
  border-bottom: 1px solid #f0f0f0;
  vertical-align: middle;
  height: 72px;
}

.data-table td:first-child {
  padding-left: 1rem;
}

.data-table td:last-child {
  padding-right: 0.5rem;
}

.data-table th {
  background-color: #f9f9f9;
  font-weight: 600;
  color: #333;
}

.data-table tr:hover {
  background-color: #f9f9f9;
}

.actions-cell {
  display: flex;
  gap: 0.5rem;
  white-space: nowrap;
  height: 100%;
  align-items: center;
  padding: 0.5rem 0;
  justify-content: flex-start;
}

.empty-state {
  background-color: #f9f9f9;
  border-radius: 8px;
  padding: 3rem;
  text-align: center;
  color: #666;
}

/* Button Styles */
.btn-primary {
  background-color: #08c;
  color: #fff;
  border: none;
  border-radius: 8px;
  padding: 0.75rem 1.5rem;
  font-weight: 500;
  cursor: pointer;
  transition: background-color 0.2s ease;
}

.btn-primary:hover {
  background-color: #0077b3;
}

.btn-secondary {
  background-color: #f5f5f5;
  color: #333;
  border: 1px solid #ddd;
  border-radius: 8px;
  padding: 0.75rem 1.5rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
}

.btn-secondary:hover {
  background-color: #e5e5e5;
}

.btn-edit {
  background-color: transparent;
  color: #08c;
  border: 1px solid #08c;
  border-radius: 4px;
  padding: 0.4rem 0.7rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
  font-size: 0.85rem;
}

.btn-edit:hover {
  background-color: #08c;
  color: #fff;
}

.btn-delete {
  background-color: transparent;
  color: #dc3545;
  border: 1px solid #dc3545;
  border-radius: 4px;
  padding: 0.4rem 0.7rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
  font-size: 0.85rem;
}

.btn-delete:hover {
  background-color: #dc3545;
  color: #fff;
}

/* Modal Styles */
.modal {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-content {
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
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
  margin-top: 1.5rem;
}
</style>
