# Architecture

## Backend

- Utilise .NET for backend development
- Layered approach - BLL, DAL, Presentation (API and UI)
- BLL (business logic layer) to handle all business rules and logic
- DAL (data access layer) to handle database interactions and data retrieval
- Presentation layer to expose API endpoints and serve the UI
- Time is a consideration so use best practice but don't go full enterprise
- No authentication for now - Just simple API. Would utilise JWT auth if time allowed
  - We can just implement some basic access control

### Service Layer

- Will create a generic response DTO
  - Never been a fan of this pattern in .NET, but I've seen it in 90% of .NET projects and it should be easy to pick up for those only familiar with .NET
  - It makes sense in the layered approach, but I prefer to rely on HTTP status codes or other communication protocol status


### Data Access Layer

- Don't implement AutoMapper for now
  - Overhead in setting it up 
  - We can use manual mapping for DTOs to entities and address this at a later date if it becomes annoying

## Front End

- Vue.js for views (note: Haven't used before and this is exploratory. Any friction, switch to React)
- Utilise vite proxy for API calls to avoid CORS issues during development
- Production to build into `wwwroot`
- Use Vue Router for client-side routing
- This will sit under `web-client` directory

### UI Components

- Usual React approach is to utilise `shadcn` for components. Investigate what this looks like in Vue.js (looks promising and is just tailwind at the end of the day)
- Pure tailwind as fallback

# Testing

## Front end

- TDD (unit tests)
- Add e2e tests for critical path using playwright

## Backend

- Catching up on .NET 9 and new .NET features - Priority is getting the API up and running
- DDT (unit tests)

# Database and Caching

- SQLite for database 
- Entity Framework Core for ORM
- Use in-memory caching for performance improvements without having to perform a search on db

## Why a database?

- To store locations initially, without having to do a lot of lookups
- To store user preferences or settings in the future

## Why Caching?

- I generally set this up for projects from the get-go, so it's a pattern I follow
- Allows for quick access to frequently used data, without the round-trip to the DB and the complex queries

### Caching Strategy

- Write-through caching for locations
  - We are going for simplicity

# Docker

- Use Docker for containerisation
- Create Dockerfile for Presentation layer (API and UI)
- Stretch: Deploy to Azure

# Locations

- Would usually use Geocoding but that's a config to setup for demo
- Just pulling key locations from an existing world city csv file

# Additional Notes

- Developing on MacOS so unable to verify on Visual Studio (which is what I would normally use for .NET)
- Utilising Visual Studio Code for development
