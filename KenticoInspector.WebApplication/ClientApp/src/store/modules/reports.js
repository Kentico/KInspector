import Vue from 'vue'
import api from '../../api'

const state = {
  items: {},
  reportResults: {},
  loadingItems: false
}

const getters = {
  getReportResult: (state) => (codename) => {
    const currentResult = state.reportResults[codename]
    return currentResult ? currentResult : {
      loading: false,
      results: null
    }
  }
}

const actions = {
  getAll: ({ commit }) => {
    api.reportService.getReports()
      .then(items => {
        commit('setItems', items)
      })
  },
  runReport: ({ commit }, { codename, instanceGuid }) => {
    commit('setItemResults', { codename, loading: true })
    api.reportService.getReportResults({codename, instanceGuid})
      .then(results =>{
        commit('setItemResults', { codename, loading: false, results })
      })
    }
}

const mutations = {
  setItems (state, items) {
    state.items = items
  },
  setItemResults (state, { codename, loading, results }) {
    Vue.set(state.reportResults, codename, { loading, results })
  },
}

export default {
  namespaced: true,
  state,
  getters,
  actions,
  mutations
}