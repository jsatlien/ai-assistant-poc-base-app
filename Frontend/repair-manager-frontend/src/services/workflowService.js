import api from './api';

export default {
  // Workflows
  getWorkflows() {
    return api.get('/workflows');
  },
  getWorkflow(id) {
    return api.get(`/workflows/${id}`);
  },
  createWorkflow(workflow) {
    return api.post('/workflows', workflow);
  },
  updateWorkflow(id, workflow) {
    return api.put(`/workflows/${id}`, workflow);
  },
  deleteWorkflow(id) {
    return api.delete(`/workflows/${id}`);
  },
  
  // Programs
  getPrograms() {
    return api.get('/programs');
  },
  getProgram(id) {
    return api.get(`/programs/${id}`);
  },
  createProgram(program) {
    return api.post('/programs', program);
  },
  updateProgram(id, program) {
    return api.put(`/programs/${id}`, program);
  },
  deleteProgram(id) {
    return api.delete(`/programs/${id}`);
  },
  
  // Status Codes
  getStatusCodes() {
    return api.get('/status-codes');
  },
  getStatusCode(id) {
    return api.get(`/status-codes/${id}`);
  },
  createStatusCode(statusCode) {
    return api.post('/status-codes', statusCode);
  },
  updateStatusCode(id, statusCode) {
    return api.put(`/status-codes/${id}`, statusCode);
  },
  deleteStatusCode(id) {
    return api.delete(`/status-codes/${id}`);
  }
};
