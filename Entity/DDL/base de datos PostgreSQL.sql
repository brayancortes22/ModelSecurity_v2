-- Script de creación de tablas para ModelSecurity (PostgreSQL)
-- Fecha: 2024-04-07

-- Creación de tablas base
CREATE TABLE Rol (
    Id SERIAL PRIMARY KEY,
    TypeRol VARCHAR(255) NOT NULL,
    Description TEXT,
    Active BOOLEAN NOT NULL
);

CREATE TABLE Verification (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Observation TEXT,
    Active BOOLEAN NOT NULL,
    CreateDate TIMESTAMP,
    DeleteDate TIMESTAMP,
    UpdateDate TIMESTAMP
);

CREATE TABLE Person (
    Id SERIAL PRIMARY KEY,
    Active BOOLEAN NOT NULL,
    Name VARCHAR(255) NOT NULL,
    FirstName VARCHAR(255),
    SecondName VARCHAR(255),
    FirstLastName VARCHAR(255),
    SecondLastName VARCHAR(255),
    PhoneNumber VARCHAR(50),
    Email VARCHAR(255) UNIQUE,
    TypeIdentification VARCHAR(50) NOT NULL,
    NumberIdentification INTEGER NOT NULL UNIQUE,
    Signig BOOLEAN NOT NULL,
    CreateDate TIMESTAMP NOT NULL,
    DeleteDate TIMESTAMP,
    UpdateDate TIMESTAMP
);

CREATE TABLE "User" (
    Id SERIAL PRIMARY KEY,
    Username VARCHAR(255) NOT NULL UNIQUE,
    Email VARCHAR(255) NOT NULL UNIQUE,
    Password VARCHAR(255) NOT NULL,
    PersonId INTEGER NOT NULL,
    Active BOOLEAN NOT NULL,
    FOREIGN KEY (PersonId) REFERENCES Person(Id)
);

CREATE TABLE UserRol (
    Id SERIAL PRIMARY KEY,
    UserId INTEGER NOT NULL,
    RolId INTEGER NOT NULL,
    FOREIGN KEY (UserId) REFERENCES "User"(Id),
    FOREIGN KEY (RolId) REFERENCES Rol(Id)
);

-- Creación de tablas de ubicación
CREATE TABLE Regional (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Address VARCHAR(255),
    Description TEXT,
    CodeRegional VARCHAR(100) UNIQUE NOT NULL,
    Active BOOLEAN NOT NULL
);

CREATE TABLE Center (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Address VARCHAR(255),
    CodeCenter VARCHAR(50) NOT NULL,
    Active BOOLEAN NOT NULL,
    CreateDate TIMESTAMP NOT NULL,
    DeleteDate TIMESTAMP,
    UpdateDate TIMESTAMP,
    RegionalId INTEGER NOT NULL,
    FOREIGN KEY (RegionalId) REFERENCES Regional(Id)
);

CREATE TABLE Sede (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    CodeSede VARCHAR(50) UNIQUE NOT NULL,
    Address VARCHAR(255),
    PhoneSede VARCHAR(50),
    EmailContact VARCHAR(255),
    Active BOOLEAN NOT NULL,
    CreateDate TIMESTAMP NOT NULL,
    DeleteDate TIMESTAMP,
    UpdateDate TIMESTAMP,
    CenterId INTEGER NOT NULL,
    FOREIGN KEY (CenterId) REFERENCES Center(Id)
);

CREATE TABLE UserSede (
    Id SERIAL PRIMARY KEY,
    StatusProcedure VARCHAR(255),
    UserId INTEGER NOT NULL,
    SedeId INTEGER NOT NULL,
    FOREIGN KEY (UserId) REFERENCES "User"(Id),
    FOREIGN KEY (SedeId) REFERENCES Sede(Id)
);

-- Creación de tablas de usuarios específicos
CREATE TABLE Aprendiz (
    Id SERIAL PRIMARY KEY,
    PreviousProgram VARCHAR(255),
    Active BOOLEAN NOT NULL,
    UserId INTEGER NOT NULL,
    FOREIGN KEY (UserId) REFERENCES "User"(Id)
);

CREATE TABLE Instructor (
    Id SERIAL PRIMARY KEY,
    Active BOOLEAN NOT NULL,
    UserId INTEGER NOT NULL,
    FOREIGN KEY (UserId) REFERENCES "User"(Id)
);

-- Creación de tablas de programas y procesos
CREATE TABLE Program (
    Id SERIAL PRIMARY KEY,
    CodeProgram NUMERIC NOT NULL UNIQUE,
    Name VARCHAR(255) NOT NULL,
    TypeProgram VARCHAR(255) NOT NULL,
    CreateDate TIMESTAMP NOT NULL,
    DeleteDate TIMESTAMP,
    UpdateDate TIMESTAMP,
    Active BOOLEAN NOT NULL,
    Description TEXT
);

