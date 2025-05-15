import axios from 'axios';
import apiConfig from '../config/api.config';
import jwtService from './jwtService';
import router from '../router';
import store from '../store';

// Create axios instance with base URL pointing to our backend API
// Configuration is loaded from the environment-specific config file
const api = axios.create({
  baseURL: apiConfig.baseURL,
  timeout: apiConfig.timeout,
  headers: {
    'Content-Type': 'application/json'
  }
});

// Add request interceptor to include auth token in requests
api.interceptors.request.use(
  config => {
    const token = jwtService.getToken();
    if (token) {
      // Check if token is expired
      if (jwtService.isTokenExpired(token)) {
        // Token is expired, log out the user
        store.dispatch('logout');
        return Promise.reject(new Error('Authentication token has expired'));
      }
      
      // Add token to headers
      config.headers['Authorization'] = `Bearer ${token}`;
    }
    return config;
  },
  error => {
    return Promise.reject(error);
  }
);

// Add response interceptor to handle common errors
api.interceptors.response.use(
  response => {
    return response;
  },
  error => {
    // Handle 401 Unauthorized errors by redirecting to login
    if (error.response && error.response.status === 401) {
      // Clear authentication data
      jwtService.removeToken();
      store.dispatch('logout');
      
      // Redirect to login page if not already there
      if (router.currentRoute.path !== '/login') {
        router.push('/login');
      }
    }
    return Promise.reject(error);
  }
);

export default api;
