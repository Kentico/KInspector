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
        <v-form
          ref="form"
          v-model="valid"
          lazy-validation
          >
          <v-text-field
            v-model="instance.databaseConnection.server"
            :rules="[v => !!v || 'Server is required']"
            label="Server"
            required
            />

          <v-text-field
            v-model="instance.databaseConnection.database"
            :rules="[v => !!v || 'Database is required']"
            label="Database"
            required
            />

          <v-btn
            :disabled="!valid"
            color="success"
            @click="validate"
            >
            Validate
          </v-btn>

          <v-btn
            color="error"
            @click="reset"
          >
            Reset Form
          </v-btn>
        </v-form>
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
  export default {
    data: () => ({
      valid: true,
      instance: {
        databaseConnection: {
          server: "",
          database: ""
        },
        url: "",
        path: ""
      }
    }),
    methods: {
      validate () {
        if (this.$refs.form.validate()) {
          this.snackbar = true
        }
      },
      reset () {
        this.$refs.form.reset()
      }
    }
  }
</script>

<style>

</style>
