// Normalized weather types used by composables and UI

import type { City } from '@/composables/useCitySearch.ts'

export type UnitSystem = 'metric' | 'imperial'

export interface Coordinates {
  lat: number
  lon: number
}

export interface WeatherResponse<T> {
  weather: T
  location: City
}

export interface CurrentWeather {
  asOf: string // ISO timestamp
  tempC: number
  tempF: number
  feelsLikeC?: number
  feelsLikeF?: number
  conditionCode?: string
  conditionText?: string
  windKph?: number
  windMph?: number
  windDir?: string
  humidity?: number
  pressureMb?: number
  uvIndex?: number
  visibilityKm?: number
  sunrise?: string
  sunset?: string
  isDay?: boolean
}

export interface HourlyForecast {
  time: string // ISO timestamp
  tempC: number
  tempF: number
  conditionCode?: string
  precipMm?: number
  pop?: number // probability of precipitation (0-100)
  windKph?: number
  windMph?: number
  windDir?: string
}

export interface DailyForecast {
  date: string // ISO date (YYYY-MM-DD)
  minTempC: number
  maxTempC: number
  minTempF: number
  maxTempF: number
  conditionCode?: string
  sunrise?: string
  sunset?: string
  precipMm?: number
  pop?: number
}

export interface WeatherBundle {
  current: CurrentWeather | null
  hourly: HourlyForecast[]
  daily: DailyForecast[]
}
