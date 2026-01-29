# Building Facility Booking System - Gherkin Scenarios

**Project**: Educational project using the BDD development approach

**Developers**: Built by a dedicated development team

---

## US-1: Resident account activation

**User Story**

As a resident, I want to receive an activation email and set my password, so that I can access my personal account securely.

**Criteria**

- An activation email is sent when the account is created
- The email contains a unique activation link
- The activation link is time-limited
- The resident must set a password to activate the account
- Login is not possible before activation

**Scenario: Successful account activation**

```gherkin
Given a resident account has been created and is inactive
When the resident opens the activation email
And clicks the activation link
And sets a valid password
Then the account becomes active
And the resident can log in
```

---

## US-2: Resident login

**User Story**

As a resident, I want to log in using my apartment number and password, so that I can manage my bookings.

**Criteria**

- Resident can log in using apartment number
- Password is required
- Only active accounts can log in
- Invalid credentials show an error message
- Successful login redirects to the resident dashboard

**Scenario: Successful login**

```gherkin
Given the resident account is active
When valid login credentials are entered
Then the resident is logged in
And redirected to the dashboard
```

**Scenario: Login fails for inactive account**

```gherkin
Given the resident account is inactive
When the resident attempts to log in
Then access is denied
```

---

## US-3: Admin login

**User Story**

As an administrator, I want to log in using username, password, and two-factor authentication, so that I can securely access the admin panel.

**Criteria**

- Username and password are required
- Two-factor authentication is mandatory
- 2FA code is sent via email or phone
- Admin access is granted only after successful 2FA

**Scenario: Successful admin login with 2FA**

```gherkin
Given the administrator account exists
When the administrator enters valid username and password
And enters the correct 2FA code
Then access to the admin panel is granted
```

---

## US-4: Admin creates a resident account

**User Story**

As an administrator, I want to manually create a resident account with name, apartment number, email, and phone, so that the resident can start using the booking system when moving in.

**Criteria**

- Admin must be logged in
- Admin can enter name, apartment number, email, and phone
- Email and phone must be unique in the system
- Resident account is created as inactive initially
- System sends an activation email to the resident
- Resident cannot log in until the account is activated

**Scenario: Successful account creation**

```gherkin
Given the administrator is logged in
When the administrator enters valid name, apartment number, email, and phone
And clicks "Create Account"
Then the resident account is created as inactive
And an activation email is sent to the resident
```

**Scenario: Attempt to create account with duplicate email or phone**

```gherkin
Given the administrator is logged in
When the administrator enters an email or phone already used by another resident
Then the system rejects the creation
And displays an error message
```

**Scenario: Resident tries to log in before activation**

```gherkin
Given a resident account is created but not activated
When the resident attempts to log in
Then access is denied
And a message indicates the account is not activated
```

---

## US-5: Admin deletes a resident account

**User Story**

As an administrator, I want to delete a resident account when the resident moves out, so that former residents no longer have access to the system.

**Criteria**

- Admin must be logged in
- Admin can select an existing resident account
- Deleted resident account immediately loses access
- System sends a notification email to the resident
- Historical bookings remain in the system for records

**Scenario: Successful deletion of a resident account**

```gherkin
Given the administrator is logged in
And the resident account exists
When the administrator deletes the resident account
Then the resident can no longer log in
And a notification email is sent to the resident
```

**Scenario: Attempt to delete a non-existent account**

```gherkin
Given the administrator is logged in
When the administrator tries to delete a resident account that does not exist
Then the system shows an error message
```

**Scenario: Deleted resident tries to log in**

```gherkin
Given the resident account has been deleted
When the former resident attempts to log in
Then access is denied
And a message indicates the account no longer exists
```

---

## US-6: Resident views washing machine availability

**User Story**

As a resident, I want to see the washing machine and its available time slots, so that I can choose a suitable time to book it.

**Scenario: Resident sees the washing machine and its available time slots**

```gherkin
Given the resident has an active account
And the resident is logged into their personal account
When the resident opens the booking calendar
And selects the washing machine facility
Then the system displays the washing machine's booking calendar
And the resident sees all available time slots marked as available
And the resident sees all unavailable time slots marked as unavailable
So that the resident can choose a suitable time to book it
```

**Scenario: Resident views availability for a different day**

```gherkin
Given the resident is logged into their personal account
And the washing machine booking calendar is open
When the resident navigates to a different date in the calendar
Then the system loads the availability for the selected date
And the resident sees available and unavailable time slots for that day
So that the resident can choose a suitable time on another date
```

**Scenario: Resident tries to view availability without being logged in**

