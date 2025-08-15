
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;


const LOCATIONS_API_URL = `${API_BASE_URL}locations`;

export async function searchCitiesAPI(query: string) {

  const url = new URL(`${LOCATIONS_API_URL}/search`);
  url.searchParams.append('cityName', query);

  try {
    const response = await fetch(url.toString())
    return await response.json()
  } catch (error) {
    console.error('Error fetching cities:', error)
    return []
  }
}
