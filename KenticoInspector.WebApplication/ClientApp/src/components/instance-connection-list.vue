<template>
  <v-container
    fluid
    grid-list-lg
    px-0
    >
    <v-data-iterator
      :items="items"
      :rows-per-page-items="[10,20,{ text: 'All', value:-1}]"
      content-tag="v-layout"
      row
      wrap
      pa-0
      >
      <template slot="item" slot-scope="props">
          <v-flex
          sm12
          md6
          lg4
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

