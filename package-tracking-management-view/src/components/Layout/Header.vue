<template>
  <div class="ui fixed inverted menu">
    <div class="ui container">
      <a href="#" class="header item">
        <img class="logo" src="../../assets/logo.png">
        Project Name
      </a>
      <router-link class="item" to="/">Home</router-link>
      <div class="ui simple dropdown item">
        Gerenciamento <i class="dropdown icon"></i>
        <div class="menu">
          <router-link class="item" to="/package/list">Pacotes</router-link>
          <a class="item" href="#" v-if="accessType == 'administrator'">
            Dispositivos
          </a>
          <div class="divider"></div>
          <div class="header">Acesso</div>
          <div class="item" v-if="accessType == 'administrator'">
            <i class="dropdown icon"></i>
            Usuários
            <div class="menu">
              <router-link class="item" to="/user/list">Gerenciar usuários</router-link>
              <router-link class="item" to="/user/permission">Permissões</router-link>
            </div>
          </div>
          <a class="item" href="#">Editar perfil</a>
        </div>
      </div>
      <div class="right menu">
        <a class="item">Autenticado como &nbsp; <b>{{name}}</b></a>
        <router-link class="item" to="/logout">Logout</router-link>
      </div>
    </div>
  </div>
</template>

<script>
import authenticationService from 'services/Authentication.js'

export default {
  name: 'header-layout',
  data () {
    return {
      accessType: '',
      name: ''
    }
  },
  mounted () {
    this.accessType = authenticationService.accessType()
    this.name = authenticationService.getName()
  }
}
</script>
