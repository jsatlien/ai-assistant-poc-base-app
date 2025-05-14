import api from './api';

export default {
  // Work Orders
  getWorkOrders() {
    return api.get('/WorkOrders');
  },
  getWorkOrder(id) {
    return api.get(`/WorkOrders/${id}`);
  },
  createWorkOrder(workOrder) {
    return api.post('/WorkOrders', workOrder);
  },
  updateWorkOrder(id, workOrder) {
    return api.put(`/WorkOrders/${id}`, workOrder);
  },
  deleteWorkOrder(id) {
    return api.delete(`/WorkOrders/${id}`);
  },
  
  // Get work orders by status
  getWorkOrdersByStatus(status) {
    return api.get(`/work-orders/status/${status}`);
  },
  
  // Get work orders by group
  getWorkOrdersByGroup(groupId) {
    return api.get(`/work-orders/group/${groupId}`);
  },
  
  // Update work order status
  updateWorkOrderStatus(id, status) {
    return api.put(`/work-orders/${id}/status`, { status });
  }
};
