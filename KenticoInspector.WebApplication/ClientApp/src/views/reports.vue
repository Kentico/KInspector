<template>
  <v-container>
    <v-layout
      row
      wrap
      class="glass-pane pa-3"
      >
      <v-flex xs12>
        <h1 class="display-2 mb-3">
         Reports
        </h1>
      </v-flex>

      <template v-if="isConnected">
        <v-flex xs12>
          <v-select
              v-model="selectedTags"
              :items="tags"
              label="Show reports by tag(s)"
              clearable
              small-chips
              multiple
              solo
              hide-details
            >
          </v-select>
        </v-flex>

        <v-flex sm6>
          <v-switch
            v-model="showUntested"
            label="Show untested reports"
            color="red"
            >
          </v-switch>
        </v-flex>

        <v-flex sm6>
          <v-switch
            v-model="showIncompatible"
            label="Show incompatible reports"
            color="red"
            >
          </v-switch>
        </v-flex>

        <v-flex xs12>
          <report-list :reports="filteredReports" />
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
import ReportList from '../components/report-list'

export default {
  components: {
    ReportList
  },
  data: () => ({
    showIncompatible: false,
    showUntested: false,
    selectedTags: [],
    allReports: [],
  }),
  computed: {
    ...mapGetters('instances', [
      'connectedInstanceDetails',
      'isConnected'
    ]),
    ...mapGetters('reports', {
      tags: 'getTags',
      getFilteredReports: 'getFiltered'
    }),
    filteredReports: function () {
      return this.getFilteredReports({version: this.connectedInstanceDetails.databaseVersion.major, showIncompatible: this.showIncompatible, showUntested: this.showUntested })
    }
  },
  methods: {
    ...mapActions('reports', {
      getAllReports: 'getAll'
    }),
    hasSelectedTag(report) {
      return report.tags.reduce(
        (acc,cur) => {
          return acc || this.selectedTags.includes(cur)
        }, false)
    },
    getReports: function() {
      this.getAllReports()
    }
  },
  watch: {
    '$route': {
      handler: 'getReports',
      immediate: true
    }
  }
}
</script>
