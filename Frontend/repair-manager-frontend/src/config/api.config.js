/**
 * API Configuration
 * This file contains environment-specific configuration for the API
 */

// Define environment-specific API URLs
const apiConfig = {
  development: {
    baseURL: 'http://localhost:5097/api',
    timeout: 10000
  },
  test: {
    baseURL: 'http://localhost:5097/api',
    timeout: 10000
  },
  production: {
    // In production, this would be your actual API URL
    baseURL: '/api', // Relative URL for same-origin deployment
    timeout: 30000
  }
};

// Determine current environment
// In a real Vue app, this would use process.env.NODE_ENV
const currentEnvironment = process.env.NODE_ENV || 'development';

// Export the configuration for the current environment
export default apiConfig[currentEnvironment];
