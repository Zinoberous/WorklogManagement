CREATE TABLE [dbo].[AbsenceType] (
    [Id]   INT            IDENTITY (1, 1) NOT NULL,
    [Name] NVARCHAR (255) NOT NULL,
    CONSTRAINT [PK_AbsenceType_Id] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UX_AbsenceType_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);

