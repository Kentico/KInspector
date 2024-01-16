<template>
  <div>
   <h4 v-if="name">{{ name }}</h4>
   <v-data-table
    :headers="headers"
    :items="rows"

    :rows-per-page-items="[10, 25, 100, { text: 'All', value:-1}]"
    >
    <template slot="items" slot-scope="props">
      <td v-for="(header, index) in headers" :key="`header-${index}`">
        {{ props.item[header.value] }}
      </td>
    </template>
   </v-data-table>
  </div>
</template>

<script>
export default {
  props: {
    name: {
      type: String,
      default: ""
    },
    rows: {
      type: Array,
      default: () => ([])
    }
  },
  computed: {
    headers: function () {
      const isValid = this.rows && this.rows.length > 0 && this.rows[0]
      return isValid ? Object.keys(this.rows[0]).map(header=>({
        text: header,
        value: header
      })) : []
    }
  }
}
</script>