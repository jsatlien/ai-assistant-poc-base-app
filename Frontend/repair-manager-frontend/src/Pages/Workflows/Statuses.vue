<template>
  <div class="statuses-page">
    <h1>Status Codes</h1>
    <div class="page-actions">
      <button class="btn-primary" @click="openStatusModal()">Add Status Code</button>
      <div class="search-container">
        <input type="text" v-model="searchQuery" placeholder="Search status codes..." class="search-input" />
      </div>
    </div>

    <div v-if="filteredStatuses.length > 0" class="table-container">
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
          <tr v-for="status in filteredStatuses" :key="status.id">
            <td>{{ status.name }}</td>
            <td>{{ status.description }}</td>
            <td>
              <div class="actions-cell">
                <button class="btn-edit" @click="editStatus(status)">Edit</button>
                <button class="btn-delete" @click="confirmDeleteStatus(status)">Delete</button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <div v-else class="empty-state">
      <p>No status codes found. Add your first status code to get started.</p>
    </div>

    <!-- Status Modal -->
    <div v-if="showStatusModal" class="modal">
      <div class="modal-content">
        <div class="modal-header">
          <h2>{{ isEditing ? 'Edit Status Code' : 'Add Status Code' }}</h2>
          <button class="close-btn" @click="closeStatusModal">&times;</button>
        </div>
        <div class="modal-body">
          <form @submit.prevent="saveStatus">
            <div class="form-group">
              <label for="statusName">Name</label>
              <input 
                id="statusName" 
                v-model="currentStatus.name" 
                class="form-control"
                required
              />
            </div>
            <div class="form-group">
              <label for="statusDescription">Description</label>
              <textarea 
                id="statusDescription" 
                v-model="currentStatus.description" 
                class="form-control"
                rows="3"
              ></textarea>
            </div>
            <div class="form-actions">
              <button type="button" class="btn-secondary" @click="closeStatusModal">Cancel</button>
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
          <p>Are you sure you want to delete the status code "{{ statusToDelete?.name }}"?</p>
          <div class="form-actions">
            <button class="btn-secondary" @click="closeConfirmationModal">Cancel</button>
            <button class="btn-delete" @click="deleteStatus">Delete</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'WorkflowStatuses',
  data() {
    return {
      searchQuery: '',
      showStatusModal: false,
      showConfirmationModal: false,
      isEditing: false,
      currentStatus: {
        id: null,
        name: '',
        description: ''
      },
      statusToDelete: null
    }
  },
  computed: {
    statuses() {
      return this.$store.getters.getStatusCodes;
    },
    filteredStatuses() {
      if (!this.searchQuery) return this.statuses;
      
      const query = this.searchQuery.toLowerCase();
      return this.statuses.filter(status => {
        const name = status.name || status.Name || status.code || status.Code || '';
        const description = status.description || status.Description || '';
        return name.toLowerCase().includes(query) || 
               description.toLowerCase().includes(query);
      });
    }
  },
  created() {
    // Fetch status codes from the API
    this.$store.dispatch('fetchStatusCodes');
  },
  methods: {
    openStatusModal(status = null) {
      this.isEditing = !!status
      this.currentStatus = status 
        ? { ...status } 
        : { id: null, name: '', description: '' }
      this.showStatusModal = true
    },
    closeStatusModal() {
      this.showStatusModal = false
      this.currentStatus = { id: null, name: '', description: '' }
    },
    saveStatus() {
      if (!this.currentStatus.name.trim()) {
        alert('Status name is required')
        return
      }

      if (this.isEditing) {
        // Update existing status
        const index = this.statuses.findIndex(s => s.id === this.currentStatus.id)
        if (index !== -1) {
          this.statuses.splice(index, 1, this.currentStatus)
        }
      } else {
        // Add new status with a generated ID
        const newStatus = {
          ...this.currentStatus,
          id: Math.max(0, ...this.statuses.map(s => s.id)) + 1
        }
        this.statuses.push(newStatus)
      }
      
      this.closeStatusModal()
    },
    editStatus(status) {
      this.openStatusModal(status)
    },
    confirmDeleteStatus(status) {
      this.statusToDelete = status
      this.showConfirmationModal = true
    },
    closeConfirmationModal() {
      this.showConfirmationModal = false
      this.statusToDelete = null
    },
    deleteStatus() {
      if (!this.statusToDelete) return
      
      const index = this.statuses.findIndex(s => s.id === this.statusToDelete.id)
      if (index !== -1) {
        this.statuses.splice(index, 1)
      }
      
      this.closeConfirmationModal()
    }
  }
}
</script>

<style scoped>
.statuses-page {
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
