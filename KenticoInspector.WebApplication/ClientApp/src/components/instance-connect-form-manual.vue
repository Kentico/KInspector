<template>
  <v-card>
    <v-card-text>
      <v-form
        ref="form"
        v-model="valid"
        lazy-validation
        >
        <v-text-field
          v-model="instanceConfiguration.databaseConfiguration.serverName"
          :rules="[v => !!v || 'Database server name is required']"
          label="Database server name"
          required
          clearable
          />

        <v-text-field
          v-model="instanceConfiguration.databaseConfiguration.databaseName"
          :rules="[v => !!v || 'Database name is required']"
          label="Database name"
          required
          clearable
          />

        <v-switch
          v-model="instanceConfiguration.databaseConfiguration.integratedSecurity"
          label="Integrated Security"
          >
        </v-switch>

        <div v-show="!instanceConfiguration.databaseConfiguration.integratedSecurity">
          <v-text-field
            v-model="instanceConfiguration.databaseConfiguration.user"
            :rules="[v => {
              const isIntegrated = this.instanceConfiguration.databaseConfiguration.integratedSecurity
              isValid = isIntegrated ? true : !!v
              return isValid || 'SQL user is required'
            }]"
            label="SQL User"
            required
            clearable
            />

          <v-text-field
            v-model="instanceConfiguration.databaseConfiguration.password"
            type="password"
            label="SQL user password"
            clearable
            />
        </div>

        <v-text-field
          v-model="instanceConfiguration.administrationConfiguration.uri"
          :rules="[v => !!v || 'Site URL is required']"
          label="Site URL"
          placeholder="https:\\localhost\DancingGoat"
          required
          clearable
          />

        <v-text-field
          v-model="instanceConfiguration.administrationConfiguration.directoryPath"
          :rules="[v => !!v || 'Administration instance root folder is required']"
          label="Administration instance root folder"
          placeholder="C:\inetpub\wwwroot\DancingGoat\CMS"
          required
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
    instanceConfiguration: {
      databaseConfiguration: {
        serverName: "",
        databaseName: "",
        integratedSecurity: false,
        user: "",
        password: ""
      },
      administrationConfiguration: {
        uri: "",
        directoryPath: ""
      }
    }
  }),
  methods: {
    ...mapActions([
      'UPSERT_INSTANCE_CONFIGURATION'
    ]),
    submit () {
      if (this.$refs.form.validate()) {
        this.UPSERT_INSTANCE_CONFIGURATION(this.instanceConfiguration)
        this.$refs.form.reset()
      }
    },
    reset () {
      this.$refs.form.reset()
    }
  }
}
</script>
