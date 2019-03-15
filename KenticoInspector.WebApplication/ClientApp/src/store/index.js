import Vue from 'vue'
import Vuex from 'vuex'
import instances from './modules/instances'

Vue.use(Vuex)

const debug = process.env.NODE_ENV !== 'production'

export default new Vuex.Store({
  modules: {
    instances
  },
  strict: debug
})