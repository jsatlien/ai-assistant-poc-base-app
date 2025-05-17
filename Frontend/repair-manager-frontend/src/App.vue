<template>
  <div id="app">
    <header v-if="!isLoginPage" class="app-header">
      <div class="logo-container">
        <img src="./assets/header-logo.png" alt="ServiceManager Lite Logo" class="logo">
      </div>
      <nav class="main-nav">
        <router-link to="/" class="nav-link">Dashboard</router-link>
        <div class="dropdown">
          <button class="nav-link dropdown-toggle">Repair Catalog</button>
          <div class="dropdown-menu">
            <router-link to="/catalog/devices" class="dropdown-item">Devices</router-link>
            <router-link to="/catalog/services" class="dropdown-item">Services</router-link>
            <router-link to="/catalog/parts" class="dropdown-item">Parts</router-link>
            <router-link to="/catalog/all" class="dropdown-item">All Items</router-link>
          </div>
        </div>
        <div class="dropdown">
          <button class="nav-link dropdown-toggle">Master Data</button>
          <div class="dropdown-menu">
            <router-link to="/catalog/service-categories" class="dropdown-item">Service Categories</router-link>
            <router-link to="/catalog/product-categories" class="dropdown-item">Product Categories</router-link>
            <router-link to="/catalog/manufacturers" class="dropdown-item">Manufacturers</router-link>
          </div>
        </div>
        <router-link to="/workorders" class="nav-link">Work Orders</router-link>
        <router-link to="/inventory" class="nav-link">Inventory</router-link>
        <div class="dropdown">
          <button class="nav-link dropdown-toggle">Repair Workflows</button>
          <div class="dropdown-menu">
            <router-link to="/workflows/statuses" class="dropdown-item">Status Codes</router-link>
            <router-link to="/workflows/flows" class="dropdown-item">Workflows</router-link>
            <router-link to="/workflows/programs" class="dropdown-item">Programs</router-link>
          </div>
        </div>
        <div class="dropdown" v-if="isAdmin">
          <button class="nav-link dropdown-toggle">Administration</button>
          <div class="dropdown-menu">
            <router-link to="/admin/users" class="dropdown-item">Users</router-link>
            <router-link to="/admin/user-roles" class="dropdown-item">User Roles</router-link>
            <router-link to="/admin/groups" class="dropdown-item">Groups</router-link>
            <router-link to="/admin/catalog-pricing" class="dropdown-item">Catalog Pricing</router-link>
          </div>
        </div>
      </nav>
      <div class="user-menu">
        <div v-if="currentGroup" class="current-group-display" @click="showGroupSelector = true">
          <span class="current-group-label">Group:</span>
          <span class="current-group-value">{{ currentGroup.code }}</span>
          <span class="change-group-icon">🔄</span>
        </div>
        <div class="dropdown">
          <button class="user-button dropdown-toggle">
            <span class="user-name">Admin User</span>
            <span class="user-icon">👤</span>
          </button>
          <div class="dropdown-menu user-dropdown">
            <button class="dropdown-item logout-button" @click="logout">
              <span class="logout-icon">🚪</span> Logout
            </button>
          </div>
        </div>
      </div>
      
      <!-- Group Selector Modal -->
      <div v-if="showGroupSelector" class="modal group-selector-modal">
        <div class="modal-content">
          <div class="modal-header">
            <h2>Select Group</h2>
            <button class="close-btn" @click="showGroupSelector = false">&times;</button>
          </div>
          <div class="modal-body">
            <div v-if="groups.length === 0" class="empty-state">
              <p>No groups available. Please create a group first.</p>
            </div>
            <div v-else class="group-list">
              <div 
                v-for="group in groups" 
                :key="group.id" 
                class="group-item"
                :class="{ 'active': currentGroup && currentGroup.id === group.id }"
                @click="selectGroup(group)"
              >
                <div class="group-code">{{ group.code }}</div>
                <div class="group-description">{{ group.description }}</div>
                <div class="group-location">{{ group.city }}, {{ group.state }}</div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </header>
    <main class="app-content">
      <router-view />
    </main>
    <!-- Onboarding Assistant Widget -->
    <AssistantWidget 
      api-base-url="http://localhost:5197/api"
      assistant-name="Walli"
      application-name="ServiceManager Flex"
    />
  </div>
</template>

<script>
// Import the local AssistantWidget component
import AssistantWidget from './lib/onboarding-assistant/AssistantWidget.vue';
import Shepherd from 'shepherd.js';
import './lib/onboarding-assistant/shepherd-custom.css';

// Make Shepherd globally available for the AssistantWidget
window.Shepherd = Shepherd;

