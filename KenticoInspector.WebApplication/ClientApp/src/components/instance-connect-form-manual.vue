<template>
  <v-card>
    <v-card-text>
      <v-form
        ref="form"
        v-model="valid"
        lazy-validation
        >
        <v-text-field
          v-model="instance.databaseSettings.server"
          :rules="[v => !!v || 'Database server name is required']"
          label="Database server name"
          required
          clearable
          />

        <v-text-field
          v-model="instance.databaseSettings.database"
          :rules="[v => !!v || 'Database name is required']"
          label="Database name"
          required
          clearable
          />

        <v-switch
          v-model="instance.databaseSettings.integratedSecurity"
          label="Integrated Security"
          >
        </v-switch>

        <div v-show="!instance.databaseSettings.integratedSecurity">
          <v-text-field
            v-model="instance.databaseSettings.user"
            :rules="[v => {
              const isIntegrated = this.instance.databaseSettings.integratedSecurity
              isValid = isIntegrated ? true : !!v
              return isValid || 'SQL user is required'
            }]"
            label="SQL User"
            required
            clearable
            />

          <v-text-field
            v-model="instance.databaseSettings.password"
            type="password"
            label="SQL user password"
            clearable
            />
        </div>

        <v-text-field
          v-model="instance.url"
          :rules="[v => !!v || 'Site URL is required']"
          label="Site URL"
          placeholder="https:\\localhost\DancingGoat"
          required
          clearable
          />

        <v-text-field
          v-model="instance.path"
          :rules="[v => !!v || 'Administration instance root folder is required']"
          label="Administration instance root folder"
          placeholder="C:\inetpub\wwwroot\DancingGoat\CMS"
          required
          clearable
          />
        <v-text-field
          v-model="instance.name"
          label="Name (optional)"
          placeholder="Dancing Goat"
          clearable
          />
      </v-form>
    </v-card-text>
    <v-card-actions>
      <v-spacer />
      <v-btn
        large
        :disabled="!valid"
        color="primary"
        @click="submit"
        >
        Connect to Kentico Instance
      </v-btn>
    </v-card-actions>
  </v-card>
</template>

<script>
import { mapActions } from 'vuex'
export default {
  data: () => ({
    authenticationTypes: [
      {
        text: "Integrated Windows Authentication",
        value: "windows"
      },
      {
        text: "SQL Server Account",
        value: "account"
      }
    ],
    valid: false,
    instance: {
      databaseSettings: {
        server: "",
        database: "",
        integratedSecurity: false,
        user: "",
        password: ""
      },
      url: "",
      path: "",
      name: ""
    }
  }),
  methods: {
    ...mapActions([
      'upsertInstance'
    ]),
    submit () {
      if (this.$refs.form.validate()) {
        this.upsertInstance(this.instance)
        //this.$refs.form.reset()
      }
    },
    reset () {
      this.$refs.form.reset()
    }
  }
}
</script>
