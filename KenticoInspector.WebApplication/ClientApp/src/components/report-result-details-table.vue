<template>
  <div>
    <v-card-title>
      <h4 v-if="name">{{ name }}</h4>
      <v-spacer></v-spacer>
      <v-text-field v-if="rows.length > 1"
                    v-model="search"
                    label="Search"
                    single-line
                    hide-details></v-text-field>
    </v-card-title>

    <v-data-table :headers="headers"
                  :items="rows"
                  :search="search"
                  :rows-per-page-items="[10, 25, 100, { text: 'All', value:-1}]">
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
      },
      search: {
        type: String
      }
    },
    computed: {
      headers: function () {
        const isValid = this.rows && this.rows.length > 0 && this.rows[0]
        return isValid ? Object.keys(this.rows[0]).map(header => ({
          text: header,
          value: header
        })) : []
      }
    }
  }
</script>