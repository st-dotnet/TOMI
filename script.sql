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

CREATE TABLE [Categories] (
    [Id] int NOT NULL IDENTITY,
    [Division] nvarchar(max) NULL,
    [DivisionName] nvarchar(max) NULL,
    [Category] nvarchar(max) NULL,
    [CategoryName] nvarchar(max) NULL,
    [CustomerId] uniqueidentifier NULL,
    [StoreId] uniqueidentifier NULL,
    [StockDate] datetime2 NULL,
    [CreatedAt] datetimeoffset NULL,
    [UpdatedAt] datetimeoffset NULL,
    [DeletedAt] datetimeoffset NULL,
    [CreatedBy] uniqueidentifier NULL,
    [UpdatedBy] uniqueidentifier NULL,
    [Deletedby] uniqueidentifier NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_Categories] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Customers] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(max) NULL,
    [CreatedAt] datetimeoffset NULL,
    [UpdatedAt] datetimeoffset NULL,
    [DeletedAt] datetimeoffset NULL,
    [CreatedBy] uniqueidentifier NULL,
    [UpdatedBy] uniqueidentifier NULL,
    [Deletedby] uniqueidentifier NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_Customers] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Departments] (
    [Id] uniqueidentifier NOT NULL,
    [Division] nvarchar(max) NULL,
    [DivisionName] nvarchar(max) NULL,
    [Department] nvarchar(max) NULL,
    [DepartmentName] nvarchar(max) NULL,
    [CustomerId] uniqueidentifier NULL,
    [StoreId] uniqueidentifier NULL,
    [StockDate] datetimeoffset NULL,
    [CreatedAt] datetimeoffset NULL,
    [UpdatedAt] datetimeoffset NULL,
    [DeletedAt] datetimeoffset NULL,
    [CreatedBy] uniqueidentifier NULL,
    [UpdatedBy] uniqueidentifier NULL,
    [Deletedby] uniqueidentifier NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_Departments] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [DwnErrors] (
    [id] int NOT NULL IDENTITY,
    [Tag] int NOT NULL,
    [ErrorMessage] nvarchar(max) NULL,
    [Term] nvarchar(max) NULL,
    [EmpNo] nvarchar(max) NULL,
    [Lines] int NOT NULL,
    [Qty] int NOT NULL,
    [ExtPrice] float NOT NULL,
    [CreatedAt] datetimeoffset NULL,
    [UpdatedAt] datetimeoffset NULL,
    [DeletedAt] datetimeoffset NULL,
    [CreatedBy] uniqueidentifier NULL,
    [UpdatedBy] uniqueidentifier NULL,
    [Deletedby] uniqueidentifier NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_DwnErrors] PRIMARY KEY ([id])
);
GO

CREATE TABLE [Employee] (
    [EmpNumber] nvarchar(450) NOT NULL,
    [EmpName] nvarchar(max) NULL,
    [LastName] nvarchar(max) NULL,
    [Postion] nvarchar(max) NULL,
    [inventory_key] nvarchar(max) NULL,
    CONSTRAINT [PK_Employee] PRIMARY KEY ([EmpNumber])
);
GO

CREATE TABLE [FileStore] (
    [Id] int NOT NULL IDENTITY,
    [Header] nvarchar(max) NULL,
    [FileName] nvarchar(max) NULL,
    [FileDate] nvarchar(max) NULL,
    [StoreNumber] nvarchar(max) NULL,
    [Category] nvarchar(max) NULL,
    [RecordCount] nvarchar(max) NULL,
    [Status] nvarchar(max) NULL,
    [CreatedAt] datetimeoffset NULL,
    [UpdatedAt] datetimeoffset NULL,
    [DeletedAt] datetimeoffset NULL,
    [CreatedBy] uniqueidentifier NULL,
    [UpdatedBy] uniqueidentifier NULL,
    [Deletedby] uniqueidentifier NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_FileStore] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Group] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(max) NULL,
    [CreatedAt] datetimeoffset NULL,
    [UpdatedAt] datetimeoffset NULL,
    [DeletedAt] datetimeoffset NULL,
    [CreatedBy] uniqueidentifier NULL,
    [UpdatedBy] uniqueidentifier NULL,
    [Deletedby] uniqueidentifier NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_Group] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [InfoLoad] (
    [Id] int NOT NULL IDENTITY,
    [Terminal] nvarchar(max) NULL,
    [Send] int NOT NULL,
    [Emp] nvarchar(max) NULL,
    [Downloaded] datetime2 NOT NULL,
    [Lines] int NOT NULL,
    [Qty] int NOT NULL,
    [ExtPrice] float NOT NULL,
    [DownloadedErrors] bit NOT NULL,
    [CreatedAt] datetimeoffset NULL,
    [UpdatedAt] datetimeoffset NULL,
    [DeletedAt] datetimeoffset NULL,
    [CreatedBy] uniqueidentifier NULL,
    [UpdatedBy] uniqueidentifier NULL,
    [Deletedby] uniqueidentifier NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_InfoLoad] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [MF2] (
    [Department] nvarchar(450) NOT NULL,
    [Id] uniqueidentifier NOT NULL,
    [creation_time] datetimeoffset NOT NULL,
    [CreatedAt] datetimeoffset NULL,
    [UpdatedAt] datetimeoffset NULL,
    [DeletedAt] datetimeoffset NULL,
    [CreatedBy] uniqueidentifier NULL,
    [UpdatedBy] uniqueidentifier NULL,
    [Deletedby] uniqueidentifier NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_MF2] PRIMARY KEY ([Department])
);
GO

