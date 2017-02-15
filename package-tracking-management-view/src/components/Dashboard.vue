<template>
  <div class="ui main text container">
    <h1 class="ui header">
      Home
    </h1>

    <p>
      Aplicação de exemplo.
    </p>
  </div>
</template>

<script>
import authenticationService from 'services/Authentication.js'

export default {
  name: 'dashboard',
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
