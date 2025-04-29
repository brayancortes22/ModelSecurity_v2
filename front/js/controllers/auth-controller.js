/**
 * Controlador de autenticación
 * Gestiona las operaciones de inicio y cierre de sesión
 */

const AuthController = {
    // Elementos del DOM para el formulario de login
    elements: {
        loginForm: null,
        usernameInput: null,
        passwordInput: null,
        submitButton: null
    },
    
    /**
     * Inicializa el controlador de autenticación
     */
    init() {
        // Obtener referencias a elementos del DOM
        this.elements.loginForm = document.getElementById('loginForm');
        this.elements.usernameInput = document.getElementById('username');
        this.elements.passwordInput = document.getElementById('password');
        
        // Si el formulario existe, configurar el evento de envío
        if (this.elements.loginForm) {
            this.elements.loginForm.addEventListener('submit', (e) => {
                e.preventDefault();
                this.handleLogin();
            });
        }
    },
    
    /**
     * Maneja el proceso de inicio de sesión
     */
    async handleLogin() {
        try {
            // Obtener valores del formulario
            const username = this.elements.usernameInput.value.trim();
            const password = this.elements.passwordInput.value.trim();
            
            // Validación básica
            if (!username || !password) {
                Helpers.showError('Por favor, complete todos los campos.');
                return;
            }
            
            // Mostrar spinner de carga
            Helpers.toggleSpinner(true);
            
            // Intentar inicio de sesión
            await AuthService.login(username, password);
            
            // Actualizar información de usuario en la interfaz
            AppController.updateUserInfo();
            
            // Limpiar el formulario
            this.elements.loginForm.reset();
            
            // Cargar la vista del dashboard
            AppController.loadView(CONSTANTS.VIEWS.DASHBOARD);
            
        } catch (error) {
            console.error('Error en el inicio de sesión:', error);
            Helpers.showError('Error en el inicio de sesión: ' + error.message);
        } finally {
            Helpers.toggleSpinner(false);
        }
    },
    
    /**
     * Maneja el proceso de cierre de sesión
     */
    async handleLogout() {
        try {
            Helpers.toggleSpinner(true);
            
            // Cerrar sesión en el servidor y limpiar datos locales
            await AuthService.logout();
            
            // Actualizar interfaz
            AppController.updateUserInfo();
            
            // Redirigir a la página de login
            AppController.loadView(CONSTANTS.VIEWS.LOGIN);
            
        } catch (error) {
            console.error('Error al cerrar sesión:', error);
            Helpers.showError('Error al cerrar sesión: ' + error.message);
        } finally {
            Helpers.toggleSpinner(false);
        }
    }
};