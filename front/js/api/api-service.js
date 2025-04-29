/**
 * Servicio API base para realizar solicitudes HTTP
 * Este servicio proporciona métodos reutilizables para realizar
 * operaciones CRUD contra la API del backend.
 */

const ApiService = {
    /**
     * Obtiene el token de autenticación del almacenamiento local
     * @returns {string|null} El token JWT o null si no existe
     */
    getAuthToken() {
        return localStorage.getItem(API_CONFIG.TOKEN_KEY);
    },

    /**
     * Crea las cabeceras HTTP para una solicitud
     * @param {boolean} includeAuth - Indica si se debe incluir el token de autenticación
     * @returns {Object} Objeto con las cabeceras HTTP
     */
    getHeaders(includeAuth = true) {
        const headers = { ...API_CONFIG.HEADERS };
        
        if (includeAuth) {
            const token = this.getAuthToken();
            if (token) {
                headers['Authorization'] = `Bearer ${token}`;
            }
        }
        
        return headers;
    },

    /**
     * Realiza una solicitud HTTP GET
     * @param {string} endpoint - Ruta del endpoint
     * @param {boolean} requiresAuth - Indica si la solicitud requiere autenticación
     * @returns {Promise<any>} Promesa con la respuesta
     */
    async get(endpoint, requiresAuth = true) {
        try {
            const url = `${API_CONFIG.BASE_URL}${endpoint}`;
            const headers = this.getHeaders(requiresAuth);
            
            const response = await fetch(url, {
                method: 'GET',
                headers,
                credentials: 'include'
            });
            
            return this.handleResponse(response);
        } catch (error) {
            return this.handleError(error);
        }
    },

    /**
     * Realiza una solicitud HTTP POST
     * @param {string} endpoint - Ruta del endpoint
     * @param {Object} data - Datos a enviar en el cuerpo de la solicitud
     * @param {boolean} requiresAuth - Indica si la solicitud requiere autenticación
     * @returns {Promise<any>} Promesa con la respuesta
     */
    async post(endpoint, data, requiresAuth = true) {
        try {
            const url = `${API_CONFIG.BASE_URL}${endpoint}`;
            const headers = this.getHeaders(requiresAuth);
            
            const response = await fetch(url, {
                method: 'POST',
                headers,
                body: JSON.stringify(data),
                credentials: 'include'
            });
            
            return this.handleResponse(response);
        } catch (error) {
            return this.handleError(error);
        }
    },

    /**
     * Realiza una solicitud HTTP PUT
     * @param {string} endpoint - Ruta del endpoint
     * @param {Object} data - Datos a enviar en el cuerpo de la solicitud
     * @param {boolean} requiresAuth - Indica si la solicitud requiere autenticación
     * @returns {Promise<any>} Promesa con la respuesta
     */
    async put(endpoint, data, requiresAuth = true) {
        try {
            const url = `${API_CONFIG.BASE_URL}${endpoint}`;
            const headers = this.getHeaders(requiresAuth);
            
            const response = await fetch(url, {
                method: 'PUT',
                headers,
                body: JSON.stringify(data),
                credentials: 'include'
            });
            
            return this.handleResponse(response);
        } catch (error) {
            return this.handleError(error);
        }
    },

    /**
     * Realiza una solicitud HTTP PATCH
     * @param {string} endpoint - Ruta del endpoint
     * @param {Object} data - Datos a enviar en el cuerpo de la solicitud
     * @param {boolean} requiresAuth - Indica si la solicitud requiere autenticación
     * @returns {Promise<any>} Promesa con la respuesta
     */
    async patch(endpoint, data = null, requiresAuth = true) {
        try {
            const url = `${API_CONFIG.BASE_URL}${endpoint}`;
            const headers = this.getHeaders(requiresAuth);
            const options = {
                method: 'PATCH',
                headers,
                credentials: 'include'
            };
            
            if (data) {
                options.body = JSON.stringify(data);
            }
            
            const response = await fetch(url, options);
            
            return this.handleResponse(response);
        } catch (error) {
            return this.handleError(error);
        }
    },

    /**
     * Realiza una solicitud HTTP DELETE
     * @param {string} endpoint - Ruta del endpoint
     * @param {boolean} requiresAuth - Indica si la solicitud requiere autenticación
     * @returns {Promise<any>} Promesa con la respuesta
     */
    async delete(endpoint, requiresAuth = true) {
        try {
            const url = `${API_CONFIG.BASE_URL}${endpoint}`;
            const headers = this.getHeaders(requiresAuth);
            
            const response = await fetch(url, {
                method: 'DELETE',
                headers,
                credentials: 'include'
            });
            
            return this.handleResponse(response);
        } catch (error) {
            return this.handleError(error);
        }
    },

    /**
     * Maneja la respuesta de una solicitud HTTP
     * @param {Response} response - Objeto Response de la Fetch API
     * @returns {Promise<any>} Promesa con los datos de la respuesta
     * @throws {Error} Error si la respuesta no es exitosa
     */
    async handleResponse(response) {
        // Primero verificamos si hay datos
        let data;
        try {
            // Algunas respuestas pueden no tener cuerpo (como los 204 No Content)
            data = response.status !== 204 ? await response.json() : null;
        } catch (e) {
            // Si no hay JSON, usamos un objeto vacío
            data = {};
        }

        // Verificamos si la respuesta es exitosa (código 2xx)
        if (response.ok) {
            return data;
        }

        // Manejo específico según el código de estado HTTP
        switch (response.status) {
            case 400:
                throw new Error(data.message || 'Solicitud incorrecta');
            case 401:
                // Desconectar al usuario si hay un error de autenticación
                if (typeof AuthService !== 'undefined') {
                    AuthService.logout();
                }
                throw new Error('No autorizado. Por favor, inicie sesión nuevamente.');
            case 403:
                throw new Error('Acceso prohibido. No tiene permisos para realizar esta acción.');
            case 404:
                throw new Error(data.message || 'Recurso no encontrado');
            case 409:
                throw new Error(data.message || 'Conflicto con el estado actual del recurso');
            case 422:
                throw new Error(data.message || 'Entidad no procesable. Verifique los datos enviados.');
            case 500:
                throw new Error('Error interno del servidor. Por favor, inténtelo más tarde.');
            default:
                throw new Error(`Error: ${response.status} - ${data.message || 'Error desconocido'}`);
        }
    },

    /**
     * Maneja los errores de red
     * @param {Error} error - Objeto Error
     * @throws {Error} Reenvía el error con un mensaje descriptivo
     */
    handleError(error) {
        console.error('Error en la solicitud API:', error);
        throw new Error('Error de conexión. Verifique su conexión a internet e inténtelo de nuevo.');
    }
};