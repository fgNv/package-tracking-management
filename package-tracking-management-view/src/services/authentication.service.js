import Vue from 'vue'
import VueResource from 'vue-resource'
Vue.use(VueResource)

export default class AuthenticationService {

  authenticate (data) {
    Vue.http.options.emulateJSON = true
    data['grant_type'] = 'password'
    return Vue.http
              .post('http://localhost:8090/' + 'token',
                    data)
              .then((r) => {
                console.log('success')
                return r
              })
              .catch((err) => {
                window.alert('error on authentication')
                throw err
              })
  }
}
