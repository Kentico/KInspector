<template>
  <v-container>
    <v-layout>
      <v-flex xs12>
        <h1 class="display-2">
          Welcome <span class="display-1 font-weight-light">to Kentico Inspector</span>
        </h1>
        <p>The open source tool for analyzing Kentico EMS instances.</p>
        <v-card
          color="light-blue lighten-4"
          class="mb-3"
          >
          <v-card-title class="pb-0">
            <span class="title">
              <v-icon color="black">mdi-information</v-icon>
              Data Modification Notice</span>
          </v-card-title>
          <v-card-text class="title font-weight-light">
            The application does not modify data without clear warning. Regardless, it is not recommended to use run this in production.
          </v-card-text>
        </v-card>
      </v-flex>
    </v-layout>
    <v-layout>
      <v-flex xs12>
        <v-btn
          large
          dark
          color="primary"
          to="/connect"
          class="mx-auto"
          >
          Get Started
        </v-btn>
      </v-flex>
    </v-layout>
  </v-container>
</template>
