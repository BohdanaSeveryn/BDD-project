# Building Facility Booking System

A complete web application for managing facility bookings in residential buildings. Built with .NET 8.0 backend and HTML/CSS/JavaScript frontend with Finnish language UI.

## 🎯 Project Overview

This is a BDD (Behavior-Driven Development) project implementing an MVP for a building facility booking system. Residents can book shared facilities (washing machines, laundry rooms) through a secure online platform, while administrators manage bookings and resident accounts.

## 📋 Features

### Resident Features
- ✅ Account registration and email activation
- ✅ Secure login with apartment number
- ✅ View facility availability calendar
- ✅ Book available time slots
- ✅ View personal booking history
- ✅ Cancel bookings
- ✅ Receive booking confirmation emails

### Administrator Features  
- ✅ Secure login with two-factor authentication (2FA)
- ✅ Create, read, update, delete resident accounts
- ✅ View all bookings on selected date
- ✅ Cancel bookings with reason notification
- ✅ Prevent double-booking with unique constraints
- ✅ Color-coded calendar (green=available, red=booked)

## 🛠️ Technology Stack

### Backend
- **Framework**: ASP.NET Core 8.0
- **Language**: C#
- **Database**: SQL Server (LocalDB for development)
- **ORM**: Entity Framework Core 8.0
- **Authentication**: JWT tokens + 2FA
- **Password Security**: BCrypt

### Frontend
- **Markup**: HTML5
- **Styling**: CSS3 (responsive design)
- **Scripting**: JavaScript (ES6+)
- **Language**: Finnish (Suomi)
- **HTTP Client**: Fetch API

### Testing
- **Framework**: xUnit
- **Mocking**: Moq
- **Assertions**: FluentAssertions
- **Specifications**: Gherkin BDD format

## 📂 Project Structure

```
BDD-project/
├── BookingSystem.Web/
│   ├── Controllers/              # API endpoints
│   ├── Services/                 # Business logic layer
│   ├── Data/                     # Database context & repositories
│   ├── Models/                   # Entity models
│   ├── DTOs/                     # Data transfer objects
│   ├── Utilities/                # Helper classes (JWT, Password, Email)
│   ├── Middleware/               # Custom middleware
│   ├── wwwroot/                  # Static files
│   │   ├── css/style.css         # Main stylesheet
│   │   ├── js/                   # JavaScript modules
│   │   │   ├── api-client.js     # API communication
│   │   │   ├── auth.js           # Authentication logic
│   │   │   ├── booking.js        # Booking functionality
│   │   │   └── admin.js          # Admin panel logic
│   │   ├── index.html            # Resident login
│   │   ├── activation.html       # Account activation
│   │   ├── booking.html          # Booking calendar
│   │   ├── my-bookings.html      # View bookings
│   │   ├── admin-login.html      # Admin login
│   │   ├── admin-dashboard.html  # Admin bookings
│   │   └── admin-residents.html  # Admin resident management
│   ├── Program.cs                # Application startup
│   ├── appsettings.json          # Development configuration
│   └── BookingSystem.Web.csproj  # Project file
│
├── BookingSystem.Tests/
│   ├── Features/                 # Feature tests
│   │   ├── AuthenticationTests.cs
│   │   ├── AccountManagementTests.cs
│   │   ├── BookingManagementTests.cs
│   │   ├── BookingViewTests.cs
│   │   ├── AdminBookingManagementTests.cs
│   │   └── NotificationTests.cs
│   ├── Fixtures/                 # Test data builders
│   │   ├── TestModelsAndInterfaces.cs
│   │   └── TestDataBuilders.cs
│   └── BookingSystem.Tests.csproj
│
├── BDD-project.sln               # Solution file
├── BUILD_GUIDE.md                # Detailed implementation guide
├── IMPLEMENTATION-GUIDE.md       # Setup and deployment guide
└── README.md                      # This file
```

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 SDK or later
- SQL Server 2019+ or SQL Server Express with LocalDB
- Visual Studio 2022 / Visual Studio Code (recommended)

### Installation

1. **Clone the repository**
```bash
git clone <repository-url>
cd BDD-project
```

2. **Restore NuGet packages**
```bash
dotnet restore
```

3. **Build the project**
```bash
dotnet build
```

4. **Create the database** (automatic with Entity Framework)
```bash
cd BookingSystem.Web
dotnet ef database update
```

5. **Run the application**
```bash
dotnet run
```

The application will be available at: **http://localhost:5000**

## 🧪 Running Tests

### All Tests
```bash
dotnet test
```

### Specific Test Class
```bash
dotnet test --filter "FullyQualifiedName=BookingSystem.Tests.Features.AuthenticationTests"
```

### With Detailed Output
```bash
dotnet test --verbosity detailed
```

### With Code Coverage
```bash
dotnet test /p:CollectCoverage=true
```

## 📝 Test Coverage

The project includes comprehensive BDD tests covering:

