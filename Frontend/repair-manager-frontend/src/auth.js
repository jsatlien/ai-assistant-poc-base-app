/**
 * Authentication utility for the RepairManager application
 * This provides a centralized place to handle all authentication logic
 */

import store from './store';
import router from './router';
import jwtService from './services/jwtService';

// Authentication state
const auth = {
  /**
   * Initialize authentication - call this when the app starts
   */
  init() {
    // Check if we have a valid token
    const token = jwtService.getToken();
    if (token && !jwtService.isTokenExpired(token)) {
      // We have a valid token, get the user data
      const userData = jwtService.getUser();
      if (userData) {
        // Set the authentication state in the store
        store.commit('SET_AUTH', { user: userData, isAuthenticated: true });
        
        // Load groups if needed
        if (userData.groupId) {
          store.dispatch('fetchGroups').then(() => {
            const group = store.getters.getGroupById(userData.groupId);
            if (group) {
              store.dispatch('setCurrentGroup', group);
            }
          });
        }
      }
    } else if (token) {
      // Token exists but is expired, log out
      this.logout();
    }
  },
  
  /**
   * Check if the user is authenticated
   * @returns {boolean} - True if authenticated, false otherwise
   */
  isAuthenticated() {
    const token = jwtService.getToken();
    return token && !jwtService.isTokenExpired(token) && store.state.isAuthenticated;
  },
  
  /**
   * Check if the user is an admin
   * @returns {boolean} - True if admin, false otherwise
   */
  isAdmin() {
    return this.isAuthenticated() && store.getters.isAdmin;
  },
  
  /**
   * Login the user
   * @param {Object} credentials - User credentials
   * @returns {Promise<boolean>} - Promise that resolves to true if login was successful
   */
  async login(credentials) {
    try {
      // Just dispatch the login action without handling navigation
      return await store.dispatch('login', credentials);
    } catch (error) {
      console.error('Login error:', error);
      return false;
    }
  },
  
  /**
   * Logout the user
   */
  async logout() {
    await store.dispatch('logout');
    router.push('/login');
  },
  
  /**
   * Check if the user can access a route
   * @param {Object} route - Route object
   * @returns {boolean} - True if the user can access the route, false otherwise
   */
  canAccess(route) {
    // Check if the route requires authentication
    const requiresAuth = route.meta && route.meta.requiresAuth;
    if (!requiresAuth) {
      return true;
    }
    
    // Check if the user is authenticated
    if (!this.isAuthenticated()) {
      return false;
    }
    
    // Check if the route requires admin permissions
    const requiresAdmin = route.meta && route.meta.requiresAdmin;
    if (requiresAdmin && !this.isAdmin()) {
      return false;
    }
    
    return true;
  }
};

export default auth;
