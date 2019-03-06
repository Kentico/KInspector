<template>
  <v-container>
    <v-layout wrap>
      <v-flex xs12>
        <h1 class="display-2 mb-3">
         Reports
        </h1>
      </v-flex>

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
          ></v-select>
        <v-switch
          v-model="showIncompatible"
          label="Show incompatible reports"
          color="red"
          ></v-switch>
      </v-flex>

      <v-flex xs12>
        <report-list :reports="filteredReports" />
      </v-flex>

    </v-layout>
  </v-container>
</template>

<script>
import { reportService } from '../api/report-service'
import ReportList from '../components/report-list'

export default {
  components: {
    ReportList
  },
  data: () => ({
    showIncompatible: false,
    version: "V12",
    selectedTags: [],
    reports: []
  }),
  computed: {
    tags: function () {
      const allTags = this.reports.reduce(getTagsFromReports,[])
      const uniqueTags = getUniqueTags(allTags)
      return uniqueTags
    },
    filteredReports: function() {
      return this.reports.filter(report => {
        const showForCompatibility = this.showIncompatible ? true : report.compatible.includes(this.version)
        const showForTags = this.selectedTags.length === 0 ? true : report.tags.reduce((acc,cur) => {
          return acc || this.selectedTags.includes(cur)
        }, false)
        return showForCompatibility && showForTags;
      })
    }
  },
  methods: {
    getReports: function() {
      reportService.getReports().then(reports => {
        this.reports = reports
      })
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
