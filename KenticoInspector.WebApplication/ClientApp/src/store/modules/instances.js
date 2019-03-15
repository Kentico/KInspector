import api from '../../api'

const state = {
  items: {},
  selectedItemGuid: null
}

const getters = {
  isConnected: state => {
    return !!state.selectedItemGuid
  },

  connectedInstance: (state, getters) => {
    return getters.isConnected ? state.items[state.selectedItemGuid] : null
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

  selectItem: ({ commit }, guid) => {
    commit('setSelectedItemGuid', guid)
  },

  deselectItem: ({ commit }) => {
    commit('setSelectedItemGuid', null)
  },
}

const mutations = {
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