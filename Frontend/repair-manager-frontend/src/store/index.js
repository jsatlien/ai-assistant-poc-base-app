import Vue from 'vue'
import Vuex from 'vuex'
import api from '../services/api'

Vue.use(Vuex)

export default new Vuex.Store({
  state: {
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
    getWorkOrdersByGroup: state => groupId => {
      if (groupId === 'all') return state.workOrders;
      return state.workOrders.filter(wo => wo.groupId === groupId);
    },
    // Workflow getters
    getWorkflows: state => state.workflows,
    getWorkflowById: state => id => state.workflows.find(w => w.id === id),
    // Program getters
    getPrograms: state => state.programs,
    getProgramById: state => id => state.programs.find(p => p.id === id),
    // Group getters
    getGroups: state => state.groups,
    getGroupById: state => id => state.groups.find(g => g.id === id),
    getCurrentGroup: state => state.currentGroup,
    // Auth getters
    isAuthenticated: state => state.isAuthenticated,
    currentUser: state => state.user,
    isAdmin: state => state.user && state.user.isAdmin,
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
    SET_WORK_ORDER_COUNTER(state, counter) {
      state.workOrderCounter = counter
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
        const response = await api.get('/catalog/devices')
        commit('SET_DEVICES', response.data)
      } catch (error) {
        console.error('Error fetching devices:', error)
        // For demo purposes, load some sample devices if API fails
        const sampleDevices = [
          { id: 1, name: 'iPhone 13', description: 'Apple smartphone with 6.1-inch display', sku: 'DEV-APL-001' },
          { id: 2, name: 'Samsung Galaxy S21', description: 'Android smartphone with 6.2-inch display', sku: 'DEV-SMS-001' },
          { id: 3, name: 'iPad Pro', description: '12.9-inch tablet with M1 chip', sku: 'DEV-APL-002' },
          { id: 4, name: 'MacBook Air', description: '13-inch laptop with Apple M1 chip', sku: 'DEV-APL-003' },
          { id: 5, name: 'Dell XPS 13', description: '13-inch Windows laptop with Intel Core i7', sku: 'DEV-DEL-001' }
        ]
        commit('SET_DEVICES', sampleDevices)
      }
    },
    async addDevice({ commit }, device) {
      try {
        // In a real app, this would call the API
        // const response = await api.post('/catalog/devices', device)
        // commit('ADD_DEVICE', response.data)
        
        // For demo purposes, we'll just add it directly
        commit('ADD_DEVICE', device)
      } catch (error) {
        console.error('Error adding device:', error)
      }
    },
    async updateDevice({ commit }, device) {
      try {
        // In a real app, this would call the API
        // const response = await api.put(`/catalog/devices/${device.id}`, device)
        // commit('UPDATE_DEVICE', response.data)
        
        // For demo purposes, we'll just update it directly
        commit('UPDATE_DEVICE', device)
      } catch (error) {
        console.error('Error updating device:', error)
      }
    },
    async deleteDevice({ commit }, deviceId) {
      try {
        // In a real app, this would call the API
        // await api.delete(`/catalog/devices/${deviceId}`)
        
        // For demo purposes, we'll just delete it directly
        commit('DELETE_DEVICE', deviceId)
      } catch (error) {
        console.error('Error deleting device:', error)
      }
    },
    async fetchServiceCategories({ commit }) {
      try {
        const response = await api.get('/service-categories')
        commit('SET_SERVICE_CATEGORIES', response.data)
      } catch (error) {
        console.error('Error fetching service categories:', error)
        // Fallback to sample data if API fails
        const sampleCategories = [
          { id: 1, name: 'Screen Repairs', description: 'Services for screen replacements and repairs' },
          { id: 2, name: 'Battery Services', description: 'Battery replacement and optimization services' },
          { id: 3, name: 'Water Damage', description: 'Water damage assessment and repair services' },
          { id: 4, name: 'Hardware Repairs', description: 'Hardware component repair and replacement' },
          { id: 5, name: 'Data Services', description: 'Data recovery and transfer services' }
        ]
        commit('SET_SERVICE_CATEGORIES', sampleCategories)
      }
    },
    async fetchProductCategories({ commit }) {
      try {
        const response = await api.get('/product-categories')
        commit('SET_PRODUCT_CATEGORIES', response.data)
      } catch (error) {
        console.error('Error fetching product categories:', error)
        // Fallback to sample data if API fails
        const sampleProductCategories = [
          { id: 1, name: 'Smartphones', description: 'Mobile phones with advanced computing capability' },
          { id: 2, name: 'Tablets', description: 'Portable touchscreen computers' },
          { id: 3, name: 'Laptops', description: 'Portable personal computers' },
          { id: 4, name: 'Desktops', description: 'Personal computers designed for regular use at a single location' },
          { id: 5, name: 'Wearables', description: 'Smart electronic devices that can be worn on the body' }
        ]
        commit('SET_PRODUCT_CATEGORIES', sampleProductCategories)
      }
    },
    async fetchManufacturers({ commit }) {
      try {
        const response = await api.get('/manufacturers')
        commit('SET_MANUFACTURERS', response.data)
      } catch (error) {
        console.error('Error fetching manufacturers:', error)
        // Fallback to sample data if API fails
        const sampleManufacturers = [
          { id: 1, name: 'Apple', description: 'Manufacturer of iPhone, iPad, and Mac computers' },
          { id: 2, name: 'Samsung', description: 'Manufacturer of Galaxy smartphones and tablets' },
          { id: 3, name: 'Google', description: 'Manufacturer of Pixel smartphones and other devices' },
          { id: 4, name: 'Microsoft', description: 'Manufacturer of Surface devices and Xbox consoles' },
          { id: 5, name: 'Dell', description: 'Manufacturer of laptops, desktops, and servers' },
          { id: 6, name: 'HP', description: 'Manufacturer of laptops, desktops, and printers' },
          { id: 7, name: 'Lenovo', description: 'Manufacturer of ThinkPad laptops and other devices' }
        ]
        commit('SET_MANUFACTURERS', sampleManufacturers)
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
        // In a real app, this would call the API
        const response = await api.post('/manufacturers', manufacturer)
        commit('ADD_MANUFACTURER', response.data)
        return response.data
      } catch (error) {
        console.error('Error adding manufacturer:', error)
        // For demo, we'll just add it to the store with a generated ID
        const newManufacturer = {
          ...manufacturer,
          id: Date.now()
        }
        commit('ADD_MANUFACTURER', newManufacturer)
        return newManufacturer
      }
    },
    async updateManufacturer({ commit }, manufacturer) {
      try {
        const response = await api.put(`/manufacturers/${manufacturer.id}`, manufacturer)
        commit('UPDATE_MANUFACTURER', response.data)
        return response.data
      } catch (error) {
        console.error('Error updating manufacturer:', error)
        // For demo, we'll just update it in the store
        commit('UPDATE_MANUFACTURER', manufacturer)
        return manufacturer
      }
    },
    async deleteManufacturer({ commit }, manufacturerId) {
      try {
        await api.delete(`/manufacturers/${manufacturerId}`)
        commit('DELETE_MANUFACTURER', manufacturerId)
        return true
      } catch (error) {
        console.error('Error deleting manufacturer:', error)
        // For demo, we'll just delete it from the store
        commit('DELETE_MANUFACTURER', manufacturerId)
        return true
      }
    },
    async fetchServices({ commit }) {
      try {
        const response = await api.get('/catalog/services')
        commit('SET_SERVICES', response.data)
      } catch (error) {
        console.error('Error fetching services:', error)
        // For demo purposes, load some sample services if API fails
        const sampleServices = [
          { 
            id: 1, 
            name: 'iPhone Screen Replacement', 
            description: 'Replace damaged screens for iPhone devices',
            categoryId: 1, // Screen Repairs
            deviceId: 1, // iPhone 13
            sku: 'SRV-SCR-001'
          },
          { 
            id: 2, 
            name: 'Samsung Screen Replacement', 
            description: 'Replace damaged screens for Samsung devices',
            categoryId: 1, // Screen Repairs
            deviceId: 2, // Samsung Galaxy S21
            sku: 'SRV-SCR-002'
          },
          { 
            id: 3, 
            name: 'Battery Replacement - All Phones', 
            description: 'Replace old or damaged batteries for all phone types',
            categoryId: 2, // Battery Services
            deviceId: null, // Not device-specific
            sku: 'SRV-BAT-001'
          },
          { 
            id: 4, 
            name: 'Water Damage Assessment', 
            description: 'Initial assessment of water-damaged devices',
            categoryId: 3, // Water Damage
            deviceId: null, // Not device-specific
            sku: 'SRV-WTR-001'
          },
          { 
            id: 5, 
            name: 'Data Recovery Service', 
            description: 'Recover lost or deleted data from damaged devices',
            categoryId: 5, // Data Services
            deviceId: null, // Not device-specific
            sku: 'SRV-DAT-001'
          }
        ]
        commit('SET_SERVICES', sampleServices)
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
    async fetchPrograms({ commit }) {
      try {
        const response = await api.get('/programs')
        commit('SET_PROGRAMS', response.data)
      } catch (error) {
        console.error('Error fetching programs:', error)
        // For demo purposes, load some sample programs if API fails
        const samplePrograms = [
          { 
            id: 1, 
            name: 'Standard Repair', 
            description: 'Standard repair program for most devices', 
            workflowId: 1,
            warrantyType: 'ALL',
            eligibleDevices: {
              type: 'ALL',
              productCategories: []
            },
            pricingMode: 'Itemized'
          },
          { 
            id: 2, 
            name: 'Express Repair', 
            description: 'Expedited repair program with faster turnaround', 
            workflowId: 2,
            warrantyType: 'IW',
            eligibleDevices: {
              type: 'SELECTED',
              productCategories: [1, 2]
            },
            pricingMode: 'Service Levels'
          },
          { 
            id: 3, 
            name: 'Advanced Repair', 
            description: 'Advanced repair program for complex issues', 
            workflowId: 3,
            warrantyType: 'OOW',
            eligibleDevices: {
              type: 'SELECTED',
              productCategories: [3, 4]
            },
            pricingMode: 'Itemized'
          }
        ]
        commit('SET_PROGRAMS', samplePrograms)
      }
    },
    async fetchServiceParts({ commit }) {
      try {
        const response = await api.get('/catalog/service-parts')
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
        const response = await api.get('/workorders')
        commit('SET_WORK_ORDERS', response.data)
      } catch (error) {
        console.error('Error fetching work orders:', error)
        // For demo purposes, load some sample work orders if API fails
        const sampleWorkOrders = [
          { 
            id: 1, 
            code: 'WO00001',
            customerName: 'John Smith', 
            customerPhone: '555-123-4567',
            deviceId: 1,
            deviceName: 'iPhone 13',
            serviceId: 1,
            serviceName: 'Screen Replacement',
            issueDescription: 'Cracked screen after dropping phone',
            status: 'in-progress',
            groupId: 1, // Main Repair Center
            createdAt: '2025-05-10T14:30:00Z',
            updatedAt: '2025-05-11T09:15:00Z'
          },
          { 
            id: 2, 
            code: 'WO00002',
            customerName: 'Sarah Johnson', 
            customerPhone: '555-987-6543',
            deviceId: 2,
            deviceName: 'Samsung Galaxy S21',
            serviceId: 2,
            serviceName: 'Battery Replacement',
            issueDescription: 'Battery drains very quickly, needs replacement',
            status: 'open',
            groupId: 1, // Main Repair Center
            createdAt: '2025-05-11T16:45:00Z',
            updatedAt: null
          },
          { 
            id: 3, 
            code: 'WO00003',
            customerName: 'Michael Brown', 
            customerPhone: '555-456-7890',
            deviceId: 4,
            deviceName: 'MacBook Air',
            serviceId: 4,
            serviceName: 'Software Troubleshooting',
            issueDescription: 'System crashes frequently when using video editing software',
            status: 'completed',
            groupId: 2, // West Coast Repair Center
            createdAt: '2025-05-09T10:15:00Z',
            updatedAt: '2025-05-12T11:30:00Z'
          },
          { 
            id: 4, 
            code: 'WO00004',
            customerName: 'Emily Davis', 
            customerPhone: '555-789-0123',
            deviceId: 3,
            deviceName: 'iPad Pro',
            serviceId: 3,
            serviceName: 'Water Damage Repair',
            issueDescription: 'Device was dropped in water, not turning on',
            status: 'cancelled',
            groupId: 3, // Southern Repair Center
            createdAt: '2025-05-08T09:00:00Z',
            updatedAt: '2025-05-08T14:20:00Z'
          }
        ]
        commit('SET_WORK_ORDERS', sampleWorkOrders)
        // Set the work order counter to the highest ID
        commit('SET_WORK_ORDER_COUNTER', 4)
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
          id: Date.now(), // Generate a temporary ID
          code,
          groupId,
          deviceName: device ? device.name : 'Unknown Device',
          serviceName: service ? service.name : 'Unknown Service',
          createdAt: new Date().toISOString(),
          updatedAt: null
        }
        
        // In a real app, this would call the API
        // const response = await api.post('/workorders', completeWorkOrder)
        // commit('ADD_WORK_ORDER', response.data)
        
        // For demo purposes, we'll just add it directly
        commit('ADD_WORK_ORDER', completeWorkOrder)
        commit('SET_WORK_ORDER_COUNTER', counter)
      } catch (error) {
        console.error('Error adding work order:', error)
      }
    },
    async updateWorkOrder({ commit, state }, workOrder) {
      try {
        // Add device and service names for display
        const device = state.devices.find(d => d.id === workOrder.deviceId);
        const service = state.services.find(s => s.id === workOrder.serviceId);
        
        const updatedWorkOrder = {
          ...workOrder,
          deviceName: device ? device.name : 'Unknown Device',
          serviceName: service ? service.name : 'Unknown Service'
        };
        
        // In a real app, this would call the API
        // const response = await api.put(`/workorders/${workOrder.id}`, updatedWorkOrder)
        // commit('UPDATE_WORK_ORDER', response.data)
        
        // For demo purposes, we'll just update it directly
        commit('UPDATE_WORK_ORDER', updatedWorkOrder)
      } catch (error) {
        console.error('Error updating work order:', error)
      }
    },
    async deleteWorkOrder({ commit }, workOrderId) {
      try {
        // In a real app, this would call the API
        // await api.delete(`/workorders/${workOrderId}`)
        
        // For demo purposes, we'll just delete it directly
        commit('DELETE_WORK_ORDER', workOrderId)
      } catch (error) {
        console.error('Error deleting work order:', error)
      }
    },
    async fetchWorkOrder(context, id) {
      try {
        const response = await api.get(`/workorders/${id}`)
        return response.data
      } catch (error) {
        console.error(`Error fetching work order ${id}:`, error)
      }
    },
    async createWorkOrder({ commit }, workOrder) {
      try {
        const response = await api.post('/workorders', workOrder)
        commit('ADD_WORK_ORDER', response.data)
        return response.data
      } catch (error) {
        console.error('Error creating work order:', error)
      }
    },

    // Workflow actions
    async fetchWorkflows({ commit }) {
      try {
        const response = await api.get('/workflows')
        commit('SET_WORKFLOWS', response.data)
      } catch (error) {
        console.error('Error fetching workflows:', error)
      }
    },
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
        const response = await api.put(`/programs/${id}`, program)
        commit('UPDATE_PROGRAM', response.data)
        return response.data
      } catch (error) {
        console.error(`Error updating program ${id}:`, error)
      }
    },
    // Auth actions
    async login({ commit }, credentials) {
      try {
        const response = await api.post('/auth/login', credentials)
        const { user, token } = response.data
        
        // Store token in localStorage
        localStorage.setItem('auth_token', token)
        
        // Set default Authorization header for all future requests
        api.defaults.headers.common['Authorization'] = `Bearer ${token}`
        
        // For demo purposes, set admin status
        user.isAdmin = true
        
        commit('SET_AUTH', { user, isAuthenticated: true })
        return user
      } catch (error) {
        console.error('Login error:', error)
        throw error
      }
    },
    
    // User Roles actions
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
        const response = await api.get('/inventory')
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
        const response = await api.get(`/inventory/group/${groupId}`)
        return response.data
      } catch (error) {
        console.error(`Error fetching inventory for group ${groupId}:`, error)
        // Return demo data filtered by group
        return this.state.inventory.filter(item => item.groupId === groupId)
      }
    },
    async addInventoryItem({ commit }, item) {
      try {
        const response = await api.post('/inventory', item)
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
        const response = await api.put(`/inventory/${item.id}`, item)
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
        await api.delete(`/inventory/${groupId}/${catalogItemId}/${catalogItemType}`)
        commit('DELETE_INVENTORY_ITEM', { groupId, catalogItemId, catalogItemType })
      } catch (error) {
        console.error(`Error deleting inventory item:`, error)
        // For demo purposes
        commit('DELETE_INVENTORY_ITEM', { groupId, catalogItemId, catalogItemType })
      }
    },
    logout({ commit }) {
      // Remove token from localStorage
      localStorage.removeItem('auth_token')
      
      // Remove Authorization header
      delete api.defaults.headers.common['Authorization']
      
      commit('SET_AUTH', { user: null, isAuthenticated: false })
    },
    checkAuth({ commit }) {
      const token = localStorage.getItem('auth_token')
      if (token) {
        // Set default Authorization header
        api.defaults.headers.common['Authorization'] = `Bearer ${token}`
        
        // For simplicity, we're just setting isAuthenticated to true
        // In a real app, you'd verify the token with the server
        commit('SET_AUTH', { user: { name: 'Admin User', isAdmin: true }, isAuthenticated: true })

        // Restore current group from localStorage if available
        const savedGroup = localStorage.getItem('currentGroup')
        if (savedGroup) {
          commit('SET_CURRENT_GROUP', JSON.parse(savedGroup))
        }
        
        return true
      }
      return false
    },

    // Group actions
    async fetchGroups({ commit }) {
      try {
        const response = await api.get('/groups')
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
        const response = await api.get(`/groups/${id}`)
        return response.data
      } catch (error) {
        console.error(`Error fetching group ${id}:`, error)
      }
    },
    async addGroup({ commit }, group) {
      try {
        const response = await api.post('/groups', group)
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
        const response = await api.put(`/groups/${group.id}`, group)
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
        await api.delete(`/groups/${groupId}`)
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
