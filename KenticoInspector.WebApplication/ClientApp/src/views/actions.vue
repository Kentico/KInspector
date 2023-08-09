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

          <v-card
          color="light-blue lighten-4"
          class="mb-3"
          >
          <v-card-title class="pb-0">
            <span class="title">
              <v-icon color="black">mdi-information</v-icon>
              Data Modification Notice</span>
          </v-card-title>
          <v-card-text class="title font-weight-light">
            Actions modify data when options are provided! It is not recommended to run this in production.
          </v-card-text>
        </v-card>
      </v-flex>

      <template v-if="isConnected">
        <action-filters />
        <v-flex xs12>
        <action-list :actions="filtered" />
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
import  ActionList from '../components/action-list'
import ActionFilters from '../components/action-filters'

export default {
  components: {
    ActionList,
    ActionFilters
  },
  computed: {
    ...mapGetters('instances', [
      'connectedInstanceDetails',
      'isConnected'
    ]),
    ...mapGetters('actions', {
      tags: 'getTags',
      filtered: 'filtered'
    })
  },
  methods: {
    ...mapActions('actions', {
      getAll: 'getAll',
      resetFilterSettings: 'resetFilterSettings'
    }),
    initPage: function() {
      if(this.isConnected) {
        this.getAll(this.connectedInstanceDetails.guid)
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
