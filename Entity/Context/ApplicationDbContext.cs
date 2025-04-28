using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using Entity.Model;

namespace Entity.Contexts
{
    /// <summary>
    /// Representa el contexto de la base de datos de la aplicación, proporcionando configuraciones y métodos
    /// para la gestión de entidades y consultas personalizadas con Dapper.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Configuración de la aplicación.
        /// </summary>
        protected readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor del contexto de la base de datos.
        /// </summary>
        /// <param name="options">Opciones de configuración para el contexto de base de datos.</param>
        /// <param name="configuration">Instancia de IConfiguration para acceder a la configuración de la aplicación.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration)
        : base(options)
        {
            _configuration = configuration;
        }
        ///DB SETS
        /// <summary>
        /// Conjunto de entidades para la gestión de roles en el sistema.
        /// </summary>
        public DbSet<Rol> Rol { get; set; }
        public DbSet<Verification> Verification { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Person> Person { get; set; }
        public DbSet<UserRol> UserRol { get; set; }
        public DbSet<Sede> Sede { get; set; }
        public DbSet<UserSede> UserSede { get; set; }
        public DbSet<Aprendiz> Aprendiz { get; set; }
        public DbSet<Instructor> Instructor { get; set; }
        public DbSet<Process> Process { get; set; }
        public DbSet<Program> Program { get; set; }
        public DbSet<InstructorProgram> InstructorProgram { get; set; }
        public DbSet<AprendizProgram> AprendizProgram { get; set; }
        public DbSet<AprendizProcessInstructor> AprendizProcessInstructor { get; set; }
        /// <summary>
        /// Conjunto de entidades para la gestión de formularios en el sistema.
        /// </summary>
        public DbSet<Form> Form { get; set; }
        /// <summary>
        /// Conjunto de entidades para la gestión de módulos en el sistema.
        /// Nota: Se utiliza el nombre completo Entity.Model.Module para evitar ambigüedad con System.Reflection.Module
        /// </summary>
        public DbSet<Entity.Model.Module> Module { get; set; }
        /// <summary>
        /// Conjunto de entidades para la gestión de la relación entre formularios y módulos.
        /// </summary>
        public DbSet<FormModule> FormModule { get; set; }
        public DbSet<RolForm> RolForm { get; set; }
        public DbSet<TypeModality> TypeModality { get; set; }
        public DbSet<State> State { get; set; }
        public DbSet<RegisterySofia> RegisterySofia { get; set; }
        public DbSet<Regional> Regional { get; set; }
        public DbSet<Center> Center { get; set; }
        public DbSet<Enterprise> Enterprise { get; set; }
        public DbSet<ChangeLog> ChangeLog { get; set; }
        public DbSet<Concept> Concept { get; set; }

        /// <summary>
        /// Configura los modelos de la base de datos aplicando configuraciones desde ensamblados.
        /// </summary>
        /// <param name="modelBuilder">Constructor del modelo de base de datos.</param>
        /// <remarks>
        /// Aquí se configuran las relaciones entre las entidades.
        /// </remarks>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relación 1 a 1: User - Person
            modelBuilder.Entity<User>()
                .HasOne(u => u.Person)
                .WithOne(p => p.User)
                .HasForeignKey<User>(u => u.PersonId);

            // Relación muchos a muchos: User - Rol (UserRol)
            modelBuilder.Entity<UserRol>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRols)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRol>()
                .HasOne(ur => ur.Rol)
                .WithMany(r => r.UserRols)
                .HasForeignKey(ur => ur.RolId);

            // Relación 1 a muchos: Sede - Center
            modelBuilder.Entity<Sede>()
                .HasOne(s => s.Center)
                .WithMany(c => c.Sedes)
                .HasForeignKey(s => s.CenterId);

            // Relación muchos a muchos: User - Sede (UserSede)
            modelBuilder.Entity<UserSede>()
                .HasOne(us => us.User)
                .WithMany(u => u.UserSedes)
                .HasForeignKey(us => us.UserId);

            modelBuilder.Entity<UserSede>()
                .HasOne(us => us.Sede)
                .WithMany(s => s.UserSedes)
                .HasForeignKey(us => us.SedeId);

            // Relación 1 a 1: Aprendiz - User
            modelBuilder.Entity<Aprendiz>()
                .HasOne(a => a.User)
                .WithOne(u => u.Aprendiz)
                .HasForeignKey<Aprendiz>(a => a.UserId);

            // Relación 1 a 1: Instructor - User
            modelBuilder.Entity<Instructor>()
                .HasOne(i => i.User)
                .WithOne(u => u.Instructor)
                .HasForeignKey<Instructor>(i => i.UserId);

