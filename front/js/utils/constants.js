/**
 * Constantes utilizadas en la aplicación
 */

const CONSTANTS = {
    // Estados de las entidades
    STATUS: {
        ACTIVE: true,
        INACTIVE: false
    },
    
    // Mensajes comunes
    MESSAGES: {
        CREATE_SUCCESS: "Registro creado exitosamente.",
        UPDATE_SUCCESS: "Registro actualizado exitosamente.",
        DELETE_SUCCESS: "Registro eliminado exitosamente.",
        ACTIVATE_SUCCESS: "Registro activado exitosamente.",
        DEACTIVATE_SUCCESS: "Registro desactivada exitosamente.",
        CONFIRMATION: "¿Está seguro de realizar esta acción?",
        DELETE_CONFIRMATION: "¿Está seguro de eliminar este registro? Esta acción no se puede deshacer.",
        GENERIC_ERROR: "Ha ocurrido un error. Por favor, inténtelo nuevamente.",
        SESSION_EXPIRED: "Su sesión ha expirado. Por favor, inicie sesión nuevamente.",
        REQUIRED_FIELD: "Este campo es obligatorio.",
        INVALID_FORMAT: "El formato ingresado no es válido."
    },
    
    // Tipos de permisos
    PERMISSIONS: {
        READ: "READ",
        CREATE: "CREATE",
        UPDATE: "UPDATE",
        DELETE: "DELETE",
        ADMIN: "ADMIN"
    },
    
    // Tipos de identificación
    IDENTIFICATION_TYPES: [
        "DNI", 
        "Pasaporte", 
        "Tarjeta De Identidad",
        "Cédula", 
        "Licencia", 
        "Otro"
    ],
    
    // Roles predefinidos
    ROLES: {
        ADMIN: "Administrador",
        USER: "Usuario",
        GUEST: "Invitado"
    },
    
    // Vistas de la aplicación
    VIEWS: {
        LOGIN: "login",
        DASHBOARD: "dashboard",
        USERS: "users",
        PERSONS: "persons",
        ROLES: "roles",
        MODULES: "modules",
        FORMS: "forms"
    }
};