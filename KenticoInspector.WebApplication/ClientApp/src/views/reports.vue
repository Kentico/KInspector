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

      <template v-if="connected">
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
import { mapGetters } from 'vuex'
import { reportService } from '../api/report-service'
import ReportList from '../components/report-list'

export default {
  components: {
    ReportList
  },
  data: () => ({
    showIncompatible: false,
    showUntested: false,
    version: "V12",
    selectedTags: [],
    reports: []
  }),
  computed: {
    ...mapGetters(['connected']),
    tags: function () {
      const allTags = this.reports.reduce(getTagsFromReports,[])
      const uniqueTags = getUniqueTags(allTags)
      return uniqueTags
    },
    filteredReports: function() {
      return this.reports.filter(report => {
        const isCompatible = report.compatible.includes(this.version)
        const isIncompatible = report.notCompatible.includes(this.version)
        const isUntested = !isCompatible && !isIncompatible

        const hasSelectedTags = this.selectedTags.length > 0
        const meetsTagRequirements = hasSelectedTags ? this.hasSelectedTag(report) : true

        if(meetsTagRequirements) {
          return isCompatible
            || (this.showUntested && isUntested)
            || (this.showIncompatible && isIncompatible)
        }

        return false
      })
    }
  },
  methods: {
    getReports: function() {
      reportService.getReports().then(reports => {
        this.reports = reports
      })
    },
    hasSelectedTag(report) {
      return report.tags.reduce(
        (acc,cur) => {
          return acc || this.selectedTags.includes(cur)
        }, false)
    }
  },
  watch: {
    '$route': {
      handler: 'getReports',
      immediate: true
    }
  }
}

function getTagsFromReports(allTags, report) {
  allTags.push(...report.tags)
  return allTags
}

function getUniqueTags(allTags) {
  return [...new Set(allTags)]
}
</script>
