<template>
  <div class="devices-page">
    <div class="page-header">
      <h1 class="page-title">Devices</h1>
      <button class="btn-primary" @click="showAddDeviceModal = true">Add Device</button>
    </div>

    <div class="search-filter">
      <div class="search-box">
        <input 
          type="text" 
          v-model="searchQuery" 
          placeholder="Search devices..." 
          class="search-input"
        />
      </div>
    </div>

    <div v-if="filteredDevices.length > 0" class="table-container">
      <table class="data-table">
        <thead>
          <tr>
            <th>SKU</th>
            <th>Name</th>
            <th>Description</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="device in filteredDevices" :key="device.id">
            <td>{{ device.sku }}</td>
            <td>{{ device.name }}</td>
            <td>{{ device.description }}</td>
            <td class="actions-cell">
              <button class="btn-edit" @click="editDevice(device)">Edit</button>
              <button class="btn-delete" @click="confirmDeleteDevice(device)">Delete</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <div v-else class="empty-state">
      <p v-if="searchQuery">No devices found matching your search criteria.</p>
      <p v-else>No devices found. Add your first device to get started.</p>
      <button class="btn-primary" @click="showAddDeviceModal = true">Add Device</button>
    </div>

    <!-- Add/Edit Device Modal -->
    <div v-if="showAddDeviceModal || showEditDeviceModal" class="modal">
      <div class="modal-content">
        <div class="modal-header">
          <h2 class="modal-title">{{ showEditDeviceModal ? 'Edit Device' : 'Add New Device' }}</h2>
          <button class="modal-close" @click="closeModal">&times;</button>
        </div>
        <div class="modal-body">
          <div class="form-group">
            <label for="deviceSku">SKU</label>
            <input 
              type="text" 
              id="deviceSku" 
              v-model="currentDevice.sku" 
              placeholder="Enter device SKU"
              class="form-control"
            />
          </div>
          <div class="form-group">
            <label for="deviceName">Device Name</label>
            <input 
              type="text" 
              id="deviceName" 
              v-model="currentDevice.name" 
              placeholder="Enter device name"
              class="form-control"
            />
          </div>
          <div class="form-group">
            <label for="deviceDescription">Description</label>
            <textarea 
              id="deviceDescription" 
              v-model="currentDevice.description" 
              placeholder="Enter device description"
              class="form-control"
              rows="4"
            ></textarea>
          </div>
        </div>
        <div class="modal-footer">
          <button class="btn-secondary" @click="closeModal">Cancel</button>
          <button class="btn-primary" @click="saveDevice">Save</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { mapGetters } from 'vuex';

export default {
  name: 'DevicesCatalog',
  data() {
    return {
      searchQuery: '',
      showAddDeviceModal: false,
      showEditDeviceModal: false,
      currentDevice: {
        id: null,
        name: '',
        description: '',
        sku: ''
      }
    };
  },
  computed: {
    ...mapGetters({
      devices: 'getDevices'
    }),
    filteredDevices() {
      if (!this.searchQuery) {
        return this.devices;
      }
      
      const query = this.searchQuery.toLowerCase();
      return this.devices.filter(device => 
        device.name.toLowerCase().includes(query) || 
        device.description.toLowerCase().includes(query) ||
        (device.sku && device.sku.toLowerCase().includes(query))
      );
    }
  },
  methods: {
    editDevice(device) {
      this.currentDevice = { ...device };
      this.showEditDeviceModal = true;
    },
    confirmDeleteDevice(device) {
      if (confirm(`Are you sure you want to delete ${device.name}?`)) {
        this.$store.dispatch('deleteDevice', device.id);
      }
    },
    closeModal() {
      this.showAddDeviceModal = false;
      this.showEditDeviceModal = false;
      this.currentDevice = {
        id: null,
        name: '',
        description: '',
        sku: ''
      };
    },
    saveDevice() {
      if (!this.currentDevice.name.trim()) {
        alert('Device name is required');
        return;
      }
      
      if (!this.currentDevice.sku.trim()) {
        alert('SKU is required');
        return;
      }
      
      if (this.showEditDeviceModal) {
        // Update existing device
        this.$store.dispatch('updateDevice', this.currentDevice);
      } else {
        // Add new device
        const newDevice = {
          ...this.currentDevice,
          id: Date.now() // Temporary ID for demo
        };
        this.$store.dispatch('addDevice', newDevice);
      }
      
      this.closeModal();
    }
  },
  created() {
    // Fetch devices when component is created
    this.$store.dispatch('fetchDevices');
  }
};
</script>

<style scoped>
.devices-page {
  padding: 2rem;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
}

.page-title {
  font-size: 2rem;
  font-weight: 700;
  color: #333;
  margin: 0;
}

.search-filter {
  margin-bottom: 2rem;
}

.search-input {
  width: 100%;
  padding: 0.75rem 1rem;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  font-size: 1rem;
}

.table-container {
  background-color: #fff;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  overflow: hidden;
  margin-bottom: 2rem;
}

.data-table {
  width: 100%;
  border-collapse: collapse;
}

.data-table th,
.data-table td {
  padding: 1rem;
  text-align: left;
  border-bottom: 1px solid #f0f0f0;
}

.data-table th {
  background-color: #f9f9f9;
  font-weight: 600;
  color: #333;
}

.data-table tr:last-child td {
  border-bottom: none;
}

.data-table tr:hover td {
  background-color: #f5f5f5;
}

.actions-cell {
  display: flex;
  gap: 0.5rem;
}

.empty-state {
  text-align: center;
  padding: 4rem 0;
  background-color: #fff;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.empty-state p {
  margin-bottom: 1.5rem;
  color: #666;
}

/* Modal Styles */
.modal {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-content {
  background-color: #fff;
  border-radius: 12px;
  width: 100%;
  max-width: 500px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.15);
}

.modal-header {
  padding: 1.5rem;
  border-bottom: 1px solid #f0f0f0;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.modal-title {
  font-size: 1.5rem;
  font-weight: 600;
  margin: 0;
  color: #333;
}

.modal-close {
  background: none;
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
  color: #666;
}

.modal-body {
  padding: 1.5rem;
}

.modal-footer {
  padding: 1.5rem;
  border-top: 1px solid #f0f0f0;
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
}

.form-group {
  margin-bottom: 1.5rem;
}

.form-group label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 500;
  color: #333;
}

.form-control {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  font-size: 1rem;
}

.form-control:focus {
  outline: none;
  border-color: #08c;
  box-shadow: 0 0 0 2px rgba(0, 136, 204, 0.2);
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
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  padding: 0.75rem 1.5rem;
  font-weight: 500;
  cursor: pointer;
  transition: background-color 0.2s ease;
}

.btn-secondary:hover {
  background-color: #e0e0e0;
}

.btn-edit {
  background-color: transparent;
  color: #08c;
  border: 1px solid #08c;
  border-radius: 4px;
  padding: 0.5rem 1rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
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
  padding: 0.5rem 1rem;
  margin-left: 0.5rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
}

.btn-delete:hover {
  background-color: #dc3545;
  color: #fff;
}
</style>
