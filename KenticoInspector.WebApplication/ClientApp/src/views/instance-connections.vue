<template>
  <div>
    <v-container>
      <v-layout
        row
        wrap
        class="glass-pane pa-3"
        >
        <v-flex xs12>
          <h1 class="display-2 mb-3">
            Connect <span class="display-1 font-weight-light">to a Kentico EMS Instance</span>
          </h1>
        </v-flex>

        <v-flex justify-center xs12>

          <v-tabs
            centered
            color="primary"
            dark
            v-model="tab"
            >
            <v-tabs-slider></v-tabs-slider>
            <v-tab key="recent">
              Saved Connections
            </v-tab>
            <v-tab key="new">
              New Connection
            </v-tab>
          </v-tabs>

          <v-tabs-items
            v-model="tab"
            >

            <v-tab-item key="recent">
              <instance-connection-list />
            </v-tab-item>

            <v-tab-item key="new">
                <v-layout>
                  <v-flex>
                    <instance-connect-form-manual />
                  </v-flex>
                </v-layout>
            </v-tab-item>

          </v-tabs-items>

        </v-flex>
      </v-layout>
    </v-container>
    <modal-instance-connecting></modal-instance-connecting>
  </div>
</template>

<script>
import InstanceConnectFormManual from '../components/instance-connect-form-manual'
import InstanceConnectionList from '../components/instance-connection-list'
import ModalInstanceConnecting from '../components/modal-instance-connecting'

export default {
  components: {
    InstanceConnectFormManual,
    InstanceConnectionList,
    ModalInstanceConnecting
  },
  data: () => ({
    tab: null
  }),
}
</script>

