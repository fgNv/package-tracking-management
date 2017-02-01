<template>
  <div>
    <header-layout></header-layout>
    <div class="ui main text container">
        <h1 class="ui header">
          Autenticado
        </h1>
      </div>
    <footer-layout></footer-layout>
  </div>
</template>

<script>
import HeaderLayout from './Header'
import FooterLayout from './Footer'

export default {
  name: 'dashboard',
  localStorage: {
    access_data: {
      type: Object
    }
  },
  components: {
    HeaderLayout,
    FooterLayout
  }
}
</script>
