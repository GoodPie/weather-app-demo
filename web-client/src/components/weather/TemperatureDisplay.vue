<script lang="ts" setup>
import { computed } from 'vue'
import type { CurrentWeather, UnitSystem } from '@/types/weather'

interface Props {
  data: CurrentWeather
  unitSystem: UnitSystem
}

const props = defineProps<Props>()

const tempDisplay = computed(() => {
  return props.unitSystem === 'metric'
    ? `${Math.round(props.data.tempC)}°C`
    : `${Math.round(props.data.tempF)}°F`
})

const feelsLikeDisplay = computed(() => {
  const v = props.unitSystem === 'metric' ? props.data.feelsLikeC : props.data.feelsLikeF
  return typeof v === 'number' ? `${Math.round(v)}°` : null
})
</script>

<template>
  <div class="text-center space-y-2">
    <div class="text-6xl font-bold bg-gradient-to-r from-sunset-400 to-sunset-600 bg-clip-text text-transparent">
      {{ tempDisplay }}
    </div>
    <div class="text-xl font-medium text-app-text/95 capitalize">
      {{ data.conditionText ?? '' }}
    </div>
    <div v-if="feelsLikeDisplay" class="text-app-text/85">
      Feels like <span class="font-semibold text-app-text">{{ feelsLikeDisplay }}</span>
    </div>
  </div>
</template>
