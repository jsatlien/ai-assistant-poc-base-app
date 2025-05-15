import api from './api';

export default {
  // Devices
  getDevices() {
    return api.get('/Catalog/devices');
  },
  getDevice(id) {
    return api.get(`/Catalog/devices/${id}`);
  },
  createDevice(device) {
    return api.post('/Catalog/devices', device);
  },
  updateDevice(id, device) {
    return api.put(`/Catalog/devices/${id}`, device);
  },
  deleteDevice(id) {
    return api.delete(`/Catalog/devices/${id}`);
  },

  // Product Categories
  getProductCategories() {
    return api.get('/product-categories');
  },
  getProductCategory(id) {
    return api.get(`/product-categories/${id}`);
  },
  createProductCategory(category) {
    return api.post('/product-categories', category);
  },
  updateProductCategory(id, category) {
    return api.put(`/product-categories/${id}`, category);
  },
  deleteProductCategory(id) {
    return api.delete(`/product-categories/${id}`);
  },

  // Service Categories
  getServiceCategories() {
    return api.get('/service-categories');
  },
  getServiceCategory(id) {
    return api.get(`/service-categories/${id}`);
  },
  createServiceCategory(category) {
    return api.post('/service-categories', category);
  },
  updateServiceCategory(id, category) {
    return api.put(`/service-categories/${id}`, category);
  },
  deleteServiceCategory(id) {
    return api.delete(`/service-categories/${id}`);
  },

  // Manufacturers
  getManufacturers() {
    return api.get('/manufacturers');
  },
  getManufacturer(id) {
    return api.get(`/manufacturers/${id}`);
  },
  createManufacturer(manufacturer) {
    return api.post('/manufacturers', manufacturer);
  },
  updateManufacturer(id, manufacturer) {
    return api.put(`/manufacturers/${id}`, manufacturer);
  },
  deleteManufacturer(id) {
    return api.delete(`/manufacturers/${id}`);
  },

  // Services
  getServices() {
    return api.get('/Catalog/services');
  },
  getService(id) {
    return api.get(`/Catalog/services/${id}`);
  },
  createService(service) {
    return api.post('/Catalog/services', service);
  },
  updateService(id, service) {
    return api.put(`/Catalog/services/${id}`, service);
  },
  deleteService(id) {
    return api.delete(`/Catalog/services/${id}`);
  },

  // Parts
  getParts() {
    return api.get('/Catalog/parts');
  },
  getPart(id) {
    return api.get(`/Catalog/parts/${id}`);
  },
  createPart(part) {
    return api.post('/Catalog/parts', part);
  },
  updatePart(id, part) {
    return api.put(`/Catalog/parts/${id}`, part);
  },
  deletePart(id) {
    return api.delete(`/Catalog/parts/${id}`);
  },

  // Catalog Pricing
  getCatalogPricing() {
    return api.get('/catalog-pricing');
  },
  getCatalogPricingItem(id) {
    return api.get(`/catalog-pricing/${id}`);
  },
  createCatalogPricing(pricing) {
    return api.post('/catalog-pricing', pricing);
  },
  updateCatalogPricing(id, pricing) {
    return api.put(`/catalog-pricing/${id}`, pricing);
  },
  deleteCatalogPricing(id) {
    return api.delete(`/catalog-pricing/${id}`);
  }
};
