import type {
  CurrentWeather,
  DailyForecast,
  HourlyForecast,
  UnitSystem,
  WeatherBundle, WeatherResponse,
} from '@/types/weather'
import type { City } from '@/composables/useCitySearch'

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL
const API_BASE_HTTP_URL = import.meta.env.VITE_BASE_HTTP_URL

const LOCATIONS_API_URL = `${API_BASE_URL}locations`
const WEATHER_API_URL = `${API_BASE_URL}weather`


/**
 * Wrapper around fetch that parses JSON response.
 * @param url URL to fetch
 * @param init Fetch options
 */
async function jsonFetch<T>(url: string, init?: RequestInit): Promise<T> {
  const res = await fetch(url, {
    headers: {
      Accept: 'application/json',
      ...(init?.headers || {})
    },
    ...init
  })
  if (!res.ok) {
    let message = `HTTP ${res.status}`
    try {
      const data = await res.json()
      if (data?.message) message = data.message
    } catch {
      // ignore JSON parse errors
    }
    throw new Error(message)
  }
  return (await res.json()) as T
}


// Locations
export async function searchCitiesAPI(query: string): Promise<City[]> {
  const url = new URL(`${LOCATIONS_API_URL}/search`)
  url.searchParams.set('cityName', query)
  return await jsonFetch<City[]>(url.toString())
}


// Weather - Current by coordinates
export async function getCurrentWeather(
  lat: number,
  lon: number,
  units: UnitSystem
): Promise<WeatherResponse<CurrentWeather>> {
  const url = new URL(`${WEATHER_API_URL}/current`)
  url.searchParams.set('latitude', String(lat))
  url.searchParams.set('longitude', String(lon))
  url.searchParams.set('units', units)
  return await jsonFetch<WeatherResponse<CurrentWeather>>(url.toString())
}

// Weather - Current by location id
export async function getCurrentWeatherById(id: number | string): Promise<WeatherResponse<CurrentWeather>> {
  const url = `${WEATHER_API_URL}/current/${encodeURIComponent(String(id))}`
  return await jsonFetch<WeatherResponse<CurrentWeather>>(url)
}

// Weather - Hourly forecast by coordinates
export async function getHourlyForecast(
  lat: number,
  lon: number,
  hours: number,
  units: UnitSystem
): Promise<WeatherResponse<HourlyForecast[]>> {
  const url = new URL(`${WEATHER_API_URL}/forecast/hourly`)
  url.searchParams.set('latitude', String(lat))
  url.searchParams.set('longitude', String(lon))
  url.searchParams.set('hours', String(hours))
  url.searchParams.set('units', units)
  return await jsonFetch<WeatherResponse<HourlyForecast[]>>(url.toString())
}

// Weather - Daily forecast by coordinates
export async function getDailyForecast(
  lat: number,
  lon: number,
  days: number,
  units: UnitSystem
): Promise<WeatherResponse<DailyForecast[]>> {
  const url = new URL(`${WEATHER_API_URL}/forecast/daily`)
  url.searchParams.set('latitude', String(lat))
  url.searchParams.set('longitude', String(lon))
  url.searchParams.set('days', String(days))
  url.searchParams.set('units', units)
  return await jsonFetch<WeatherResponse<DailyForecast[]>>(url.toString())
}

// Weather - Bundle (current + forecasts) by coordinates
export async function getWeatherBundle(
  lat: number,
  lon: number,
  units: UnitSystem
): Promise<WeatherResponse<WeatherBundle>> {
  const url = new URL(`${WEATHER_API_URL}/bundle`)
  url.searchParams.set('latitude', String(lat))
  url.searchParams.set('longitude', String(lon))
  url.searchParams.set('units', units)
  return await jsonFetch<WeatherResponse<WeatherBundle>>(url.toString())
}

// Weather - Bundle by location id
export async function getWeatherBundleById(id: number | string): Promise<WeatherResponse<WeatherBundle>> {
  const url = `${WEATHER_API_URL}/bundle/${encodeURIComponent(String(id))}`
  return await jsonFetch<WeatherResponse<WeatherBundle>>(url)
}

// OpenAPI (useful for dev tools)
export async function getOpenApiSchema(): Promise<unknown> {
  const url = `${API_BASE_HTTP_URL}/openapi/v1.json`
  return await jsonFetch<unknown>(url)
}
