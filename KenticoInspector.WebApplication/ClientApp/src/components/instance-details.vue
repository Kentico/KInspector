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
        <v-list-tile-sub-title>Instance</v-list-tile-sub-title>
      </v-list-tile-content>
    </v-list-tile>
    <template v-if="isConnected">
      <v-list-tile>
        <v-list-tile-content>
          <v-list-tile-title>
              {{currentInstanceDetails.administrationVersion.major}}.{{currentInstanceDetails.administrationVersion.minor}}.{{currentInstanceDetails.administrationVersion.build}}
          </v-list-tile-title>
          <v-list-tile-sub-title>Administration Version</v-list-tile-sub-title>
        </v-list-tile-content>
      </v-list-tile>
      <v-list-tile>
        <v-list-tile-content>
          <v-list-tile-title>
              {{currentInstanceDetails.databaseVersion.major}}.{{currentInstanceDetails.databaseVersion.minor}}.{{currentInstanceDetails.databaseVersion.build}}
          </v-list-tile-title>
          <v-list-tile-sub-title>Database Version</v-list-tile-sub-title>
        </v-list-tile-content>
      </v-list-tile>
      <v-list-tile>
        <v-list-tile-content>
          <v-list-tile-title>
              {{currentInstanceDetails.sites.length}}
          </v-list-tile-title>
          <v-list-tile-sub-title>Site Count</v-list-tile-sub-title>
        </v-list-tile-content>
      </v-list-tile>
      <v-list-tile>
        <v-list-tile-content>
          <v-list-tile-title>
              {{currentInstanceDetails.sites.length}}
          </v-list-tile-title>
          <v-list-tile-sub-title>Site Count</v-list-tile-sub-title>
        </v-list-tile-content>
      </v-list-tile>
    </template>
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
import { mapGetters, mapState } from 'vuex'
export default {
  props: {
    instance: {
      type: Object,
      required: true
    }
  },
  computed: {
    ...mapGetters('instances', [
      'getInstanceDisplayName',
      'isConnected'
    ]),
    ...mapState('instances',['currentInstanceDetails']),
    displayName: function() {
      const name = this.getInstanceDisplayName(this.instance.guid)
      return name
    }
  }
}
</script>
