<template>
  <div>
    <report-list-item
      v-for="report in reports"
      :key="report.name"
      :report="report"
      />
    <div v-if="reports.length === 0">
      No reports
    </div>
  </div>
</template>

<script>
import ReportListItem from '../components/report-list-item'

export default {
  components: {
    ReportListItem
  },
  props: {
    reports: {
      type: Array,
      default: () => ([])
    }
  }
}
</script>