<template>
  <div class="ui fixed inverted menu">
    <div class="ui container">
      <a href="#" class="header item">
        <img class="logo" src="../../assets/logo.png">
        Project Name
      </a>
      <router-link class="item" to="/">Home</router-link>
      <div class="ui simple dropdown item">
        Gerenciamento <i class="dropdown icon"></i>
        <div class="menu">
          <router-link class="item" to="/package/list">Pacotes</router-link>
          <a class="item" href="#">Dispositivos</a>
          <div class="divider"></div>
          <div class="header">Acesso</div>
          <div class="item">
            <i class="dropdown icon"></i>
            Usuários
            <div class="menu">
              <router-link class="item" to="/user/list">Gerenciar usuários</router-link>
              <a class="item" href="#">Permissões</a>
            </div>
          </div>
          <a class="item" href="#">Editar perfil</a>
        </div>
      </div>
      <router-link class="item" to="/logout">Logout</router-link>
    </div>
  </div>
</template>
