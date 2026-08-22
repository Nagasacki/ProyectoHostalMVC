IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'HOSTALVELAMORMVC')
BEGIN
    CREATE DATABASE HOSTALVELAMORMVC;
END
GO

USE HOSTALVELAMORMVC;
GO

-- =============================================
-- 1. TABLA DE USUARIOS (AUTENTICACIÓN Y ROLES)
-- =============================================
IF OBJECT_ID('dbo.Usuario', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Usuario (
        IdUsuario INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Nombre VARCHAR(100) NOT NULL,
        Correo VARCHAR(100) NOT NULL UNIQUE,
        Clave VARCHAR(100) NOT NULL,
        Rol VARCHAR(30) NOT NULL DEFAULT 'Recepcionista', -- 'Administrador', 'Recepcionista'
        Estado VARCHAR(20) NOT NULL DEFAULT 'Activo',
        FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
    );
END
GO

-- =============================================
-- 2. TABLA DE CLIENTES
-- =============================================
IF OBJECT_ID('dbo.Cliente', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Cliente (
        IdCliente INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Dni VARCHAR(8) NOT NULL UNIQUE,
        Nombre VARCHAR(100) NOT NULL,
        Telefono VARCHAR(15) NULL,
        Correo VARCHAR(100) NULL
    );
END
GO

-- =============================================
-- 3. TABLA DE HABITACIONES
-- =============================================
IF OBJECT_ID('dbo.Habitacion', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Habitacion (
        IdHabitacion INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Numero VARCHAR(10) NOT NULL UNIQUE,
        Tipo VARCHAR(50) NOT NULL,
        Precio DECIMAL(10, 2) NOT NULL,
        Estado VARCHAR(20) NOT NULL DEFAULT 'Disponible' -- 'Disponible', 'Ocupada', 'Mantenimiento'
    );
END
GO

-- =============================================
-- 4. TABLA DE RESERVAS
-- =============================================
IF OBJECT_ID('dbo.Reserva', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Reserva (
        IdReserva INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        IdCliente INT NOT NULL CONSTRAINT FK_Reserva_Cliente REFERENCES dbo.Cliente(IdCliente),
        IdHabitacion INT NOT NULL CONSTRAINT FK_Reserva_Habitacion REFERENCES dbo.Habitacion(IdHabitacion),
        FechaEntrada DATE NOT NULL,
        FechaSalida DATE NOT NULL,
        CantidadDias INT NOT NULL,
        PrecioDia DECIMAL(10, 2) NOT NULL,
        Total DECIMAL(10, 2) NOT NULL,
        Estado VARCHAR(20) NOT NULL DEFAULT 'Reservada', -- 'Reservada', 'Finalizada', 'Cancelada'
        FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
    );
END
GO

-- =============================================
-- 5. PROCEDIMIENTOS ALMACENADOS - SEGURIDAD Y LOGIN
-- =============================================

CREATE OR ALTER PROCEDURE dbo.sp_ValidarUsuario
    @Correo VARCHAR(100),
    @Clave VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        IdUsuario,
        Nombre,
        Correo,
        Rol,
        Estado
    FROM dbo.Usuario
    WHERE Correo = @Correo 
      AND Clave = @Clave
      AND Estado = 'Activo';
END
GO

-- =============================================
-- 6. PROCEDIMIENTOS ALMACENADOS - DASHBOARD
-- =============================================

CREATE OR ALTER PROCEDURE dbo.sp_DashboardResumen
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        (SELECT COUNT(*) FROM dbo.Habitacion) AS TotalHabitaciones,
        (SELECT COUNT(*) FROM dbo.Habitacion WHERE Estado = 'Disponible') AS HabitacionesDisponibles,
        (SELECT COUNT(*) FROM dbo.Habitacion WHERE Estado = 'Ocupada') AS HabitacionesOcupadas,
        (SELECT COUNT(*) FROM dbo.Cliente) AS TotalClientes,
        (SELECT COUNT(*) FROM dbo.Reserva WHERE Estado = 'Reservada') AS ReservasActivas;
END
GO

-- =============================================
-- 7. PROCEDIMIENTOS ALMACENADOS - CLIENTES
-- =============================================

CREATE OR ALTER PROCEDURE dbo.sp_ListarClientes
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        IdCliente,
        Dni,
        Nombre,
        Telefono,
        Correo
    FROM dbo.Cliente
    ORDER BY Nombre;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_BuscarCliente
    @IdCliente INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        IdCliente,
        Dni,
        Nombre,
        Telefono,
        Correo
    FROM dbo.Cliente
    WHERE IdCliente = @IdCliente;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_BuscarClientePorDni
    @Dni VARCHAR(8)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        IdCliente,
        Dni,
        Nombre,
        Telefono,
        Correo
    FROM dbo.Cliente
    WHERE Dni = @Dni;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_MergeCliente
    @IdCliente INT,
    @Dni VARCHAR(8),
    @Nombre VARCHAR(100),
    @Telefono VARCHAR(15),
    @Correo VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    MERGE dbo.Cliente AS TARGET
    USING (
        SELECT
            @IdCliente AS IdCliente,
            @Dni AS Dni,
            @Nombre AS Nombre,
            @Telefono AS Telefono,
            @Correo AS Correo
    ) AS SOURCE
    ON TARGET.IdCliente = SOURCE.IdCliente
    WHEN MATCHED THEN
        UPDATE SET
            TARGET.Dni = SOURCE.Dni,
            TARGET.Nombre = SOURCE.Nombre,
            TARGET.Telefono = SOURCE.Telefono,
            TARGET.Correo = SOURCE.Correo
    WHEN NOT MATCHED THEN
        INSERT (Dni, Nombre, Telefono, Correo)
        VALUES (SOURCE.Dni, SOURCE.Nombre, SOURCE.Telefono, SOURCE.Correo);
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_EliminarCliente
    @IdCliente INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.Cliente WHERE IdCliente = @IdCliente;
END
GO

-- =============================================
-- 8. PROCEDIMIENTOS ALMACENADOS - HABITACIONES
-- =============================================

CREATE OR ALTER PROCEDURE dbo.sp_ListarHabitaciones
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        IdHabitacion,
        Numero,
        Tipo,
        Precio,
        Estado
    FROM dbo.Habitacion
    ORDER BY Numero;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_ListarHabitacionesDisponibles
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        IdHabitacion,
        Numero,
        Tipo,
        Precio,
        Estado
    FROM dbo.Habitacion
    WHERE Estado = 'Disponible'
    ORDER BY Numero;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_BuscarHabitacion
    @IdHabitacion INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        IdHabitacion,
        Numero,
        Tipo,
        Precio,
        Estado
    FROM dbo.Habitacion
    WHERE IdHabitacion = @IdHabitacion;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_MergeHabitacion
    @IdHabitacion INT,
    @Numero VARCHAR(10),
    @Tipo VARCHAR(50),
    @Precio DECIMAL(10,2),
    @Estado VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    MERGE dbo.Habitacion AS TARGET
    USING (
        SELECT
            @IdHabitacion AS IdHabitacion,
            @Numero AS Numero,
            @Tipo AS Tipo,
            @Precio AS Precio,
            @Estado AS Estado
    ) AS SOURCE
    ON TARGET.IdHabitacion = SOURCE.IdHabitacion
    WHEN MATCHED THEN
        UPDATE SET
            TARGET.Numero = SOURCE.Numero,
            TARGET.Tipo = SOURCE.Tipo,
            TARGET.Precio = SOURCE.Precio,
            TARGET.Estado = SOURCE.Estado
    WHEN NOT MATCHED THEN
        INSERT (Numero, Tipo, Precio, Estado)
        VALUES (SOURCE.Numero, SOURCE.Tipo, SOURCE.Precio, SOURCE.Estado);
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_EliminarHabitacion
    @IdHabitacion INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.Habitacion WHERE IdHabitacion = @IdHabitacion;
END
GO

-- =============================================
-- 9. PROCEDIMIENTOS ALMACENADOS - RESERVAS
-- =============================================

CREATE OR ALTER PROCEDURE dbo.sp_ListarReservas
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        R.IdReserva,
        R.IdCliente,
        C.Nombre AS NombreCliente,
        R.IdHabitacion,
        H.Numero AS NumeroHabitacion,
        H.Tipo AS TipoHabitacion,
        R.FechaEntrada,
        R.FechaSalida,
        R.CantidadDias,
        R.PrecioDia,
        R.Total,
        R.Estado,
        R.FechaRegistro
    FROM dbo.Reserva R
    INNER JOIN dbo.Cliente C ON R.IdCliente = C.IdCliente
    INNER JOIN dbo.Habitacion H ON R.IdHabitacion = H.IdHabitacion
    ORDER BY R.IdReserva DESC;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_BuscarReserva
    @IdReserva INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        R.IdReserva,
        R.IdCliente,
        C.Nombre AS NombreCliente,
        R.IdHabitacion,
        H.Numero AS NumeroHabitacion,
        H.Tipo AS TipoHabitacion,
        R.FechaEntrada,
        R.FechaSalida,
        R.CantidadDias,
        R.PrecioDia,
        R.Total,
        R.Estado,
        R.FechaRegistro
    FROM dbo.Reserva R
    INNER JOIN dbo.Cliente C ON R.IdCliente = C.IdCliente
    INNER JOIN dbo.Habitacion H ON R.IdHabitacion = H.IdHabitacion
    WHERE R.IdReserva = @IdReserva;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_RegistrarReserva
    @IdCliente INT,
    @IdHabitacion INT,
    @FechaEntrada DATE,
    @FechaSalida DATE,
    @CantidadDias INT,
    @PrecioDia DECIMAL(10,2),
    @Total DECIMAL(10,2)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        INSERT INTO dbo.Reserva (
            IdCliente,
            IdHabitacion,
            FechaEntrada,
            FechaSalida,
            CantidadDias,
            PrecioDia,
            Total,
            Estado
        )
        VALUES (
            @IdCliente,
            @IdHabitacion,
            @FechaEntrada,
            @FechaSalida,
            @CantidadDias,
            @PrecioDia,
            @Total,
            'Reservada'
        );

        UPDATE dbo.Habitacion
        SET Estado = 'Ocupada'
        WHERE IdHabitacion = @IdHabitacion;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_FinalizarReserva
    @IdReserva INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        DECLARE @IdHabitacion INT;

        SELECT @IdHabitacion = IdHabitacion
        FROM dbo.Reserva
        WHERE IdReserva = @IdReserva;

        UPDATE dbo.Reserva
        SET Estado = 'Finalizada'
        WHERE IdReserva = @IdReserva;

        UPDATE dbo.Habitacion
        SET Estado = 'Disponible'
        WHERE IdHabitacion = @IdHabitacion;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_CancelarReserva
    @IdReserva INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        DECLARE @IdHabitacion INT;

        SELECT @IdHabitacion = IdHabitacion
        FROM dbo.Reserva
        WHERE IdReserva = @IdReserva;

        UPDATE dbo.Reserva
        SET Estado = 'Cancelada'
        WHERE IdReserva = @IdReserva;

        UPDATE dbo.Habitacion
        SET Estado = 'Disponible'
        WHERE IdHabitacion = @IdHabitacion;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- =============================================
-- 10. PROCEDIMIENTOS ALMACENADOS - REPORTES
-- =============================================

CREATE OR ALTER PROCEDURE dbo.sp_ReporteResumen
    @FechaInicio DATE,
    @FechaFin DATE
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        COUNT(*) AS TotalReservas,
        ISNULL(SUM(CASE WHEN Estado = 'Finalizada' THEN 1 ELSE 0 END), 0) AS ReservasFinalizadas,
        ISNULL(SUM(CASE WHEN Estado = 'Cancelada' THEN 1 ELSE 0 END), 0) AS ReservasCanceladas,
        ISNULL(SUM(CASE WHEN Estado = 'Finalizada' THEN Total ELSE 0 END), 0) AS TotalIngresos
    FROM dbo.Reserva
    WHERE FechaEntrada BETWEEN @FechaInicio AND @FechaFin;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_ReporteReservas
    @FechaInicio DATE,
    @FechaFin DATE
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        R.IdReserva,
        C.Nombre AS NombreCliente,
        H.Numero AS NumeroHabitacion,
        H.Tipo AS TipoHabitacion,
        R.FechaEntrada,
        R.FechaSalida,
        R.CantidadDias,
        R.PrecioDia,
        R.Total,
        R.Estado
    FROM dbo.Reserva R
    INNER JOIN dbo.Cliente C ON R.IdCliente = C.IdCliente
    INNER JOIN dbo.Habitacion H ON R.IdHabitacion = H.IdHabitacion
    WHERE R.FechaEntrada BETWEEN @FechaInicio AND @FechaFin
    ORDER BY R.FechaEntrada DESC;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_ReporteHabitaciones
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        IdHabitacion,
        Numero,
        Tipo,
        Precio,
        Estado
    FROM dbo.Habitacion
    ORDER BY Numero;
END
GO

-- =============================================
-- 11. INSERCIÓN DE DATOS SEMILLA
-- =============================================

IF NOT EXISTS (SELECT 1 FROM dbo.Usuario WHERE Correo = 'admin@hostalvelamor.com')
BEGIN
    INSERT INTO dbo.Usuario (Nombre, Correo, Clave, Rol, Estado)
    VALUES ('Administrador General', 'admin@hostalvelamor.com', 'Admin123*', 'Administrador', 'Activo');
END

IF NOT EXISTS (SELECT 1 FROM dbo.Usuario WHERE Correo = 'recepcion@hostalvelamor.com')
BEGIN
    INSERT INTO dbo.Usuario (Nombre, Correo, Clave, Rol, Estado)
    VALUES ('Recepcionista Principal', 'recepcion@hostalvelamor.com', 'Recepcion123*', 'Recepcionista', 'Activo');
END

IF NOT EXISTS (SELECT 1 FROM dbo.Habitacion WHERE Numero = '101')
BEGIN
    INSERT INTO dbo.Habitacion (Numero, Tipo, Precio, Estado) VALUES
    ('101', 'Simple', 60.00, 'Disponible'),
    ('102', 'Simple', 60.00, 'Disponible'),
    ('103', 'Matrimonial', 90.00, 'Disponible'),
    ('201', 'Matrimonial', 90.00, 'Disponible'),
    ('202', 'Doble', 120.00, 'Disponible'),
    ('203', 'Doble', 120.00, 'Disponible'),
    ('301', 'Suite', 160.00, 'Disponible'),
    ('302', 'Suite Presidencial', 220.00, 'Disponible');
END

IF NOT EXISTS (SELECT 1 FROM dbo.Cliente WHERE Dni = '71829304')
BEGIN
    INSERT INTO dbo.Cliente (Dni, Nombre, Telefono, Correo) VALUES
    ('71829304', 'Carlos Mendoza Vargas', '987654321', 'carlos.mendoza@gmail.com'),
    ('72938415', 'Ana Lucía Torres Quispe', '912345678', 'ana.torres@hotmail.com'),
    ('73849526', 'Jorge Luis Ramos Benitez', '998877665', 'jorge.ramos@outlook.com'),
    ('74958637', 'Valeria Sofia Castro Silva', '945612378', 'valeria.castro@gmail.com'),
    ('75069748', 'Ricardo Alonso Vega Ruiz', '933221144', 'ricardo.vega@empresa.pe');
END
GO

-- =============================================
-- 12. FUNCIONES HOTELERAS COMPLETAS
-- Este archivo es el único script necesario para instalar la base de datos.
-- =============================================

IF COL_LENGTH('dbo.Cliente', 'TipoDocumento') IS NULL
    ALTER TABLE dbo.Cliente ADD TipoDocumento VARCHAR(20) NOT NULL CONSTRAINT DF_Cliente_TipoDocumento DEFAULT 'DNI';
IF COL_LENGTH('dbo.Cliente', 'Nacionalidad') IS NULL
    ALTER TABLE dbo.Cliente ADD Nacionalidad VARCHAR(50) NULL;
IF COL_LENGTH('dbo.Cliente', 'Direccion') IS NULL
    ALTER TABLE dbo.Cliente ADD Direccion VARCHAR(150) NULL;
IF COL_LENGTH('dbo.Cliente', 'FechaNacimiento') IS NULL
    ALTER TABLE dbo.Cliente ADD FechaNacimiento DATE NULL;
IF COL_LENGTH('dbo.Cliente', 'ContactoEmergencia') IS NULL
    ALTER TABLE dbo.Cliente ADD ContactoEmergencia VARCHAR(100) NULL;
IF COL_LENGTH('dbo.Cliente', 'Observaciones') IS NULL
    ALTER TABLE dbo.Cliente ADD Observaciones VARCHAR(250) NULL;
GO

ALTER TABLE dbo.Cliente ALTER COLUMN Dni VARCHAR(15) NOT NULL;
GO

IF COL_LENGTH('dbo.Reserva', 'FechaCheckInReal') IS NULL
    ALTER TABLE dbo.Reserva ADD FechaCheckInReal DATETIME2(0) NULL;
IF COL_LENGTH('dbo.Reserva', 'FechaCheckOutReal') IS NULL
    ALTER TABLE dbo.Reserva ADD FechaCheckOutReal DATETIME2(0) NULL;
GO

UPDATE dbo.Reserva SET Estado = 'Confirmada' WHERE Estado = 'Reservada';
UPDATE H
SET Estado = 'Disponible'
FROM dbo.Habitacion H
WHERE H.Estado = 'Ocupada'
  AND NOT EXISTS (SELECT 1 FROM dbo.Reserva R WHERE R.IdHabitacion = H.IdHabitacion AND R.Estado = 'Hospedado');
GO

IF OBJECT_ID('dbo.Pago', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Pago (
        IdPago INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        IdReserva INT NOT NULL CONSTRAINT FK_Pago_Reserva REFERENCES dbo.Reserva(IdReserva),
        FechaPago DATETIME2(0) NOT NULL CONSTRAINT DF_Pago_Fecha DEFAULT SYSDATETIME(),
        Monto DECIMAL(10,2) NOT NULL,
        Metodo VARCHAR(20) NOT NULL,
        Tipo VARCHAR(20) NOT NULL DEFAULT 'Adelanto',
        NumeroOperacion VARCHAR(50) NULL,
        CONSTRAINT CK_Pago_Monto CHECK (Monto > 0)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Reserva_Disponibilidad' AND object_id = OBJECT_ID('dbo.Reserva'))
    CREATE INDEX IX_Reserva_Disponibilidad ON dbo.Reserva(IdHabitacion, FechaEntrada, FechaSalida, Estado);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Pago_Reserva' AND object_id = OBJECT_ID('dbo.Pago'))
    CREATE INDEX IX_Pago_Reserva ON dbo.Pago(IdReserva);
GO

CREATE OR ALTER PROCEDURE dbo.sp_ListarClientes
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdCliente, Dni, TipoDocumento, Nombre, Telefono, Correo, Nacionalidad,
           Direccion, FechaNacimiento, ContactoEmergencia, Observaciones
    FROM dbo.Cliente ORDER BY Nombre;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_BuscarCliente
    @IdCliente INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdCliente, Dni, TipoDocumento, Nombre, Telefono, Correo, Nacionalidad,
           Direccion, FechaNacimiento, ContactoEmergencia, Observaciones
    FROM dbo.Cliente WHERE IdCliente = @IdCliente;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_BuscarClientePorDni
    @Dni VARCHAR(15)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdCliente, Dni, TipoDocumento, Nombre, Telefono, Correo, Nacionalidad,
           Direccion, FechaNacimiento, ContactoEmergencia, Observaciones
    FROM dbo.Cliente WHERE Dni = @Dni;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_MergeCliente
    @IdCliente INT,
    @Dni VARCHAR(15),
    @TipoDocumento VARCHAR(20),
    @Nombre VARCHAR(100),
    @Telefono VARCHAR(15),
    @Correo VARCHAR(100),
    @Nacionalidad VARCHAR(50) = NULL,
    @Direccion VARCHAR(150) = NULL,
    @FechaNacimiento DATE = NULL,
    @ContactoEmergencia VARCHAR(100) = NULL,
    @Observaciones VARCHAR(250) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @IdCliente = 0
        INSERT INTO dbo.Cliente (Dni, TipoDocumento, Nombre, Telefono, Correo, Nacionalidad, Direccion,
                                 FechaNacimiento, ContactoEmergencia, Observaciones)
        VALUES (@Dni, @TipoDocumento, @Nombre, @Telefono, @Correo, @Nacionalidad, @Direccion,
                @FechaNacimiento, @ContactoEmergencia, @Observaciones);
    ELSE
        UPDATE dbo.Cliente
        SET Dni = @Dni, TipoDocumento = @TipoDocumento, Nombre = @Nombre, Telefono = @Telefono,
            Correo = @Correo, Nacionalidad = @Nacionalidad, Direccion = @Direccion,
            FechaNacimiento = @FechaNacimiento, ContactoEmergencia = @ContactoEmergencia,
            Observaciones = @Observaciones
        WHERE IdCliente = @IdCliente;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_ListarHabitacionesDisponiblesPorFechas
    @FechaEntrada DATE,
    @FechaSalida DATE
AS
BEGIN
    SET NOCOUNT ON;
    IF @FechaSalida <= @FechaEntrada
        THROW 50001, 'El rango de fechas no es válido.', 1;

    SELECT H.IdHabitacion, H.Numero, H.Tipo, H.Precio, H.Estado
    FROM dbo.Habitacion H
    WHERE H.Estado NOT IN ('Mantenimiento', 'Fuera de servicio')
      AND NOT EXISTS (
          SELECT 1 FROM dbo.Reserva R
          WHERE R.IdHabitacion = H.IdHabitacion
            AND R.Estado IN ('Confirmada', 'Hospedado')
            AND R.FechaEntrada < @FechaSalida
            AND R.FechaSalida > @FechaEntrada
      )
    ORDER BY H.Numero;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_ListarReservas
AS
BEGIN
    SET NOCOUNT ON;
    SELECT R.IdReserva, R.IdCliente, C.Nombre AS NombreCliente, R.IdHabitacion,
           H.Numero AS NumeroHabitacion, H.Tipo AS TipoHabitacion,
           R.FechaEntrada, R.FechaSalida, R.CantidadDias, R.PrecioDia, R.Total,
           R.Estado, R.FechaRegistro, R.FechaCheckInReal, R.FechaCheckOutReal,
           ISNULL(P.MontoPagado, 0) AS MontoPagado
    FROM dbo.Reserva R
    INNER JOIN dbo.Cliente C ON C.IdCliente = R.IdCliente
    INNER JOIN dbo.Habitacion H ON H.IdHabitacion = R.IdHabitacion
    OUTER APPLY (SELECT SUM(Monto) AS MontoPagado FROM dbo.Pago WHERE IdReserva = R.IdReserva) P
    ORDER BY R.IdReserva DESC;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_BuscarReserva
    @IdReserva INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT R.IdReserva, R.IdCliente, C.Nombre AS NombreCliente, R.IdHabitacion,
           H.Numero AS NumeroHabitacion, H.Tipo AS TipoHabitacion,
           R.FechaEntrada, R.FechaSalida, R.CantidadDias, R.PrecioDia, R.Total,
           R.Estado, R.FechaRegistro, R.FechaCheckInReal, R.FechaCheckOutReal,
           ISNULL(P.MontoPagado, 0) AS MontoPagado
    FROM dbo.Reserva R
    INNER JOIN dbo.Cliente C ON C.IdCliente = R.IdCliente
    INNER JOIN dbo.Habitacion H ON H.IdHabitacion = R.IdHabitacion
    OUTER APPLY (SELECT SUM(Monto) AS MontoPagado FROM dbo.Pago WHERE IdReserva = R.IdReserva) P
    WHERE R.IdReserva = @IdReserva;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_RegistrarReserva
    @IdCliente INT,
    @IdHabitacion INT,
    @FechaEntrada DATE,
    @FechaSalida DATE,
    @CantidadDias INT,
    @PrecioDia DECIMAL(10,2),
    @Total DECIMAL(10,2)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        IF @FechaSalida <= @FechaEntrada
            THROW 50001, 'La salida debe ser posterior a la entrada.', 1;

        IF EXISTS (SELECT 1 FROM dbo.Habitacion WITH (UPDLOCK, HOLDLOCK)
                   WHERE IdHabitacion = @IdHabitacion AND Estado IN ('Mantenimiento', 'Fuera de servicio'))
            THROW 50002, 'La habitación no está operativa.', 1;

        IF EXISTS (
            SELECT 1 FROM dbo.Reserva WITH (UPDLOCK, HOLDLOCK)
            WHERE IdHabitacion = @IdHabitacion
              AND Estado IN ('Confirmada', 'Hospedado')
              AND FechaEntrada < @FechaSalida
              AND FechaSalida > @FechaEntrada
        )
            THROW 50003, 'La habitación ya está reservada en esas fechas.', 1;

        INSERT INTO dbo.Reserva (IdCliente, IdHabitacion, FechaEntrada, FechaSalida,
                                 CantidadDias, PrecioDia, Total, Estado)
        VALUES (@IdCliente, @IdHabitacion, @FechaEntrada, @FechaSalida,
                @CantidadDias, @PrecioDia, @Total, 'Confirmada');
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_CheckInReserva
    @IdReserva INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        DECLARE @IdHabitacion INT;
        SELECT @IdHabitacion = IdHabitacion FROM dbo.Reserva WITH (UPDLOCK, HOLDLOCK)
        WHERE IdReserva = @IdReserva AND Estado = 'Confirmada';
        IF @IdHabitacion IS NULL THROW 50004, 'La reserva no está disponible para check-in.', 1;
        IF EXISTS (SELECT 1 FROM dbo.Habitacion WHERE IdHabitacion = @IdHabitacion AND Estado <> 'Disponible')
            THROW 50005, 'La habitación no está disponible físicamente.', 1;

        UPDATE dbo.Reserva SET Estado = 'Hospedado', FechaCheckInReal = SYSDATETIME()
        WHERE IdReserva = @IdReserva;
        UPDATE dbo.Habitacion SET Estado = 'Ocupada' WHERE IdHabitacion = @IdHabitacion;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_CheckOutReserva
    @IdReserva INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        DECLARE @IdHabitacion INT, @Total DECIMAL(10,2), @Pagado DECIMAL(10,2);
        SELECT @IdHabitacion = IdHabitacion, @Total = Total
        FROM dbo.Reserva WITH (UPDLOCK, HOLDLOCK)
        WHERE IdReserva = @IdReserva AND Estado = 'Hospedado';
        IF @IdHabitacion IS NULL THROW 50006, 'La reserva no está disponible para check-out.', 1;
        SELECT @Pagado = ISNULL(SUM(Monto), 0) FROM dbo.Pago WHERE IdReserva = @IdReserva;
        IF @Pagado < @Total THROW 50007, 'Debe cancelar el saldo pendiente antes del check-out.', 1;

        UPDATE dbo.Reserva SET Estado = 'Finalizada', FechaCheckOutReal = SYSDATETIME()
        WHERE IdReserva = @IdReserva;
        UPDATE dbo.Habitacion SET Estado = 'Limpieza' WHERE IdHabitacion = @IdHabitacion;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_CancelarReserva
    @IdReserva INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Reserva SET Estado = 'Cancelada'
    WHERE IdReserva = @IdReserva AND Estado = 'Confirmada';
    IF @@ROWCOUNT = 0 THROW 50008, 'Solo se puede cancelar una reserva confirmada.', 1;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_NoPresentadoReserva
    @IdReserva INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Reserva SET Estado = 'No presentado'
    WHERE IdReserva = @IdReserva AND Estado = 'Confirmada';
    IF @@ROWCOUNT = 0 THROW 50009, 'Solo una reserva confirmada puede marcarse como no presentada.', 1;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_MarcarHabitacionDisponible
    @IdHabitacion INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Habitacion SET Estado = 'Disponible'
    WHERE IdHabitacion = @IdHabitacion AND Estado = 'Limpieza';
    IF @@ROWCOUNT = 0 THROW 50010, 'La habitación no está pendiente de limpieza.', 1;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_ListarPagos
    @IdReserva INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT P.IdPago, P.IdReserva, P.FechaPago, P.Monto, P.Metodo, P.Tipo,
           P.NumeroOperacion, C.Nombre AS NombreCliente, H.Numero AS NumeroHabitacion
    FROM dbo.Pago P
    INNER JOIN dbo.Reserva R ON R.IdReserva = P.IdReserva
    INNER JOIN dbo.Cliente C ON C.IdCliente = R.IdCliente
    INNER JOIN dbo.Habitacion H ON H.IdHabitacion = R.IdHabitacion
    WHERE @IdReserva IS NULL OR P.IdReserva = @IdReserva
    ORDER BY P.FechaPago DESC;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_RegistrarPago
    @IdReserva INT,
    @Monto DECIMAL(10,2),
    @Metodo VARCHAR(20),
    @Tipo VARCHAR(20),
    @NumeroOperacion VARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        DECLARE @Total DECIMAL(10,2), @Pagado DECIMAL(10,2);
        SELECT @Total = Total FROM dbo.Reserva WITH (UPDLOCK, HOLDLOCK)
        WHERE IdReserva = @IdReserva AND Estado IN ('Confirmada', 'Hospedado');
        IF @Total IS NULL THROW 50011, 'La reserva no admite pagos.', 1;
        SELECT @Pagado = ISNULL(SUM(Monto), 0) FROM dbo.Pago WHERE IdReserva = @IdReserva;
        IF @Monto <= 0 OR @Pagado + @Monto > @Total
            THROW 50012, 'El monto supera el saldo pendiente.', 1;

        INSERT INTO dbo.Pago (IdReserva, Monto, Metodo, Tipo, NumeroOperacion)
        VALUES (@IdReserva, @Monto, @Metodo, @Tipo, NULLIF(@NumeroOperacion, ''));
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_DashboardResumen
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        (SELECT COUNT(*) FROM dbo.Reserva WHERE FechaEntrada = CAST(GETDATE() AS DATE) AND Estado = 'Confirmada') AS LlegadasHoy,
        (SELECT COUNT(*) FROM dbo.Reserva WHERE FechaSalida = CAST(GETDATE() AS DATE) AND Estado = 'Hospedado') AS SalidasHoy,
        (SELECT COUNT(*) FROM dbo.Reserva WHERE Estado = 'Hospedado') AS HuespedesAlojados,
        (SELECT COUNT(*) FROM dbo.Habitacion WHERE Estado = 'Limpieza') AS HabitacionesLimpieza,
        (SELECT COUNT(*) FROM dbo.Habitacion) AS TotalHabitaciones,
        (SELECT COUNT(*) FROM dbo.Habitacion WHERE Estado = 'Disponible') AS HabitacionesDisponibles,
        (SELECT COUNT(*) FROM dbo.Habitacion WHERE Estado = 'Ocupada') AS HabitacionesOcupadas,
        (SELECT COUNT(*) FROM dbo.Habitacion WHERE Estado IN ('Mantenimiento', 'Fuera de servicio')) AS HabitacionesMantenimiento,
        (SELECT ISNULL(SUM(Monto),0) FROM dbo.Pago WHERE CAST(FechaPago AS DATE) = CAST(GETDATE() AS DATE)) AS CobradoHoy;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_DashboardMovimientosHoy
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 8 R.IdReserva, 'Llegada' AS Tipo, C.Nombre AS Huesped, H.Numero AS Habitacion,
           R.FechaEntrada AS FechaProgramada, R.Estado
    FROM dbo.Reserva R JOIN dbo.Cliente C ON C.IdCliente=R.IdCliente JOIN dbo.Habitacion H ON H.IdHabitacion=R.IdHabitacion
    WHERE R.FechaEntrada = CAST(GETDATE() AS DATE) AND R.Estado IN ('Confirmada','Hospedado')
    UNION ALL
    SELECT TOP 8 R.IdReserva, 'Salida', C.Nombre, H.Numero, R.FechaSalida, R.Estado
    FROM dbo.Reserva R JOIN dbo.Cliente C ON C.IdCliente=R.IdCliente JOIN dbo.Habitacion H ON H.IdHabitacion=R.IdHabitacion
    WHERE R.FechaSalida = CAST(GETDATE() AS DATE) AND R.Estado IN ('Hospedado','Finalizada')
    ORDER BY FechaProgramada, Tipo;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_DashboardPagosPendientes
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 5 R.IdReserva, C.Nombre AS Huesped, H.Numero AS Habitacion,
           R.Total - ISNULL(P.Pagado,0) AS Saldo
    FROM dbo.Reserva R
    JOIN dbo.Cliente C ON C.IdCliente=R.IdCliente
    JOIN dbo.Habitacion H ON H.IdHabitacion=R.IdHabitacion
    OUTER APPLY (SELECT SUM(Monto) Pagado FROM dbo.Pago WHERE IdReserva=R.IdReserva) P
    WHERE R.Estado IN ('Confirmada','Hospedado') AND R.Total > ISNULL(P.Pagado,0)
    ORDER BY R.FechaEntrada;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_ReporteResumen
    @FechaInicio DATE,
    @FechaFin DATE
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        COUNT(*) AS TotalReservas,
        SUM(CASE WHEN Estado='Finalizada' THEN 1 ELSE 0 END) AS ReservasFinalizadas,
        SUM(CASE WHEN Estado IN ('Cancelada','No presentado') THEN 1 ELSE 0 END) AS ReservasCanceladas,
        ISNULL((SELECT SUM(Monto) FROM dbo.Pago WHERE CAST(FechaPago AS DATE) BETWEEN @FechaInicio AND @FechaFin),0) AS TotalIngresos,
        ISNULL(SUM(Total),0) AS TotalFacturado,
        ISNULL(SUM(Total),0) - ISNULL((SELECT SUM(Monto) FROM dbo.Pago WHERE CAST(FechaPago AS DATE) BETWEEN @FechaInicio AND @FechaFin),0) AS SaldoPendiente
    FROM dbo.Reserva
    WHERE FechaEntrada BETWEEN @FechaInicio AND @FechaFin;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_ReporteReservas
    @FechaInicio DATE,
    @FechaFin DATE
AS
BEGIN
    SET NOCOUNT ON;
    SELECT R.IdReserva, C.Nombre AS NombreCliente, H.Numero AS NumeroHabitacion, H.Tipo AS TipoHabitacion,
           R.FechaEntrada, R.FechaSalida, R.CantidadDias, R.PrecioDia, R.Total, R.Estado,
           ISNULL(P.Pagado,0) AS MontoPagado, R.Total-ISNULL(P.Pagado,0) AS Saldo
    FROM dbo.Reserva R
    JOIN dbo.Cliente C ON C.IdCliente=R.IdCliente
    JOIN dbo.Habitacion H ON H.IdHabitacion=R.IdHabitacion
    OUTER APPLY (SELECT SUM(Monto) Pagado FROM dbo.Pago WHERE IdReserva=R.IdReserva) P
    WHERE R.FechaEntrada BETWEEN @FechaInicio AND @FechaFin
    ORDER BY R.FechaEntrada DESC;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_ReportePagosPorMetodo
    @FechaInicio DATE,
    @FechaFin DATE
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Metodo, COUNT(*) AS Cantidad, SUM(Monto) AS Total
    FROM dbo.Pago
    WHERE CAST(FechaPago AS DATE) BETWEEN @FechaInicio AND @FechaFin
    GROUP BY Metodo ORDER BY Total DESC;
END
GO

/* Datos iniciales: solo se agregan cuando aún no hay reservas. */
IF NOT EXISTS (SELECT 1 FROM dbo.Reserva)
BEGIN
    DECLARE @C1 INT=(SELECT MIN(IdCliente) FROM dbo.Cliente);
    DECLARE @C2 INT=(SELECT MIN(IdCliente)+1 FROM dbo.Cliente);
    DECLARE @C3 INT=(SELECT MIN(IdCliente)+2 FROM dbo.Cliente);
    DECLARE @H1 INT=(SELECT IdHabitacion FROM dbo.Habitacion WHERE Numero='101');
    DECLARE @H2 INT=(SELECT IdHabitacion FROM dbo.Habitacion WHERE Numero='102');
    DECLARE @H3 INT=(SELECT IdHabitacion FROM dbo.Habitacion WHERE Numero='103');
    DECLARE @H4 INT=(SELECT IdHabitacion FROM dbo.Habitacion WHERE Numero='201');
    DECLARE @H5 INT=(SELECT IdHabitacion FROM dbo.Habitacion WHERE Numero='202');
    DECLARE @H6 INT=(SELECT IdHabitacion FROM dbo.Habitacion WHERE Numero='203');

    INSERT dbo.Reserva(IdCliente,IdHabitacion,FechaEntrada,FechaSalida,CantidadDias,PrecioDia,Total,Estado,FechaCheckInReal)
    VALUES
    (@C1,@H1,CAST(GETDATE() AS DATE),DATEADD(DAY,2,CAST(GETDATE() AS DATE)),2,60,120,'Confirmada',NULL),
    (@C2,@H2,DATEADD(DAY,-1,CAST(GETDATE() AS DATE)),CAST(GETDATE() AS DATE),1,60,60,'Hospedado',DATEADD(DAY,-1,SYSDATETIME())),
    (@C3,@H3,DATEADD(DAY,-5,CAST(GETDATE() AS DATE)),DATEADD(DAY,-3,CAST(GETDATE() AS DATE)),2,90,180,'Finalizada',DATEADD(DAY,-5,SYSDATETIME())),
    (@C1,@H4,DATEADD(DAY,-7,CAST(GETDATE() AS DATE)),DATEADD(DAY,-5,CAST(GETDATE() AS DATE)),2,90,180,'Cancelada',NULL),
    (@C2,@H5,DATEADD(DAY,1,CAST(GETDATE() AS DATE)),DATEADD(DAY,3,CAST(GETDATE() AS DATE)),2,120,240,'Confirmada',NULL),
    (@C3,@H6,CAST(GETDATE() AS DATE),DATEADD(DAY,3,CAST(GETDATE() AS DATE)),3,120,360,'Hospedado',SYSDATETIME());

    UPDATE dbo.Habitacion SET Estado='Ocupada' WHERE IdHabitacion IN (@H2,@H6);
    UPDATE dbo.Habitacion SET Estado='Limpieza' WHERE Numero='301';

    INSERT dbo.Pago(IdReserva,Monto,Metodo,Tipo,NumeroOperacion)
    SELECT IdReserva, CASE WHEN Estado='Finalizada' THEN Total ELSE Total/2 END,
           CASE WHEN IdReserva%2=0 THEN 'Yape' ELSE 'Efectivo' END,
           CASE WHEN Estado='Finalizada' THEN 'Pago final' ELSE 'Adelanto' END,
           CASE WHEN IdReserva%2=0 THEN CONCAT('OP-',IdReserva,'01') ELSE NULL END
    FROM dbo.Reserva WHERE Estado IN ('Confirmada','Hospedado','Finalizada');
END
GO

