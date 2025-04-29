/**
 * Configuración de la API
 * Define las URL base y los endpoints para las llamadas a la API
 */

const API_CONFIG = {
    // URL base de la API
    BASE_URL: 'http://localhost:5000/api',
    
    // Timeout para peticiones en milisegundos
    TIMEOUT: 30000,
    
    // Endpoints de la API
    ENDPOINTS: {
        // Autenticación
        AUTH: {
            LOGIN: '/auth/login',
            LOGOUT: '/auth/logout',
            REFRESH: '/auth/refresh',
            VALIDATE: '/auth/validate'
        },
        
        // Usuarios
        USER: {
            BASE: '/users',
            BY_ID: (id) => `/users/${id}`,
            ACTIVATE: (id) => `/users/${id}/activate`,
            DEACTIVATE: (id) => `/users/${id}/deactivate`,
            CHANGE_STATUS: (id) => `/users/${id}/status`,
            CHANGE_PASSWORD: (id) => `/users/${id}/password`
        },
        
        // Personas
        PERSON: {
            BASE: '/persons',
            BY_ID: (id) => `/persons/${id}`,
            ACTIVATE: (id) => `/persons/${id}/activate`,
            DEACTIVATE: (id) => `/persons/${id}/deactivate`,
            CHANGE_STATUS: (id) => `/persons/${id}/status`
        },
        
        // Roles
        ROL: {
            BASE: '/roles',
            BY_ID: (id) => `/roles/${id}`,
            FORMS: (id) => `/roles/${id}/forms`,
            ACTIVATE: (id) => `/roles/${id}/activate`,
            DEACTIVATE: (id) => `/roles/${id}/deactivate`
        },
        
        // Módulos
        MODULE: {
            BASE: '/modules',
            BY_ID: (id) => `/modules/${id}`,
            FORMS: (id) => `/modules/${id}/forms`,
            ACTIVATE: (id) => `/modules/${id}/activate`,
            DEACTIVATE: (id) => `/modules/${id}/deactivate`
        },
        
        // Formularios
        FORM: {
            BASE: '/forms',
            BY_ID: (id) => `/forms/${id}`,
            ACTIVATE: (id) => `/forms/${id}/activate`,
            DEACTIVATE: (id) => `/forms/${id}/deactivate`
        },
        
        // Registro de cambios
        CHANGELOG: {
            BASE: '/changelog',
            BY_ID: (id) => `/changelog/${id}`,
            BY_TABLE: (table) => `/changelog/table/${table}`,
            BY_USER: (user) => `/changelog/user/${user}`
        }
    }
};