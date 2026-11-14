# JWT Login React Client

React frontend for the JWT Login and Role-Based Access API.

## Requirements

- Node.js and npm
- The ASP.NET Core API running locally

## Run locally

```bash
npm install
npm start
```

The development server runs on the default Create React App port (usually http://localhost:3000).

The login component currently sends requests to `https://localhost:7219/api/Auth/Login`. Update that API URL if the backend uses a different HTTPS port.

## Available scripts

- `npm start` — start the development server.
- `npm test` — run tests.
- `npm run build` — create a production build.
