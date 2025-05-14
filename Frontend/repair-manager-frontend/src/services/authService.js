import api from './api';

export default {
  // Authentication
  login(credentials) {
    return api.post('/Auth/login', credentials);
  },
  
  // Get current user
  getCurrentUser() {
    // Note: The backend doesn't have a /me endpoint yet, so this will fallback to the demo data
    return api.get('/Auth/me');
  },
  
  // Logout
  logout() {
    localStorage.removeItem('auth_token');
    return Promise.resolve();
  },
  
  // Check if user is authenticated
  isAuthenticated() {
    return !!localStorage.getItem('auth_token');
  }
};
