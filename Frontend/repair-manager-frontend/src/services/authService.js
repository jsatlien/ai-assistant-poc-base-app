import api from './api';
import jwtService from './jwtService';

export default {
  /**
   * Authenticate user and save token
   * @param {Object} credentials - User credentials
   * @returns {Promise} - Promise with login response
   */
  async login(credentials) {
    try {
      const response = await api.post('/Auth/login', credentials);
      const { token, user } = response.data;
      
      console.log('Login response user object:', user);
      
      // Ensure user has isAdmin property set correctly
      // Use the IsAdmin property from the database
      if (user) {
        // If IsAdmin exists in the response, use it directly
        if (user.IsAdmin !== undefined) {
          user.isAdmin = !!user.IsAdmin; // Convert to boolean
          console.log('Setting isAdmin from IsAdmin:', user.isAdmin);
        } else {
          // Force admin status for the 'admin' user
          if (user.Username === 'admin' || user.username === 'admin') {
            user.isAdmin = true;
            console.log('Forcing isAdmin=true for admin user');
          }
        }
      }
      
      console.log('Final user object with admin status:', user);
      
      // Save token and user data
      jwtService.saveToken(token, user);
      
      return response;
    } catch (error) {
      // For demo purposes, provide a fallback if the API fails
      if (credentials.username === 'admin' && credentials.password === 'admin123') {
        const user = {
          id: 1,
          username: 'admin',
          fullName: 'System Administrator',
          email: 'admin@repairmanager.com',
          role: 'Administrator',
          isAdmin: true,
          groupId: credentials.groupId // Store the selected group ID
        };
        const token = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwibmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluaXN0cmF0b3IiLCJpc0FkbWluIjp0cnVlLCJleHAiOjE5MDAwMDAwMDB9.3fFSvgEGpJatjLGdA-N3gEfbXN9HtrtSt8OA8gTYXo8';
        
        // Save token and user data
        jwtService.saveToken(token, user);
        
        return {
          data: { token, user }
        };
      }
      
      throw error;
    }
  },
  
  /**
   * Get current user information
   * @returns {Promise} - Promise with user data
   */
  async getCurrentUser() {
    try {
      return await api.get('/Auth/me');
    } catch (error) {
      // If the API fails, try to use stored user data
      const user = jwtService.getUser();
      if (user) {
        return { data: user };
      }
      throw error;
    }
  },
  
  /**
   * Logout the current user
   * @returns {Promise} - Promise that resolves when logout is complete
   */
  logout() {
    // Remove token and user data
    jwtService.removeToken();
    return Promise.resolve();
  },
  
  /**
   * Check if the user is authenticated
   * @returns {boolean} - True if authenticated, false otherwise
   */
  isAuthenticated() {
    return jwtService.isAuthenticated();
  },
  
  /**
   * Check if the current token is valid
   * @returns {boolean} - True if token is valid, false otherwise
   */
  isTokenValid() {
    const token = jwtService.getToken();
    return token && !jwtService.isTokenExpired(token);
  }
};
