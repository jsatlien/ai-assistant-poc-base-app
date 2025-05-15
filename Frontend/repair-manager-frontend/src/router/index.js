import Vue from 'vue'
import VueRouter from 'vue-router'

Vue.use(VueRouter)

const routes = [
  {
    path: '/login',
    name: 'Login',
    component: () => import('../Pages/Login.vue'),
    meta: { requiresAuth: false }
  },
  {
    path: '/',
    name: 'Home',
    component: () => import('../Pages/HomePage.vue'),
    meta: { requiresAuth: true }
  },
  // Catalog routes
  {
    path: '/catalog/devices',
    name: 'Devices',
    component: () => import('../Pages/Catalog/Devices.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/catalog/service-categories',
    name: 'ServiceCategories',
    component: () => import('../Pages/Catalog/ServiceCategories.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/catalog/product-categories',
    name: 'ProductCategories',
    component: () => import('../Pages/Catalog/ProductCategories.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/catalog/manufacturers',
    name: 'Manufacturers',
    component: () => import('../Pages/Catalog/Manufacturers.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/catalog/services',
    name: 'Services',
    component: () => import('../Pages/Catalog/Services.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/catalog/parts',
    name: 'Parts',
    component: () => import('../Pages/Catalog/Parts.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/catalog/all',
    name: 'AllCatalogItems',
    component: () => import('../Pages/Catalog/AllItems.vue'),
    meta: { requiresAuth: true }
  },
  // Work Orders routes
  {
    path: '/workorders',
    name: 'WorkOrders',
    component: () => import('../Pages/WorkOrders/WorkOrders.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/workorders/:id',
    name: 'EditWorkOrder',
    component: () => import('../Pages/WorkOrders/Edit.vue'),
    meta: { requiresAuth: true }
  },
  // Workflows routes
  {
    path: '/workflows',
    name: 'Workflows',
    component: () => import('../Pages/Workflows/Index.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/workflows/new',
    name: 'NewWorkflow',
    component: () => import('../Pages/Workflows/New.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/workflows/statuses',
    name: 'WorkflowStatuses',
    component: () => import('../Pages/Workflows/Statuses.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/workflows/flows',
    name: 'WorkflowFlows',
    component: () => import('../Pages/Workflows/Flows.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/workflows/programs',
    name: 'WorkflowPrograms',
    component: () => import('../Pages/Workflows/Programs.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/workflows/:id',
    name: 'EditWorkflow',
    component: () => import('../Pages/Workflows/Edit.vue'),
    meta: { requiresAuth: true }
  },
  // Administration routes
  {
    path: '/admin/users',
    name: 'Users',
    component: () => import('../Pages/Administration/Users.vue'),
    meta: { requiresAuth: true, requiresAdmin: true }
  },
  {
    path: '/admin/user-roles',
    name: 'UserRoles',
    component: () => import('../Pages/Administration/UserRoles.vue'),
    meta: { requiresAuth: true, requiresAdmin: true }
  },
  {
    path: '/admin/groups',
    name: 'Groups',
    component: () => import('../Pages/Groups/Groups.vue'),
    meta: { requiresAuth: true, requiresAdmin: true }
  },
  {
    path: '/admin/catalog-pricing',
    name: 'CatalogPricing',
    component: () => import('../Pages/Administration/CatalogPricing.vue'),
    meta: { requiresAuth: true, requiresAdmin: true }
  },
  {
    path: '/inventory',
    name: 'Inventory',
    component: () => import('../Pages/Administration/Inventory.vue'),
    meta: { requiresAuth: true }
  },
  // Programs routes
  {
    path: '/programs',
    name: 'Programs',
    component: () => import('../Pages/Programs/Index.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/programs/new',
    name: 'NewProgram',
    component: () => import('../Pages/Programs/New.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/programs/:id',
    name: 'EditProgram',
    component: () => import('../Pages/Programs/Edit.vue'),
    meta: { requiresAuth: true }
  },
  // Catch-all redirect to home
  {
    path: '*',
    redirect: '/'
  }
]

const router = new VueRouter({
  mode: 'history',
  base: process.env.BASE_URL,
  routes
})

// No navigation guards - we'll handle authentication at the component level

export default router
