<template>
  <div>
    <vue-showdown
      v-if="type === 'String'"
      key="string-result"
      :markdown="data"
      >
    </vue-showdown>
    <div
      v-else-if="type === 'StringList'"
      key="string-list-result"
      >
      <vue-showdown
        v-for="(item, index) in data"
        :key="`result-${index}`"
        :markdown="item"
        >
      </vue-showdown>
    </div>
    <div
      v-else-if="type === 'Table'"
      key="table-result"
      >
      <report-result-details-table
        :name="data.name"
        :rows="data.rows"
        ></report-result-details-table>
    </div>
    <div
      v-else-if="type === 'TableList'"
      key="table-list-result"
      >
      <report-result-details-table
        v-for="(item, index) in data"
        :key="`result-${index}`"
        :name="item.name"
        :rows="item.rows"
        ></report-result-details-table>
    </div>

  </div>
</template>

<script>
import ReportResultDetailsTable from "./report-result-details-table"
export default {
  components: {
    ReportResultDetailsTable
  },
  props: {
    type: {
      type: String,
      required: true
    },
    data: {
      required: true
    }
  }
}
</script>