<template>
  <v-card>
    <instance-details :instance="item"></instance-details>
    <v-divider></v-divider>
    <v-card-actions>
      <v-btn
        color="success"
        @click="connectToInstance(item.guid)"
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
    ...mapActions('instances',{
      deleteInstance: 'deleteItem',
      connectInstance: 'connect'
    }),
    connectToInstance(guid) {
      this.connectInstance(guid)
        .then(() => {
          this.$router.push('/reports')
        })
    }
  }
}
</script>

