<template>
  <div class="services-page">
    <div class="page-header">
      <h1 class="page-title">Services</h1>
      <button class="btn-primary" @click="showAddServiceModal = true">Add Service</button>
    </div>

    <div class="search-filter">
      <div class="search-box">
        <input 
          type="text" 
          v-model="searchQuery" 
          placeholder="Search services..." 
          class="search-input"
        />
      </div>
    </div>

    <div v-if="filteredServices.length > 0" class="table-container">
      <table class="data-table">
        <colgroup>
          <col style="width: 10%">
          <col style="width: 15%">
          <col style="width: 12%">
          <col style="width: 13%">
          <col style="width: 20%">
          <col style="width: 30%">
        </colgroup>
        <thead>
          <tr>
            <th>SKU</th>
            <th>Name</th>
            <th>Category</th>
            <th>Device</th>
            <th>Description</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="service in filteredServices" :key="service.id">
            <td>{{ service.sku }}</td>
            <td>{{ service.name }}</td>
            <td>{{ getCategoryName(service.categoryId) }}</td>
            <td>{{ getDeviceName(service.deviceId) }}</td>
            <td>{{ service.description }}</td>
            <td>
              <div class="actions-cell">
                <button class="btn-view" @click="viewServiceDetails(service)">Parts</button>
                <button class="btn-edit" @click="editService(service)">Edit</button>
                <button class="btn-delete" @click="confirmDeleteService(service)">Delete</button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <div v-else class="empty-state">
      <p v-if="searchQuery">No services found matching your search criteria.</p>
      <p v-else>No services found. Add your first service to get started.</p>
      <button class="btn-primary" @click="showAddServiceModal = true">Add Service</button>
    </div>

    <!-- Add/Edit Service Modal -->
    <div v-if="showAddServiceModal || showEditServiceModal" class="modal">
      <div class="modal-content">
        <div class="modal-header">
          <h2 class="modal-title">{{ showEditServiceModal ? 'Edit Service' : 'Add New Service' }}</h2>
          <button class="modal-close" @click="closeModal">&times;</button>
        </div>
        <div class="modal-body">
          <div class="form-group">
            <label for="serviceSku">SKU</label>
            <input 
              type="text" 
              id="serviceSku" 
              v-model="currentService.sku" 
              placeholder="Enter service SKU"
              class="form-control"
            />
          </div>
          <div class="form-group">
            <label for="serviceName">Service Name</label>
            <input 
              type="text" 
              id="serviceName" 
              v-model="currentService.name" 
              placeholder="Enter service name"
              class="form-control"
            />
          </div>
          <div class="form-group">
            <label for="serviceCategory">Category</label>
            <select 
              id="serviceCategory" 
              v-model="currentService.categoryId" 
              class="form-control"
              required
            >
              <option value="">Select a category</option>
              <option v-for="category in serviceCategories" :key="category.id" :value="category.id">
                {{ category.name }}
              </option>
            </select>
          </div>
          <div class="form-group">
            <label for="serviceDevice">Device (Optional)</label>
            <select 
              id="serviceDevice" 
              v-model="currentService.deviceId" 
              class="form-control"
            >
              <option :value="null">Not device-specific</option>
              <option v-for="device in devices" :key="device.id" :value="device.id">
                {{ device.name }}
              </option>
            </select>
          </div>
          <div class="form-group">
            <label for="serviceDescription">Description</label>
            <textarea 
              id="serviceDescription" 
              v-model="currentService.description" 
              placeholder="Enter service description"
              class="form-control"
              rows="4"
            ></textarea>
          </div>
        </div>
        <div class="modal-footer">
          <button class="btn-secondary" @click="closeModal">Cancel</button>
          <button class="btn-primary" @click="saveService">Save</button>
        </div>
      </div>
    </div>

    <!-- Service Details Modal with Parts Management -->
    <div v-if="showServiceDetailsModal" class="modal">
      <div class="modal-content modal-lg">
        <div class="modal-header">
          <h2 class="modal-title">Service Details: {{ currentService.name }}</h2>
          <button class="modal-close" @click="closeServiceDetailsModal">&times;</button>
        </div>
        <div class="modal-body">
          <div class="service-info">
            <div class="info-row">
              <div class="info-label">SKU:</div>
              <div class="info-value">{{ currentService.sku }}</div>
            </div>
            <div class="info-row">
              <div class="info-label">Category:</div>
              <div class="info-value">{{ getCategoryName(currentService.categoryId) }}</div>
            </div>
            <div class="info-row">
              <div class="info-label">Device:</div>
              <div class="info-value">{{ getDeviceName(currentService.deviceId) }}</div>
            </div>
            <div class="info-row">
              <div class="info-label">Description:</div>
              <div class="info-value">{{ currentService.description }}</div>
            </div>
          </div>

          <div class="section-divider"></div>
          
          <div class="parts-section">
            <div class="section-header">
              <h3>Required Parts</h3>
              <button class="btn-primary btn-sm" @click="showAddPartModal = true">Add Part</button>
            </div>

            <div v-if="serviceParts.length > 0" class="parts-table-container">
              <table class="parts-table">
                <thead>
                  <tr>
                    <th>SKU</th>
                    <th>Part Name</th>
                    <th>Quantity</th>
                    <th>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="part in serviceParts" :key="part.id">
                    <td>{{ part.sku }}</td>
                    <td>{{ part.name }}</td>
                    <td>
                      <div class="quantity-control">
                        <button 
                          class="quantity-btn" 
                          @click="updatePartQuantity(getServicePartId(part.id), getServicePartQuantity(part.id) - 1)"
                          :disabled="getServicePartQuantity(part.id) <= 1"
                        >-</button>
                        <span class="quantity-value">{{ getServicePartQuantity(part.id) }}</span>
                        <button 
                          class="quantity-btn" 
                          @click="updatePartQuantity(getServicePartId(part.id), getServicePartQuantity(part.id) + 1)"
                        >+</button>
                      </div>
                    </td>
                    <td>
                      <button class="btn-delete btn-sm" @click="removePart(getServicePartId(part.id))">Remove</button>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
            <div v-else class="empty-parts-message">
              <p>No parts added to this service yet.</p>
            </div>
          </div>
        </div>
        <div class="modal-footer">
          <button class="btn-secondary" @click="closeServiceDetailsModal">Close</button>
        </div>
      </div>
    </div>

    <!-- Add Part to Service Modal -->
    <div v-if="showAddPartModal" class="modal">
      <div class="modal-content">
        <div class="modal-header">
          <h2 class="modal-title">Add Part to Service</h2>
          <button class="modal-close" @click="closeAddPartModal">&times;</button>
        </div>
        <div class="modal-body">
          <div class="form-group">
            <label for="partSelect">Select Part</label>
            <select 
              id="partSelect" 
              v-model="selectedPartId" 
              class="form-control"
            >
              <option value="">Select a part</option>
              <option 
                v-for="part in compatibleParts" 
                :key="part.id" 
                :value="part.id"
                :disabled="isPartAlreadyAdded(part.id)"
              >
                {{ part.name }} {{ isPartAlreadyAdded(part.id) ? '(Already Added)' : '' }}
              </option>
            </select>
          </div>
          <div class="form-group">
            <label for="partQuantity">Quantity</label>
            <input 
              type="number" 
              id="partQuantity" 
              v-model.number="partQuantity" 
              min="1" 
              class="form-control"
            />
          </div>
        </div>
        <div class="modal-footer">
          <button class="btn-secondary" @click="closeAddPartModal">Cancel</button>
          <button 
            class="btn-primary" 
            @click="addPartToService"
            :disabled="!selectedPartId || partQuantity < 1"
          >Add Part</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { mapGetters } from 'vuex';

