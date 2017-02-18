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
                    :draggable="false"></gmap-marker>
              <gmap-polyline v-for="l in lines"
                        :path="l.path"
                        :editable="false"
                        :draggable="false"
                        :options="{geodesic:true, strokeColor:'#FF0000'}">
              </gmap-polyline>
    </gmap-map>

    <router-link to='/package/list' tag='button'
                 style="margin-top: 3em;"
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
        lines: [],
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

        // $('.dimmer').dimmer('show')
        manualPointService.create(request)
                          .then(response => {
                            toasterService.success('Ponto manual criado com sucesso')
                            var destination = {position: {lat: latitude, lng: longitude}}
                            this.package.manualPoints.push(destination)

                            if (this.package.manualPoints.length <= 1) {
                              return
                            }

                            var source = this.package.manualPoints[this.package.manualPoints.length - 2]
                            this.lines.push({path: [source.position, destination.position]})
                          })
                          .catch((err) => {
                            toasterService.error('Erro ao criar ponto manual')
                            throw err
                          })
                          .finally(() => {
                            // $('.dimmer').dimmer('hide')
                          })
      }
    },
    props: ['id'],
    mounted: function () {
      $('.dimmer').dimmer('show')
      packageService.get(this.id)
                    .then(response => {
                      this.package = response
                      this.package.manualPoints = this.package.manualPoints
                      this.lines = []
                      for (var i = 1; i < this.package.manualPoints.length; i++) {
                        var source = this.package.manualPoints[i - 1]
                        var destination = this.package.manualPoints[i]
                        this.lines.push({path: [source.position, destination.position]})
                      }
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
