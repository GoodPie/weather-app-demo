<script lang="ts" setup>
import { computed } from 'vue'
import type { HourlyForecast, UnitSystem } from '@/types/weather'

interface Props {
  data: HourlyForecast[] | null | undefined
  unitSystem: UnitSystem
  title?: string
  hoursToShow?: number
  loading?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  title: 'Hourly Forecast',
  hoursToShow: 12,
  loading: false
})

const items = computed(() => (props.data ?? []).slice(0, props.hoursToShow))

function formatHour(iso: string): string {
  const d = new Date(iso)
  return new Intl.DateTimeFormat(undefined, { hour: 'numeric' }).format(d)
}

function formatTemp(h: HourlyForecast): string {
  const v = props.unitSystem === 'metric' ? h.tempC : h.tempF
  return `${Math.round(v)}°`
}
</script>

<template>
  <div class="rounded-2xl bg-app-surface p-4 shadow-md border border-sunset-500/20">
    <h2 class="text-xl font-semibold mb-3">{{ title }}</h2>

    <div v-if="loading" class="text-app-text/70" data-testid="hf-loading">Loading...</div>
    <div v-else-if="!items.length" class="text-app-text/70" data-testid="hf-empty">No forecast</div>
    <div v-else class="flex gap-3 overflow-x-auto pb-2" data-testid="hf-content">
      <div
        v-for="(h, idx) in items"
        :key="idx + h.time"
        class="min-w-[90px] flex-shrink-0 rounded-xl border border-sunset-500/30 bg-white/5 p-3 text-center"
      >
        <div class="text-sm text-app-text/80">{{ formatHour(h.time) }}</div>
        <div class="text-2xl font-semibold">{{ formatTemp(h) }}</div>
        <div class="text-xs text-app-text/70 truncate" :title="h.conditionCode || ''">{{ h.conditionCode || '' }}</div>
        <div v-if="typeof h.pop === 'number'" class="text-xs text-app-text/80 mt-1">💧 {{ Math.round(h.pop) }}%</div>
      </div>
    </div>
  </div>
</template>
