import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useLocationStore = defineStore('locations', () => {

  // Store the previously selected location
  const selectedLocation = ref('')

  // Store the list of locations that this user has searched for
  const locations = ref([] as Array<string>)

  /**
   * Add a location to the list of locations that this user has searched for
   * @param location
   */
  function addLocation(location: string) {
    if (!locations.value.includes(location)) {
      locations.value.push(location)
    }
  }

  /**
   * This is the last selected location of the user
   * @param location
   */
  function onSelectLocation(location: string) {
    selectedLocation.value = location
    addLocation(location);
  }

  return { selectedLocation, locations, onSelectLocation, addLocation }
})
