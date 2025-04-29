/**
 * Servicio de autenticación
 * Gestiona las operaciones de inicio de sesión, cierre de sesión y validación de token
 */

const AuthService = {
    // Clave para almacenar token en localStorage
    TOKEN_KEY: 'ms_auth_token',
    USER_KEY: 'ms_current_user',
    
    // Inicia sesión con usuario y contraseña
    async login(username, password) {
        try {
            const response = await ApiService.post('/auth/login', {
                username,
                password
            });
            
            if (response && response.token) {
                // Guardar token en localStorage
                localStorage.setItem(this.TOKEN_KEY, response.token);
                
                // Guardar información del usuario
                localStorage.setItem(this.USER_KEY, JSON.stringify(response.user));
                
                return response.user;
            } else {
                throw new Error('Respuesta de autenticación inválida');
            }
        } catch (error) {
            console.error('Error en login:', error);
            throw error;
        }
    },
    
    // Cierra la sesión actual
    async logout() {
        try {
            // Llamar al endpoint de logout (opcional, depende de la implementación del backend)
            await ApiService.post('/auth/logout');
        } catch (error) {
            console.warn('Error al cerrar sesión en el servidor:', error);
            // Continuar con el cierre de sesión local aunque falle en el servidor
        } finally {
            // Limpiar almacenamiento local
            this.clearAuth();
        }
    },
    
    // Valida el token actual con el servidor
    async validateToken() {
        try {
            const token = this.getToken();
            
            if (!token) {
                return false;
            }
            
            // Llamar al endpoint para validar token
            const response = await ApiService.get('/auth/validate');
            
            return response && response.valid === true;
        } catch (error) {
            console.error('Error al validar token:', error);
            return false;
        }
    },
    
    // Obtiene el token actual
    getToken() {
        return localStorage.getItem(this.TOKEN_KEY);
    },
    
    // Obtiene el usuario actual desde localStorage
    getCurrentUser() {
        const userJson = localStorage.getItem(this.USER_KEY);
        return userJson ? JSON.parse(userJson) : null;
    },
    
    // Verifica si hay un usuario autenticado
    isAuthenticated() {
        return !!this.getToken();
    },
    
    // Limpia información de autenticación
    clearAuth() {
        localStorage.removeItem(this.TOKEN_KEY);
        localStorage.removeItem(this.USER_KEY);
    },
    
    // Actualiza la información del usuario actual
    updateCurrentUser(userData) {
        const currentUser = this.getCurrentUser();
        
        if (currentUser) {
            const updatedUser = { ...currentUser, ...userData };
            localStorage.setItem(this.USER_KEY, JSON.stringify(updatedUser));
            return updatedUser;
        }
        
        return null;
    },
    
    // Obtener roles y permisos del usuario actual
    getUserRoles() {
        const user = this.getCurrentUser();
        return user && user.roles ? user.roles : [];
    },
    
    // Verificar si el usuario tiene un permiso específico
    hasPermission(permissionCode) {
        const user = this.getCurrentUser();
        
        if (!user || !user.permissions) {
            return false;
        }
        
        return user.permissions.includes(permissionCode);
    },
    
    // Verificar si el usuario tiene un rol específico
    hasRole(roleName) {
        const roles = this.getUserRoles();
        return roles.some(role => role.name === roleName);
    }
};