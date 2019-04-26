import Vue from 'vue'
import api from '../../api'

const state = {
  items: [],
  filterSettings: {
    majorVersion: -1,
    showIncompatible: false,
    showUntested: false,
    taggedWith: []
  },
  reportResults: {},
  loadingItems: false
}

const getters = {
  filtered: (state) => {
    const items = state.items.filter(item => {
      const version = state.filterSettings.majorVersion
      const showIncompatible = state.filterSettings.showIncompatible
      const showUntested = state.filterSettings.showUntested
      const taggedWith = state.filterSettings.taggedWith

      const isCompatible = version > 0 && item.compatibleVersions.filter(x => x.major === version).length > 0
      const isIncompatible = item.incompatibleVersions.filter(x => x.major === version).length > 0
      const isUntested = !isCompatible && !isIncompatible

      const meetsCompatibilityFilters = isCompatible || (showIncompatible && isIncompatible) || (showUntested && isUntested)

      const meetsTagFilter = taggedWith.length == 0 || item.tags.some(t=>taggedWith.includes(t))

      return meetsCompatibilityFilters && meetsTagFilter
    })

    return items
  },

  getTags: (state) => {
    const allTags = state.items.reduce(getTagsFromReports, [])
    return getUniqueTags(allTags)
  },

  getReportResult: (state) => (codename, instanceGuid) => {
    const resultId = `${codename}-${instanceGuid}`
    const currentResult = state.reportResults[resultId]
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
    api.reportService.getReportResults({ codename, instanceGuid })
      .then(results => {
        const resultId = `${codename}-${instanceGuid}`
        commit('setItemResults', { resultId, loading: false, results })
      })
  }
}

const mutations = {
  setItems (state, items) {
    state.items = items
  },
  setItemResults (state, { resultId, loading, results }) {
    Vue.set(state.reportResults, resultId, { loading, results })
  },
}

export default {
  namespaced: true,
  state,
  getters,
  actions,
  mutations
}

function getTagsFromReports (allTags, report) {
  allTags.push(...report.tags)
  return allTags
}

function getUniqueTags (allTags) {
  return [...new Set(allTags)]
}