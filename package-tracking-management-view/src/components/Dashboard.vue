<template>
  <div class="ui main text container">
    <h1 class="ui header">
      Dashboard
    </h1>

    <line-example :width="30" :height="15"></line-example>
  </div>
</template>

<script>
import authenticationService from 'services/Authentication.js'
import { Line } from 'vue-chartjs'

let lineExample = Line.extend({
  mounted () {
    this.renderChart({
      labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],
      datasets: [
        {
          label: 'Pacotes cadastrados',
          backgroundColor: '#f87979',
          data: [40, 20, 12, 39, 10, 40, 39, 20, 40, 20, 12, 11]
        }
      ]
    }, {responsive: true, legend: {position: 'bottom'}})
  }
})

export default {
  name: 'dashboard',
  components: { lineExample },
  localStorage: {
    access_data: {
      type: Object
    }
  },
  mounted () {
    var accessType = authenticationService.accessType()
    if (accessType === 'user') {
      this.$router.push('/package/list')
    }
  }
}
</script>
