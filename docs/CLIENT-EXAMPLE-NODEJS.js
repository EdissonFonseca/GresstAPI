/**
 * Cliente Node.js para Gresst API con soporte de RefreshToken
 * 
 * Instalación:
 *   npm install axios
 * 
 * Uso:
 *   const client = new GreesstAPIClient('http://localhost:5000/api');
 *   await client.login('user@example.com', 'password123');
 *   const facilities = await client.getFacilities();
 */

const axios = require('axios');

class GreesstAPIClient {
    constructor(baseURL) {
        this.baseURL = baseURL;
        this.accessToken = null;
        this.refreshToken = null;

        // Crear cliente axios
        this.client = axios.create({ baseURL });

        // Request interceptor: Agregar token automáticamente
        this.client.interceptors.request.use(config => {
            if (this.accessToken) {
                config.headers.Authorization = `Bearer ${this.accessToken}`;
                // AccountId viene en el token, NO es necesario enviarlo en header
            }
            return config;
        });

        // Response interceptor: Manejar refresh automático
        this.client.interceptors.response.use(
            response => response,
            async error => {
                const originalRequest = error.config;

                // Si es 401 y no hemos intentado refresh
                if (error.response?.status === 401 && !originalRequest._retry) {
                    originalRequest._retry = true;

                    try {
                        console.log('🔄 AccessToken expirado, refrescando...');
                        await this.refresh();
                        
                        // Reintentar request original
                        originalRequest.headers.Authorization = `Bearer ${this.accessToken}`;
                        return this.client(originalRequest);

                    } catch (refreshError) {
                        console.error('❌ Refresh token inválido, necesitas hacer login nuevamente');
                        this.clearTokens();
                        throw new Error('Session expired - please login again');
                    }
                }

                return Promise.reject(error);
            }
        );
    }

    // ==================== AUTENTICACIÓN ====================

    /**
     * Login con usuario y contraseña
     */
    async login(username, password) {
        const response = await axios.post(`${this.baseURL}/auth/login`, {
            username,
            password
        });

        const data = response.data;
        
        if (data.success) {
            this.accessToken = data.accessToken;
            this.refreshToken = data.refreshToken;
            // AccountId ya viene en el token, no necesita guardarse separado
            
            console.log('✅ Login exitoso');
            console.log(`   UserId: ${data.userId}`);
            console.log(`   AccountId: ${data.accountId} (incluido en token)`);
            console.log(`   AccessToken expira: ${data.accessTokenExpiresAt}`);
            console.log(`   RefreshToken expira: ${data.refreshTokenExpiresAt}`);
            
            return data;
        } else {
            throw new Error(data.error || 'Login failed');
        }
    }

    /**
     * Refrescar AccessToken usando RefreshToken
     */
    async refresh() {
        if (!this.refreshToken) {
            throw new Error('No refresh token available');
        }

        const response = await axios.post(`${this.baseURL}/auth/refresh`, {
            accessToken: this.accessToken,
            refreshToken: this.refreshToken
        });

        const data = response.data;
        
        if (data.success) {
            this.accessToken = data.accessToken;
            this.refreshToken = data.refreshToken; // Nuevo refresh token
            
            console.log('✅ Tokens refrescados exitosamente');
            return data;
        } else {
            throw new Error(data.error || 'Refresh failed');
        }
    }

    /**
     * Logout y revocar refresh token
     */
    async logout() {
        if (!this.accessToken) return;

        try {
            await this.client.post('/auth/logout', {
                refreshToken: this.refreshToken
            });
            console.log('✅ Logout exitoso');
        } finally {
            this.clearTokens();
        }
    }

    /**
     * Limpiar tokens guardados
     */
    clearTokens() {
        this.accessToken = null;
        this.refreshToken = null;
        // accountId no se guarda, viene en el token
    }

    /**
     * Verificar si está autenticado
     */
    isAuthenticated() {
        return this.accessToken != null;
    }

    // ==================== FACILITIES ====================

    async getFacilities() {
        const { data } = await this.client.get('/facility');
        return data;
    }

    async getFacilityById(id) {
        const { data } = await this.client.get(`/facility/${id}`);
        return data;
    }

    async createFacility(facility) {
        const { data } = await this.client.post('/facility', facility);
        return data;
    }

    async updateFacility(id, facility) {
        const { data } = await this.client.put(`/facility/${id}`, facility);
        return data;
    }

    async deleteFacility(id) {
        await this.client.delete(`/facility/${id}`);
    }

    // ==================== WASTES ====================

    async getWastes() {
        const { data } = await this.client.get('/waste');
        return data;
    }

    async getWasteById(id) {
        const { data } = await this.client.get(`/waste/${id}`);
        return data;
    }

    // ==================== MANAGEMENT ====================

    async generateWaste(waste) {
        const { data } = await this.client.post('/management/generate', waste);
        return data;
    }

    async collectWaste(collectData) {
        const { data } = await this.client.post('/management/collect', collectData);
        return data;
    }

    async transportWaste(transportData) {
        const { data } = await this.client.post('/management/transport', transportData);
        return data;
    }

    async getWasteHistory(wasteId) {
        const { data } = await this.client.get(`/management/waste/${wasteId}/history`);
        return data;
    }

    // ==================== INVENTORY ====================

    async getInventory(filters = {}) {
        const { data } = await this.client.get('/inventory', { params: filters });
        return data;
    }

    // ==================== USERS ====================

    async getCurrentUser() {
        const { data } = await this.client.get('/user/me');
        return data;
    }

    async getUserPermissions() {
        const { data } = await this.client.get('/permission/me/permissions');
        return data;
    }
}

// ==================== EJEMPLO DE USO ====================

async function ejemplo() {
    const api = new GreesstAPIClient('http://localhost:5000/api');

    try {
        // 1. Login
        await api.login('admin@gresst.com', 'admin123');

        // 2. Obtener datos (automáticamente usa AccessToken)
        const facilities = await api.getFacilities();
        console.log(`📦 Facilities: ${facilities.length}`);

        // 3. Si AccessToken expira, se refresca automáticamente
        // y el request se reintenta
        const wastes = await api.getWastes();
        console.log(`♻️  Wastes: ${wastes.length}`);

        // 4. Operaciones de negocio
        const newWaste = await api.generateWaste({
            code: 'WASTE-001',
            description: 'Residuos plásticos',
            wasteTypeId: '00000000-0000-0000-0000-000000000001',
            quantity: 100,
            unit: 1,
            generatorId: '00000000-0000-0000-0002-000000000001',
            isHazardous: false
        });
        console.log(`✅ Residuo generado: ${newWaste.id}`);

        // 5. Logout cuando termines
        await api.logout();

    } catch (error) {
        console.error('❌ Error:', error.message);
    }
}

// Ejecutar ejemplo
if (require.main === module) {
    ejemplo();
}

module.exports = GreesstAPIClient;

