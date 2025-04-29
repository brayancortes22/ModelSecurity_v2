/**
 * Servicio para gestionar operaciones relacionadas con personas
 * Utiliza el ApiService base para comunicarse con el backend
 */

const PersonService = {
    /**
     * Obtiene todas las personas
     * @returns {Promise<Array>} Lista de personas
     */
    async getAll() {
        try {
            return await ApiService.get(API_CONFIG.ENDPOINTS.PERSON.BASE);
        } catch (error) {
            console.error('Error al obtener las personas:', error);
            throw error;
        }
    },

    /**
     * Obtiene una persona específica por su ID
     * @param {number} id - ID de la persona
     * @returns {Promise<Object>} Datos de la persona
     */
    async getById(id) {
        try {
            return await ApiService.get(API_CONFIG.ENDPOINTS.PERSON.BY_ID(id));
        } catch (error) {
            console.error(`Error al obtener la persona con ID ${id}:`, error);
            throw error;
        }
    },

    /**
     * Crea una nueva persona
     * @param {Object} personData - Datos de la persona a crear
     * @returns {Promise<Object>} Datos de la persona creada
     */
    async create(personData) {
        try {
            return await ApiService.post(API_CONFIG.ENDPOINTS.PERSON.BASE, personData);
        } catch (error) {
            console.error('Error al crear la persona:', error);
            throw error;
        }
    },

    /**
     * Actualiza una persona existente (reemplazo completo)
     * @param {number} id - ID de la persona
     * @param {Object} personData - Nuevos datos de la persona
     * @returns {Promise<Object>} Datos actualizados de la persona
     */
    async update(id, personData) {
        try {
            return await ApiService.put(API_CONFIG.ENDPOINTS.PERSON.BY_ID(id), personData);
        } catch (error) {
            console.error(`Error al actualizar la persona con ID ${id}:`, error);
            throw error;
        }
    },

    /**
     * Actualiza parcialmente una persona
     * @param {number} id - ID de la persona
     * @param {Object} partialData - Datos parciales a actualizar
     * @returns {Promise<Object>} Datos actualizados de la persona
     */
    async patch(id, partialData) {
        try {
            return await ApiService.patch(API_CONFIG.ENDPOINTS.PERSON.BY_ID(id), partialData);
        } catch (error) {
            console.error(`Error al actualizar parcialmente la persona con ID ${id}:`, error);
            throw error;
        }
    },

    /**
     * Elimina permanentemente una persona
     * @param {number} id - ID de la persona a eliminar
     * @returns {Promise<void>}
     */
    async delete(id) {
        try {
            return await ApiService.delete(API_CONFIG.ENDPOINTS.PERSON.BY_ID(id));
        } catch (error) {
            console.error(`Error al eliminar la persona con ID ${id}:`, error);
            throw error;
        }
    },

    /**
     * Desactiva una persona (eliminación lógica)
     * @param {number} id - ID de la persona a desactivar
     * @returns {Promise<void>}
     */
    async softDelete(id) {
        try {
            return await ApiService.delete(API_CONFIG.ENDPOINTS.PERSON.SOFT_DELETE(id));
        } catch (error) {
            console.error(`Error al desactivar la persona con ID ${id}:`, error);
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
            return await ApiService.patch(API_CONFIG.ENDPOINTS.PERSON.ACTIVATE(id));
        } catch (error) {
            console.error(`Error al activar la persona con ID ${id}:`, error);
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
            return await ApiService.patch(API_CONFIG.ENDPOINTS.PERSON.DEACTIVATE(id));
        } catch (error) {
            console.error(`Error al desactivar la persona con ID ${id}:`, error);
            throw error;
        }
    },

    /**
     * Cambia el estado de activación de una persona
     * @param {number} id - ID de la persona
     * @param {boolean} status - Nuevo estado (true=activo, false=inactivo)
     * @returns {Promise<Object>} Resultado de la operación
     */
    async changeStatus(id, status) {
        try {
            return await ApiService.patch(
                `${API_CONFIG.ENDPOINTS.PERSON.CHANGE_STATUS(id)}?estado=${status}`
            );
        } catch (error) {
            console.error(`Error al cambiar el estado de la persona con ID ${id}:`, error);
            throw error;
        }
    },

    /**
     * Reactiva una persona previamente desactivada
     * @param {number} id - ID de la persona
     * @returns {Promise<Object>} Datos de la persona reactivada
     */
    async reactivate(id) {
        try {
            return await ApiService.patch(API_CONFIG.ENDPOINTS.PERSON.REACTIVATE(id));
        } catch (error) {
            console.error(`Error al reactivar la persona con ID ${id}:`, error);
            throw error;
        }
    }
};