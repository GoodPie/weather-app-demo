<script lang="ts" setup>
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { useTimeOfDayTheme } from '@/composables/useTimeOfDayTheme'
import { useUnits } from '@/composables/useUnits'
import { getWeatherBundleById } from '@/services/api'
import type { WeatherBundle, WeatherResponse } from '@/types/weather'
import WeatherBundleView from '@/components/weather/WeatherBundleView.vue'

// Apply base theming (will later derive from city local time)
useTimeOfDayTheme()

const route = useRoute()
const id = route.params.id as string

const { unitSystem, toggleUnitSystem } = useUnits()
const loading = ref<boolean>(true)
const error = ref<string | null>(null)
const response = ref<WeatherResponse<WeatherBundle> | null>(null)

async function load() {
  try {
    error.value = null
    loading.value = true
    response.value = await getWeatherBundleById(id)
  } catch (e) {
    error.value = e instanceof Error ? e.message : 'Failed to load weather data'
    response.value = null
  } finally {
    loading.value = false
  }
}

onMounted(load)
</script>

<template>
  <div class="min-h-screen transition-colors duration-300 p-6">
    <div class="container mx-auto space-y-4">
      <div class="flex items-center justify-between gap-4">
        <div>
          <h1 class="text-3xl md:text-4xl font-bold">{{ response?.location?.label ?? ('#' + id) }}</h1>
        </div>
        <button
          class="px-3 py-2 rounded-xl border border-sunset-500/50 hover:bg-white/5"
          @click="toggleUnitSystem"
        >Toggle Units ({{ unitSystem }})</button>
      </div>

      <div v-if="error" class="p-3 rounded-lg border border-red-500/70 bg-red-500/10 text-red-200">
        {{ error }}
        <button class="ml-3 underline" @click="load">Retry</button>
      </div>

      <WeatherBundleView :response="response" :unit-system="unitSystem" :loading="loading" />
    </div>
  </div>
</template>
