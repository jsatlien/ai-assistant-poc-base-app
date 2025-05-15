<template>
  <div class="flows-page">
    <h1>Workflows</h1>
    <div class="page-actions">
      <button class="btn-primary" @click="openFlowModal()">Add Workflow</button>
      <div class="search-container">
        <input type="text" v-model="searchQuery" placeholder="Search workflows..." class="search-input" />
      </div>
    </div>

    <div v-if="filteredFlows.length > 0" class="table-container">
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
          <tr v-for="flow in filteredFlows" :key="flow.id">
            <td>{{ flow.name }}</td>
            <td>{{ flow.description }}</td>
            <td>
              <div class="actions-cell">
                <button class="btn-view" @click="viewFlow(flow)">View</button>
                <button class="btn-edit" @click="editFlow(flow)">Edit</button>
                <button class="btn-delete" @click="confirmDeleteFlow(flow)">Delete</button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <div v-else class="empty-state">
      <p>No workflows found. Add your first workflow to get started.</p>
    </div>

    <!-- Flow Modal -->
    <div v-if="showFlowModal" class="modal">
      <div class="modal-content flow-modal">
        <div class="modal-header">
          <h2>{{ isEditing ? 'Edit Workflow' : 'Add Workflow' }}</h2>
          <button class="close-btn" @click="closeFlowModal">&times;</button>
        </div>
        <div class="modal-body">
          <form @submit.prevent="saveFlow">
            <div class="form-group">
              <label for="flowName">Name</label>
              <input 
                id="flowName" 
                v-model="currentFlow.name" 
                class="form-control"
                required
              />
            </div>
            <div class="form-group">
              <label for="flowDescription">Description</label>
              <textarea 
                id="flowDescription" 
                v-model="currentFlow.description" 
                class="form-control"
                rows="3"
              ></textarea>
            </div>
            <div class="form-group">
              <label>Status Code Transitions</label>
              <div class="workflow-transition-container">
                <div class="workflow-column">
                  <h3>Available Statuses</h3>
                  <div class="status-list">
                    <div 
                      v-for="status in statuses" 
                      :key="status.id" 
                      class="status-item" 
                      draggable="true"
                      @dragstart="dragStart($event, status)"
                    >
                      {{ status.name }}
                    </div>
                  </div>
                </div>
                <div class="workflow-column">
                  <h3>Next Status Flow</h3>
                  <div 
                    class="transition-flow"
                    :class="{ 'drag-active': isDragging }"
                    @dragover="handleDragOver($event)"
                    @dragleave="handleDragLeave($event)"
                    @drop="handleDrop($event)"
                  >
                    <div v-for="(fromStatus, fromId) in groupedTransitions" :key="fromId" class="status-group">
                      <div class="from-status">{{ getStatusName(fromId) }}</div>
                      <div class="to-statuses">
                        <div v-for="(toStatus, index) in fromStatus" :key="index" class="to-status-item">
                          <span>{{ getStatusName(toStatus.toStatusId) }}</span>
                          <button type="button" class="btn-icon" @click="removeSpecificTransition(toStatus)">
                            <span class="delete-icon">×</span>
                          </button>
                        </div>
                        <!-- Drop zone for existing status groups -->
                        <div 
                          v-if="isDragging" 
                          class="drop-zone"
                          @dragover.prevent
                          @drop="handleDropOnStatus($event, fromId)"
                        >
                          Drop here to add transition from {{ getStatusName(fromId) }}
                        </div>
                      </div>
                    </div>
                    
                    <!-- Main drop zone when no transitions exist -->
                    <div 
                      v-if="Object.keys(groupedTransitions).length === 0" 
                      class="empty-transitions"
                      :class="{ 'drop-highlight': isDragging }"
                    >
                      <p>{{ isDragging ? 'Drop here to create a new transition' : 'Drag statuses here to create transitions' }}</p>
                    </div>
                    
                    <!-- New status drop zone that appears only when dragging -->
                    <div 
                      v-if="isDragging && Object.keys(groupedTransitions).length > 0" 
                      class="new-status-drop-zone"
                      @dragover.prevent
                      @drop="handleDrop($event)"
                    >
                      <p>Drop here to create a new transition</p>
                    </div>
                  </div>
                </div>
              </div>
            </div>
            <div class="form-actions">
              <button type="button" class="btn-secondary" @click="closeFlowModal">Cancel</button>
              <button type="submit" class="btn-primary">Save</button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <!-- View Flow Modal -->
    <div v-if="showViewModal" class="modal">
      <div class="modal-content flow-modal">
        <div class="modal-header">
          <h2>{{ currentFlow.name }}</h2>
          <button class="close-btn" @click="closeViewModal">&times;</button>
        </div>
        <div class="modal-body">
          <div class="flow-info">
            <p><strong>Description:</strong> {{ currentFlow.description }}</p>
          </div>
          <div class="flow-diagram">
            <h3>Status Code Flow Diagram</h3>
            <div class="flow-chart">
              <div v-for="(transition, index) in currentFlow.transitions" :key="index" class="flow-step">
                <div class="status-box">{{ getStatusName(transition.fromStatusId) }}</div>
                <div class="flow-arrow">→</div>
                <div class="status-box">{{ getStatusName(transition.toStatusId) }}</div>
              </div>
            </div>
          </div>
          <div class="form-actions">
            <button class="btn-secondary" @click="closeViewModal">Close</button>
            <button class="btn-primary" @click="editCurrentFlow">Edit</button>
          </div>
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
          <p>Are you sure you want to delete the workflow "{{ flowToDelete?.name }}"?</p>
          <div class="form-actions">
            <button class="btn-secondary" @click="closeConfirmationModal">Cancel</button>
            <button class="btn-delete" @click="deleteFlow">Delete</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'WorkflowFlows',
  data() {
    return {
      searchQuery: '',
      currentFlow: null,
      showFlowModal: false,
      showViewModal: false,
      showConfirmationModal: false,
      isEditing: false,
      flowToDelete: null,
      draggedStatus: null,
      isDragging: false,
      // Sample statuses for demo
      statuses: [
        { id: 1, name: 'Received', description: 'Device has been received at the repair center' },
        { id: 2, name: 'Diagnosing', description: 'Technician is diagnosing the issue' },
        { id: 3, name: 'Awaiting Parts', description: 'Waiting for parts to arrive' },
        { id: 4, name: 'In Repair', description: 'Device is being repaired' },
        { id: 5, name: 'Quality Check', description: 'Performing quality assurance checks' },
        { id: 6, name: 'Ready for Pickup', description: 'Device is repaired and ready for pickup' },
        { id: 7, name: 'Delivered', description: 'Device has been returned to customer' },
        { id: 8, name: 'Cancelled', description: 'Repair has been cancelled' }
      ],
      // Sample flows for demo
      flows: [
        {
          id: 1,
          name: 'Standard Repair Flow',
          description: 'Standard workflow for most device repairs',
          transitions: [
            { fromStatusId: 1, toStatusId: 2 },
            { fromStatusId: 2, toStatusId: 3 },
            { fromStatusId: 2, toStatusId: 4 },
            { fromStatusId: 3, toStatusId: 4 },
            { fromStatusId: 4, toStatusId: 5 },
            { fromStatusId: 5, toStatusId: 6 },
            { fromStatusId: 6, toStatusId: 7 },
            { fromStatusId: 1, toStatusId: 8 },
            { fromStatusId: 2, toStatusId: 8 },
            { fromStatusId: 3, toStatusId: 8 }
          ]
        },
        {
          id: 2,
          name: 'Express Repair Flow',
          description: 'Expedited workflow for urgent repairs',
          transitions: [
            { fromStatusId: 1, toStatusId: 2 },
            { fromStatusId: 2, toStatusId: 4 },
            { fromStatusId: 4, toStatusId: 5 },
            { fromStatusId: 5, toStatusId: 6 },
            { fromStatusId: 6, toStatusId: 7 },
            { fromStatusId: 1, toStatusId: 8 },
            { fromStatusId: 2, toStatusId: 8 }
          ]
        },
        {
          id: 3,
          name: 'Advanced Repair Flow',
          description: 'Workflow for complex device repairs',
          transitions: [
            { fromStatusId: 1, toStatusId: 2 },
            { fromStatusId: 2, toStatusId: 3 },
            { fromStatusId: 3, toStatusId: 4 },
            { fromStatusId: 4, toStatusId: 5 },
            { fromStatusId: 5, toStatusId: 4 },
            { fromStatusId: 5, toStatusId: 6 },
            { fromStatusId: 6, toStatusId: 7 },
            { fromStatusId: 1, toStatusId: 8 },
            { fromStatusId: 2, toStatusId: 8 },
            { fromStatusId: 3, toStatusId: 8 },
            { fromStatusId: 4, toStatusId: 8 }
          ]
        }
      ]
    }
  },
  computed: {
    workflows() {
      return this.$store.getters.getWorkflows;
    },
    statusCodes() {
      return this.$store.getters.getStatusCodes;
    },
    filteredFlows() {
      if (!this.searchQuery) return this.workflows;
      const query = this.searchQuery.toLowerCase();
      return this.workflows.filter(flow => {
        const name = flow.name || flow.Name || '';
        const description = flow.description || flow.Description || '';
        return name.toLowerCase().includes(query) ||
               description.toLowerCase().includes(query);
      });
    },
    groupedTransitions() {
      const grouped = {};
      if (this.currentFlow && this.currentFlow.transitions) {
        this.currentFlow.transitions.forEach(transition => {
          if (transition.fromStatusId && transition.toStatusId) {
            if (!grouped[transition.fromStatusId]) {
              grouped[transition.fromStatusId] = [];
            }
            grouped[transition.fromStatusId].push(transition);
          }
        });
      }
      return grouped;
    }
  },
  created() {
    // Fetch workflows and status codes from the API
    this.$store.dispatch('fetchWorkflows');
    this.$store.dispatch('fetchStatusCodes');
  },
  methods: {
    openFlowModal(flow = null) {
      this.currentFlow = flow 
        ? JSON.parse(JSON.stringify(flow)) // Deep copy to avoid reference issues
        : { id: null, name: '', description: '', transitions: [] }
      this.isEditing = !!flow
      this.showFlowModal = true
      this.draggedStatus = null
    },
    
    // Drag and drop methods
    dragStart(event, status) {
      this.draggedStatus = status
      this.isDragging = true
      event.dataTransfer.effectAllowed = 'copy'
    },
    
    handleDragOver(event) {
      event.preventDefault()
      // Add visual feedback
      if (event.currentTarget.classList.contains('transition-flow')) {
        event.currentTarget.classList.add('drag-active')
      }
    },
    
    handleDragLeave(event) {
      // Only remove the class if we're leaving the main container
      if (event.currentTarget.classList.contains('transition-flow')) {
        event.currentTarget.classList.remove('drag-active')
      }
    },
    
    handleDrop(event) {
      event.preventDefault()
      if (!this.draggedStatus) return
      
      // Show a dropdown to select the 'from' status
      this.$nextTick(() => {
        const fromStatus = prompt('Select a status that can transition to ' + this.draggedStatus.name + ':\n\n' + 
          this.statuses.map(s => s.id + ': ' + s.name).join('\n'))
        
        if (fromStatus) {
          const fromStatusId = parseInt(fromStatus, 10)
          if (!isNaN(fromStatusId)) {
            this.addSpecificTransition(fromStatusId, this.draggedStatus.id)
          }
        }
      })
      
      this.isDragging = false
      this.draggedStatus = null
    },
    
    handleDropOnStatus(event, fromStatusId) {
      event.preventDefault()
      event.stopPropagation()
      
      if (!this.draggedStatus) return
      
      // Add transition from the specified status to the dragged status
      this.addSpecificTransition(fromStatusId, this.draggedStatus.id)
      
      this.isDragging = false
      this.draggedStatus = null
    },
    
    addSpecificTransition(fromStatusId, toStatusId) {
      // Check if this transition already exists
      const exists = this.currentFlow.transitions.some(t => 
        t.fromStatusId === fromStatusId && t.toStatusId === toStatusId
      )
      
      if (!exists) {
        this.currentFlow.transitions.push({
          fromStatusId,
          toStatusId
        })
      }
    },
    
    getStatusName(statusId) {
      const status = this.statuses.find(s => s.id === parseInt(statusId, 10))
      return status ? status.name : 'Unknown Status'
    },
    
    removeSpecificTransition(transition) {
      const index = this.currentFlow.transitions.findIndex(t => 
        t.fromStatusId === transition.fromStatusId && t.toStatusId === transition.toStatusId
      )
      
      if (index !== -1) {
        this.currentFlow.transitions.splice(index, 1)
      }
    },
    closeFlowModal() {
      this.showFlowModal = false
      this.currentFlow = { id: null, name: '', description: '', transitions: [] }
    },
    addTransition() {
      this.currentFlow.transitions.push({ fromStatusId: '', toStatusId: '' })
    },
    removeTransition(index) {
      this.currentFlow.transitions.splice(index, 1)
    },
    saveFlow() {
      if (!this.currentFlow.name.trim()) {
        alert('Workflow name is required')
        return
      }

      // Validate transitions
      const invalidTransitions = this.currentFlow.transitions.filter(
        t => !t.fromStatusId || !t.toStatusId
      )
      
      if (invalidTransitions.length > 0) {
        alert('All transitions must have both a from and to status')
        return
      }

      if (this.isEditing) {
        // Update existing flow
        const index = this.flows.findIndex(f => f.id === this.currentFlow.id)
        if (index !== -1) {
          this.flows.splice(index, 1, this.currentFlow)
        }
      } else {
        // Add new flow with a generated ID
        const newFlow = {
          ...this.currentFlow,
          id: Math.max(0, ...this.flows.map(f => f.id)) + 1
        }
        this.flows.push(newFlow)
      }
      
      this.closeFlowModal()
    },
    viewFlow(flow) {
      this.currentFlow = JSON.parse(JSON.stringify(flow)) // Deep copy
      this.showViewModal = true
    },
    closeViewModal() {
      this.showViewModal = false
      this.currentFlow = { id: null, name: '', description: '', transitions: [] }
    },
    editCurrentFlow() {
      this.closeViewModal()
      this.openFlowModal(this.currentFlow)
    },
    editFlow(flow) {
      this.openFlowModal(flow)
    },
    confirmDeleteFlow(flow) {
      this.flowToDelete = flow
      this.showConfirmationModal = true
    },
    closeConfirmationModal() {
      this.showConfirmationModal = false
      this.flowToDelete = null
    },
    deleteFlow() {
      if (!this.flowToDelete) return
      
      const index = this.flows.findIndex(f => f.id === this.flowToDelete.id)
      if (index !== -1) {
        this.flows.splice(index, 1)
      }
      
      this.closeConfirmationModal()
    }
  }
}
</script>

