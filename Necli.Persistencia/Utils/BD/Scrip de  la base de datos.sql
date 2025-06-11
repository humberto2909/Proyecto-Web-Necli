-- Crear base de datos
CREATE DATABASE NecliProject;
GO

-- Usar la base de datos
USE NecliProject;
GO

-- Tabla: Usuario
CREATE TABLE Usuario (
    IdUsuario INT PRIMARY KEY IDENTITY(1,1),
    TipoUsuario NVARCHAR(50),
    Contrasena NVARCHAR(255),
    Cedula VARCHAR(20),
    NombreUsuario NVARCHAR(100),
    ApellidoUsuario NVARCHAR(100),
    Email NVARCHAR(150),
    FechaNacimiento DATE,
    FechaTokenReset DATETIME NULL,
    TokenResetPassword NVARCHAR(255) NULL
);
GO

-- Tabla: Cuenta
CREATE TABLE Cuenta (
    IdCuenta INT PRIMARY KEY IDENTITY(1,1),
    NombreTitular NVARCHAR(150),
    FechaCreacion DATETIME,
    Saldo DECIMAL(18,2),
    Telefono VARCHAR(20),
    UsuarioId INT NOT NULL,
    EsConfirmada BIT,
    TokenConfirmacion NVARCHAR(255) NULL,
    FOREIGN KEY (UsuarioId) REFERENCES Usuario(IdUsuario)
);
GO

-- Tabla: Transaccion
CREATE TABLE Transaccion (
    IdTransaccion INT PRIMARY KEY IDENTITY(1,1),
    FechaTransaccion DATETIME,
    Monto DECIMAL(18,2),
    TipoTransaccion NVARCHAR(50),
    CuentaOrigenId INT NOT NULL,
    CuentaDestinoId INT NOT NULL,
    FOREIGN KEY (CuentaOrigenId) REFERENCES Cuenta(IdCuenta),
    FOREIGN KEY (CuentaDestinoId) REFERENCES Cuenta(IdCuenta)
);
GO
