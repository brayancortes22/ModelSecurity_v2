-- Script de creación de tablas para ModelSecurity
-- Fecha: 2024-04-07

-- Creación de tablas base
CREATE TABLE Rol (
    Id INT PRIMARY KEY IDENTITY,
    TypeRol VARCHAR(255) NOT NULL,
    Description VARCHAR(MAX),
    Active BIT NOT NULL
);

CREATE TABLE Verification (
    Id INT IDENTITY PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Observation VARCHAR(MAX),
    Active BIT NOT NULL,
    CreateDate DATETIME,
    DeleteDate DATETIME,
    UpdateDate DATETIME
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
    Signig BIT NOT NULL,
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
    FOREIGN KEY (UserId) REFERENCES [User](Id),
    FOREIGN KEY (RolId) REFERENCES Rol(Id)
);

-- Creación de tablas de ubicación
CREATE TABLE Regional (
    Id INT PRIMARY KEY IDENTITY,
    Name VARCHAR(255) NOT NULL,
    Address VARCHAR(255),
    Description VARCHAR(MAX),
    CodeRegional VARCHAR(100) UNIQUE NOT NULL,
    Active BIT NOT NULL
);

CREATE TABLE Center (
    Id INT PRIMARY KEY IDENTITY,
    Name VARCHAR(255) NOT NULL,
    Address VARCHAR(255),
    CodeCenter VARCHAR(50) NOT NULL,
    Active BIT NOT NULL,
    CreateDate DATETIME NOT NULL,
    DeleteDate DATETIME,
    UpdateDate DATETIME,
    RegionalId INT NOT NULL,
    FOREIGN KEY (RegionalId) REFERENCES Regional(Id)
);

CREATE TABLE Sede (
    Id INT IDENTITY PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    CodeSede VARCHAR(50) UNIQUE NOT NULL,
    Address VARCHAR(255),
    PhoneSede VARCHAR(50),
    EmailContact VARCHAR(255),
    Active BIT NOT NULL,
    CreateDate DATETIME NOT NULL,
    DeleteDate DATETIME,
    UpdateDate DATETIME,
    CenterId INT NOT NULL,
    FOREIGN KEY (CenterId) REFERENCES Center(Id)
);

CREATE TABLE UserSede (
    Id INT IDENTITY PRIMARY KEY,
    StatusProcedure VARCHAR(255),
    UserId INT NOT NULL,
    SedeId INT NOT NULL,
    FOREIGN KEY (UserId) REFERENCES [User](Id),
    FOREIGN KEY (SedeId) REFERENCES Sede(Id)
);

-- Creación de tablas de usuarios específicos
CREATE TABLE Aprendiz (
    Id INT PRIMARY KEY IDENTITY,
    PreviousProgram VARCHAR(255),
    Active BIT NOT NULL,
    UserId INT NOT NULL,
    FOREIGN KEY (UserId) REFERENCES [User](Id)
);

CREATE TABLE Instructor (
    Id INT PRIMARY KEY IDENTITY,
    Active BIT NOT NULL,
    UserId INT NOT NULL,
    FOREIGN KEY (UserId) REFERENCES [User](Id)
);

-- Creación de tablas de programas y procesos
CREATE TABLE Program (
    Id INT PRIMARY KEY IDENTITY,
    CodeProgram DECIMAL NOT NULL UNIQUE,
    Name VARCHAR(255) NOT NULL,
    TypeProgram VARCHAR(255) NOT NULL,
    CreateDate DATETIME NOT NULL,
    DeleteDate DATETIME,
    UpdateDate DATETIME,
    Active BIT NOT NULL,
    Description VARCHAR(MAX)
);

CREATE TABLE Process (
    Id INT PRIMARY KEY IDENTITY,
    StartAprendiz VARCHAR(255) NOT NULL,
    Observation VARCHAR(MAX),
    TypeProcess VARCHAR(255) NOT NULL,
    Active BIT NOT NULL,
    CreateDate DATETIME NOT NULL,
    DeleteDate DATETIME,
    UpdateDate DATETIME
);

