<template>
  <div class="user-roles-page">
    <h1>User Roles</h1>
    <div class="page-actions">
      <button class="btn-primary" @click="openRoleModal()">Add Role</button>
      <div class="search-container">
        <input type="text" v-model="searchQuery" placeholder="Search roles..." class="search-input" />
      </div>
    </div>

    <div v-if="filteredRoles.length > 0" class="table-container">
      <table class="data-table">
        <colgroup>
          <col style="width: 20%">
          <col style="width: 40%">
          <col style="width: 15%">
          <col style="width: 25%">
        </colgroup>
        <thead>
          <tr>
            <th>Name</th>
            <th>Description</th>
            <th>Permissions</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="role in filteredRoles" :key="role.id">
            <td>{{ role.name }}</td>
            <td>{{ role.description }}</td>
            <td>
              <div class="permissions-list">
                <span v-if="role.isAdmin" class="permission admin">Admin</span>
                <span v-if="role.isReadOnly" class="permission readonly">Read-only</span>
                <span v-if="!role.isAdmin && !role.isReadOnly" class="permission standard">Standard</span>
              </div>
            </td>
            <td>
              <div class="actions-cell">
                <button class="btn-edit" @click="editRole(role)">Edit</button>
                <button class="btn-delete" @click="confirmDeleteRole(role)">Delete</button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <div v-else class="empty-state">
      <p>No roles found. Add your first role to get started.</p>
    </div>

    <!-- Role Modal -->
    <div v-if="showRoleModal" class="modal">
      <div class="modal-content">
        <div class="modal-header">
          <h2>{{ isEditing ? 'Edit Role' : 'Add Role' }}</h2>
          <button class="close-btn" @click="closeRoleModal">&times;</button>
        </div>
        <div class="modal-body">
          <form @submit.prevent="saveRole">
            <div class="form-group">
              <label for="roleName">Role Name</label>
              <input 
                id="roleName" 
                v-model="currentRole.name" 
                class="form-control"
                required
              />
            </div>
            
            <div class="form-group">
              <label for="roleDescription">Description</label>
              <textarea 
                id="roleDescription" 
                v-model="currentRole.description" 
                class="form-control"
                rows="3"
                required
              ></textarea>
            </div>
            
            <div class="form-group permissions-group">
              <label>Permissions</label>
              <div class="checkbox-group">
                <div class="checkbox-item">
                  <input 
                    type="checkbox" 
                    id="isAdmin" 
                    v-model="currentRole.isAdmin"
                  />
                  <label for="isAdmin">Administrator (Full access to all features)</label>
                </div>
                <div class="checkbox-item">
                  <input 
                    type="checkbox" 
                    id="isReadOnly" 
                    v-model="currentRole.isReadOnly"
                    :disabled="currentRole.isAdmin"
                  />
                  <label for="isReadOnly">Read-only (View-only access)</label>
                </div>
              </div>
            </div>
            
            <div class="form-actions">
              <button type="button" class="btn-secondary" @click="closeRoleModal">Cancel</button>
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
          <p>Are you sure you want to delete the role "{{ roleToDelete?.name }}"?</p>
          <div class="form-actions">
            <button class="btn-secondary" @click="closeConfirmationModal">Cancel</button>
            <button class="btn-delete" @click="deleteRole">Delete</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'UserRolesManager',
  data() {
    return {
      searchQuery: '',
      showRoleModal: false,
      showConfirmationModal: false,
      isEditing: false,
      currentRole: {
        id: null,
        name: '',
        description: '',
        isAdmin: false,
        isReadOnly: false
      },
      roleToDelete: null
    }
  },
  computed: {
    roles() {
      return this.$store.getters.getUserRoles
    },
    filteredRoles() {
      if (!this.searchQuery) return this.roles
      
      const query = this.searchQuery.toLowerCase()
      return this.roles.filter(role => 
        role.name.toLowerCase().includes(query) || 
        role.description.toLowerCase().includes(query)
      )
    }
  },
  created() {
    this.$store.dispatch('fetchUserRoles')
  },
  methods: {
    openRoleModal(role = null) {
      this.isEditing = !!role
      this.currentRole = role 
        ? JSON.parse(JSON.stringify(role)) // Deep copy to avoid reference issues
        : {
            id: null,
            name: '',
            description: '',
            isAdmin: false,
            isReadOnly: false
          }
      this.showRoleModal = true
    },
    closeRoleModal() {
      this.showRoleModal = false
      this.currentRole = {
        id: null,
        name: '',
        description: '',
        isAdmin: false,
        isReadOnly: false
      }
    },
    async saveRole() {
      if (!this.currentRole.name.trim() || !this.currentRole.description.trim()) {
        alert('Role name and description are required')
        return
      }

      try {
        if (this.isEditing) {
          await this.$store.dispatch('updateUserRole', this.currentRole)
        } else {
          await this.$store.dispatch('addUserRole', this.currentRole)
        }
        this.closeRoleModal()
      } catch (error) {
        console.error('Error saving role:', error)
        alert('Failed to save role. Please try again.')
      }
    },
    editRole(role) {
      this.openRoleModal(role)
    },
    confirmDeleteRole(role) {
      this.roleToDelete = role
      this.showConfirmationModal = true
    },
    closeConfirmationModal() {
      this.showConfirmationModal = false
      this.roleToDelete = null
    },
    async deleteRole() {
      if (!this.roleToDelete) return

      try {
        await this.$store.dispatch('deleteUserRole', this.roleToDelete.id)
        this.closeConfirmationModal()
      } catch (error) {
        console.error('Error deleting role:', error)
        alert('Failed to delete role. Please try again.')
      }
    }
  }
}
</script>

<style scoped>
.user-roles-page {
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
  padding: 0.3rem 0;
  justify-content: flex-start;
  flex-wrap: nowrap;
}

.empty-state {
  background-color: #f9f9f9;
  border-radius: 8px;
  padding: 3rem;
  text-align: center;
  color: #666;
}

.permissions-list {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
}

.permission {
  display: inline-block;
  padding: 0.25rem 0.5rem;
  border-radius: 4px;
  font-size: 0.8rem;
  font-weight: 500;
}

.permission.admin {
  background-color: #f8d7da;
  color: #721c24;
}

.permission.readonly {
  background-color: #d1ecf1;
  color: #0c5460;
}

.permission.standard {
  background-color: #d4edda;
  color: #155724;
}

.permissions-group {
  margin-top: 1.5rem;
}

.checkbox-group {
  margin-top: 0.5rem;
}

.checkbox-item {
  display: flex;
  align-items: center;
  margin-bottom: 0.75rem;
}

.checkbox-item input[type="checkbox"] {
  margin-right: 0.5rem;
  width: auto;
}

.checkbox-item label {
  margin-bottom: 0;
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
  padding: 0.3rem 0.5rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
  margin-right: 0.2rem;
  font-size: 0.8rem;
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
  padding: 0.3rem 0.5rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
  font-size: 0.8rem;
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
  width: 600px;
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
