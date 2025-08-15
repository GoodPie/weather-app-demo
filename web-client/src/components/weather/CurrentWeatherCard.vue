<script lang="ts" setup>
import type { CurrentWeather, UnitSystem } from '@/types/weather'
import TemperatureDisplay from './TemperatureDisplay.vue'
import WeatherMetricsGrid from './WeatherMetricsGrid.vue'
import SunTimesDisplay from './SunTimesDisplay.vue'

interface Props {
  data: CurrentWeather | null
  unitSystem: UnitSystem
  title?: string
  loading?: boolean
}

withDefaults(defineProps<Props>(), {
  title: 'Current Weather',
  loading: false,
})
</script>

<template>
  <div
    class="w-full rounded-3xl mt-4 p-6 shadow-xl border bg-app-surface/90 border-app-surface/50 backdrop-blur-md transition-all duration-300 hover:shadow-2xl hover:bg-app-surface/95"
  >
    <h2 class="font-groovy text-2xl font-bold mb-6 text-center text-app-text">{{ title }}</h2>

    <div v-if="loading" class="text-center py-8">
      <div class="animate-pulse text-lg text-app-text/80" data-testid="cw-loading">
        <i class="pi pi-spin pi-spinner mr-2 text-sunset-400"></i>
        Loading weather...
      </div>
    </div>

    <div v-else-if="!data" class="text-center py-8 text-app-text/80" data-testid="cw-empty">
      <div class="text-4xl mb-2">
        <i class="pi pi-cloud text-app-text/60"></i>
      </div>
      <div>No weather data available</div>
    </div>

    <div v-else class="space-y-6" data-testid="cw-content">
      <!-- Primary: Temperature & Condition -->
      <TemperatureDisplay :data="data" :unit-system="unitSystem" />

      <!-- Secondary: Key Metrics Grid -->
      <WeatherMetricsGrid :data="data" :unit-system="unitSystem" />

      <!-- Tertiary: Sun Times -->
      <SunTimesDisplay :data="data" />
    </div>
  </div>
</template>
