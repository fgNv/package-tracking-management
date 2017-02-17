import Vue from 'vue'

export default {
  getByPackage (packageId) {
    return Vue.http
              .get('package/' + packageId + '/permissions')
              .then((r) => {
                return r.body
              })
              .catch((err) => {
                throw err
              })
  },
  grant (userId, packageId) {
    var request = {userId, packageId}
    return Vue.http
              .post('permission', request)
              .then((r) => {
                return r.body
              })
              .catch((err) => {
                throw err
              })
  },
  revoke (userId, packageId) {
    var request = {userId, packageId}
    return Vue.http
              .delete('permission', request)
              .then((r) => {
                return r.body
              })
              .catch((err) => {
                throw err
              })
  }
}
