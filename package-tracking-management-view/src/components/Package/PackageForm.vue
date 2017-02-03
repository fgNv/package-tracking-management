<template>
  <div class="ui main text container">
    <h1 class="ui header">
      Cadastrar pacote
    </h1>

    <form class="ui form">
      <h4 class="ui dividing header">Dados do pacote</h4>
      <div class="field">
        <label>Nome</label>
        <div class="two fields">
          <div class="field">
            <input type="text" name="shipping[first-name]">
          </div>
        </div>
      </div>
      <div class="field">
        <label>Descrição</label>
        <textarea rows="2"></textarea>
      </div>
      <button class="ui button primary" tabindex="0">
        Salvar
      </button>
      <router-link to="/package/list" tag="button" class="ui button secondary" tabindex="1">
        Cancelar
      </router-link>
    </form>
  </div>
</template>
