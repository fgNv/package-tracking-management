import Vue from 'vue'
import VueResource from 'vue-resource'
Vue.use(VueResource)

export default class AuthenticationService {

  authenticate (data) {
    console.log(Vue.$http)
    return Vue.http
              .get('google.com')
              .then((r) => {
                console.log(r)
              })
              .catch((r) => {
                console.log('ruim deu')
              })
              .finally((r) => {
                console.log('fino')
              })
  }
}
