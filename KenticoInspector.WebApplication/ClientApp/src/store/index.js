import Vue from 'vue'
import Vuex from 'vuex'
import actions from './modules/actions'
import instances from './modules/instances'
import reports from './modules/reports'

Vue.use(Vuex)

const debug = process.env.NODE_ENV !== 'production'

export default new Vuex.Store({
  modules: {
    actions,
    instances,
    reports
  },
  strict: debug
})