```gherkin
Given the resident is not logged into the system
When the resident attempts to open the washing machine booking calendar
Then the system redirects the resident to the login page
And displays a message indicating that login is required
So that only authenticated residents can view availability
```

---

## US-7: Resident views booking calendar

**User Story**

As a resident I want to see a calendar with available (green) and unavailable time slots, so that I can understand when the washing machine is free.

**Scenario: Viewing the calendar for the current day**

```gherkin
Given I am logged into the housing portal as a resident
And I navigate to the "Washing Machine Booking" page
When I view the calendar for today
Then I should see a list of all hourly time slots
And slots that have no bookings should be highlighted in green
And slots that are already booked by others should be highlighted as unavailable
```

**Scenario: Distinguishing between free and reserved slots**

```gherkin
Given the time slot "14:00 - 15:00" is not booked
And the time slot "15:00 - 16:00" is already reserved by another resident
When I look at the booking calendar
Then the "14:00 - 15:00" slot should be labeled as "Available" with a green background
And the "15:00 - 16:00" slot should be labeled as "Reserved" and be unselectable
```

**Scenario: Checking availability for a future date**

```gherkin
Given I am viewing the booking calendar
When I select a date in the future from the date picker
Then the calendar should update to show the availability for that specific day
And I should be able to see which hours are green and which are unavailable
```

---

## US-8: Resident selects a time slot

**User Story**

As a resident I want to click on an available time slot, so that I can start the booking process.

**Scenario: Selecting an available time slot**

```gherkin
Given I am viewing the booking calendar
And the time slot "10:00 - 11:00" is marked as green (available)
When I click on the "10:00 - 11:00" time slot
Then the slot should be visually highlighted as selected
And I should see a "Confirm Booking" button or a summary of the selection
```

**Scenario: Changing selection before confirming**

```gherkin
Given I have already selected the time slot "10:00 - 11:00"
When I click on a different available time slot "12:00 - 13:00"
Then the selection should move from the first slot to the new slot
And only "12:00 - 13:00" should be highlighted as selected
```

**Scenario: Attempting to select an unavailable slot**

```gherkin
Given the time slot "08:00 - 09:00" is marked as unavailable
When I try to click on the "08:00 - 09:00" slot
Then nothing should happen
And the slot should not be highlighted as selected
```

---

## US-9: Resident confirms booking

**User Story**

As a resident I want to press a "Confirm booking" button, so that the system finalizes my reservation.

**Scenario: Successful booking confirmation**

```gherkin
Given I have selected an available time slot "10:00 - 11:00"
And I can see the "Confirm Booking" button
When I click the "Confirm Booking" button
Then the system should save my reservation
And I should see a confirmation message "Booking successful!"
And the time slot should now appear as unavailable to other users
```

**Scenario: Canceling before confirmation**

```gherkin
Given I have selected a time slot "14:00 - 15:00"
When I click a "Cancel" or "Clear selection" button
Then the selection should be removed
And no reservation should be created in the system
And the time slot should remain green (available)
```

**Scenario: Booking fails due to a connection error**

```gherkin
Given I have selected a time slot "16:00 - 17:00"
And there is a technical issue with the server
When I click the "Confirm Booking" button
Then I should see an error message "Booking failed, please try again later"
And the time slot should not be reserved under my name
```

---

## US-10: Prevent double-booking

**User Story**

As a resident I want to be notified if someone else booked the slot before I confirmed, so that I don't create an invalid or duplicate booking.

**Scenario: Slot becomes unavailable during the booking process**

```gherkin
Given I have selected the time slot "11:00 - 12:00"
And another resident confirms a booking for the same "11:00 - 12:00" slot while I am still on the confirmation page
When I click the "Confirm Booking" button
Then the system should not create a second reservation
And I should see an alert message "Sorry, this slot was just booked by someone else"
And the calendar should refresh to show the slot as unavailable
```

**Scenario: Attempting to book a slot that is already confirmed**

```gherkin
Given a time slot is already marked as "Reserved" in the database
When I try to send a booking request for that specific slot
Then the system should return an error
And my booking should be rejected to prevent duplicate entries
```

**Scenario: Refreshing availability after a failed double-booking**

```gherkin
Given I received a "Slot already booked" notification
When I close the notification
Then I should be redirected back to the updated calendar view
And I should be able to select a different green (available) time slot
```

---

## US-11: Booking appears in my calendar

**User Story**

As a resident I want to see my confirmed booking in my personal calendar with a washing machine icon, so that I can easily track my reservation.

**Scenario: Viewing a personal booking on the calendar**

