<template>  
  <dashboard v-if="authenticated"></dashboard>  
  <login v-else="" v-on:user-authenticated-successfully="grantAccess()"></login>
</template>

<script>
  import Login from './components/Login'
  import Dashboard from './components/Dashboard'

  export default {
    name: 'app',
    components: {
      Login,
      Dashboard
    },
    data () {
      return {'authenticated': false}
    },
    methods: {
      grantAccess () {
        console.log('call grantAccess')
        this.authenticated = true
      },
      huehue () {
        console.log('BR')
      }
    }
  }
</script>

<style>
</style>
