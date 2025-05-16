/**
 * JWT Authentication Service
 * Handles all JWT token operations including storage, retrieval, and validation
 * 
 * This service is designed to work with ASP.NET Core Identity JWT tokens
 * and supports secure token handling for server-side authentication
 */

// Token storage key in localStorage
const TOKEN_KEY = 'auth_token';

/**
 * Save the JWT token to localStorage
 * @param {string} token - JWT token
 */
const saveToken = (token) => {
  localStorage.setItem(TOKEN_KEY, token);
};

/**
 * Get the JWT token from localStorage
 * @returns {string|null} - JWT token or null if not found
 */
const getToken = () => {
  return localStorage.getItem(TOKEN_KEY);
};

/**
 * Get user information from the JWT token
 * @returns {object|null} - User data extracted from token or null if not found
 */
const getUserFromToken = () => {
  const token = getToken();
  if (!token) return null;
  
  const payload = parseToken(token);
  if (!payload) return null;
  
  // Extract user information from the token claims
  return {
    id: payload.userId || payload.sub,
    username: payload.username || payload.sub,
    email: payload.email,
    isAdmin: payload.isAdmin === 'True' || payload.isAdmin === true,
    role: payload.role,
    groupId: payload.groupId ? parseInt(payload.groupId) : null
  };
};

/**
 * Remove the JWT token from localStorage
 */
const removeToken = () => {
  localStorage.removeItem(TOKEN_KEY);
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
  getUserFromToken,
  removeToken,
  isAuthenticated,
  parseToken,
  isTokenExpired,
  getTokenRemainingTime
};
