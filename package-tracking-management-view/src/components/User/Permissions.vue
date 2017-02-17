<template>
    <div class='ui main text container'>
      <h1 class='ui header'>
        Gerenciar permissões
      </h1>

      <div class="ui search">
        <div class="ui left icon input">
          <input class="prompt" type="text" placeholder="Busque um pacote">
          <i class="github icon"></i>
        </div>
      </div>
      {{permissions}}
      <div class="ui card" v-for="user in userOptions" v-if="selectedPackage">
        <div class="content">
          <a class="header">{{user.name}}</a>
          <div class="meta">
            <span class="date">{{user.email}}</span>
          </div>
          <div class="description">
            <div class="ui checkbox">
              <input type="checkbox"
                     v-on:click="grantPermission(user.id)"
                     name="example">
              <label>Habilitar visualização de pacote</label>
            </div>
          </div>
        </div>
      </div>
    </div>

</template>

<script>
  import $ from 'jquery'
  import sharedData from '../../services/SharedData.js'
  import authenticationService from 'services/Authentication.js'
  import userService from 'services/User.js'
  import permissionService from 'services/Permission.js'

  function initializePackageSearch (componentInstance) {
    $('.ui.search').search({
      apiSettings: {
        url: sharedData.apiBaseUrl + '/package?nameFilter={query}',
        beforeXHR (xhr) {
          xhr.setRequestHeader('Authorization', 'bearer ' + authenticationService.getToken())
        }
      },
      fields: {
        results: 'items',
        title: 'name',
        url: 'html_url'
      },
      minCharacters: 3,
      onSelect (result, response) {
        componentInstance.selectedPackage = result
        componentInstance.loadPermissions()
      }
    })
  }

  export default {
    data () {
      return {
        selectedPackage: null,
        userOptions: [],
        permissions: []
      }
    },
    methods: {
      loadPermissions () {
        permissionService.getByPackage(this.selectedPackage.id)
                         .then(permissions => {
                           this.permissions = permissions
                         })
      },
      grantPermission (userId) {
        permissionService.grant(userId, this.selectedPackage.id)
      }
    },
    mounted () {
      initializePackageSearch(this)
      userService.getUsers()
                 .then(response => {
                   this.userOptions = response.items
                 })
    }
  }

</script>
