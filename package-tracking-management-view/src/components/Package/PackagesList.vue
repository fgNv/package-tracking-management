<template>
  <div class="ui main text container">
    <h1 class="ui header">
      Gerenciamento de pacotes
      <router-link to="/package/create" tag="button"
                   v-if="accessType == 'administrator'"
                   class="ui secondary button right floated">
        Cadastrar pacote
      </router-link>
    </h1>
    <div class="ui info message" v-if="items.length == 0">
      <div class="header">
        Não há pacotes cadastrados
      </div>
    </div>
    <table class="ui celled table" v-if="items.length > 0">
      <thead>
         <tr>
           <th>Nome do pacote</th>
           <th>Última alteração</th>
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
         <td> {{item.updatedAt | moment}} </td>
         <td>
          <button class="ui secondary button" v-on:click="remove(item.id)"
                  v-tooltip="'Deletar pacote'"
                  v-if="accessType == 'administrator'">
            <i class="fa fa-trash"></i>
          </button>
          <router-link tag="button" class="ui secondary button"
                       :to="{name: 'package-edit', params: {id: item.id} }"
                       v-tooltip="'Editar pacote'"
                       v-if="accessType == 'administrator'">
            <i class="fa fa-edit"></i>
          </router-link>
          <router-link tag="button" class="ui secondary button"
                       :to="{name: 'package-manual-route', params: {id: item.id} }"
                       v-tooltip="'Atualizar rota manual do pacote'">
            <i class="fa fa-map"></i>
          </router-link>
         </td>
       </tr>
    </table>
  </div>
</template>

<script>
import toasterService from 'services/Toaster.js'
import PackageService from 'services/Package.js'
import $ from 'jquery'
import authenticationService from 'services/Authentication.js'

export default {
  name: 'package-list',
  localStorage: {
    access_data: {
      type: Object
    }
  },
  methods: {
    edit (id) {
      this.$router.push({ name: 'package-edit', parameters: {id} })
    },
    loadPackages () {
      $('.dimmer').dimmer('show')
      PackageService.query()
                    .then(response => {
                      this.items = response.items
                      this.total = response.total
                    })
                    .finally(() => {
                      $('.dimmer').dimmer('hide')
                    })
    },
    remove (id) {
      if (!window.confirm('Tem certeza que deseja remover o pacote?')) {
        return
      }

      $('.dimmer').dimmer('show')
      PackageService.remove(id)
                    .then(() => {
                      this.loadPackages()
                      toasterService.success('Pacote removido com sucesso')
                    })
                    .catch((err) => {
                      console.log('err on create package', err)
                      toasterService.error('Erro ao remover pacote')
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
      total: 0,
      accessType: ''
    }
  },
  mounted () {
    this.loadPackages()
    this.accessType = authenticationService.accessType()
  }
}
</script>
