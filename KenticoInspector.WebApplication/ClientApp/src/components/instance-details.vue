<template>
  <v-list
    dense
    two-line
    subheader
    >
    <v-subheader>Administration Configuration</v-subheader>
    <v-list-tile>
      <v-list-tile-content>
        <v-list-tile-title>
          <a
            :href="instance.administrationConfiguration.uri"
            target="_blank"
            >
            {{instance.administrationConfiguration.uri}}
          </a>
        </v-list-tile-title>
        <v-list-tile-sub-title>Site</v-list-tile-sub-title>
      </v-list-tile-content>
    </v-list-tile>
    <v-list-tile>
      <v-list-tile-content>
        <v-list-tile-title>{{instance.administrationConfiguration.directoryPath}}</v-list-tile-title>
        <v-list-tile-sub-title>Directory Path</v-list-tile-sub-title>
      </v-list-tile-content>
    </v-list-tile>
    <v-subheader>Database Configuration</v-subheader>
    <v-list-tile>
      <v-list-tile-content>
        <v-list-tile-title>{{instance.databaseConfiguration.serverName}}</v-list-tile-title>
        <v-list-tile-sub-title>Server</v-list-tile-sub-title>
      </v-list-tile-content>
    </v-list-tile>
    <v-list-tile>
      <v-list-tile-content>
        <v-list-tile-title>{{instance.databaseConfiguration.databaseName}}</v-list-tile-title>
        <v-list-tile-sub-title>Database</v-list-tile-sub-title>
      </v-list-tile-content>
    </v-list-tile>
    <v-list-tile>
      <v-list-tile-content>
        <v-list-tile-title>{{instance.databaseConfiguration.user}}</v-list-tile-title>
        <v-list-tile-sub-title>User</v-list-tile-sub-title>
      </v-list-tile-content>
    </v-list-tile>
    <v-list-tile>
      <v-list-tile-content>
        <v-list-tile-title>{{instance.databaseConfiguration.password}}</v-list-tile-title>
        <v-list-tile-sub-title>Password</v-list-tile-sub-title>
      </v-list-tile-content>
    </v-list-tile>
    <v-list-tile>
      <v-list-tile-content>
        <v-list-tile-title>{{instance.databaseConfiguration.integratedSecurity}}</v-list-tile-title>
        <v-list-tile-sub-title>Integrated Security</v-list-tile-sub-title>
      </v-list-tile-content>
    </v-list-tile>
  </v-list>
</template>

<script>
export default {
  props: {
    instance: {
      type: Object,
      required: true
    }
  }
}
</script>
