<template>
  <v-card>
    <v-card-title>{{ item.administrationConfiguration.uri }}</v-card-title>
    <v-divider></v-divider>
    <v-card-actions>
      <v-btn
        color="success"
        @click="selectInstanceConfiguration(item.guid)"
        >
        Connect
      </v-btn>
      <v-btn
        color="error"
        @click="deleteInstanceConfiguration(item.guid)"
        >
        Delete
      </v-btn>
    </v-card-actions>
  </v-card>
</template>

<script>
import { mapActions } from 'vuex'

export default {
  props: {
    item: {
      type: Object,
      required: true
    }
  },
  methods: {
    ...mapActions([
      'deleteInstanceConfiguration',
      'selectInstanceConfiguration'
    ]),
  }
}
</script>

