# Documentación de Scripts DDL

Este directorio contiene los scripts de creación de tablas (DDL - Data Definition Language) para diferentes motores de base de datos. Cada script está optimizado para su respectivo motor de base de datos.

## Estructura de Archivos

- `base de datos sql server.sql`: Script para Microsoft SQL Server
- `base de datos PostgreSQL.sql`: Script para PostgreSQL
- `base de datos MySQL.sql`: Script para MySQL

## Diferencias entre Motores de Base de Datos

### Auto-incremento de IDs

| Motor de Base de Datos | Sintaxis | Ejemplo |
|-----------------------|----------|---------|
| SQL Server | `IDENTITY` | `Id INT IDENTITY PRIMARY KEY` |
| PostgreSQL | `SERIAL` | `Id SERIAL PRIMARY KEY` |
| MySQL | `AUTO_INCREMENT` | `Id INT AUTO_INCREMENT PRIMARY KEY` |

### Tipos de Datos

#### Texto Largo
| Motor de Base de Datos | Tipo de Dato | Ejemplo |
|-----------------------|--------------|---------|
| SQL Server | `VARCHAR(MAX)` | `Description VARCHAR(MAX)` |
| PostgreSQL | `TEXT` | `Description TEXT` |
| MySQL | `TEXT` | `Description TEXT` |

#### Booleanos
| Motor de Base de Datos | Tipo de Dato | Ejemplo |
|-----------------------|--------------|---------|
| SQL Server | `BIT` | `Active BIT NOT NULL` |
| PostgreSQL | `BOOLEAN` | `Active BOOLEAN NOT NULL` |
| MySQL | `BOOLEAN` | `Active BOOLEAN NOT NULL` |

#### Fechas
| Motor de Base de Datos | Tipo de Dato | Ejemplo |
|-----------------------|--------------|---------|
| SQL Server | `DATETIME` | `CreateDate DATETIME NOT NULL` |
| PostgreSQL | `TIMESTAMP` | `CreateDate TIMESTAMP NOT NULL` |
| MySQL | `DATETIME` | `CreateDate DATETIME NOT NULL` |

### Nombres Reservados

| Motor de Base de Datos | Sintaxis | Ejemplo |
|-----------------------|----------|---------|
| SQL Server | Sin comillas especiales | `User` |
| PostgreSQL | Comillas dobles | `"User"` |
| MySQL | Backticks | `` `User` `` |

## Estructura de las Tablas

### Tablas Base
1. **Rol**: Almacena los roles del sistema
2. **Verification**: Registros de verificaciones
3. **Person**: Información personal de usuarios
4. **User**: Credenciales y datos de acceso
5. **UserRol**: Relación entre usuarios y roles

### Tablas de Ubicación
1. **Regional**: Información de regionales
2. **Center**: Centros de formación
3. **Sede**: Sedes educativas
4. **UserSede**: Relación entre usuarios y sedes

### Tablas de Usuarios Específicos
1. **Aprendiz**: Datos específicos de aprendices
2. **Instructor**: Datos específicos de instructores

### Tablas de Programas y Procesos
1. **Program**: Programas educativos
2. **Process**: Procesos formativos
3. **AprendizProgram**: Relación aprendices-programas
4. **InstructorProgram**: Relación instructores-programas

### Tablas de Configuración
1. **TypeModality**: Tipos de modalidad
2. **State**: Estados del sistema
3. **RegisterySofia**: Registros Sofia
4. **Enterprise**: Empresas
5. **Concept**: Conceptos del sistema

### Tablas de Proceso-Aprendiz-Instructor
1. **AprendizProcessInstructor**: Relación compleja entre aprendices, procesos e instructores

### Tablas de Módulos y Formularios
1. **Module**: Módulos del sistema
2. **Form**: Formularios
3. **FormModule**: Relación formularios-módulos
4. **RolForm**: Relación roles-formularios

### Tabla de Registro de Cambios
1. **ChangeLog**: Registro de cambios en el sistema

## Relaciones entre Tablas

### Relaciones Uno a Uno (1:1)
- User - Person
- Aprendiz - User
- Instructor - User

### Relaciones Uno a Muchos (1:N)
- Center - Sede
- Regional - Center
- Center - Regional

### Relaciones Muchos a Muchos (N:N)
- User - Rol (a través de UserRol)
- User - Sede (a través de UserSede)
- Aprendiz - Program (a través de AprendizProgram)
- Instructor - Program (a través de InstructorProgram)
- Form - Module (a través de FormModule)
- Rol - Form (a través de RolForm)

## Consideraciones de Implementación

1. **Orden de Creación**: Las tablas deben crearse en el orden correcto para respetar las restricciones de clave foránea.
2. **Índices**: Considerar la creación de índices para campos frecuentemente consultados.
3. **Restricciones**: Todas las tablas incluyen campos de auditoría (CreateDate, UpdateDate, DeleteDate).
4. **Soft Delete**: Implementado a través del campo `Active` en todas las tablas relevantes.

## Ejecución de Scripts

### SQL Server
```sql
USE [NombreBaseDatos];
GO
-- Ejecutar el contenido de base de datos sql server.sql
```

### PostgreSQL
```sql
\c nombre_base_datos
-- Ejecutar el contenido de base de datos PostgreSQL.sql
```

### MySQL
```sql
USE nombre_base_datos;
-- Ejecutar el contenido de base de datos MySQL.sql
```

## Mantenimiento

1. Realizar copias de seguridad antes de ejecutar los scripts
2. Verificar la compatibilidad con la versión específica del motor de base de datos
3. Revisar los logs de ejecución para detectar posibles errores
4. Validar las restricciones y relaciones después de la ejecución 