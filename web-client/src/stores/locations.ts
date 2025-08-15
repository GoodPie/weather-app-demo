import { defineStore } from 'pinia'
import type { City } from '@/composables/useCitySearch'

const STORAGE_KEY = 'locationsStore:v1'

export const useLocationsStore = defineStore('locations', {
  state: () => ({
    favorites: [] as City[],
    recents: [] as City[]
  }),
  getters: {
    getById: (state) => (id: number) =>
      state.favorites.find((c) => c.id === id) || state.recents.find((c) => c.id === id) || null,
    isFavorite: (state) => (id: number) => state.favorites.some((c) => c.id === id)
  },
  actions: {
    loadFromStorage() {
      try {
        const raw = localStorage.getItem(STORAGE_KEY)
        if (raw) {
          const parsed = JSON.parse(raw) as { favorites?: City[]; recents?: City[] }
          this.favorites = parsed.favorites ?? []
          this.recents = parsed.recents ?? []
        }
      } catch {
        // ignore
      }
    },
    persist() {
      try {
        localStorage.setItem(
          STORAGE_KEY,
          JSON.stringify({ favorites: this.favorites, recents: this.recents })
        )
      } catch {
        // ignore
      }
    },
    addRecent(city: City) {
      // remove existing duplicate
      this.recents = this.recents.filter((c) => c.id !== city.id)
      // add to front
      this.recents.unshift(city)
      // cap to 10
      if (this.recents.length > 10) this.recents = this.recents.slice(0, 10)
      this.persist()
    },
    addFavorite(city: City) {
      if (!this.favorites.some((c) => c.id === city.id)) {
        this.favorites.push(city)
        this.persist()
      }
    },
    removeFavorite(id: number) {
      this.favorites = this.favorites.filter((c) => c.id !== id)
      this.persist()
    }
  }
})