<style scoped>
.flows-page {
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

/* Flow Specific Styles */
.flow-modal {
  width: 1200px;
  max-width: 90%;
}

.workflow-transition-container {
  display: flex;
  gap: 2rem;
  margin-top: 1rem;
}

.workflow-column {
  flex: 1;
  border: 1px solid #ddd;
  border-radius: 8px;
  overflow: hidden;
  display: flex;
  flex-direction: column;
  min-height: 400px;
}

.workflow-column h3 {
  margin: 0;
  padding: 1rem;
  background-color: #f5f5f5;
  border-bottom: 1px solid #ddd;
  font-size: 1.1rem;
  color: #333;
}

.status-list {
  padding: 1rem;
  overflow-y: auto;
  flex-grow: 1;
  background-color: #fff;
}

.status-item {
  padding: 0.75rem 1rem;
  margin-bottom: 0.5rem;
  background-color: #f0f7ff;
  border: 1px solid #cce5ff;
  border-radius: 4px;
  color: #0066cc;
  font-weight: 500;
  cursor: grab;
  transition: all 0.2s ease;
}

.status-item:hover {
  background-color: #e0f0ff;
  transform: translateY(-2px);
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
}

.transition-flow {
  padding: 1rem;
  overflow-y: auto;
  flex-grow: 1;
  background-color: #fff;
  min-height: 300px;
}

.status-group {
  margin-bottom: 1.5rem;
}

.from-status {
  font-weight: 600;
  margin-bottom: 0.5rem;
  padding: 0.5rem;
  background-color: #f0f7ff;
  border-left: 4px solid #0066cc;
  border-radius: 4px;
}

.to-statuses {
  padding-left: 1.5rem;
}

.to-status-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0.5rem 0.75rem;
  margin-bottom: 0.5rem;
  background-color: #f9f9f9;
  border: 1px solid #eee;
  border-radius: 4px;
}

