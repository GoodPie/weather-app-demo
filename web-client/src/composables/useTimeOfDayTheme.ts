import { onMounted, onUnmounted, ref } from 'vue'

export type TimeOfDayClass =
  | 'time-midnight'
  | 'time-night'
  | 'time-dusk'
  | 'time-midday'
  | 'time-dawn'

export function useTimeOfDayTheme() {
  const isDark = ref(false)
  const currentTimeClass = ref<TimeOfDayClass>('time-midday')

  let timer: number | undefined
  let media: MediaQueryList | null = null
  let mediaListener: ((e: MediaQueryListEvent) => void) | null = null

  function getTimeOfDayClass(date = new Date()): TimeOfDayClass {
    const h = date.getHours()
    if (h < 3) return 'time-midnight'
    if (h < 6) return 'time-dawn'
    if (h < 16) return 'time-midday'
    if (h < 19) return 'time-dusk'
    return 'time-night'
  }

  function applyHtmlClasses() {
    if (typeof document === 'undefined') return

    const html = document.documentElement
    // Toggle dark/light
    html.classList.remove('dark', 'light')
    html.classList.add(isDark.value ? 'dark' : 'light')

    // Replace previous time-of-day class
    const classesToRemove = Array.from(html.classList).filter(c => c.startsWith('time-'))
    classesToRemove.forEach(c => html.classList.remove(c))
    html.classList.add(currentTimeClass.value)
  }

  function updateThemeFromSystemPreference() {
    if (typeof window === 'undefined') return

    isDark.value = window.matchMedia('(prefers-color-scheme: dark)').matches
    applyHtmlClasses()
  }

  function updateTimeTheme() {
    currentTimeClass.value = getTimeOfDayClass()
    applyHtmlClasses()
  }

  onMounted(() => {
    // Initial apply
    updateThemeFromSystemPreference()
    updateTimeTheme()

    // Watch system dark mode changes
    if (typeof window !== 'undefined') {
      media = window.matchMedia('(prefers-color-scheme: dark)')
      mediaListener = (e: MediaQueryListEvent) => {
        isDark.value = e.matches
        applyHtmlClasses()
      }
      // Optional chaining for broader browser support
      media.addEventListener?.('change', mediaListener)

      // Update time-of-day periodically (every 5 minutes)
      timer = window.setInterval(updateTimeTheme, 5 * 60 * 1000)
    }
  })

  onUnmounted(() => {
    if (media && mediaListener) media.removeEventListener?.('change', mediaListener)
    if (timer) window.clearInterval(timer)
  })

  return {
    // state
    isDark,
    currentTimeClass,
    // helpers (exposed for testing/extensibility)
    getTimeOfDayClass,
    updateTimeTheme,
    updateThemeFromSystemPreference
  }
}
