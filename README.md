# TalentPlus HR Management System

## Overview
TalentPlus HR Management System is a comprehensive web application and REST API built with ASP.NET Core for managing employee information, automating HR processes, and providing intelligent insights through AI integration.

## Features

###  Web Application (HR Admin Portal)
- **Employee Management**: Full CRUD operations for employees
- **Excel Import**: Upload and process Excel files to update employee database
- **PDF Generation**: Dynamic CV/Resume generation for employees
- **AI-Powered Dashboard**: 
  - Real-time statistics (total employees, vacation status, etc.)
  - Natural language query interface using AI (Gemini/OpenAI)
- **Admin Authentication**: ASP.NET Core Identity

###  REST API
- **Public Endpoints**:
  - List departments
  - Employee self-registration
  - Employee login (JWT authentication)
- **Protected Endpoints** (JWT required):
  - Get employee information
  - Download personal CV in PDF format
- **Real Email Integration**: Welcome emails sent via SMTP

## Technology Stack
- **Backend**: ASP.NET Core 8.0
- **Database**: PostgreSQL
- **Authentication**: ASP.NET Core Identity + JWT
- **AI Integration**: Google Gemini (or alternative)
- **PDF Generation**: iTextSharp or similar
- **Excel Processing**: EPPlus or ClosedXML
- **Email Service**: SMTP integration
- **Containerization**: Docker & Docker Compose
- **Testing**: xUnit/NUnit

## Architecture
- **Pattern**: Repository Pattern with Clean Architecture
- **Layers**:
  - Presentation Layer (Web API + MVC)
  - Application/Business Layer
  - Domain Layer
  - Infrastructure Layer
- **Separation of Concerns**: Clear division between presentation, domain, and infrastructure

## Prerequisites
- .NET 8.0 SDK
- Docker & Docker Compose
- PostgreSQL 15+
- Google Gemini API key (for AI features)
- SMTP credentials (for email service)

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
Default Admin Account
URL: http://localhost:5000

Username: admin@talentplus.com

Password: Admin@123

API Access
Base URL: http://localhost:5001

Swagger UI: http://localhost:5001/swagger

---
