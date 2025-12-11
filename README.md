# TalentPlus HR Management System

## Overview
TalentPlus HR Management System is a comprehensive web application and REST API built with ASP.NET Core for managing employee information, automating HR processes, and supporting key HR workflows such as PDF generation, Excel data imports, employee records, and secure authentication.

---

## Features

### Web Application (HR Admin Portal)
- **Employee Management** – Full CRUD operations.
- **Excel Import** – Upload and process Excel files to update the employee database.
- **PDF Generation** – Dynamic CV/Resume generation in PDF format.
- **Admin Authentication** – Built using ASP.NET Core Identity.
- **Role-based Access Control** – Secure user and admin permissions.

### REST API

#### Public Endpoints
- List departments.
- Employee self-registration.
- Employee login (JWT authentication).

#### Protected Endpoints (JWT required)
- Retrieve employee information.
- Download employee CV in PDF format.

### Real Email Integration
- Sends welcome emails via SMTP.

---

## Technology Stack
- **Backend:** ASP.NET Core 8.0  
- **Database:** PostgreSQL  
- **Authentication:** ASP.NET Core Identity + JWT  
- **PDF Generation:** iTextSharp or similar  
- **Excel Processing:** EPPlus or ClosedXML  
- **Email Service:** SMTP  
- **Containerization:** Docker & Docker Compose  
- **Testing:** xUnit / NUnit  

---

## Architecture
- **Pattern:** Repository Pattern with Clean Architecture  
- **Layers:**
  - Presentation Layer (Web API + MVC)
  - Application / Business Layer
  - Domain Layer
  - Infrastructure Layer
- **Separation of Concerns:** Clear and modular project structure.

---

## Prerequisites
Before running the application, ensure the following are installed:

- .NET 8.0 SDK  
- Docker & Docker Compose  
- PostgreSQL 15+  
- SMTP credentials (Gmail, Outlook, SendGrid, etc.)  

---

## Setup Instructions

### Environment Configuration


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

# Email Configuration
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_USER=your-email@gmail.com
SMTP_PASSWORD=your-app-specific-password
SMTP_FROM=noreply@talentplus.com

# Application URLs
WEB_URL=http://localhost:5000
API_URL=http://localhost:5001


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
