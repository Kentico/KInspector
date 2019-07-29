import Vue from 'vue'
import Vuetify from 'vuetify'
import colors from 'vuetify/es5/util/colors'
import 'vuetify/dist/vuetify.min.css'

Vue.use(Vuetify, {
  iconfont: 'mdi',
  theme: {
    primary: "#f05a22",
    secondary: colors.grey.darken3,
    accent: colors.deepOrange.lighten4,
    error: colors.red.lighten2,
    info: colors.blue.lighten3,
    success: colors.green.lighten1,
    warning: colors.amber.lighten1
  },
})
