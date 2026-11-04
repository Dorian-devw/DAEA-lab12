-- ============================================================
-- SCRIPT DE BASE DE DATOS PARA LA NUBE (MonsterASP.net)
-- Base de datos: db78019
-- ============================================================

-- 1. Eliminar tablas si existen (orden inverso por clave foránea)
IF OBJECT_ID('dbo.Sesiones', 'U') IS NOT NULL
    DROP TABLE dbo.Sesiones;
GO

IF OBJECT_ID('dbo.Salas', 'U') IS NOT NULL
    DROP TABLE dbo.Salas;
GO

-- 2. Creación de tabla Padre: Salas
CREATE TABLE dbo.Salas (
    Id INT IDENTITY(1,1) NOT NULL,
    Nombre NVARCHAR(50) NOT NULL,
    Descripcion NVARCHAR(250) NULL,
    CapacidadMaxima INT NOT NULL,
    NivelProteccion NVARCHAR(20) NOT NULL,
    PrecioBase DECIMAL(10,2) NOT NULL,
    Activa BIT NOT NULL CONSTRAINT DF_Salas_Activa DEFAULT (1),
    CONSTRAINT PK_Salas PRIMARY KEY CLUSTERED (Id ASC),
    CONSTRAINT UQ_Salas_Nombre UNIQUE (Nombre),
    CONSTRAINT CK_Salas_Capacidad CHECK (CapacidadMaxima >= 1 AND CapacidadMaxima <= 10),
    CONSTRAINT CK_Salas_Precio CHECK (PrecioBase >= 0)
);
GO

-- 3. Creación de tabla Hija: Sesiones (Relación 1:N)
CREATE TABLE dbo.Sesiones (
    Id INT IDENTITY(1,1) NOT NULL,
    SalaId INT NOT NULL,
    ClienteNombre NVARCHAR(100) NOT NULL,
    ClienteEmail NVARCHAR(100) NOT NULL,
    ClienteTelefono NVARCHAR(20) NULL,
    FechaHora DATETIME2 NOT NULL,
    DuracionMinutos INT NOT NULL,
    CantidadPersonas INT NOT NULL,
    PrecioTotal DECIMAL(10,2) NOT NULL,
    Estado NVARCHAR(20) NOT NULL CONSTRAINT DF_Sesiones_Estado DEFAULT ('Pendiente'),
    CONSTRAINT PK_Sesiones PRIMARY KEY CLUSTERED (Id ASC),
    CONSTRAINT FK_Sesiones_Salas FOREIGN KEY (SalaId) REFERENCES dbo.Salas (Id),
    CONSTRAINT CK_Sesiones_Duracion CHECK (DuracionMinutos IN (15, 30, 45, 60)),
    CONSTRAINT CK_Sesiones_Personas CHECK (CantidadPersonas >= 1),
    CONSTRAINT CK_Sesiones_Precio CHECK (PrecioTotal >= 0)
);
GO

-- 4. Inserción de datos de ejemplo (Salas)
INSERT INTO dbo.Salas (Nombre, Descripcion, CapacidadMaxima, NivelProteccion, PrecioBase, Activa)
VALUES 
('Sala Furia Caos', 'Equipada con electrodomesticos pesados y bates de beisbol reforzados.', 4, 'Extremo', 120.00, 1),
('Sala Terapia Express', 'Ideal para desestresarse rapidamente rompiendo botellas y vajilla de cristal.', 2, 'Basico', 60.00, 1),
('Sala Destruccion VIP', 'Espacio premium para grupos con monitores, impresoras y mobiliario.', 6, 'Intermedio', 180.00, 1),
('Sala Bunker Cero', 'Sala acustica individual para liberacion de tension extrema en remodelacion.', 1, 'Extremo', 90.00, 0);
GO

-- 5. Inserción de datos de ejemplo (Sesiones)
INSERT INTO dbo.Sesiones (SalaId, ClienteNombre, ClienteEmail, ClienteTelefono, FechaHora, DuracionMinutos, CantidadPersonas, PrecioTotal, Estado)
VALUES
(1, 'Carlos Mendoza', 'carlos.mendoza@gmail.com', '987654321', '2026-11-05 15:00:00', 30, 2, 120.00, 'Confirmada'),
(1, 'Andrea Torres', 'andrea.torres@hotmail.com', '912345678', '2026-11-05 17:00:00', 45, 3, 160.00, 'Pendiente'),
(2, 'Roberto Gomez', 'rgomez@outlook.com', '998877665', '2026-11-06 11:00:00', 15, 1, 60.00, 'Completada'),
(2, 'Valeria Rios', 'valeria.rios@yahoo.com', '933445566', '2026-11-06 16:30:00', 30, 2, 90.00, 'Confirmada'),
(3, 'Mariana Silva', 'msilva@empresa.pe', '955667788', '2026-11-07 18:00:00', 60, 5, 240.00, 'Pendiente');
GO

