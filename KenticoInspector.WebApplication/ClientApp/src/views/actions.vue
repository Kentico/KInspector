<template>
  <v-container>
    <v-layout
      row
      wrap
      class="glass-pane pa-3"
      >
      <v-flex xs12>
        <h1 class="display-2 mb-3">
         Actions
        </h1>
      </v-flex>

      <template v-if="isConnected">
        <v-flex xs12>
        Available Actions
        </v-flex>
      </template>

      <v-flex
        v-else
        xs12
        >
        <v-card
          color="error"
          >
          <v-card-text>
            Disconnected
          </v-card-text>
        </v-card>
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
import { mapActions, mapGetters } from 'vuex'

export default {
  components: {
  },
  computed: {
    ...mapGetters('instances', [
      'connectedInstanceDetails',
      'isConnected'
    ])
  },
  methods: {
    ...mapActions('reports', {
      getAllReports: 'getAll',
      resetFilterSettings: 'resetFilterSettings'
    }),
    initPage: function() {
      if(this.isConnected) {
                    this.getAllReports(this.connectedInstanceDetails.guid)
        this.resetFilterSettings({ majorVersion: this.connectedInstanceDetails.databaseVersion.major })
      }
    }
  },
  watch: {
    '$route': {
      handler: 'initPage',
      immediate: true
    }
  }
}
</script>
