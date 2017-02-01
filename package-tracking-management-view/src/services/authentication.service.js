import Vue from 'vue'
import VueResource from 'vue-resource'
import VueLocalStorage from 'vue-localstorage'
import Moment from 'moment'

Vue.use(VueResource)
Vue.use(VueLocalStorage)

export default {
  authenticate (data) {
    Vue.http.options.emulateJSON = true
    data['grant_type'] = 'password'

    return Vue.http
              .post('http://localhost:8090/' + 'token', data)
              .then((r) => {
                return r
              })
              .catch((err) => {
                window.alert('error on authentication')
                throw err
              })
  },
  logout () {
    window.localStorage.removeItem('access_data')
  },
  isLoggedIn () {
    var accessData = JSON.parse(window.localStorage.getItem('access_data'))
    if (!accessData) {
      return false
    }

    var now = new Moment()
    var expired = now.isAfter(accessData.expiresAt)
    return !expired
  }
}
