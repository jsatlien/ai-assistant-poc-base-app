import Vue from 'vue'
import App from './App.vue'
import router from './router'
import store from './store'
import auth from './auth'

Vue.config.productionTip = false

// Initialize authentication before mounting the app
auth.init()

// Create a global auth property
Vue.prototype.$auth = auth

new Vue({
  router,
  store,
  render: h => h(App)
}).$mount('#app')
