import Vue from 'vue'

export default {
  update (data) {
    return Vue.http
              .put('user', data, { emulateJSON: false })
              .then((r) => {
                return r.body
              })
              .catch((err) => {
                window.alert('Erro ao atualizar usuário')
                throw err
              })
  },
  get (id) {
    return Vue.http
              .get('user/' + id)
              .then((r) => {
                return r.body
              })
              .catch((err) => {
                window.alert('Erro ao carregar usuário')
                throw err
              })
  },
  create (data) {
    return Vue.http
              .post('user', data, { emulateJSON: false })
              .then((r) => {
                return r.body
              })
              .catch((err) => {
                window.alert('Erro ao criar usuário')
                throw err
              })
  },
  remove (id) {
    return Vue.http
              .delete('user/' + id)
              .then((r) => {
                return r.body
              })
              .catch((err) => {
                window.alert('Erro ao remover usuário')
                console.log('err removing usuário -> ', err)
                throw err
              })
  },
  getUsers () {
    var page = 1
    var itemsPerPage = 100
    var accessTypeFilter = 'user'
    var data = {page, itemsPerPage, accessTypeFilter}
    return Vue.http
              .get('user', {params: data})
              .then((r) => {
                return r.body
              })
              .catch((err) => {
                window.alert('Erro ao carregar usuários')
                throw err
              })
  },
  query (page, itemsPerPage) {
    var data = {page, itemsPerPage}
    return Vue.http
              .get('user', data)
              .then((r) => {
                return r.body
              })
              .catch((err) => {
                window.alert('Erro ao carregar usuários')
                throw err
              })
  }
}
