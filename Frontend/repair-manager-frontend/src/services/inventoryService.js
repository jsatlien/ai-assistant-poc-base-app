import api from './api';

export default {
  // Inventory
  getInventory() {
    return api.get('/inventory');
  },
  getInventoryItem(id) {
    return api.get(`/inventory/${id}`);
  },
  createInventoryItem(item) {
    return api.post('/inventory', item);
  },
  updateInventoryItem(id, item) {
    return api.put(`/inventory/${id}`, item);
  },
  deleteInventoryItem(id) {
    return api.delete(`/inventory/${id}`);
  },
  
  // Get inventory by group
  getInventoryByGroup(groupId) {
    return api.get(`/inventory/group/${groupId}`);
  },
  
  // Get inventory by part
  getInventoryByPart(partId) {
    return api.get(`/inventory/part/${partId}`);
  }
};
