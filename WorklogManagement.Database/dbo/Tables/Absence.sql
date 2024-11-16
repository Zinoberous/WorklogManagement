CREATE TABLE [dbo].[Absence] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [AbsenceTypeId]   INT            NOT NULL,
    [Date]            DATE           NOT NULL,
    [DurationMinutes] INT            NOT NULL,
    [Note]            NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Absence_Id] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Absence_AbsenceTypeId_AbsenceType_Id] FOREIGN KEY ([AbsenceTypeId]) REFERENCES [dbo].[AbsenceType] ([Id])
);

