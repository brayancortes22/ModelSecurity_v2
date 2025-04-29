/**
 * Controlador para la gestión de personas
 * Permite listar, crear, editar, eliminar y cambiar el estado de personas
 */

const PersonController = {
    // Elementos del DOM
    elements: {
        personsList: null,
        personForm: null,
        searchInput: null,
        addButton: null,
    },
    
    // Datos
    persons: [],
    currentPerson: null,
    isEditing: false,
    
    /**
     * Inicializa el controlador de personas
     */
    async init() {
        // Obtener referencias a elementos del DOM
        this.elements.personsList = document.getElementById('personsList');
        this.elements.personForm = document.getElementById('personForm');
        this.elements.searchInput = document.getElementById('searchPerson');
        this.elements.addButton = document.getElementById('addPersonBtn');
        
        // Configurar eventos
        this.setupEventListeners();
        
        // Cargar lista de personas
        await this.loadPersons();
    },
    
    /**
     * Configura los escuchadores de eventos
     */
    setupEventListeners() {
        // Evento para botón Agregar
        if (this.elements.addButton) {
            this.elements.addButton.addEventListener('click', () => {
                this.showPersonForm();
            });
        }
        
        // Evento para formulario
        if (this.elements.personForm) {
            this.elements.personForm.addEventListener('submit', (e) => {
                e.preventDefault();
                this.savePerson();
            });
        }
        
        // Evento para búsqueda
        if (this.elements.searchInput) {
            this.elements.searchInput.addEventListener('input', (e) => {
                this.filterPersons(e.target.value);
            });
        }
    },
    
    /**
     * Carga la lista de personas desde el servidor
     */
    async loadPersons() {
        try {
            Helpers.toggleSpinner(true);
            
            // Obtener personas del servidor
            this.persons = await PersonService.getAll();
            
            // Renderizar la lista
            this.renderPersonsList();
            
        } catch (error) {
            console.error('Error al cargar personas:', error);
            Helpers.showError('Error al cargar la lista de personas: ' + error.message);
        } finally {
            Helpers.toggleSpinner(false);
        }
    },
    
    /**
     * Renderiza la lista de personas en la interfaz
     */
    renderPersonsList() {
        if (!this.elements.personsList) return;
        
        // Limpiar lista actual
        this.elements.personsList.innerHTML = '';
        
        // Si no hay personas, mostrar mensaje
        if (!this.persons || this.persons.length === 0) {
            this.elements.personsList.innerHTML = `
                <tr>
                    <td colspan="7" class="text-center">No hay personas registradas</td>
                </tr>
            `;
            return;
        }
        
        // Renderizar cada persona
        this.persons.forEach(person => {
            const row = document.createElement('tr');
            row.dataset.id = person.id;
            
            row.innerHTML = `
                <td>${person.id}</td>
                <td>${person.name}</td>
                <td>${person.firstName} ${person.secondName || ''} ${person.firstLastName} ${person.secondLastName || ''}</td>
                <td>${person.email}</td>
                <td>${person.typeIdentification}: ${person.numberIdentification}</td>
                <td>${Helpers.createStatusBadge(person.active)}</td>
                <td>
                    <button class="btn btn-sm btn-primary btn-edit" title="Editar">
                        <i class="bi bi-pencil"></i>
                    </button>
                    <button class="btn btn-sm ${person.active ? 'btn-danger' : 'btn-success'} btn-toggle-status" title="${person.active ? 'Desactivar' : 'Activar'}">
                        <i class="bi ${person.active ? 'bi-toggle-off' : 'bi-toggle-on'}"></i>
                    </button>
                    <button class="btn btn-sm btn-danger btn-delete" title="Eliminar">
                        <i class="bi bi-trash"></i>
                    </button>
                </td>
            `;
            
            // Agregar eventos a los botones
            row.querySelector('.btn-edit').addEventListener('click', () => {
                this.editPerson(person.id);
            });
            
            row.querySelector('.btn-toggle-status').addEventListener('click', () => {
                this.togglePersonStatus(person.id, !person.active);
            });
            
            row.querySelector('.btn-delete').addEventListener('click', () => {
                this.deletePerson(person.id);
            });
            
            this.elements.personsList.appendChild(row);
        });
    },
    
    /**
     * Filtra la lista de personas según el término de búsqueda
     * @param {string} searchTerm - Término de búsqueda
     */
    filterPersons(searchTerm) {
        if (!searchTerm) {
            this.renderPersonsList();
            return;
        }
        
        searchTerm = searchTerm.toLowerCase();
        
        // Filtrar personas que coincidan con el término de búsqueda
        const filtered = this.persons.filter(person => {
            return (
                person.name.toLowerCase().includes(searchTerm) ||
                person.firstName.toLowerCase().includes(searchTerm) ||
                person.firstLastName.toLowerCase().includes(searchTerm) ||
                person.email.toLowerCase().includes(searchTerm) ||
                person.numberIdentification.toString().includes(searchTerm)
            );
        });
        
        // Guardar personas originales
        const originalPersons = this.persons;
        
        // Actualizar lista con resultados filtrados
        this.persons = filtered;
        this.renderPersonsList();
        
        // Restaurar lista original
        this.persons = originalPersons;
    },
    
    /**
     * Muestra el formulario para crear/editar una persona
     * @param {Object} person - Datos de la persona a editar (opcional)
     */
    showPersonForm(person = null) {
        // Obtener referencia al modal y formulario
        const modal = new bootstrap.Modal(document.getElementById('personModal'));
        const form = document.getElementById('personForm');
        
        if (!form) return;
        
        // Limpiar formulario
        form.reset();
        
        // Establecer modo (creación o edición)
        this.isEditing = !!person;
        this.currentPerson = person;
        
        // Actualizar título del modal
        document.getElementById('personModalTitle').textContent = 
            this.isEditing ? 'Editar Persona' : 'Crear Nueva Persona';
        
        // Si estamos editando, rellenar el formulario
        if (this.isEditing && person) {
            Object.keys(person).forEach(key => {
                const input = form.elements[key];
                if (input) {
                    input.value = person[key];
                }
            });
        }
        
        // Mostrar modal
        modal.show();
    },
    
    /**
     * Guarda los datos de una persona (creación o edición)
     */
    async savePerson() {
        try {
            // Obtener formulario y datos
            const form = document.getElementById('personForm');
            
            if (!form) return;
            
            // Validar formulario
            if (!form.checkValidity()) {
                form.classList.add('was-validated');
                return;
            }
            
            // Recopilar datos del formulario
            const formData = new FormData(form);
            const personData = {};
            
            formData.forEach((value, key) => {
                // Convertir valores según el tipo de dato esperado
                if (key === 'numberIdentification') {
                    personData[key] = parseInt(value, 10);
                } else if (key === 'active' || key === 'signig') {
                    personData[key] = value === 'true' || value === 'on';
                } else {
                    personData[key] = value;
                }
            });
            
            // Mostrar spinner
            Helpers.toggleSpinner(true);
            
            // Guardar datos en el servidor
            let savedPerson;
            
            if (this.isEditing) {
                // Edición: actualizar persona existente
                personData.id = this.currentPerson.id;
                savedPerson = await PersonService.update(this.currentPerson.id, personData);
                Helpers.showMessage('Persona actualizada correctamente');
            } else {
                // Creación: crear nueva persona
                savedPerson = await PersonService.create(personData);
                Helpers.showMessage('Persona creada correctamente');
            }
            
            // Cerrar modal
            const modal = bootstrap.Modal.getInstance(document.getElementById('personModal'));
            modal.hide();
            
            // Recargar lista de personas
            await this.loadPersons();
            
        } catch (error) {
            console.error('Error al guardar persona:', error);
            Helpers.showError('Error al guardar los datos: ' + error.message);
        } finally {
            Helpers.toggleSpinner(false);
        }
    },
    
    /**
     * Carga los datos de una persona para edición
     * @param {number} id - ID de la persona
     */
    async editPerson(id) {
        try {
            Helpers.toggleSpinner(true);
            
            // Obtener datos de la persona desde el servidor
            const person = await PersonService.getById(id);
            
            // Mostrar formulario con los datos cargados
            this.showPersonForm(person);
            
        } catch (error) {
            console.error(`Error al cargar datos de la persona ID ${id}:`, error);
            Helpers.showError('Error al cargar los datos de la persona: ' + error.message);
        } finally {
            Helpers.toggleSpinner(false);
        }
    },
    
    /**
     * Cambia el estado de activación de una persona
     * @param {number} id - ID de la persona
     * @param {boolean} newStatus - Nuevo estado (true = activo, false = inactivo)
     */
    async togglePersonStatus(id, newStatus) {
        if (!confirm(newStatus 
            ? '¿Está seguro de activar esta persona?' 
            : '¿Está seguro de desactivar esta persona?')) {
            return;
        }
        
        try {
            Helpers.toggleSpinner(true);
            
            // Actualizar estado en el servidor
            if (newStatus) {
                await PersonService.activate(id);
                Helpers.showMessage('Persona activada correctamente');
            } else {
                await PersonService.deactivate(id);
                Helpers.showMessage('Persona desactivada correctamente');
            }
            
            // Recargar lista de personas
            await this.loadPersons();
            
        } catch (error) {
            console.error(`Error al cambiar estado de la persona ID ${id}:`, error);
            Helpers.showError('Error al cambiar el estado de la persona: ' + error.message);
        } finally {
            Helpers.toggleSpinner(false);
        }
    },
    
    /**
     * Elimina una persona
     * @param {number} id - ID de la persona
     */
    async deletePerson(id) {
        if (!confirm('¿Está seguro de eliminar esta persona? Esta acción no se puede deshacer.')) {
            return;
        }
        
        try {
            Helpers.toggleSpinner(true);
            
            // Eliminar persona del servidor (usando soft delete)
            await PersonService.softDelete(id);
            
            Helpers.showMessage('Persona eliminada correctamente');
            
            // Recargar lista de personas
            await this.loadPersons();
            
        } catch (error) {
            console.error(`Error al eliminar la persona ID ${id}:`, error);
            Helpers.showError('Error al eliminar la persona: ' + error.message);
        } finally {
            Helpers.toggleSpinner(false);
        }
    }
};