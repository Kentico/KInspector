import api from '../../api'

const state = {
  items: {},
  currentInstanceDetails: null,
  upserting: false,
  upsertingError: null,
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
  async getAll ({ commit }) {
    commit('setItems', await api.getInstances())
  },

  async upsertItem ({ commit, dispatch }, instance) {
    commit('setUpserting',true)
    try {
      const newInstance = await api.upsertInstance(instance)
      dispatch('getAll')
      return newInstance
    } catch (error) {
      commit('setUpsertingError', error)
    }

    commit('setUpserting',false)
  },

  async deleteItem ({ dispatch }, guid) {
    await api.deleteInstance(guid)
    await dispatch('getAll')
  },

  async connect ({ commit }, guid) {
    commit('setConnecting',true)

    try {
      const instanceDetails = await api.getInstanceDetails(guid)
      commit('setCurrentInstanceDetails', instanceDetails)
    } catch (error) {
      commit('setConnectionError', error)
    }

    commit('setConnecting',false)
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

  setUpserting (state, status) {
    state.upserting = status
  },

  setUpsertingError (state, reason) {
    state.upsertingError = reason
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