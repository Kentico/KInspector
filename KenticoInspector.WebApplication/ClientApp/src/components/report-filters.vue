<template>
  <v-container pa-0 grid-list-sm>
    <v-layout>
      <v-flex xs12>
        <v-card>
          <v-container grid-list-md pa-3>
            <v-layout pa-0 row wrap>
              <v-flex xs12>
                <v-select
                  v-model="taggedWith"
                    :items="tags"
                    label="Show reports by tag(s)"
                    clearable
                    small-chips
                    multiple
                    hide-details
                  >
                </v-select>
              </v-flex>

              <v-flex xs12 sm6 md4>
                <v-switch
                  v-model="showUntested"
                  label="Show untested reports"
                  color="warning"
                  >
                </v-switch>
              </v-flex>

              <v-flex xs12 sm6 md4>
                <v-switch
                  v-model="showIncompatible"
                  label="Show incompatible reports"
                  color="error"
                  >
                </v-switch>
              </v-flex>
            </v-layout>
          </v-container>
        </v-card>
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
import { mapGetters, mapState, mapMutations } from 'vuex';

export default {
  computed: {
    ...mapGetters('reports', {
      tags: 'getTags'
    }),
    ...mapState('reports', ['filterSettings']),
    showIncompatible: {
      set(showIncompatible) {
        this.setFilterSetting({name: 'showIncompatible', value: showIncompatible})
      },
      get() {
        return this.filterSettings.showIncompatible
      }
    },
    showUntested: {
      set(showUntested) {
        this.setFilterSetting({name: 'showUntested', value: showUntested})
      },
      get() {
        return this.filterSettings.showUntested
      }
    },
    taggedWith: {
      set(taggedWith) {
        this.setFilterSetting({name: 'taggedWith', value: taggedWith})
      },
      get() {
        return this.filterSettings.taggedWith
      }
    }
  },
  methods: {
    ...mapMutations('reports', ['setFilterSetting'])
  }
}
</script>