```gherkin
Given I have a confirmed booking for "Tuesday at 10:00 - 11:00"
When I view the booking calendar
Then my own booking should be visually distinct from other residents' bookings
And my booking should display a washing machine icon
And the slot should be labeled as "My Booking" or similar
```

**Scenario: Differentiating between own and others' bookings**

```gherkin
Given there is a booking at 09:00 by "Another Resident"
And I have a booking at 10:00
When I look at the calendar view
Then the 09:00 slot should appear as a standard "Unavailable" slot
And only the 10:00 slot should show my personal washing machine icon and special styling
```

**Scenario: Viewing booking details from the icon**

```gherkin
Given I see my booking with the washing machine icon on the calendar
When I click or hover over my booking
Then I should see a summary including the date, time, and an option to manage the booking
```

---

## US-12: Resident views all bookings

**User Story**

As a resident, I want to see all my upcoming washing machine bookings, so that I can manage my schedule.

**Scenario: Resident views all upcoming bookings**

```gherkin
Given the resident has an active account
And the resident is logged into their personal account
When the resident navigates to the "My Bookings" page
Then the system displays a list of all upcoming washing machine bookings
And each booking shows the facility name, date, and time
So that the resident can manage their schedule
```

**Scenario: Resident has no upcoming bookings**

```gherkin
Given the resident is logged into their personal account
And the resident has no upcoming washing machine bookings
When the resident opens the "My Bookings" page
Then the system displays a message indicating that no bookings are scheduled
So that the resident understands they have no upcoming reservations
```

**Scenario: Resident tries to view bookings without being logged in**

```gherkin
Given the resident is not logged into the system
When the resident attempts to access the "My Bookings" page
Then the system redirects the resident to the login page
And displays a message indicating that login is required
So that only authenticated users can view booking information
```

---

## US-13: Resident views booking details

**User Story**

As a resident, I want to click on a booking icon and see details, so that I can review the facility, date, and time.

**Scenario: Resident views booking details**

```gherkin
Given the resident is logged into their personal account
And the resident has at least one upcoming booking
When the resident clicks on a booking icon in the calendar
Then the system opens the booking details view
And the system displays the facility name
And the system displays the booking date and time
And the system displays a "Cancel booking" button
So that the resident can review the booking information
```

**Scenario: Resident views details of multiple bookings**

```gherkin
Given the resident is logged into their personal account
And the resident has multiple upcoming bookings
When the resident clicks on a booking icon for a specific date
Then the system displays the details for that specific booking
And the resident can return to the calendar to view other bookings
So that the resident can review each booking individually
```

**Negative Scenario 1: Booking no longer exists**

**Scenario: Resident tries to view details of a booking that was deleted**

```gherkin
Given the resident is logged into their personal account
And the resident previously had a booking
But the administrator has cancelled or deleted this booking
When the resident clicks the booking icon
Then the system shows a message that the booking is no longer available
And the booking icon disappears from the calendar
```

**Negative Scenario 2: Booking data fails to load**

**Scenario: System error while loading booking details**

```gherkin
Given the resident is logged into their personal account
And the resident clicks on a booking icon
When the system fails to load booking details due to a temporary error
Then the system displays an error message
And suggests the resident try again later
```

---

## US-14: Resident cancels a booking

**User Story**

As a resident, I want to cancel my booking directly from my account, so that I can free the time slot if I no longer need it.

**Scenario: Resident cancels a booking from the calendar view**

```gherkin
Given the resident is logged into their personal account
And the resident has at least one upcoming booking
When the resident clicks the booking icon directly in the calendar
And selects "Cancel booking"
Then the system cancels the booking
And removes the booking icon from the resident's calendar
And the time slot becomes available again in the general calendar
And the system displays a confirmation message on the screen
So that the resident can manage bookings from either view
```

**Scenario: Resident tries to cancel a booking after the allowed cancellation window**

```gherkin
Given the resident is logged into their personal account
And the resident has a booking starting soon
When the resident clicks "Cancel booking"
Then the system displays a message that the booking cannot be cancelled due to policy
And the booking remains unchanged
So that the resident understands why cancellation is not allowed
```

**Scenario: System fails to cancel the booking**

```gherkin
Given the resident is logged into their personal account
And the resident clicks "Cancel booking"
When the system encounters a temporary error
Then the system displays an error message
And the booking remains visible in the resident's calendar
So that the resident knows the cancellation did not go through
```

**Scenario: Resident tries to cancel a booking that has already started**

```gherkin
Given the resident is logged into their personal account
And the booking start time has already passed
When the resident clicks "Cancel booking"
Then the system displays a message that the booking cannot be cancelled
And the booking remains in the past bookings list
So that the resident understands the booking is already active or completed
```

---

