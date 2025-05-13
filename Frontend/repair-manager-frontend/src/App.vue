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
        <div class="dropdown">
          <button class="nav-link dropdown-toggle">Repair Workflows</button>
          <div class="dropdown-menu">
            <router-link to="/workflows/statuses" class="dropdown-item">Status Codes</router-link>
            <router-link to="/workflows/flows" class="dropdown-item">Workflows</router-link>
            <router-link to="/workflows/programs" class="dropdown-item">Programs</router-link>
          </div>
        </div>
      </nav>
      <div class="user-menu">
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
    </header>
    <main class="app-content">
      <router-view />
    </main>
  </div>
</template>

<script>
export default {
  name: 'App',
  computed: {
    isLoginPage() {
      return this.$route && this.$route.path === '/login';
    }
  },
  methods: {
    logout() {
      // Clear authentication data
      localStorage.removeItem('auth_token');
      
      // Reset authentication state in store
      this.$store.commit('LOGOUT');
      
      // Redirect to login page
      this.$router.push('/login');
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
</style>
