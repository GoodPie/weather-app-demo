<script lang="ts" setup>
import AutoComplete from 'primevue/autocomplete'
import { searchCitiesAPI } from '@/services/api.ts'
import { useCitySearch, type City, type SearchFunction } from '@/composables/useCitySearch'

type CompleteEvent = {
  query: string
}

type ItemSelectEvent = {
  value: City
}

interface Props {
  searchFunction?: SearchFunction
  minQueryLength?: number
  delay?: number
}

// Props with defaults for dependency injection
const props = withDefaults(defineProps<Props>(), {
  searchFunction: searchCitiesAPI,
  minQueryLength: 2,
  delay: 300
})

// Emit events for parent component
const emit = defineEmits<{
  (e: 'city-selected', city: City): void
  (e: 'search-initiated', query: string): void
  (e: 'search-error', error: Error): void
}>()

// Use the composable with injected dependencies
const {
  selectedCity,
  filteredCities,
  isLoading,
  error,
  searchCities,
  selectCity,
  clearError
} = useCitySearch({
  searchFunction: props.searchFunction,
  minQueryLength: props.minQueryLength,
  onSearchInitiated: (query: string) => emit('search-initiated', query),
  onCitySelected: (city: City) => emit('city-selected', city),
  onError: (error: Error) => emit('search-error', error)
})

// Event handlers
const handleComplete = (event: CompleteEvent): void => {
  searchCities(event.query)
}

const handleCitySelect = (event: ItemSelectEvent): void => {
  selectCity(event.value)
}

// Expose for testing
defineExpose({
  selectedCity,
  filteredCities,
  isLoading,
  error,
  searchCities,
  selectCity,
  clearError
})
</script>

<template>
  <div class="search-container space-y-6">
    <!-- Groovy Title -->
    <h1 class="font-groovy text-white text-4xl md:text-5xl lg:text-6xl text-center leading-tight">
      Weather App
    </h1>

    <!-- AutoComplete Search Input -->
    <div class="search-input-wrapper w-full">
      <!-- Error Display -->
      <div
        v-if="error"
        class="mb-4 p-3 bg-red-100 border border-red-400 text-red-700 rounded-lg"
        role="alert"
        data-testid="search-error"
      >
        {{ error }}
        <button
          @click="clearError"
          class="ml-2 underline hover:no-underline"
          data-testid="clear-error-button"
        >
          Dismiss
        </button>
      </div>

      <AutoComplete
        v-model="selectedCity"
        :suggestions="filteredCities"
        :loading="isLoading"
        @complete="handleComplete"
        @item-select="handleCitySelect"
        placeholder="Enter city name..."
        class="w-full"
        inputClass="w-full px-6 py-4 text-lg bg-app-surface border-2 border-transparent rounded-2xl shadow-lg font-body transition-all duration-200 focus:outline-none focus:ring-4 focus:ring-sunset-500/20 focus:border-sunset-500d placeholder:text-app-text"
        panelClass="mt-1 bg-app-surface border border-sunset-500/90 rounded-lg shadow-lg p-4"
        :aria-label="'Search for city weather information'"
        :delay="delay"
        :minLength="minQueryLength"
        optionLabel="label"
        data-testid="city-autocomplete"
      />

      <!-- Loading Indicator -->
      <div
        v-if="isLoading"
        class="mt-2 text-sm text-gray-600"
        data-testid="loading-indicator"
      >
        Searching cities...
      </div>
    </div>
  </div>
</template>
