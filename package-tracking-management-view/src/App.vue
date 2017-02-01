<template>
  <transition-group appear name="fade">
    <div class="ui page dimmer" key="loader">
      <div class="ui text loader" key="loader">Carregando</div>
    </div>
    <router-view class="view" key="view"></router-view>
  </transition-group>
</template>

<script>
  import Login from './components/Login'
  import Dashboard from './components/Dashboard'
  import HeaderLayout from './components/Header'
  import FooterLayout from './components/Footer'
  import PackagesList from './components/PackagesList'
  import Application from './components/Application'

  export default {
    name: 'app',
    components: {
      Login,
      Dashboard,
      HeaderLayout,
      FooterLayout,
      Application,
      PackagesList
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
    height: 100%;
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
</style>
