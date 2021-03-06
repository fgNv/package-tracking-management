// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import Vue from 'vue'
import App from './App'
import VueRouter from 'vue-router'
import PackagesList from './components/Package/PackagesList'
import PackageForm from './components/Package/PackageForm'
import PackageManualRoute from './components/Package/PackageManualRoute'
import UsersList from './components/User/UserList'
import UserForm from './components/User/UserForm'
import Dashboard from './components/Dashboard'
import Login from './components/Login'
import VueLocalStorage from 'vue-localstorage'
import authenticationService from 'services/Authentication.js'
import Permission from './components/User/Permissions'
import vueDateFilter from 'filters/VueDateFormat.js'
import VTooltip from 'v-tooltip'
import * as VueGoogleMaps from 'vue2-google-maps'
import sharedData from './services/SharedData.js'

import 'toastr/build/toastr.css'
import 'semantic-ui-css/semantic.css'
import 'semantic-ui-css/semantic.js'
import 'font-awesome/css/font-awesome.css'

Vue.use(VueRouter)
Vue.use(VueLocalStorage)
vueDateFilter.install(Vue)
Vue.use(VTooltip)
Vue.use(VueGoogleMaps, {
  load: {
    key: 'AIzaSyCrw3FzpYhbJq1F_Lgh8DS7Ur-NUbjXsmc'
  }
})

Vue.http.options.root = sharedData.apiBaseUrl
Vue.http.interceptors.push((request, next) => {
  if (authenticationService.isLoggedIn()) {
    request.headers.set('Authorization', 'bearer ' + authenticationService.getToken())
  }
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
  {
    path: '/package/manual-route/:id',
    props: true,
    component: PackageManualRoute,
    beforeEnter: requireAuth,
    name: 'package-manual-route'
  },
  { path: '/user/list', component: UsersList, beforeEnter: requireAuth },
  { path: '/user/create', component: UserForm, beforeEnter: requireAuth },
  { path: '/user/permission', component: Permission, beforeEnter: requireAuth },
  {
    path: '/user/edit/:id',
    component: UserForm,
    beforeEnter: requireAuth,
    props: true,
    name: 'user-edit'
  },
  { path: '/package/list', component: PackagesList, beforeEnter: requireAuth },
  { path: '/package/create', component: PackageForm, beforeEnter: requireAuth },
  {
    path: '/package/edit/:id',
    component: PackageForm,
    beforeEnter: requireAuth,
    props: true,
    name: 'package-edit'
  },
  { path: '/login', component: Login },
  { path: '/logout',
    beforeEnter (to, from, next) {
      authenticationService.logout()
      next('/login')
    }
  }
]

const router = new VueRouter({
  history: true,
  transitionOnLoad: true,
  routes
})

// load('AIzaSyCrw3FzpYhbJq1F_Lgh8DS7Ur-NUbjXsmc')

/* eslint-disable no-new */
new Vue({
  el: '#app',
  router,
  template: '<App/>',
  components: { App }
})
