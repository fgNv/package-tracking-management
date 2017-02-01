<template>
  <div>
    <header-layout></header-layout>
    <div class="ui main text container">
        <h1 class="ui header">
          Gerenciamento de pacotes
          <button class="ui secondary button right floated">Cadastrar pacote</button>
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
            <tr>
              <td>
                <div class="ui ribbon label">First</div>
              </td>
              <td>Cell</td>
              <td>Cell</td>
            </tr>
            <tr>
              <td>Cell</td>
              <td>Cell</td>
              <td>Cell</td>
            </tr>
            <tr>
              <td>Cell</td>
              <td>Cell</td>
              <td>Cell</td>
            </tr>
          </tbody>
          <tfoot>
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
    <footer-layout></footer-layout>
  </div>
</template>

<script>
import HeaderLayout from './Header'
import FooterLayout from './Footer'

export default {
  name: 'package-list',
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
