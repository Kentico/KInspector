<template>
  <v-container>
    <v-layout>
      <v-flex xs12>
        <h1 class="display-2 mb-3">
          Connect <span class="display-1 font-weight-light">to a Kentico EMS Instance</span>
        </h1>
      </v-flex>
    </v-layout>
    <v-layout justify-center>
      <v-flex xs12>
        <v-tabs
          centered
          color="primary"
          dark
          v-model="tab"
          >
          <v-tabs-slider></v-tabs-slider>
          <v-tab key="new">
            New Connection
          </v-tab>
          <v-tab key="recent">
            Recent Connections
          </v-tab>
          <v-tab key="iis">
            IIS Connection
          </v-tab>
        </v-tabs>
        <v-tabs-items
          v-model="tab"
          >
          <v-tab-item key="new">
            <instance-connect-form-manual />
          </v-tab-item>
          <v-tab-item key="recent">
            <card-coming-soon />
          </v-tab-item>
          <v-tab-item key="iis">
            <card-coming-soon />
          </v-tab-item>
        </v-tabs-items>

      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
import CardComingSoon from '../components/card-coming-soon'
import InstanceConnectFormManual from '../components/instance-connect-form-manual'

export default {
  components: {
    CardComingSoon,
    InstanceConnectFormManual
  },
  data: () => ({
    tab: null
  })
}
</script>

