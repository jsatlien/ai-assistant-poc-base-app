import Vue from 'vue'
import App from './App.vue'
import router from './router'
import store from './store'
import auth from './auth'
// Onboarding Assistant is now imported directly in App.vue
// import OnboardingAssistant from 'onboarding-assistant-sdk'


Vue.config.productionTip = false

// Initialize authentication before mounting the app
auth.init()

// Create a global auth property
Vue.prototype.$auth = auth

// Onboarding Assistant is now registered as a component in App.vue
// Vue.use(OnboardingAssistant)

new Vue({
  router,
  store,
  render: h => h(App)
}).$mount('#app')
