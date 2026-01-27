# Booking System - User Interface

A modern, responsive web interface for the Booking System built with HTML, CSS, and JavaScript. Features a beautiful light mint color scheme.

## 🎨 Features

- **Responsive Design**: Works seamlessly on desktop, tablet, and mobile devices
- **Light Mint Color Scheme**: Calming and professional interface
- **Multiple User Roles**: 
  - Resident - Create and manage personal bookings
  - Admin - Oversee all bookings and residents
- **Real-time API Integration**: Connects to the Booking System API
- **Two-Factor Authentication**: Admin login with 2FA for security
- **Modern UI Components**: Cards, modals, tables, forms, and alerts

## 📁 Project Structure

```
BookingSystem.UI/
├── index.html                      # Welcome/login selection page
├── css/
│   └── styles.css                 # Global styles with mint color theme
├── js/
│   └── api-client.js              # API client and utility functions
└── pages/
    ├── resident-login.html        # Resident login
    ├── resident-register.html     # Resident registration
    ├── resident-dashboard.html    # Resident booking dashboard
    ├── admin-login.html           # Admin login with 2FA
    └── admin-dashboard.html       # Admin management dashboard
```

## 🎯 Color Scheme

The interface uses a carefully selected light mint palette:

- **Primary Mint**: `#A8E6D5` - Main accent color
- **Primary Dark**: `#20C997` - Headers and highlights
- **Light Mint**: `#E0F7F4` - Hover states
- **Very Light**: `#F0F9F7` - Backgrounds
- **Text Dark**: `#2C3E50` - Primary text
- **Text Light**: `#7F8C8D` - Secondary text

## 🚀 Getting Started

### Prerequisites
- A running instance of the Booking System API (http://localhost:5000)
- A modern web browser (Chrome, Firefox, Safari, Edge)

### Installation

1. Open the UI folder in your browser or run a local server:
   ```bash
   # Using Python 3
   python -m http.server 8000
   
   # Or using Node.js (http-server)
   npx http-server
   ```

2. Navigate to `http://localhost:8000` or `http://localhost:3000` in your browser

3. Click on your user type (Resident or Admin) to begin

### API Configuration

The API base URL is set in `js/api-client.js`:

```javascript
const API_BASE = 'http://localhost:5000/api';
```

Update this if your API is running on a different host/port.

## 📄 Pages Overview

### Welcome Page (`index.html`)
- Login/Register selection for residents
- Admin login option
- API status indicator

### Resident Login (`resident-login.html`)
- Apartment number and password login
- Account activation status check
- Registration link

### Resident Registration (`resident-register.html`)
- New account creation
- Activation email automatic trigger
- Form validation

### Resident Dashboard (`resident-dashboard.html`)
- View upcoming bookings
- Create new bookings
- Cancel bookings
- Browse available time slots
- Select facility and date

### Admin Login (`admin-login.html`)
- Two-step login process
- Username/password entry
- 2FA code verification
- Email-based code delivery

### Admin Dashboard (`admin-dashboard.html`)
- Dashboard with quick stats
- Booking management (view all, filter by date, cancel)
- Resident management
- Create new resident accounts

## 🔧 JavaScript Modules

### API Client (`api-client.js`)

Handles all communication with the backend API:

```javascript
// Resident operations
ApiClient.loginResident(apartmentNumber, password)
ApiClient.registerResident(data)
ApiClient.getResident(id)
ApiClient.updateResident(id, data)

// Booking operations
ApiClient.getAvailability(facilityId, date)
ApiClient.confirmBooking(data)
ApiClient.cancelBooking(bookingId, residentId, reason)
ApiClient.getMyBookings(residentId, date)
ApiClient.getUpcomingBookings(residentId)

// Admin operations
ApiClient.adminLogin(username, password)
ApiClient.verify2FA(adminId, code)
ApiClient.createResident(adminId, data)
ApiClient.deleteResident(adminId, residentId)
ApiClient.getAllBookings(adminId, date)
```

### Session Manager

Manages user authentication state:

```javascript
SessionManager.setSession(type, data)  // Store login info
SessionManager.getSession()             // Retrieve session
SessionManager.clearSession()           // Logout
SessionManager.isLoggedIn()             // Check auth status
```

### Utility Functions

```javascript
showAlert(message, type)    // Display notification
showModal(modalId)          // Open modal dialog
closeModal(modalId)         // Close modal
setLoading(element, bool)   // Loading state UI
formatDate(dateString)      // Format dates
formatTime(timeString)      // Format times
requireLogin(userType)      // Protect pages
```

## 🎨 Responsive Design

The interface is fully responsive with breakpoints for:
- Desktop (1200px+)
- Tablet (768px - 1199px)
- Mobile (under 768px)

All cards, forms, and tables adapt to screen size automatically.

## 🔐 Security Features

- **Session Management**: Local storage-based authentication
- **Two-Factor Authentication**: Admin login includes 2FA
- **Password Fields**: Proper input masking
- **Authorization Checks**: Role-based page access
- **Token Handling**: API tokens stored securely

## 📱 Browser Support

- Chrome 90+
- Firefox 88+
- Safari 14+
- Edge 90+

## 🤝 Integration with API

The UI is designed to work with the Booking System REST API. Ensure all API endpoints match the documented specifications in the API README.

### Example: Creating a Booking

```javascript
// User selects facility, date, and time
const booking = {
  residentId: '123',
  facilityId: 1,
  date: '2024-01-25',
  timeSlotId: 'slot-456'
};

// API call
const result = await ApiClient.confirmBooking(booking);

// Success feedback
showAlert('Booking confirmed!', 'success');
```

## 🐛 Troubleshooting

### API Connection Issues
- Verify API is running on localhost:5000
- Check browser console (F12) for error messages
- Ensure CORS is enabled on the API

### Login Problems
- Clear browser cookies/localStorage: `localStorage.clear()`
- Verify credentials in the API
- Check email for activation links (resident accounts)

### Booking Issues
- Ensure date is in correct format (YYYY-MM-DD)
- Verify facility ID exists
- Check if time slots are available

## 📝 License

This project is part of the Booking System solution.

## 📧 Support

For issues or feature requests, contact the development team.
