import Vue from 'vue'

export default {
  get (id) {
    return Vue.http
              .get('package/' + id)
              .then((r) => {
                return r.body
              })
              .catch((err) => {
                window.alert('Erro ao obter pacote')
                console.log('err getting package -> ', err)
                throw err
              })
  },
  update (data) {
    return Vue.http
              .put('package', data, { emulateJSON: false })
              .then((r) => {
                return r.body
              })
              .catch((err) => {
                window.alert('Erro ao atualizar pacote')
                throw err
              })
  },
  create (data) {
    return Vue.http
              .post('package', data, { emulateJSON: false })
              .then((r) => {
                return r.body
              })
              .catch((err) => {
                window.alert('Erro ao criar pacote')
                throw err
              })
  },
  remove (id) {
    return Vue.http
              .delete('package/' + id)
              .then((r) => {
                return r.body
              })
              .catch((err) => {
                window.alert('Erro ao remover pacote')
                console.log('err removing package -> ', err)
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
