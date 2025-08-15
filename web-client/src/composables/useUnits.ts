import { ref, computed, watch } from 'vue'
import type { UnitSystem } from '@/types/weather'

const STORAGE_KEY = 'unitSystem'

const unit = ref<UnitSystem>((localStorage.getItem(STORAGE_KEY) as UnitSystem) || 'metric')

watch(
  unit,
  (val) => {
    try {
      localStorage.setItem(STORAGE_KEY, val)
    } catch {
      // ignore storage errors
    }
  },
  { immediate: true }
)

function setUnitSystem(value: UnitSystem) {
  unit.value = value
}

function toggleUnitSystem() {
  unit.value = unit.value === 'metric' ? 'imperial' : 'metric'
}

// Helpers
function toFahrenheit(celsius: number): number {
  return Math.round((celsius * 9) / 5 + 32)
}

function toCelsius(fahrenheit: number): number {
  return Math.round(((fahrenheit - 32) * 5) / 9)
}

function formatTemp(valueC: number, withUnit = true): string {
  if (unit.value === 'metric') return `${Math.round(valueC)}${withUnit ? '°C' : ''}`
  return `${toFahrenheit(valueC)}${withUnit ? '°F' : ''}`
}

function formatSpeed(kph: number, withUnit = true): string {
  if (unit.value === 'metric') return `${Math.round(kph)}${withUnit ? ' km/h' : ''}`
  const mph = Math.round(kph * 0.621371)
  return `${mph}${withUnit ? ' mph' : ''}`
}

export function useUnits() {
  const unitSystem = computed(() => unit.value)

  return {
    unitSystem,
    setUnitSystem,
    toggleUnitSystem,
    // conversion helpers
    toFahrenheit,
    toCelsius,
    formatTemp,
    formatSpeed
  }
}
