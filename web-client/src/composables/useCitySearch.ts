import { ref, type Ref } from 'vue'

export type City = {
  id: number
  name: string
  label: string
}

export type SearchFunction = (query: string) => Promise<City[]>

export interface UseCitySearchOptions {
  searchFunction: SearchFunction
  minQueryLength?: number
  onSearchInitiated?: (query: string) => void
  onCitySelected?: (city: City) => void
  onError?: (error: Error) => void
}

export function useCitySearch(options: UseCitySearchOptions) {
  const {
    searchFunction,
    minQueryLength = 2,
    onSearchInitiated,
    onCitySelected,
    onError
  } = options

  const selectedCity: Ref<string | City | null> = ref('')
  const filteredCities: Ref<City[]> = ref([])
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  const clearError = () => {
    error.value = null
  }

  const searchCities = async (query: string): Promise<void> => {
    const trimmedQuery = query.toLowerCase().trim()

    // Clear previous error
    clearError()

    // Reset if query too short
    if (trimmedQuery.length < minQueryLength) {
      filteredCities.value = []
      return
    }

    // Notify about search initiation
    onSearchInitiated?.(trimmedQuery)

    try {
      isLoading.value = true
      filteredCities.value = await searchFunction(trimmedQuery)
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Search failed'
      error.value = errorMessage
      filteredCities.value = []
      onError?.(err instanceof Error ? err : new Error(errorMessage))
    } finally {
      isLoading.value = false
    }
  }

  const selectCity = (city: City): void => {
    selectedCity.value = city
    onCitySelected?.(city)
  }

  return {
    selectedCity,
    filteredCities,
    isLoading,
    error,
    searchCities,
    selectCity,
    clearError
  }
}
