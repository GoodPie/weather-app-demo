import { computed, type Ref } from 'vue'
import type { CurrentWeather } from '@/types/weather'

// Wind speed thresholds (km/h)
const WIND_THRESHOLDS = {
  LIGHT: 10,
  MODERATE: 25,
  HIGH: 50,
} as const

// Humidity percentage thresholds
const HUMIDITY_THRESHOLDS = {
  LOW: 30,
  HIGH: 70
} as const

// UV Index thresholds
const UV_THRESHOLDS = {
  LOW: 3,
  MODERATE: 6,
  HIGH: 8
} as const

export function useWeatherIcons(weatherData: Ref<CurrentWeather | null>) {
  const windIcon = computed(() => {
    if (!weatherData.value || typeof weatherData.value.windKph !== 'number') {
      return 'pi pi-flag'
    }

    const windSpeed = weatherData.value.windKph

    if (windSpeed < WIND_THRESHOLDS.LIGHT) {
      return 'pi pi-flag'
    }
    if (windSpeed < WIND_THRESHOLDS.MODERATE) {
      return 'pi pi-send'
    }
    return 'pi pi-forward'
  })

  const humidityIcon = computed(() => {
    if (!weatherData.value || typeof weatherData.value.humidity !== 'number') {
      return 'pi pi-circle'
    }

    const humidity = weatherData.value.humidity

    if (humidity < HUMIDITY_THRESHOLDS.LOW) {
      return 'pi pi-circle'
    }
    if (humidity < HUMIDITY_THRESHOLDS.HIGH) {
      return 'pi pi-circle-fill'
    }
    return 'pi pi-verified'
  })

  const uvIcon = computed(() => {
    if (!weatherData.value || typeof weatherData.value.uvIndex !== 'number') {
      return 'pi pi-sun'
    }

    const uv = weatherData.value.uvIndex

    if (uv < UV_THRESHOLDS.LOW) {
      return 'pi pi-sun'
    }
    if (uv < UV_THRESHOLDS.MODERATE) {
      return 'pi pi-verified'
    }
    if (uv < UV_THRESHOLDS.HIGH) {
      return 'pi pi-exclamation-triangle'
    }
    return 'pi pi-shield'
  })

  const uvIconColor = computed(() => {
    if (!weatherData.value || typeof weatherData.value.uvIndex !== 'number') {
      return 'text-yellow-400'
    }

    const uv = weatherData.value.uvIndex

    if (uv < UV_THRESHOLDS.LOW) {
      return 'text-green-400'
    }
    if (uv < UV_THRESHOLDS.MODERATE) {
      return 'text-yellow-400'
    }
    if (uv < UV_THRESHOLDS.HIGH) {
      return 'text-orange-400'
    }
    return 'text-red-400'
  })

  return {
    windIcon,
    humidityIcon,
    uvIcon,
    uvIconColor,
    // Export thresholds for testing or display purposes
    WIND_THRESHOLDS,
    HUMIDITY_THRESHOLDS,
    UV_THRESHOLDS
  }
}
