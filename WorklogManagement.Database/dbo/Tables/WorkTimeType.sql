-- Office, Mobile
CREATE TABLE [dbo].[WorkTimeType] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [Name]       NVARCHAR (255) NOT NULL,
    [RowVersion] ROWVERSION     NOT NULL,
    CONSTRAINT [PK_WorkTimeType_Id] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UX_WorkTimeType_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);

