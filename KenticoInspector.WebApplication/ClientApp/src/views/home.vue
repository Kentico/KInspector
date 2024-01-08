<template>
  <v-container>
    <v-layout
      row
      wrap
      class="glass-pane pa-3"
      >
      <v-flex xs12>
        <h1 class="display-2">
          Welcome <span class="display-1 font-weight-light">to Kentico Inspector</span>
        </h1>
        <p>The open source tool for analyzing Kentico EMS instances.</p>
      </v-flex>

      <v-flex
        xs12
        text-xs-center
        >
        <v-btn
          large
          dark
          color="primary"
          to="/connect"
          class="mx-auto"
          >
          Connect to Get Started
        </v-btn>
      </v-flex>
    </v-layout>
  </v-container>
</template>
