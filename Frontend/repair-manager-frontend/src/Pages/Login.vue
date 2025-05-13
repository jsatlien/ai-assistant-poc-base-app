<template>
  <div class="login-page">
    <div class="login-container">
      <div class="logo-container">
        <div class="logo-wrapper">
          <img src="../assets/logo.png" alt="ServiceManager Lite Logo" class="logo">
        </div>
      </div>
      
      <div class="card login-card">
        <h2 class="login-title">Sign In</h2>
        
        <div v-if="error" class="error-message">
          {{ error }}
        </div>
        
        <form @submit.prevent="handleLogin" class="login-form">
          <div class="form-group">
            <label for="username">Username</label>
            <input 
              type="text" 
              id="username" 
              v-model="username" 
              placeholder="Enter your username"
              required
              class="form-control"
            />
          </div>
          
          <div class="form-group">
            <label for="password">Password</label>
            <input 
              type="password" 
              id="password" 
              v-model="password" 
              placeholder="Enter your password"
              required
              class="form-control"
            />
          </div>

          <div class="form-group">
            <label for="group">Select Group</label>
            <select 
              id="group" 
              v-model="selectedGroupId" 
              class="form-control"
              required
            >
              <option value="" disabled>Select a group</option>
              <option v-for="group in groups" :key="group.id" :value="group.id">
                {{ group.code }} - {{ group.description }}
              </option>
            </select>
          </div>
          
          <div class="form-actions">
            <button type="submit" class="btn-primary" :disabled="isLoading || !selectedGroupId">
              {{ isLoading ? 'Signing in...' : 'Sign In' }}
            </button>
          </div>
          
          <div class="login-help">
            <p>For demo purposes, use:</p>
            <p>Username: <strong>admin</strong></p>
            <p>Password: <strong>admin123</strong></p>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'LoginPage',
  data() {
    return {
      username: '',
      password: '',
      selectedGroupId: '',
      error: null,
      isLoading: false
    };
  },
  computed: {
    groups() {
      return this.$store.getters.getGroups;
    }
  },

  methods: {
    async handleLogin() {
      this.isLoading = true;
      this.error = null;
      
      try {
        // For demo purposes, we'll bypass the actual API call
        // and simulate a successful login
        console.log('Login attempt with:', this.username, this.password);
        
        // Check if credentials match our demo account
        if (this.username === 'admin' && this.password === 'admin123') {
          // Simulate successful login
          const user = {
            username: 'admin',
            displayName: 'Admin User',
            role: 'Administrator',
            isAdmin: true
          };
          
          // Store token in localStorage (simulated token)
          const token = 'demo-jwt-token-' + Date.now();
          localStorage.setItem('auth_token', token);
          
          // Update Vuex store
          this.$store.commit('SET_AUTH', { user, isAuthenticated: true });
          
          // Set the selected group
          const selectedGroup = this.groups.find(g => g.id === this.selectedGroupId);
          if (selectedGroup) {
            this.$store.dispatch('setCurrentGroup', selectedGroup);
          }
          
          // Redirect to home page after successful login
          this.$router.push('/');
        } else {
          this.error = 'Invalid username or password';
        }
      } catch (error) {
        console.error('Login error:', error);
        this.error = 'Failed to sign in. Please check your credentials.';
      } finally {
        this.isLoading = false;
      }
    }
  },
  created() {
    // Check if user is already logged in
    if (this.$store.state.isAuthenticated) {
      this.$router.push('/');
    }
    
    // Fetch groups for the dropdown
    this.$store.dispatch('fetchGroups');
  }
};
</script>

<style scoped>
.login-page {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: #f5f5f5;
  padding: 2rem;
}

.login-container {
  width: 100%;
  max-width: 400px;
}

.logo-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  margin-bottom: 1.5rem;
}

.logo-wrapper {
  display: flex;
  justify-content: center;
  margin-bottom: 1rem;
}

.logo {
  max-height: 120px;
  max-width: 100%;
  width: auto;
  height: auto;
  object-fit: contain;
  margin-bottom: 1rem;
}

.login-card {
  background-color: #fff;
  border-radius: 12px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
  padding: 2rem;
}

.login-title {
  font-size: 1.5rem;
  font-weight: 600;
  color: #333;
  margin-top: 0;
  margin-bottom: 1.5rem;
  text-align: center;
}

.error-message {
  background-color: #f8d7da;
  color: #721c24;
  padding: 0.75rem;
  border-radius: 8px;
  margin-bottom: 1.5rem;
  font-size: 0.875rem;
}

.login-form {
  display: flex;
  flex-direction: column;
}

.form-group {
  margin-bottom: 1.5rem;
}

.form-group label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 500;
  color: #333;
}

.form-control {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  font-size: 1rem;
  transition: border-color 0.2s ease, box-shadow 0.2s ease;
}

.form-control:focus {
  outline: none;
  border-color: #08c;
  box-shadow: 0 0 0 2px rgba(0, 136, 204, 0.2);
}

.form-actions {
  margin-top: 1.5rem;
}

.btn-primary {
  width: 100%;
  background-color: #08c;
  color: #fff;
  border: none;
  border-radius: 8px;
  padding: 0.75rem 1.5rem;
  font-weight: 500;
  cursor: pointer;
  transition: background-color 0.2s ease;
}

.btn-primary:hover:not(:disabled) {
  background-color: #0077b3;
}

.btn-primary:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}

.login-help {
  margin-top: 2rem;
  text-align: center;
  color: #666;
  font-size: 0.875rem;
  padding-top: 1rem;
  border-top: 1px solid #e0e0e0;
}

.login-help p {
  margin: 0.25rem 0;
}
</style>
