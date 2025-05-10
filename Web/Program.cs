using Microsoft.EntityFrameworkCore;
using Entity.Contexts;
using Business;
using Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Http;
using Data.Interfaces;
using Entity.Model; // Corregido: namespace correcto para Form
using Entity.DTOs; // Agregado: namespace para FormDto
using Entity.Interfaces; // Agregado: namespace para interfaces de entidades
using Business.Interfaces; // Agregado: namespace para interfaces de negocio
using Data.Repositories; // Agregado: namespace para repositorios
using Data.Factory; // Agregado: namespace para factory de repositorios
using Business.Factory; // Agregado: namespace para factory de servicios de negocio
using Business.Mappers; // Agregado: namespace para mappers
using System.Reflection; // Para AutoMapper
using ModuleEntity = Entity.Model.Module; // Alias para evitar ambigüedad

var builder = WebApplication.CreateBuilder(args);

// Añade esto después de var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    // Configura el servidor para escuchar en puertos alternativos
    serverOptions.ListenAnyIP(7009, listenOptions => {
        listenOptions.UseHttps();
    });
    serverOptions.ListenAnyIP(5188, listenOptions => {
        listenOptions.UseHttps();
    });
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configuración CORS - permitir cualquier origen en desarrollo
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Configuración mejorada de Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { 
        Title = "ModelSecurity API", 
        Version = "v1",
        Description = "API para el sistema de seguridad ModelSecurity"
    });
    
    // Añadir configuración para JWT Authentication en Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Configuración de autenticación JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                builder.Configuration["Jwt:Key"] ?? "DefaultSecretKey123!@#$%^&*()"))
        };
    });

// Configuración predeterminada para SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

Console.WriteLine("Usando SQL Server como proveedor de base de datos");

// Registrar AutoMapper con sus perfiles
// Estamos siendo específicos con el namespace completo para evitar ambigüedades
builder.Services.AddAutoMapper(typeof(Business.Mappers.BaseMapperProfile).Assembly);

// Registrar servicio de mapeo
builder.Services.AddScoped<Business.Mappers.IMappingService, Business.Mappers.MappingService>();

// Registrar los factories (NUEVO)
builder.Services.AddScoped<IRepositoryFactory, RepositoryFactory>();
builder.Services.AddScoped<IBusinessFactory, BusinessFactory>();
builder.Services.AddScoped<IActivacionDataFactory, ActivacionDataFactory>();

// Registrar implementaciones genéricas
// Form - Utilizar solo AutoMapper para simplificar
builder.Services.AddScoped<IGenericRepository<Form, int>, FormData>();
builder.Services.AddScoped<IGenericBusiness<FormDto, int>, AutoMapperFormBusiness>();
builder.Services.AddScoped<AutoMapperFormBusiness>();  // Registro explícito

// También registrar la versión original de FormBusiness para compatibilidad usando factory específico
builder.Services.AddScoped<FormBusiness>(provider => {
    var repositoryFactory = provider.GetRequiredService<IRepositoryFactory>();
    var logger = provider.GetRequiredService<ILogger<FormBusiness>>();
    var mappingService = provider.GetRequiredService<IMappingService>();
    return new FormBusiness(repositoryFactory, logger, mappingService);
});

// Module (usando alias para evitar ambigüedad)
builder.Services.AddScoped<IGenericRepository<ModuleEntity, int>, ModuleData>();
builder.Services.AddScoped<IGenericBusiness<ModuleDto, int>, AutoMapperModuleBusiness>();
builder.Services.AddScoped<AutoMapperModuleBusiness>();  // Registro explícito
builder.Services.AddScoped<ModuleBusiness>(provider => {
    var repositoryFactory = provider.GetRequiredService<IRepositoryFactory>();
    var logger = provider.GetRequiredService<ILogger<ModuleBusiness>>();
    var mappingService = provider.GetRequiredService<IMappingService>();
    return new ModuleBusiness(repositoryFactory, logger, mappingService);
});

// Person
builder.Services.AddScoped<IGenericRepository<Person, int>, PersonData>();
builder.Services.AddScoped<IGenericBusiness<PersonDto, int>, AutoMapperPersonBusiness>();
builder.Services.AddScoped<AutoMapperPersonBusiness>();  // Registro explícito
builder.Services.AddScoped<PersonBusiness>(provider => {
    var repositoryFactory = provider.GetRequiredService<IRepositoryFactory>();
    var logger = provider.GetRequiredService<ILogger<PersonBusiness>>();
    var mappingService = provider.GetRequiredService<IMappingService>();
    return new PersonBusiness(repositoryFactory, logger, mappingService);
});

