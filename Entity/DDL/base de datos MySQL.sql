-- Script de creación de tablas para ModelSecurity (MySQL)
-- Fecha: 2024-04-07

-- Creación de tablas base
CREATE TABLE Rol (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    TypeRol VARCHAR(255) NOT NULL,
    Description TEXT,
    Active BOOLEAN NOT NULL
);

CREATE TABLE Verification (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Observation TEXT,
    Active BOOLEAN NOT NULL,
    CreateDate DATETIME,
    DeleteDate DATETIME,
    UpdateDate DATETIME
);

CREATE TABLE Person (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Active BOOLEAN NOT NULL,
    Name VARCHAR(255) NOT NULL,
    FirstName VARCHAR(255),
    SecondName VARCHAR(255),
    FirstLastName VARCHAR(255),
    SecondLastName VARCHAR(255),
    PhoneNumber VARCHAR(50),
    Email VARCHAR(255) UNIQUE,
    TypeIdentification VARCHAR(50) NOT NULL,
    NumberIdentification INT NOT NULL UNIQUE,
    Signig BOOLEAN NOT NULL,
    CreateDate DATETIME NOT NULL,
    DeleteDate DATETIME,
    UpdateDate DATETIME
);

CREATE TABLE `User` (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Username VARCHAR(255) NOT NULL UNIQUE,
    Email VARCHAR(255) NOT NULL UNIQUE,
    Password VARCHAR(255) NOT NULL,
    PersonId INT NOT NULL,
    Active BOOLEAN NOT NULL,
    FOREIGN KEY (PersonId) REFERENCES Person(Id)
);

CREATE TABLE UserRol (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    RolId INT NOT NULL,
    FOREIGN KEY (UserId) REFERENCES `User`(Id),
    FOREIGN KEY (RolId) REFERENCES Rol(Id)
);

-- Creación de tablas de ubicación
CREATE TABLE Regional (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Address VARCHAR(255),
    Description TEXT,
    CodeRegional VARCHAR(100) UNIQUE NOT NULL,
    Active BOOLEAN NOT NULL
);

CREATE TABLE Center (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Address VARCHAR(255),
    CodeCenter VARCHAR(50) NOT NULL,
    Active BOOLEAN NOT NULL,
    CreateDate DATETIME NOT NULL,
    DeleteDate DATETIME,
    UpdateDate DATETIME,
    RegionalId INT NOT NULL,
    FOREIGN KEY (RegionalId) REFERENCES Regional(Id)
);

CREATE TABLE Sede (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    CodeSede VARCHAR(50) UNIQUE NOT NULL,
    Address VARCHAR(255),
    PhoneSede VARCHAR(50),
    EmailContact VARCHAR(255),
    Active BOOLEAN NOT NULL,
    CreateDate DATETIME NOT NULL,
    DeleteDate DATETIME,
    UpdateDate DATETIME,
    CenterId INT NOT NULL,
    FOREIGN KEY (CenterId) REFERENCES Center(Id)
);

CREATE TABLE UserSede (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    StatusProcedure VARCHAR(255),
    UserId INT NOT NULL,
    SedeId INT NOT NULL,
    FOREIGN KEY (UserId) REFERENCES `User`(Id),
    FOREIGN KEY (SedeId) REFERENCES Sede(Id)
);

-- Creación de tablas de usuarios específicos
CREATE TABLE Aprendiz (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    PreviousProgram VARCHAR(255),
    Active BOOLEAN NOT NULL,
    UserId INT NOT NULL,
    FOREIGN KEY (UserId) REFERENCES `User`(Id)
);

CREATE TABLE Instructor (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Active BOOLEAN NOT NULL,
    UserId INT NOT NULL,
    FOREIGN KEY (UserId) REFERENCES `User`(Id)
);

-- Creación de tablas de programas y procesos
CREATE TABLE Program (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    CodeProgram DECIMAL NOT NULL UNIQUE,
    Name VARCHAR(255) NOT NULL,
    TypeProgram VARCHAR(255) NOT NULL,
    CreateDate DATETIME NOT NULL,
    DeleteDate DATETIME,
    UpdateDate DATETIME,
    Active BOOLEAN NOT NULL,
    Description TEXT
);

