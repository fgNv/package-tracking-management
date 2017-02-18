import Vue from 'vue'
import VueResource from 'vue-resource'
import VueLocalStorage from 'vue-localstorage'
import Moment from 'moment'

Vue.use(VueResource)
Vue.use(VueLocalStorage)

function getAccessData () {
  var accessData = JSON.parse(window.localStorage.getItem('access_data'))
  return accessData
}

export default {
  authenticate (data) {
    data['grant_type'] = 'password'

    return Vue.http
              .post('token', data, { emulateJSON: true })
              .then((r) => {
                var response = r.body
                var expiresAt = new Moment().add(response.expires_in, 'seconds')
                response.expiresAt = expiresAt
                console.log(response)
                return response
              })
              .catch((err) => {
                window.alert('error on authentication')
                throw err
              })
  },
  logout () {
    window.localStorage.removeItem('access_data')
  },
  getToken () {
    return getAccessData().access_token
  },
  getName () {
    return getAccessData().name
  },
  isLoggedIn () {
    var accessData = getAccessData()
    if (!accessData) {
      return false
    }

    var now = new Moment()
    var expired = now.isAfter(accessData.expiresAt)
    return !expired
  },
  accessType () {
    var accessData = getAccessData()
    if (!accessData) {
      return false
    }

    return accessData.accessType
  }
}