## US-16: Admin views all bookings

**User Story**

As an administrator, I want to see all washing machine bookings in a calendar, so that I can monitor usage and resolve conflicts.

**Scenario: Administrator views all washing machine bookings in the calendar**

```gherkin
Given the administrator is logged into the admin panel
When the administrator opens the "Bookings Calendar" page
Then the system displays the full washing machine booking calendar
And the administrator sees all booked time slots
And the administrator sees all available time slots
So that the administrator can monitor usage and identify potential conflicts
```

**Scenario: Administrator filters bookings by date**

```gherkin
Given the administrator is logged into the admin panel
And the "Bookings Calendar" page is open
When the administrator selects a different date or week in the calendar
Then the system loads the bookings for the selected date range
And displays all booked and available time slots for that period
So that the administrator can review bookings across different days
```

**Scenario: Administrator views an empty booking calendar**

```gherkin
Given the administrator is logged into the admin panel
And there are no bookings in the system
When the administrator opens the "Bookings Calendar" page
Then the system displays an empty calendar with all time slots available
And shows a message indicating that no bookings have been made
So that the administrator understands the current usage status
```

**Scenario: System error while loading bookings**

```gherkin
Given the administrator is logged into the admin panel
When the administrator opens the "Bookings Calendar" page
And the system encounters a temporary loading error
Then the system displays an error message
And does not show any booking data
So that the administrator knows the issue is technical, not related to bookings
```

**Scenario: Administrator sees incomplete booking information**

```gherkin
Given the administrator is logged into the admin panel
When the system loads the booking calendar
But some booking entries contain missing or corrupted data
Then the system marks those entries as "Data error"
And logs the issue for further investigation
So that the administrator is aware of data inconsistencies
```

**Scenario: Administrator navigates to a date with no available calendar data**

```gherkin
Given the administrator is logged into the admin panel
When the administrator scrolls far beyond the supported date range
Then the system displays a message that no booking data is available for that period
And keeps the calendar empty
So that the administrator understands the limits of the booking system
```

---

## US-17: Admin views booking details

**User Story**

As an administrator, I want to click on a booking to see resident information, so that I can contact them or manage the booking.

**Scenario: Administrator views booking details**

```gherkin
Given the administrator is logged into the admin panel
And the administrator sees at least one booking in the bookings calendar
When the administrator clicks on a specific booking
Then the system opens the booking details view
And the system displays the resident's name
And the system displays the resident's apartment number
And the system displays the resident's email and phone number
And the system displays the booking date and time
So that the administrator can contact the resident or manage the booking
```

**Scenario: System error while loading booking details**

```gherkin
Given the administrator is logged into the admin panel
And the administrator clicks on a booking
When the system encounters a temporary error
Then the system displays an error message
And does not show any booking details
So that the administrator understands the issue is technical
```

**Scenario: Administrator sees incomplete booking information**

```gherkin
Given the administrator is logged into the admin panel
When the administrator clicks on a booking
But some resident data is missing or corrupted
Then the system displays a warning that some information cannot be shown
And logs the issue for further investigation
So that the administrator is aware of data inconsistencies
```

**Scenario: Administrator opens booking details for a removed resident**

```gherkin
Given the administrator is logged into the admin panel
And the booking calendar contains a booking made by a resident who has since been removed
When the administrator clicks on the booking
Then the system displays a message that the resident account no longer exists
And suggests cancelling or cleaning up the booking
So that the administrator can resolve inconsistent data
```

---

## US-18: Admin cancels any booking

**User Story**

As an administrator I want to cancel any resident's booking, so that I can handle maintenance, conflicts, or misuse.

**Scenario: Admin cancels a booking for maintenance purposes**

```gherkin
Given I am logged in as an "Administrator"
And there is an existing booking for "Washing Machine 1" at "12:00 - 14:00" by a resident
When I view the master booking calendar
And I select the specific booking and click "Cancel Booking"
Then the system should remove the reservation from the calendar
And the time slot should be marked as "Unavailable/Maintenance" or become free again
```

**Scenario: Notifying the resident of a cancellation**

```gherkin
Given an administrator cancels a resident's booking
When the cancellation is confirmed in the system
Then the system should automatically send a notification (e.g., email or in-app message) to the resident
And the notification should state that their booking has been cancelled by management
```

**Scenario: Admin provides a reason for cancellation**

```gherkin
Given I am cancelling a resident's booking as an administrator
When I click "Cancel Booking"
Then a pop-up should ask me to provide a reason for the cancellation (e.g., "Maintenance", "Emergency")
And this reason should be included in the notification sent to the resident
```

**Scenario: Unauthorized user tries to cancel someone else's booking**

