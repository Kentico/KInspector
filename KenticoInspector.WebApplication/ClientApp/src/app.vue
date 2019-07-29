<template>
  <v-app
    :style="`background: url(${require('./assets/background.png')}) no-repeat bottom fixed; background-size: cover;`"
    >
    <the-main-toolbar></the-main-toolbar>

    <v-content>
      <router-view/>
    </v-content>
    <v-footer class="pa-3" color="rgba(255,255,255,.85)">
      <v-spacer></v-spacer>
      <v-btn
        flat
        href="https://github.com/kentico/kinspector"
        target="_blank"
        class="my-0"
        >
        <span class="mr-2">Source on Github</span>
        <v-icon>mdi-github-box</v-icon>
      </v-btn>
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

  .glass-pane {
    background-color: rgba(255,255,255,.85);
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
