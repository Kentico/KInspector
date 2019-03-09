<template>
  <v-app
    :style="`background: url(${require('./assets/background.png')}) no-repeat bottom fixed; background-size: cover;`"
    >
    <v-navigation-drawer
      v-model="drawer"
      fixed
      clipped
      app
      >
      <v-list>
        <v-list-tile
          to="/"
          >
          <v-list-tile-action>
            <v-icon>mdi-home</v-icon>
          </v-list-tile-action>
          <v-list-tile-content>
            <v-list-tile-title>
              Home
            </v-list-tile-title>
          </v-list-tile-content>
        </v-list-tile>
        <v-list-tile
          to="/reports"
          >
          <v-list-tile-action>
            <v-icon>mdi-file-chart</v-icon>
          </v-list-tile-action>
          <v-list-tile-content>
            <v-list-tile-title>
              Reports
            </v-list-tile-title>
          </v-list-tile-content>
        </v-list-tile>
        <v-list-tile
          to="/connect"
          >
          <v-list-tile-action>
            <v-icon>mdi-power-plug</v-icon>
          </v-list-tile-action>
          <v-list-tile-content>
            <v-list-tile-title>
              Connect
            </v-list-tile-title>
          </v-list-tile-content>
        </v-list-tile>
      </v-list>
    </v-navigation-drawer>

    <the-main-toolbar></the-main-toolbar>

    <v-content>
      <router-view/>
    </v-content>
    <v-footer class="pa-3" color="rgba(255,255,255,.85)">
      <v-spacer></v-spacer>
      <div>Version 4.0 Alpha</div>
    </v-footer>
  </v-app>
</template>

<script>
import TheMainToolbar from './components/the-main-toolbar'

export default {
  name: 'App',
  components: {
    TheMainToolbar
  },
  data () {
    return {
      drawer: null
    }
  }
}
</script>

<style>
  html { overflow-y: auto }

  .layout {
    background-color: rgba(255,255,255,.85);
    padding: 0 24px;
  }
  .layout:first-of-type {
    padding-top: 24px;
  }
  .layout:last-of-type {
    padding-bottom: 24px;
  }

  .theme--light .v-label {
    color: rgba(0,0,0,.87)
  }
</style>
