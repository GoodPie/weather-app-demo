<script lang="ts" setup>
import { useRouter } from 'vue-router'
import { storeToRefs } from 'pinia'
import { useLocationsStore } from '@/stores/locations'
import { computed } from 'vue'
import Badge from '@/volt/Badge.vue'

const router = useRouter()
const locationsStore = useLocationsStore()
const { recents } = storeToRefs(locationsStore)

// Show only the last 3 recents
const lastThree = computed(() => recents.value.slice(0, 3))

function goToCity(cityId: number) {
  router.push({ name: 'CityWeather', params: { id: cityId } })
}
</script>

<template>
  <div v-if="lastThree.length" class="recent-searches">
    <h3 class="font-semibold mb-2">Recent Searches</h3>
    <ul class="flex gap-2 flex-wrap">
      <li v-for="city in lastThree" :key="city.id">
        <button
          class="focus:outline-none"
          @click="goToCity(city.id)"
          type="button"
        >
          <Badge>
            {{ city.label }}
          </Badge>
        </button>
      </li>
    </ul>
  </div>
</template>

<style scoped>
.recent-searches {
  margin-bottom: 1rem;
}
</style>
