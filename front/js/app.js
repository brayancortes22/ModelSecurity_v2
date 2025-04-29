/**
 * Archivo principal de la aplicación
 * Punto de entrada para inicializar el frontend
 */

document.addEventListener('DOMContentLoaded', () => {
    // Verificar dependencias
    if (!window.bootstrap) {
        console.error('Bootstrap no está cargado. Por favor, asegúrese de incluir bootstrap.bundle.min.js');
        alert('Error al cargar dependencias. Consulte la consola para más detalles.');
        return;
    }
    
    // Inicializar controlador principal
    try {
        AppController.init();
        console.log('Aplicación ModelSecurity inicializada correctamente');
    } catch (error) {
        console.error('Error al inicializar la aplicación:', error);
        alert('Error al inicializar la aplicación: ' + error.message);
    }
    
    // Habilitar tooltips globalmente
    const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]');
    [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl));
});