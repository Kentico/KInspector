<template>
  <v-container>
    <v-layout
      row
      wrap
      >
      <v-flex xs12>
        <h1>
          Connect to Kentico EMS instance
        </h1>
        <p>
          TBD
        </p>
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
  export default {
    data: () => ({
      instance: {
        databaseConnection: {
          server: "",
          database: ""
        },
        url: "",
        path: ""
      }
    })
  }
</script>

<style>

</style>
