<template>
  <div class="ui segment">
    <div class="ui dimmer">
        <div class="ui text loader">
          Carregando
        </div>
    </div>
    <div class="ui middle aligned center aligned grid">
      <div class="column">
        <h2 class="ui teal image header">
          <img src="../assets/logo.png" class="image">
          <div class="content">
            Accesse sua conta
          </div>
        </h2>
        <form class="ui large form" v-on:submit.prevent="challengeCredentials">
            <div class="ui stacked segment">
              <div class="field">
                <div class="ui left icon input">
                  <i class="user icon"></i>
                  <input type="text" name="username"
                         v-model="request.username" placeholder="Nome de usuário">
                </div>
              </div>
              <div class="field">
                <div class="ui left icon input">
                  <i class="lock icon"></i>
                  <input type="password" name="password"
                         v-model="request.password" placeholder="Senha">
                </div>
              </div>
              <button class="ui fluid large teal submit button">Login</button>
            </div>
            <div class="ui error message"></div>
        </form>
        <div class="ui message">
          Novo por aqui? <a href="mailto:felipegarcia156@hotmail.com">Mande uma mensagem</a>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import $ from 'jquery'
import AuthenticationService from 'services/authentication.service.js'

var authenticationService = new AuthenticationService()

export default {
  name: 'login',
  data () {
    return {
      msg: 'Welcome to Your Vue.js App',
      request: {}
    }
  },
  methods: {
    challengeCredentials: function (ev) {
      $('.dimmer').dimmer('show')
      authenticationService.authenticate(this.request)
                           .then((r) => {
                             this.$emit('user-authenticated-successfully')
                           })
                           .catch((err) => {
                             console.log('err on challengeCredentials', err)
                           })
                           .finally(() => {
                             $('.segment').dimmer('hide')
                           })
    }
  }
}
</script>

<style scoped>
body > .segment, div > .grid {
  height: 100%;
}
.image {
  margin-top: -100px;
}
.column {
  max-width: 450px;
}
</style>
