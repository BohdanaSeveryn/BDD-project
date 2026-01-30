let selectedSlotId = null;
let selectedFacilityId = null;
let facilities = [];

document.addEventListener('DOMContentLoaded', function () {
    const bookingDate = document.getElementById('bookingDate');
    const timeSlotContainer = document.getElementById('timeSlotContainer');
    const confirmBookingBtn = document.getElementById('confirmBookingBtn');
    const clearSelectionBtn = document.getElementById('clearSelectionBtn');
    const facilityContainer = document.getElementById('facilityContainer');

    const residentToken = localStorage.getItem('residentToken');
    if (residentToken) {
        apiClient.setToken(residentToken);
    }

    if (bookingDate) {
        const today = new Date().toISOString().split('T')[0];
        bookingDate.value = today;
        bookingDate.min = today;

        bookingDate.addEventListener('change', loadAvailability);
        confirmBookingBtn?.addEventListener('click', confirmBooking);
        clearSelectionBtn?.addEventListener('click', clearSelection);

        loadFacilities().then(() => loadAvailability());
    }

    if (window.location.pathname.includes('my-bookings.html')) {
        loadMyBookings();
    }
});

async function loadFacilities() {
    const facilityContainer = document.getElementById('facilityContainer');
    const errorMessage = document.getElementById('errorMessage');

    if (!facilityContainer) {
        return;
    }

    try {
        facilities = await apiClient.getFacilities();
        if (facilities && facilities.length > 0) {
            facilityContainer.innerHTML = '';
            facilities.forEach((facility, index) => {
                const card = document.createElement('div');
                card.className = 'facility-card';
                card.dataset.facilityId = facility.id;
                card.innerHTML = `
                    <div class="icon">${facility.icon ?? '🧺'}</div>
                    <div class="name">${facility.name}</div>
                `;
                card.addEventListener('click', () => selectFacility(facility.id));
                facilityContainer.appendChild(card);

                if (index === 0) {
                    selectFacility(facility.id);
                }
            });
        } else {
            facilityContainer.innerHTML = '<p>Ei saatavilla olevia tiloja</p>';
        }
    } catch (error) {
        showError(errorMessage, 'Tilojen lataaminen epäonnistui: ' + error.message);
    }
}

function selectFacility(facilityId) {
    selectedFacilityId = facilityId;
    document.querySelectorAll('.facility-card').forEach(card => {
        card.classList.toggle('selected', card.dataset.facilityId === facilityId);
    });
    loadAvailability();
}

async function loadAvailability() {
    const bookingDate = document.getElementById('bookingDate').value;
    const timeSlotContainer = document.getElementById('timeSlotContainer');
    const errorMessage = document.getElementById('errorMessage');

    if (!selectedFacilityId) {
        showError(errorMessage, 'Valitse ensin laite, jonka haluat varata.');
        return;
    }

    try {
        const slots = await apiClient.getAvailability(selectedFacilityId, new Date(bookingDate));

        timeSlotContainer.innerHTML = '';
        if (slots && slots.length > 0) {
            slots.forEach(slot => {
                const slotDiv = document.createElement('div');
                slotDiv.className = `time-slot ${slot.color === 'green' ? 'available' : 'booked'}`;
                slotDiv.textContent = `${slot.startTime} - ${slot.endTime}`;
                slotDiv.dataset.slotId = slot.id;

                if (slot.color === 'green') {
                    slotDiv.addEventListener('click', selectSlot);
                }

                timeSlotContainer.appendChild(slotDiv);
            });
        } else {
            timeSlotContainer.innerHTML = '<p style="grid-column: 1/-1; text-align: center;">Ei saatavilla olevia aikoja tälle päivälle.</p>';
        }
    } catch (error) {
        showError(errorMessage, 'Saatavuuden lataaminen epäonnistui: ' + error.message);
    }
}

function selectSlot(e) {
    const slot = e.target;
    selectedSlotId = slot.dataset.slotId;

    document.querySelectorAll('.time-slot').forEach(s => s.classList.remove('selected'));

    slot.classList.add('selected');

    const selectedSlotInfo = document.getElementById('selectedSlotInfo');
    const selectedSlotDisplay = document.getElementById('selectedSlotDisplay');
    selectedSlotDisplay.textContent = slot.textContent;
    selectedSlotInfo.style.display = 'block';
}

function clearSelection() {
    selectedSlotId = null;
    document.querySelectorAll('.time-slot').forEach(s => s.classList.remove('selected'));
    document.getElementById('selectedSlotInfo').style.display = 'none';
}

async function confirmBooking() {
    const residentId = localStorage.getItem('residentId');
    const errorMessage = document.getElementById('errorMessage');
    const successMessage = document.getElementById('successMessage');

    if (!selectedSlotId) {
        showError(errorMessage, 'Valitse aika-alue');
        return;
    }

    try {
        const booking = await apiClient.confirmBooking(residentId, selectedSlotId, selectedFacilityId);
        if (booking) {
            showSuccess(successMessage, 'Varaus vahvistettu onnistuneesti!');
            clearSelection();
            setTimeout(() => {
                loadAvailability();
            }, 1500);
        }
    } catch (error) {
        showError(errorMessage, 'Varauksen vahvistaminen epäonnistui: ' + error.message);
    }
}

async function loadMyBookings() {
    const residentId = localStorage.getItem('residentId');
    const bookingsList = document.getElementById('bookingsList');
    const noBookingsMessage = document.getElementById('noBookingsMessage');
    const errorMessage = document.getElementById('errorMessage');

    try {
        const bookings = await apiClient.getUpcomingBookings(residentId);

        if (bookings && bookings.length > 0) {
            bookingsList.innerHTML = '';
            bookings.forEach(booking => {
                const card = createBookingCard(booking);
                bookingsList.appendChild(card);
            });
            noBookingsMessage.style.display = 'none';
        } else {
            bookingsList.innerHTML = '';
            noBookingsMessage.style.display = 'block';
        }
    } catch (error) {
        showError(errorMessage, 'Varausten lataaminen epäonnistui: ' + error.message);
    }
}

function createBookingCard(booking) {
    const card = document.createElement('div');
    card.className = 'booking-card';

    const icon = '🔧';
    const date = new Date(booking.bookingDate).toLocaleDateString('fi-FI');

    card.innerHTML = `
        <div class="icon">${icon}</div>
        <h3>${booking.facilityName}</h3>
        <p class="time">${booking.startTime} - ${booking.endTime}</p>
        <p>${date}</p>
        <button class="btn btn-danger" onclick="cancelBooking('${booking.id}')">Peruuta</button>
    `;

    return card;
}

async function cancelBooking(bookingId) {
    if (!confirm('Oletko varma, että haluat perua varauksen?')) {
        return;
    }

    const residentId = localStorage.getItem('residentId');
    const errorMessage = document.getElementById('errorMessage');
    const successMessage = document.getElementById('successMessage');

    try {
        await apiClient.cancelBooking(bookingId, residentId);
        showSuccess(successMessage, 'Varaus peruutettu onnistuneesti');
        setTimeout(() => {
            loadMyBookings();
        }, 1500);
    } catch (error) {
        showError(errorMessage, 'Varauksen peruutus epäonnistui: ' + error.message);
    }
}

function showError(element, message) {
    if (element) {
        element.textContent = message;
        element.classList.add('show');
        setTimeout(() => {
            element.classList.remove('show');
        }, 5000);
    }
}

function showSuccess(element, message) {
    if (element) {
        element.textContent = message;
        element.classList.add('show');
        setTimeout(() => {
            element.classList.remove('show');
        }, 5000);
    }
}
