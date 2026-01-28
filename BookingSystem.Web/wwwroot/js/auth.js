document.addEventListener('DOMContentLoaded', function () {
    const loginForm = document.getElementById('loginForm');
    const activationForm = document.getElementById('activationForm');
    const adminLoginForm = document.getElementById('adminLoginForm');
    const logoutBtn = document.getElementById('logoutBtn');

    if (loginForm) {
        loginForm.addEventListener('submit', handleResidentLogin);
    }

    if (activationForm) {
        activationForm.addEventListener('submit', handleAccountActivation);
    }

    if (adminLoginForm) {
        adminLoginForm.addEventListener('submit', handleAdminLogin);
    }

    if (logoutBtn) {
        logoutBtn.addEventListener('click', handleLogout);
    }

    // Check authentication on protected pages
    checkAuthentication();
});

async function handleResidentLogin(e) {
    e.preventDefault();

    const apartmentNumber = document.getElementById('apartmentNumber').value;
    const password = document.getElementById('password').value;
    const errorMessage = document.getElementById('errorMessage');

    try {
        const response = await apiClient.login(apartmentNumber, password);

        if (response && response.token) {
            apiClient.setToken(response.token);
            localStorage.setItem('residentId', response.residentId);
            window.location.href = '/booking.html';
        } else {
            showError(errorMessage, response?.errorMessage || 'Kirjautuminen epäonnistui');
        }
    } catch (error) {
        showError(errorMessage, 'Kirjautumisen aikana tapahtui virhe: ' + error.message);
    }
}

async function handleAdminLogin(e) {
    e.preventDefault();

    const username = document.getElementById('username').value;
    const password = document.getElementById('password').value;
    const errorMessage = document.getElementById('errorMessage');

    try {
        const response = await apiClient.adminLogin(username, password);

        if (response && response.requiresTwoFactor) {
            document.getElementById('twoFactorSection').style.display = 'block';
            document.getElementById('verifyTwoFactorBtn').addEventListener('click', handleTwoFactorVerification);
        } else if (response && response.token) {
            apiClient.setToken(response.token);
            localStorage.setItem('adminId', response.adminId);
            window.location.href = '/admin-dashboard.html';
        } else {
            showError(errorMessage, response?.errorMessage || 'Kirjautuminen epäonnistui');
        }
    } catch (error) {
        showError(errorMessage, 'Kirjautumisen aikana tapahtui virhe: ' + error.message);
    }
}

async function handleTwoFactorVerification() {
    const code = document.getElementById('twoFactorCode').value;
    const errorMessage = document.getElementById('errorMessage');

    try {
        const response = await apiClient.verifyTwoFactorCode(code);

        if (response && response.token) {
            apiClient.setToken(response.token);
            window.location.href = '/admin-dashboard.html';
        } else {
            showError(errorMessage, 'Virheellinen 2FA-koodi');
        }
    } catch (error) {
        showError(errorMessage, '2FA-vahvistus epäonnistui: ' + error.message);
    }
}

async function handleAccountActivation(e) {
    e.preventDefault();

    const newPassword = document.getElementById('newPassword').value;
    const confirmPassword = document.getElementById('confirmPassword').value;
    const errorMessage = document.getElementById('errorMessage');
    const successMessage = document.getElementById('successMessage');

    if (newPassword !== confirmPassword) {
        showError(errorMessage, 'Salasanat eivät vastaa toisiaan');
        return;
    }

    try {
        const token = new URLSearchParams(window.location.search).get('token');

        const response = await apiClient.activateAccount(token, newPassword);

        if (response) {
            showSuccess(successMessage, 'Tili aktivoitu onnistuneesti. Ohjataan kirjautumissivulle...');
            setTimeout(() => {
                window.location.href = '/index.html';
            }, 2000);
        }
    } catch (error) {
        showError(errorMessage, 'Aktivointi epäonnistui: ' + error.message);
    }
}

function handleLogout() {
    apiClient.clearToken();
    localStorage.removeItem('residentId');
    localStorage.removeItem('adminId');
    window.location.href = '/index.html';
}

function checkAuthentication() {
    const token = localStorage.getItem('token');
    const currentPath = window.location.pathname;
    const publicPages = ['/index.html', '/activation.html', '/admin-login.html', '/'];

    if (!token && !publicPages.some(page => currentPath.includes(page))) {
        window.location.href = '/index.html';
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