```gherkin
Given I am logged in as a "Resident"
When I try to access the cancellation action for another resident's booking
Then the system should not show the "Cancel" option
And I should be prevented from deleting any bookings other than my own
```

---

## US-19: Admin adds a new resident

**User Story**

As an administrator I want to add a new resident with their personal information, so that they can access the booking system.

**Scenario: Successfully adding a new resident**

```gherkin
Given I am logged in as an "Administrator"
And I am on the "User Management" page
When I enter the new resident's details (name, email, and apartment number)
And I click the "Add Resident" button
Then the new resident should be saved in the system
And I should see a confirmation message "Resident added successfully"
And the resident should appear in the list of active users
```

**Scenario: Automatic invitation email after adding**

```gherkin
Given an administrator has successfully added a new resident with a valid email address
When the resident's profile is created
Then the system should automatically send an invitation email to the resident
And the email should contain a link to set up their password and log in
```

**Scenario: Attempting to add a resident with missing information**

```gherkin
Given I am on the "Add New Resident" form
When I leave the email address field empty
And I click "Add Resident"
Then the system should show a validation error "Email is required"
And no new resident should be created in the database
```

**Scenario: Adding a resident with an email that already exists**

```gherkin
Given a resident with the email "resident@example.com" already exists in the system
When I try to add a new resident with the same email "resident@example.com"
Then I should see an error message "A user with this email already exists"
And the system should prevent duplicate registration
```

---

## US-20: Admin edits resident information

**User Story**

As an administrator I want to edit a resident's name, email, phone, or apartment number, so that their account stays up to date.

**Scenario: Successfully editing resident details**

```gherkin
Given I am logged in as an "Administrator"
And I am viewing the profile of an existing resident "Matti Meikäläinen"
When I change the apartment number to "B 12" and update the phone number
And I click the "Save Changes" button
Then the resident's profile should be updated in the database
And I should see a confirmation message "Resident information updated successfully"
```

**Scenario: Changing a resident's email address**

```gherkin
Given I am editing a resident's profile
When I enter a new email address "new.email@example.com"
And I click "Save Changes"
Then the system should update the login email for that resident
And the resident should receive a notification that their account email has been changed
```

**Scenario: Attempting to save invalid information**

```gherkin
Given I am in the "Edit Resident" view
When I clear the "Name" field and try to save
Then the system should prevent the save operation
And I should see a validation error "Name field cannot be empty"
```

**Scenario: Cancel editing without saving**

```gherkin
Given I have modified the resident's phone number in the edit form
When I click the "Cancel" button or navigate away without saving
Then the system should discard the changes
And the resident's information should remain as it was before editing
```

---

## US-22: Resident receives booking confirmation

**User Story**

As a resident, I want to receive a confirmation after booking a facility, so that I know my reservation is successfully recorded.

**Criteria**

- Resident must have an active account
- Booking must be successfully created
- System sends a confirmation email and/or SMS to the resident
- The confirmation includes facility name, date, time, and a cancellation link
- The confirmation is sent immediately after booking

**Scenario: Successful booking confirmation**

```gherkin
Given the resident has successfully booked a facility
When the booking is confirmed by the system
Then a confirmation email or SMS is sent to the resident
And it contains the facility name, date, time, and cancellation link
```

**Scenario: Booking fails**

```gherkin
Given the resident attempts to book a facility
When the booking cannot be created (e.g., slot already taken)
Then the resident does not receive a confirmation
And an error message is displayed instead
```

**Scenario: Resident checks confirmation**

```gherkin
Given the resident has received the booking confirmation
When the resident opens the email or SMS
Then the details of the booking are correct
And the resident can use the cancellation link if needed
```

---

## US-23: Resident receives account deletion email

**User Story**

As a resident, I want to receive an email when my account is deleted, so that I am informed that I no longer have access to the booking system.

**Criteria**

- Admin must delete the resident account
- System automatically sends an email to the deleted resident
- Email contains a message that the account has been removed
- Resident cannot log in after deletion
- Historical bookings remain in the system for record-keeping

**Scenario: Resident receives deletion email**

```gherkin
Given the administrator deletes a resident account
When the account deletion is processed
Then the system sends an email to the resident
And the email clearly states that the account has been deleted
```

**Scenario: Deleted resident tries to log in**

```gherkin
Given the resident account has been deleted
When the former resident attempts to log in
Then access is denied
And a message indicates the account no longer exists
```

**Scenario: Admin deletes a non-existent account**

```gherkin
Given the administrator attempts to delete a resident account that does not exist
When the deletion action is performed
Then the system displays an error message
And no email is sent
```
