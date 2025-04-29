/**
 * Servicio para gestionar operaciones de autenticación
 * Proporciona funciones para iniciar sesión, cerrar sesión y gestionar tokens
 */

const AuthService = {
    /**
     * Almacena el token de autenticación y los datos del usuario
     * @param {string} token - Token JWT
     * @param {Object} userData - Datos del usuario
     */
    setAuth(token, userData) {
        localStorage.setItem(API_CONFIG.TOKEN_KEY, token);
        localStorage.setItem(API_CONFIG.USER_KEY, JSON.stringify(userData));
    },

    /**
     * Elimina el token y los datos del usuario almacenados
     */
    clearAuth() {
        localStorage.removeItem(API_CONFIG.TOKEN_KEY);
        localStorage.removeItem(API_CONFIG.USER_KEY);
    },

    /**
     * Obtiene los datos del usuario actual
     * @returns {Object|null} Datos del usuario o null si no hay sesión
     */
    getCurrentUser() {
        const userData = localStorage.getItem(API_CONFIG.USER_KEY);
        return userData ? JSON.parse(userData) : null;
    },

    /**
     * Verifica si hay un usuario autenticado
     * @returns {boolean} true si hay un usuario autenticado, false en caso contrario
     */
    isAuthenticated() {
        return !!localStorage.getItem(API_CONFIG.TOKEN_KEY);
    },

    /**
     * Inicia sesión con credenciales
     * @param {string} username - Nombre de usuario
     * @param {string} password - Contraseña
     * @returns {Promise<Object>} Datos de la sesión (token y usuario)
     */
    async login(username, password) {
        try {
            const response = await ApiService.post(
                API_CONFIG.ENDPOINTS.AUTH.LOGIN, 
                { username, password }, 
                false // No requiere autenticación previa
            );
            
            if (response && response.token) {
                this.setAuth(response.token, response.user);
                return response;
            }
            
            throw new Error('Respuesta de autenticación inválida');
        } catch (error) {
            console.error('Error en el inicio de sesión:', error);
            throw error;
        }
    },

    /**
     * Cierra la sesión del usuario actual
     * @returns {Promise<void>}
     */
    async logout() {
        try {
            // Intenta hacer logout en el servidor (puede fallar si el token ya expiró)
            try {
                await ApiService.post(API_CONFIG.ENDPOINTS.AUTH.LOGOUT);
            } catch (error) {
                console.warn('Error al hacer logout en el servidor:', error);
                // Continuamos con el logout local aunque falle en el servidor
            }
            
            // Siempre limpiar los datos locales
            this.clearAuth();
            
            // Redirigir a la página de login
            window.location.reload();
        } catch (error) {
            console.error('Error al cerrar sesión:', error);
            this.clearAuth(); // Forzar limpieza local en caso de error
            throw error;
        }
    },

    /**
     * Verifica si el token actual es válido
     * @returns {Promise<boolean>} true si el token es válido, false en caso contrario
     */
    async validateToken() {
        try {
            if (!this.isAuthenticated()) {
                return false;
            }
            
            const response = await ApiService.get(API_CONFIG.ENDPOINTS.AUTH.VALIDATE_TOKEN);
            return !!response && response.valid === true;
        } catch (error) {
            console.error('Error al validar token:', error);
            return false;
        }
    },

    /**
     * Intenta renovar el token de autenticación
     * @returns {Promise<boolean>} true si se renovó el token, false en caso contrario
     */
    async refreshToken() {
        try {
            const response = await ApiService.post(API_CONFIG.ENDPOINTS.AUTH.REFRESH_TOKEN);
            
            if (response && response.token) {
                const userData = this.getCurrentUser();
                this.setAuth(response.token, userData);
                return true;
            }
            
            return false;
        } catch (error) {
            console.error('Error al renovar token:', error);
            return false;
        }
    }
};