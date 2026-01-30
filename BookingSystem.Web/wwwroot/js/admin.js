let adminFacilities = [];
let selectedFacilityId = null;

document.addEventListener('DOMContentLoaded', function () {
    if (window.location.pathname.includes('admin-dashboard.html')) {
        if (!ensureAdminToken()) {
            return;
        }
        const filterDate = document.getElementById('filterDate');
        if (filterDate) {
            const today = new Date().toISOString().split('T')[0];
            filterDate.value = today;
        }
        loadFacilities();
        loadAdminBookings();
        document.getElementById('refreshBtn')?.addEventListener('click', loadAdminBookings);
        document.getElementById('filterDate')?.addEventListener('change', loadAdminTimeSlots);
        document.getElementById('generateSlotsBtn')?.addEventListener('click', openTimeSlotModal);

        const timeSlotForm = document.getElementById('timeSlotForm');
        timeSlotForm?.addEventListener('submit', handleGenerateSlots);

        const modalClose = document.getElementById('timeSlotModalClose');
        modalClose?.addEventListener('click', closeTimeSlotModal);
    }

    if (window.location.pathname.includes('admin-residents.html')) {
        if (!ensureAdminToken()) {
            return;
        }
        loadResidents();
        document.getElementById('addResidentBtn')?.addEventListener('click', openAddResidentModal);

        const closeBtn = document.querySelector('.close');
        if (closeBtn) {
            closeBtn.onclick = function () {
                document.getElementById('residentModal').style.display = 'none';
            }
        }
    }
});

function ensureAdminToken() {
    const adminToken = localStorage.getItem('adminToken');
    const token = localStorage.getItem('token');
    const effectiveToken = adminToken || token;
    if (!effectiveToken) {
        window.location.href = '/admin-login.html';
        return false;
    }
    apiClient.setToken(effectiveToken);
    return true;
}

async function loadAdminBookings() {
    const filterDate = document.getElementById('filterDate')?.value || new Date().toISOString().split('T')[0];
    const bookingTableBody = document.getElementById('bookingTableBody');
    const errorMessage = document.getElementById('errorMessage');

    try {
        const bookings = await apiClient.getAdminBookings(new Date(filterDate));

        bookingTableBody.innerHTML = '';
        if (bookings && bookings.length > 0) {
            bookings.forEach(booking => {
                const row = document.createElement('tr');
                const date = new Date(booking.bookingDate).toLocaleDateString('fi-FI');
                row.innerHTML = `
                    <td>${booking.residentName}</td>
                    <td>${booking.residentApartment}</td>
                    <td>${booking.facilityName}</td>
                    <td>${booking.startTime} - ${booking.endTime}</td>
                    <td>${date}</td>
                    <td>
                        <button class="btn btn-danger" onclick="adminCancelBooking('${booking.id}')">Peruuta</button>
                    </td>
                `;
                bookingTableBody.appendChild(row);
            });
        } else {
            bookingTableBody.innerHTML = '<tr><td colspan="6" style="text-align: center;">Ei varauksia tälle päivälle.</td></tr>';
        }
    } catch (error) {
        if (error.message.includes('401')) {
            apiClient.clearToken();
            window.location.href = '/admin-login.html';
            return;
        }
        showError(errorMessage, 'Varausten lataaminen epäonnistui: ' + error.message);
    }
}

