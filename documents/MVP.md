# Building Facility Booking System - MVP (Version 1.0)

**Project**: Educational project using the BDD development approach

**Developers**:
- Anni Myllyniemi
- Usama Shahla
- Eero Hirsimäki
- Bohdana Severyn

---

## 1. Core Purpose of Version 1.0

Version 1.0 provides the essential functionality for residents to:

- Log in
- View available Washing Machine and book a time slot
- View and cancel their bookings

And for administrators to:

- Log in
- Manage residents
- View and cancel bookings

## 2. User Roles (MVP)

### 2.1 Resident

In Version 1.0, a resident can:

- Log into their personal account
- View available Washing Machine and time slots
- Make a booking
- View their own bookings
- Cancel their own bookings

### 2.2 Administrator

In Version 1.0, an administrator can:

- Log in with two-factor authentication
- Add and remove residents
- View all bookings
- Cancel any booking

## 3. Authentication & User Management (MVP)

### 3.1 Resident Accounts

- Created manually by the administrator when a resident moves in
- Required fields:
  - Name
  - Apartment number
  - Email
  - Phone number
- System sends an activation email with a link to set a password
- Resident logs in using email/phone + password

### 3.2 Administrator Accounts

- Created manually by building management
- Login requires:
  - Username + password
  - Two-factor authentication (email or phone)

### 3.3 Move-out

- Administrator deletes the resident's account
- System sends an automatic notification email

## 4. Booking Process (MVP)

### 4.1 Access

- Resident logs into their personal account
- System automatically knows their stored profile data

### 4.2 Booking Flow

- Resident opens the booking calendar
- Resident selects a facility
- Resident sees available time slots (green)
- Resident clicks a slot → it becomes "pending"
- Resident presses Confirm booking
- If the slot is still free → booking is created
- If another resident booked it first → system shows a message and the slot becomes unavailable

### 4.3 Booking Confirmation

After successful booking:

- A booking icon appears in the resident's calendar
- Clicking the icon shows:
  - Facility name
  - Date & time
  - Cancel booking button

## 5. Viewing & Managing Bookings (MVP)

### 5.1 My Bookings Page

Resident sees:

- All upcoming bookings
- Facility name
- Date & time
- Cancellation button

### 5.2 Cancelling a Booking

- If logged in → cancellation happens instantly
- System sends a confirmation email

## 6. Administrator Features (MVP)

### 6.1 Booking Management

Administrator can:

- View all bookings in a calendar
- Click a booking to see resident details
- Cancel any booking

### 6.2 Resident Management

Administrator can:

- Add new residents
- Delete residents
- Edit resident information





