<script lang="ts" setup>
import { computed } from 'vue'
import type { CurrentWeather, UnitSystem } from '@/types/weather'

interface Props {
  data: CurrentWeather | null
  unitSystem: UnitSystem
  title?: string
  loading?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  title: 'Current Weather',
  loading: false
})

const tempDisplay = computed(() => {
  if (!props.data) return '-'
  return props.unitSystem === 'metric'
    ? `${Math.round(props.data.tempC)}°C`
    : `${Math.round(props.data.tempF)}°F`
})

const feelsLikeDisplay = computed(() => {
  if (!props.data) return null
  const v = props.unitSystem === 'metric' ? props.data.feelsLikeC : props.data.feelsLikeF
  return typeof v === 'number' ? `${Math.round(v)}°` : null
})

const windDisplay = computed(() => {
  if (!props.data) return null
  const speed = props.unitSystem === 'metric' ? props.data.windKph : props.data.windMph
  const unit = props.unitSystem === 'metric' ? 'km/h' : 'mph'
  if (typeof speed !== 'number') return null
  return `${Math.round(speed)} ${unit}`
})
</script>

<template>
  <div class="rounded-2xl bg-app-surface p-4 shadow-md border border-sunset-500/20">
    <h2 class="text-xl font-semibold mb-2">{{ title }}</h2>

    <div v-if="loading" class="text-app-text/70" data-testid="cw-loading">Loading...</div>
    <div v-else-if="!data" class="text-app-text/70" data-testid="cw-empty">No data</div>
    <div v-else class="grid grid-cols-2 gap-3" data-testid="cw-content">
      <div class="col-span-2 flex items-baseline gap-3">
        <span class="text-4xl font-bold">{{ tempDisplay }}</span>
        <span class="text-app-text/80">{{ data.conditionText ?? '' }}</span>
      </div>
      <div v-if="feelsLikeDisplay" class="text-app-text/90">
        Feels like: <span class="font-medium">{{ feelsLikeDisplay }}</span>
      </div>
      <div v-if="windDisplay" class="text-app-text/90">
        Wind: <span class="font-medium">{{ windDisplay }}</span>
        <span v-if="data.windDir" class="ml-1">({{ data.windDir }})</span>
      </div>
      <div v-if="typeof data.humidity === 'number'">Humidity: <span class="font-medium">{{ Math.round(data.humidity!) }}%</span></div>
      <div v-if="typeof data.uvIndex === 'number'">UV Index: <span class="font-medium">{{ data.uvIndex }}</span></div>
      <div v-if="data.sunrise">Sunrise: <span class="font-medium">{{ data.sunrise }}</span></div>
      <div v-if="data.sunset">Sunset: <span class="font-medium">{{ data.sunset }}</span></div>
    </div>
  </div>
</template>
