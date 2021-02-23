import Vue from 'vue'
import Vuex from 'vuex'
import instances from './modules/instances'
import reports from './modules/reports'
import versions from './modules/versions'

Vue.use(Vuex)

const debug = process.env.NODE_ENV !== 'production'

export default new Vuex.Store({
  modules: {
    instances,
        reports,
        versions
  },
  strict: debug
})