<script lang="ts" setup>
import { computed } from 'vue'
import type { DailyForecast, UnitSystem } from '@/types/weather'

interface Props {
  data: DailyForecast[] | null | undefined
  unitSystem: UnitSystem
  title?: string
  daysToShow?: number
  loading?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  title: 'Daily Forecast',
  daysToShow: 7,
  loading: false
})

const items = computed(() => (props.data ?? []).slice(0, props.daysToShow))

function formatDay(isoDate: string): string {
  const d = new Date(isoDate)
  return new Intl.DateTimeFormat(undefined, { weekday: 'short' }).format(d)
}

function formatTemps(d: DailyForecast): string {
  if (props.unitSystem === 'metric') {
    return `${Math.round(d.minTempC)}° / ${Math.round(d.maxTempC)}°C`
  }
  return `${Math.round(d.minTempF)}° / ${Math.round(d.maxTempF)}°F`
}
</script>

<template>
  <div class="rounded-2xl bg-app-surface p-4 shadow-md border border-sunset-500/20">
    <h2 class="text-xl font-semibold mb-3">{{ title }}</h2>

    <div v-if="loading" class="text-app-text/70" data-testid="df-loading">Loading...</div>
    <div v-else-if="!items.length" class="text-app-text/70" data-testid="df-empty">No forecast</div>
    <ul v-else class="divide-y divide-sunset-500/20" data-testid="df-content">
      <li v-for="(d, idx) in items" :key="idx + d.date" class="py-2 flex items-center justify-between">
        <div class="flex items-center gap-3">
          <span class="w-10 text-app-text/80">{{ formatDay(d.date) }}</span>
          <span class="text-sm text-app-text/70 truncate" :title="d.conditionCode || ''">{{ d.conditionCode || '' }}</span>
        </div>
        <div class="font-medium">{{ formatTemps(d) }}</div>
      </li>
    </ul>
  </div>
</template>
