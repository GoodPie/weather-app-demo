import { ref } from 'vue'
import type { CurrentWeather, DailyForecast, HourlyForecast, UnitSystem } from '@/types/weather'
import { getCurrentWeather, getDailyForecast, getHourlyForecast } from '@/services/api'

const TTL_CURRENT_MS = 10 * 60 * 1000 // 10 minutes
const TTL_FORECAST_MS = 60 * 60 * 1000 // 60 minutes

type CacheEntry = {
  current: { data: CurrentWeather | null; ts: number }
  hourly: { data: HourlyForecast[]; ts: number }
  daily: { data: DailyForecast[]; ts: number }
}

const cache = new Map<string, CacheEntry>()

function key(lat: number, lon: number, units: UnitSystem) {
  return `${lat.toFixed(4)},${lon.toFixed(4)}|${units}`
}

export function useWeather(params: {
  lat: number
  lon: number
  unitSystem: UnitSystem
  hours?: number
  days?: number
}) {
  const { lat, lon, unitSystem, hours = 24, days = 7 } = params

  const current = ref<CurrentWeather | null>(null)
  const hourly = ref<HourlyForecast[]>([])
  const daily = ref<DailyForecast[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  function isFresh(ts: number, ttl: number) {
    return Date.now() - ts < ttl
  }

  async function load(force = false) {
    error.value = null
    const k = key(lat, lon, unitSystem)
    const entry = cache.get(k)

    const needsCurrent =
      force || !entry || !entry.current.data || !isFresh(entry.current.ts, TTL_CURRENT_MS)
    const needsHourly =
      force || !entry || !entry.hourly.data || !isFresh(entry.hourly.ts, TTL_FORECAST_MS)
    const needsDaily =
      force || !entry || !entry.daily.data || !isFresh(entry.daily.ts, TTL_FORECAST_MS)

    // Initialize cache entry if missing
    if (!entry) {
      cache.set(k, {
        current: { data: null, ts: 0 },
        hourly: { data: [], ts: 0 },
        daily: { data: [], ts: 0 }
      })
    }

    const effective = cache.get(k)!

    loading.value = true
    try {
      const tasks: Promise<void>[] = []

      if (needsCurrent) {
        tasks.push(
          getCurrentWeather(lat, lon, unitSystem).then((response) => {
            const data = response.weather;
            effective.current = { data, ts: Date.now() }
          })
        )
      }
      if (needsHourly) {
        tasks.push(
          getHourlyForecast(lat, lon, hours, unitSystem).then((response) => {
            const data = response.weather;
            effective.hourly = { data, ts: Date.now() }
          })
        )
      }
      if (needsDaily) {
        tasks.push(
          getDailyForecast(lat, lon, days, unitSystem).then((response) => {
            const data = response.weather;
            effective.daily = { data, ts: Date.now() }
          })
        )
      }

      if (tasks.length) {
        await Promise.all(tasks)
      }

      // Sync refs from cache
      current.value = effective.current.data
      hourly.value = effective.hourly.data
      daily.value = effective.daily.data
    } catch (e: unknown) {
      error.value = e instanceof Error ? e.message : 'Failed to load weather'
    } finally {
      loading.value = false
    }
  }

  async function refresh() {
    await load(true)
  }

  // initial load
  void load()

  return {
    current,
    hourly,
    daily,
    loading,
    error,
    refresh,
    load
  }
}
