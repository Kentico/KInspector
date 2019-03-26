import api from '../../api'

const state = {
  items: {},
  selectedItemGuid: null,
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

  upsertItem: ({ commit, dispatch }, instance) => {
    api.upsertInstance(instance)
      .then(guid=>{
        dispatch('getAll')
        commit('setSelectedItemGuid', guid)
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
    commit('setSelectedItemGuid', null)
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

  setSelectedItemGuid (state, guid) {
    state.selectedItemGuid = guid
  }
}

export default {
  namespaced: true,
  state,
  getters,
  actions,
  mutations
}