export default {
  name: 'App',
  components: {
    AssistantWidget
  },
  data() {
    return {
      showGroupSelector: false
    }
  },
  computed: {
    isLoginPage() {
      return this.$route && this.$route.path === '/login';
    },
    currentGroup() {
      return this.$store.getters.getCurrentGroup;
    },
    groups() {
      return this.$store.getters.getGroups;
    },
    isAdmin() {
      // Direct approach to determine admin status
      // This will show the admin menu for the admin user regardless of store state
      const user = this.$store.getters.currentUser;
      console.log('App.vue checking admin status, user:', user);
      
      // If we have a user object in the store, use that
      if (user) {
        const isAdmin = user.isAdmin || user.IsAdmin;
        console.log('User found in store, isAdmin:', isAdmin);
        return isAdmin;
      }
      
      // Fallback: Check if we're logged in as admin from localStorage
      try {
        const storedUser = JSON.parse(localStorage.getItem('auth_user'));
        console.log('Checking localStorage user:', storedUser);
        
        if (storedUser) {
          // If username is admin, consider them an admin
          const isAdmin = storedUser.isAdmin || 
                          storedUser.IsAdmin || 
                          storedUser.username === 'admin' || 
                          storedUser.Username === 'admin';
          console.log('Using localStorage user, isAdmin:', isAdmin);
          return isAdmin;
        }
      } catch (e) {
        console.error('Error parsing user from localStorage:', e);
      }
      
      // Final fallback: Check if we're on a development environment and logged in
      const isLoggedIn = this.$store.getters.isAuthenticated;
      if (isLoggedIn && process.env.NODE_ENV === 'development') {
        console.log('Development environment, logged in, assuming admin');
        return true;
      }
      
      return false;
    }
  },
  created() {
    // Fetch groups when app is created
    if (this.$store.state.isAuthenticated) {
      this.$store.dispatch('fetchGroups');
    }
  },
  methods: {
    logout() {
      // Clear authentication data
      localStorage.removeItem('auth_token');
      localStorage.removeItem('currentGroup');
      
      // Reset authentication state in store
      this.$store.commit('LOGOUT');
      
      // Redirect to login page
      this.$router.push('/login');
    },
    selectGroup(group) {
      this.$store.dispatch('setCurrentGroup', group);
      this.showGroupSelector = false;
    }
  }
}
</script>

<style>
:root {
  --primary-color: #08c;
  --light-gray: #f5f5f5;
  --medium-gray: #e0e0e0;
  --dark-gray: #333;
  --white: #fff;
}

* {
  box-sizing: border-box;
  margin: 0;
  padding: 0;
}

.logo-container {
  display: flex;
  align-items: center;
}

.logo {
  height: 40px;
  width: auto;
}

body {
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif;
  color: var(--dark-gray);
  background-color: var(--light-gray);
  margin: 0;
  padding: 0;
}

.app-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0.5rem 1.5rem;
  background-color: var(--white);
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  height: 60px;
}

.main-nav {
  display: flex;
  gap: 1.5rem;
}

.nav-link {
  text-decoration: none;
  color: var(--dark-gray);
  font-weight: 500;
  padding: 0.5rem 0;
  border-bottom: 2px solid transparent;
  transition: all 0.2s ease;
}

.nav-link:hover, .router-link-active {
  color: var(--primary-color);
  border-bottom-color: var(--primary-color);
}

/* Dropdown Styles */
.dropdown {
  position: relative;
  display: inline-block;
}

.dropdown-toggle {
  background: none;
  border: none;
  font-size: 1rem;
  cursor: pointer;
  color: var(--dark-gray);
  padding: 0.5rem 0;
  font-weight: 500;
  border-bottom: 2px solid transparent;
  transition: all 0.2s ease;
  font-family: inherit;
}

.dropdown-toggle:hover {
  color: var(--primary-color);
  border-bottom-color: var(--primary-color);
}

.dropdown-menu {
  display: none;
  position: absolute;
  background-color: var(--white);
  min-width: 160px;
  box-shadow: 0 8px 16px rgba(0, 0, 0, 0.1);
  z-index: 1;
  border-radius: 4px;
  padding: 0.5rem 0;
  margin-top: 0;
  top: 100%;
}

.dropdown:hover .dropdown-menu {
  display: block;
}

.dropdown-item {
  color: var(--dark-gray);
  padding: 0.75rem 1rem;
  text-decoration: none;
  display: block;
  font-weight: 400;
}

.dropdown-item:hover {
  background-color: var(--light-gray);
  color: var(--primary-color);
}

.user-dropdown {
  right: 0;
  left: auto;
  min-width: 180px;
}

.logout-button {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: #dc3545;
}

