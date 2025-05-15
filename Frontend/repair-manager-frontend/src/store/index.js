import Vue from 'vue'
import Vuex from 'vuex'
import createPersistedState from 'vuex-persistedstate'

import api from '../services/api'
import { 
  authService, 
  catalogService, 
  workflowService,
  groupService,
  workOrderService,
  inventoryService,
  // Commented out unused services until we need them
  // userService 
} from '../services'
import partService from '../services/partService'

Vue.use(Vuex)

export default new Vuex.Store({
  plugins: [
    createPersistedState({
      // Only persist authentication-related state
      paths: ['user', 'isAuthenticated', 'currentGroup'],
      // Use localStorage by default
      storage: window.localStorage
    })
  ],
  state: {
    // Authentication state tracking
    initialAuthCheckDone: false,
    // Catalog items
    devices: [],
    serviceCategories: [],
    productCategories: [],
    manufacturers: [],
    services: [],
    parts: [],
    serviceParts: [], // Mapping between services and parts
    // Work orders
    workOrders: [],
    workOrderCounter: 0, // Counter for generating work order codes
    // Workflows
    workflows: [],
    // Programs
    programs: [],
    // Status Codes
    statusCodes: [],
    // Groups
    groups: [],
    currentGroup: null,
    // Authentication
    user: null,
    isAuthenticated: false,
    userRoles: [],
    catalogPricing: [],
    // Inventory
    inventory: []
  },
  getters: {
    // Catalog getters
    getDevices: state => state.devices,
    getServiceCategories: state => state.serviceCategories,
    getServices: state => state.services,
    getParts: state => state.parts,
    getProductCategories: state => state.productCategories,
    getManufacturers: state => state.manufacturers,
    getAllCatalogItems: state => {
      const devices = state.devices.map(device => ({ ...device, type: 'Device' }));
      const services = state.services.map(service => ({ ...service, type: 'Service' }));
      const parts = state.parts.map(part => ({ ...part, type: 'Part' }));
      return [...devices, ...services, ...parts];
    },
    getServiceParts: state => serviceId => {
      return state.serviceParts
        .filter(sp => sp.serviceId === serviceId)
        .map(sp => state.parts.find(part => part.id === sp.partId));
    },
    getCompatibleParts: state => service => {
      // Get parts that are either not linked to any device or linked to the same device as the service
      return state.parts.filter(part => 
        part.deviceId === null || part.deviceId === service.deviceId
      );
    },
    // Work order getters
    getWorkOrders: state => state.workOrders,
    getWorkOrderById: state => id => state.workOrders.find(wo => wo.id === id),
    
    // Workflow getters
    getWorkflows: state => state.workflows,
    getWorkflowById: state => id => state.workflows.find(w => w.id === id),
    
    // Program getters
    getPrograms: state => state.programs,
    getProgramById: state => id => state.programs.find(p => p.id === id),
    
    // Status code getters
    getStatusCodes: state => state.statusCodes,
    getStatusCodeById: state => id => state.statusCodes.find(s => s.id === id),
    getWorkOrdersByGroup: state => groupId => {
      if (groupId === 'all') return state.workOrders;
      return state.workOrders.filter(wo => wo.groupId === groupId);
    },
    // Group getters
    getGroups: state => state.groups,
    getGroupById: state => id => state.groups.find(g => g.id === id),
    getCurrentGroup: state => state.currentGroup,
    // Auth getters
    isAuthenticated: state => state.isAuthenticated,
    currentUser: state => state.user,
    isAdmin: state => {
      // Add logging to debug admin status
      console.log('Checking isAdmin, user object:', state.user);
      
      // If no user, not admin
      if (!state.user) {
        console.log('No user object, not admin');
        return false;
      }
      
      // Check all possible admin flags
      const isAdminFlag = state.user.isAdmin || state.user.IsAdmin;
      const isAdminRole = state.user.role === 'Administrator' || state.user.Role === 'Administrator';
      const isAdminUsername = state.user.username === 'admin' || state.user.Username === 'admin';
      
      console.log('Admin checks:', { isAdminFlag, isAdminRole, isAdminUsername });
      
      // Return true if any admin check passes
      return isAdminFlag || isAdminRole || isAdminUsername;
    },
    getUserRoles: state => state.userRoles,
    getCatalogPricing: state => state.catalogPricing,
    // Inventory getters
    getInventory: state => state.inventory,
    getInventoryByGroup: state => groupId => {
      return state.inventory.filter(item => item.groupId === groupId);
    },
    getCurrentGroupInventory: state => {
      if (!state.currentGroup) return [];
      return state.inventory.filter(item => item.groupId === state.currentGroup.id);
    }
  },
  mutations: {
    // Authentication tracking mutations
    SET_INITIAL_AUTH_CHECK_DONE(state, value) {
      state.initialAuthCheckDone = value;
    },
    // Catalog mutations
    SET_DEVICES(state, devices) {
      state.devices = devices
    },
    ADD_DEVICE(state, device) {
      state.devices.push(device)
    },
    UPDATE_DEVICE(state, updatedDevice) {
      const index = state.devices.findIndex(d => d.id === updatedDevice.id)
      if (index !== -1) {
        state.devices.splice(index, 1, updatedDevice)
      }
    },
    DELETE_DEVICE(state, deviceId) {
      state.devices = state.devices.filter(d => d.id !== deviceId)
    },
    SET_SERVICE_CATEGORIES(state, categories) {
      state.serviceCategories = categories
    },
    ADD_SERVICE_CATEGORY(state, category) {
      state.serviceCategories.push(category)
    },
    UPDATE_SERVICE_CATEGORY(state, updatedCategory) {
      const index = state.serviceCategories.findIndex(c => c.id === updatedCategory.id)
      if (index !== -1) {
        state.serviceCategories.splice(index, 1, updatedCategory)
      }
    },
    DELETE_SERVICE_CATEGORY(state, categoryId) {
      state.serviceCategories = state.serviceCategories.filter(c => c.id !== categoryId)
    },
    SET_PRODUCT_CATEGORIES(state, categories) {
      state.productCategories = categories
    },
    ADD_PRODUCT_CATEGORY(state, category) {
      state.productCategories.push(category)
    },
    UPDATE_PRODUCT_CATEGORY(state, updatedCategory) {
      const index = state.productCategories.findIndex(c => c.id === updatedCategory.id)
      if (index !== -1) {
        state.productCategories.splice(index, 1, updatedCategory)
      }
    },
    DELETE_PRODUCT_CATEGORY(state, categoryId) {
      state.productCategories = state.productCategories.filter(c => c.id !== categoryId)
    },
    SET_MANUFACTURERS(state, manufacturers) {
      state.manufacturers = manufacturers
    },
    ADD_MANUFACTURER(state, manufacturer) {
      state.manufacturers.push(manufacturer)
    },
    UPDATE_MANUFACTURER(state, updatedManufacturer) {
      const index = state.manufacturers.findIndex(m => m.id === updatedManufacturer.id)
      if (index !== -1) {
        state.manufacturers.splice(index, 1, updatedManufacturer)
      }
    },
    DELETE_MANUFACTURER(state, manufacturerId) {
      state.manufacturers = state.manufacturers.filter(m => m.id !== manufacturerId)
    },
    SET_SERVICES(state, services) {
      state.services = services
    },
    ADD_SERVICE(state, service) {
      state.services.push(service)
    },
    UPDATE_SERVICE(state, updatedService) {
      const index = state.services.findIndex(s => s.id === updatedService.id)
      if (index !== -1) {
        state.services.splice(index, 1, updatedService)
      }
    },
    DELETE_SERVICE(state, serviceId) {
      state.services = state.services.filter(s => s.id !== serviceId)
    },
    SET_PARTS(state, parts) {
      state.parts = parts
    },
    ADD_PART(state, part) {
      state.parts.push(part)
    },
    UPDATE_PART(state, updatedPart) {
      const index = state.parts.findIndex(p => p.id === updatedPart.id)
      if (index !== -1) {
        state.parts.splice(index, 1, updatedPart)
      }
    },
    DELETE_PART(state, partId) {
      state.parts = state.parts.filter(p => p.id !== partId)
      // Also remove any service-part relationships for this part
      state.serviceParts = state.serviceParts.filter(sp => sp.partId !== partId)
    },
    SET_SERVICE_PARTS(state, serviceParts) {
      state.serviceParts = serviceParts
    },
    ADD_SERVICE_PART(state, { serviceId, partId, quantity = 1 }) {
      // Check if this relationship already exists
      const existing = state.serviceParts.find(
        sp => sp.serviceId === serviceId && sp.partId === partId
      )
      
      if (existing) {
        // Update quantity if it already exists
        existing.quantity += quantity
      } else {
        // Add new relationship
        state.serviceParts.push({
          id: Date.now(),
          serviceId,
          partId,
          quantity
        })
      }
    },
    UPDATE_SERVICE_PART(state, { id, quantity }) {
      const servicePart = state.serviceParts.find(sp => sp.id === id)
      if (servicePart) {
        servicePart.quantity = quantity
      }
    },
    REMOVE_SERVICE_PART(state, id) {
      state.serviceParts = state.serviceParts.filter(sp => sp.id !== id)
    },
    // Work order mutations
    SET_WORK_ORDERS(state, workOrders) {
      state.workOrders = workOrders
    },
    ADD_WORK_ORDER(state, workOrder) {
      state.workOrders.push(workOrder)
    },
    UPDATE_WORK_ORDER(state, updatedWorkOrder) {
      const index = state.workOrders.findIndex(w => w.id === updatedWorkOrder.id)
      if (index !== -1) {
        state.workOrders.splice(index, 1, updatedWorkOrder)
      }
    },
    DELETE_WORK_ORDER(state, workOrderId) {
      state.workOrders = state.workOrders.filter(w => w.id !== workOrderId)
    },
    SET_WORK_ORDER_COUNTER(state, count) {
      state.workOrderCounter = count
    },
    // Workflow mutations
    SET_WORKFLOWS(state, workflows) {
      state.workflows = workflows
    },
    ADD_WORKFLOW(state, workflow) {
      state.workflows.push(workflow)
    },
    UPDATE_WORKFLOW(state, updatedWorkflow) {
      const index = state.workflows.findIndex(w => w.id === updatedWorkflow.id)
      if (index !== -1) {
        state.workflows.splice(index, 1, updatedWorkflow)
      }
    },
    // Program mutations
    SET_PROGRAMS(state, programs) {
      state.programs = programs
    },
    ADD_PROGRAM(state, program) {
      state.programs.push(program)
    },
    UPDATE_PROGRAM(state, updatedProgram) {
      const index = state.programs.findIndex(p => p.id === updatedProgram.id)
      if (index !== -1) {
        state.programs.splice(index, 1, updatedProgram)
      }
    },
    // Group mutations
    SET_GROUPS(state, groups) {
      state.groups = groups
    },
    ADD_GROUP(state, group) {
      state.groups.push(group)
    },
    UPDATE_GROUP(state, updatedGroup) {
      const index = state.groups.findIndex(g => g.id === updatedGroup.id)
      if (index !== -1) {
        state.groups.splice(index, 1, updatedGroup)
      }
      // If this is the current group, update it as well
      if (state.currentGroup && state.currentGroup.id === updatedGroup.id) {
        state.currentGroup = updatedGroup
        localStorage.setItem('currentGroup', JSON.stringify(updatedGroup))
      }
    },
    DELETE_GROUP(state, groupId) {
      state.groups = state.groups.filter(g => g.id !== groupId)
      // If this is the current group, clear it
      if (state.currentGroup && state.currentGroup.id === groupId) {
        state.currentGroup = null
        localStorage.removeItem('currentGroup')
      }
    },
    SET_CURRENT_GROUP(state, group) {
      state.currentGroup = group
      if (group) {
        localStorage.setItem('currentGroup', JSON.stringify(group))
      } else {
        localStorage.removeItem('currentGroup')
      }
    },
    // User Roles mutations
    SET_USER_ROLES(state, roles) {
      state.userRoles = roles
    },
    ADD_USER_ROLE(state, role) {
      state.userRoles.push(role)
    },
    UPDATE_USER_ROLE(state, updatedRole) {
      const index = state.userRoles.findIndex(r => r.id === updatedRole.id)
      if (index !== -1) {
        state.userRoles.splice(index, 1, updatedRole)
      }
    },
    DELETE_USER_ROLE(state, roleId) {
      state.userRoles = state.userRoles.filter(r => r.id !== roleId)
    },
    // Catalog Pricing mutations
    SET_CATALOG_PRICING(state, pricing) {
      state.catalogPricing = pricing
    },
    // Status Code mutations
    SET_STATUS_CODES(state, statusCodes) {
      state.statusCodes = statusCodes
    },
    ADD_CATALOG_PRICE(state, price) {
      state.catalogPricing.push(price)
    },
    UPDATE_CATALOG_PRICE(state, updatedPrice) {
      const index = state.catalogPricing.findIndex(p => p.id === updatedPrice.id)
      if (index !== -1) {
        state.catalogPricing.splice(index, 1, updatedPrice)
      }
    },
    DELETE_CATALOG_PRICE(state, priceId) {
      state.catalogPricing = state.catalogPricing.filter(p => p.id !== priceId)
    },
    // Inventory mutations
    SET_INVENTORY(state, inventory) {
      state.inventory = inventory
    },
    ADD_INVENTORY_ITEM(state, item) {
      state.inventory.push(item)
    },
    UPDATE_INVENTORY_ITEM(state, updatedItem) {
      const index = state.inventory.findIndex(i => 
        i.groupId === updatedItem.groupId && i.catalogItemId === updatedItem.catalogItemId && i.catalogItemType === updatedItem.catalogItemType
      )
      if (index !== -1) {
        state.inventory.splice(index, 1, updatedItem)
      }
    },
    DELETE_INVENTORY_ITEM(state, { groupId, catalogItemId, catalogItemType }) {
      state.inventory = state.inventory.filter(i => 
        !(i.groupId === groupId && i.catalogItemId === catalogItemId && i.catalogItemType === catalogItemType)
      )
    },
    // Auth mutations
    SET_USER(state, user) {
      state.user = user
    },
    SET_AUTH(state, { user, isAuthenticated }) {
      state.user = user
      state.isAuthenticated = isAuthenticated
    },
    LOGOUT(state) {
      state.user = null
      state.isAuthenticated = false
    },
  },
  actions: {
    // Catalog actions
    async fetchDevices({ commit }) {
      try {
        const response = await catalogService.getDevices()
        commit('SET_DEVICES', response.data)
      } catch (error) {
        console.error('Error fetching devices:', error)
        // No fallback to dummy data - we want to rely on the API
        commit('SET_DEVICES', [])
      }
    },
    async addDevice({ commit }, device) {
      try {
        const response = await catalogService.createDevice(device)
        commit('ADD_DEVICE', response.data)
      } catch (error) {
        console.error('Error adding device:', error)
        // Fallback for demo purposes if API fails
        commit('ADD_DEVICE', { ...device, id: Date.now() })
      }
    },
    async updateDevice({ commit }, device) {
      try {
        const response = await catalogService.updateDevice(device.id, device)
        commit('UPDATE_DEVICE', response.data)
      } catch (error) {
        console.error('Error updating device:', error)
        // Fallback for demo purposes if API fails
        commit('UPDATE_DEVICE', device)
      }
    },
    async deleteDevice({ commit }, deviceId) {
      try {
        await catalogService.deleteDevice(deviceId)
        commit('DELETE_DEVICE', deviceId)
      } catch (error) {
        console.error('Error deleting device:', error)
        // Fallback for demo purposes if API fails
        commit('DELETE_DEVICE', deviceId)
      }
    },
    async fetchServiceCategories({ commit }) {
      try {
        const response = await catalogService.getServiceCategories()
        commit('SET_SERVICE_CATEGORIES', response.data)
      } catch (error) {
        console.error('Error fetching service categories:', error)
        // No fallback to dummy data - we want to rely on the API
        commit('SET_SERVICE_CATEGORIES', [])
      }
    },
    async fetchProductCategories({ commit }) {
      try {
        const response = await catalogService.getProductCategories()
        commit('SET_PRODUCT_CATEGORIES', response.data)
      } catch (error) {
        console.error('Error fetching product categories:', error)
        // No fallback to dummy data - we want to rely on the API
        commit('SET_PRODUCT_CATEGORIES', [])
      }
    },
    async fetchManufacturers({ commit }) {
      try {
        const response = await catalogService.getManufacturers()
        console.log('Manufacturers API response:', response.data)
        commit('SET_MANUFACTURERS', response.data)
      } catch (error) {
        console.error('Error fetching manufacturers:', error)
        // No fallback to dummy data - we want to rely on the API
        commit('SET_MANUFACTURERS', [])
      }
    },
    async addServiceCategory({ commit }, category) {
      try {
        // In a real app, this would call the API
        const response = await api.post('/service-categories', category)
        commit('ADD_SERVICE_CATEGORY', response.data)
        return response.data
      } catch (error) {
        console.error('Error adding service category:', error)
        // For demo, we'll just add it to the store with a generated ID
        const newCategory = {
          ...category,
          id: Date.now()
        }
        commit('ADD_SERVICE_CATEGORY', newCategory)
        return newCategory
      }
    },
    async updateServiceCategory({ commit }, category) {
      try {
        const response = await api.put(`/service-categories/${category.id}`, category)
        commit('UPDATE_SERVICE_CATEGORY', response.data)
        return response.data
      } catch (error) {
        console.error('Error updating service category:', error)
        // For demo, we'll just update it in the store
        commit('UPDATE_SERVICE_CATEGORY', category)
        return category
      }
    },
    async deleteServiceCategory({ commit }, categoryId) {
      try {
        await api.delete(`/service-categories/${categoryId}`)
        commit('DELETE_SERVICE_CATEGORY', categoryId)
        return true
      } catch (error) {
        console.error('Error deleting service category:', error)
        // For demo, we'll just delete it from the store
        commit('DELETE_SERVICE_CATEGORY', categoryId)
        return true
      }
    },
    async addProductCategory({ commit }, category) {
      try {
        // In a real app, this would call the API
        const response = await api.post('/product-categories', category)
        commit('ADD_PRODUCT_CATEGORY', response.data)
        return response.data
      } catch (error) {
        console.error('Error adding product category:', error)
        // For demo, we'll just add it to the store with a generated ID
        const newCategory = {
          ...category,
          id: Date.now()
        }
        commit('ADD_PRODUCT_CATEGORY', newCategory)
        return newCategory
      }
    },
    async updateProductCategory({ commit }, category) {
      try {
        const response = await api.put(`/product-categories/${category.id}`, category)
        commit('UPDATE_PRODUCT_CATEGORY', response.data)
        return response.data
      } catch (error) {
        console.error('Error updating product category:', error)
        // For demo, we'll just update it in the store
        commit('UPDATE_PRODUCT_CATEGORY', category)
        return category
      }
    },
    async deleteProductCategory({ commit }, categoryId) {
      try {
        await api.delete(`/product-categories/${categoryId}`)
        commit('DELETE_PRODUCT_CATEGORY', categoryId)
        return true
      } catch (error) {
        console.error('Error deleting product category:', error)
        // For demo, we'll just delete it from the store
        commit('DELETE_PRODUCT_CATEGORY', categoryId)
        return true
      }
    },
    async addManufacturer({ commit }, manufacturer) {
      try {
        // Convert to Pascal case for backend API
        const normalizedManufacturer = {
          Name: manufacturer.name,
          Description: manufacturer.description
        }
        
        const response = await catalogService.createManufacturer(normalizedManufacturer)
        
        // Convert back to camel case for frontend consistency
        const newManufacturer = {
          id: response.data.Id || response.data.id,
          name: response.data.Name || response.data.name,
          description: response.data.Description || response.data.description
        }
        
        commit('ADD_MANUFACTURER', newManufacturer)
        return newManufacturer
      } catch (error) {
        console.error('Error adding manufacturer:', error)
        throw error
      }
    },
    async updateManufacturer({ commit }, manufacturer) {
      try {
        // Convert to Pascal case for backend API
        const normalizedManufacturer = {
          Id: manufacturer.id,
          Name: manufacturer.name,
          Description: manufacturer.description
        }
        
        const response = await catalogService.updateManufacturer(manufacturer.id, normalizedManufacturer)
        
        // Convert back to camel case for frontend consistency
        const updatedManufacturer = {
          id: response.data.Id || response.data.id,
          name: response.data.Name || response.data.name,
          description: response.data.Description || response.data.description
        }
        
        commit('UPDATE_MANUFACTURER', updatedManufacturer)
        return updatedManufacturer
      } catch (error) {
        console.error('Error updating manufacturer:', error)
        throw error
      }
    },
    async deleteManufacturer({ commit }, manufacturerId) {
      try {
        await catalogService.deleteManufacturer(manufacturerId)
        commit('DELETE_MANUFACTURER', manufacturerId)
        return true
      } catch (error) {
        console.error('Error deleting manufacturer:', error)
        throw error
      }
    },
    
    async fetchParts({ commit }) {
      try {
        const response = await partService.getParts()
        commit('SET_PARTS', response.data)
      } catch (error) {
        console.error('Error fetching parts:', error)
        // No fallback to dummy data - we want to rely on the API
        commit('SET_PARTS', [])
      }
    },
    
    // Workflow actions
    async fetchWorkflows({ commit }) {
      try {
        const response = await workflowService.getWorkflows()
        commit('SET_WORKFLOWS', response.data)
      } catch (error) {
        console.error('Error fetching workflows:', error)
        // No fallback to dummy data - we want to rely on the API
        commit('SET_WORKFLOWS', [])
      }
    },
    
    async fetchPrograms({ commit }) {
      try {
        const response = await workflowService.getPrograms()
        commit('SET_PROGRAMS', response.data)
      } catch (error) {
        console.error('Error fetching programs:', error)
        // No fallback to dummy data - we want to rely on the API
        commit('SET_PROGRAMS', [])
      }
    },
    
    async fetchStatusCodes({ commit }) {
      try {
        const response = await workflowService.getStatusCodes()
        commit('SET_STATUS_CODES', response.data)
      } catch (error) {
        console.error('Error fetching status codes:', error)
        // No fallback to dummy data - we want to rely on the API
        commit('SET_STATUS_CODES', [])
      }
    },
    
    async fetchServices({ commit }) {
      try {
        const response = await api.get('/catalog/services')
        commit('SET_SERVICES', response.data)
      } catch (error) {
        console.error('Error fetching services:', error)
        // No fallback to dummy data - we want to rely on the API
        commit('SET_SERVICES', [])
      }
    },
    async addService({ commit }, service) {
      try {
        // In a real app, this would call the API
        // const response = await api.post('/catalog/services', service)
        // commit('ADD_SERVICE', response.data)
        
        // For demo purposes, we'll just add it directly
        commit('ADD_SERVICE', service)
      } catch (error) {
        console.error('Error adding service:', error)
      }
    },
    async updateService({ commit }, service) {
      try {
        // In a real app, this would call the API
        // const response = await api.put(`/catalog/services/${service.id}`, service)
        // commit('UPDATE_SERVICE', response.data)
        
        // For demo purposes, we'll just update it directly
        commit('UPDATE_SERVICE', service)
      } catch (error) {
        console.error('Error updating service:', error)
      }
    },
    async deleteService({ commit }, serviceId) {
      try {
        // In a real app, this would call the API
        // await api.delete(`/catalog/services/${serviceId}`)
        
        // For demo purposes, we'll just delete it directly
        commit('DELETE_SERVICE', serviceId)
      } catch (error) {
        console.error('Error deleting service:', error)
      }
    },

    async fetchServiceParts({ commit }) {
      try {
        const response = await catalogService.getServiceParts()
        commit('SET_SERVICE_PARTS', response.data)
      } catch (error) {
        console.error('Error fetching service parts:', error)
        // For demo purposes, load some sample service-part relationships if API fails
        const sampleServiceParts = [
          { id: 1, serviceId: 1, partId: 1, quantity: 1 }, // iPhone Screen Replacement needs iPhone 13 Screen
          { id: 2, serviceId: 2, partId: 2, quantity: 1 }, // Samsung Screen Replacement needs Samsung Galaxy S21 Screen
          { id: 3, serviceId: 3, partId: 3, quantity: 1 }, // Battery Replacement - All Phones needs iPhone Battery
          { id: 4, serviceId: 3, partId: 4, quantity: 1 }, // Battery Replacement - All Phones needs Samsung Battery
          { id: 5, serviceId: 4, partId: 5, quantity: 1 }  // Water Damage Assessment needs Charging Port
        ]
        commit('SET_SERVICE_PARTS', sampleServiceParts)
      }
    },
    async addServicePart({ commit }, { serviceId, partId, quantity }) {
      try {
        // In a real app, this would call the API
        // const response = await api.post('/catalog/service-parts', { serviceId, partId, quantity })
        // commit('ADD_SERVICE_PART', response.data)
        
        // For demo purposes, we'll just add it directly
        commit('ADD_SERVICE_PART', { serviceId, partId, quantity })
      } catch (error) {
        console.error('Error adding service part:', error)
      }
    },
    async updateServicePart({ commit }, { id, quantity }) {
      try {
        // In a real app, this would call the API
        // const response = await api.put(`/catalog/service-parts/${id}`, { quantity })
        // commit('UPDATE_SERVICE_PART', response.data)
        
        // For demo purposes, we'll just update it directly
        commit('UPDATE_SERVICE_PART', { id, quantity })
      } catch (error) {
        console.error('Error updating service part:', error)
      }
    },
    async removeServicePart({ commit }, id) {
      try {
        // In a real app, this would call the API
        // await api.delete(`/catalog/service-parts/${id}`)
        
        // For demo purposes, we'll just delete it directly
        commit('REMOVE_SERVICE_PART', id)
      } catch (error) {
        console.error('Error removing service part:', error)
      }
    },
    async addPart({ commit }, part) {
      try {
        // In a real app, this would call the API
        // const response = await api.post('/catalog/parts', part)
        // commit('ADD_PART', response.data)
        
        // For demo purposes, we'll just add it directly
        commit('ADD_PART', part)
      } catch (error) {
        console.error('Error adding part:', error)
      }
    },
    async updatePart({ commit }, part) {
      try {
        // In a real app, this would call the API
        // const response = await api.put(`/catalog/parts/${part.id}`, part)
        // commit('UPDATE_PART', response.data)
        
        // For demo purposes, we'll just update it directly
        commit('UPDATE_PART', part)
      } catch (error) {
        console.error('Error updating part:', error)
      }
    },
    async deletePart({ commit }, partId) {
      try {
        // In a real app, this would call the API
        // await api.delete(`/catalog/parts/${partId}`)
        
        // For demo purposes, we'll just delete it directly
        commit('DELETE_PART', partId)
      } catch (error) {
        console.error('Error deleting part:', error)
      }
    },
    // Work order actions
    async fetchWorkOrders({ commit }) {
      try {
        const response = await workOrderService.getWorkOrders()
        commit('SET_WORK_ORDERS', response.data)
        
        // Set the work order counter based on the highest ID from the API
        if (response.data && response.data.length > 0) {
          const highestId = Math.max(...response.data.map(wo => wo.id || wo.Id || 0))
          commit('SET_WORK_ORDER_COUNTER', highestId)
        }
      } catch (error) {
        console.error('Error fetching work orders:', error)
        // No fallback to dummy data - we want to rely on the API
        commit('SET_WORK_ORDERS', [])
        commit('SET_WORK_ORDER_COUNTER', 0)
      }
    },
    async addWorkOrder({ commit, state }, workOrder) {
      try {
        // Generate a new work order code
        const counter = state.workOrderCounter + 1
        const code = `WO${counter.toString().padStart(5, '0')}`
        
        // Set the group ID to the current group if not provided
        const groupId = workOrder.groupId || (state.currentGroup ? state.currentGroup.id : 1)
        
        // Add device and service names for display
        const device = state.devices.find(d => d.id === workOrder.deviceId);
        const service = state.services.find(s => s.id === workOrder.serviceId);
        
        // Create the complete work order object
        const completeWorkOrder = {
          ...workOrder,
          code,
          groupId,
          deviceName: device ? device.name : 'Unknown Device',
          serviceName: service ? service.name : 'Unknown Service',
          createdAt: new Date().toISOString(),
          updatedAt: null
        }
        
        // Call the API
        const response = await workOrderService.createWorkOrder(completeWorkOrder)
        commit('ADD_WORK_ORDER', response.data)
        commit('SET_WORK_ORDER_COUNTER', counter)
        return response.data
      } catch (error) {
        console.error('Error adding work order:', error)
        
        // For demo purposes, provide a fallback if API fails
        const completeWorkOrder = {
          ...workOrder,
          id: Date.now(), // Generate a temporary ID
          code: `WO${state.workOrderCounter + 1}`.padStart(7, '0'),
          groupId: workOrder.groupId || (state.currentGroup ? state.currentGroup.id : 1),
          deviceName: state.devices.find(d => d.id === workOrder.deviceId)?.name || 'Unknown Device',
          serviceName: state.services.find(s => s.id === workOrder.serviceId)?.name || 'Unknown Service',
          createdAt: new Date().toISOString(),
          updatedAt: null
        }
        commit('ADD_WORK_ORDER', completeWorkOrder)
        commit('SET_WORK_ORDER_COUNTER', state.workOrderCounter + 1)
        return completeWorkOrder
      }
    },
    async updateWorkOrder({ commit, state }, workOrder) {
      // Add device and service names for display
      const device = state.devices.find(d => d.id === workOrder.deviceId);
      const service = state.services.find(s => s.id === workOrder.serviceId);
      
      const updatedWorkOrder = {
        ...workOrder,
        deviceName: device ? device.name : 'Unknown Device',
        serviceName: service ? service.name : 'Unknown Service'
      };
      
      try {
        // Call the API
        const response = await workOrderService.updateWorkOrder(workOrder.id, updatedWorkOrder)
        commit('UPDATE_WORK_ORDER', response.data)
        return response.data
      } catch (error) {
        console.error('Error updating work order:', error)
        
        // For demo purposes, provide a fallback if API fails
        commit('UPDATE_WORK_ORDER', updatedWorkOrder)
        return updatedWorkOrder
      }
    },
    async deleteWorkOrder({ commit }, workOrderId) {
      try {
        // Call the API
        await workOrderService.deleteWorkOrder(workOrderId)
        commit('DELETE_WORK_ORDER', workOrderId)
      } catch (error) {
        console.error('Error deleting work order:', error)
        
        // For demo purposes, provide a fallback if API fails
        commit('DELETE_WORK_ORDER', workOrderId)
      }
    },
    async fetchWorkOrder(context, id) {
      try {
        const response = await workOrderService.getWorkOrder(id)
        return response.data
      } catch (error) {
        console.error(`Error fetching work order ${id}:`, error)
      }
    },
    async createWorkOrder({ commit }, workOrder) {
      try {
        const response = await workOrderService.createWorkOrder(workOrder)
        commit('ADD_WORK_ORDER', response.data)
        return response.data
      } catch (error) {
        console.error('Error creating work order:', error)
      }
    },

    // Workflow actions

    async fetchWorkflow(context, id) {
      try {
        const response = await api.get(`/workflows/${id}`)
        return response.data
      } catch (error) {
        console.error(`Error fetching workflow ${id}:`, error)
      }
    },
    async createWorkflow({ commit }, workflow) {
      try {
        const response = await api.post('/workflows', workflow)
        commit('ADD_WORKFLOW', response.data)
        return response.data
      } catch (error) {
        console.error('Error creating workflow:', error)
      }
    },
    async updateWorkflow({ commit }, { id, workflow }) {
      try {
        const response = await api.put(`/workflows/${id}`, workflow)
        commit('UPDATE_WORKFLOW', response.data)
        return response.data
      } catch (error) {
        console.error(`Error updating workflow ${id}:`, error)
      }
    },
    // Program actions

    async fetchProgram(context, id) {
      try {
        const response = await api.get(`/programs/${id}`)
        return response.data
      } catch (error) {
        console.error(`Error fetching program ${id}:`, error)
      }
    },
    async createProgram({ commit }, program) {
      try {
        const response = await api.post('/programs', program)
        commit('ADD_PROGRAM', response.data)
        return response.data
      } catch (error) {
        console.error('Error creating program:', error)
      }
    },
    async updateProgram({ commit }, { id, program }) {
      try {
        const response = await workflowService.updateProgram(id, program)
        commit('UPDATE_PROGRAM', response.data)
        return response.data
      } catch (error) {
        console.error(`Error updating program ${id}:`, error)
      }
    },
    async login({ commit, dispatch }, credentials) {
      try {
        // Use the authService to login and handle JWT token storage
        const response = await authService.login(credentials);
        const { user } = response.data;
        
        // Update Vuex state
        commit('SET_AUTH', { user, isAuthenticated: true });
        
        // Set the current group if provided in credentials
        if (credentials.groupId) {
          await dispatch('fetchGroups');
          const group = this.getters.getGroupById(credentials.groupId);
          if (group) {
            dispatch('setCurrentGroup', group);
          }
        }
        
        return true;
      } catch (error) {
        console.error('Login error:', error);
        return false;
      }
    },
    
    async logout({ commit }) {
      // Use the authService to handle logout and JWT token removal
      await authService.logout();
      
      // Update Vuex state
      commit('LOGOUT');
    },
    
    async checkAuth({ commit, dispatch, state }) {
      // If we're already authenticated in Vuex state, just verify token
      if (state.isAuthenticated && state.user) {
        // If token is invalid, log out
        if (!authService.isTokenValid()) {
          await dispatch('logout');
        }
        return;
      }
      
      // Check if we have a valid token
      if (authService.isTokenValid()) {
        try {
          // Get current user data
          const userData = authService.getUser();
          if (userData) {
            // Update Vuex state with the stored user data
            commit('SET_AUTH', { user: userData, isAuthenticated: true });
            
            // If the user has a group, set it as current
            if (userData.groupId) {
              await dispatch('fetchGroups');
              const group = this.getters.getGroupById(userData.groupId);
              if (group) {
                dispatch('setCurrentGroup', group);
              }
            }
            return;
          }
          
          // If no stored user data, try to get it from the API
          const response = await authService.getCurrentUser();
          commit('SET_AUTH', { user: response.data, isAuthenticated: true });
        } catch (error) {
          console.error('Authentication check error:', error);
          await dispatch('logout');
        }
      }
    },
// ... (rest of the code remains the same)
    async fetchUserRoles({ commit }) {
      try {
        const response = await api.get('/user-roles')
        commit('SET_USER_ROLES', response.data)
      } catch (error) {
        console.error('Error fetching user roles:', error)
        // For demo purposes, create some sample roles if API fails
        const demoRoles = [
          {
            id: 1,
            name: 'Administrator',
            description: 'Full system access',
            isAdmin: true,
            isReadOnly: false
          },
          {
            id: 2,
            name: 'Manager',
            description: 'Can manage work orders and catalog',
            isAdmin: false,
            isReadOnly: false
          },
          {
            id: 3,
            name: 'Viewer',
            description: 'Read-only access to the system',
            isAdmin: false,
            isReadOnly: true
          }
        ]
        commit('SET_USER_ROLES', demoRoles)
      }
    },
    async addUserRole({ commit }, role) {
      try {
        const response = await api.post('/user-roles', role)
        commit('ADD_USER_ROLE', response.data)
        return response.data
      } catch (error) {
        console.error('Error creating user role:', error)
        // For demo purposes
        const newRole = {
          ...role,
          id: Math.floor(Math.random() * 10000) + 10
        }
        commit('ADD_USER_ROLE', newRole)
        return newRole
      }
    },
    async updateUserRole({ commit }, role) {
      try {
        const response = await api.put(`/user-roles/${role.id}`, role)
        commit('UPDATE_USER_ROLE', response.data)
        return response.data
      } catch (error) {
        console.error(`Error updating user role ${role.id}:`, error)
        // For demo purposes
        commit('UPDATE_USER_ROLE', role)
        return role
      }
    },
    async deleteUserRole({ commit }, roleId) {
      try {
        await api.delete(`/user-roles/${roleId}`)
        commit('DELETE_USER_ROLE', roleId)
      } catch (error) {
        console.error(`Error deleting user role ${roleId}:`, error)
        // For demo purposes
        commit('DELETE_USER_ROLE', roleId)
      }
    },
    
    // Catalog Pricing actions
    async fetchCatalogPricing({ commit }) {
      try {
        const response = await api.get('/catalog-pricing')
        commit('SET_CATALOG_PRICING', response.data)
      } catch (error) {
        console.error('Error fetching catalog pricing:', error)
        // For demo purposes, create some sample pricing if API fails
        const demoPricing = [
          {
            id: 1,
            catalogItemId: 1,
            catalogItemType: 'Service',
            catalogItemName: 'Screen Replacement',
            price: 149.99
          },
          {
            id: 2,
            catalogItemId: 2,
            catalogItemType: 'Service',
            catalogItemName: 'Battery Replacement',
            price: 79.99
          },
          {
            id: 3,
            catalogItemId: 3,
            catalogItemType: 'Part',
            catalogItemName: 'iPhone Screen',
            price: 89.99
          }
        ]
        commit('SET_CATALOG_PRICING', demoPricing)
      }
    },
    async addCatalogPrice({ commit }, price) {
      try {
        const response = await api.post('/catalog-pricing', price)
        commit('ADD_CATALOG_PRICE', response.data)
        return response.data
      } catch (error) {
        console.error('Error creating catalog price:', error)
        // For demo purposes
        const newPrice = {
          ...price,
          id: Math.floor(Math.random() * 10000) + 10
        }
        commit('ADD_CATALOG_PRICE', newPrice)
        return newPrice
      }
    },
    async updateCatalogPrice({ commit }, price) {
      try {
        const response = await api.put(`/catalog-pricing/${price.id}`, price)
        commit('UPDATE_CATALOG_PRICE', response.data)
        return response.data
      } catch (error) {
        console.error(`Error updating catalog price ${price.id}:`, error)
        // For demo purposes
        commit('UPDATE_CATALOG_PRICE', price)
        return price
      }
    },
    async deleteCatalogPrice({ commit }, priceId) {
      try {
        await api.delete(`/catalog-pricing/${priceId}`)
        commit('DELETE_CATALOG_PRICE', priceId)
      } catch (error) {
        console.error(`Error deleting catalog price ${priceId}:`, error)
        // For demo purposes
        commit('DELETE_CATALOG_PRICE', priceId)
      }
    },
    
    // Inventory actions
    async fetchInventory({ commit }) {
      try {
        const response = await inventoryService.getInventory()
        commit('SET_INVENTORY', response.data)
      } catch (error) {
        console.error('Error fetching inventory:', error)
        // For demo purposes, create some sample inventory if API fails
        const demoInventory = [
          {
            id: 1,
            groupId: 1,
            catalogItemId: 1,
            catalogItemType: 'Part',
            catalogItemName: 'iPhone Screen',
            quantity: 25,
            minimumQuantity: 5,
            lastUpdated: new Date()
          },
          {
            id: 2,
            groupId: 1,
            catalogItemId: 2,
            catalogItemType: 'Part',
            catalogItemName: 'Samsung Screen',
            quantity: 15,
            minimumQuantity: 3,
            lastUpdated: new Date()
          },
          {
            id: 3,
            groupId: 2,
            catalogItemId: 1,
            catalogItemType: 'Part',
            catalogItemName: 'iPhone Screen',
            quantity: 10,
            minimumQuantity: 5,
            lastUpdated: new Date()
          }
        ]
        commit('SET_INVENTORY', demoInventory)
      }
    },
    async fetchGroupInventory(context, groupId) {
      try {
        const response = await inventoryService.getInventoryByGroup(groupId)
        return response.data
      } catch (error) {
        console.error(`Error fetching inventory for group ${groupId}:`, error)
        // Return demo data filtered by group
        return this.state.inventory.filter(item => item.groupId === groupId)
      }
    },
    async addInventoryItem({ commit }, item) {
      try {
        const response = await inventoryService.createInventoryItem(item)
        commit('ADD_INVENTORY_ITEM', response.data)
        return response.data
      } catch (error) {
        console.error('Error creating inventory item:', error)
        // For demo purposes
        const newItem = {
          ...item,
          id: Math.floor(Math.random() * 10000) + 10,
          lastUpdated: new Date()
        }
        commit('ADD_INVENTORY_ITEM', newItem)
        return newItem
      }
    },
    async updateInventoryItem({ commit }, item) {
      try {
        const response = await inventoryService.updateInventoryItem(item.id, item)
        commit('UPDATE_INVENTORY_ITEM', response.data)
        return response.data
      } catch (error) {
        console.error(`Error updating inventory item ${item.id}:`, error)
        // For demo purposes
        const updatedItem = {
          ...item,
          lastUpdated: new Date()
        }
        commit('UPDATE_INVENTORY_ITEM', updatedItem)
        return updatedItem
      }
    },
    async deleteInventoryItem({ commit }, { groupId, catalogItemId, catalogItemType }) {
      try {
        await inventoryService.deleteInventoryItem(catalogItemId)
        commit('DELETE_INVENTORY_ITEM', { groupId, catalogItemId, catalogItemType })
      } catch (error) {
        console.error(`Error deleting inventory item:`, error)
        // For demo purposes
        commit('DELETE_INVENTORY_ITEM', { groupId, catalogItemId, catalogItemType })
      }
    },

    // Group actions
    async fetchGroups({ commit }) {
      try {
        const response = await groupService.getGroups()
        commit('SET_GROUPS', response.data)
      } catch (error) {
        console.error('Error fetching groups:', error)
        // For demo purposes, create some sample groups if API fails
        const demoGroups = [
          {
            id: 1,
            code: 'MAIN',
            description: 'Main Repair Center',
            address1: '123 Main Street',
            address2: 'Suite 100',
            address3: '',
            address4: '',
            city: 'Boston',
            state: 'MA',
            zip: '02108',
            country: 'USA'
          },
          {
            id: 2,
            code: 'WEST',
            description: 'West Coast Repair Center',
            address1: '456 Tech Blvd',
            address2: '',
            address3: '',
            address4: '',
            city: 'San Francisco',
            state: 'CA',
            zip: '94105',
            country: 'USA'
          },
          {
            id: 3,
            code: 'SOUTH',
            description: 'Southern Repair Center',
            address1: '789 Repair Lane',
            address2: 'Building B',
            address3: '',
            address4: '',
            city: 'Austin',
            state: 'TX',
            zip: '78701',
            country: 'USA'
          }
        ]
        commit('SET_GROUPS', demoGroups)
      }
    },
    async fetchGroup(context, id) {
      try {
        const response = await groupService.getGroup(id)
        return response.data
      } catch (error) {
        console.error(`Error fetching group ${id}:`, error)
      }
    },
    async addGroup({ commit }, group) {
      try {
        const response = await groupService.createGroup(group)
        commit('ADD_GROUP', response.data)
        return response.data
      } catch (error) {
        console.error('Error creating group:', error)
        // For demo purposes
        const newGroup = {
          ...group,
          id: Math.floor(Math.random() * 10000) + 10
        }
        commit('ADD_GROUP', newGroup)
        return newGroup
      }
    },
    async updateGroup({ commit }, group) {
      try {
        const response = await groupService.updateGroup(group.id, group)
        commit('UPDATE_GROUP', response.data)
        return response.data
      } catch (error) {
        console.error(`Error updating group ${group.id}:`, error)
        // For demo purposes
        commit('UPDATE_GROUP', group)
        return group
      }
    },
    async deleteGroup({ commit }, groupId) {
      try {
        await groupService.deleteGroup(groupId)
        commit('DELETE_GROUP', groupId)
      } catch (error) {
        console.error(`Error deleting group ${groupId}:`, error)
        // For demo purposes
        commit('DELETE_GROUP', groupId)
      }
    },
    setCurrentGroup({ commit }, group) {
      commit('SET_CURRENT_GROUP', group)
    }
  }
})
