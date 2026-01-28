# Building Facility Booking System

**Project**: Educational project using the BDD development approach

**Developers**:
- Anni Myllyniemi
- Usama Shahla
- Eero Hirsimäki
- Bohdana Severyn

---

## System Specification Document

### 1. Purpose of the System

The system allows residents of a building to book shared facilities (e.g., sauna, laundry room) through a secure online platform. The system ensures:

- Reliable identity verification
- Transparent booking management
- Minimal administrative workload
- Clear communication between residents and the administrator

### 2. User Roles

#### 2.1 Resident

A resident can:

- Log into their personal account
- View available facilities and time slots
- Make bookings
- View all their bookings
- Cancel bookings
- Report issues or malfunctions

#### 2.2 Administrator

An administrator can:

- Log in with two-factor authentication
- Manage resident accounts
- View all bookings
- Cancel bookings
- Mark facilities as "out of use"
- Receive and manage problem reports

### 3. Authentication and User Management

#### 3.1 Resident Accounts

- Residents do not register manually
- Accounts are created manually when resident is moving in
- The system imports:
  - Resident name
  - Apartment number
  - Email
  - Phone number
- The resident receives an automatic email with a link to set their password
- After activation, the resident logs in using email/phone + password

#### 3.2 Administrator Accounts

- Created manually by building management
- Login requires:
  - Username + password
  - Two-factor authentication (work phone or work email)

#### 3.3 Move-out Procedure

- Administrator removes the resident from the system
- The resident receives an automatic email notification

### 4. Booking Process

#### 4.1 Access

- The resident logs into their personal account
- The system automatically knows their name, apartment number, email, and phone

#### 4.2 Booking Flow

- Resident opens the booking calendar
- Resident selects a facility
- Resident sees available time slots (green)
- Resident clicks a slot → it becomes blue (pending confirmation)
- Pressing the "confirm booking" button:
  - If confirmed → slot becomes reserved
  - If another user books the slot first → the pending user sees the slot turn red and the booking is cancelled

#### 4.3 Booking Confirmation

- After successful confirmation, in his personal account, a booking mark appears in the calendar with the icon of the facility (washing machine, dryer, barbecue, sauna)
- By clicking on the icon, the user sees the details of the booking:
  - Facility name
  - Date and time
  - Booking cancellation button

### 5. Viewing and Managing Bookings

#### 5.1 My Bookings Page

Residents can see:

- All upcoming bookings
- Booking status
- Cancellation button
- Facility details

#### 5.2 Cancelling a Booking

- If logged in → cancellation happens directly
- If using email link → cancellation requires entering the received code

### 6. Administrator Features

#### 6.1 Booking Management

- View bookings in a calendar view
- Click on a booking to see resident details
- Cancel bookings if needed

#### 6.2 Facility Status

Administrator can set a facility to "Out of use" via a dedicated option. This blocks all time slots automatically.

#### 6.3 Messaging

Administrator can:

- Send individual messages
- Send bulk announcements

#### 6.4 Invoices

- Automatically generated monthly
- Administrator can view and edit them
- Used for rent billing

### 7. Cancellation Policy

- Free cancellation up to X hours before the booking
- After that:
  - Cancellation may be blocked
  - Or a fee may apply
  - Or marked as "no-show"

(The building management chooses the exact rule.)

### 8. System Notifications

**Residents receive notifications for**:

- Account activation
- Booking confirmation
- Booking cancellation
- Booking failure
- Issue report confirmation
- Account deletion after move-out

**Administrators receive notifications for**:

- Issue reports
- Facility status changes
- System errors

### 9. Summary of Key Design Decisions

These are the best choices selected from the discussions:

- ✔ Residents do not register manually — accounts are created automatically after signing the rental agreement
- ✔ Residents log into a personal account, so they do not enter name/email/apartment manually
- ✔ Booking confirmation uses email or SMS codes
- ✔ Color-coded booking flow (green → blue → red) is adopted
- ✔ Booking key exists only as a fallback for email-based cancellation
- ✔ Administrator has a full dashboard with calendar, messages, issue reports, and facility status
- ✔ Facility "Out of use" is a dedicated admin option, not a fake reservation
- ✔ Monthly invoices are generated automatically

