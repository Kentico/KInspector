import '@babel/polyfill'
import Vue from 'vue'
import VueShowdown from 'vue-showdown'
import './plugins/vuetify'
import App from './app.vue'
import router from './router'
import store from './store'
import 'roboto-fontface/css/roboto/roboto-fontface.css'
import '@mdi/font/css/materialdesignicons.css'

Vue.config.productionTip = false

Vue.use(VueShowdown, {
    options: {
        openLinksInNewWindow: true
    }
})

new Vue({
  router,
  store,
  render: h => h(App)
}).$mount('#app')
