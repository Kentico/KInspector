<template>
    <v-btn
      flat
      to="/connect"
      active-class="ignore"
      :color="color"
      >
      <v-icon
        left
        >
        {{icon}}
      </v-icon>
      {{status}}
    </v-btn>
</template>

<script>
import { mapGetters } from 'vuex'
export default {
  computed: {
    ...mapGetters([
      'connected',
      'connectedInstance',
    ]),
    color () {
      return this.connected ? '' : 'error'
    },
    status () {
      return this.connected ? `Connected to ${this.connectedInstanceName}` : 'Disconnected'
    },
    icon () {
      return this.connected ? 'mdi-power-plug' : 'mdi-power-plug-off'
    },
    connectedInstanceName () {
      return !!this.connectedInstance ? this.connectedInstance.administrationConfiguration.uri : 'UNKNOWN'
    }
  }
}
</script>
