class ApiClient {
    constructor(baseUrl = '/api') {
        this.baseUrl = baseUrl;
        this.token = localStorage.getItem('token');
    }

    setToken(token) {
        this.token = token;
        localStorage.setItem('token', token);
    }

    clearToken() {
        this.token = null;
        localStorage.removeItem('token');
    }

    async request(endpoint, options = {}) {
        const url = `${this.baseUrl}${endpoint}`;
        const headers = {
            'Content-Type': 'application/json',
            ...options.headers
        };

        if (!this.token) {
            this.token = localStorage.getItem('adminToken') || localStorage.getItem('token');
        }

        if (this.token) {
            headers['Authorization'] = `Bearer ${this.token}`;
        }

        try {
            const response = await fetch(url, {
                method: options.method || 'GET',
                headers: headers,
                body: options.body ? JSON.stringify(options.body) : undefined
            });

            let data = null;
            const contentType = response.headers.get('content-type');
            if (contentType && contentType.includes('application/json')) {
                const text = await response.text();
                data = text ? JSON.parse(text) : null;
            }

            if (!response.ok) {
                const errorMessage = data?.errorMessage || `HTTP Error: ${response.status}`;
                throw new Error(errorMessage);
            }
            return data;
        } catch (error) {
            console.error('API Error:', error);
            throw error;
        }
    }

    async login(apartmentNumber, password) {
        return this.request('/auth/login', {
            method: 'POST',
            body: { apartmentNumber, password }
        });
    }

    async adminLogin(username, password) {
        return this.request('/auth/admin-login', {
            method: 'POST',
            body: { username, password }
        });
    }

    async activateAccount(token, password) {
        return this.request(`/auth/activate?token=${token}`, {
            method: 'POST',
            body: { password }
        });
    }

    async getFacilities() {
        return this.request('/bookings/facilities');
    }

    async getAvailability(facilityId, date) {
        const dateStr = date.toISOString().split('T')[0];
        return this.request(`/bookings/availability?facilityId=${facilityId}&date=${dateStr}`);
    }

    async confirmBooking(residentId, timeSlotId, facilityId) {
        return this.request('/bookings/confirm', {
            method: 'POST',
            body: { residentId, timeSlotId, facilityId }
        });
    }

    async getUpcomingBookings(residentId) {
        return this.request(`/bookings/my-bookings/${residentId}`);
    }

    async getBookingDetails(bookingId) {
        return this.request(`/bookings/${bookingId}`);
    }

    async cancelBooking(bookingId, residentId) {
        return this.request(`/bookings/${bookingId}/cancel`, {
            method: 'DELETE',
            body: { residentId }
        });
    }

    async getAdminBookings(date) {
        const dateStr = date.toISOString().split('T')[0];
        return this.request(`/admin/bookings?date=${dateStr}`);
    }

    async adminCancelBooking(bookingId, reason) {
        return this.request(`/admin/bookings/${bookingId}/cancel`, {
            method: 'DELETE',
            body: { bookingId, reason }
        });
    }

    async getAdminFacilities() {
        return this.request('/admin/facilities');
    }

    async generateDailyTimeSlots(facilityId, date, startTime, endTime) {
        const dateStr = date.toISOString().split('T')[0];
        return this.request('/admin/time-slots/generate', {
            method: 'POST',
            body: { facilityId, date: dateStr, startTime, endTime }
        });
    }

    async getResidents() {
        return this.request('/admin/residents');
    }

    async createResident(data) {
        return this.request('/admin/residents', {
            method: 'POST',
            body: data
        });
    }

    async updateResident(residentId, data) {
        return this.request(`/admin/residents/${residentId}`, {
            method: 'PUT',
            body: data
        });
    }

    async deleteResident(residentId) {
        return this.request(`/admin/residents/${residentId}`, {
            method: 'DELETE'
        });
    }

    async verifyTwoFactorCode(code) {
        return this.request('/auth/2fa/verify', {
            method: 'POST',
            body: { code }
        });
    }
}

const apiClient = new ApiClient();
