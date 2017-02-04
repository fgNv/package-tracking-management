<template>
  <div class="ui main text container">
    <h1 class="ui header">
      Gerenciamento de pacotes
      <router-link to="/package/create" tag="button" class="ui secondary button right floated">
        Cadastrar pacote
      </router-link>
    </h1>
    <table class="ui celled table">
      <thead>
         <tr>
           <th>Nome do pacote</th>
           <th>Última alteração</th>
           <th>Ações</th>
         </tr>
      </thead>
      <tbody>
       <tr v-for="(item, index) in items">
         <td>
          <div v-bind:class="{'ui ribbon label': index == 0}">
            {{item.name}}
          </div>
         </td>
         <td> {{item.updatedAt}} </td>
         <td>Cell</td>
       </tr>
      <tfoot v-on:click="loadPackages()">
        <tr>
          <th colspan="3">
            <div class="ui right floated pagination menu">
              <a class="icon item">
                <i class="left chevron icon"></i>
              </a>
              <a class="item">1</a>
              <a class="item">2</a>
              <a class="item">3</a>
              <a class="item">4</a>
              <a class="icon item">
                <i class="right chevron icon"></i>
              </a>
            </div>
          </th>
        </tr>
      </tfoot>
    </table>
  </div>
</template>

<script>
import PackageService from 'services/Package.js'

export default {
  name: 'package-list',
  localStorage: {
    access_data: {
      type: Object
    }
  },
  methods: {
    loadPackages () {
      PackageService.query()
                    .then(response => {
                      this.items = response.items
                      this.total = response.total
                    })
    }
  },
  data () {
    return {
      currentPage: 1,
      items: [],
      total: 0
    }
  },
  mounted: function () {
    this.loadPackages()
  }
}
</script>
