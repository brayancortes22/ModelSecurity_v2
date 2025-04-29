/**
 * Servicio para gestionar las personas en el sistema
 * Proporciona métodos para realizar operaciones CRUD sobre las personas
 */

const PersonService = {
    /**
     * Obtiene todas las personas
     * @param {Object} params - Parámetros para filtrar y paginar
     * @returns {Promise<Array>} Lista de personas
     */
    async getAll(params = {}) {
        try {
            return await ApiService.get(API_CONFIG.ENDPOINTS.PERSON.BASE, params);
        } catch (error) {
            console.error('Error al obtener personas:', error);
            throw error;
        }
    },
    
    /**
     * Obtiene una persona por su ID
     * @param {number} id - ID de la persona
     * @returns {Promise<Object>} Datos de la persona
     */
    async getById(id) {
        try {
            return await ApiService.get(API_CONFIG.ENDPOINTS.PERSON.BY_ID(id));
        } catch (error) {
            console.error(`Error al obtener persona con ID ${id}:`, error);
            throw error;
        }
    },
    
    /**
     * Crea una nueva persona
     * @param {Object} personData - Datos de la persona
     * @returns {Promise<Object>} Persona creada
     */
    async create(personData) {
        try {
            return await ApiService.post(API_CONFIG.ENDPOINTS.PERSON.BASE, personData);
        } catch (error) {
            console.error('Error al crear persona:', error);
            throw error;
        }
    },
    
    /**
     * Actualiza una persona existente
     * @param {number} id - ID de la persona
     * @param {Object} personData - Datos actualizados de la persona
     * @returns {Promise<Object>} Persona actualizada
     */
    async update(id, personData) {
        try {
            return await ApiService.put(API_CONFIG.ENDPOINTS.PERSON.BY_ID(id), personData);
        } catch (error) {
            console.error(`Error al actualizar persona con ID ${id}:`, error);
            throw error;
        }
    },
    
    /**
     * Elimina una persona (desactivación lógica)
     * @param {number} id - ID de la persona
     * @returns {Promise<Object>} Resultado de la operación
     */
    async delete(id) {
        try {
            return await ApiService.delete(API_CONFIG.ENDPOINTS.PERSON.BY_ID(id));
        } catch (error) {
            console.error(`Error al eliminar persona con ID ${id}:`, error);
            throw error;
        }
    },
    
    /**
     * Activa una persona
     * @param {number} id - ID de la persona
     * @returns {Promise<Object>} Resultado de la operación
     */
    async activate(id) {
        try {
            return await ApiService.post(API_CONFIG.ENDPOINTS.PERSON.ACTIVATE(id));
        } catch (error) {
            console.error(`Error al activar persona con ID ${id}:`, error);
            throw error;
        }
    },
    
    /**
     * Desactiva una persona
     * @param {number} id - ID de la persona
     * @returns {Promise<Object>} Resultado de la operación
     */
    async deactivate(id) {
        try {
            return await ApiService.post(API_CONFIG.ENDPOINTS.PERSON.DEACTIVATE(id));
        } catch (error) {
            console.error(`Error al desactivar persona con ID ${id}:`, error);
            throw error;
        }
    },
    
    /**
     * Cambia el estado de activación de una persona
     * @param {number} id - ID de la persona
     * @param {boolean} active - Nuevo estado de activación
     * @returns {Promise<Object>} Resultado de la operación
     */
    async changeStatus(id, active) {
        try {
            return await ApiService.post(API_CONFIG.ENDPOINTS.PERSON.CHANGE_STATUS(id), { active });
        } catch (error) {
            console.error(`Error al cambiar estado de persona con ID ${id}:`, error);
            throw error;
        }
    },
    
    /**
     * Busca personas por nombre o identificación
     * @param {string} searchTerm - Término de búsqueda
     * @returns {Promise<Array>} Lista de personas que coinciden con la búsqueda
     */
    async search(searchTerm) {
        try {
            return await ApiService.get(API_CONFIG.ENDPOINTS.PERSON.BASE, { search: searchTerm });
        } catch (error) {
            console.error(`Error al buscar personas con término "${searchTerm}":`, error);
            throw error;
        }
    }
};