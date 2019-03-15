import Vue from 'vue'
import Vuex from 'vuex'

import api from './api'

Vue.use(Vuex)

export default new Vuex.Store({
  state: {
    instances: [],
    connectedInstanceGuid: null
  },
  mutations: {
    setInstances (state, instances) {
      state.instances = instances
    },

    setCurrentInstanceGuid (state, guid) {
      state.connectedInstanceGuid = guid
    }
  },
  actions: {
    getInstances: ({ commit }) => {
      api.getInstances()
        .then(instances => {
          commit('setInstances', instances)
        })
    },

    upsertInstance: ({ commit, dispatch }, instance) => {
      api.upsertInstance(instance)
        .then(guid=>{
          dispatch('getInstances')
          commit('setCurrentInstanceGuid', guid)
        })
    },

    deleteInstance: ({ dispatch }, guid) => {
      api.deleteInstance(guid)
        .then(()=>{
          dispatch('getInstances')
        })
    },

    selectInstance: ({ commit }, guid) => {
      commit('setCurrentInstanceGuid', guid)
    },

    deselectInstance: ({ commit }) => {
      commit('setCurrentInstanceGuid', null)
    },
  },
  getters: {
    isConnected: state => {
      return !!state.connectedInstanceGuid
    },

    connectedInstance: (state, getters) => {
      return getters.isConnected ? state.instances[state.connectedInstanceGuid] : null
    },

    getInstanceDisplayName: (state) => (guid) => {
      const name = state.instances[guid].name
      return name ? name : "(Unnamed)"
    }
  }
})
