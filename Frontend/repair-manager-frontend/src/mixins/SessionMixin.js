/**
 * Session management mixin for the RepairManager application
 * This provides a simple way to check authentication status in components
 */
import jwtService from '../services/jwtService';

export default {
  computed: {
    isAuthenticated() {
      // Check if we have a valid token and the user is authenticated in the store
      const token = jwtService.getToken();
      const isTokenValid = token && !jwtService.isTokenExpired(token);
      return isTokenValid && this.$store.state.isAuthenticated;
    },
    
    isAdmin() {
      return this.isAuthenticated && this.$store.getters.isAdmin;
    },
    
    currentUser() {
      return this.$store.state.user || {};
    }
  },
  
  methods: {
    /**
     * Check if the current route requires authentication
     * @returns {boolean} - True if the current route requires authentication
     */
    routeRequiresAuth() {
      return this.$route.matched.some(record => record.meta.requiresAuth);
    },
    
    /**
     * Check if the current route requires admin permissions
     * @returns {boolean} - True if the current route requires admin permissions
     */
    routeRequiresAdmin() {
      return this.$route.matched.some(record => record.meta.requiresAdmin);
    },
    
    /**
     * Check if the user can access the current route
     * @returns {boolean} - True if the user can access the current route
     */
    canAccessCurrentRoute() {
      // If the route doesn't require authentication, allow access
      if (!this.routeRequiresAuth()) {
        return true;
      }
      
      // If the route requires authentication but the user is not authenticated, deny access
      if (!this.isAuthenticated) {
        return false;
      }
      
      // If the route requires admin permissions but the user is not an admin, deny access
      if (this.routeRequiresAdmin() && !this.isAdmin) {
        return false;
      }
      
      // Otherwise, allow access
      return true;
    },
    
    /**
     * Redirect to the login page
     */
    redirectToLogin() {
      this.$router.replace('/login');
    },
    
    /**
     * Redirect to the home page
     */
    redirectToHome() {
      this.$router.replace('/');
    }
  }
};
