import CoreListViewVue from '@/views/CoreListView.vue'
import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import LoginView from '../views/LoginView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/home',
      name: 'home',
      component: HomeView
    },
    {
      path: '/login',
      name: 'login',
      component: LoginView
    },
    {
      path: '/host',
      name: 'host',
      component: () => import('../views/HostView.vue')
    },
    {
      path: '/core',
      name: 'core',
      component: CoreListViewVue
    }
  ]
})


export default router
