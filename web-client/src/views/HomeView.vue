<script lang="ts" setup>
import { ref, computed } from 'vue'
import SearchBar from '@/components/search/SearchBar.vue'
const isDark = ref(false)

const themeClasses = computed(() => ({
  background: isDark.value
    ? 'dark bg-bg-primary text-primary]'
    : 'light bg-primary text-text-primary',
}))

function updateTheme(darkMode: boolean) {
  isDark.value = darkMode
  document.documentElement.className = darkMode ? 'dark' : 'light'
}

// Initialize theme on mount
updateTheme(window.matchMedia('(prefers-color-scheme: dark)').matches)
</script>

<template>
  <div class="min-h-screen transition-colors duration-300" :class="themeClasses.background">
    <!-- Main Container -->
    <main class="container mx-auto px-4 py-8">
      <div class="flex flex-col items-center justify-center min-h-[80vh] md:min-h-screen">
        <div class="w-full max-w-md mx-auto">
          <SearchBar />
        </div>
      </div>
    </main>
  </div>
</template>
