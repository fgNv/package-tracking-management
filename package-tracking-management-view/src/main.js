// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import Vue from 'vue'
import App from './App'
import VueRouter from 'vue-router'
import PackagesList from './components/PackagesList'
import Dashboard from './components/Dashboard'
import Login from './components/Login'
import VueLocalStorage from 'vue-localstorage'
import authenticationService from 'services/authentication.service.js'

require('semantic-ui-css/semantic.css')
require('semantic-ui-css/semantic.js')

Vue.use(VueRouter)
Vue.use(VueLocalStorage)

function requireAuth (to, from, next) {
  console.log('authenticationService.isLoggedIn()', authenticationService.isLoggedIn())
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
  { path: '/login', component: Login },
  { path: '/logout',
    beforeEnter (to, from, next) {
      authenticationService.logout()
      next('/')
    }
  }
]

const router = new VueRouter({
  mode: 'history',
  routes
})

/* eslint-disable no-new */
new Vue({
  el: '#app',
  router,
  template: '<App/>',
  components: { App }
})