CREATE TABLE Process (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    StartAprendiz VARCHAR(255) NOT NULL,
    Observation TEXT,
    TypeProcess VARCHAR(255) NOT NULL,
    Active BOOLEAN NOT NULL,
    CreateDate DATETIME NOT NULL,
    DeleteDate DATETIME,
    UpdateDate DATETIME
);

CREATE TABLE AprendizProgram (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    AprendizId INT NOT NULL,
    ProgramId INT NOT NULL,
    FOREIGN KEY (AprendizId) REFERENCES Aprendiz(Id),
    FOREIGN KEY (ProgramId) REFERENCES Program(Id)
);

CREATE TABLE InstructorProgram (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    InstructorId INT NOT NULL,
    ProgramId INT NOT NULL,
    FOREIGN KEY (InstructorId) REFERENCES Instructor(Id),
    FOREIGN KEY (ProgramId) REFERENCES Program(Id)
);

-- Creación de tablas de configuración
CREATE TABLE TypeModality (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Description TEXT,
    Active BOOLEAN NOT NULL
);

CREATE TABLE State (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Active BOOLEAN NOT NULL,
    CreateDate DATETIME NOT NULL,
    DeleteDate DATETIME,
    UpdateDate DATETIME,
    Description VARCHAR(255),
    TypeState VARCHAR(255)
);

CREATE TABLE RegisterySofia (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Description TEXT,
    Document VARCHAR(255),
    Active BOOLEAN NOT NULL,
    CreateDate DATETIME NOT NULL,
    DeleteDate DATETIME,
    UpdateDate DATETIME
);

CREATE TABLE Enterprise (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Observation TEXT,
    NameEnterprise VARCHAR(255) NOT NULL,
    PhoneEnterprise VARCHAR(50),
    Locate VARCHAR(255),
    NitEnterprise VARCHAR(50) NOT NULL UNIQUE,
    EmailEnterprise VARCHAR(255),
    Active BOOLEAN NOT NULL,
    CreateDate DATETIME NOT NULL,
    DeleteDate DATETIME,
    UpdateDate DATETIME
);

CREATE TABLE Concept (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Observation VARCHAR(255),
    Active BOOLEAN NOT NULL,
    CreateDate DATETIME NOT NULL,
    DeleteDate DATETIME,
    UpdateDate DATETIME
);

-- Creación de tablas de proceso-aprendiz-instructor
CREATE TABLE AprendizProcessInstructor (
    Id INT AUTO_INCREMENT PRIMARY KEY,
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
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Description TEXT,
    Active BOOLEAN NOT NULL,
    CreateDate DATETIME NOT NULL,
    DeleteDate DATETIME,
    UpdateDate DATETIME
);

CREATE TABLE Form (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Description TEXT,
    Cuestion TEXT,
    TypeCuestion VARCHAR(255),
    Answer TEXT,
    Active BOOLEAN NOT NULL,
    CreateDate DATETIME NOT NULL,
    DeleteDate DATETIME,
    UpdateDate DATETIME
);

CREATE TABLE FormModule (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    StatusProcedure VARCHAR(255) NOT NULL,
    FormId INT NOT NULL,
    ModuleId INT NOT NULL,
    FOREIGN KEY (FormId) REFERENCES Form(Id),
    FOREIGN KEY (ModuleId) REFERENCES Module(Id)
);

CREATE TABLE RolForm (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Permission VARCHAR(255),
    RolId INT NOT NULL,
    FormId INT NOT NULL,
    FOREIGN KEY (RolId) REFERENCES Rol(Id),
    FOREIGN KEY (FormId) REFERENCES Form(Id)
);

-- Creación de tabla de registro de cambios
CREATE TABLE ChangeLog (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    TableName VARCHAR(255) NOT NULL,
    IdTable INT NOT NULL,
    OldValues TEXT,
    NewValues TEXT,
    Action VARCHAR(50) NOT NULL,
    Active BOOLEAN NOT NULL,
    UserName VARCHAR(255) NOT NULL,
    CreateDate DATETIME NOT NULL,
    UpdateDate DATETIME
); 