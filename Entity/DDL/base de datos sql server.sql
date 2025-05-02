-- Script de creación de tablas para ModelSecurity - Modelo de Seguridad
-- Fecha: 2024-04-28

-- Creación de tablas base
CREATE TABLE Rol (
    Id INT PRIMARY KEY IDENTITY,
    TypeRol VARCHAR(255) NOT NULL,
    Description VARCHAR(MAX),
    Active BIT NOT NULL
);

CREATE TABLE Person (
    Id INT PRIMARY KEY IDENTITY,
    Active BIT NOT NULL,
    Name VARCHAR(255) NOT NULL,
    FirstName VARCHAR(255),
    SecondName VARCHAR(255),
    FirstLastName VARCHAR(255),
    SecondLastName VARCHAR(255),
    PhoneNumber VARCHAR(50),
    Email NVARCHAR(255) UNIQUE,
    TypeIdentification VARCHAR(50) NOT NULL,
    NumberIdentification INT NOT NULL UNIQUE,
    Signing VARCHAR(255),
    CreateDate DATETIME NOT NULL,
    DeleteDate DATETIME,
    UpdateDate DATETIME
);

CREATE TABLE [User] (
    Id INT IDENTITY PRIMARY KEY,
    Username VARCHAR(255) NOT NULL UNIQUE,
    Email VARCHAR(255) NOT NULL UNIQUE,
    Password VARCHAR(255) NOT NULL,
    PersonId INT NOT NULL,
    Active BIT NOT NULL,
    FOREIGN KEY (PersonId) REFERENCES Person(Id)
);

CREATE TABLE UserRol (
    Id INT IDENTITY PRIMARY KEY,
    UserId INT NOT NULL,
    RolId INT NOT NULL,
    Active BIT NOT NULL DEFAULT 1,
    CreateDate DATETIME NOT NULL DEFAULT GETDATE(),
    DeleteDate DATETIME NULL,
    UpdateDate DATETIME NULL,
    FOREIGN KEY (UserId) REFERENCES [User](Id),
    FOREIGN KEY (RolId) REFERENCES Rol(Id)
);

-- Creación de tablas de módulos y formularios
CREATE TABLE Module (
    Id INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(255) NOT NULL,
    Description VARCHAR(MAX),
    Active BIT NOT NULL,
    CreateDate DATETIME NOT NULL,
    DeleteDate DATETIME,
    UpdateDate DATETIME
);

CREATE TABLE Form (
    Id INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(255) NOT NULL,
    Description VARCHAR(MAX),
    Cuestion VARCHAR(MAX),
    TypeCuestion VARCHAR(255),
    Answer VARCHAR(MAX),
    [Route] VARCHAR(255),
    Active BIT NOT NULL,
    CreateDate DATETIME NOT NULL,
    DeleteDate DATETIME NULL,
    UpdateDate DATETIME NULL
);

CREATE TABLE FormModule (
    Id INT PRIMARY KEY IDENTITY,
    StatusProcedure VARCHAR(255) NOT NULL,
    FormId INT NOT NULL,
    ModuleId INT NOT NULL,
    FOREIGN KEY (FormId) REFERENCES Form(Id),
    FOREIGN KEY (ModuleId) REFERENCES Module(Id)
);

CREATE TABLE RolForm (
    Id INT IDENTITY PRIMARY KEY,
    Permission VARCHAR(255),
    RolId INT NOT NULL,
    FormId INT NOT NULL,
    FOREIGN KEY (RolId) REFERENCES Rol(Id),
    FOREIGN KEY (FormId) REFERENCES Form(Id)
);

-- Creación de tabla de registro de cambios
CREATE TABLE ChangeLog (
    Id INT IDENTITY PRIMARY KEY,
    TableName VARCHAR(255) NOT NULL,
    IdTable INT NOT NULL,
    OldValues VARCHAR(MAX),
    NewValues VARCHAR(MAX),
    Action VARCHAR(50) NOT NULL,
    Active BIT NOT NULL,
    UserName VARCHAR(255) NOT NULL,
    CreateDate DATETIME NOT NULL,
    UpdateDate DATETIME
);