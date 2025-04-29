/**
 * Funciones de utilidad para la aplicación
 */

const Helpers = {
    /**
     * Muestra un mensaje utilizando el modal
     * @param {string} message - Mensaje a mostrar
     * @param {string} title - Título del modal (opcional)
     */
    showMessage(message, title = 'Información') {
        const modalTitle = document.getElementById('modalTitle');
        const modalMessage = document.getElementById('modalMessage');
        const messageModal = new bootstrap.Modal(document.getElementById('messageModal'));
        
        modalTitle.textContent = title;
        modalMessage.textContent = message;
        messageModal.show();
    },
    
    /**
     * Muestra un mensaje de error utilizando el modal
     * @param {string|Error} error - Error o mensaje de error
     */
    showError(error) {
        const message = error instanceof Error ? error.message : error;
        this.showMessage(message, 'Error');
    },
    
    /**
     * Muestra u oculta el spinner de carga
     * @param {boolean} show - true para mostrar, false para ocultar
     */
    toggleSpinner(show) {
        const spinner = document.getElementById('loadingSpinner');
        if (show) {
            spinner.classList.remove('d-none');
        } else {
            spinner.classList.add('d-none');
        }
    },
    
    /**
     * Formatea una fecha a formato local
     * @param {string|Date} date - Fecha a formatear
     * @param {Object} options - Opciones de formato (opcional)
     * @returns {string} Fecha formateada
     */
    formatDate(date, options = {}) {
        if (!date) return '';
        
        const defaultOptions = { 
            day: '2-digit', 
            month: '2-digit', 
            year: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        };
        
        const formatOptions = { ...defaultOptions, ...options };
        const dateObj = date instanceof Date ? date : new Date(date);
        
        return dateObj.toLocaleDateString('es-ES', formatOptions);
    },
    
    /**
     * Valida que un campo no esté vacío
     * @param {string} value - Valor a validar
     * @returns {boolean} true si el valor no está vacío
     */
    isNotEmpty(value) {
        return value !== null && value !== undefined && value.toString().trim() !== '';
    },
    
    /**
     * Valida una dirección de email
     * @param {string} email - Email a validar
     * @returns {boolean} true si el email es válido
     */
    isValidEmail(email) {
        const emailPattern = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$/;
        return emailPattern.test(email);
    },
    
    /**
     * Valida un número de identificación
     * @param {string|number} id - Número a validar
     * @returns {boolean} true si el número es válido
     */
    isValidIdNumber(id) {
        return /^\d+$/.test(id.toString());
    },
    
    /**
     * Formatea el texto para la visualización (por ejemplo, convierte camelCase a palabras)
     * @param {string} text - Texto a formatear
     * @returns {string} Texto formateado
     */
    formatLabel(text) {
        if (!text) return '';
        
        // Convierte camelCase a palabras separadas
        const formatted = text
            .replace(/([A-Z])/g, ' $1')
            .replace(/^./, str => str.toUpperCase());
            
        return formatted;
    },
    
    /**
     * Formatea un valor booleano como texto (Sí/No, Activo/Inactivo)
     * @param {boolean} value - Valor a formatear
     * @param {string} format - Formato de salida ('yesno' o 'activestatus')
     * @returns {string} Texto formateado
     */
    formatBoolean(value, format = 'yesno') {
        if (format === 'activestatus') {
            return value ? 'Activo' : 'Inactivo';
        }
        return value ? 'Sí' : 'No';
    },
    
    /**
     * Crea un elemento HTML para un badge de estado (activo/inactivo)
     * @param {boolean} isActive - Indicador de si está activo
     * @returns {string} HTML para el badge
     */
    createStatusBadge(isActive) {
        const badgeClass = isActive ? 'bg-success' : 'bg-danger';
        const text = isActive ? 'Activo' : 'Inactivo';
        return `<span class="badge ${badgeClass}">${text}</span>`;
    },
    
    /**
     * Convierte un objeto a parámetros de URL
     * @param {Object} params - Objeto con los parámetros
     * @returns {string} Cadena de parámetros URL
     */
    toQueryString(params) {
        return Object.keys(params)
            .filter(key => params[key] !== null && params[key] !== undefined)
            .map(key => `${encodeURIComponent(key)}=${encodeURIComponent(params[key])}`)
            .join('&');
    },
    
    /**
     * Obtiene los parámetros de la URL actual
     * @returns {Object} Objeto con los parámetros
     */
    getUrlParams() {
        const params = {};
        const queryString = window.location.search.substring(1);
        
        if (queryString) {
            const pairs = queryString.split('&');
            for (const pair of pairs) {
                const [key, value] = pair.split('=');
                params[decodeURIComponent(key)] = decodeURIComponent(value || '');
            }
        }
        
        return params;
    },
    
    /**
     * Genera un ID único temporal
     * @returns {string} ID único
     */
    generateTempId() {
        return `temp_${Date.now()}_${Math.floor(Math.random() * 1000)}`;
    }
};