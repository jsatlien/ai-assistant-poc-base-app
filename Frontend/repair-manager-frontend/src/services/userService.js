import api from './api';

export default {
  // Users
  getUsers() {
    return api.get('/users');
  },
  getUser(id) {
    return api.get(`/users/${id}`);
  },
  createUser(user) {
    return api.post('/users', user);
  },
  updateUser(id, user) {
    return api.put(`/users/${id}`, user);
  },
  deleteUser(id) {
    return api.delete(`/users/${id}`);
  },
  
  // User Roles
  getUserRoles() {
    return api.get('/user-roles');
  },
  getUserRole(id) {
    return api.get(`/user-roles/${id}`);
  },
  createUserRole(role) {
    return api.post('/user-roles', role);
  },
  updateUserRole(id, role) {
    return api.put(`/user-roles/${id}`, role);
  },
  deleteUserRole(id) {
    return api.delete(`/user-roles/${id}`);
  }
};
