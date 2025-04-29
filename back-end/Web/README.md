# Configuración de Base de Datos ModelSecurity

Este documento describe los pasos necesarios para configurar la base de datos y la conexión para el proyecto ModelSecurity.

## Requisitos Previos

- SQL Server instalado
- HeidiSQL instalado
- Visual Studio o Visual Studio Code
- .NET Core SDK

## 1. Configuración de SQL Server

### 1.1 Habilitar TCP/IP
1. Abrir SQL Server Configuration Manager
2. Ir a "Configuración de red de SQL Server"
3. Seleccionar "Protocolos de MSSQLSERVER"
4. Habilitar TCP/IP
5. En la pestaña "Direcciones IP", asegurarse que el puerto TCP sea 1433

### 1.2 Configurar el Firewall
Ejecutar en PowerShell como administrador:
```powershell
New-NetFirewallRule -DisplayName "SQL Server" -Direction Inbound -Protocol TCP -LocalPort 1433 -Action Allow
New-NetFirewallRule -DisplayName "SQL Server" -Direction Outbound -Protocol TCP -LocalPort 1433 -Action Allow
```

## 2. Crear la Base de Datos y Usuario

### 2.1 Conectar a SQL Server con HeidiSQL
1. Abrir HeidiSQL
2. Crear nueva sesión:
   - Tipo de red: Microsoft SQL Server (TCP/IP)
   - Hostname: localhost
   - Puerto: 1433
   - Usuario: sa o usar autenticación de Windows

### 2.2 Crear Usuario y Base de Datos
Ejecutar los siguientes comandos SQL:
```sql
-- Crear el login
CREATE LOGIN ModelSecurityUser 
WITH PASSWORD = '123456';

-- Crear la base de datos
CREATE DATABASE ModelSecurity;

-- Usar la base de datos
USE ModelSecurity;

-- Crear el usuario y asignar permisos
CREATE USER ModelSecurityUser FOR LOGIN ModelSecurityUser;
ALTER ROLE db_owner ADD MEMBER ModelSecurityUser;
```

### 2.3 Verificar la Configuración
```sql
-- Verificar que el usuario existe
SELECT name, type_desc 
FROM sys.database_principals 
WHERE name = 'ModelSecurityUser';

-- Verificar sus permisos
SELECT r.name as RoleName
FROM sys.database_principals r
INNER JOIN sys.database_role_members m ON r.principal_id = m.role_principal_id
INNER JOIN sys.database_principals u ON m.member_principal_id = u.principal_id
WHERE u.name = 'ModelSecurityUser';
```

## 3. Configuración del Proyecto

### 3.1 Configurar appsettings.json
```json
{
    "Logging": {
        "LogLevel": {
            "Default": "Information",
            "Microsoft.AspNetCore": "Warning"
        }
    },
    "AllowedHosts": "*",
    "OrigenesPermitidos": "http://localhost:4200,http://localhost:3000",
    "ConnectionStrings": {
        "DefaultConnection": "Server=localhost;Database=ModelSecurity;User=ModelSecurityUser;Password=123456;TrustServerCertificate=True;MultipleActiveResultSets=true"
    }
}
```

### 3.2 Configurar CORS en Program.cs
```csharp
var origenesPermitidos = builder.Configuration.GetValue<string>("OrigenesPermitidos")?.Split(',') 
    ?? new[] { "http://localhost:4200" };

builder.Services.AddCors(opciones =>
{
    opciones.AddDefaultPolicy(politica =>
    {
        politica.WithOrigins(origenesPermitidos)
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});
```

## 4. Verificación

Para verificar que todo está configurado correctamente:

1. La base de datos debe aparecer en HeidiSQL
2. El usuario ModelSecurityUser debe tener el rol db_owner
3. La aplicación debe poder conectarse sin errores
4. Las peticiones CORS desde el frontend deben funcionar correctamente

## Solución de Problemas

Si hay problemas de conexión, verificar:
1. Que SQL Server está ejecutándose
2. Que TCP/IP está habilitado
3. Que el puerto 1433 está abierto
4. Que el usuario tiene los permisos correctos
5. Que la cadena de conexión es correcta

## Notas de Seguridad

En producción:
1. Usar contraseñas seguras
2. Limitar los permisos del usuario según sea necesario
3. Configurar correctamente los orígenes permitidos en CORS
4. Considerar el uso de variables de entorno para datos sensibles