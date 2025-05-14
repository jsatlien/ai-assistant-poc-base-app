import api from './api';

export default {
  // Groups
  getGroups() {
    return api.get('/Groups');
  },
  getGroup(id) {
    return api.get(`/Groups/${id}`);
  },
  createGroup(group) {
    return api.post('/Groups', group);
  },
  updateGroup(id, group) {
    return api.put(`/Groups/${id}`, group);
  },
  deleteGroup(id) {
    return api.delete(`/Groups/${id}`);
  }
};
