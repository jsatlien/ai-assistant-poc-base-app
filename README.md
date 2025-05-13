# Repair Manager Application

A modern repair management system built with Vue.js and .NET Core, designed to track device repairs through structured workflows, work orders, and repair programs.

## Project Overview

This application consists of two main components:

1. **Frontend**: Vue.js application with Inertia.js
2. **Backend**: .NET Core Web API with SQLite database

## Features

- **Repair Catalog**: Manage devices, services, and parts
- **Work Orders**: Create, track, and update repair work orders
- **Repair Workflows**: Define custom repair statuses and workflows
- **Repair Programs**: Create repair programs with specific workflows
- **Authentication**: Secure login system

## Getting Started

### Prerequisites

- .NET Core 6.0 SDK
- Node.js and npm
- Vue CLI

### Backend Setup

1. Navigate to the backend directory:
   ```
   cd RepairManager/Backend/RepairManagerApi
   ```

2. Restore the .NET packages:
   ```
   dotnet restore
   ```

3. Run the API:
   ```
   dotnet run
   ```

The API will be available at https://localhost:7001 and http://localhost:5001.

### Frontend Setup

1. Navigate to the frontend directory:
   ```
   cd RepairManager/Frontend/repair-manager-frontend
   ```

2. Install the dependencies:
   ```
   npm install
   ```

3. Run the development server:
   ```
   npm run serve
   ```

The frontend will be available at http://localhost:8080.

## Authentication

For demo purposes, use the following credentials:
- Username: `admin`
- Password: `admin123`

## API Endpoints

### Catalog
- `GET /api/catalog/devices` - Get all devices
- `GET /api/catalog/services` - Get all services
- `GET /api/catalog/parts` - Get all parts

### Work Orders
- `GET /api/workorders` - Get all work orders
- `POST /api/workorders` - Create a new work order
- `GET /api/workorders/{id}` - Get a specific work order
- `PUT /api/workorders/{id}` - Update a work order

### Workflows
- `GET /api/workflows` - Get all workflows
- `POST /api/workflows` - Create a new workflow
- `GET /api/workflows/{id}` - Get a specific workflow
- `PUT /api/workflows/{id}` - Update a workflow

### Programs
- `GET /api/programs` - Get all programs
- `POST /api/programs` - Create a new program
- `GET /api/programs/{id}` - Get a specific program
- `PUT /api/programs/{id}` - Update a program

## Project Structure

### Backend
```
RepairManagerApi/
├── Controllers/      # API controllers
├── Models/           # Data models
├── Data/             # Database context
└── Program.cs        # Application configuration
```

### Frontend
```
repair-manager-frontend/
├── src/
│   ├── Pages/        # Vue components for pages
│   ├── Shared/       # Shared components
│   ├── assets/       # Static assets
│   ├── router/       # Vue Router configuration
│   └── store/        # Vuex store
└── public/           # Public static files
```

## Docker Support

The application can be containerized using Docker. Docker files are provided for both frontend and backend components.

## License

This project is licensed under the MIT License.
