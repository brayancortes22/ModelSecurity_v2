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
builder.Services.AddSingleton<IRepositoryFactory, RepositoryFactory>();
builder.Services.AddSingleton<IBusinessFactory, BusinessFactory>();
builder.Services.AddSingleton<IActivacionDataFactory, ActivacionDataFactory>();

// Registrar implementaciones genéricas
// Form
builder.Services.AddScoped<IGenericRepository<Form, int>, FormData>();
// Usando la clase existente que ahora usa AutoMapper internamente
builder.Services.AddScoped<IGenericBusiness<FormDto, int>, FormBusiness>();
// Registrar nuestras versiones con AutoMapper
builder.Services.AddScoped<AutoMapperFormBusiness>();
builder.Services.AddScoped<AutoMapperModuleBusiness>();
builder.Services.AddScoped<AutoMapperPersonBusiness>();
builder.Services.AddScoped<AutoMapperRolBusiness>();
builder.Services.AddScoped<AutoMapperUserBusiness>();

// Module (usando alias para evitar ambigüedad)
builder.Services.AddScoped<IGenericRepository<ModuleEntity, int>, ModuleData>();
builder.Services.AddScoped<IGenericBusiness<ModuleDto, int>, ModuleBusiness>();
// Versión alternativa con AutoMapper - comentada, descomentar cuando quieras usar esta versión
// builder.Services.AddScoped<IGenericBusiness<ModuleDto, int>, AutoMapperModuleBusiness>();

// Person
builder.Services.AddScoped<IGenericRepository<Person, int>, PersonData>();
builder.Services.AddScoped<IGenericBusiness<PersonDto, int>, PersonBusiness>();
// Versión alternativa con AutoMapper - comentada, descomentar cuando quieras usar esta versión
// builder.Services.AddScoped<IGenericBusiness<PersonDto, int>, AutoMapperPersonBusiness>();

// Rol
builder.Services.AddScoped<IGenericRepository<Rol, int>, RolData>();
builder.Services.AddScoped<IGenericBusiness<RolDto, int>, RolBusiness>();
builder.Services.AddScoped<RolBusiness>(); // También necesitamos registrar la implementación concreta para métodos específicos
// Versión alternativa con AutoMapper - comentada, descomentar cuando quieras usar esta versión
// builder.Services.AddScoped<IGenericBusiness<RolDto, int>, AutoMapperRolBusiness>();

// User
builder.Services.AddScoped<IGenericRepository<User, int>, UserData>();
builder.Services.AddScoped<IGenericBusiness<UserDto, int>, UserBusiness>();
builder.Services.AddScoped<UserData>(); // Registrar también la implementación concreta para métodos específicos
builder.Services.AddScoped<UserBusiness>(); // Registrar también la implementación concreta para métodos específicos
// Versión alternativa con AutoMapper - comentada, descomentar cuando quieras usar esta versión
// builder.Services.AddScoped<IGenericBusiness<UserDto, int>, AutoMapperUserBusiness>();

// A medida que vayas refactorizando otros servicios, podrás registrarlos de manera similar:
// Ejemplo para futuras entidades:
// builder.Services.AddScoped<IGenericRepository<OtraEntidad, int>, OtraEntidadData>();
// builder.Services.AddScoped<IGenericBusiness<OtraEntidadDto, int>, OtraEntidadBusiness>();

// Las siguientes clases aún no han sido refactorizadas, así que mantienen su registro original
// Registrar clases de ChangeLog
builder.Services.AddScoped<ChangeLogData>();

// Registrar clases de FormModule
builder.Services.AddScoped<IGenericRepository<FormModule, int>, FormModuleData>();
builder.Services.AddScoped<IGenericBusiness<FormModuleDto, int>, FormModuleBusiness>();
builder.Services.AddScoped<FormModuleData>(); // Para métodos específicos
builder.Services.AddScoped<FormModuleBusiness>(); // Para métodos específicos

// Registrar clases de RolForm
builder.Services.AddScoped<IRolFormRepository, RolFormData>();
builder.Services.AddScoped<IGenericRepository<RolForm, int>, RolFormData>();
builder.Services.AddScoped<IGenericBusiness<RolFormDto, int>, RolFormBusiness>();
builder.Services.AddScoped<RolFormBusiness>(); // Para métodos específicos

// Registrar clases de UserRol - Actualizado para usar patrón genérico
builder.Services.AddScoped<IGenericRepository<UserRol, int>, UserRolData>();
builder.Services.AddScoped<IGenericBusiness<UserRolDto, int>, UserRolBusiness>();
builder.Services.AddScoped<UserRolData>(); // Para métodos específicos
builder.Services.AddScoped<UserRolBusiness>();

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
