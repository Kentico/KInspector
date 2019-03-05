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
            <v-icon>home</v-icon>
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
            <v-icon>apps</v-icon>
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
            <v-icon>power</v-icon>
          </v-list-tile-action>
          <v-list-tile-content>
            <v-list-tile-title>
              Connect
            </v-list-tile-title>
          </v-list-tile-content>
        </v-list-tile>
      </v-list>
    </v-navigation-drawer>

    <v-toolbar
      fixed
      clipped-left
      app
      dark
      color="#262524"
      >
      <v-toolbar-side-icon @click.stop="drawer = !drawer"></v-toolbar-side-icon>
      <span>
        <v-img
          :src="require('./assets/kentico.svg')"
          contain
          height="38"
          width="38"
          style="background-color: #F05A22"
          />
      </span>
      <v-toolbar-title class="headline text-uppercase ml-1">
        <span>Kentico</span>
        <span class="font-weight-light">Inspector</span>
      </v-toolbar-title>
      <v-spacer></v-spacer>
      <v-btn
        flat
        href="https://github.com/kentico/kinspector"
        target="_blank"
      >
        <span class="mr-2">View on Github</span>
        <v-icon>open_in_new</v-icon>
      </v-btn>
    </v-toolbar>

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

export default {
  name: 'App',
  data () {
    return {
      drawer: null
    }
  }
}
</script>

<style>
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