// Rol
builder.Services.AddScoped<IGenericRepository<Rol, int>, RolData>();
builder.Services.AddScoped<IGenericBusiness<RolDto, int>, AutoMapperRolBusiness>();
builder.Services.AddScoped<AutoMapperRolBusiness>();  // Registro explícito
builder.Services.AddScoped<RolFormData>(); // Necesario para RolBusiness
builder.Services.AddScoped<RolBusiness>(provider => {
    var repositoryFactory = provider.GetRequiredService<IRepositoryFactory>();
    var rolFormData = provider.GetRequiredService<RolFormData>();
    var logger = provider.GetRequiredService<ILogger<RolBusiness>>();
    var mappingService = provider.GetRequiredService<IMappingService>();
    return new RolBusiness(repositoryFactory, rolFormData, logger, mappingService);
});

// User
builder.Services.AddScoped<IGenericRepository<User, int>, UserData>();
builder.Services.AddScoped<IGenericBusiness<UserDto, int>, AutoMapperUserBusiness>();
builder.Services.AddScoped<AutoMapperUserBusiness>();  // Registro explícito
builder.Services.AddScoped<UserData>(); // Necesario para métodos específicos
builder.Services.AddScoped<UserBusiness>(provider => {
    var repositoryFactory = provider.GetRequiredService<IRepositoryFactory>();
    var userData = provider.GetRequiredService<UserData>();
    var logger = provider.GetRequiredService<ILogger<UserBusiness>>();
    var mappingService = provider.GetRequiredService<IMappingService>();
    return new UserBusiness(repositoryFactory, userData, logger, mappingService);
});

// FormModule
builder.Services.AddScoped<IGenericRepository<FormModule, int>, FormModuleData>();
builder.Services.AddScoped<FormModuleData>(); // Necesario para métodos específicos
builder.Services.AddScoped<IGenericBusiness<FormModuleDto, int>, AutoMapperFormModuleBusiness>();
builder.Services.AddScoped<AutoMapperFormModuleBusiness>();  // Registro explícito
builder.Services.AddScoped<FormModuleBusiness>(provider => {
    var repositoryFactory = provider.GetRequiredService<IRepositoryFactory>();
    var formModuleData = provider.GetRequiredService<FormModuleData>();
    var logger = provider.GetRequiredService<ILogger<FormModuleBusiness>>();
    var mappingService = provider.GetRequiredService<IMappingService>();
    return new FormModuleBusiness(repositoryFactory, formModuleData, logger, mappingService);
});

// RolForm
builder.Services.AddScoped<IRolFormRepository, RolFormData>();
builder.Services.AddScoped<IGenericRepository<RolForm, int>, RolFormData>();
builder.Services.AddScoped<IGenericBusiness<RolFormDto, int>, AutoMapperRolFormBusiness>();
builder.Services.AddScoped<AutoMapperRolFormBusiness>();  // Registro explícito
builder.Services.AddScoped<RolFormBusiness>(provider => {
    var repositoryFactory = provider.GetRequiredService<IRepositoryFactory>();
    var logger = provider.GetRequiredService<ILogger<RolFormBusiness>>();
    var mappingService = provider.GetRequiredService<IMappingService>();
    return new RolFormBusiness(repositoryFactory, logger, mappingService);
});

// UserRol
builder.Services.AddScoped<IGenericRepository<UserRol, int>, UserRolData>();
builder.Services.AddScoped<UserRolData>(); // Necesario para métodos específicos
builder.Services.AddScoped<IGenericBusiness<UserRolDto, int>, AutoMapperUserRolBusiness>();
builder.Services.AddScoped<AutoMapperUserRolBusiness>();  // Registro explícito
builder.Services.AddScoped<UserRolBusiness>(provider => {
    var repositoryFactory = provider.GetRequiredService<IRepositoryFactory>();
    var userRolData = provider.GetRequiredService<UserRolData>();
    var logger = provider.GetRequiredService<ILogger<UserRolBusiness>>();
    var mappingService = provider.GetRequiredService<IMappingService>();
    return new UserRolBusiness(repositoryFactory, userRolData, logger, mappingService);
});

// Registrar clases de ChangeLog
builder.Services.AddScoped<ChangeLogData>();

try
{
    var app = builder.Build();

    // Configurar el path base para la aplicación - DEBE ir PRIMERO en el pipeline
    app.UsePathBase(new PathString(""));

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger(c => {
            c.RouteTemplate = "swagger/{documentName}/swagger.json";
        });
        
        app.UseSwaggerUI(c => {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "ModelSecurity API v1");
            c.RoutePrefix = "swagger";
        });
        
        // Imprimir las rutas de Swagger en la consola
       // Por estas
Console.WriteLine("Swagger disponible en:");
Console.WriteLine("https://localhost:7009/swagger");
Console.WriteLine("https://localhost:5188/swagger");
    }

    // Usar CORS antes de otros middlewares
    app.UseCors("AllowAll");
    
    app.UseHttpsRedirection();

    // Agregamos la autenticación antes de la autorización
    app.UseAuthentication();
    app.UseAuthorization();
    
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Error al iniciar la aplicación: {ex.Message}");
    throw;
}
