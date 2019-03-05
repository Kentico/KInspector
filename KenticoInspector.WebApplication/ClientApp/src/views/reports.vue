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

import ReportList from '../components/report-list'

export default {
  components: {
    ReportList
  },
  data: () => ({
    showIncompatible: false,
    version: "V12",
    selectedTags: [],
    reports: [
      {
        name: "Report has success",
        shortDescription: "This is a very short description highlighting the basics",
        longDescription: `
          <p>This is a longer description with a lot more details.</p>
          <p>There many be multiple paragraphs and <a href="https://docs.kentico.com/k12" target="_blank">links to documentation</a> for more information.</p>`,
        tags: ['content','unique'],
        results: {
          status: "success"
        },
        compatibility: ['V10','V11','V12']
      },
      {
        name: "Report has error",
        shortDescription: "This is a very short description highlighting the basics",
        longDescription: `
          <p>This is a longer description with a lot more details.</p>
          <p>There many be multiple paragraphs and <a href="https://docs.kentico.com/k12" target="_blank">links to documentation</a> for more information.</p>`,
        tags: ['content','settings'],
        results: {
          status: "error"
        },
        compatibility: ['V10','V11','V12']
      },
      {
        name: "Report has warning",
        shortDescription: "This is a very short description highlighting the basics",
        longDescription: `
          <p>This is a longer description with a lot more details.</p>
          <p>There many be multiple paragraphs and <a href="https://docs.kentico.com/k12" target="_blank">links to documentation</a> for more information.</p>`,
        tags: ['security','settings'],
        results: {
          status: "warning"
        },
        compatibility: ['V10','V11','V12']
      },
      {
        name: "Report run, only info",
        shortDescription: "This is a very short description highlighting the basics",
        longDescription: `
          <p>This is a longer description with a lot more details.</p>
          <p>There many be multiple paragraphs and <a href="https://docs.kentico.com/k12" target="_blank">links to documentation</a> for more information.</p>`,
        tags: ['security','web-config'],
        results: {
          status: "info"
        },
        compatibility: ['V10','V11','V12']
      },
      {
        name: "Report not run",
        shortDescription: "This is a very short description highlighting the basics",
        longDescription: `
          <p>This is a longer description with a lot more details.</p>
          <p>There many be multiple paragraphs and <a href="https://docs.kentico.com/k12" target="_blank">links to documentation</a> for more information.</p>`,
        tags: ['content','settings'],
        compatibility: ['V10','V11','V12']
      },
      {
        name: "Incompatible report",
        shortDescription: "This is a very short description highlighting the basics",
        longDescription: `
          <p>This is a longer description with a lot more details.</p>
          <p>There many be multiple paragraphs and <a href="https://docs.kentico.com/k12" target="_blank">links to documentation</a> for more information.</p>`,
        tags: ['content','settings'],
        compatibility: ['V10','V11']
      }
    ]
  }),
  computed: {
    tags: function () {
      const allTags = this.reports.reduce(getTagsFromReports,[])
      const uniqueTags = getUniqueTags(allTags)
      return uniqueTags
    },
    filteredReports: function() {
      return this.reports.filter(report => {
        const showForCompatibility = this.showIncompatible ? true : report.compatibility.includes(this.version)
        const showForTags = this.selectedTags.length === 0 ? true : report.tags.reduce((acc,cur) => {
          return acc || this.selectedTags.includes(cur)
        }, false)
        return showForCompatibility && showForTags;
      })
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
