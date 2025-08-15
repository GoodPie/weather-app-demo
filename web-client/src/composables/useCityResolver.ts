import { ref, watchEffect } from 'vue'
import { useLocationsStore } from '@/stores/locations'
import type { City } from '@/composables/useCitySearch'

export function useCityResolver(idParam: string | number) {
  const city = ref<City | null>(null)
  const idNum = typeof idParam === 'string' ? Number(idParam) : idParam

  const locations = useLocationsStore()
  // Ensure hydration for direct navigation
  locations.loadFromStorage()

  watchEffect(() => {
    if (Number.isFinite(idNum)) {
      city.value = locations.getById(idNum as number)
    } else {
      city.value = null
    }
  })

  return { city }
}
