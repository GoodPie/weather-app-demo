<script lang="ts" setup>
import type { CurrentWeather } from '@/types/weather'

interface Props {
  data: CurrentWeather
}

defineProps<Props>()
</script>

<template>
  <div v-if="data.sunrise || data.sunset" class="flex justify-center gap-6 pt-2 border-t border-app-text/20">
    <div v-if="data.sunrise" class="flex items-center gap-2 text-app-text/90">
      <i class="pi pi-angle-up text-orange-300 text-lg"></i>
      <div class="text-center">
        <div class="text-xs text-app-text/70">Sunrise</div>
        <div class="font-medium text-sm text-app-text">{{ data.sunrise }}</div>
      </div>
    </div>
    <div v-if="data.sunset" class="flex items-center gap-2 text-app-text/90">
      <i class="pi pi-angle-down text-orange-400 text-lg"></i>
      <div class="text-center">
        <div class="text-xs text-app-text/70">Sunset</div>
        <div class="font-medium text-sm text-app-text">{{ data.sunset }}</div>
      </div>
    </div>
  </div>
</template>
