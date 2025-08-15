import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView,
    },
    {
      path: '/city/:id',
      name: 'CityWeather',
      component: () => import('../views/CityWeatherView.vue')
    }
  ],
})

export default router
