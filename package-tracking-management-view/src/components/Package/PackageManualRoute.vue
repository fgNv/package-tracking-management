<template>
  <div class='ui main text container'>
    <h1 class='ui header'>
      Definição manual de rota para pacote {{package.name}}
    </h1>

    <h3>Descrição</h3>
    <p>{{package.description}}</p>

    <gmap-map :center="initialCenter" :zoom="zoom" style="height: 500px;"
              @click="save">
              <gmap-marker
                    v-for="m in package.manualPoints"
                    :position="m.position"
                    :clickable="false"
                    :draggable="false"
                  ></gmap-marker>

    </gmap-map>

    <router-link to='/package/list' tag='button'
                 class='ui button secondary' tabindex='1'>
      Voltar
    </router-link>
  </div>
</template>

<script>
  import packageService from 'services/Package.js'
  import toasterService from 'services/Toaster.js'
  import manualPointService from 'services/ManualPoint.js'
  import $ from 'jquery'

  export default {
    data () {
      return {
        package: {},
        mapType: 'terrain',
        zoom: 5,
        initialCenter: {lat: -18.8800397, lng: -47.05878999999999}
      }
    },
    methods: {
      save (mouseArgs) {
        var latitude = mouseArgs.latLng.lat()
        var longitude = mouseArgs.latLng.lng()

        var request = {
          packageId: this.package.id,
          latitude,
          longitude
        }

        $('.dimmer').dimmer('show')
        manualPointService.create(request)
                          .then(response => {
                            toasterService.success('Ponto manual criado com sucesso')
                          })
                          .catch((err) => {
                            toasterService.error('Erro ao criar ponto manual')
                            throw err
                          })
                          .finally(() => {
                            $('.dimmer').dimmer('hide')
                          })
      }
    },
    props: ['id'],
    mounted: function () {
      $('.dimmer').dimmer('show')
      packageService.get(this.id)
                    .then(response => {
                      this.package = response
                      this.package.manualPoints = this.package
                                                      .manualPoints.map(p => {
                                                        return p
                                                      })
                    })
                    .finally(() => {
                      $('.dimmer').dimmer('hide')
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
