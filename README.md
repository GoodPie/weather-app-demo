# Weather Application

A full-stack weather application built with .NET 9 backend API and Vue.js frontend, demonstrating modern software architecture patterns and best practices.

## Architecture Overview

This application follows a layered architecture approach:

- **Presentation Layer**: ASP.NET Core Web API with controllers and static file serving
- **Business Logic Layer (BLL)**: Contains business rules, services, and logic
- **Data Access Layer (DAL)**: Handles database interactions and data models
- **Frontend**: Vue 3 + TypeScript application with modern tooling

## Technology Stack

### Backend
- **.NET 9** - Modern C# framework
- **ASP.NET Core** - Web API framework
- **Entity Framework Core** - ORM with SQLite database
- **SQLite** - Lightweight database for development
- **Google Maps APIs** - Geocoding and Weather services
- **UnitsNet** - Type-safe unit conversions

### Frontend
- **Vue 3** - Progressive JavaScript framework
- **TypeScript** - Type-safe JavaScript
- **Vite** - Modern build tool
- **Tailwind CSS** - Utility-first CSS framework
- **PrimeVue** - UI component library

## Prerequisites

- **.NET 9 SDK** or later ([Download here](https://dotnet.microsoft.com/download))
- **Node.js** v20.19.0 or v22.12.0+ ([Download here](https://nodejs.org/))
- **Google Cloud Platform account** for API access

### Install Required Tools

```bash
# Install Entity Framework CLI tools
dotnet tool install --global dotnet-ef
```

## Getting Started

### 1. Clone the Repository

```bash
git clone <repository-url>
cd WeatherApp
```

### 2. Configure Google API Access

#### Create Google Cloud Project and API Key

1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Create a new project or select existing project
3. Enable the following APIs:
   - **Geocoding API**
   - **Maps Weather API**
4. Create credentials:
   - Go to **APIs & Services > Credentials**
   - Click **Create Credentials > API Key**
   - Copy the generated API key

#### Configure API Key in Application

```bash
cd Presentation
dotnet user-secrets init
dotnet user-secrets set "GoogleMaps:ApiKey" "YOUR_GOOGLE_API_KEY_HERE"
dotnet user-secrets set "GoogleWeather:ApiKey" "YOUR_GOOGLE_API_KEY_HERE"
```

### 3. Setup Database

The application uses SQLite with Entity Framework Core migrations.

```bash
# Navigate to the root directory
cd <project-root>

# Create and apply database migrations
dotnet ef database update --project DAL --startup-project Presentation
```

This will create the SQLite database file at `DAL/Database/WeatherApp.db`.

### 4. Generate HTTPS Certificate

```bash
# Generate and trust development HTTPS certificate
dotnet dev-certs https --trust
```

### 5. Start the Backend API

```bash
# From the root directory
dotnet run --project Presentation

# The API will be available at:
# https://localhost:5006 (HTTPS)
# http://localhost:5005 (HTTP)
```

### 6. Start the Frontend Development Server

```bash
# Navigate to frontend directory
cd web-client

# Install dependencies
npm install

# Start development server
npm run dev

# The frontend will be available at:
# http://localhost:3000
```

## API Documentation

Once the backend is running, you can access:

- **OpenAPI**: `https://localhost:5006/openapi/v1.json`
- **API Testing**: Use the included `Presentation/API.http` file with your IDE

### Available Endpoints

#### Location Search
- `GET /api/v1/locations/search?cityName={city}` - Search for cities

#### Weather Data
- `GET /api/v1/weather/current?latitude={lat}&longitude={lon}&units={units}` - Current weather
- `GET /api/v1/weather/forecast/hourly?latitude={lat}&longitude={lon}&hours={hours}` - Hourly forecast
- `GET /api/v1/weather/forecast/daily?latitude={lat}&longitude={lon}&days={days}` - Daily forecast
- `GET /api/v1/weather/bundle?latitude={lat}&longitude={lon}&units={units}` - Complete weather data

## Development Workflow

### Backend Development
```bash
# Build solution
dotnet build

# Run tests (when available)
dotnet test

# Run API
dotnet run --project Presentation
```

### Frontend Development
```bash
cd web-client

# Type checking
npm run type-check

# Linting
npm run lint

# Unit tests
npm run test:unit

# E2E tests (Playwright)
npm run test:e2e

# Build for production
npm run build
```

## Production Deployment

### Build Frontend for Production
```bash
cd web-client
npm run build
```

The built frontend files will be placed in `Presentation/wwwroot/` and served by the .NET API.

### Docker Deployment
```bash
# Build Docker image
docker compose up --build

# Or build manually
docker build -t weatherapp -f ./Presentation/Dockerfile .
docker run -p 8080:8080 weatherapp
```

## Project Structure

```
WeatherApp/
├── BLL/                    # Business Logic Layer
│   ├── Services/           # Business services
│   └── Services/Google/    # Google API integrations
├── DAL/                    # Data Access Layer
│   ├── Models/             # Database entities
│   ├── Repository/         # Data repositories
│   └── Dtos/              # Data transfer objects
├── Presentation/           # Web API Layer
│   ├── Controllers/        # API controllers
│   └── wwwroot/           # Static files (built frontend)
├── web-client/            # Vue.js Frontend
│   ├── src/
│   │   ├── components/    # Vue components
│   │   ├── composables/   # Vue composition functions
│   │   └── services/      # API service layer
└── README.md
```

## Key Features

- **City Search**: Autocomplete search with Google Geocoding API
- **Weather Data**: Current conditions with temperature, humidity, wind, UV index
- **Unit Conversion**: Automatic conversion between Celsius/Fahrenheit, KPH/MPH
- **Caching**: Smart 5-minute caching to minimize API calls
- **Error Handling**: Comprehensive error handling and logging
- **Type Safety**: Full TypeScript support throughout
- **Responsive Design**: Mobile-first responsive UI

## Configuration

Key configuration files:
- `Presentation/appsettings.json` - Backend configuration
- `web-client/.env` - Frontend environment variables

## Testing

The project includes comprehensive testing setup across multiple layers:

### Backend Testing (.NET)
- **Unit Tests**: xUnit with Moq and FluentAssertions for BLL testing
- **Coverage**: 41 tests covering services, business logic, and unit conversions
- **Mocking**: Professional mocking patterns for external dependencies

```bash
# Run backend unit tests
dotnet test

# Run tests with detailed output
dotnet test --verbosity normal

# Run tests for specific project
dotnet test BLL.Tests/BLL.Tests.csproj
```

### Frontend Testing (Vue.js)
- **Unit Tests**: Vitest for component testing
- **E2E Tests**: Playwright for integration testing

```bash
cd web-client

# Run unit tests
npm run test:unit

# Run E2E tests
npm run test:e2e
```

### API Testing
- **Manual Testing**: HTTP files for API endpoint testing
- **Integration**: Use `Presentation/API.http` with your IDE
- **Automated**: xUnit integration tests (planned)

## Development Notes

- Developed using modern C# patterns and Vue 3 Composition API
- Follows clean architecture principles
- Uses dependency injection throughout
- Implements proper separation of concerns
- Includes comprehensive logging and error handling

For questions or issues, please refer to the code documentation or contact the development team.