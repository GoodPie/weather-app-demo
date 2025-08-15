<script lang="ts" setup>
import { computed, toRef } from 'vue'
import type { CurrentWeather, UnitSystem } from '@/types/weather'
import { useWeatherIcons } from '@/composables/useWeatherIcons'

interface Props {
  data: CurrentWeather
  unitSystem: UnitSystem
}

const props = defineProps<Props>()

const windDisplay = computed(() => {
  const speed = props.unitSystem === 'metric' ? props.data.windKph : props.data.windMph
  const unit = props.unitSystem === 'metric' ? 'km/h' : 'mph'
  if (typeof speed !== 'number') return null
  return `${Math.round(speed)} ${unit}`
})

// Use the weather icons composable
const weatherDataRef = toRef(props, 'data')
const { windIcon, humidityIcon, uvIcon, uvIconColor } = useWeatherIcons(weatherDataRef)
</script>

<template>
  <div class="grid grid-cols-2 gap-4">
    <div v-if="windDisplay" class="bg-sky-500/20 border border-sky-400/30 backdrop-blur-sm rounded-xl p-3 text-center">
      <div class="text-2xl mb-1">
        <i :class="windIcon" class="text-sky-400"></i>
      </div>
      <div class="text-sm text-app-text/90 mb-1">Wind</div>
      <div class="font-semibold text-app-text">{{ windDisplay }}</div>
      <div v-if="data.windDir" class="text-xs text-app-text/80">({{ data.windDir }})</div>
    </div>

    <div v-if="typeof data.humidity === 'number'" class="bg-blue-500/20 border border-blue-400/30 backdrop-blur-sm rounded-xl p-3 text-center">
      <div class="text-2xl mb-1">
        <i :class="humidityIcon" class="text-blue-400"></i>
      </div>
      <div class="text-sm text-app-text/90 mb-1">Humidity</div>
      <div class="font-semibold text-app-text">{{ Math.round(data.humidity!) }}%</div>
    </div>

    <div v-if="typeof data.uvIndex === 'number'" class="bg-orange-500/20 border border-orange-400/30 backdrop-blur-sm rounded-xl p-3 text-center">
      <div class="text-2xl mb-1">
        <i :class="[uvIcon, uvIconColor]"></i>
      </div>
      <div class="text-sm text-app-text/90 mb-1">UV Index</div>
      <div class="font-semibold text-app-text">{{ data.uvIndex }}</div>
    </div>
  </div>
</template>
