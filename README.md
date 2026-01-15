# Car Rental System

A web-based Car Rental system built using ASP.NET that demonstrates a complete backend–frontend setup with database integration.

This project is structured as a single system consisting of:
- A backend API
- A frontend MVC client
- SQL database scripts

---

## Features
- User authentication using JWT
- Car listing and availability management
- Rental booking workflow
- Backend API consumed by a frontend MVC application
- Database access using SQL Server and stored procedures
- Swagger for API testing

---

## Project Structure

```

CarRental/              → Backend API (ASP.NET)
CarRentalFrontEnd/      → Frontend MVC application
CarRental_Query/        → SQL scripts and stored procedures

```

---

## Tech Stack
- ASP.NET Core
- C#
- SQL Server
- JWT Authentication
- Swagger
- Razor Views (Frontend)

---

## Backend (CarRental)
- Exposes REST APIs for car rental operations
- Uses JWT for authentication and authorization
- Configured with CORS and Swagger
- Connects to SQL Server using a configurable connection string

---

## Frontend (CarRentalFrontEnd)
- ASP.NET MVC application
- Acts as a client consuming backend APIs
- Uses cookie-based authentication
- Communicates with backend via HttpClient

---

## Database
- SQL Server is used as the database
- Stored procedures are provided in the `CarRental_Query` folder
- These scripts must be executed before running the application

---

## Configuration

### Database Connection
The project uses environment-based configuration.

- `appsettings.json` contains a generic connection string
- Local machine configuration should be placed in:
```

appsettings.Development.json

```
(This file is ignored by Git and not included in the repository)

---

## How to Run the Project

1. Clone the repository
2. Open the solution in Visual Studio
3. Run the SQL scripts from `CarRental_Query` on your SQL Server
4. Update the connection string in `appsettings.Development.json`
5. Start the backend project
6. Start the frontend project
7. Access the application via the frontend URL

---

## Notes
- Build folders (`bin`, `obj`) and user-specific files are excluded using `.gitignore`
- Secrets and environment-specific values are not committed to the repository
