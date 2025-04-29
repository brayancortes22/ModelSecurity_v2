/**
 * Servicio base para realizar llamadas a la API
 * Proporciona métodos para todas las operaciones HTTP comunes
 */

const ApiService = {
    /**
     * Realiza una petición HTTP
     * @param {string} url - URL a la que realizar la petición
     * @param {Object} options - Opciones de la petición
     * @returns {Promise<any>} Respuesta de la API
     */
    async request(url, options = {}) {
        try {
            // Agregar URL base si la URL no es absoluta
            const fullUrl = url.startsWith('http') 
                ? url 
                : `${API_CONFIG.BASE_URL}${url}`;
            
            // Agregar token de autenticación si está disponible
            const token = AuthService.getAuthToken();
            if (token) {
                if (!options.headers) {
                    options.headers = {};
                }
                options.headers['Authorization'] = `Bearer ${token}`;
            }
            
            // Configurar headers por defecto para JSON
            if (!options.headers) {
                options.headers = {};
            }
            
            if (!options.headers['Content-Type'] && options.method !== 'GET') {
                options.headers['Content-Type'] = 'application/json';
            }
            
            // Configurar timeout
            const controller = new AbortController();
            const timeoutId = setTimeout(() => controller.abort(), API_CONFIG.TIMEOUT);
            options.signal = controller.signal;
            
            // Realizar la petición
            const response = await fetch(fullUrl, options);
            
            // Limpiar timeout
            clearTimeout(timeoutId);
            
            // En caso de error HTTP
            if (!response.ok) {
                // Si es un error de autenticación y tenemos un token, intentar renovarlo
                if (response.status === 401 && token) {
                    const tokenRefreshed = await AuthService.refreshToken();
                    
                    if (tokenRefreshed) {
                        // Reintentar con el nuevo token
                        return this.request(url, options);
                    } else {
                        // Si no se pudo renovar, cerrar sesión
                        AuthService.clearAuth();
                        throw new Error('Sesión expirada. Por favor, inicie sesión nuevamente.');
                    }
                }
                
                // Intentar obtener mensaje de error del servidor
                let errorMessage;
                try {
                    const errorData = await response.json();
                    errorMessage = errorData.message || errorData.error || `Error ${response.status}: ${response.statusText}`;
                } catch (e) {
                    errorMessage = `Error ${response.status}: ${response.statusText}`;
                }
                
                throw new Error(errorMessage);
            }
            
            // Comprobar si la respuesta está vacía
            const contentType = response.headers.get('content-type');
            if (!contentType || !contentType.includes('application/json')) {
                return response.text();
            }
            
            // Devolver respuesta como JSON
            return await response.json();
            
        } catch (error) {
            // Manejar errores de red o timeout
            if (error.name === 'AbortError') {
                throw new Error('La solicitud ha excedido el tiempo máximo de espera.');
            }
            
            console.error('Error en la petición API:', error);
            throw error;
        }
    },
    
    /**
     * Realiza una petición GET
     * @param {string} url - URL a la que realizar la petición
     * @param {Object} params - Parámetros de la petición
     * @returns {Promise<any>} Respuesta de la API
     */
    async get(url, params = {}) {
        // Construir URL con parámetros
        let urlWithParams = url;
        
        if (Object.keys(params).length > 0) {
            const queryParams = new URLSearchParams();
            
            for (const key in params) {
                if (params[key] !== undefined && params[key] !== null) {
                    queryParams.append(key, params[key]);
                }
            }
            
            urlWithParams = `${url}?${queryParams.toString()}`;
        }
        
        return this.request(urlWithParams, { method: 'GET' });
    },
    
    /**
     * Realiza una petición POST
     * @param {string} url - URL a la que realizar la petición
     * @param {Object} data - Datos a enviar
     * @returns {Promise<any>} Respuesta de la API
     */
    async post(url, data = {}) {
        return this.request(url, {
            method: 'POST',
            body: JSON.stringify(data)
        });
    },
    
    /**
     * Realiza una petición PUT
     * @param {string} url - URL a la que realizar la petición
     * @param {Object} data - Datos a enviar
     * @returns {Promise<any>} Respuesta de la API
     */
    async put(url, data = {}) {
        return this.request(url, {
            method: 'PUT',
            body: JSON.stringify(data)
        });
    },
    
    /**
     * Realiza una petición PATCH
     * @param {string} url - URL a la que realizar la petición
     * @param {Object} data - Datos a enviar
     * @returns {Promise<any>} Respuesta de la API
     */
    async patch(url, data = {}) {
        return this.request(url, {
            method: 'PATCH',
            body: JSON.stringify(data)
        });
    },
    
    /**
     * Realiza una petición DELETE
     * @param {string} url - URL a la que realizar la petición
     * @returns {Promise<any>} Respuesta de la API
     */
    async delete(url) {
        return this.request(url, { method: 'DELETE' });
    },
    
    /**
     * Sube un archivo
     * @param {string} url - URL a la que realizar la petición
     * @param {FormData} formData - FormData con los archivos a subir
     * @returns {Promise<any>} Respuesta de la API
     */
    async uploadFile(url, formData) {
        return this.request(url, {
            method: 'POST',
            body: formData,
            // No establecer Content-Type para que el navegador establezca el boundary correcto
            headers: {}
        });
    }
};