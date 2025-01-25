-- Holiday, Vacation, Ill
CREATE TABLE [dbo].[AbsenceType] (
    [Id]         INT            NOT NULL,
    [Name]       NVARCHAR (255) NOT NULL,
    [RowVersion] ROWVERSION     NOT NULL,
    CONSTRAINT [PK_AbsenceType_Id] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UX_AbsenceType_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);
