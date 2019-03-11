import Vue from 'vue'
import Vuex from 'vuex'

import api from './api'

Vue.use(Vuex)

export default new Vuex.Store({
  state: {
    instanceConfigurations: [],
    curentInstanceConfiguration: null
  },
  mutations: {
    setInstanceConfigurations (state, instanceConfigurations) {
      state.instanceConfigurations = instanceConfigurations
    },

    setCurrentInstanceConfiguration (state, instanceConfigurationGuid) {
      state.curentInstanceConfiguration = instanceConfigurationGuid
    }
  },
  actions: {
    getInstanceConfigurations: ({ commit }) => {
      api.getInstanceConfigurations()
        .then(instanceConfigurations => {
          commit('setInstanceConfigurations', instanceConfigurations)
        })
    },

    upsertInstanceConfiguration: ({ commit, dispatch }, instanceConfiguration) => {
      api.upsertInstanceConfiguration(instanceConfiguration)
        .then(guid=>{
          dispatch('getInstanceConfigurations')
          commit('setCurrentInstanceConfiguration', guid)
        })
    },

    deleteInstanceConfiguration: ({ dispatch }, guid) => {
      api.deleteInstanceConfiguration(guid)
        .then(()=>{
          dispatch('getInstanceConfigurations')
        })
    },

    selectInstanceConfiguration: ({ commit }, guid) => {
      commit('setCurrentInstanceConfiguration', guid)
    },

    clearInstanceConfiguration: ({ commit }) => {
      commit('setCurrentInstanceConfiguration', null)
    },
  },
  getters: {
    connected: state => {
      return !!state.curentInstanceConfiguration
    },

    connectedInstance: (state, getters) => {
      if(getters.connected) {
        return state.instanceConfigurations.find(i=>i.guid == state.curentInstanceConfiguration)
      }
    },
  }
})
