<script setup lang="ts">
import { ref, onMounted } from 'vue'

const forecast = ref<any[] | null>(null)
const error = ref<string | null>(null)
const loading = ref(true)

onMounted(async () => {
  try {
    const res = await fetch('/api/v1/WeatherForecast')
    if (!res.ok) throw new Error('Network response was not ok')
    forecast.value = await res.json()
  } catch (e: any) {
    error.value = e?.message ?? 'Unknown error'
  } finally {
    loading.value = false
  }
})
</script>

<template>
  <div class="about">
    <h1 v-if="loading">Loading…</h1>
    <h1 v-else-if="error">Error: {{ error }}</h1>
    <pre v-else>{{ forecast }}</pre>
  </div>
</template>
