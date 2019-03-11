<template>
  <v-menu offset-y>
    <template v-slot:activator="{ on }">
      <v-btn
        flat
        v-on="on"
        active-class="ignore"
        :color="color"
        >
        <div
          v-if="connected"
          class="text-xs-right caption"
          >
          <span class="grey--text text-lowercase">Server </span>
          <span class="white--text">{{connectedInstance.databaseConfiguration.serverName}}</span>
          <br>
          <span class="grey--text text-lowercase">Database </span>
          <span class="white--text">{{connectedInstance.databaseConfiguration.databaseName}}</span>
        </div>
        <span v-else>Disconnected</span>
        <v-icon
          right
          >
          {{icon}}
        </v-icon>
      </v-btn>
    </template>
    <v-card>
      <instance-details
        v-if="connected"
        :instance="connectedInstance">
        </instance-details>
      <v-card-actions>
        <v-btn
          v-if="!connected"
          to="/connect"
          block
          color="success"
          >
          Connect
        </v-btn>
        <v-btn
          v-else
          @click="disconnect()"
          block
          color="error"
          >
          Disconnect
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-menu>
</template>

<script>
import { mapActions, mapGetters } from 'vuex'

import InstanceDetails from './instance-details'

export default {
  components: {
    InstanceDetails
  },
  computed: {
    ...mapGetters([
      'connected',
      'connectedInstance',
      'connectedInstanceName',
    ]),
    color () {
      return this.connected ? 'success' : 'error'
    },
    status () {
      return this.connected ? `Server: ${this.connectedInstance.databaseConfiguration.serverName}<br>Database: ${this.connectedInstance.databaseConfiguration.databaseName}` : 'Disconnected'
    },
    icon () {
      return this.connected ? 'mdi-power-plug' : 'mdi-power-plug-off'
    }
  },
  methods: {
    ...mapActions([
      'clearInstanceConfiguration'
    ]),
    disconnect() {
      this.clearInstanceConfiguration()
      this.$router.push('/connect')
    }
  }
}
</script>