CREATE TABLE AprendizProgram (
    Id INT PRIMARY KEY IDENTITY,
    AprendizId INT NOT NULL,
    ProgramId INT NOT NULL,
    FOREIGN KEY (AprendizId) REFERENCES Aprendiz(Id),
    FOREIGN KEY (ProgramId) REFERENCES Program(Id)
);

CREATE TABLE InstructorProgram (
    Id INT PRIMARY KEY IDENTITY,
    InstructorId INT NOT NULL,
    ProgramId INT NOT NULL,
    FOREIGN KEY (InstructorId) REFERENCES Instructor(Id),
    FOREIGN KEY (ProgramId) REFERENCES Program(Id)
);

-- Creación de tablas de configuración
CREATE TABLE TypeModality (
    Id INT IDENTITY PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Description VARCHAR(MAX),
    Active BIT NOT NULL
);

CREATE TABLE State (
    Id INT IDENTITY PRIMARY KEY,
    Active BIT NOT NULL,
    CreateDate DATETIME NOT NULL,
    DeleteDate DATETIME,
    UpdateDate DATETIME,
    Description NVARCHAR(255),
    TypeState NVARCHAR(255)
);

CREATE TABLE RegisterySofia (
    Id INT PRIMARY KEY IDENTITY,
    Name VARCHAR(255) NOT NULL,
    Description VARCHAR(MAX),
    Document VARCHAR(255),
    Active BIT NOT NULL,
    CreateDate DATETIME NOT NULL,
    DeleteDate DATETIME,
    UpdateDate DATETIME
);

CREATE TABLE Enterprise (
    Id INT PRIMARY KEY IDENTITY,
    Observation VARCHAR(MAX),
    NameEnterprise VARCHAR(255) NOT NULL,
    PhoneEnterprise VARCHAR(50),
    Locate VARCHAR(255),
    NitEnterprise VARCHAR(50) NOT NULL UNIQUE,
    EmailEnterprise VARCHAR(255),
    Active BIT NOT NULL,
    CreateDate DATETIME NOT NULL,
    DeleteDate DATETIME NULL,
    UpdateDate DATETIME NULL
);

CREATE TABLE Concept (
    Id INT PRIMARY KEY IDENTITY,
    Name VARCHAR(255) NOT NULL,
    Observation VARCHAR(255),
    Active BIT NOT NULL,
    CreateDate DATETIME NOT NULL,
    DeleteDate DATETIME,
    UpdateDate DATETIME
);

-- Creación de tablas de proceso-aprendiz-instructor
CREATE TABLE AprendizProcessInstructor (
    Id INT PRIMARY KEY IDENTITY,
    TypeModalityId INT NOT NULL,
    RegisterySofiaId INT NOT NULL,
    ConceptId INT NOT NULL,
    EnterpriseId INT NOT NULL,
    ProcessId INT NOT NULL,
    AprendizId INT NOT NULL,
    InstructorId INT NOT NULL,
    StateId INT NOT NULL,
    VerificationId INT NOT NULL,
    FOREIGN KEY (TypeModalityId) REFERENCES TypeModality(Id),
    FOREIGN KEY (RegisterySofiaId) REFERENCES RegisterySofia(Id),
    FOREIGN KEY (ConceptId) REFERENCES Concept(Id),
    FOREIGN KEY (EnterpriseId) REFERENCES Enterprise(Id),
    FOREIGN KEY (ProcessId) REFERENCES Process(Id),
    FOREIGN KEY (AprendizId) REFERENCES Aprendiz(Id),
    FOREIGN KEY (InstructorId) REFERENCES Instructor(Id),
    FOREIGN KEY (StateId) REFERENCES State(Id),
    FOREIGN KEY (VerificationId) REFERENCES Verification(Id)
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