- **US-1, US-2, US-3**: Authentication (account activation, resident/admin login)
- **US-4, US-5, US-19, US-20**: Account management (create, delete, update residents)
- **US-6, US-7**: Availability & calendar viewing
- **US-9 through US-13**: Booking management (confirm, view, cancel, prevent double-booking)
- **US-16, US-17, US-18**: Admin booking operations
- **US-22, US-23**: Email notifications

## 🔑 Key Implementation Details

### Database Schema
- **Residents**: User accounts with activation tokens
- **Admins**: Administrator accounts with 2FA support
- **Facilities**: Bookable resources (washing machines, laundry rooms)
- **TimeSlots**: Available time slots for each facility
- **Bookings**: Confirmed reservations with cancellation tracking
- **AuditLogs**: Activity logging for compliance
- **EmailAuditLogs**: Email delivery tracking

### API Endpoints

#### Authentication
- `POST /api/auth/login` - Resident login
- `POST /api/auth/admin-login` - Admin login
- `POST /api/auth/activate` - Account activation
- `POST /api/auth/2fa/verify` - 2FA verification

#### Bookings
- `GET /api/bookings/availability` - Get available slots
- `POST /api/bookings/confirm` - Make a booking
- `GET /api/bookings/my-bookings/{residentId}` - View personal bookings
- `DELETE /api/bookings/{id}/cancel` - Cancel booking

#### Admin
- `GET /api/admin/bookings` - View all bookings
- `DELETE /api/admin/bookings/{id}/cancel` - Cancel booking
- `GET /api/admin/residents` - List all residents
- `POST /api/admin/residents` - Create resident
- `PUT /api/admin/residents/{id}` - Update resident
- `DELETE /api/admin/residents/{id}` - Delete resident

### Security Features
- JWT token-based authentication (1-hour expiration)
- Two-factor authentication for admins (6-digit codes)
- BCrypt password hashing with SHA384
- CORS properly configured
- Unique constraints on email, apartment number, phone
- Time slot occupancy lock-in mechanism

### Finnish Language UI
All frontend pages are in Finnish:
- Login: "Kirjautuminen"
- Booking: "Varaa Aika"
- Admin: "Ylläpitäjän Paneeli"
- Messages: Error/success notifications in Finnish

## ⚙️ Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BookingSystemDb;Trusted_Connection=true;"
  },
  "JwtSettings": {
    "Key": "your-secret-key-min-32-characters",
    "Issuer": "BookingSystem",
    "Audience": "BookingSystemUsers",
    "ExpirationMinutes": 60
  }
}
```

## 🔐 Default Test Credentials

### Admin Account
- **Username**: admin  
- **Password**: Admin@123

### Test Residents
Create residents through the admin panel or tests.

## 📚 Documentation

- [BUILD_GUIDE.md](Documents/BUILD_GUIDE.md) - Complete implementation specification
- [IMPLEMENTATION-GUIDE.md](Documents/IMPLEMENTATION-GUIDE.md) - Setup and deployment instructions
- Test files in `BookingSystem.Tests/Features/` - Expected behavior examples

## 🐛 Troubleshooting

### Database Connection Issues
```bash
# Verify connection string in appsettings.json
# For LocalDB, use: (localdb)\mssqllocaldb
```

### Port 5000 Already in Use
Edit `launchSettings.json` to use a different port or terminate the process:
```powershell
netstat -ano | findstr :5000
taskkill /PID <PID> /F
```

### CORS Errors
Ensure API client URL matches server URL in `wwwroot/js/api-client.js`:
```javascript
const apiClient = new ApiClient('http://localhost:5000/api');
```

## 📦 Deployment

### Production Build
```bash
dotnet publish -c Release -o ./publish
```

### IIS Deployment
1. Create IIS application pointing to `publish` folder
2. Configure .NET Core application pool
3. Update connection string in `appsettings.Production.json`
4. Set environment to `Production`

## 👥 Development Team

Built by a dedicated development team.

## 📄 License

Educational project - BDD Development Exercise

## 🤝 Contributing

1. Follow the existing code structure and naming conventions
2. Write tests for new features
3. Ensure all tests pass before submitting
4. Update documentation as needed

## ✅ Checklist for Full Implementation

- [x] Project structure created
- [x] All models implemented
- [x] Database schema with migrations
- [x] Service interfaces and implementations
- [x] API controllers with endpoints
- [x] JWT authentication configured
- [x] 2FA system implemented
- [x] Frontend HTML pages (Finnish)
- [x] CSS stylesheet with responsive design
- [x] JavaScript API client and logic modules
- [x] Password hashing with BCrypt
- [x] Email notification system
- [x] Comprehensive test suite
- [x] Configuration files
- [x] Documentation

## 📞 Support

For questions or issues:
1. Review test specifications in `BookingSystem.Tests/Features/`
2. Check detailed implementation in [BUILD_GUIDE.md](Documents/BUILD_GUIDE.md)
3. Refer to API documentation in controllers
4. Check configuration in `appsettings.json`

---

**Happy Coding! 🚀**
