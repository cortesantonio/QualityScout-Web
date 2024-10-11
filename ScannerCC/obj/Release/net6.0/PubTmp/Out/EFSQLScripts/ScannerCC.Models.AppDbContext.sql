IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231204031511_InitialCreate')
BEGIN
    CREATE TABLE [Producto] (
        [idProducto] int NOT NULL IDENTITY,
        [CodigoBarra] nvarchar(max) NOT NULL,
        [Nombre] nvarchar(max) NOT NULL,
        [Cepa] nvarchar(max) NOT NULL,
        [PaisOrigen] nvarchar(max) NULL,
        [PaisDestino] nvarchar(max) NULL,
        [FechaCosecha] datetime2 NULL,
        [FechaProduccion] datetime2 NULL,
        [Capacidad] int NULL,
        [GradoAlcohol] real NULL,
        [Azucar] real NULL,
        [Sulfuroso] real NULL,
        [Densidad] real NULL,
        [TipoCapsula] nvarchar(max) NULL,
        [TipoEtiqueta] nvarchar(max) NULL,
        [ColorBotella] nvarchar(max) NULL,
        [Medalla] bit NULL,
        [ColorCapsula] nvarchar(max) NULL,
        [TipoCorcho] nvarchar(max) NULL,
        [TipoBotella] nvarchar(max) NULL,
        [AlturaBotella] real NOT NULL,
        [AnchoBotella] real NOT NULL,
        [MedidadEtiquetaABoquete] real NOT NULL,
        [MedidaEtiquetaABase] real NOT NULL,
        CONSTRAINT [PK_Producto] PRIMARY KEY ([idProducto])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231204031511_InitialCreate')
BEGIN
    CREATE TABLE [Rol] (
        [idRol] int NOT NULL IDENTITY,
        [Nombre] nvarchar(max) NULL,
        CONSTRAINT [PK_Rol] PRIMARY KEY ([idRol])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231204031511_InitialCreate')
BEGIN
    CREATE TABLE [Usuario] (
        [idUsuario] int NOT NULL IDENTITY,
        [Nombre] nvarchar(max) NULL,
        [Rut] nvarchar(max) NULL,
        [Email] nvarchar(max) NULL,
        [RolId] int NOT NULL,
        [PasswordHash] varbinary(max) NULL,
        [PasswordSalt] varbinary(max) NULL,
        CONSTRAINT [PK_Usuario] PRIMARY KEY ([idUsuario]),
        CONSTRAINT [FK_Usuario_Rol_RolId] FOREIGN KEY ([RolId]) REFERENCES [Rol] ([idRol]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231204031511_InitialCreate')
BEGIN
    CREATE TABLE [Escaneo] (
        [IdEscaneo] int NOT NULL IDENTITY,
        [EscaneoId] int NOT NULL,
        [ProductoId] int NOT NULL,
        [UsuarioId] int NOT NULL,
        [Fecha] datetime2 NOT NULL,
        CONSTRAINT [PK_Escaneo] PRIMARY KEY ([IdEscaneo]),
        CONSTRAINT [FK_Escaneo_Producto_ProductoId] FOREIGN KEY ([ProductoId]) REFERENCES [Producto] ([idProducto]) ON DELETE CASCADE,
        CONSTRAINT [FK_Escaneo_Usuario_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuario] ([idUsuario]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231204031511_InitialCreate')
BEGIN
    CREATE TABLE [UsuarioProducto] (
        [IdUsuarioProducto] nvarchar(450) NOT NULL,
        [ProductoId] int NOT NULL,
        [UsuarioId] int NOT NULL,
        [Fecha] datetime2 NOT NULL,
        [Hora] datetime2 NOT NULL,
        CONSTRAINT [PK_UsuarioProducto] PRIMARY KEY ([IdUsuarioProducto]),
        CONSTRAINT [FK_UsuarioProducto_Producto_ProductoId] FOREIGN KEY ([ProductoId]) REFERENCES [Producto] ([idProducto]) ON DELETE CASCADE,
        CONSTRAINT [FK_UsuarioProducto_Usuario_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuario] ([idUsuario]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231204031511_InitialCreate')
BEGIN
    CREATE INDEX [IX_Escaneo_ProductoId] ON [Escaneo] ([ProductoId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231204031511_InitialCreate')
BEGIN
    CREATE INDEX [IX_Escaneo_UsuarioId] ON [Escaneo] ([UsuarioId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231204031511_InitialCreate')
BEGIN
    CREATE INDEX [IX_Usuario_RolId] ON [Usuario] ([RolId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231204031511_InitialCreate')
BEGIN
    CREATE INDEX [IX_UsuarioProducto_ProductoId] ON [UsuarioProducto] ([ProductoId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231204031511_InitialCreate')
BEGIN
    CREATE INDEX [IX_UsuarioProducto_UsuarioId] ON [UsuarioProducto] ([UsuarioId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231204031511_InitialCreate')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20231204031511_InitialCreate', N'7.0.13');
END;
GO

COMMIT;
GO

