import api from '../../api'

const state = {
    coreVersion: ""
}

const getters = {
    coreVersion: state => {
        return state.coreVersion;
    }
}

const actions = {
    async getCoreVersion({ commit }) {
        commit('setCoreVersion', await api.getCoreVersion())
    }
}

const mutations = {
    setCoreVersion(state, coreVersion) {
        state.coreVersion = coreVersion
    }
}

export default {
    namespaced: true,
    state,
    getters,
    actions,
    mutations
}