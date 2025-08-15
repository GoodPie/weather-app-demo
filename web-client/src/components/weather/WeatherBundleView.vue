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
  loading: false
})
</script>

<template>
  <section class="space-y-4">
    <h2 class="text-2xl font-semibold" v-if="title">{{ title }}</h2>

    <CurrentWeatherCard :data="response?.weather?.current ?? null" :unit-system="unitSystem" :loading="loading" />
  </section>
</template>
