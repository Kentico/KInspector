<template>
  <v-card>
    <v-toolbar
      dark
      color="primary"
      >
      <v-toolbar-title>
        Connect Manually
      </v-toolbar-title>
    </v-toolbar>
    <v-card-text>
      <v-form
        ref="form"
        v-model="valid"
        lazy-validation
        >
        <v-text-field
          v-model="instance.database.serverName"
          :rules="[v => !!v || 'Database server name is required']"
          label="Database server name"
          required
          />

        <v-text-field
          v-model="instance.database.databaseName"
          :rules="[v => !!v || 'Database name is required']"
          label="Database name"
          required
          />

        <v-select
          v-model="instance.database.authentication.type"
          :items="authenticationTypes"
          item-text="text"
          item-value="value"
          label="Authentication Type"
        ></v-select>

        <div v-show="instance.database.authentication.type === 'account'">
          <v-text-field
            v-model="instance.database.authentication.user"
            :rules="[v => !!v || 'SQL user is required']"
            label="SQL User"
            required
            />

          <v-text-field
            v-model="instance.database.authentication.password"
            type="password"
            label="SQL user password"
            />
        </div>

        <v-text-field
          v-model="instance.site.url"
          :rules="[v => !!v || 'Site URL is required']"
          label="Site URL"
          placeholder="https:\\localhost\DancingGoat"
          required
          />

        <v-text-field
          v-model="instance.site.filePath"
          :rules="[v => !!v || 'Administration instance root folder is required']"
          label="Administration instance root folder"
          placeholder="C:\inetpub\wwwroot\DancingGoat\CMS"
          required
          />
      </v-form>
    </v-card-text>
    <v-card-actions>
      <v-spacer />
      <v-btn
        :disabled="!valid"
        color="success"
        @click="validate"
        >
        Connect
      </v-btn>
      <v-btn
        color="error"
        @click="reset"
      >
        Reset
      </v-btn>
    </v-card-actions>
  </v-card>
</template>

<script>
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
    valid: true,
    instance: {
      database: {
        serverName: "",
        databaseName: "",
        authentication: {
          type: "",
          username: "",
          password: "",
        }
      },
      site: {
        url: "",
        filePath: ""
      }
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
