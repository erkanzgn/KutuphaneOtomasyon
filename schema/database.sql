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

CREATE TABLE [Books] (
    [Id] int NOT NULL IDENTITY,
    [ISBN] nvarchar(13) NOT NULL,
    [Title] nvarchar(200) NOT NULL,
    [Author] nvarchar(100) NOT NULL,
    [Publisher] nvarchar(100) NULL,
    [PublicationYear] int NULL,
    [Category] nvarchar(50) NULL,
    [PageCount] int NULL,
    [Language] nvarchar(30) NOT NULL DEFAULT N'Türkçe',
    [Description] nvarchar(1000) NULL,
    [ImageUrl] nvarchar(max) NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedDate] datetime2 NULL,
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    CONSTRAINT [PK_Books] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Members] (
    [Id] int NOT NULL IDENTITY,
    [MemberNumber] nvarchar(50) NOT NULL,
    [FirstName] nvarchar(50) NOT NULL,
    [LastName] nvarchar(50) NOT NULL,
    [Email] nvarchar(100) NOT NULL,
    [Phone] nvarchar(20) NOT NULL,
    [Address] nvarchar(250) NULL,
    [DateOfBirth] datetime2 NULL,
    [RegistrationDate] datetime2 NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [Notes] nvarchar(500) NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedDate] datetime2 NULL,
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    CONSTRAINT [PK_Members] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Copies] (
    [Id] int NOT NULL IDENTITY,
    [BookId] int NOT NULL,
    [CopyNumber] nvarchar(20) NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [ShelfLocation] nvarchar(50) NULL,
    [AcquisitionDate] datetime2 NOT NULL,
    [Price] decimal(18,2) NULL,
    [Condition] nvarchar(500) NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedDate] datetime2 NULL,
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    CONSTRAINT [PK_Copies] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Copies_Books_BookId] FOREIGN KEY ([BookId]) REFERENCES [Books] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Users] (
    [Id] int NOT NULL IDENTITY,
    [Username] nvarchar(50) NOT NULL,
    [Email] nvarchar(100) NOT NULL,
    [PasswordHash] nvarchar(255) NOT NULL,
    [FirstName] nvarchar(50) NOT NULL,
    [LastName] nvarchar(50) NOT NULL,
    [Role] nvarchar(20) NOT NULL,
    [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
    [LastLoginDate] datetime2 NULL,
    [MemberId] int NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedDate] datetime2 NULL,
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Users_Members_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [Members] ([Id]) ON DELETE SET NULL
);
GO

CREATE TABLE [Loans] (
    [Id] int NOT NULL IDENTITY,
    [CopyId] int NOT NULL,
    [MemberId] int NOT NULL,
    [LoanDate] datetime2 NOT NULL,
    [DueDate] datetime2 NOT NULL,
    [ReturnDate] datetime2 NULL,
    [Status] nvarchar(20) NOT NULL,
    [Notes] nvarchar(500) NULL,
    [LoanedByUserId] int NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedDate] datetime2 NULL,
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    CONSTRAINT [PK_Loans] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Loans_Copies_CopyId] FOREIGN KEY ([CopyId]) REFERENCES [Copies] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Loans_Members_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [Members] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Loans_Users_LoanedByUserId] FOREIGN KEY ([LoanedByUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);
GO

CREATE UNIQUE INDEX [IX_Books_ISBN] ON [Books] ([ISBN]) WHERE [IsDeleted] = 0;
GO

CREATE UNIQUE INDEX [IX_Copies_BookId_CopyNumber] ON [Copies] ([BookId], [CopyNumber]) WHERE [IsDeleted] = 0;
GO

CREATE INDEX [IX_Copies_Status] ON [Copies] ([Status]);
GO

CREATE INDEX [IX_Loans_CopyId_Status] ON [Loans] ([CopyId], [Status]);
GO

CREATE INDEX [IX_Loans_DueDate] ON [Loans] ([DueDate]);
GO

CREATE INDEX [IX_Loans_LoanedByUserId] ON [Loans] ([LoanedByUserId]);
GO

CREATE INDEX [IX_Loans_MemberId_Status] ON [Loans] ([MemberId], [Status]);
GO

CREATE UNIQUE INDEX [IX_Members_Email] ON [Members] ([Email]) WHERE [IsDeleted] = 0 AND [Email] IS NOT NULL;
GO

CREATE UNIQUE INDEX [IX_Members_MemberNumber] ON [Members] ([MemberNumber]) WHERE [IsDeleted] = 0;
GO

CREATE INDEX [IX_Members_Phone] ON [Members] ([Phone]);
GO

CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]) WHERE [IsDeleted] = 0;
GO

CREATE INDEX [IX_Users_MemberId] ON [Users] ([MemberId]);
GO

CREATE UNIQUE INDEX [IX_Users_Username] ON [Users] ([Username]) WHERE [IsDeleted] = 0;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251129163915_mig1', N'8.0.22');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Users] DROP CONSTRAINT [FK_Users_Members_MemberId];
GO

ALTER TABLE [Users] ADD CONSTRAINT [FK_Users_Members_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [Members] ([Id]) ON DELETE CASCADE;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251129171412_mig2', N'8.0.22');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251202100851_mig3', N'8.0.22');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251203093513_mig4Duzenlemeler', N'8.0.22');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [ContactMessages] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [Subject] nvarchar(max) NOT NULL,
    [MessageType] int NOT NULL,
    [Message] nvarchar(max) NOT NULL,
    [IsRead] bit NOT NULL,
    [AdminReplay] nvarchar(max) NULL,
    [ReplayDate] datetime2 NULL,
    [UserId] int NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedDate] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_ContactMessages] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ContactMessages_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id])
);
GO

CREATE INDEX [IX_ContactMessages_UserId] ON [ContactMessages] ([UserId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251206132434_mig5', N'8.0.22');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Members] ADD [BanExpirationDate] datetime2 NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251209100334_mig6BanSystem', N'8.0.22');
GO

COMMIT;
GO

