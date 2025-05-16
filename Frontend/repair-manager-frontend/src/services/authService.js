import api from './api';
import jwtService from './jwtService';
import store from '../store';

const authService = {
  /**
   * Login with the server-side authentication system
   * @param {Object} credentials - User credentials (username, password)
   * @returns {Promise<Object>} Login result
   */
  async login(credentials) {
    try {
      console.log('Attempting login with credentials:', credentials);
      const response = await api.post('/Auth/login', credentials);
      const { token, user } = response.data;
      
      // Ensure user object has consistent property names
      if (user) {
        // Normalize admin status
        if (user.isAdmin === undefined && user.IsAdmin !== undefined) {
          user.isAdmin = user.IsAdmin;
        }
        console.log('User object from API:', user);
      }
      
      // Store token in localStorage
      jwtService.saveToken(token);
      
      // Update store with user data
      store.commit('SET_AUTH', { user, isAuthenticated: true });
      
      return { success: true, data: response.data };
    } catch (error) {
      console.error('Login error:', error);
      return { success: false, error: error.response?.data?.message || 'Login failed' };
    }
  },
  
  /**
   * Register a new user
   * @param {Object} userData - User registration data
   * @returns {Promise<Object>} Registration result
   */
  async register(userData) {
    try {
      const response = await api.post('/Auth/register', userData);
      const { token, user } = response.data;
      
      // Store token in localStorage
      jwtService.saveToken(token);
      
      // Update store with user data
      store.commit('SET_AUTH', { user, isAuthenticated: true });
      
      return { success: true, data: response.data };
    } catch (error) {
      console.error('Registration error:', error);
      return { 
        success: false, 
        error: error.response?.data?.message || 'Registration failed',
        errors: error.response?.data?.errors
      };
    }
  },
  
  /**
   * Get current user information from the server
   * @returns {Promise<Object>} User data
   */
  async getCurrentUser() {
    try {
      // This endpoint would need to be implemented on the backend
      const response = await api.get('/Auth/me');
      return { success: true, data: response.data };
    } catch (error) {
      console.error('Error fetching current user:', error);
      return { success: false, error: 'Failed to fetch user data' };
    }
  },
  
  /**
   * Logout the current user
   */
  logout() {
    // Remove token from localStorage
    jwtService.removeToken();
    
    // Clear auth state in store
    store.commit('LOGOUT');
  },
  
  /**
   * Check if the user is authenticated
   * @returns {boolean} True if authenticated
   */
  checkAuth() {
    const token = jwtService.getToken();
    if (!token) {
      return false;
    }
    
    // Check if token is expired
    if (jwtService.isTokenExpired(token)) {
      this.logout();
      return false;
    }
    
    // If we have a valid token but no user in store, try to restore session
    if (!store.getters.currentUser) {
      // In a real app, we would fetch the user profile here
      // For now, just clear the invalid token
      this.logout();
      return false;
    }
    
    return true;
  },
  
  /**
   * Get the current authentication status
   * @returns {Object} Authentication status
   */
  getAuthStatus() {
    return {
      isAuthenticated: store.getters.isAuthenticated,
      user: store.getters.currentUser,
      token: jwtService.getToken()
    };
  }
};

export default authService;
