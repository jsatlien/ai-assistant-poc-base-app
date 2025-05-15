import api from './api';

/**
 * Service for managing parts in the RepairManager application
 */
export default {
  /**
   * Get all parts
   * @returns {Promise} - Promise with parts data
   */
  getParts() {
    return api.get('/catalog/parts');
  },
  
  /**
   * Get a part by ID
   * @param {number} id - Part ID
   * @returns {Promise} - Promise with part data
   */
  getPart(id) {
    return api.get(`/catalog/parts/${id}`);
  },
  
  /**
   * Create a new part
   * @param {Object} part - Part data
   * @returns {Promise} - Promise with created part data
   */
  createPart(part) {
    return api.post('/catalog/parts', part);
  },
  
  /**
   * Update a part
   * @param {number} id - Part ID
   * @param {Object} part - Part data
   * @returns {Promise} - Promise with updated part data
   */
  updatePart(id, part) {
    return api.put(`/catalog/parts/${id}`, part);
  },
  
  /**
   * Delete a part
   * @param {number} id - Part ID
   * @returns {Promise} - Promise that resolves when the part is deleted
   */
  deletePart(id) {
    return api.delete(`/catalog/parts/${id}`);
  }
};
