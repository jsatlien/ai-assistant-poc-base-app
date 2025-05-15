/**
 * JWT Authentication Service
 * Handles all JWT token operations including storage, retrieval, and validation
 */

// Token storage key in localStorage
const TOKEN_KEY = 'auth_token';
const USER_KEY = 'auth_user';

/**
 * Save the JWT token and user data to localStorage
 * @param {string} token - JWT token
 * @param {object} user - User data object
 */
const saveToken = (token, user) => {
  localStorage.setItem(TOKEN_KEY, token);
  localStorage.setItem(USER_KEY, JSON.stringify(user));
};

/**
 * Get the JWT token from localStorage
 * @returns {string|null} - JWT token or null if not found
 */
const getToken = () => {
  return localStorage.getItem(TOKEN_KEY);
};

/**
 * Get the user data from localStorage
 * @returns {object|null} - User data object or null if not found
 */
const getUser = () => {
  const userData = localStorage.getItem(USER_KEY);
  if (userData) {
    try {
      return JSON.parse(userData);
    } catch (e) {
      console.error('Error parsing user data from localStorage:', e);
      return null;
    }
  }
  return null;
};

/**
 * Remove the JWT token and user data from localStorage
 */
const removeToken = () => {
  localStorage.removeItem(TOKEN_KEY);
  localStorage.removeItem(USER_KEY);
};

/**
 * Check if the user is authenticated (has a valid token)
 * @returns {boolean} - True if authenticated, false otherwise
 */
const isAuthenticated = () => {
  const token = getToken();
  return !!token; // Convert to boolean
};

/**
 * Parse the JWT token to get the payload
 * @param {string} token - JWT token
 * @returns {object|null} - Decoded payload or null if invalid
 */
const parseToken = (token) => {
  if (!token) return null;
  
  try {
    // Split the token into parts
    const parts = token.split('.');
    if (parts.length !== 3) return null;
    
    // Decode the payload (middle part)
    const payload = atob(parts[1].replace(/-/g, '+').replace(/_/g, '/'));
    return JSON.parse(payload);
  } catch (e) {
    console.error('Error parsing JWT token:', e);
    return null;
  }
};

/**
 * Check if the token is expired
 * @param {string} token - JWT token
 * @returns {boolean} - True if expired, false otherwise
 */
const isTokenExpired = (token) => {
  const payload = parseToken(token);
  if (!payload || !payload.exp) return true;
  
  // Convert exp to milliseconds and compare with current time
  const expiry = payload.exp * 1000; // Convert seconds to milliseconds
  return Date.now() >= expiry;
};

/**
 * Get the remaining time before the token expires (in seconds)
 * @param {string} token - JWT token
 * @returns {number} - Seconds until expiry, 0 if expired or invalid
 */
const getTokenRemainingTime = (token) => {
  const payload = parseToken(token);
  if (!payload || !payload.exp) return 0;
  
  const expiry = payload.exp * 1000; // Convert seconds to milliseconds
  const remaining = expiry - Date.now();
  return remaining > 0 ? Math.floor(remaining / 1000) : 0;
};

export default {
  saveToken,
  getToken,
  getUser,
  removeToken,
  isAuthenticated,
  parseToken,
  isTokenExpired,
  getTokenRemainingTime
};
