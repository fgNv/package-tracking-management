<template>
  <div class="ui main text container">
    <h1 class="ui header">
      Gerenciamento de usuários
      <router-link to="/user/create" tag="button"
                   class="ui secondary button right floated">
        Cadastrar usuário
      </router-link>
    </h1>
    <div class="ui info message" v-if="items.length == 0">
      <div class="header">
        Não há usuários cadastrados
      </div>
    </div>
    <table class="ui celled table" v-if="items.length > 0">
      <thead>
         <tr>
           <th>Nome</th>
           <th>Email</th>
           <th>Perfil de acesso</th>
           <th>Ações</th>
         </tr>
      </thead>
      <tbody>
       <tr v-for="(item, index) in items">
         <td>
          <div v-bind:class="{'ui ribbon label': index == 0}">
            {{item.name}}
          </div>
         </td>
         <td> {{item.email}} </td>
         <td> {{item.accessType.accessType}} </td>
         <td>
          <button class="ui secondary button" v-on:click="remove(item.id)"
                  v-tooltip="'Deletar usuário'">
            <i class="fa fa-trash"></i>
          </button>
          <router-link tag="button" class="ui secondary button"
                       :to="{name: 'user-edit', params: {id: item.id} }"
                       v-tooltip="'Editar usuário'">
            <i class="fa fa-edit"></i>
          </router-link>
         </td>
       </tr>
    </table>
  </div>
</template>

<script>
import toasterService from 'services/Toaster.js'
import UserService from 'services/User.js'
import $ from 'jquery'

export default {
  name: 'user-list',
  methods: {
    edit (id) {
      this.$router.push({ name: 'user-edit', parameters: {id} })
    },
    loadUsers () {
      $('.dimmer').dimmer('show')
      UserService.query()
                 .then(response => {
                   this.items = response.items
                   this.total = response.total
                 })
                 .finally(() => {
                   $('.dimmer').dimmer('hide')
                 })
    },
    remove (id) {
      if (!window.confirm('Tem certeza que deseja remover o usuário?')) {
        return
      }

      $('.dimmer').dimmer('show')
      UserService.remove(id)
                 .then(() => {
                   this.loadUsers()
                   toasterService.success('Usuário removido com sucesso')
                 })
                 .catch((err) => {
                   console.log('err on remove user', err)
                   toasterService.error('Erro ao remover usuário')
                 })
                 .finally(() => {
                   $('.dimmer').dimmer('hide')
                 })
    }
  },
  data () {
    return {
      currentPage: 1,
      items: [],
      total: 0
    }
  },
  mounted: function () {
    this.loadUsers()
  }
}
</script>
