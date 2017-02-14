import Vue from 'vue'

export default {
  create (request) {
    return Vue.http
              .post('package/point/manual', request)
              .then((r) => {
                return r.body
              })
              .catch((err) => {
                window.alert('Erro ao criar ponto manual')
                console.log('err getting package -> ', err)
                throw err
              })
  }
}
