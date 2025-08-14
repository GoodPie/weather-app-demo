# Weather App

A simple weather app that uses the OpenWeatherMap API to fetch and display current weather data for a given city.

## Requirements

- .NET 9 SDK or later
- Node JS (LTS)
- OpenWeatherMap API key (sign up for free at https://openweathermap.org/api)

1. Install `dotnet-ef`
```bash
dotnet tool install --global dotnet-ef
```

## Setup

1. Get a free API key from OpenWeatherMap.
2. Clone the repository

## API Key Configuration

1. Create a Google API Key with the following services enabled:
    - Geocoding API
    - Places API

2. Setup dotnet user secrets to store your API key:
   ```bash
   cd Presentation
   dotnet user-secrets init
   dotnet user-secrets set "GoogleMaps:ApiKey" "YOUR_GOOGLE_API_KEY"
   ```

## API

1. Generate the `https` certificate
   ```bash
   # This command will generate a self-signed certificate and trust it on your machine
   dotnet dev-certs https --trust
   ```

2. Run the Presentation Server
    ```bash
    dotnet run --project Presentation/Presentation.csproj --launch-profile https  
    ```
    This will start the server on `http://localhost:5000`.

## Web App (Vue 3)

1. Navigate to the `web-client` directory:
   ```bash
   cd web-client
   ```
2. Install the dependencies:
   ```bash
    npm install
    ```

### Development

1. Start the development server:
   ```bash
   npm run dev
   ```
   
## Docker

1. Build the Docker image:
   ```bash
   docker build . -t weather-app -f ./Presentation/Dockerfile 
   ```
   
2. Run the Docker container:
   ```bash
     docker run -p 8080:8080 weather-app -h weather-app
    ```
   

   
This will create a proxy to the backend API, allowing you to make requests without CORS issues.

2. Open your browser and go to `http://localhost:3000` to view the app.


### Notes

This was developed on a non-windows machine using Visual Studio Code