export default {
  name: 'ServicesCatalog',
  data() {
    return {
      searchQuery: '',
      showAddServiceModal: false,
      showEditServiceModal: false,
      showServiceDetailsModal: false,
      showAddPartModal: false,
      selectedPartId: '',
      partQuantity: 1,
      currentService: {
        id: null,
        name: '',
        description: '',
        sku: '',
        categoryId: '',
        deviceId: null
      }
    };
  },
  computed: {
    ...mapGetters({
      services: 'getServices',
      serviceCategories: 'getServiceCategories',
      devices: 'getDevices',
      parts: 'getParts',
      getServicePartsById: 'getServiceParts',
      getCompatiblePartsForService: 'getCompatibleParts'
    }),
    filteredServices() {
      if (!this.searchQuery) {
        return this.services;
      }
      
      const query = this.searchQuery.toLowerCase();
      return this.services.filter(service => 
        service.name.toLowerCase().includes(query) || 
        service.description.toLowerCase().includes(query) ||
        (service.sku && service.sku.toLowerCase().includes(query)) ||
        this.getCategoryName(service.categoryId).toLowerCase().includes(query) ||
        this.getDeviceName(service.deviceId).toLowerCase().includes(query)
      );
    },
    serviceParts() {
      if (!this.currentService.id) return [];
      return this.getServicePartsById(this.currentService.id);
    },
    compatibleParts() {
      if (!this.currentService.id) return [];
      return this.getCompatiblePartsForService(this.currentService);
    },
    servicePartRelationships() {
      return this.$store.state.serviceParts.filter(sp => sp.serviceId === this.currentService.id);
    }
  },
  methods: {
    getCategoryName(categoryId) {
      if (!categoryId) return 'None';
      const category = this.serviceCategories.find(c => c.id === categoryId);
      return category ? category.name : 'Unknown';
    },
    getDeviceName(deviceId) {
      if (!deviceId) return 'Not device-specific';
      const device = this.devices.find(d => d.id === deviceId);
      return device ? device.name : 'Unknown';
    },
    editService(service) {
      this.currentService = { ...service };
      this.showEditServiceModal = true;
    },
    confirmDeleteService(service) {
      if (confirm(`Are you sure you want to delete ${service.name}?`)) {
        this.$store.dispatch('deleteService', service.id);
      }
    },
    closeModal() {
      this.showAddServiceModal = false;
      this.showEditServiceModal = false;
      this.currentService = {
        id: null,
        name: '',
        description: '',
        sku: '',
        categoryId: '',
        deviceId: null
      };
    },
    viewServiceDetails(service) {
      this.currentService = { ...service };
      this.showServiceDetailsModal = true;
      // Fetch service parts data
      this.$store.dispatch('fetchServiceParts');
    },
    closeServiceDetailsModal() {
      this.showServiceDetailsModal = false;
      this.currentService = {
        id: null,
        name: '',
        description: '',
        sku: '',
        categoryId: '',
        deviceId: null
      };
    },
    closeAddPartModal() {
      this.showAddPartModal = false;
      this.selectedPartId = '';
      this.partQuantity = 1;
    },
    getServicePartId(partId) {
      const relationship = this.servicePartRelationships.find(sp => sp.partId === partId);
      return relationship ? relationship.id : null;
    },
    getServicePartQuantity(partId) {
      const relationship = this.servicePartRelationships.find(sp => sp.partId === partId);
      return relationship ? relationship.quantity : 0;
    },
    isPartAlreadyAdded(partId) {
      return this.servicePartRelationships.some(sp => sp.partId === partId);
    },
    addPartToService() {
      if (!this.selectedPartId || this.partQuantity < 1) return;
      
      this.$store.dispatch('addServicePart', {
        serviceId: this.currentService.id,
        partId: parseInt(this.selectedPartId),
        quantity: this.partQuantity
      });
      
      this.closeAddPartModal();
    },
    updatePartQuantity(servicePartId, newQuantity) {
      if (newQuantity < 1) return;
      
      this.$store.dispatch('updateServicePart', {
        id: servicePartId,
        quantity: newQuantity
      });
    },
    removePart(servicePartId) {
      if (confirm('Are you sure you want to remove this part from the service?')) {
        this.$store.dispatch('removeServicePart', servicePartId);
      }
    },
    saveService() {
      if (!this.currentService.name.trim()) {
        alert('Service name is required');
        return;
      }
      
      if (!this.currentService.sku.trim()) {
        alert('SKU is required');
        return;
      }
      
      if (!this.currentService.categoryId) {
        alert('Category is required');
        return;
      }
      
      if (this.showEditServiceModal) {
        // Update existing service
        this.$store.dispatch('updateService', this.currentService);
      } else {
        // Add new service
        const newService = {
          ...this.currentService,
          id: Date.now() // Temporary ID for demo
        };
        this.$store.dispatch('addService', newService);
      }
      
      this.closeModal();
    }
  },
  created() {
    // Fetch services when component is created
    this.$store.dispatch('fetchServices');
  }
};
</script>

