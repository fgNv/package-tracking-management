// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import Vue from 'vue'
import App from './App'
import VueRouter from 'vue-router'
import PackagesList from './components/Package/PackagesList'
import PackageForm from './components/Package/PackageForm'
import Dashboard from './components/Dashboard'
import Login from './components/Login'
import VueLocalStorage from 'vue-localstorage'
import authenticationService from 'services/Authentication.js'

require('semantic-ui-css/semantic.css')
require('semantic-ui-css/semantic.js')

Vue.use(VueRouter)
Vue.use(VueLocalStorage)

Vue.http.options.root = '--api-base-url--'
Vue.http.interceptors.push((request, next) => {
  if (authenticationService.isLoggedIn()) {
    request.headers.set('Authorization', 'bearer ' + authenticationService.getToken())
  }
  console.log('request', request)
  request.headers.set('Accept', 'application/json')
  next()
})

function requireAuth (to, from, next) {
  console.log('requireAuth -> isLoggedIn', authenticationService.isLoggedIn())
  if (!authenticationService.isLoggedIn()) {
    next({
      path: '/login',
      query: { redirect: to.fullPath }
    })
  } else {
    next()
  }
}

const routes = [
  { path: '/', component: Dashboard, beforeEnter: requireAuth },
  { path: '/package/list', component: PackagesList, beforeEnter: requireAuth },
  { path: '/package/create', component: PackageForm, beforeEnter: requireAuth },
  { path: '/login', component: Login },
  { path: '/logout',
    beforeEnter (to, from, next) {
      authenticationService.logout()
      next('/login')
    }
  }
]

const router = new VueRouter({
  routes
})

/* eslint-disable no-new */
new Vue({
  el: '#app',
  router,
  template: '<App/>',
  components: { App }
})
