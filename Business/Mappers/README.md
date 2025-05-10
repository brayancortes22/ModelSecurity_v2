# Documentación del Sistema de Mapeo Automático

## Introducción

El sistema de mapeo automático utiliza AutoMapper para facilitar la conversión entre entidades de dominio y DTOs (Data Transfer Objects), eliminando la necesidad de escribir código de mapeo manual en cada clase de negocio.

## Estructura

### Interfaz y Servicio de Mapeo

- **IMappingService**: Define los métodos para el mapeo de objetos.
- **MappingService**: Implementa la interfaz y utiliza AutoMapper para realizar el mapeo.

### Perfiles de Mapeo

Los perfiles de mapeo definen cómo se realiza la conversión entre entidades y DTOs:

- **BaseMapperProfile**: Clase base para todos los perfiles de mapeo.
- **FormProfile**: Perfil para la entidad Form.
- **PersonProfile**: Perfil para la entidad Person.
- **ModuleProfile**: Perfil para la entidad Module.
- **RolProfile**: Perfil para la entidad Rol.
- **RolFormProfile**: Perfil para la entidad RolForm.
- **UserProfile**: Perfil para la entidad User.
- **UserRolProfile**: Perfil para la entidad UserRol.
- **FormModuleProfile**: Perfil para la entidad FormModule.
- **ChangeLogProfile**: Perfil para la entidad ChangeLog.

### Clase Base de Negocio

- **AutoMapperGenericBusiness**: Versión de GenericBusiness que utiliza AutoMapper para realizar el mapeo de objetos.

## Cómo Usar

### 1. Crear un Perfil de Mapeo

```csharp
public class MyEntityProfile : BaseMapperProfile
{
    public MyEntityProfile()
    {
        CreateMap<MyEntity, MyEntityDto>();
        
        CreateMap<MyEntityDto, MyEntity>()
            .ForMember(dest => dest.Id, opt => opt.Condition(src => src.Id > 0))
            .ForMember(dest => dest.CreateDate, opt => opt.Ignore())
            .ForMember(dest => dest.UpdateDate, opt => opt.Ignore())
            .ForMember(dest => dest.DeleteDate, opt => opt.Ignore());
    }
}
```

### 2. Crear una Clase de Negocio

```csharp
public class MyEntityBusiness : AutoMapperGenericBusiness<MyEntity, MyEntityDto, int>, IGenericBusiness<MyEntityDto, int>
{
    public MyEntityBusiness(
        IRepositoryFactory repositoryFactory, 
        ILogger<MyEntityBusiness> logger,
        IMappingService mappingService)
        : base(repositoryFactory, logger, mappingService)
    {
    }

    protected override void ValidateBeforeSave(MyEntityDto dto)
    {
        base.ValidateBeforeSave(dto);
        
        // Aquí se agregan validaciones específicas
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            throw new Utilities.exeptions.BusinessExceptio("El nombre es obligatorio");
        }
    }
}
```

### 3. Registrar en el Contenedor de Servicios

```csharp
// Registrar AutoMapper
builder.Services.AddAutoMapper(typeof(Business.Mappers.BaseMapperProfile).Assembly);

// Registrar servicio de mapeo
builder.Services.AddScoped<Business.Mappers.IMappingService, Business.Mappers.MappingService>();

// Registrar las clases de negocio con AutoMapper
builder.Services.AddScoped<AutoMapperFormBusiness>();
builder.Services.AddScoped<AutoMapperModuleBusiness>();
builder.Services.AddScoped<AutoMapperPersonBusiness>();

// Para usar la implementación como el servicio principal para esa entidad:
builder.Services.AddScoped<IGenericBusiness<ModuleDto, int>, AutoMapperModuleBusiness>();
```

### 4. Usar el Servicio Directamente

```csharp
public class SomeService
{
    private readonly IMappingService _mappingService;
    
    public SomeService(IMappingService mappingService)
    {
        _mappingService = mappingService;
    }
    
    public void SomeMethod()
    {
        var entity = GetEntityFromSomewhere();
        var dto = _mappingService.Map<Entity, EntityDto>(entity);
        
        // Hacer algo con el DTO
        
        var updatedEntity = _mappingService.Map<EntityDto, Entity>(dto);
    }
}
```

## Ventajas del Sistema

1. **Reduce el código repetitivo**: No es necesario escribir métodos de mapeo manual en cada clase.
2. **Centraliza la lógica de mapeo**: Los mapeos están definidos en perfiles dedicados.
3. **Facilita mantenimiento**: Al modificar una entidad o DTO, solo se necesita actualizar el perfil correspondiente.
4. **Mejora la legibilidad**: Las clases de negocio son más limpias y enfocadas en la lógica de negocio.
5. **Mejora la coherencia**: Asegura que el mapeo se realice de manera consistente en toda la aplicación.

## Configuraciones Avanzadas

AutoMapper ofrece muchas opciones avanzadas que pueden ser utilizadas en los perfiles:

- **Mapeo condicional**: `.ForMember(dest => dest.Prop, opt => opt.Condition(src => /* condición */))`
- **Conversión de tipos**: `.ForMember(dest => dest.Prop, opt => opt.ConvertUsing(/* conversor */))`
- **Mapeo personalizado**: `.ForMember(dest => dest.Prop, opt => opt.MapFrom(src => /* expresión */))`
- **Ignorar propiedades**: `.ForMember(dest => dest.Prop, opt => opt.Ignore())`

## Reglas Comunes de Mapeo

En la mayoría de los perfiles se aplican las siguientes reglas:

1. **Propiedades de auditoría**: Las propiedades como `CreateDate`, `UpdateDate` y `DeleteDate` se ignoran al mapear de DTO a entidad.
2. **ID condicional**: El ID solo se mapea si es mayor que 0 para evitar sobrescribir IDs existentes.
3. **Propiedades de navegación**: Las propiedades de navegación se ignoran al mapear de DTO a entidad para evitar referencias circulares.

## Ejemplos Implementados

1. **AutoMapperFormBusiness**: Implementación para la entidad Form
2. **AutoMapperModuleBusiness**: Implementación para la entidad Module
3. **AutoMapperPersonBusiness**: Implementación para la entidad Person
4. **AutoMapperRolBusiness**: Implementación para la entidad Rol
5. **AutoMapperUserBusiness**: Implementación para la entidad User

## Conclusión

El sistema de mapeo automático con AutoMapper proporciona una forma eficiente y mantenible de gestionar la conversión entre entidades y DTOs en la aplicación ModelSecurity_v2. Reduce significativamente la cantidad de código repetitivo y mejora la coherencia del mapeo en toda la aplicación.

## Migración desde el Sistema Manual

Para migrar una clase existente desde `GenericBusiness` a `AutoMapperGenericBusiness`:

1. Cambiar la clase base a `AutoMapperGenericBusiness<TEntity, TDto, TId>`
2. Añadir el parámetro `IMappingService` al constructor
3. Eliminar los métodos `MapToDto`, `MapToEntity`, `MapToDtoList` y `UpdateEntityFromDto`
4. Mantener los métodos `ValidateId`, `ValidateDto` y `PatchEntityFromDto`
5. Crear un perfil de mapeo para la entidad y DTO