<style scoped>
.services-page {
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
  height: 72px; /* Set a fixed height for all cells */
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

.data-table tr:last-child td {
  border-bottom: none;
}

.data-table tr:hover td {
  background-color: #f5f5f5;
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

.btn-primary:disabled {
  background-color: #ccc;
  cursor: not-allowed;
}

.btn-sm {
  padding: 0.5rem 1rem;
  font-size: 0.875rem;
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
  padding: 0.4rem 0.7rem;
  font-weight: 500;
  cursor: pointer;
  transition: background-color 0.2s ease;
  margin-right: 0.3rem;
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
  margin-left: 0.3rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
  font-size: 0.85rem;
}

.btn-delete:hover {
  background-color: #dc3545;
  color: #fff;
}

.btn-view {
  background-color: transparent;
  color: #17a2b8;
  border: 1px solid #17a2b8;
  border-radius: 4px;
  padding: 0.4rem 0.7rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
  margin-right: 0.3rem;
  font-size: 0.85rem;
}

.btn-view:hover {
  background-color: #17a2b8;
  color: #fff;
}

/* Service Details Modal Styles */
.modal-lg {
  max-width: 800px;
}

.service-info {
  margin-bottom: 1.5rem;
  padding: 1rem;
  background-color: #f8f9fa;
  border-radius: 8px;
}

.info-row {
  display: flex;
  margin-bottom: 0.75rem;
}

.info-row:last-child {
  margin-bottom: 0;
}

.info-label {
  font-weight: 600;
  width: 120px;
  color: #555;
}

.info-value {
  flex: 1;
}

.section-divider {
  height: 1px;
  background-color: #e0e0e0;
  margin: 1.5rem 0;
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.section-header h3 {
  margin: 0;
  font-size: 1.25rem;
  font-weight: 600;
  color: #333;
}

.parts-table-container {
  margin-bottom: 1.5rem;
}

.parts-table {
  width: 100%;
  border-collapse: collapse;
  border: 1px solid #e0e0e0;
  border-radius: 4px;
  overflow: hidden;
}

.parts-table th,
.parts-table td {
  padding: 0.75rem 1rem;
  text-align: left;
  border-bottom: 1px solid #e0e0e0;
}

.parts-table th {
  background-color: #f5f5f5;
  font-weight: 600;
  color: #333;
}

.parts-table tr:last-child td {
  border-bottom: none;
}

.quantity-control {
  display: flex;
  align-items: center;
}

.quantity-btn {
  width: 28px;
  height: 28px;
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: #f0f0f0;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-weight: bold;
  cursor: pointer;
  transition: background-color 0.2s ease;
}

.quantity-btn:hover {
  background-color: #e0e0e0;
}

.quantity-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.quantity-value {
  margin: 0 0.5rem;
  min-width: 24px;
  text-align: center;
}

.empty-parts-message {
  padding: 2rem;
  text-align: center;
  background-color: #f9f9f9;
  border-radius: 8px;
  color: #666;
}
</style>
