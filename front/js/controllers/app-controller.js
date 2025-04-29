/**
 * Controlador principal de la aplicación
 * Gestiona la navegación y carga de vistas
 */

const AppController = {
    // Vista actual
    currentView: null,
    
    // Elementos del DOM
    elements: {
        viewContainer: null,
        navLinks: null,
        userInfo: null,
        logoutButton: null,
    },
    
    /**
     * Inicializa el controlador de la aplicación
     */
    init() {
        this.elements.viewContainer = document.getElementById('viewContainer');
        this.elements.navLinks = document.querySelectorAll('.nav-link[data-view]');
        this.elements.userInfo = document.getElementById('currentUser');
        this.elements.logoutButton = document.getElementById('btnLogout');
        
        // Evento para enlaces de navegación
        this.elements.navLinks.forEach(link => {
            link.addEventListener('click', (e) => {
                e.preventDefault();
                const view = e.target.getAttribute('data-view');
                this.loadView(view);
            });
        });
        
        // Evento para botón de cerrar sesión
        this.elements.logoutButton.addEventListener('click', () => {
            AuthController.handleLogout();
        });
        
        // Comprobar autenticación al iniciar
        this.checkAuth();
    },
    
    /**
     * Comprueba si el usuario está autenticado y muestra la vista correspondiente
     */
    async checkAuth() {
        const isLoggedIn = AuthService.isAuthenticated();
        
        if (isLoggedIn) {
            // Verificar que el token sea válido
            const isTokenValid = await AuthService.validateToken();
            
            if (isTokenValid) {
                this.updateUserInfo();
                // Cargar vista inicial para usuario autenticado
                this.loadView(CONSTANTS.VIEWS.DASHBOARD);
                return;
            } else {
                // Token no válido, intentar renovar
                const tokenRefreshed = await AuthService.refreshToken();
                
                if (tokenRefreshed) {
                    this.updateUserInfo();
                    this.loadView(CONSTANTS.VIEWS.DASHBOARD);
                    return;
                }
                
                // Si no se pudo renovar, mostrar login
                AuthService.clearAuth();
            }
        }
        
        // Usuario no autenticado o token inválido
        this.loadView(CONSTANTS.VIEWS.LOGIN);
    },
    
    /**
     * Actualiza la información del usuario en la interfaz
     */
    updateUserInfo() {
        const user = AuthService.getCurrentUser();
        
        if (user) {
            this.elements.userInfo.textContent = user.username || 'Usuario';
            this.elements.logoutButton.classList.remove('d-none');
            
            // Mostrar barra de navegación
            document.querySelectorAll('.navbar-nav').forEach(nav => {
                nav.classList.remove('d-none');
            });
        } else {
            this.elements.userInfo.textContent = 'Usuario no autenticado';
            this.elements.logoutButton.classList.add('d-none');
            
            // Ocultar barra de navegación
            document.querySelectorAll('.navbar-nav').forEach(nav => {
                nav.classList.add('d-none');
            });
        }
    },
    
    /**
     * Carga una vista específica en el contenedor principal
     * @param {string} viewName - Nombre de la vista a cargar
     */
    async loadView(viewName) {
        if (this.currentView === viewName) return;
        
        // Si no es la vista de login, verificar autenticación
        if (viewName !== CONSTANTS.VIEWS.LOGIN && !AuthService.isAuthenticated()) {
            this.loadView(CONSTANTS.VIEWS.LOGIN);
            return;
        }
        
        try {
            Helpers.toggleSpinner(true);
            
            // Limpiar contenedor de vista
            this.elements.viewContainer.innerHTML = '';
            
            // Cargar template de la vista
            const viewTemplate = await this.fetchViewTemplate(viewName);
            this.elements.viewContainer.innerHTML = viewTemplate;
            
            // Actualizar enlaces activos
            this.updateActiveNavLink(viewName);
            
            // Inicializar controlador específico de la vista
            await this.initViewController(viewName);
            
            // Actualizar vista actual
            this.currentView = viewName;
        } catch (error) {
            console.error('Error al cargar la vista:', error);
            Helpers.showError('Error al cargar la vista: ' + error.message);
        } finally {
            Helpers.toggleSpinner(false);
        }
    },
    
    /**
     * Obtiene la plantilla HTML para una vista
     * @param {string} viewName - Nombre de la vista
     * @returns {Promise<string>} HTML de la vista
     */
    async fetchViewTemplate(viewName) {
        // Si es login, usamos el template que ya está en el HTML
        if (viewName === CONSTANTS.VIEWS.LOGIN) {
            const loginTemplate = document.getElementById('loginView').outerHTML;
            return loginTemplate;
        }
        
        // Para otras vistas, cargar desde archivos
        try {
            const response = await fetch(`views/${viewName}.html`);
            
            if (!response.ok) {
                throw new Error(`Error al cargar la vista ${viewName}`);
            }
            
            return await response.text();
        } catch (error) {
            console.error('Error al cargar template:', error);
            return `<div class="alert alert-danger">Error al cargar la vista ${viewName}</div>`;
        }
    },
    
    /**
     * Actualiza el enlace activo en la navegación
     * @param {string} viewName - Nombre de la vista actual
     */
    updateActiveNavLink(viewName) {
        // Quitar clase activa de todos los enlaces
        this.elements.navLinks.forEach(link => {
            link.classList.remove('active');
        });
        
        // Añadir clase activa al enlace correspondiente
        const activeLink = document.querySelector(`.nav-link[data-view="${viewName}"]`);
        if (activeLink) {
            activeLink.classList.add('active');
        }
    },
    
    /**
     * Inicializa el controlador específico para una vista
     * @param {string} viewName - Nombre de la vista
     */
    async initViewController(viewName) {
        switch (viewName) {
            case CONSTANTS.VIEWS.LOGIN:
                AuthController.init();
                break;
            case CONSTANTS.VIEWS.DASHBOARD:
                // Inicializar dashboard cuando se implemente
                break;
            case CONSTANTS.VIEWS.USERS:
                if (typeof UserController !== 'undefined') {
                    UserController.init();
                }
                break;
            case CONSTANTS.VIEWS.PERSONS:
                if (typeof PersonController !== 'undefined') {
                    PersonController.init();
                }
                break;
            case CONSTANTS.VIEWS.ROLES:
                if (typeof RoleController !== 'undefined') {
                    RoleController.init();
                }
                break;
            case CONSTANTS.VIEWS.MODULES:
                if (typeof ModuleController !== 'undefined') {
                    ModuleController.init();
                }
                break;
            default:
                console.warn(`No se encontró controlador para la vista: ${viewName}`);
        }
    }
};