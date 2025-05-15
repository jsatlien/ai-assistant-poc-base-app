<template>
  <div>
    <!-- Render the default slot (child components) if authenticated -->
    <slot v-if="isAuthenticated"></slot>
    <!-- Otherwise show login required message and redirect -->
    <div v-else class="auth-required">
      <h2>Authentication Required</h2>
      <p>You need to be logged in to access this page.</p>
      <p>Redirecting to login page...</p>
    </div>
  </div>
</template>

<script>
export default {
  name: 'AuthGuard',
  props: {
    requireAdmin: {
      type: Boolean,
      default: false
    }
  },
  data() {
    return {
      isAuthenticated: false
    };
  },
  created() {
    this.checkAuth();
  },
  methods: {
    checkAuth() {
      // Check if the user is authenticated
      const isAuth = this.$auth.isAuthenticated();
      
      // For admin routes, also check if the user is an admin
      if (this.requireAdmin) {
        this.isAuthenticated = isAuth && this.$auth.isAdmin();
      } else {
        this.isAuthenticated = isAuth;
      }
      
      // If not authenticated, redirect to login page after a short delay
      if (!this.isAuthenticated) {
        setTimeout(() => {
          this.$router.replace('/login');
        }, 1500);
      }
    }
  }
};
</script>

<style scoped>
.auth-required {
  text-align: center;
  margin: 2rem auto;
  max-width: 500px;
  padding: 2rem;
  background-color: #f8f9fa;
  border-radius: 8px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}
</style>
