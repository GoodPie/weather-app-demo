<script lang="ts" setup>
import { ref } from 'vue'
import AutoComplete from 'primevue/autocomplete'
import { searchCitiesAPI } from '@/services/api.ts'
import { useCitySearch, type City, type SearchFunction } from '@/composables/useCitySearch'

type CompleteEvent = { query: string }
type ItemSelectEvent = { value: City }

interface Props {
  searchFunction?: SearchFunction
  minQueryLength?: number
  delay?: number
}

const props = withDefaults(defineProps<Props>(), {
  searchFunction: searchCitiesAPI,
  minQueryLength: 2,
  delay: 300
})

const emit = defineEmits<{
  (e: 'city-selected', city: City): void
  (e: 'search-initiated', query: string): void
  (e: 'search-error', error: Error): void
}>()

// Local query model strictly as a string for the input
const inputQuery = ref('')

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

const handleComplete = (event: CompleteEvent): void => {
  // event.query contains the current typed string
  searchCities(event.query)
}

const handleCitySelect = (event: ItemSelectEvent): void => {
  selectCity(event.value)
  // reflect the selection in the input text if desired
  inputQuery.value = event.value.label ?? ''
}

defineExpose({
  inputQuery,
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
    <h1 class="font-groovy text-white text-4xl md:text-5xl lg:text-6xl text-center leading-tight">
      Weather App
    </h1>

    <div class="search-input-wrapper w-full">
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
        v-model="inputQuery"
        :suggestions="filteredCities"
        :loading="isLoading"
        @complete="handleComplete"
        @item-select="handleCitySelect"
        placeholder="Enter city name..."
        class="w-full"
        inputClass="w-full pr-12 px-6 py-4 text-lg bg-app-surface border-2 border-transparent rounded-2xl shadow-lg font-body transition-all duration-200 focus:outline-none focus:ring-4 focus:ring-sunset-500/20 focus:border-sunset-500 placeholder:text-app-text"
        panelClass="mt-1 bg-app-surface border border-sunset-500/90 rounded-lg shadow-lg p-4"
        :aria-label="'Search for city weather information'"
        :delay="delay"
        :minLength="minQueryLength"
        optionLabel="label"
        data-testid="city-autocomplete"
      >
        <template #loadingicon>
          <i class="pi pi-spinner pi-spin text-sunset-500 text-lg absolute right-4 top-1/2 transform -translate-y-1/2" />
        </template>
      </AutoComplete>
    </div>
  </div>
</template>
