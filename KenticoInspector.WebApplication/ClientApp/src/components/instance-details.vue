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
            :href="instance.url"
            target="_blank"
            >
            {{displayName}}
          </a>
        </v-list-tile-title>
        <v-list-tile-sub-title>Site</v-list-tile-sub-title>
      </v-list-tile-content>
    </v-list-tile>
    <v-list-tile>
      <v-list-tile-content>
        <v-list-tile-title>{{instance.path}}</v-list-tile-title>
        <v-list-tile-sub-title>Path</v-list-tile-sub-title>
      </v-list-tile-content>
    </v-list-tile>
    <v-subheader>Database Configuration</v-subheader>
    <v-list-tile>
      <v-list-tile-content>
        <v-list-tile-title>{{instance.databaseSettings.server}}</v-list-tile-title>
        <v-list-tile-sub-title>Server</v-list-tile-sub-title>
      </v-list-tile-content>
    </v-list-tile>
    <v-list-tile>
      <v-list-tile-content>
        <v-list-tile-title>{{instance.databaseSettings.database}}</v-list-tile-title>
        <v-list-tile-sub-title>Database</v-list-tile-sub-title>
      </v-list-tile-content>
    </v-list-tile>
    <v-list-tile>
      <v-list-tile-content>
        <v-list-tile-title>{{instance.databaseSettings.user}}</v-list-tile-title>
        <v-list-tile-sub-title>User</v-list-tile-sub-title>
      </v-list-tile-content>
    </v-list-tile>
    <v-list-tile>
      <v-list-tile-content>
        <v-list-tile-title>{{instance.databaseSettings.password}}</v-list-tile-title>
        <v-list-tile-sub-title>Password</v-list-tile-sub-title>
      </v-list-tile-content>
    </v-list-tile>
    <v-list-tile>
      <v-list-tile-content>
        <v-list-tile-title>{{instance.databaseSettings.integratedSecurity}}</v-list-tile-title>
        <v-list-tile-sub-title>Integrated Security</v-list-tile-sub-title>
      </v-list-tile-content>
    </v-list-tile>
  </v-list>
</template>

<script>
import { mapGetters } from 'vuex'
export default {
  props: {
    instance: {
      type: Object,
      required: true
    }
  },
  computed: {
    ...mapGetters('instances', [
      'getInstanceDisplayName'
    ]),
    displayName: function() {
      const name = this.getInstanceDisplayName(this.instance.guid)
      return name
    }
  }
}
</script>
