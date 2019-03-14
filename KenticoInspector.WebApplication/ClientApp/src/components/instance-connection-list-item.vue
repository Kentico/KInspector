<template>
  <v-card>
    <instance-details :instance="item"></instance-details>
    <v-divider></v-divider>
    <v-card-actions>
      <v-btn
        color="success"
        @click="connect(item.guid)"
        >
        Connect
      </v-btn>
      <v-btn
        color="error"
        @click="deleteInstance(item.guid)"
        >
        Delete
      </v-btn>
    </v-card-actions>
  </v-card>
</template>

<script>
import { mapActions } from 'vuex'
import InstanceDetails from './instance-details'

export default {
  components: {
    InstanceDetails
  },
  props: {
    item: {
      type: Object,
      required: true
    }
  },
  methods: {
    ...mapActions([
      'deleteInstance',
      'selectInstance'
    ]),
    connect(guid) {
      this.selectInstance(guid)
      this.$router.push('/reports')
    }
  }
}
</script>

