<template>
  <div class='ui main text container'>
    <h1 class='ui header' v-if="!request.id">
      Cadastrar pacote
    </h1>
    <h1 class='ui header' v-if="request.id">
      Editar pacote
    </h1>

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
      <router-link to='/package/list' tag='button'
                   class='ui button secondary' tabindex='1'>
        Cancelar
      </router-link>
    </form>
  </div>
</template>

<script>
  import toasterService from 'services/Toaster.js'
  import packageService from 'services/Package.js'
  import $ from 'jquery'

  function create (request) {
    return packageService.create(request)
                         .then(response => {
                           toasterService.success('Pacote criado com sucesso')
                         })
                         .catch((err) => {
                           console.log('err on create package', err)
                           toasterService.error('Erro ao criar pacote')
                           throw err
                         })
                         .finally(() => {
                           $('.dimmer').dimmer('hide')
                         })
  }

  function update (request) {
    return packageService.update(request)
                         .then(response => {
                           toasterService.success('Pacote atualizado com sucesso')
                         })
                         .catch((err) => {
                           console.log('err on create package', err)
                           toasterService.error('Erro ao atualizar pacote')
                           throw err
                         })
                         .finally(() => {
                           $('.dimmer').dimmer('hide')
                         })
  }

  export default {
    name: 'package-form',
    data () {
      return {
        request: {
          name: '',
          description: '',
          id: null
        }
      }
    },
    props: ['id'],
    mounted () {
      if (!this.id) {
        return
      }
      packageService.get(this.id)
                    .then(response => {
                      this.request = response
                      console.log('response', response)
                    })
    },
    methods: {
      save: function () {
        var promise = this.request.id
                      ? update(this.request)
                      : create(this.request)
        $('.dimmer').dimmer('show')
        promise.then(() => {
          this.$router.push('/package/list')
        }).finally(() => {
          $('.dimmer').dimmer('hide')
        })
      }
    }
  }
</script>
