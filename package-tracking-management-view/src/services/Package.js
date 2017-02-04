import Vue from 'vue'

export default {
  create (data) {
    return Vue.http
              .post('package', data)
              .then((r) => {
                return r.body
              })
              .catch((err) => {
                window.alert('Erro ao criar pacote')
                throw err
              })
  },
  query (page, itemsPerPage) {
    var data = {page, itemsPerPage}
    return Vue.http
              .get('package', data)
              .then((r) => {
                return r.body
              })
              .catch((err) => {
                window.alert('Erro ao carregar pacotes')
                throw err
              })
  }
}
