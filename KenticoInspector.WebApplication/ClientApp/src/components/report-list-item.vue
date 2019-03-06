<template>
  <v-card class="mb-2" >
    <v-toolbar
      flat
      :color="status"
      dark
      >
      <v-toolbar-title>
            <v-avatar
              v-if="notTested || notCompatible"
              :color="notCompatible ? 'error darken-1' : 'warning lighten-1'"
              size="32"
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
      <v-btn icon @click="show = !show">
        <v-icon>{{ show ? 'keyboard_arrow_down' : 'keyboard_arrow_up' }}</v-icon>
      </v-btn>
    </v-toolbar>
    <v-card-text class="py-2">
      <v-container fluid class="pa-0">
        <v-layout
          align-center
          justify-center
          row
          fill-height
          class="pa-0"
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
          </v-flex>
        </v-layout>
      </v-container>
    </v-card-text>
    <v-slide-y-transition>
      <v-card-text v-show="show">
        <div v-html="report.longDescription" />
      </v-card-text>
    </v-slide-y-transition>
  </v-card>
</template>

<script>
export default {
  props: {
    report: {
      type: Object,
      required: true
    }
  },
  data: () => ({
    show: false
  }),
  computed: {
    hasResults: function () {
      return !!this.report.results && !!this.report.results.status
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

