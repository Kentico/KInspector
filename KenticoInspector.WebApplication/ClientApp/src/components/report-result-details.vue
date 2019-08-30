<template>
  <div>
    <div v-for="(item, index) in data"
         :key="`result-${index}`">
      <div v-if="item.hasOwnProperty('string')">

        <report-result-details-string :label="item.label"
                                      :string="item.string">
        </report-result-details-string>
      </div>
      <div v-else-if="item.hasOwnProperty('rows')">
        <report-result-details-table :label="item.label"
                                     :rows="item.rows">
        </report-result-details-table>
      </div>
    </div>
  </div>
</template>

<script>
  import ReportResultDetailsTable from "./report-result-details-table"
  import ReportResultDetailsString from "./report-result-details-string"
  export default {
    components: {
      ReportResultDetailsTable,
      ReportResultDetailsString
    },
    props: {
      data: {
        required: true
      }
    }
  }
</script>