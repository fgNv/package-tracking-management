import Vue from 'vue'

export default {
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
