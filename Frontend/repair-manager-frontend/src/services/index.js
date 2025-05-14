// Export all services from a single file for easier imports
import authService from './authService';
import catalogService from './catalogService';
import workOrderService from './workOrderService';
import groupService from './groupService';
import userService from './userService';
import workflowService from './workflowService';
import inventoryService from './inventoryService';

export {
  authService,
  catalogService,
  workOrderService,
  groupService,
  userService,
  workflowService,
  inventoryService
};
