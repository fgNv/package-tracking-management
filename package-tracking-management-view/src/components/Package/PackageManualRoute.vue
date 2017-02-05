<template>
  <div class='ui main text container'>
    <h1 class='ui header'>
      Definição manual de rota para pacote {{package.name}}
    </h1>

    <h3>Descrição</h3>
    <p>{{package.description}}</p>

    <gmap-map :center="initialCenter" :zoom="zoom" style="height: 500px;"></gmap-map>

    <router-link to='/package/list' tag='button'
                 class='ui button secondary' tabindex='1'>
      Voltar
    </router-link>
  </div>
</template>

<script>
  import packageService from 'services/Package.js'

  export default {
    data () {
      return {
        package: {},
        mapType: 'terrain',
        zoom: 5,
        initialCenter: {lat: -18.8800397, lng: -47.05878999999999}
      }
    },
    props: ['id'],
    mounted: function () {
      packageService.get(this.id)
                    .then(response => {
                      console.log('response', response)
                      this.package = response
                    })
    }
  }
</script>

<style scoped>
gmap-map {
  width:100%;
  height: 600px;
  display: block;
}
</style>
