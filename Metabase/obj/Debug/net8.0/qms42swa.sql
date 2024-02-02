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

CREATE TABLE [Databases] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Databases] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Relations] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [DatabaseId] int NOT NULL,
    CONSTRAINT [PK_Relations] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Relations_Databases_DatabaseId] FOREIGN KEY ([DatabaseId]) REFERENCES [Databases] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Attributes] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Type] int NOT NULL,
    [RelationId] int NOT NULL,
    [NotNull] bit NULL,
    [Unique] bit NULL,
    [PrimaryKey] bit NULL,
    CONSTRAINT [PK_Attributes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Attributes_Relations_RelationId] FOREIGN KEY ([RelationId]) REFERENCES [Relations] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [ForeignKeyConstraints] (
    [Id] int NOT NULL IDENTITY,
    [ReferencedRelationId] int NOT NULL,
    [Name] nvarchar(max) NULL,
    [RelationId] int NOT NULL,
    CONSTRAINT [PK_ForeignKeyConstraints] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ForeignKeyConstraints_Relations_ReferencedRelationId] FOREIGN KEY ([ReferencedRelationId]) REFERENCES [Relations] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ForeignKeyConstraints_Relations_RelationId] FOREIGN KEY ([RelationId]) REFERENCES [Relations] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AttributeModelForeignKeyConstraint] (
    [ForeignKeyConstraintId] int NOT NULL,
    [ReferencingAttributesId] int NOT NULL,
    CONSTRAINT [PK_AttributeModelForeignKeyConstraint] PRIMARY KEY ([ForeignKeyConstraintId], [ReferencingAttributesId]),
    CONSTRAINT [FK_AttributeModelForeignKeyConstraint_Attributes_ReferencingAttributesId] FOREIGN KEY ([ReferencingAttributesId]) REFERENCES [Attributes] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AttributeModelForeignKeyConstraint_ForeignKeyConstraints_ForeignKeyConstraintId] FOREIGN KEY ([ForeignKeyConstraintId]) REFERENCES [ForeignKeyConstraints] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AttributeModelForeignKeyConstraint1] (
    [ForeignKeyConstraint1Id] int NOT NULL,
    [ReferencedAttributesId] int NOT NULL,
    CONSTRAINT [PK_AttributeModelForeignKeyConstraint1] PRIMARY KEY ([ForeignKeyConstraint1Id], [ReferencedAttributesId]),
    CONSTRAINT [FK_AttributeModelForeignKeyConstraint1_Attributes_ReferencedAttributesId] FOREIGN KEY ([ReferencedAttributesId]) REFERENCES [Attributes] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AttributeModelForeignKeyConstraint1_ForeignKeyConstraints_ForeignKeyConstraint1Id] FOREIGN KEY ([ForeignKeyConstraint1Id]) REFERENCES [ForeignKeyConstraints] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_AttributeModelForeignKeyConstraint_ReferencingAttributesId] ON [AttributeModelForeignKeyConstraint] ([ReferencingAttributesId]);
GO

CREATE INDEX [IX_AttributeModelForeignKeyConstraint1_ReferencedAttributesId] ON [AttributeModelForeignKeyConstraint1] ([ReferencedAttributesId]);
GO

CREATE INDEX [IX_Attributes_RelationId] ON [Attributes] ([RelationId]);
GO

CREATE INDEX [IX_ForeignKeyConstraints_ReferencedRelationId] ON [ForeignKeyConstraints] ([ReferencedRelationId]);
GO

CREATE INDEX [IX_ForeignKeyConstraints_RelationId] ON [ForeignKeyConstraints] ([RelationId]);
GO

CREATE INDEX [IX_Relations_DatabaseId] ON [Relations] ([DatabaseId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240131144618_MetaInit', N'8.0.1');
GO

COMMIT;
GO

