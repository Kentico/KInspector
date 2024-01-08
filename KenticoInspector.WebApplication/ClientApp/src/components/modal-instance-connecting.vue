<template>
  <v-dialog
      v-model="connecting"
      persistent
      width="300"
    >
      <v-card
        color="primary"
        dark
      >
        <v-card-text v-if="!connectionError">
          Getting instance details
          <v-progress-linear
            indeterminate
            color="white"
            class="mb-0"
          ></v-progress-linear>
        </v-card-text>
        <v-card-text v-else>
          {{ connectionError.message }}
        </v-card-text>
        <v-card-actions>
          <v-btn
            @click="cancelConnecting"
            >
            Cancel
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
</template>

<script>
import { mapActions, mapState } from 'vuex'

export default {
  computed: {
    ...mapState('instances',[
      'connecting',
      'connectionError'
      ])
  },
  methods: {
    ...mapActions('instances', ['cancelConnecting'])
  }
}
</script>
