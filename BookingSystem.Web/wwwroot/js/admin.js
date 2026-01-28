document.addEventListener('DOMContentLoaded', function () {
    if (window.location.pathname.includes('admin-dashboard.html')) {
        const filterDate = document.getElementById('filterDate');
        if (filterDate) {
            const today = new Date().toISOString().split('T')[0];
            filterDate.value = today;
        }
        loadAdminBookings();
        document.getElementById('refreshBtn')?.addEventListener('click', loadAdminBookings);
    }

    if (window.location.pathname.includes('admin-residents.html')) {
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
        showError(errorMessage, 'Varausten lataaminen epäonnistui: ' + error.message);
    }
}

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