CREATE TABLE [OrderJob] (
    [Id] uniqueidentifier NOT NULL,
    [SKU] nvarchar(max) NULL,
    [Code] nvarchar(max) NULL,
    [Department] nvarchar(max) NULL,
    [Description] nvarchar(max) NULL,
    [SalePrice] nvarchar(max) NULL,
    [PriceWithoutTaxes] nvarchar(max) NULL,
    [Store] nvarchar(max) NULL,
    [Category] nvarchar(max) NULL,
    [CustomerId] uniqueidentifier NULL,
    [StoreId] uniqueidentifier NULL,
    [StockDate] datetimeoffset NULL,
    [CreatedAt] datetimeoffset NULL,
    [UpdatedAt] datetimeoffset NULL,
    [DeletedAt] datetimeoffset NULL,
    [CreatedBy] uniqueidentifier NULL,
    [UpdatedBy] uniqueidentifier NULL,
    [Deletedby] uniqueidentifier NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_OrderJob] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [ParametersByDepartment] (
    [Id] uniqueidentifier NOT NULL,
    [Department] nvarchar(max) NULL,
    [Quantity] nvarchar(max) NULL,
    [Pesos] nvarchar(max) NULL,
    [PercentageInPieces] nvarchar(max) NULL,
    [CustomerId] uniqueidentifier NULL,
    [StoreId] uniqueidentifier NULL,
    [StockDate] datetime2 NULL,
    [CreatedAt] datetimeoffset NULL,
    [UpdatedAt] datetimeoffset NULL,
    [DeletedAt] datetimeoffset NULL,
    [CreatedBy] uniqueidentifier NULL,
    [UpdatedBy] uniqueidentifier NULL,
    [Deletedby] uniqueidentifier NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_ParametersByDepartment] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Reserved] (
    [Id] uniqueidentifier NOT NULL,
    [Store] nvarchar(max) NULL,
    [Code] nvarchar(max) NULL,
    [Quantity] nvarchar(max) NULL,
    [Filler] nvarchar(max) NULL,
    [CustomerId] uniqueidentifier NULL,
    [StoreId] uniqueidentifier NULL,
    [StockDate] datetimeoffset NULL,
    [CreatedAt] datetimeoffset NULL,
    [UpdatedAt] datetimeoffset NULL,
    [DeletedAt] datetimeoffset NULL,
    [CreatedBy] uniqueidentifier NULL,
    [UpdatedBy] uniqueidentifier NULL,
    [Deletedby] uniqueidentifier NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_Reserved] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Stock] (
    [Id] uniqueidentifier NOT NULL,
    [Store] nvarchar(max) NULL,
    [SKU] nvarchar(max) NULL,
    [Departament] nvarchar(max) NULL,
    [Description] nvarchar(max) NULL,
    [PrecVtaNorm] nvarchar(max) NULL,
    [PrecVtaNorm_SImpto] nvarchar(max) NULL,
    [SOH] nvarchar(max) NULL,
    [Category] nvarchar(max) NULL,
    [CustomerId] uniqueidentifier NULL,
    [StoreId] uniqueidentifier NULL,
    [StockDate] datetimeoffset NULL,
    [CreatedAt] datetimeoffset NULL,
    [UpdatedAt] datetimeoffset NULL,
    [DeletedAt] datetimeoffset NULL,
    [CreatedBy] uniqueidentifier NULL,
    [UpdatedBy] uniqueidentifier NULL,
    [Deletedby] uniqueidentifier NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_Stock] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [UploadFileName] (
    [Id] int NOT NULL IDENTITY,
    [Header] nvarchar(max) NULL,
    [FileName] nvarchar(max) NULL,
    [FileDate] nvarchar(max) NULL,
    [StoreNumber] nvarchar(max) NULL,
    [Category] nvarchar(max) NULL,
    [RecordCount] nvarchar(max) NULL,
    CONSTRAINT [PK_UploadFileName] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Stores] (
    [Id] uniqueidentifier NOT NULL,
    [CustomerId] uniqueidentifier NOT NULL,
    [Name] nvarchar(max) NULL,
    [CreatedAt] datetimeoffset NULL,
    [UpdatedAt] datetimeoffset NULL,
    [DeletedAt] datetimeoffset NULL,
    [CreatedBy] uniqueidentifier NULL,
    [UpdatedBy] uniqueidentifier NULL,
    [Deletedby] uniqueidentifier NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_Stores] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Stores_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Ranges] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(max) NULL,
    [GroupId] uniqueidentifier NULL,
    [TagFrom] int NULL,
    [TagTo] int NULL,
    [CustomerId] uniqueidentifier NULL,
    [StoreId] uniqueidentifier NULL,
    [StockDate] datetimeoffset NULL,
    [IsDeleted] bit NOT NULL,
    [CreatedAt] datetimeoffset NULL,
    [UpdatedAt] datetimeoffset NULL,
    [DeletedAt] datetimeoffset NULL,
    [CreatedBy] uniqueidentifier NULL,
    [UpdatedBy] uniqueidentifier NULL,
    [Deletedby] uniqueidentifier NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_Ranges] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Ranges_Group_GroupId] FOREIGN KEY ([GroupId]) REFERENCES [Group] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [StockAdjustment] (
    [Id] uniqueidentifier NOT NULL,
    [Empno] int NULL,
    [Rec] int NULL,
    [Term] nvarchar(max) NULL,
    [Dload] int NULL,
    [Tag] int NULL,
    [Shelf] int NULL,
    [Barcode] nvarchar(max) NULL,
    [SKU] uniqueidentifier NOT NULL,
    [NOF] tinyint NOT NULL,
    [Department] int NULL,
    [Quantity] int NULL,
    [Isdeleted] bit NOT NULL,
    [CreatedAt] datetimeoffset NULL,
    [UpdatedAt] datetimeoffset NULL,
    [DeletedAt] datetimeoffset NULL,
    [CreatedBy] uniqueidentifier NULL,
    [UpdatedBy] uniqueidentifier NULL,
    [Deletedby] uniqueidentifier NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_StockAdjustment] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_StockAdjustment_OrderJob_SKU] FOREIGN KEY ([SKU]) REFERENCES [OrderJob] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [MF1] (
    [Id] uniqueidentifier NOT NULL,
    [CustomerId] uniqueidentifier NOT NULL,
    [Terminal] nvarchar(max) NULL,
    [StoreId] uniqueidentifier NOT NULL,
    [Employee_Number] nvarchar(max) NULL,
    [Inventory_Date] datetimeoffset NULL,
    [Department] nvarchar(450) NULL,
    [Code] nvarchar(max) NULL,
    [Sale_Price] decimal(18,2) NOT NULL,
    [tag] int NOT NULL,
    [shelf] int NOT NULL,
    [operation] int NOT NULL,
    [creation_time] datetime2 NOT NULL,
    [inventory_key] nvarchar(max) NULL,
    [sync_to_terminal_status] bit NOT NULL,
    [sync_to_terminal_time] datetime2 NOT NULL,
    [sync_back_from_terminal_status] bit NOT NULL,
    [sync_back_from_terminal_time] datetime2 NOT NULL,
    [count_type] int NOT NULL,
    [total_counted] int NOT NULL,
    [count_time] datetime2 NOT NULL,
    [nof] bit NOT NULL,
    [counted_status] bit NOT NULL,
    [CreatedAt] datetimeoffset NULL,
    [UpdatedAt] datetimeoffset NULL,
    [DeletedAt] datetimeoffset NULL,
    [CreatedBy] uniqueidentifier NULL,
    [UpdatedBy] uniqueidentifier NULL,
    [Deletedby] uniqueidentifier NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_MF1] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_MF1_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([Id]),
    CONSTRAINT [FK_MF1_MF2_Department] FOREIGN KEY ([Department]) REFERENCES [MF2] ([Department]) ON DELETE NO ACTION,
    CONSTRAINT [FK_MF1_Stores_StoreId] FOREIGN KEY ([StoreId]) REFERENCES [Stores] ([Id])
);
GO

