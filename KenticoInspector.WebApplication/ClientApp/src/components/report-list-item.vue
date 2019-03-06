<template>
  <v-card class="mb-2" >
    <v-toolbar
      flat
      dense
      >
      <v-toolbar-title>
            <v-avatar
              v-if="notTested || notCompatible"
              :color="notCompatible ? 'error darken-1' : 'warning lighten-1'"
              size="30"
              tile
              class="elevation-4 "
              style="position: absolute; left: -16px"
              >
              <v-icon
                color="white"
                >
                {{ notCompatible ? "not_interested" : "warning" }}
              </v-icon>
            </v-avatar>
        {{ report.name }}
      </v-toolbar-title>
      <v-spacer />
      <v-btn icon :disabled="notCompatible">
        <v-icon>{{ hasResults ? 'replay' : 'play_arrow' }}</v-icon>
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
          pr-0
          pl-3
          py-1
          >
          <v-flex grow>
            {{ report.shortDescription }}
          </v-flex>
          <v-spacer></v-spacer>
          <v-flex shrink>
            <v-chip
              v-for="tag in report.tags"
              :key="tag"
              small
              >
              {{ tag }}
            </v-chip>
            <v-btn icon @click="showDescription = !showDescription">
              <v-icon>{{ showDescription ? 'expand_less' : 'expand_more' }}</v-icon>
            </v-btn>
          </v-flex>
        </v-layout>
    </v-card-text>

    <v-slide-y-transition>
      <v-card-text v-show="showDescription">
        <div v-html="report.longDescription" />
      </v-card-text>
    </v-slide-y-transition>

    <v-card-text v-if="hasResults" class="pa-0 subheading white--text">
      <v-layout
        align-center
        justify-center
        row
        fill-height
        pr-0
        pl-3
        py-1
        :class="status"
        >
        <v-flex grow>
          <div v-html="report.results.summary" />
        </v-flex>
        <v-spacer></v-spacer>
        <v-flex shrink>
          <v-btn icon @click="showResults = !showResults">
            <v-icon color="white">{{ showResults ? 'expand_less' : 'expand_more' }}</v-icon>
          </v-btn>
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

    notTested: function () {
      //TODO: reach out to get instance version instead of hard coded value
      return !this.report.compatible.includes('V12')
    },
    notCompatible: function () {
      //TODO: reach out to get instance version instead of hard coded value
      return this.report.notCompatible.includes('V12')
    }
  }
}
</script>

