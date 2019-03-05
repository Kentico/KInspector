<template>
  <v-container>
    <v-layout>
      <v-flex xs12>
        <h1 class="display-4 mb-3">
          Connect <span class="grey--text display-3 font-weight-light">to a Kentico EMS Instance</span>
        </h1>
      </v-flex>
    </v-layout>
    <v-layout justify-center>
      <v-flex xs12>
        <instance-connect-form-manual />
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
import InstanceConnectFormManual from '../components/instance-connect-form-manual'

export default {
  components: {
    InstanceConnectFormManual
  }
}
</script>

