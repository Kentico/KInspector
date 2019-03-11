<template>
   <v-toolbar
      fixed
      clipped-left
      app
      dark
      color="#262524"
      >
      <span>
        <v-img
          :src="require('../assets/kentico.svg')"
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
        icon
        to="/"
        >
        <v-icon>mdi-home</v-icon>
      </v-btn>
      <connection-status-indicator></connection-status-indicator>
      <v-btn
        flat
        href="https://github.com/kentico/kinspector"
        target="_blank"
      >
        <span class="mr-2">View on Github</span>
        <v-icon>mdi-open-in-new</v-icon>
      </v-btn>
    </v-toolbar>
</template>

<script>
import ConnectionStatusIndicator from './connection-status-indicator'
export default {
  components: {
    ConnectionStatusIndicator
  }
}
</script>
