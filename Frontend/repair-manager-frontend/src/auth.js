/**
 * Authentication utility for the RepairManager application
 * This provides a centralized place to handle all authentication logic
 * 
 * Updated to work with ASP.NET Core Identity and server-side authentication
 */

import store from './store';
import router from './router';
import jwtService from './services/jwtService';
// Commented out as it's currently unused
// import authService from './services/authService';

// Authentication state
const auth = {
  /**
   * Initialize authentication - call this when the app starts
   */
  init() {
    // Check if we have a valid token
    const token = jwtService.getToken();
    if (token && !jwtService.isTokenExpired(token)) {
      // We have a valid token, extract user data from token
      const userData = jwtService.getUserFromToken();
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
    
    // Set initial auth check to true
    store.commit('SET_INITIAL_AUTH_CHECK', true);
  },
  
  /**
   * Refresh the authentication state from the token
   */
  refreshAuth() {
    const token = jwtService.getToken();
    if (token && !jwtService.isTokenExpired(token)) {
      const userData = jwtService.getUserFromToken();
      if (userData) {
        store.commit('SET_AUTH', { user: userData, isAuthenticated: true });
        return true;
      }
    }
    return false;
  },
  
  /**
   * Get the remaining time before the token expires (in seconds)
   * @returns {number} - Seconds until expiry, 0 if expired or invalid
   */
  getTokenRemainingTime() {
    const token = jwtService.getToken();
    return jwtService.getTokenRemainingTime(token);
  },
  
  /**
   * Get the user data from the token
   * @returns {object|null} - User data or null if not authenticated
   */
  getUser() {
    if (!this.isAuthenticated()) {
      return null;
    }
    return store.getters.currentUser;
  },
  
  /**
   * Get the user's roles from the token
   * @returns {array} - Array of role names
   */
  getUserRoles() {
    const user = this.getUser();
    if (!user || !user.role) {
      return [];
    }
    return Array.isArray(user.role) ? user.role : [user.role];
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
   * @returns {Promise<Object>} - Promise that resolves with login result
   */
  async login(credentials) {
    try {
      // Dispatch the login action and return the result
      const result = await store.dispatch('login', credentials);
      if (result.success) {
        // Refresh the authentication state
        this.refreshAuth();
      }
      return result;
    } catch (error) {
      console.error('Login error:', error);
      return { success: false, error: 'Authentication failed' };
    }
  },
  
  /**
   * Register a new user
   * @param {Object} userData - User registration data
   * @returns {Promise<Object>} - Promise that resolves with registration result
   */
  async register(userData) {
    try {
      // Dispatch the register action and return the result
      const result = await store.dispatch('register', userData);
      if (result.success) {
        // Refresh the authentication state
        this.refreshAuth();
      }
      return result;
    } catch (error) {
      console.error('Registration error:', error);
      return { success: false, error: 'Registration failed' };
    }
  },
  
  /**
   * Logout the user
   */
  logout() {
    store.dispatch('logout');
    if (router.currentRoute.path !== '/login') {
      router.push('/login');
    }
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