CREATE TABLE Process (
    Id SERIAL PRIMARY KEY,
    StartAprendiz VARCHAR(255) NOT NULL,
    Observation TEXT,
    TypeProcess VARCHAR(255) NOT NULL,
    Active BOOLEAN NOT NULL,
    CreateDate TIMESTAMP NOT NULL,
    DeleteDate TIMESTAMP,
    UpdateDate TIMESTAMP
);

CREATE TABLE AprendizProgram (
    Id SERIAL PRIMARY KEY,
    AprendizId INTEGER NOT NULL,
    ProgramId INTEGER NOT NULL,
    FOREIGN KEY (AprendizId) REFERENCES Aprendiz(Id),
    FOREIGN KEY (ProgramId) REFERENCES Program(Id)
);

CREATE TABLE InstructorProgram (
    Id SERIAL PRIMARY KEY,
    InstructorId INTEGER NOT NULL,
    ProgramId INTEGER NOT NULL,
    FOREIGN KEY (InstructorId) REFERENCES Instructor(Id),
    FOREIGN KEY (ProgramId) REFERENCES Program(Id)
);

-- Creación de tablas de configuración
CREATE TABLE TypeModality (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Description TEXT,
    Active BOOLEAN NOT NULL
);

CREATE TABLE State (
    Id SERIAL PRIMARY KEY,
    Active BOOLEAN NOT NULL,
    CreateDate TIMESTAMP NOT NULL,
    DeleteDate TIMESTAMP,
    UpdateDate TIMESTAMP,
    Description VARCHAR(255),
    TypeState VARCHAR(255)
);

CREATE TABLE RegisterySofia (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Description TEXT,
    Document VARCHAR(255),
    Active BOOLEAN NOT NULL,
    CreateDate TIMESTAMP NOT NULL,
    DeleteDate TIMESTAMP,
    UpdateDate TIMESTAMP
);

CREATE TABLE Enterprise (
    Id SERIAL PRIMARY KEY,
    Observation TEXT,
    NameEnterprise VARCHAR(255) NOT NULL,
    PhoneEnterprise VARCHAR(50),
    Locate VARCHAR(255),
    NitEnterprise VARCHAR(50) NOT NULL UNIQUE,
    EmailEnterprise VARCHAR(255),
    Active BOOLEAN NOT NULL,
    CreateDate TIMESTAMP NOT NULL,
    DeleteDate TIMESTAMP,
    UpdateDate TIMESTAMP
);

CREATE TABLE Concept (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Observation VARCHAR(255),
    Active BOOLEAN NOT NULL,
    CreateDate TIMESTAMP NOT NULL,
    DeleteDate TIMESTAMP,
    UpdateDate TIMESTAMP
);

-- Creación de tablas de proceso-aprendiz-instructor
CREATE TABLE AprendizProcessInstructor (
    Id SERIAL PRIMARY KEY,
    TypeModalityId INTEGER NOT NULL,
    RegisterySofiaId INTEGER NOT NULL,
    ConceptId INTEGER NOT NULL,
    EnterpriseId INTEGER NOT NULL,
    ProcessId INTEGER NOT NULL,
    AprendizId INTEGER NOT NULL,
    InstructorId INTEGER NOT NULL,
    StateId INTEGER NOT NULL,
    VerificationId INTEGER NOT NULL,
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
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Description TEXT,
    Active BOOLEAN NOT NULL,
    CreateDate TIMESTAMP NOT NULL,
    DeleteDate TIMESTAMP,
    UpdateDate TIMESTAMP
);

CREATE TABLE Form (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Description TEXT,
    Cuestion TEXT,
    TypeCuestion VARCHAR(255),
    Answer TEXT,
    Active BOOLEAN NOT NULL,
    CreateDate TIMESTAMP NOT NULL,
    DeleteDate TIMESTAMP,
    UpdateDate TIMESTAMP
);

CREATE TABLE FormModule (
    Id SERIAL PRIMARY KEY,
    StatusProcedure VARCHAR(255) NOT NULL,
    FormId INTEGER NOT NULL,
    ModuleId INTEGER NOT NULL,
    FOREIGN KEY (FormId) REFERENCES Form(Id),
    FOREIGN KEY (ModuleId) REFERENCES Module(Id)
);

CREATE TABLE RolForm (
    Id SERIAL PRIMARY KEY,
    Permission VARCHAR(255),
    RolId INTEGER NOT NULL,
    FormId INTEGER NOT NULL,
    FOREIGN KEY (RolId) REFERENCES Rol(Id),
    FOREIGN KEY (FormId) REFERENCES Form(Id)
);

-- Creación de tabla de registro de cambios
CREATE TABLE ChangeLog (
    Id SERIAL PRIMARY KEY,
    TableName VARCHAR(255) NOT NULL,
    IdTable INTEGER NOT NULL,
    OldValues TEXT,
    NewValues TEXT,
    Action VARCHAR(50) NOT NULL,
    Active BOOLEAN NOT NULL,
    UserName VARCHAR(255) NOT NULL,
    CreateDate TIMESTAMP NOT NULL,
    UpdateDate TIMESTAMP
); 