.empty-transitions {
  display: flex;
  align-items: center;
  justify-content: center;
  height: 100%;
  color: #999;
  font-style: italic;
  text-align: center;
  padding: 2rem;
  transition: all 0.3s ease;
}

.drop-highlight {
  background-color: #e0f0ff;
  border: 2px dashed #0066cc;
  color: #0066cc;
  font-weight: 500;
}

.drag-active {
  background-color: #f9f9f9;
}

.drop-zone {
  margin-top: 0.5rem;
  padding: 0.75rem;
  border: 2px dashed #0066cc;
  border-radius: 4px;
  background-color: #e0f0ff;
  color: #0066cc;
  text-align: center;
  font-size: 0.9rem;
  transition: all 0.2s ease;
}

.drop-zone:hover {
  background-color: #d0e8ff;
}

.new-status-drop-zone {
  margin-top: 1.5rem;
  padding: 1rem;
  border: 2px dashed #0066cc;
  border-radius: 4px;
  background-color: #e0f0ff;
  color: #0066cc;
  text-align: center;
  font-weight: 500;
  transition: all 0.2s ease;
}

.new-status-drop-zone:hover {
  background-color: #d0e8ff;
}

.status-transitions {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.transition-item {
  background-color: #f9f9f9;
  border-radius: 8px;
  padding: 1rem;
}

.transition-controls {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.transition-arrow {
  font-size: 1.5rem;
  color: #666;
  margin: 0 0.5rem;
}

.btn-icon {
  background: none;
  border: none;
  color: #dc3545;
  font-size: 1.5rem;
  cursor: pointer;
  padding: 0;
  display: flex;
  align-items: center;
  justify-content: center;
}

.delete-icon {
  font-weight: bold;
}

.btn-sm {
  padding: 0.5rem 1rem;
  font-size: 0.875rem;
}

.flow-info {
  margin-bottom: 1.5rem;
}

.flow-diagram {
  margin-bottom: 1.5rem;
}

.flow-diagram h3 {
  margin-bottom: 1rem;
  font-size: 1.25rem;
  color: #333;
}

.flow-chart {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.flow-step {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.status-box {
  background-color: #f0f7ff;
  border: 1px solid #cce5ff;
  border-radius: 4px;
  padding: 0.75rem 1rem;
  color: #0066cc;
  font-weight: 500;
}

.flow-arrow {
  color: #666;
  font-size: 1.25rem;
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
  width: 1200px;
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
