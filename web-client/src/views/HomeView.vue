<script lang="ts" setup>
import SearchBar from '@/components/search/SearchBar.vue'
import { useTimeOfDayTheme } from '@/composables/useTimeOfDayTheme'
import { useRouter } from 'vue-router'
import type { City } from '@/composables/useCitySearch'
import { useLocationsStore } from '@/stores/locations'

// Initialize time-of-day and dark mode theming for the search page
useTimeOfDayTheme()

const router = useRouter()
const locations = useLocationsStore()
locations.loadFromStorage()

function onCitySelected(city: City) {
  locations.addRecent(city)
  router.push({ name: 'CityWeather', params: { id: city.id } })
}
</script>

<template>
  <div class="min-h-screen transition-colors duration-300">
    <!-- Main Container -->
    <main class="container mx-auto px-4 py-8">
      <div class="flex flex-col items-center justify-center min-h-[80vh] md:min-h-screen">
        <div class="w-full max-w-md mx-auto">
          <SearchBar @city-selected="onCitySelected" />
        </div>
      </div>
    </main>
  </div>
</template>
