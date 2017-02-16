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

      {{selected}}
    </div>

</template>

<script>
  import $ from 'jquery'
  import sharedData from '../../services/SharedData.js'
  import authenticationService from 'services/Authentication.js'

  export default {
    data () {
      return {
        selected: {}
      }
    },
    mounted () {
      var self = this

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
          self.selected = result
        }
      })
    }
  }

</script>
