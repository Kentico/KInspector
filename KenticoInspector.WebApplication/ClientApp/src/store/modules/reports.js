import api from '../../api'

const state = {
  items: {},
  resultItems: {},
  loadingItems: false
}

const getters = {
}

const actions = {
  getAll: ({ commit }) => {
    api.reportService.getReports()
      .then(items => {
        commit('setItems', items)
      })
  },
}

const mutations = {
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