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

- SQL Server for database 
- Entity Framework Core for ORM
- Use in-memory caching for performance improvements

# Docker

- Use Docker for containerization
- Create Dockerfile for Presentation layer (API and UI)
- Stretch: Deploy to Azure

# Additional Notes

- Developing on MacOS so unable to verify on Visual Studio (which is what I would normally use for .NET)
- Utilising Visual Studio Code for development

