import Vue from 'vue'
import Router from 'vue-router'
import Home from './views/home.vue'

Vue.use(Router)

export default new Router({
  mode: 'history',
  base: process.env.BASE_URL,
  routes: [
    {
      path: '/',
      name: 'home',
      component: Home
    },
    {
      path: '/connect',
      name: 'connect',
      // route level code-splitting
      // this generates a separate chunk (instance-connect.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () => import(/* webpackChunkName: "instance-connect" */ './views/instance-connect.vue')
    },
    {
      path: '/reports',
      name: 'reports',
      // route level code-splitting
      // this generates a separate chunk (instance-connect.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () => import(/* webpackChunkName: "reports" */ './views/reports.vue')
    }
  ]
})
