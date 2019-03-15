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
          v-if="isConnected"
          class="text-xs-right caption"
          >
          <span class="grey--text text-lowercase">Server </span>
          <span class="white--text">{{connectedInstance.databaseSettings.server}}</span>
          <br>
          <span class="grey--text text-lowercase">Database </span>
          <span class="white--text">{{connectedInstance.databaseSettings.database}}</span>
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
        v-if="isConnected"
        :instance="connectedInstance">
        </instance-details>
      <v-card-actions>
        <v-btn
          v-if="!isConnected"
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
    ...mapGetters('instances',[
      'isConnected',
      'connectedInstance',
      'getInstanceDisplayName',
    ]),
    color () {
      return this.isConnected ? 'success' : 'error'
    },
    status () {
      return this.isConnected ? `Server: ${this.connectedInstance.databaseConfiguration.serverName}<br>Database: ${this.connectedInstance.databaseConfiguration.databaseName}` : 'Disconnected'
    },
    icon () {
      return this.isConnected ? 'mdi-power-plug' : 'mdi-power-plug-off'
    }
  },
  methods: {
    ...mapActions([
      'deselectItem'
    ]),
    disconnect() {
      this.deselectItem()
      this.$router.push('/connect')
    }
  }
}
</script>
