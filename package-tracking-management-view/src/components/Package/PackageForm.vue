<template>
  <div class='ui main text container'>
    <h1 class='ui header'>
      Cadastrar pacote
    </h1>

    <div class='ui inline success nag'>
      <span class='title'>
        Pacote criado com sucesso
      </span>
      <i class='close icon'></i>
    </div>

    <div class='ui inline error nag'>
      <span class='title'>
        Falha ao criar pacote
      </span>
      <i class='close icon'></i>
    </div>

    <form class='ui form' v-on:submit.prevent='save'>
      <h4 class='ui dividing header'>Dados do pacote</h4>
      <div class='field'>
        <label>Nome</label>
        <div class='two fields'>
          <div class='field'>
            <input type='text' v-model="request.name" >
          </div>
        </div>
      </div>
      <div class='field'>
        <label>Descrição</label>
        <textarea rows='2' v-model="request.description" ></textarea>
      </div>
      <button class='ui button primary' tabindex='0'>
        Salvar
      </button>
      <router-link to='/package/list' tag='button' class='ui button secondary' tabindex='1'>
        Cancelar
      </router-link>
    </form>
  </div>
</template>

<script>
  import toasterService from 'services/Toaster.js'
  import packageService from 'services/Package.js'
  import $ from 'jquery'

  export default {
    name: 'package-form',
    data () {
      return {
        request: {
          name: '',
          description: ''
        }
      }
    },
    methods: {
      save: function () {
        $('.dimmer').dimmer('show')
        packageService.create(this.request)
                      .then(response => {
                        toasterService.success('Pacote criado com sucesso')
                        this.$router.push('/package/list')
                      })
                      .catch((err) => {
                        console.log('err on create package', err)
                        toasterService.error('Erro ao criar pacote')
                      })
                      .finally(() => {
                        $('.dimmer').dimmer('hide')
                      })
      }
    }
  }
</script>
