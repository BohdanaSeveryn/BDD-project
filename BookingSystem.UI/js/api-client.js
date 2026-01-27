// API Configuration
const API_BASE = 'http://localhost:5000/api';

// API Client
class ApiClient {
  static async request(endpoint, options = {}) {
    const url = `${API_BASE}${endpoint}`;
    const headers = {
      'Content-Type': 'application/json',
      ...options.headers,
    };

    const token = localStorage.getItem('token');
    if (token) {
      headers['Authorization'] = `Bearer ${token}`;
    }

    try {
      const response = await fetch(url, {
        ...options,
        headers,
      });

      if (!response.ok) {
        const error = await response.json().catch(() => ({}));
        throw new Error(error.message || `API Error: ${response.status}`);
      }

      return await response.json();
    } catch (error) {
      console.error('API Error:', error);
      throw error;
    }
  }

  // Resident Endpoints
  static registerResident(data) {
    return this.request('/residents/register', {
      method: 'POST',
      body: JSON.stringify(data),
    });
  }

  static loginResident(apartmentNumber, password) {
    return this.request('/residents/login', {
      method: 'POST',
      body: JSON.stringify({ apartmentNumber, password }),
    });
  }

  static activateAccount(residentId, password) {
    return this.request('/residents/activate', {
      method: 'POST',
      body: JSON.stringify({ residentId, password }),
    });
  }

  static getResident(id) {
    return this.request(`/residents/${id}`);
  }

  static updateResident(id, data) {
    return this.request(`/residents/${id}`, {
      method: 'PUT',
      body: JSON.stringify(data),
    });
  }

  // Booking Endpoints
  static getAvailability(facilityId, date) {
    return this.request(`/bookings/availability/${facilityId}?date=${date}`);
  }

  static confirmBooking(data) {
    return this.request('/bookings/confirm', {
      method: 'POST',
      body: JSON.stringify(data),
    });
  }

  static getBooking(id) {
    return this.request(`/bookings/${id}`);
  }

  static getMyBookings(residentId, date) {
    return this.request(`/bookings/resident/${residentId}?date=${date}`);
  }

  static getUpcomingBookings(residentId) {
    return this.request(`/bookings/upcoming/${residentId}`);
  }

  static cancelBooking(bookingId, residentId, reason) {
    return this.request(`/bookings/${bookingId}/resident/${residentId}`, {
      method: 'DELETE',
      body: JSON.stringify({ cancellationReason: reason }),
    });
  }

  // Admin Endpoints
  static adminLogin(username, password) {
    return this.request('/admin/login', {
      method: 'POST',
      body: JSON.stringify({ username, password }),
    });
  }

  static verify2FA(adminId, code) {
    return this.request('/admin/verify-2fa', {
      method: 'POST',
      body: JSON.stringify({ adminId, code }),
    });
  }

  static createResident(adminId, data) {
    return this.request(`/admin/residents?adminId=${adminId}`, {
      method: 'POST',
      body: JSON.stringify(data),
    });
  }

  static deleteResident(adminId, residentId) {
    return this.request(`/admin/residents/${residentId}?adminId=${adminId}`, {
      method: 'DELETE',
    });
  }

  static getAllBookings(adminId, date) {
    return this.request(`/admin/bookings?adminId=${adminId}&date=${date}`);
  }

  static getAdminBookingDetails(adminId, bookingId) {
    return this.request(`/admin/bookings/${bookingId}?adminId=${adminId}`);
  }

  static cancelAdminBooking(adminId, bookingId, reason) {
    return this.request(`/admin/bookings/${bookingId}?adminId=${adminId}`, {
      method: 'DELETE',
      body: JSON.stringify({ cancellationReason: reason }),
    });
  }

  static getAdminResident(adminId, residentId) {
    return this.request(`/admin/residents/${residentId}?adminId=${adminId}`);
  }

  // Health Check
  static getHealth() {
    return this.request('/health');
  }
}

// Utility Functions
function showAlert(message, type = 'info') {
  const alertDiv = document.createElement('div');
  alertDiv.className = `alert alert-${type}`;
  alertDiv.textContent = message;
  
  const container = document.querySelector('.container') || document.body;
  container.insertBefore(alertDiv, container.firstChild);
  
  setTimeout(() => alertDiv.remove(), 5000);
}

function showModal(modalId) {
  document.getElementById(modalId).classList.add('active');
}

function closeModal(modalId) {
  document.getElementById(modalId).classList.remove('active');
}

function setLoading(element, isLoading) {
  if (isLoading) {
    element.disabled = true;
    element.innerHTML = '<span class="spinner"></span> Loading...';
  } else {
    element.disabled = false;
    element.innerHTML = element.getAttribute('data-text') || element.textContent;
  }
}

function formatDate(dateString) {
  const options = { year: 'numeric', month: 'long', day: 'numeric' };
  return new Date(dateString).toLocaleDateString('en-US', options);
}

function formatTime(timeString) {
  return timeString.slice(0, 5);
}

// Session Management
class SessionManager {
  static setSession(type, data) {
    localStorage.setItem('sessionType', type);
    localStorage.setItem('userData', JSON.stringify(data));
    localStorage.setItem('token', data.token || data.residentId);
  }

  static getSession() {
    const type = localStorage.getItem('sessionType');
    const userData = localStorage.getItem('userData');
    return type && userData ? { type, data: JSON.parse(userData) } : null;
  }

  static clearSession() {
    localStorage.removeItem('sessionType');
    localStorage.removeItem('userData');
    localStorage.removeItem('token');
  }

  static isLoggedIn() {
    return this.getSession() !== null;
  }

  static getUserType() {
    const session = this.getSession();
    return session ? session.type : null;
  }

  static getUserData() {
    const session = this.getSession();
    return session ? session.data : null;
  }
}

// Redirect if not logged in
function requireLogin(allowedType = null) {
  if (!SessionManager.isLoggedIn()) {
    window.location.href = 'index.html';
    return false;
  }

  if (allowedType && SessionManager.getUserType() !== allowedType) {
    window.location.href = 'index.html';
    return false;
  }

  return true;
}
