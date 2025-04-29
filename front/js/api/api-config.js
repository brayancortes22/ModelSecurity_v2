/**
 * Configuración central para los servicios de API
 * Este archivo permite cambiar fácilmente las URL base y otros parámetros
 * de configuración sin tener que modificar los servicios individuales.
 */

const API_CONFIG = {
    // URL base de la API - Cambiar según el entorno
    BASE_URL: 'http://localhost:5000/api',
    
    // Tiempo máximo de espera para las solicitudes en milisegundos
    TIMEOUT: 30000,
    
    // Rutas específicas de los endpoints
    ENDPOINTS: {
        // Autenticación
        AUTH: {
            LOGIN: '/auth/login',
            LOGOUT: '/auth/logout',
            REFRESH_TOKEN: '/auth/refresh-token',
            VALIDATE_TOKEN: '/auth/validate',
        },
        
        // Usuarios
        USER: {
            BASE: '/user',
            BY_ID: (id) => `/user/${id}`,
            ACTIVATE: (id) => `/user/${id}/activar`,
            DEACTIVATE: (id) => `/user/${id}/desactivar`,
            CHANGE_STATUS: (id) => `/user/${id}/cambiarEstado`,
        },
        
        // Personas
        PERSON: {
            BASE: '/person',
            BY_ID: (id) => `/person/${id}`,
            SOFT_DELETE: (id) => `/person/${id}/soft`,
            ACTIVATE: (id) => `/person/${id}/activar`,
            DEACTIVATE: (id) => `/person/${id}/desactivar`,
            CHANGE_STATUS: (id) => `/person/${id}/cambiarEstado`,
            REACTIVATE: (id) => `/person/${id}/reactivar`,
        },
        
        // Roles
        ROL: {
            BASE: '/rol',
            BY_ID: (id) => `/rol/${id}`,
            ACTIVATE: (id) => `/rol/${id}/activar`,
            DEACTIVATE: (id) => `/rol/${id}/desactivar`,
            CHANGE_STATUS: (id) => `/rol/${id}/cambiarEstado`,
        },
        
        // Módulos
        MODULE: {
            BASE: '/module',
            BY_ID: (id) => `/module/${id}`,
            ACTIVATE: (id) => `/module/${id}/activar`,
            DEACTIVATE: (id) => `/module/${id}/desactivar`,
            CHANGE_STATUS: (id) => `/module/${id}/cambiarEstado`,
        },
        
        // Formularios
        FORM: {
            BASE: '/form',
            BY_ID: (id) => `/form/${id}`,
            ACTIVATE: (id) => `/form/${id}/activar`,
            DEACTIVATE: (id) => `/form/${id}/desactivar`,
            CHANGE_STATUS: (id) => `/form/${id}/cambiarEstado`,
        },
        
        // Relación Rol-Formulario
        ROL_FORM: {
            BASE: '/rolform',
            BY_ID: (id) => `/rolform/${id}`,
        },
        
        // Relación Formulario-Módulo
        FORM_MODULE: {
            BASE: '/formmodule',
            BY_ID: (id) => `/formmodule/${id}`,
        },
        
        // Relación Usuario-Rol
        USER_ROL: {
            BASE: '/userrol',
            BY_ID: (id) => `/userrol/${id}`,
        },
    },
    
    // Configuración de cabeceras comunes
    HEADERS: {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
    },
    
    // Nombre de la clave para el token en localStorage
    TOKEN_KEY: 'modelSecurity_token',
    
    // Nombre de la clave para el usuario en localStorage
    USER_KEY: 'modelSecurity_user',
};

/**
 * Cambia la URL base de la API según el entorno
 * @param {string} newBaseUrl - Nueva URL base para la API
 */
function setApiBaseUrl(newBaseUrl) {
    API_CONFIG.BASE_URL = newBaseUrl;
    console.log(`API base URL cambiada a: ${newBaseUrl}`);
}