<template>
  <v-card class="mt-4" >
    <v-toolbar
      flat
      dense
      >
      <v-toolbar-title>
        {{ report.name }}

        <v-chip
          v-if="notTested"
          color="amber"
          label
          small
          >
          Untested
        </v-chip>

        <v-chip
          v-if="notCompatible"
          color="red darken-1"
          dark
          label
          small
          >
          Incompatible
        </v-chip>

      </v-toolbar-title>
      <v-spacer />
      <v-chip
              v-for="tag in report.tags"
              :key="tag"
              small
              disabled
              text-color="black"
              >
              {{ tag }}
            </v-chip>
      <v-btn icon :disabled="notCompatible">
        <v-icon>{{ hasResults ? 'mdi-refresh' : 'mdi-play' }}</v-icon>
      </v-btn>
    </v-toolbar>

    <v-card-text
      class="pa-0"
      >
        <v-layout
          align-center
          justify-center
          row
          fill-height
          px-3
          py-2
          @click="showDescription = !showDescription"
          v-ripple="{ class: `grey--text` }"
          >
          <v-flex>
            {{ report.shortDescription }}
          </v-flex>
          <v-spacer></v-spacer>
          <v-flex shrink class="hidden-xs-only">

          </v-flex>
          <v-flex shrink>
              <v-icon>{{ showDescription ? 'mdi-chevron-up' : 'mdi-chevron-down' }}</v-icon>
          </v-flex>
        </v-layout>
    </v-card-text>

    <v-divider v-show="showDescription"></v-divider>

    <v-slide-y-transition>
      <v-card-text v-show="showDescription">
        <div v-html="report.longDescription" />
      </v-card-text>
    </v-slide-y-transition>

    <v-card-text v-if="hasResults" class="pa-0 subheading">
      <v-layout
        align-center
        justify-center
        row
        fill-height
        px-3
        py-2
        :class="status"
         @click="showResults = !showResults"
         v-ripple
        >
        <v-flex style="height: 24px">
          <v-icon
            :color="resultIconColor"
            class="pr-1"
            >
            {{ resultIcon }}
          </v-icon>
          <div
            v-html="report.results.summary"
            class="d-inline-block"
            style="position: relative;top: -1px;"
            >
          </div>
        </v-flex>
        <v-spacer></v-spacer>
        <v-flex shrink>
          <v-icon>{{ showResults ? 'mdi-chevron-up' : 'mdi-chevron-down' }}</v-icon>
        </v-flex>
      </v-layout>
    </v-card-text>

    <v-slide-y-transition>
      <v-card-text v-if="showResults && hasResults">
        <report-result-details
          :type="report.results.type"
          :data="report.results.data"
          >
        </report-result-details>
      </v-card-text>
    </v-slide-y-transition>

  </v-card>
</template>

<script>
import ReportResultDetails from "./report-result-details"
export default {
  components: {
    ReportResultDetails
  },
  props: {
    report: {
      type: Object,
      required: true
    }
  },
  data: () => ({
    showDescription: false,
    showResults: false
  }),
  computed: {
    hasResults: function () {
      return !!this.report.results
    },
    status: function() {
      return this.hasResults ? this.report.results.status : ''
    },

    statusDark: function() {
      return this.status == 'error' || this.status == "info"
    },

    notTested: function () {
      //TODO: reach out to get instance version instead of hard coded value
      return !this.report.compatible.includes('V12') && !this.notCompatible
    },
    notCompatible: function () {
      //TODO: reach out to get instance version instead of hard coded value
      return this.report.notCompatible.includes('V12')
    },
    resultIcon: function () {
      let icon = ""
      switch (this.report.results.status) {
        case "success":
          icon = "mdi-checkbox-marked-circle"
          break
        case "info":
          icon = "mdi-information"
          break
        case "warning":
          icon = "mdi-alert"
          break
        case "error":
          icon = "mdi-alert-octagon"
          break
      }

      return icon
    },
    resultIconColor: function () {
      return `${this.report.results.status} darken-3`
    }
  }
}
</script>

