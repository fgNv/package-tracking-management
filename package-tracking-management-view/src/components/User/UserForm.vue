<template>
  <div class='ui main text container'>
    <h1 class='ui header' v-if='!request.id'>
      Cadastrar usuário
    </h1>
    <h1 class='ui header' v-if='request.id'>
      Editar usuário
    </h1>

    <form class='ui form' v-on:submit.prevent='save'>
      <h4 class='ui dividing header'>Dados do usuário</h4>
      <div class='field'>
        <label>Nome de usuário (login)</label>
        <div class='two fields'>
          <div class='field'>
            <input type='text' v-model='request.userName' >
          </div>
        </div>
      </div>
      <div class='field'>
        <label>Nome</label>
        <div class='two fields'>
          <div class='field'>
            <input type='text' v-model='request.name' >
          </div>
        </div>
      </div>
      <div class='field'>
        <label>E-mail</label>
        <input rows='2' v-model='request.email' />
      </div>
      <div class='field'>
        <label>Tipo de acesso</label>
        <select class='ui fluid dropdown' v-model='request.accessType'>
          <option v-for='option in accessTypeOptions' v-bind:value='option.value'>
            {{ option.text }}
          </option>
        </select>
      </div>
      <div class='field' v-if='!request.id'>
        <label>Senha</label>
        <input type='password' v-model='request.password' />
      </div>
      <div class='field' v-if='!request.id'>
        <label>Confirmação da senha</label>
        <input type='password' v-model='request.passwordConfirmation' />
      </div>
      <button class='ui button primary' tabindex='0'>
        Salvar
      </button>
      <router-link to='/user/list' tag='button'
                   class='ui button secondary' tabindex='1'>
        Cancelar
      </router-link>
    </form>
  </div>
</template>

<script>
  import toasterService from 'services/Toaster.js'
  import userService from 'services/User.js'
  import $ from 'jquery'

  function create (request) {
    return userService.create(request)
                      .then(response => {
                        toasterService.success('Usuário criado com sucesso')
                      })
                      .catch((err) => {
                        console.log('err on create user', err)
                        toasterService.error('Erro ao criar usuário')
                        throw err
                      })
                      .finally(() => {
                        $('.dimmer').dimmer('hide')
                      })
  }

  function update (request) {
    return userService.update(request)
                      .then(response => {
                        toasterService.success('Usuário atualizado com sucesso')
                      })
                      .catch((err) => {
                        console.log('err on create user', err)
                        toasterService.error('Erro ao atualizar usuário')
                        throw err
                      })
                      .finally(() => {
                        $('.dimmer').dimmer('hide')
                      })
  }

  export default {
    name: 'user-form',
    data () {
      return {
        request: {
          name: '',
          email: '',
          password: '',
          passwordConfirmation: '',
          accessType: '',
          id: null
        },
        accessTypeOptions: [
          {text: 'Administrador', value: {accessType: 'administrator'}},
          {text: 'Usuário', value: {accessType: 'user'}}
        ]
      }
    },
    props: ['id'],
    mounted () {
      if (!this.id) {
        return
      }
      userService.get(this.id)
                 .then(response => {
                   this.request = response
                   console.log('response', response)
                 })
    },
    methods: {
      save () {
        if (!this.request.id && this.request.password !== this.request.passwordConfirmation) {
          toasterService.error('Erro ao salvar usuário',
                               ['Senha e confirmação devem coincidir.'])
          return
        }

        var promise = this.request.id
                      ? update(this.request)
                      : create(this.request)
        $('.dimmer').dimmer('show')
        promise.then(() => {
          this.$router.push('/user/list')
        }).finally(() => {
          $('.dimmer').dimmer('hide')
        })
      }
    }
  }
</script>
