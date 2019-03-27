import api from '../../api'

const state = {
  items: {},
  currentInstanceDetails: null,
  connecting: false,
  connectionError: null
}

const getters = {
  isConnected: state => {
    return !!state.currentInstanceDetails
  },

  connectedInstance: (state, getters) => {
    return getters.isConnected ? state.items[state.currentInstanceDetails.guid] : null
  },

  connectedInstanceDetails: (state, getters) => {
    return getters.isConnected ? state.currentInstanceDetails : null
  },

  getInstanceDisplayName: (state) => (guid) => {
    const name = state.items[guid].name
    return name ? name : "(UNNAMED)"
  }
}

const actions = {
  getAll: ({ commit }) => {
    api.getInstances()
      .then(instances => {
        commit('setItems', instances)
      })
  },

  upsertItem: ({ dispatch }, instance) => {
    api.upsertInstance(instance)
      .then(()=>{
        dispatch('getAll')
      })
  },

  deleteItem: ({ dispatch }, guid) => {
    api.deleteInstance(guid)
      .then(()=>{
        dispatch('getAll')
      })
  },

  connect: ({ commit }, guid) => {
    return new Promise((resolve) => {
      commit('setConnecting',true)
      api.getInstanceDetails(guid)
        .then(instanceDetails => {
          commit('setCurrentInstanceDetails', instanceDetails)
          commit('setConnecting',false)
          resolve()
        })
        .catch(reason => {
          commit('setConnectionError', reason)
        })
    })
  },

  cancelConnecting: ({ commit }) => {
    commit('setConnecting', false)
    commit('setConnectionError', null)
  },

  disconnect: ({ commit }) => {
    commit('setCurrentInstanceDetails', null)
  },
}

const mutations = {
  setConnecting (state, status) {
    state.connecting = status
  },

  setConnectionError (state, reason) {
    state.connectionError = reason
  },

  setCurrentInstanceDetails (state, instanceDetails) {
    state.currentInstanceDetails = instanceDetails
  },

  setItems (state, items) {
    state.items = items
  },
}

export default {
  namespaced: true,
  state,
  getters,
  actions,
  mutations
}