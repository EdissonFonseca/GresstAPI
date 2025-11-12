-- Script para crear tabla RefreshToken en SQL Server
-- Ejecutar en la base de datos QA.Gresst

USE [QA.Gresst]
GO

-- Crear tabla RefreshToken
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RefreshToken]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[RefreshToken](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [IdUsuario] [bigint] NOT NULL,
        [Token] [nvarchar](500) NOT NULL,
        [JwtId] [nvarchar](500) NULL,
        [IsUsed] [bit] NOT NULL DEFAULT(0),
        [IsRevoked] [bit] NOT NULL DEFAULT(0),
        [CreatedDate] [datetime] NOT NULL DEFAULT(GETUTCDATE()),
        [ExpiryDate] [datetime] NOT NULL,
        CONSTRAINT [PK_RefreshToken] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_RefreshToken_Usuario] FOREIGN KEY([IdUsuario]) 
            REFERENCES [dbo].[Usuario] ([IdUsuario])
            ON DELETE CASCADE
    )
END
GO

-- Crear índices para mejor performance
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RefreshToken_Token')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_RefreshToken_Token] 
    ON [dbo].[RefreshToken] ([Token] ASC)
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RefreshToken_IdUsuario')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_RefreshToken_IdUsuario] 
    ON [dbo].[RefreshToken] ([IdUsuario] ASC)
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RefreshToken_JwtId')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_RefreshToken_JwtId] 
    ON [dbo].[RefreshToken] ([JwtId] ASC)
END
GO

PRINT 'Tabla RefreshToken creada exitosamente'
GO