async function loadFacilities() {
    const facilityContainer = document.getElementById('facilityContainer');
    if (!facilityContainer) {
        return;
    }

    const errorMessage = document.getElementById('errorMessage');

    try {
        const facilityResponse = await apiClient.getAdminFacilities();
        adminFacilities = Array.isArray(facilityResponse)
            ? facilityResponse
            : (facilityResponse?.value ?? facilityResponse?.Value ?? facilityResponse?.items ?? []);

        if (!adminFacilities || adminFacilities.length === 0) {
            const publicFacilities = await apiClient.getFacilities();
            adminFacilities = Array.isArray(publicFacilities)
                ? publicFacilities
                : (publicFacilities?.value ?? publicFacilities?.Value ?? publicFacilities?.items ?? []);
        }

        facilityContainer.innerHTML = '';
        if (adminFacilities && adminFacilities.length > 0) {
            adminFacilities.forEach((facility, index) => {
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
        if (error.message.includes('401')) {
            apiClient.clearToken();
            window.location.href = '/admin-login.html';
            return;
        }
        showError(errorMessage, 'Tilojen lataaminen epäonnistui: ' + error.message);
    }
}

function selectFacility(facilityId) {
    selectedFacilityId = facilityId;
    document.querySelectorAll('.facility-card').forEach(card => {
        card.classList.toggle('selected', card.dataset.facilityId === facilityId);
    });
    loadAdminTimeSlots();
}

function openTimeSlotModal() {
    const modal = document.getElementById('timeSlotModal');
    const slotDate = document.getElementById('slotDate');
    const filterDate = document.getElementById('filterDate')?.value || new Date().toISOString().split('T')[0];

    if (slotDate) {
        slotDate.value = filterDate;
    }

    if (modal) {
        modal.style.display = 'block';
    }
}

function closeTimeSlotModal() {
    const modal = document.getElementById('timeSlotModal');
    if (modal) {
        modal.style.display = 'none';
    }
}

async function handleGenerateSlots(e) {
    e.preventDefault();

    const slotDate = document.getElementById('slotDate')?.value || new Date().toISOString().split('T')[0];
    const startTime = document.getElementById('slotStartTime')?.value || '07:00';
    const endTime = document.getElementById('slotEndTime')?.value || '21:00';
    const errorMessage = document.getElementById('errorMessage');
    const successMessage = document.getElementById('successMessage');

    if (!selectedFacilityId) {
        showError(errorMessage, 'Valitse tila ennen aikavälien luontia.');
        return;
    }

    try {
        const result = await apiClient.generateDailyTimeSlots(
            selectedFacilityId,
            new Date(slotDate),
            startTime,
            endTime
        );
        showSuccess(successMessage, `Luotu ${result.createdCount} aikaväliä, ohitettu ${result.skippedCount}.`);
        closeTimeSlotModal();
        await loadAdminTimeSlots();
    } catch (error) {
        showError(errorMessage, 'Aikavälien luonti epäonnistui: ' + error.message);
    }
}

async function loadAdminTimeSlots() {
    const slotDate = document.getElementById('filterDate')?.value || new Date().toISOString().split('T')[0];
    const container = document.getElementById('adminTimeSlotContainer');
    const errorMessage = document.getElementById('errorMessage');

    if (!selectedFacilityId || !container) {
        return;
    }

    try {
        const slots = await apiClient.getAvailability(selectedFacilityId, new Date(slotDate));
        renderTimeSlots(container, slots);
    } catch (error) {
        showError(errorMessage, 'Aikavälien lataaminen epäonnistui: ' + error.message);
    }
}

function renderTimeSlots(container, slots) {
    container.innerHTML = '';

    if (!slots || slots.length === 0) {
        container.innerHTML = '<p style="grid-column: 1/-1; text-align: center;">Ei aikavälejä tälle päivälle.</p>';
        return;
    }

    slots.forEach(slot => {
        const slotDiv = document.createElement('div');
        slotDiv.className = `time-slot ${slot.color === 'green' ? 'available' : 'booked'}`;
        slotDiv.textContent = `${slot.startTime} - ${slot.endTime}`;
        container.appendChild(slotDiv);
    });
}

window.addEventListener('click', function (event) {
    const modal = document.getElementById('timeSlotModal');
    if (event.target === modal) {
        closeTimeSlotModal();
    }
});

async function adminCancelBooking(bookingId) {
    const reason = prompt('Anna peruutuksen syy:');
    if (!reason) {
        return;
    }

    const errorMessage = document.getElementById('errorMessage');
    const successMessage = document.getElementById('successMessage');

    try {
        await apiClient.adminCancelBooking(bookingId, reason);
        showSuccess(successMessage, 'Varaus peruutettu');
        setTimeout(() => {
            loadAdminBookings();
        }, 1500);
    } catch (error) {
        showError(errorMessage, 'Varauksen peruutus epäonnistui: ' + error.message);
    }
}

async function loadResidents() {
    const residentTableBody = document.getElementById('residentTableBody');
    const errorMessage = document.getElementById('errorMessage');

    try {
        const residents = await apiClient.getResidents();

        residentTableBody.innerHTML = '';
        if (residents && residents.length > 0) {
            residents.forEach(resident => {
                const row = document.createElement('tr');
                const status = resident.isActive ? 'Aktiivinen' : 'Passiivinen';
                row.innerHTML = `
                    <td>${resident.name}</td>
                    <td>${resident.apartmentNumber}</td>
                    <td>${resident.email}</td>
                    <td></td>
                    <td>${status}</td>
                    <td>
                        <button class="btn btn-secondary" onclick="editResident('${resident.id}')">Muokkaa</button>
                        <button class="btn btn-danger" onclick="deleteResident('${resident.id}')">Poista</button>
                    </td>
                `;
                residentTableBody.appendChild(row);
            });
        }
    } catch (error) {
        showError(errorMessage, 'Asukkaiden lataaminen epäonnistui: ' + error.message);
    }
}

function openAddResidentModal() {
    const modal = document.getElementById('residentModal');
    const title = document.getElementById('modalTitle');
    const form = document.getElementById('residentForm');

    title.textContent = 'Lisää Asukas';
    form.reset();
    form.onsubmit = handleAddResident;

    modal.style.display = 'block';
}

function openEditResidentModal(residentId) {
    const modal = document.getElementById('residentModal');
    const title = document.getElementById('modalTitle');
    const form = document.getElementById('residentForm');

    title.textContent = 'Muokkaa Asukasta';
    form.onsubmit = (e) => handleEditResident(e, residentId);

    modal.style.display = 'block';
}

async function handleAddResident(e) {
    e.preventDefault();

    const data = {
        name: document.getElementById('name').value,
        email: document.getElementById('email').value,
        apartmentNumber: document.getElementById('apartment').value,
        phone: document.getElementById('phone').value
    };

    try {
        await apiClient.createResident(data);
        showSuccess(document.getElementById('successMessage'), 'Asukas lisätty onnistuneesti');
        document.getElementById('residentModal').style.display = 'none';
        setTimeout(() => {
            loadResidents();
        }, 1500);
    } catch (error) {
        showError(document.getElementById('errorMessage'), 'Asukaan lisääminen epäonnistui: ' + error.message);
    }
}

async function handleEditResident(e, residentId) {
    e.preventDefault();

    const data = {
        name: document.getElementById('name').value,
        email: document.getElementById('email').value,
        apartmentNumber: document.getElementById('apartment').value,
        phone: document.getElementById('phone').value
    };

    try {
        await apiClient.updateResident(residentId, data);
        showSuccess(document.getElementById('successMessage'), 'Asukas päivitetty onnistuneesti');
        document.getElementById('residentModal').style.display = 'none';
        setTimeout(() => {
            loadResidents();
        }, 1500);
    } catch (error) {
        showError(document.getElementById('errorMessage'), 'Asukaan päivittäminen epäonnistui: ' + error.message);
    }
}

function editResident(residentId) {
    openEditResidentModal(residentId);
}

async function deleteResident(residentId) {
    if (!confirm('Oletko varma, että haluat poistaa tämän asukaan?')) {
        return;
    }

    try {
        await apiClient.deleteResident(residentId);
        showSuccess(document.getElementById('successMessage'), 'Asukas poistettu onnistuneesti');
        setTimeout(() => {
            loadResidents();
        }, 1500);
    } catch (error) {
        showError(document.getElementById('errorMessage'), 'Asukaan poistaminen epäonnistui: ' + error.message);
    }
}

window.onclick = function (event) {
    const modal = document.getElementById('residentModal');
    if (event.target === modal) {
        modal.style.display = 'none';
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
