<script lang="ts" setup>
import type { UnitSystem, WeatherBundle, WeatherResponse } from '@/types/weather'
import CurrentWeatherCard from './CurrentWeatherCard.vue'

interface Props {
  response?: WeatherResponse<WeatherBundle> | null
  unitSystem: UnitSystem
  title?: string
  loading?: boolean
}

withDefaults(defineProps<Props>(), {
  title: 'Weather',
  loading: false,
})
</script>

<template>
  <section class="flex justify-center w-full">
    <CurrentWeatherCard
      :data="response?.weather?.current ?? null"
      :unit-system="unitSystem"
      :loading="loading"
    />
  </section>
</template>
