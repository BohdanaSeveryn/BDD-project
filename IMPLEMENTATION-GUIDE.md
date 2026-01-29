# Building Facility Booking System - Implementation Guide

## Project Setup

This document provides instructions for building and running the Building Facility Booking System.

## Prerequisites

- .NET 8.0 SDK ([Download](https://dotnet.microsoft.com/en-us/download/dotnet/8.0))
- SQL Server 2019+ or SQL Server Express LocalDB
- Visual Studio 2022 or Visual Studio Code
- Git

## Building the Project

### 1. Clone the Repository and Restore NuGet Packages

```bash
git clone <repository-url>
cd BDD-project
dotnet restore
```

### 2. Build the Solution

```bash
dotnet build
```

### 3. Create the Database

The database will be created automatically on first run via Entity Framework Core:

```bash
# Navigate to the BookingSystem.Web directory
cd BookingSystem.Web

# Update database (creates it if it doesn't exist)
dotnet ef database update
```

### 4. Run the Application

```bash
dotnet run
```

The application will be available at `http://localhost:5000`

## Running Tests

### Run All Tests

```bash
dotnet test
```

### Run Tests with Verbose Output

```bash
dotnet test --verbosity detailed
```

### Run Specific Test Class

```bash
dotnet test --filter "FullyQualifiedName=BookingSystem.Tests.Features.AuthenticationTests"
```

## Project Structure

```
BookingSystem.Web/
├── Controllers/          # API endpoints
├── Services/            # Business logic
├── Data/               # Database context & repositories
├── Models/             # Entity models
├── DTOs/               # Data transfer objects
├── Utilities/          # Helper classes
├── wwwroot/            # Static files (HTML, CSS, JS)
│   ├── css/
│   ├── js/
│   └── images/
├── Program.cs          # Application startup
├── appsettings.json    # Configuration

BookingSystem.Tests/
├── Features/           # Feature tests
└── Fixtures/          # Test fixtures & builders
```

## Configuration

Edit `appsettings.json` to configure:

- **Connection String**: Database connection
- **JWT Settings**: Token configuration
- **Email Settings**: SMTP configuration

## Default Users for Testing

### Admin User
- **Username**: admin
- **Password**: Admin@123

### Test Residents
Test residents are created through the admin panel.

## Available Pages

### Resident Pages
- `/index.html` - Login
- `/activation.html` - Account activation
- `/booking.html` - Make bookings
- `/my-bookings.html` - View your bookings

### Admin Pages
- `/admin-login.html` - Admin login
- `/admin-dashboard.html` - Manage bookings
- `/admin-residents.html` - Manage residents

## Key Features Implemented

✅ **Authentication**
- Resident login with apartment number
- Admin login with 2FA
- Account activation via email token

✅ **Booking Management**
- View availability calendar
- Make bookings with double-booking prevention
- Cancel bookings
- View booking history

✅ **Admin Features**
- Create/update/delete residents
- View all bookings
- Cancel bookings with reason notification
- Two-factor authentication (2FA)

✅ **Notifications**
- Email on account activation
- Booking confirmation emails
- Account deletion notifications

## Troubleshooting

### Database Issues
```bash
# Drop and recreate database
dotnet ef database drop
dotnet ef database update
```

### Port Already in Use
Edit `launchSettings.json` to use a different port, or kill the process using port 5000.

### CORS Errors
Ensure the API client URL matches the server URL in `wwwroot/js/api-client.js`.

## Deployment

### Production Build
```bash
dotnet publish -c Release -o ./publish
```

### IIS Deployment
1. Create a new IIS website
2. Point physical path to `publish` folder
3. Configure application pool for .NET Core
4. Update connection strings in `appsettings.Production.json`

## Support

For issues or questions:
1. Check test files in `BookingSystem.Tests/Features/` for expected behavior
2. Review `BUILD_GUIDE.md` for detailed implementation specs
3. Check `appsettings.json` for configuration issues