            // Relación muchos a muchos: Aprendiz - Program (AprendizProgram)
            modelBuilder.Entity<AprendizProgram>()
                .HasOne(ap => ap.Aprendiz)
                .WithMany(a => a.AprendizPrograms)
                .HasForeignKey(ap => ap.AprendizId);

            modelBuilder.Entity<AprendizProgram>()
                .HasOne(ap => ap.Program)
                .WithMany(p => p.AprendizPrograms)
                .HasForeignKey(ap => ap.ProgramId);

            // Relación muchos a muchos: Instructor - Program (InstructorProgram)
            modelBuilder.Entity<InstructorProgram>()
                .HasOne(ip => ip.Instructor)
                .WithMany(i => i.InstructorPrograms)
                .HasForeignKey(ip => ip.InstructorId);

            modelBuilder.Entity<InstructorProgram>()
                .HasOne(ip => ip.Program)
                .WithMany(p => p.InstructorPrograms)
                .HasForeignKey(ip => ip.ProgramId);

            // Relación muchos a muchos compleja: AprendizProcessInstructor
            modelBuilder.Entity<AprendizProcessInstructor>()
                .HasOne(api => api.TypeModality)
                .WithMany(tm => tm.AprendizProcessInstructors)
                .HasForeignKey(api => api.TypeModalityId);

            modelBuilder.Entity<AprendizProcessInstructor>()
                .HasOne(api => api.RegisterySofia)
                .WithMany(rs => rs.AprendizProcessInstructors)
                .HasForeignKey(api => api.RegisterySofiaId);

            modelBuilder.Entity<AprendizProcessInstructor>()
                .HasOne(api => api.Concept)
                .WithMany(c => c.AprendizProcessInstructors)
                .HasForeignKey(api => api.ConceptId);

            modelBuilder.Entity<AprendizProcessInstructor>()
                .HasOne(api => api.Enterprise)
                .WithMany(e => e.AprendizProcessInstructors)
                .HasForeignKey(api => api.EnterpriseId);

            modelBuilder.Entity<AprendizProcessInstructor>()
                .HasOne(api => api.Process)
                .WithMany(p => p.AprendizProcessInstructors)
                .HasForeignKey(api => api.ProcessId);

            modelBuilder.Entity<AprendizProcessInstructor>()
                .HasOne(api => api.Aprendiz)
                .WithMany(a => a.AprendizProcessInstructors)
                .HasForeignKey(api => api.AprendizId);

            modelBuilder.Entity<AprendizProcessInstructor>()
                .HasOne(api => api.Instructor)
                .WithMany(i => i.AprendizProcessInstructors)
                .HasForeignKey(api => api.InstructorId);

            modelBuilder.Entity<AprendizProcessInstructor>()
                .HasOne(api => api.State)
                .WithMany(s => s.AprendizProcessInstructors)
                .HasForeignKey(api => api.StateId);

            modelBuilder.Entity<AprendizProcessInstructor>()
                .HasOne(api => api.Verification)
                .WithMany(v => v.AprendizProcessInstructors)
                .HasForeignKey(api => api.VerificationId);

            // Relación muchos a muchos: Form - Module (FormModule)
            modelBuilder.Entity<FormModule>()
                .HasOne(fm => fm.Form)
                .WithMany(f => f.FormModules)
                .HasForeignKey(fm => fm.FormId);

            modelBuilder.Entity<FormModule>()
                .HasOne(fm => fm.Module)
                .WithMany(m => m.FormModules)
                .HasForeignKey(fm => fm.ModuleId);

            // Relación muchos a muchos: Rol - Form (RolForm)
            modelBuilder.Entity<RolForm>()
                .HasOne(rf => rf.Rol)
                .WithMany(r => r.RolForms)
                .HasForeignKey(rf => rf.RolId);

            modelBuilder.Entity<RolForm>()
                .HasOne(rf => rf.Form)
                .WithMany(f => f.RolForms)
                .HasForeignKey(rf => rf.FormId);

