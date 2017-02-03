<template>
  <transition-group appear name="fade">
    <header-layout v-if="isAuthenticated()" key="header"></header-layout>
    <div class="ui page dimmer" key="loader">
      <div class="ui text loader">Carregando</div>
    </div>
    <router-view class="view" key="view"></router-view>
    <footer-layout v-if="isAuthenticated()" key="footer"></footer-layout>
  </transition-group>
</template>

<script>
  import HeaderLayout from './components/Layout/Header'
  import FooterLayout from './components/Layout/Footer'
  import authenticationService from 'services/Authentication.js'

  export default {
    name: 'app',
    methods: {
      isAuthenticated: function () {
        return authenticationService.isLoggedIn()
      }
    },
    components: {
      HeaderLayout,
      FooterLayout
    },
    localStorage: {
      access_data: {
        type: Object
      }
    }
  }
</script>

<style>
  .view{
    margin-bottom: 5em;
  }

  .fade-enter-active, .fade-leave-active {
    transition-property: opacity;
    transition-duration: .25s;
  }

  .fade-enter-active {
    transition-delay: .25s;
  }

  .fade-enter, .fade-leave-active {
    opacity: 0
  }

  .ui.menu .item img.logo {
    margin-right: 1.5em;
  }
  .main.container {
    margin-top: 7em;
  }
  .wireframe {
    margin-top: 2em;
  }
  .ui.footer.segment {
    margin: 5em 0em 0em;
    padding: 5em 0em;
  }
</style>