.logout-button:hover {
  background-color: #ffebee;
  color: #dc3545;
}

.logout-icon {
  font-size: 1.1rem;
}

.user-menu {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.user-button {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  background: none;
  border: none;
  cursor: pointer;
  padding: 0.5rem;
  border-radius: 4px;
  transition: background-color 0.2s ease;
  color: var(--dark-gray);
}

.user-button:hover {
  background-color: var(--light-gray);
}

.app-content {
  padding: 1.5rem;
  line-height: 1.6;
}

#app {
  display: flex;
  flex-direction: column;
  min-height: 100vh;
}

.app-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 2rem;
  height: 70px;
  background-color: var(--white);
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.main-nav {
  display: flex;
  gap: 1.5rem;
}

.nav-link {
  color: var(--dark-gray);
  text-decoration: none;
  font-weight: 500;
  padding: 0.5rem 0;
  border-bottom: 2px solid transparent;
  transition: all 0.2s ease;
}

.nav-link:hover, .nav-link.router-link-active {
  color: var(--primary-color);
  border-bottom-color: var(--primary-color);
}

.user-menu {
  position: relative;
}

.user-button {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  background: none;
  border: none;
  cursor: pointer;
  padding: 0.5rem;
  border-radius: 4px;
}

.user-button:hover {
  background-color: var(--light-gray);
}

.user-name {
  font-weight: 500;
}

.user-icon {
  font-size: 1.2rem;
}

.app-content {
  flex: 1;
  padding: 2rem;
  max-width: 1200px;
  width: 100%;
  margin: 0 auto;
}

/* Airbnb-inspired styles */
button, .btn {
  background-color: var(--primary-color);
  color: var(--white);
  border: none;
  border-radius: 8px;
  padding: 0.75rem 1.5rem;
  font-weight: 500;
  cursor: pointer;
  transition: background-color 0.2s ease;
}

button:hover, .btn:hover {
  background-color: #0077b3;
}

input, select, textarea {
  border: 1px solid var(--medium-gray);
  border-radius: 8px;
  padding: 0.75rem;
  width: 100%;
  font-size: 1rem;
}

input:focus, select:focus, textarea:focus {
  outline: none;
  border-color: var(--primary-color);
  box-shadow: 0 0 0 2px rgba(0, 136, 204, 0.2);
}

.card {
  background-color: var(--white);
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  padding: 1.5rem;
  margin-bottom: 1.5rem;
}

.card-title {
  font-size: 1.25rem;
  font-weight: 600;
  margin-bottom: 1rem;
  color: var(--dark-gray);
}

/* Group Selector Styles */
.current-group-display {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  background-color: var(--light-gray);
  padding: 0.5rem 0.75rem;
  border-radius: 4px;
  cursor: pointer;
  transition: background-color 0.2s ease;
}

.current-group-display:hover {
  background-color: var(--medium-gray);
}

.current-group-label {
  font-size: 0.8rem;
  color: #666;
}

.current-group-value {
  font-weight: 600;
  color: var(--primary-color);
}

.change-group-icon {
  font-size: 0.9rem;
  color: #666;
}

.group-selector-modal .modal-content {
  width: 500px;
  max-width: 90%;
}

.group-list {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  max-height: 400px;
  overflow-y: auto;
}

.group-item {
  padding: 1rem;
  border-radius: 8px;
  border: 1px solid var(--medium-gray);
  cursor: pointer;
  transition: all 0.2s ease;
}

.group-item:hover {
  border-color: var(--primary-color);
  background-color: rgba(0, 136, 204, 0.05);
}

.group-item.active {
  border-color: var(--primary-color);
  background-color: rgba(0, 136, 204, 0.1);
}

.group-code {
  font-weight: 600;
  color: var(--primary-color);
  margin-bottom: 0.25rem;
}

.group-description {
  font-weight: 500;
  margin-bottom: 0.25rem;
}

.group-location {
  font-size: 0.9rem;
  color: #666;
}

.modal {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-content {
  background-color: var(--white);
  border-radius: 8px;
  width: 500px;
  max-width: 90%;
  max-height: 90vh;
  overflow-y: auto;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.15);
}

.modal-header {
  padding: 1rem 1.5rem;
  border-bottom: 1px solid var(--medium-gray);
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.modal-header h2 {
  margin: 0;
  font-size: 1.25rem;
  color: var(--dark-gray);
}

.close-btn {
  background: none;
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
  color: #999;
  padding: 0;
}

.close-btn:hover {
  color: var(--dark-gray);
}

.modal-body {
  padding: 1.5rem;
}

.empty-state {
  text-align: center;
  padding: 2rem 0;
  color: #666;
}
</style>