            // Relación 1 a muchos: Center - Regional
            modelBuilder.Entity<Center>()
                .HasOne(c => c.Regional)
                .WithMany(r => r.Centers)
                .HasForeignKey(c => c.RegionalId);

            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        /// <summary>
        /// Configura opciones adicionales del contexto, como el registro de datos sensibles.
        /// </summary>
        /// <param name="optionsBuilder">Constructor de opciones de configuración del contexto.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            // Otras configuraciones adicionales pueden ir aquí
        }

        /// <summary>
        /// Configura convenciones de tipos de datos, estableciendo la precisión por defecto de los valores decimales.
        /// </summary>
        /// <param name="configurationBuilder">Constructor de configuración de modelos.</param>
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
        }

        /// <summary>
        /// Guarda los cambios en la base de datos, asegurando la auditoría antes de persistir los datos.
        /// </summary>
        /// <returns>Número de filas afectadas.</returns>
        public override int SaveChanges()
        {
            EnsureAudit();
            return base.SaveChanges();
        }

        /// <summary>
        /// Guarda los cambios en la base de datos de manera asíncrona, asegurando la auditoría antes de la persistencia.
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess">Indica si se deben aceptar todos los cambios en caso de éxito.</param>
        /// <param name="cancellationToken">Token de cancelación para abortar la operación.</param>
        /// <returns>Número de filas afectadas de forma asíncrona.</returns>
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            EnsureAudit();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        /// <summary>
        /// Ejecuta una consulta SQL utilizando Dapper y devuelve una colección de resultados de tipo genérico.
        /// </summary>
        /// <typeparam name="T">Tipo de los datos de retorno.</typeparam>
        /// <param name="text">Consulta SQL a ejecutar.</param>
        /// <param name="parameters">Parámetros opcionales de la consulta.</param>
        /// <param name="timeout">Tiempo de espera opcional para la consulta.</param>
        /// <param name="type">Tipo opcional de comando SQL.</param>
        /// <returns>Una colección de objetos del tipo especificado.</returns>
        public async Task<IEnumerable<T>> QueryAsync<T>(string text, object parameters = null, int? timeout = null, CommandType? type = null)
        {
            using var command = new DapperEFCoreCommand(this, text, parameters, timeout, type, CancellationToken.None);
            var connection = this.Database.GetDbConnection();
            return await connection.QueryAsync<T>(command.Definition);
        }

        /// <summary>
        /// Ejecuta una consulta SQL utilizando Dapper y devuelve un solo resultado o el valor predeterminado si no hay resultados.
        /// </summary>
        /// <typeparam name="T">Tipo de los datos de retorno.</typeparam>
        /// <param name="text">Consulta SQL a ejecutar.</param>
        /// <param name="parameters">Parámetros opcionales de la consulta.</param>
        /// <param name="timeout">Tiempo de espera opcional para la consulta.</param>
        /// <param name="type">Tipo opcional de comando SQL.</param>
        /// <returns>Un objeto del tipo especificado o su valor predeterminado.</returns>
        public async Task<T> QueryFirstOrDefaultAsync<T>(string text, object parameters = null, int? timeout = null, CommandType? type = null)
        {
            using var command = new DapperEFCoreCommand(this, text, parameters, timeout, type, CancellationToken.None);
            var connection = this.Database.GetDbConnection();
            return await connection.QueryFirstOrDefaultAsync<T>(command.Definition);
        }

        /// <summary>
        /// Método interno para garantizar la auditoría de los cambios en las entidades.
        /// </summary>
        private void EnsureAudit()
        {
            ChangeTracker.DetectChanges();
        }

        /// <summary>
        /// Estructura para ejecutar comandos SQL con Dapper en Entity Framework Core.
        /// </summary>
        public readonly struct DapperEFCoreCommand : IDisposable
        {
            /// <summary>
            /// Constructor del comando Dapper.
            /// </summary>
            /// <param name="context">Contexto de la base de datos.</param>
            /// <param name="text">Consulta SQL.</param>
            /// <param name="parameters">Parámetros opcionales.</param>
            /// <param name="timeout">Tiempo de espera opcional.</param>
            /// <param name="type">Tipo de comando SQL opcional.</param>
            /// <param name="ct">Token de cancelación.</param>
            public DapperEFCoreCommand(DbContext context, string text, object parameters, int? timeout, CommandType? type, CancellationToken ct)
            {
                var transaction = context.Database.CurrentTransaction?.GetDbTransaction();
                var commandType = type ?? CommandType.Text;
                var commandTimeout = timeout ?? context.Database.GetCommandTimeout() ?? 30;

                Definition = new CommandDefinition(
                    text,
                    parameters,
                    transaction,
                    commandTimeout,
                    commandType,
                    cancellationToken: ct
                );
            }

            /// <summary>
            /// Define los parámetros del comando SQL.
            /// </summary>
            public CommandDefinition Definition { get; }

            /// <summary>
            /// Método para liberar los recursos.
            /// </summary>
            public void Dispose()
            {
            }
        }
    }
}