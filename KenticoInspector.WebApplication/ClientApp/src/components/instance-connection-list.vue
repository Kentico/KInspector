<template>
  <v-container
    fluid
    grid-list-sm
    >
    <v-data-iterator
      :items="items"
      :rows-per-page-items="[10,20,{ text: 'All', value:-1}]"
      content-tag="v-layout"
      p
      row
      wrap
      >
      <template slot="item" slot-scope="props">
         <v-flex
          xs12
          sm6
          md4
          lg3
          >
          <instance-connection-list-item :item="props.item"></instance-connection-list-item>
         </v-flex>
      </template>
    </v-data-iterator>
  </v-container>
</template>

<script>
import InstanceConnectionListItem from './instance-connection-list-item'

export default {
  components: {
    InstanceConnectionListItem
  },
  props: {
    items: {
      type: Array,
      default: () => ([])
    }
  }
}
</script>

