TalentPlus HR Management System
Overview

TalentPlus HR Management System is a comprehensive web application and REST API built with ASP.NET Core for managing employee information, automating HR processes, and providing intelligent insights. The platform offers features such as employee registration, department management, bulk data import, and dynamic PDF report generation.

Features
Web Application (HR Admin Portal)

Employee Management: Full CRUD operations for employee records.

Excel Import: Upload and process Excel files to update the employee database.

PDF Generation: Dynamic generation of employee CVs and resumes in PDF format.

Admin Authentication: ASP.NET Core Identity for admin login and user management.

Role-based Access: Admins and users can be assigned specific permissions.

REST API

Public Endpoints:

List departments.

Employee self-registration.

Employee login (JWT authentication).

Protected Endpoints (JWT required):

Retrieve employee information.

Download personal CV in PDF format.

Real Email Integration: Sends welcome emails via SMTP.

Technology Stack

Backend: ASP.NET Core 8.0

Database: PostgreSQL 15+

Authentication: ASP.NET Core Identity + JWT

PDF Generation: iTextSharp or similar

Excel Processing: EPPlus or ClosedXML

Email Service: SMTP (e.g., Gmail, SendGrid)

Containerization: Docker & Docker Compose

Testing: xUnit/NUnit

Swagger UI: Interactive UI for testing and documenting the API.

Architecture

Pattern: Repository Pattern with Clean Architecture.

Layers:

Presentation Layer: Web API + MVC (User Interface layer).

Application/Business Layer: Core business logic.

Domain Layer: Domain entities and rules.

Infrastructure Layer: Database integration, authentication, and external services.

Separation of Concerns: A modular structure with a clear division of responsibilities.

Prerequisites

Before running the application, ensure that the following tools are installed:

.NET 8.0 SDK: To run the ASP.NET Core application.

Docker & Docker Compose: For containerization and orchestration.

PostgreSQL 15+: Relational database.

SMTP Credentials: For sending emails (e.g., Gmail, SendGrid).

Setup Instructions
Environment Configuration

Database Configuration
## Setup Instructions

# Database
DB_HOST=localhost
DB_PORT=5432
DB_NAME=talentplus_db
DB_USER=postgres
DB_PASSWORD=YourStrongPassword123

# JWT Configuration
JWT_KEY=YourSuperSecretKeyHereMinimum32CharactersLong!
JWT_ISSUER=TalentPlus
JWT_AUDIENCE=TalentPlusClients

# AI Configuration (Gemini)
GEMINI_API_KEY=your_gemini_api_key_here

# Email Configuration
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_USER=your-email@gmail.com
SMTP_PASSWORD=your-app-specific-password
SMTP_FROM=noreply@talentplus.com

# Application URLs
WEB_URL=http://localhost:5000
API_URL=http://localhost:5001


docker-compose up --build
Access Credentials
---
##API Endpoints
## Auth
| Method | Endpoint              | Description                  |
|--------|------------------------|------------------------------|
| POST   | /api/Auth/register    | Register a new employee      |
| POST   | /api/Auth/login       | Log in and receive a JWT     |


### Catalogs
- **GET** `/api/Catalogs/positions`
- **GET** `/api/Catalogs/education-levels`
- **GET** `/api/Catalogs/employee-statuses`
- **GET** `/api/Catalogs/all`

### Departments
- **GET** `/api/Departments`
- **GET** `/api/Departments/{id}`


### EducationLevels
- **GET** `/api/EducationLevels`
- **POST** `/api/EducationLevels`
- **GET** `/api/EducationLevels/{id}`
- **PUT** `/api/EducationLevels/{id}`
- **DELETE** `/api/EducationLevels/{id}`

### Employees
- **POST** `/api/Employees/import-excel`
- **GET** `/api/Employees`
- **POST** `/api/Employees`
- **GET** `/api/Employees/{id}`
- **PUT** `/api/Employees/{id}`
- **DELETE** `/api/Employees/{id}`
- **GET** `/api/Employees/{id}/resume-pdf`


### EmployeeStatuses
- **GET** `/api/EmployeeStatuses`
- **GET** `/api/EmployeeStatuses/{id}`



### Positions
- **GET** `/api/Positions`
- **POST** `/api/Positions`
- **GET** `/api/Positions/{id}`
- **PUT** `/api/Positions/{id}`
- **DELETE** `/api/Positions/{id}`

---


## API Access
Base URL: http://localhost:5001

Swagger UI: http://localhost:5001/swagger

---
Running the Application

Build Docker Containers:
Run the following command to build the containers and start the application:

docker-compose up --build


Access the HR Admin Portal:
URL: http://localhost:5000


Access the API:
API Base URL: http://localhost:5001

Swagger UI: http://localhost:5001/swagger
