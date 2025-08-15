<script lang="ts" setup>
import { ref } from 'vue'
import AutoComplete from 'primevue/autocomplete'
import { searchCitiesAPI } from '@/services/api.ts'

type City = {
  id: number,
  name: string
  country: string
  code: string,
  label: string
}

type CompleteEvent = {
  query: string
}

type ItemSelectEvent = {
  value: City
}

// Emit events for parent component (future use)
const emit = defineEmits<{
  (e: 'city-selected', city: City): void
  (e: 'search-initiated', query: string): void
}>()

const selectedCity = ref<string | City | null>('')
const filteredCities = ref<City[]>([])

// Search function for autocomplete
function searchCities(event: CompleteEvent): void {
  const query = event.query.toLowerCase().trim()

  if (query.length < 2) {
    filteredCities.value = []
    return
  }

  // Notify parent that a search was initiated
  emit('search-initiated', query)

  searchCitiesAPI(query).then((results) => {
    filteredCities.value = results
  })
}

// Handle city selection
function onCitySelect(event: ItemSelectEvent): void {
  console.log('Selected city:', event.value)
  emit('city-selected', event.value)
  // TODO: Trigger weather search
  // fetchWeatherData(event.value)
}
</script>

<template>
  <div class="search-container space-y-6">
    <!-- Groovy Title -->
    <h1 class="font-groovy text-white text-4xl md:text-5xl lg:text-6xl text-center leading-tight">
      Weather App
    </h1>

    <!-- AutoComplete Search Input using Volt -->
    <div class="search-input-wrapper w-full">
      <AutoComplete
        v-model="selectedCity"
        :suggestions="filteredCities"
        @complete="searchCities"
        placeholder="Enter city name..."
        class="w-full"
        inputClass="bg-white w-full px-6 py-4 text-lg bg-bg-surface border-2 border-transparent rounded-2xl shadow-lg font-body transition-all duration-200 focus:outline-none focus:ring-4 focus:ring-sunset-500/20 focus:border-sunset-500d placeholder:text-text-secondary"
        panelClass="mt-1 bg-white border border-sunset-500/90 rounded-lg shadow-lg max-h-60 p-4"
        :aria-label="'Search for city weather information'"
        :delay="300"
        :minLength="2"
        optionLabel="label"
        @item-select="onCitySelect"
      />
    </div>
  </div>
</template>