CREATE TABLE [Users] (
    [Id] uniqueidentifier NOT NULL,
    [CustomerId] uniqueidentifier NOT NULL,
    [StoreId] uniqueidentifier NULL,
    [Email] nvarchar(max) NULL,
    [Password] nvarchar(max) NULL,
    [FirstName] nvarchar(max) NULL,
    [LastName] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [Role] nvarchar(max) NULL,
    [EmployeeNumber] nvarchar(max) NULL,
    [CreatedAt] datetimeoffset NULL,
    [UpdatedAt] datetimeoffset NULL,
    [DeletedAt] datetimeoffset NULL,
    [CreatedBy] uniqueidentifier NULL,
    [UpdatedBy] uniqueidentifier NULL,
    [Deletedby] uniqueidentifier NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Users_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Users_Stores_StoreId] FOREIGN KEY ([StoreId]) REFERENCES [Stores] ([Id])
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'CreatedBy', N'DeletedAt', N'Deletedby', N'IsActive', N'Name', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[Customers]'))
    SET IDENTITY_INSERT [Customers] ON;
INSERT INTO [Customers] ([Id], [CreatedAt], [CreatedBy], [DeletedAt], [Deletedby], [IsActive], [Name], [UpdatedAt], [UpdatedBy])
VALUES ('b74ddd14-6340-4840-95c2-db12554843e5', NULL, NULL, NULL, NULL, CAST(0 AS bit), N'Test', NULL, NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'CreatedBy', N'DeletedAt', N'Deletedby', N'IsActive', N'Name', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[Customers]'))
    SET IDENTITY_INSERT [Customers] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'CreatedBy', N'CustomerId', N'DeletedAt', N'Deletedby', N'Email', N'EmployeeNumber', N'FirstName', N'IsActive', N'LastName', N'Password', N'PhoneNumber', N'Role', N'StoreId', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[Users]'))
    SET IDENTITY_INSERT [Users] ON;
INSERT INTO [Users] ([Id], [CreatedAt], [CreatedBy], [CustomerId], [DeletedAt], [Deletedby], [Email], [EmployeeNumber], [FirstName], [IsActive], [LastName], [Password], [PhoneNumber], [Role], [StoreId], [UpdatedAt], [UpdatedBy])
VALUES ('b74ddd14-6340-4840-95c2-db12554843e5', NULL, NULL, 'b74ddd14-6340-4840-95c2-db12554843e5', NULL, NULL, N'admin@gmail.com', N'987654', N'Admin', CAST(0 AS bit), N'Admin', N'AQAAAAEAACcQAAAAEP3bZixtlqGOACN9/SurGYIeDNtYPZrE3SWkNR330ll55rraHYPSEi0D7kfhE/k/lw==', N'1234567890', N'SuperAdmin', NULL, NULL, NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'CreatedBy', N'CustomerId', N'DeletedAt', N'Deletedby', N'Email', N'EmployeeNumber', N'FirstName', N'IsActive', N'LastName', N'Password', N'PhoneNumber', N'Role', N'StoreId', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[Users]'))
    SET IDENTITY_INSERT [Users] OFF;
GO

CREATE INDEX [IX_MF1_CustomerId] ON [MF1] ([CustomerId]);
GO

CREATE INDEX [IX_MF1_Department] ON [MF1] ([Department]);
GO

CREATE INDEX [IX_MF1_StoreId] ON [MF1] ([StoreId]);
GO

CREATE INDEX [IX_Ranges_GroupId] ON [Ranges] ([GroupId]);
GO

CREATE INDEX [IX_StockAdjustment_SKU] ON [StockAdjustment] ([SKU]);
GO

CREATE INDEX [IX_Stores_CustomerId] ON [Stores] ([CustomerId]);
GO

CREATE INDEX [IX_Users_CustomerId] ON [Users] ([CustomerId]);
GO

CREATE UNIQUE INDEX [IX_Users_StoreId] ON [Users] ([StoreId]) WHERE [StoreId] IS NOT NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20220227124442_Tomi_Intial', N'5.0.13');
GO

COMMIT;
GO

